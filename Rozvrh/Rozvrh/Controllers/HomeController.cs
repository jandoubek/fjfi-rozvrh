using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rozrvh.Exporters.Common;
using Rozvrh.Models;
using Rozvrh.Exporters;

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
            
            return PartialView("VyfiltrovaneLekce", m);
        }
    }
}
