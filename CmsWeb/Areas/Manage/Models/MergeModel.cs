﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data.Linq;
using CmsData;
using UtilityExtensions;
using System.Web.Mvc;
using System.Text;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace CmsWeb.Models
{
    public class MergeModel
    {
        public List<BasicInfo> pi { get; set; }

        public int UseTitleCode { get; set; }
        public int UseFirstName { get; set; }
        public int UseLastName { get; set; }
        public int UseNickName { get; set; }
        public int UseAltName { get; set; }
        public int UseGenderId { get; set; }
        public int UseMaritalStatusId { get; set; }
        public int UseDOB { get; set; }
        public int UseCellPhone { get; set; }
        public int UseEmailAddress { get; set; }
        public int UseEmailAddress2 { get; set; }
        public int UseSuffixCode { get; set; }
        public int UseMiddleName { get; set; }
        public int UseHomePhone { get; set; }
        public int UseAddressLineOne { get; set; }
        public int UseAddressLineTwo { get; set; }
        public int UseCityName { get; set; }
        public int UseStateCode { get; set; }
        public int UseZipCode { get; set; }

        public class BasicInfo
        {
            private CMSPresenter.CodeValueController cvc = new CMSPresenter.CodeValueController();

            public int PeopleId { get; set; }
            public string TitleCode { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string NickName { get; set; }
            public string AltName { get; set; }
            public int GenderId { get; set; }
            public string Gender
            {
                get { return cvc.GenderCodes().Single(gg => gg.Id == GenderId).Value; }
                set { GenderId = cvc.GenderCodes().Single(gg => gg.Value == value).Id; }
            }
            public int MaritalStatusId { get; set; }
            public string MaritalStatus
            {
                get { return cvc.MaritalStatusCodes().Single(gg => gg.Id == MaritalStatusId).Value; }
                set { MaritalStatusId = cvc.MaritalStatusCodes().Single(gg => gg.Value == value).Id; }
            }
            public string DOB { get; set; }
            public string CellPhone { get; set; }
            public string EmailAddress { get; set; }
            public string EmailAddress2 { get; set; }
            public string SuffixCode { get; set; }
            public string MiddleName { get; set; }
            public string HomePhone { get; set; }
            public string AddressLineOne { get; set; }
            public string AddressLineTwo { get; set; }
            public string CityName { get; set; }
            public string StateCode { get; set; }
            public string ZipCode { get; set; }
        }


        public MergeModel(int pid1, int pid2)
        {
            var Db = DbUtil.Db;
            var q = from p in DbUtil.Db.People
                    where p.PeopleId == pid1 || p.PeopleId == pid2
                    orderby p.PeopleId == pid1 ? 1 : 2
                    select new BasicInfo
                    {
                        PeopleId = p.PeopleId,
                        TitleCode = p.TitleCode,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        NickName = p.NickName,
                        AltName = p.AltName,
                        GenderId = p.GenderId,
                        MaritalStatusId = p.MaritalStatusId,
                        DOB = p.DOB,
                        CellPhone = p.CellPhone,
                        HomePhone = p.Family.HomePhone,
                        EmailAddress = p.EmailAddress,
                        EmailAddress2 = p.EmailAddress2,
                        SuffixCode = p.SuffixCode,
                        MiddleName = p.MiddleName,
                        AddressLineOne = p.Family.AddressLineOne,
                        AddressLineTwo = p.Family.AddressLineTwo,
                        CityName = p.Family.CityName,
                        StateCode = p.Family.StateCode,
                        ZipCode = p.Family.ZipCode,
                    };
            pi = q.ToList();
            pi.Add(new BasicInfo());

            UseTitleCode = 1;
            UseFirstName = 1;
            UseLastName = 1;
            UseNickName = 1;
            UseAltName = 1;
            UseGenderId = 1;
            UseMaritalStatusId = 1;
            UseDOB = 1;
            UseCellPhone = 1;
            UseEmailAddress = 1;
            UseEmailAddress2 = 1;
            UseSuffixCode = 1;
            UseMiddleName = 1;
            UseHomePhone = 1;
            UseAddressLineOne = 1;
            UseAddressLineTwo = 1;
            UseCityName = 1;
            UseStateCode = 1;
            UseZipCode = 1;
        }
        public MergeModel()
        {

        }
        public void Delete()
        {
            var p = DbUtil.Db.LoadPersonById(pi[0].PeopleId);
            p.PurgePerson(DbUtil.Db);
        }
        public void Update()
        {
            var target = DbUtil.Db.LoadPersonById(pi[1].PeopleId);
            var psb = new StringBuilder();

            target.UpdateValue(psb, "TitleCode", pi[UseTitleCode].TitleCode);
            target.UpdateValue(psb, "FirstName", pi[UseFirstName].FirstName ?? "?");
            target.UpdateValue(psb, "LastName", pi[UseLastName].LastName ?? "?");
            target.UpdateValue(psb, "NickName", pi[UseNickName].NickName);
            target.UpdateValue(psb, "AltName", pi[UseAltName].AltName);
            target.UpdateValue(psb, "GenderId", pi[UseGenderId].GenderId);
            target.UpdateValue(psb, "MaritalStatusId", pi[UseMaritalStatusId].MaritalStatusId);
            target.UpdateValue(psb, "DOB", pi[UseDOB].DOB);
            target.UpdateValue(psb, "CellPhone", pi[UseCellPhone].CellPhone.GetDigits());
            target.UpdateValue(psb, "EmailAddress", pi[UseEmailAddress].EmailAddress);
            target.UpdateValue(psb, "EmailAddress2", pi[UseEmailAddress2].EmailAddress2);
            target.UpdateValue(psb, "SuffixCode", pi[UseSuffixCode].SuffixCode);
            target.UpdateValue(psb, "MiddleName", pi[UseMiddleName].MiddleName);

            var fsb = new StringBuilder();

            target.Family.UpdateValue(fsb, "HomePhone", pi[UseHomePhone].HomePhone.GetDigits());
            target.Family.UpdateValue(fsb, "AddressLineOne", pi[UseAddressLineOne].AddressLineOne);
            target.Family.UpdateValue(fsb, "AddressLineTwo", pi[UseAddressLineTwo].AddressLineTwo);
            target.Family.UpdateValue(fsb, "CityName", pi[UseCityName].CityName);
            target.Family.UpdateValue(fsb, "StateCode", pi[UseStateCode].StateCode);
            target.Family.UpdateValue(fsb, "ZipCode", pi[UseZipCode].ZipCode);

            target.LogChanges(DbUtil.Db, psb, Util.UserPeopleId.Value);
            target.Family.LogChanges(DbUtil.Db, fsb, target.PeopleId, Util.UserPeopleId.Value);
            DbUtil.Db.SubmitChanges();
        }
        public void Move()
        {
            var from = DbUtil.Db.LoadPersonById(pi[0].PeopleId);
            from.MovePersonStuff(DbUtil.Db, pi[1].PeopleId);
        }
        public SelectList GenderList()
        {
            var cv = new CMSPresenter.CodeValueController();
            return new SelectList(cv.GenderCodes(), "Id", "Value");
        }
        public SelectList MaritalStatusList()
        {
            var cv = new CMSPresenter.CodeValueController();
            return new SelectList(cv.MaritalStatusCodes(), "Id", "Value");
        }
    }
}