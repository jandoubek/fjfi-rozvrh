using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rozrvh.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public string Show()
        {

            return "<img src=\"Images/ing2jch1.png\" />";

        }

        public PartialViewResult Filter(string rocnik, string zamereni, string kruh)
        {
            if (!String.IsNullOrEmpty(rocnik) && !String.IsNullOrEmpty(zamereni) && !String.IsNullOrEmpty(kruh) && rocnik == "5" && zamereni == "1" && kruh == "1")
                return PartialView("VyfiltrovaneLekce");
            return null;
        }
    }
}
