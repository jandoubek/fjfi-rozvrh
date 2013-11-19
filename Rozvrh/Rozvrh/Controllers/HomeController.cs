using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rozrvh.Exporters.Common;
using Rozvrh.Models;

namespace Rozrvh.Controllers
{
    public class HomeController : Controller
    {
        private Model M;
        public HomeController(){
            System.Diagnostics.Debug.WriteLine("Controller constructor");
            M = new Model();
        }

        public ActionResult Index()
        {
           return View(M);
        }

        public string Show()
        {

            return "<img src=\"Images/ing2jch1.png\" />";

        }

        public PartialViewResult Filter(string rocnik, string zamereni, string kruh)
        {
            var m = new Model();
            
            if (!String.IsNullOrEmpty(rocnik) && !String.IsNullOrEmpty(zamereni) && !String.IsNullOrEmpty(kruh) && rocnik == m.Years[4] && zamereni == m.Courses[0] && kruh == m.Groups[0])
                DownloadAsICalController.prototypeData(m.FiltredLectures);

            return PartialView("VyfiltrovaneLekce", m);
        }
    }
}
