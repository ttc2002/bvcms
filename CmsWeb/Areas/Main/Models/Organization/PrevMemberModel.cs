﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CMSWeb.Models.OrganizationPage
{
    public class PrevMemberModel
    {
        private int OrganizationId;
        public PagerModel2 Pager { get; set; }
        public PrevMemberModel(int id)
        {
            OrganizationId = id;
            Pager = new PagerModel2(Count);
            Pager.Direction = "asc";
            Pager.Sort = "Name";
        }
        private IQueryable<EnrollmentTransaction> _enrollments;
        private IQueryable<EnrollmentTransaction> FetchPrevMembers()
        {
            if (_enrollments == null)
                _enrollments = from etd in DbUtil.Db.EnrollmentTransactions
                    let mdt = DbUtil.Db.EnrollmentTransactions.Where(m =>
                        m.PeopleId == etd.PeopleId
                        && m.OrganizationId == OrganizationId
                        && m.TransactionTypeId > 3
                        && m.TransactionStatus == false).Select(m => m.TransactionDate).Max()
                    where etd.TransactionStatus == false
                    where etd.TransactionDate == mdt
                    where etd.OrganizationId == OrganizationId
                    where etd.TransactionTypeId >= 4
                    where !etd.Person.OrganizationMembers.Any(om => om.OrganizationId == OrganizationId)
                    select etd;
            return _enrollments;
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchPrevMembers().Count();
            return _count.Value;
        }
        public IEnumerable<PersonMemberInfo> PrevMembers()
        {
            var q = ApplySort();
            q = q.Skip(Pager.StartRow).Take(Pager.PageSize);
            var q2 = from om in q
                     let p = om.Person
                     select new PersonMemberInfo
                     {
                         PeopleId = p.PeopleId,
                         Name = p.Name,
                         Name2 = p.Name2,
                         BirthDate = Util.FormatBirthday(
                             p.BirthYear,
                             p.BirthMonth,
                             p.BirthDay),
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                         EmailAddress = p.EmailAddress,
                         PhonePref = p.PhonePrefId,
                         HomePhone = p.HomePhone,
                         CellPhone = p.CellPhone,
                         WorkPhone = p.WorkPhone,
                         MemberStatus = p.MemberStatus.Description,
                         Email = p.EmailAddress,
                         BFTeacher = p.BFClass.LeaderName,
                         BFTeacherId = p.BFClass.LeaderId,
                         Age = p.Age.ToString(),
                         MemberTypeCode = om.MemberType.Code,
                         MemberType = om.MemberType.Description,
                         MemberTypeId = om.MemberTypeId,
                         AttendPct = om.AttendancePercentage,
                         Joined = om.EnrollmentDate,
                     };
            return q2;
        }
        public IQueryable<EnrollmentTransaction> ApplySort()
        {
            var q = FetchPrevMembers();
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "Name":
                        q = from om in q
                            let p = om.Person
                            orderby p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "Church":
                        q = from om in q
                            let p = om.Person
                            orderby p.MemberStatus.Code,
                            p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "Member":
                        q = from om in q
                            let p = om.Person
                            orderby om.MemberType.Code,
                            p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "Primary Address":
                        q = from om in q
                            let p = om.Person
                            orderby p.Family.StateCode,
                            p.Family.CityName,
                            p.Family.AddressLineOne,
                            p.PeopleId
                            select om;
                        break;
                    case "BFTeacher":
                        q = from om in q
                            let p = om.Person
                            orderby p.BFClass.LeaderName,
                            p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "% Att.":
                        q = from om in q
                            orderby om.AttendancePercentage
                            select om;
                        break;
                    case "Age":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthYear, p.BirthMonth, p.BirthDay
                            select om;
                        break;
                    case "Bday":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthMonth, p.BirthDay,
                            p.Name2
                            select om;
                        break;
                }
            else
                switch (Pager.Sort)
                {
                    case "Church":
                        q = from om in q
                            let p = om.Person
                            orderby p.MemberStatus.Code descending,
                            p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "Member":
                        q = from om in q
                            let p = om.Person
                            orderby om.MemberType.Code descending,
                            p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "Address":
                        q = from om in q
                            let p = om.Person
                            orderby p.Family.StateCode descending,
                                   p.Family.CityName descending,
                                   p.Family.AddressLineOne descending,
                                   p.PeopleId descending
                            select om;
                        break;
                    case "BFTeacher":
                        q = from om in q
                            let p = om.Person
                            orderby p.BFClass.LeaderName descending,
                            p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "% Att.":
                        q = from om in q
                            orderby om.AttendancePercentage descending
                            select om;
                        break;
                    case "Name":
                        q = from om in q
                            let p = om.Person
                            orderby p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "Bday":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthMonth descending, p.BirthDay descending,
                            p.Name2
                            select om;
                        break;
                    case "Age":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthYear descending, p.BirthMonth descending, p.BirthDay descending
                            select om;
                        break;
                }
            return q;
        }

    }
}