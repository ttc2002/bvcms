﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using UtilityExtensions;
using System.Data.Linq.SqlClient;
using CMSPresenter;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace CMSWeb.Models
{
    [Serializable]
    public class OnlineRegModel
    {
        [NonSerialized]
        private CmsData.Division _div;
        public CmsData.Division div
        {
            get
            {
                if (_div == null && divid.HasValue)
                    _div = DbUtil.Db.Divisions.SingleOrDefault(d => d.Id == divid);
                return _div;
            }
        }

        [NonSerialized]
        private CmsData.Organization _org;
        public CmsData.Organization org
        {
            get
            {
                if (_org == null && orgid.HasValue)
                    _org = DbUtil.Db.LoadOrganizationById(orgid.Value);
                return _org;
            }
        }

        public int? divid { get; set; }
        public int? orgid { get; set; }

        public bool? testing { get; set; }
        public string qtesting
        {
            get { return testing == true ? "?testing=true" : ""; }
        }
        public bool IsEnded()
        {
            if (div != null)
                return div.Organizations.Any(o => o.ClassFilled == true);
            else
                return org.ClassFilled == true;
        }
        public bool OnlyOneAllowed()
        {
            if (org != null)
                return org.AllowOnlyOne == true || org.AskTickets == true;
            var q = from o in DbUtil.Db.Organizations
                    where o.DivisionId == divid
                    where o.AllowOnlyOne == true || o.AskTickets == true
                    select o;
            return q.Count() > 0;
        }
        public string Header
        {
            get
            {
                if (div != null)
                    return div.Name;
                else
                    return org.OrganizationName;
            }
        }
        public string Instructions
        {
            get
            {
                if (org != null)
                    return Util.PickFirst(org.Instructions, div != null ? div.Instructions : "");
                return div.Instructions;
            }
        }

        private IList<OnlineRegPersonModel> list = new List<OnlineRegPersonModel>();
        public IList<OnlineRegPersonModel> List
        {
            get { return list; }
            set { list = value; }
        }

        public decimal TotalAmount()
        {
            var amt = List.Sum(p => p.Amount());
            if (org == null)
                return amt;
            if (org.MaximumFee > 0 && amt > org.MaximumFee)
                amt = org.MaximumFee.Value;
            return amt;
        }
        public decimal TotalAmountDue()
        {
            return List.Sum(p => p.AmountDue());
        }
        public string NameOnAccount
        {
            get
            {
                var p = List[0];
                if (p.org.AskParents == true)
                    return p.fname.HasValue() ? p.fname : p.mname;
                return p.first + " " + p.last;
            }
        }
        private CmsData.Meeting _meeting;
        public CmsData.Meeting meeting()
        {
            if (_meeting == null)
            {
                var q = from m in DbUtil.Db.Meetings
                        where m.Organization.OrganizationId == orgid
                        where m.MeetingDate > Util.Now.AddHours(-12)
                        orderby m.MeetingDate
                        select m;
                _meeting = q.FirstOrDefault();
            }
            return _meeting;
        }
        public string MeetingTime
        {
            get { return meeting().MeetingDate.ToString2("ddd, MMM d h:mm tt"); }
        }
        public List<SelectListItem> ShirtSizes()
        {
            var q = from ss in DbUtil.Db.ShirtSizes
                    orderby ss.Id
                    select new SelectListItem
                    {
                        Value = ss.Code,
                        Text = ss.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            if (org != null && org.AllowLastYearShirt == true)
                list.Add(new SelectListItem { Value = "lastyear", Text = "Use shirt from last year" });
            return list;
        }

        public void EnrollAndConfirm(string TransactionID)
        {
            var smtp = Util.Smtp();
            var elist = new List<string>();
            var participants = new StringBuilder();
            var pids = new List<TransactionInfo.PeopleInfo>();
            var p0 = List[0];
            for (var i = 0; i < List.Count; i++)
            {
                var p = List[i];
                if (p.IsNew)
                    p.AddPerson(i == 0 ? null : p0.person, org.EntryPointId ?? 0);

                if (!elist.Contains(p.email))
                    elist.Add(p.email);

                if (!p.IsNew)
                {
                    if (p.person.EmailAddress.HasValue() && !elist.Contains(p.person.EmailAddress))
                        elist.Add(p.person.EmailAddress);
                    if (string.Compare(p.email, p.person.EmailAddress, true) != 0)
                        Util.Email2(smtp, p.person.EmailAddress, org.EmailAddresses, "different email address than one on record",
                                "<p>{0}({1}) registered  with '{2}' but has '{3}' in record.</p>".Fmt(
                                p.person.Name, p.person.PeopleId, p.email, p.person.EmailAddress));
                }
                participants.Append(p.ToString());
                pids.Add(new TransactionInfo.PeopleInfo
                {
                    name = p.person.Name,
                    pid = p.person.PeopleId,
                    amt = p.Amount() + p.AmountDue()
                });
            }
            var emails = string.Join(",", elist.ToArray());
            string paylink = string.Empty;
            var amtdue = TotalAmountDue();
            var amtpaid = TotalAmount();

            if (amtdue > 0)
            {
                var ti = new TransactionInfo
                {
                    Header = Header,
                    Name = NameOnAccount,
                    Address = p0.address,
                    City = p0.city,
                    State = p0.state,
                    Zip = p0.zip,
                    Phone = p0.phone.FmtFone(),
                    testing = testing ?? false,
                    AmountDue = amtdue,
                    AmountPaid = amtpaid,
                    Email = emails,
                    Participants = participants.ToString(),
                    people = pids.ToArray(),
                    orgid = orgid.Value,
                };
                var td = DbUtil.Db.GetDatum<TransactionInfo>(ti);
                var estr = HttpContext.Current.Server.UrlEncode(Util.Encrypt(td.Id.ToString()));
                paylink = Util.ResolveServerUrl("/OnlineReg/PayDue?q=" + estr);
            }

            var details = new StringBuilder("<table>");
            for (var i = 0; i < List.Count; i++)
            {
                var p = List[i];
                p.Enroll(TransactionID, paylink, testing);
                details.AppendFormat(@"
<tr><td colspan='2'><hr/></td></tr>
<tr><th valign='top'>{0}</th><td>
{1}
</td></tr>", i + 1, p.PrepareSummaryText());

                if (!elist.Contains(p.email))
                    elist.Add(p.email);
                if (p.person.EmailAddress.HasValue())
                    if (!elist.Contains(p.person.EmailAddress))
                        elist.Add(p.person.EmailAddress);
                if (string.Compare(p.email, p.person.EmailAddress, true) != 0)
                    Util.Email2(smtp, p.person.EmailAddress, org.EmailAddresses, "different email address than one on record",
                            "<p>{0}({1}) registered  with '{2}' but has '{3}' in record.</p>".Fmt(
                            p.person.Name, p.person.PeopleId, p.email, p.person.EmailAddress));
            }
            details.Append("\n</table>\n");
            DbUtil.Db.SubmitChanges();

            var subject = Util.PickFirst(org.EmailSubject, org.Division.EmailSubject, "no subject");
            var message = Util.PickFirst(org.EmailMessage, org.Division.EmailMessage, "no message");
            message = message.Replace("{first}", p0.first);
            message = message.Replace("{tickets}", p0.ntickets.ToString());
            message = message.Replace("{division}", org.Division.Name);
            message = message.Replace("{org}", org.OrganizationName);
            message = message.Replace("{cmshost}", Util.CmsHost);
            message = message.Replace("{details}", details.ToString());
            message = message.Replace("{paid}", amtpaid.ToString("c"));
            message = message.Replace("{participants}", details.ToString());
            if (amtdue > 0)
                message = message.Replace("{paylink}", "<a href='{0}'>Click this link to pay balance of {1:C}</a>.".Fmt(paylink, amtdue));
            else
                message = message.Replace("{paylink}", "You have a zero balance.");

            Util.Email2(smtp, org.EmailAddresses, emails, subject, message);
            Util.Email2(smtp, emails, org.EmailAddresses,
                "{0} Registration".Fmt(Header),
                "{0} has registered {1} participant for {2}<br/>Feepaid: {3:C}<br/>AmountDue: {4:C}"
                .Fmt(NameOnAccount, List.Count, Header, amtpaid, amtdue));
        }
    }
    [Serializable]
    public class TransactionInfo
    {
        public string Header { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public string Participants { get; set; }
        public bool testing { get; set; }
        public int orgid { get; set; }
        public PeopleInfo[] people { get; set; }
        public class PeopleInfo
        {
            public int pid { get; set; }
            public string name { get; set; }
            public decimal amt { get; set; }
        }
    }

}