using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CmsData.Registration;
using UtilityExtensions;
using System.Linq;
namespace CmsData
{
    public partial class Condition
    {
        public void Save(CMSDataContext Db, bool increment = false, string owner = null)
        {
            var q = (from e in Db.Queries
                     where e.QueryId == Id
                     select e).FirstOrDefault();

            if (q == null)
            {
                q = new Query
                {
                    QueryId = Id,
                    Owner = Util.UserName,
                    Created = DateTime.Now,
                    Ispublic = IsPublic,
                    Name = Description
                };
                Db.Queries.InsertOnSubmit(q);
            }
            q.LastRun = DateTime.Now;

            if (Description != q.Name)
            {
                var same = (from v in Db.Queries
                            where !v.Ispublic
                            where v.Owner == Util.UserName
                            where v.Name == Description
                            orderby v.LastRun descending
                            select v).FirstOrDefault();
                if (same != null)
                    same.Text = ToXml();
                else
                {
                    var c = Clone();
                    var cq = new Query
                    {
                        QueryId = c.Id,
                        Owner = Util.UserName,
                        Created = q.Created,
                        Ispublic = q.Ispublic,
                        Name = q.Name,
                        Text = c.ToXml(),
                        RunCount = q.RunCount,
                        CopiedFrom = q.CopiedFrom,
                        LastRun = q.LastRun
                    };
                    Db.Queries.InsertOnSubmit(cq);
                    q.LastRun = DateTime.Now;
                }
            }
            q.Name = Description;
            if(owner.HasValue())
                q.Owner = owner;
            q.Ispublic = IsPublic;
            if (increment)
                q.RunCount = q.RunCount + 1;
            q.Text = ToXml();
            Db.SubmitChanges();
        }

        public string ToXml(bool newGuids = false)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = new UTF8Encoding(false);
            var sb = new StringBuilder();
            using (var w = XmlWriter.Create(sb, settings))
                SendToWriter(w, newGuids);
            return sb.ToString();
        }
        public void SendToWriter(XmlWriter w, bool newGuids = false)
        {
            w.WriteStartElement("Condition");
            WriteAttributes(w, newGuids);
            foreach (var qc in Conditions)
                qc.SendToWriter(w, newGuids);
            w.WriteEndElement();
        }
        private void WriteAttributes(XmlWriter w, bool newGuids = false)
        {
            if (newGuids)
                w.WriteAttributeString("Id", Guid.NewGuid().ToString());
            else
                w.WriteAttributeString("Id", Id.ToString());

            w.WriteAttributeString("Order", Order.ToString());
            w.WriteAttributeString("Field", ConditionName);
            w.WriteAttributeString("Comparison", Comparison);
            if (Description.HasValue())
                w.WriteAttributeString("Description", Description);
            if (PreviousName.HasValue())
                w.WriteAttributeString("PreviousName", Description);
            if (TextValue.HasValue())
                w.WriteAttributeString("TextValue", TextValue);
            if (DateValue.HasValue)
                w.WriteAttributeString("DateValue", DateValue.ToString());
            if (CodeIdValue.HasValue())
                w.WriteAttributeString("CodeIdValue", CodeIdValue);
            if (StartDate.HasValue)
                w.WriteAttributeString("StartDate", StartDate.ToString());
            if (EndDate.HasValue)
                w.WriteAttributeString("EndDate", EndDate.ToString());
            if (Program > 0)
                w.WriteAttributeString("Program", Program.ToString());
            if (Division > 0)
                w.WriteAttributeString("Division", Division.ToString());
            if (Organization > 0)
                w.WriteAttributeString("Organization", Organization.ToString());
            if (OrgType > 0)
                w.WriteAttributeString("OrgType", OrgType.ToString());
            if (Days > 0)
                w.WriteAttributeString("Days", Days.ToString());
            if (Quarters.HasValue())
                w.WriteAttributeString("Quarters", Quarters);
            if (Tags.HasValue())
                w.WriteAttributeString("Tags", Tags);
            if (Schedule > 0)
                w.WriteAttributeString("Schedule", Schedule.ToString());
            if (ConditionName != "FamilyHasChildrenAged")
                Age = null;
            if (Age.HasValue)
                w.WriteAttributeString("Age", Age.ToString());
            if (SavedQuery.HasValue())
                w.WriteAttributeString("SavedQueryIdDesc", SavedQuery);
        }
        public static Condition Import(string text, string name = null, bool newGuids = false, Guid? topguid = null)
        {
            if (!text.HasValue())
                return CreateNewGroupClause(name);
            var x = XDocument.Parse(text);
            Debug.Assert(x.Root != null, "x.Root != null");
            var c = ImportClause(x.Root, null, newGuids, topguid);
            return c;
        }
        private static Condition ImportClause(XElement r, Condition p, bool newGuids, Guid? topguid)
        {
            var allClauses = p == null ? new Dictionary<Guid, Condition>() : p.AllConditions;
            Guid? parentGuid = null;
            if (p != null)
                parentGuid = p.Id;
            var c = new Condition
            {
                ParentId = parentGuid,
                Id = topguid ?? (newGuids ? Guid.NewGuid() : AttributeGuid(r, "Id")),
                Order = AttributeInt(r, "Order"),
                ConditionName = Attribute(r, "Field"),
                Comparison = Attribute(r, "Comparison"),
                TextValue = Attribute(r, "TextValue"),
                DateValue = AttributeDate(r, "DateValue"),
                CodeIdValue = Attribute(r, "CodeIdValue"),
                StartDate = AttributeDate(r, "StartDate"),
                EndDate = AttributeDate(r, "EndDate"),
                Program = Attribute(r, "Program").ToInt(),
                Division = Attribute(r, "Division").ToInt(),
                Organization = Attribute(r, "Organization").ToInt(),
                OrgType = Attribute(r, "OrgType").ToInt(),
                Days = Attribute(r, "Days").ToInt(),
                Quarters = Attribute(r, "Quarters"),
                Tags = Attribute(r, "Tags"),
                Schedule = Attribute(r, "Schedule").ToInt(),
                Age = Attribute(r, "Age").ToInt2(),
                Owner = Attribute(r, "Owner"),
                SavedQuery = Attribute(r, "SavedQueryIdDesc"),
                AllConditions = allClauses
            };
            if (c.ConditionName != "FamilyHasChildrenAged")
                c.Age = null;
            if (p == null)
            {
                c.Description = Attribute(r, "Description");
                c.PreviousName = Attribute(r, "PreviousName");
            }
            c.AllConditions.Add(c.Id, c);
            if (c.ConditionName == "Group")
                foreach (var rr in r.Elements())
                    ImportClause(rr, c, newGuids, null);
            return c;
        }
        private static string Attribute(XElement r, string attr, string def = null)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return def;
            return a.Value;
        }
        private static DateTime? AttributeDate(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return null;
            return a.Value.ToDate();
        }
        private static int AttributeInt(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return 0;
            return a.Value.ToInt();
        }
        private static Guid AttributeGuid(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return Guid.NewGuid();
            Guid g;
            return Guid.TryParse(a.Value, out g) ? g : Guid.NewGuid();
        }
    }
}