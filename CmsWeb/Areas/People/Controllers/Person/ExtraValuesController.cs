using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Code;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/DeleteExtra/{id:int}/{name}")]
        public ContentResult DeleteExtra(int id, string name)
        {
            var e = DbUtil.Db.PeopleExtras.First(ee => ee.PeopleId == id && ee.Field == HttpUtility.UrlDecode(name));
            DbUtil.Db.PeopleExtras.DeleteOnSubmit(e);
            DbUtil.Db.SubmitChanges();
            return Content("done");
        }

        [GET("Person2/ExtraValueCodes")]
        public ActionResult ExtraValueCodes(string name)
        {
            var j = StandardExtraValues.CodesJson(HttpUtility.UrlDecode(name));
            return Content(j);
        }

        [POST("Person2/ExtraValuesStandard/{id}")]
        public ActionResult ExtraValuesStandard(int id)
        {
            return View("Profile/ExtraValuesStandard", id);
        }

        [POST("Person2/ExtraValuesAdhoc/{id}")]
        public ActionResult ExtraValuesAdhoc(int id)
        {
            return View("Profile/ExtraValuesAdhoc", id);
        }

        [POST("Person2/EditExtra")]
        public ActionResult EditExtra(string pk, string name, string value)
        {
            StandardExtraValues.EditExtra(pk, HttpUtility.UrlDecode(name), value);
            return new EmptyResult();
        }

        [GET("Person2/ExtraValueBits/{id:int}")]
        public ActionResult ExtraValueBits(int id, string name)
        {
            var j = StandardExtraValues.ExtraValueBitsJson(id, HttpUtility.UrlDecode(name));
            return Content(j);
        }

        [POST("Person2/NewExtraValue/{id:int}")]
        public ActionResult NewExtraValue(int id, string field, string type, string value)
        {
            StandardExtraValues.NewExtra(id, field, type, value);
            return new EmptyResult();
        }
    }
}