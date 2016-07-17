using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VideoSharing.Business;
using VideoSharing.DAL;
using VideoSharing.Filters;

namespace VideoSharing.Controllers
{
    [ExceptionFilterAttribute]
    public class HomeController : Controller
    {
        public ActionResult Index(string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                if(!User.IsInRole("admin"))
                    return RedirectToAction("People", "User", new { searchString = searchString});
                else
                {
                    return RedirectToAction("PeopleTable", "User", new { searchString = searchString, sortOrder = "", filterValue = "" });
                }
            }
            return View();
        }

        public ActionResult About()
        {
            
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
          
            return View();
        }

        public ActionResult Change(string languageAbbreviation)
        {
            if (languageAbbreviation != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(languageAbbreviation);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageAbbreviation);
            }
            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = languageAbbreviation;
            Response.Cookies.Add(cookie);
            return View("Index");
        }
    }
}