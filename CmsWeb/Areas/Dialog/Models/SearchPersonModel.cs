using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;
using CmsData;
using System.Text;
using System.Data.Linq;
using CMSPresenter;
using CmsData.Codes;

namespace CmsWeb.Models
{
    public class SearchPersonModel
    {
        private static CodeValueController cv = new CodeValueController();

        public int index { get; set; }
        public string first { get; set; }
        public string goesby { get; set; }
        public string middle { get; set; }
        public string last { get; set; }
        public string title { get; set; }
        public string suffix { get; set; }
        public string dob { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int gender { get; set; }
        public int marital { get; set; }

        public string homephone { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }

        private DateTime? _Birthday;
        public DateTime? birthday
        {
            get
            {
                DateTime dt;
                if (!_Birthday.HasValue && dob != "na")
                    if (Util.DateValid(dob, out dt))
                        _Birthday = dt;
                return _Birthday;
            }
        }
        public int? age
        {
            get
            {
                if (birthday.HasValue)
                    return birthday.Value.AgeAsOf(Util.Now);
                return null;
            }
        }
        public string genderdisplay
        {
            get { return cv.GenderCodesWithUnspecified().ItemValue(gender); }
        }
        public string marrieddisplay
        {
            get { return cv.MaritalStatusCodes99().ItemValue(marital); }
        }
        public IEnumerable<SelectListItem> TitleCodes()
        {
            return QueryModel.ConvertToSelect(cv.TitleCodes(), "Code");
        }
        public IEnumerable<SelectListItem> GenderCodes()
        {
            return QueryModel.ConvertToSelect(cv.GenderCodesWithUnspecified(), "Id");
        }
        public IEnumerable<SelectListItem> MaritalStatuses()
        {
            return QueryModel.ConvertToSelect(cv.MaritalStatusCodes99(), "Id");
        }
        public IEnumerable<SelectListItem> StateCodes()
        {
            return QueryModel.ConvertToSelect(cv.GetStateListUnknown(), "Code");
        }
        public IEnumerable<SelectListItem> Countries()
        {
            var list = QueryModel.ConvertToSelect(CodeValueController.GetCountryList(), null);
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "" });
            return list;
        }

        public bool IsNew
        {
            get { return !PeopleId.HasValue; }
        }

        public int FamilyId { get; set; }
        private Family _family;
        public Family family
        {
            get
            {
                if (_family == null && FamilyId > 0)
                    _family = DbUtil.Db.Families.Single(f => f.FamilyId == FamilyId);
                return _family;
            }
        }
        public int? PeopleId { get; set; }
        private Person _Person;
        public Person person
        {
            get
            {
                if (_Person == null && PeopleId.HasValue)
                    _Person = DbUtil.Db.LoadPersonById(PeopleId.Value);
                return _Person;
            }
        }

        internal void ValidateModelForNew(ModelStateDictionary ModelState, bool checkaddress)
        {
            if (!first.HasValue())
                ModelState.AddModelError("first", "first name required");

            if (!last.HasValue())
                ModelState.AddModelError("last", "last name required");

            if (!birthday.HasValue && dob != "na")
                ModelState.AddModelError("dob", "valid birthday (or \"na\")");

            var d = phone.GetDigits().Length;
            if (d != 7 && d < 10 && phone != "na")
                ModelState.AddModelError("phone", "7 or 10+ digits (or \"na\")");

            int count = 0;
            SearchPeopleModel.FindPerson(first, last, birthday ?? DateTime.MinValue, string.Empty, phone.GetDigits(), out count);
            if (count > 0)
                ModelState.AddModelError("first", "name/dob already exists in db");

            if (!Util.ValidEmail(email) && email != "na")
                ModelState.AddModelError("email", "valid email address (or \"na\")");

            if (gender == 99)
                ModelState.AddModelError("gender", "specify gender");

            if (marital == 99)
                ModelState.AddModelError("marital", "specify marital status");

            if (checkaddress)
            {

                if (!address.HasValue())
                    ModelState.AddModelError("address", "address required (or \"na\")");

                if (!city.HasValue())
                    ModelState.AddModelError("city", "city required (or \"na\")");

                if (!state.HasValue())
                    ModelState.AddModelError("state", "state required");

                if (ModelState.IsValid
                    && address != "na" && city != "na" && state != "na"
                    && (country == "United States" || !country.HasValue()))
                {
                    var r = AddressVerify.LookupAddress(address, address2, city, state, zip);
                    if (r.Line1 != "error")
                    {
                        if (!r.found)
                        {
                            ModelState.AddModelError("zip", r.address + ", if your address will not validate, change the country to 'USA, Not Validated'");
                            return;
                        }
                        if (r.Line1 != address)
                        {
                            ModelState.AddModelError("address", "address changed from " + address);
                            address = r.Line1;
                        }
                        if (r.Line2 != (address2 ?? ""))
                        {
                            ModelState.AddModelError("address2", "address2 changed from " + address2);
                            address2 = r.Line2;
                        }
                        if (r.City != (city ?? ""))
                        {
                            ModelState.AddModelError("city", "city changed from " + city);
                            city = r.City;
                        }
                        if (r.State != (state ?? ""))
                        {
                            ModelState.AddModelError("state", "state changed from " + state);
                            state = r.State;
                        }
                        if (r.Zip != (zip ?? ""))
                        {
                            ModelState.AddModelError("zip", "zip changed from " + zip);
                            zip = r.Zip;
                        }
                    }
                }
            }
        }
        internal void AddPerson(int origin, int? entrypoint)
        {
            Family f;
            string na = "na";
            if (FamilyId > 0)
                f = family;
            else
                f = new Family
                {
                    HomePhone = homephone.GetDigits(),
                    AddressLineOne = address.Disallow(na),
                    AddressLineTwo = address2,
                    CityName = city.Disallow(na),
                    StateCode = state.Disallow(na),
                    ZipCode = zip.Disallow(na),
                };

            if (goesby != null)
                goesby = goesby.Trim();
            var position = PositionInFamily.Child;
            if (age >= 18)
                if (f.People.Count(per =>
                     per.PositionInFamilyId == PositionInFamily.PrimaryAdult)
                     < 2)
                    position = PositionInFamily.PrimaryAdult;
                else
                    position = PositionInFamily.SecondaryAdult;

            _Person = Person.Add(f, position,
                null, first.Trim(), goesby, last.Trim(), dob, false, gender,
                    origin, entrypoint);
            if (title.HasValue())
                person.TitleCode = title;
            person.EmailAddress = email.Disallow(na);
            person.MaritalStatusId = marital;
            person.SuffixCode = suffix;
            person.MiddleName = middle;
            person.CampusId = DbUtil.Db.Setting("DefaultCampusId", "").ToInt2();

            person.CellPhone = phone.GetDigits();
            DbUtil.Db.SubmitChanges();
            DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, person);
            PeopleId = person.PeopleId;
        }

        internal void LoadFamily()
        {
            if (FamilyId < 0)
                return;
            homephone = family.HomePhone;
            address = family.AddressLineOne;
            address2 = family.AddressLineTwo;
            city = family.CityName;
            state = family.StateCode;
            zip = family.ZipCode;
        }
        public string ToolTip
        {
            get
            {
                return "{0} {1}|{2}|{3}|Birthday: {4}|c {5}|h {6}|{7}|Gender: {8}|Marital: {9}".Fmt(
                    goesby ?? first, last,
                    address,
                    CityStateZip,
                    birthday.FormatDate(),
                    phone.FmtFone(),
                    homephone.FmtFone(),
                    email,
                    genderdisplay,
                    marrieddisplay);
            }
        }
        public string CityStateZip
        {
            get { return "{0}, {1} {2}".Fmt(city, state, zip); }
        }
    }
}
