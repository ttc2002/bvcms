﻿/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Net.Mail;
using System.Threading;
using UtilityExtensions;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace CmsData
{
    public static class Emailer
    {
        public static void Email(SmtpClient smtp,  
            string from, Person p, string subject, string body)
        {
            Email(smtp, from, p, null, subject, body);
        }
        public static void Email(SmtpClient smtp,
            string from, Person p, string addemail, string subject, string body)
        {
            var From = new MailAddress(from);
            var addr = new List<string>();
            if (p.SendEmailAddress1 ?? true)
                addr.Add(p.EmailAddress);
            if (p.SendEmailAddress2 ?? false)
                addr.Add(p.EmailAddress2);
            if (addemail.HasValue())
                addr.Add(addemail);
            if (addr.Count>0)
                Util.SendMsg(smtp, Util.SysFromEmail, Util.CmsHost, From, subject, body, 
                    p.Name, string.Join(",", addr));
        }
        public static void Email(SmtpClient smtp, 
            string from, List<Person> list, string subject, string body)
        {
            foreach (var p in list)
                Email(smtp, from, p, subject, body);
        }
        public static void QueueEmail(CMSDataContext Db, string sysfrom, string cmshost, MailAddress From, string subject, string body, IEnumerable<int> pids)
        {
            var emailqueue = new EmailQueue
            {
                Queued = DateTime.Now,
                FromAddr = From.Address,
                FromName = From.DisplayName,
                Subject = subject,
                Body = body,
                QueuedBy = Util.UserPeopleId,
            };
            Db.EmailQueues.InsertOnSubmit(emailqueue);
            foreach(var pid in pids)
                emailqueue.EmailQueueTos.Add(new EmailQueueTo { PeopleId = pid, OrgId = DbUtil.Db.CurrentOrgId });
            Db.SubmitChanges();
            Db.QueueEmail(emailqueue.Id, Util.CmsHost, Util.Host);
        }
        public static void SendPeopleEmail(CMSDataContext Db, string SysFromEmail, string CmsHost, EmailQueue emailqueue)
        {
            var From = Util.FirstAddress(emailqueue.FromAddr, emailqueue.FromName);
            if (!emailqueue.Subject.HasValue() || !emailqueue.Body.HasValue())
            {
                Util.SendMsg(Util.Smtp(), SysFromEmail, CmsHost, From, "sent emails - error", "no subject or body, no emails sent", null, From.Address, emailqueue.Id);
                return;
            }

            var Message = emailqueue.Body;
            emailqueue.Started = DateTime.Now;
            Db.SubmitChanges();

            var sb = new StringBuilder("<pre>\r\n");
            SmtpClient smtp = null;
            var i = 0;

            var q = from To in Db.EmailQueueTos
                    where To.Id == emailqueue.Id
                    where To.Sent == null
                    orderby To.PeopleId
                    select To;
            foreach (var To in q)
            {
                var qp = (from p in Db.People
                          where p.PeopleId == To.PeopleId
                          select new 
                          { 
                              p.Name, 
                              p.PreferredName, 
                              p.EmailAddress, 
                              p.OccupationOther,
                              p.EmailAddress2,
                              Send1 = p.SendEmailAddress1 ?? true,
                              Send2 = p.SendEmailAddress2 ?? false,
                          }).Single();
                string text = emailqueue.Body;

                if (qp.Name.Contains("?") || qp.Name.ToLower().Contains("unknown"))
                    text = text.Replace("{name}", string.Empty);
                else
                    text = text.Replace("{name}", qp.Name);

                if (qp.PreferredName.Contains("?") || qp.PreferredName.ToLower() == "unknown")
                    text = text.Replace("{first}", string.Empty);
                else
                    text = text.Replace("{first}", qp.PreferredName);
                text = text.Replace("{occupation}", qp.OccupationOther);

            	var re = new Regex(@"\{votelink:(?<orgid>\d+),(?<sg>[^}]*)\}", RegexOptions.Singleline | RegexOptions.Multiline);
                var list = new Dictionary<string, OneTimeLink>();
                var ma = re.Match(text);
                while (ma.Success)
                {
                    var votelink = ma.Value;
                    var orgid = ma.Groups["orgid"].Value;
                    var smallgroup = ma.Groups["sg"].Value;
                    var qs = @"{0},{1}".Fmt(orgid, To.PeopleId);
                    OneTimeLink ot;
                    if (list.ContainsKey(qs))
                        ot = list[qs];
                    else 
                    {
                        ot = new OneTimeLink
                        {
                            Id = Guid.NewGuid(),
                            Querystring = qs
                        };
                        Db.OneTimeLinks.InsertOnSubmit(ot);
                        Db.SubmitChanges();
                        list.Add(qs, ot);
                    }
                    var url = Util.URLCombine(CmsHost, "/OnlineReg/VoteLink/{0}?smallgroup={1}".Fmt(ot.Id, smallgroup));
                    text = text.Replace(votelink, @"<a href=""{0}"">{1}</a>".Fmt(url, smallgroup));
                    ma = ma.NextMatch();
                }

                var aa = new List<string>();
                if (qp.Send1)
                    aa.AddRange(qp.EmailAddress.SplitStr(",;"));
                if (qp.Send2)
                    aa.AddRange(qp.EmailAddress2.SplitStr(",;"));
                if (To.OrgId.HasValue)
                {
                    var qm = (from m in Db.OrganizationMembers
                              where m.PeopleId == To.PeopleId && m.OrganizationId == To.OrgId
                              select new { m.PayLink, m.Amount, m.AmountPaid, m.RegisterEmail }).SingleOrDefault();
                    if (qm != null)
                    {
                        if (qm.PayLink.HasValue())
                            text = text.Replace("{paylink}", "<a href=\"{0}\">payment link</a>".Fmt(qm.PayLink));
                        text = text.Replace("{amtdue}", (qm.Amount - qm.AmountPaid).ToString2("c"));
                        if (qm.RegisterEmail.HasValue() && !aa.Contains(qm.RegisterEmail, StringComparer.OrdinalIgnoreCase))
                            aa.Add(qm.RegisterEmail);
                    }
                }

                foreach (var ad in aa)
                {
                    if (Util.ValidEmail(ad))
                    {
                        if (i % 20 == 0)
                            smtp = Util.Smtp();
                        i++;

                        var qs = "OptOut/UnSubscribe/?enc=" + Util.EncryptForUrl("{0}|{1}".Fmt(To.PeopleId, From.Address));
                        var url = Util.URLCombine(CmsHost, qs);
                        var link = @"<a href=""{0}"">Unsubscribe</a>".Fmt(url);
                        text = text.Replace("{unsubscribe}", link);
                        text = text.Replace("{Unsubscribe}", link);
                        text = text.Replace("{toemail}", ad);
                        text = text.Replace("%7Btoemail%7D", ad);
                        text = text.Replace("{fromemail}", From.Address);
                        text = text.Replace("%7Bfromemail%7D", From.Address);

                        if (Db.Setting("sendemail", "true") != "false")
                        {
                            Util.SendMsg(smtp, SysFromEmail, CmsHost, From, emailqueue.Subject, text, qp.Name, ad, emailqueue.Id);
                            To.Sent = DateTime.Now;

                            sb.AppendFormat("\"{0}\" [{1}] ({2})\r\n".Fmt(qp.Name, ad, To.PeopleId));
                            if (i % 500 == 0)
                                NotifySentEmails(Db, SysFromEmail, CmsHost, sb, smtp, From, emailqueue.Subject, emailqueue.Body, emailqueue.Id);
                            Db.SubmitChanges();
                        }
                    }
                }
            }
            if (smtp != null)
                NotifySentEmails(Db, SysFromEmail, CmsHost, sb, smtp, From, emailqueue.Subject, emailqueue.Body, emailqueue.Id);
            emailqueue.Sent = DateTime.Now;
            Db.SubmitChanges();
        }
        public static void SendPeopleEmail(CMSDataContext Db, string SysFromEmail, string CmsHost, MailAddress From, IEnumerable<Person> q, string Subject, string Message)
        {
            var emailqueue = new EmailQueue
            {
                Queued = DateTime.Now,
                FromAddr = From.Address,
                FromName = From.DisplayName,
                Subject = Subject,
                Body = Message,
                SendWhen = null,
                QueuedBy = Util.UserPeopleId,
            };
            Db.EmailQueues.InsertOnSubmit(emailqueue);
            Db.SubmitChanges();

            foreach (var p in q)
            {
                if (!Util.ValidEmail(p.EmailAddress))
                    continue;
                var to = new EmailQueueTo { Id = emailqueue.Id, PeopleId = p.PeopleId, OrgId = Db.CurrentOrgId };
                Db.EmailQueueTos.InsertOnSubmit(to);
            }
            Db.SubmitChanges();
            SendPeopleEmail(Db, SysFromEmail, CmsHost, emailqueue);
        }
        private static void NotifySentEmails(CMSDataContext Db, string SysFromEmail, string CmsHost, StringBuilder sb, SmtpClient smtp, MailAddress From, string subject, string body, int id)
        {
            sb.Append("</pre>\r\n<h2>{0}</h2>".Fmt(subject));
            sb.Append(body);
            if (Db.Setting("sendemail", "true") != "false")
            {
                string subj = "sent emails: " + subject;
                Util.SendMsg(smtp, SysFromEmail, CmsHost, From, subj, sb.ToString(), null, From.Address, id);
                Util.SendMsg(smtp, SysFromEmail, CmsHost, From, subj, sb.ToString(), null, ConfigurationManager.AppSettings["senderrorsto"], id);
            }
            sb.Length = 0;
            sb.Append("<pre>\r\n");
        }
    }
}

