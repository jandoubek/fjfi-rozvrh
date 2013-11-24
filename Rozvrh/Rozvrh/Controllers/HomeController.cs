﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rozvrh.Exporters.Common;
using Rozvrh.Models;
using Rozvrh.Exporters;

namespace Rozvrh.Controllers
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

        public PartialViewResult Filter(string degreeYear, string specialization, string groups, string department, string lecturer, string building, string classroom, string day, string time)
        {
            if (!String.IsNullOrEmpty(degreeYear))
                M.FilterDegreeYear2Specialization(degreeYear);

            if (!String.IsNullOrEmpty(specialization))
                M.FilterSpecialization2Groups(specialization);

            if (!String.IsNullOrEmpty(department))
                M.FilterDepartments2Lecturers(department);

            if (!String.IsNullOrEmpty(building))
                M.FilterBuildings2Classrooms(building);

            return PartialView("Vyber", M);
        }

        public PartialViewResult FilterAll(string groups, string department, string lecturer, string classroom, string day, string time)
        {
            if (!isNull(groups) || !isNull(department) || !isNull(lecturer) || !isNull(classroom) || !isNull(day) || !isNull(time))
                M.FilterAll2TimetableFields(groups, department, lecturer, classroom, day, time);

            return PartialView("VyfiltrovaneLekce", M);
        }

        private bool isNull(string text)
        {
            return text == "null";
        }

    }
}
