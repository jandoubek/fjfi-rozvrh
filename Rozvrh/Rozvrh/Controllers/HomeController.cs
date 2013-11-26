using System;
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

        public ActionResult ExportToSVG()
        {
            var ie = new ImportExport();
            //FIX ME (VASEK): Misto retezce rozvrh prijde z UI od uzivatele nadpis rozvrhu. 
            return ie.DownloadAsSVG(M.TimetableFields,"Rozvrh");            
        }

        public ActionResult ExportToICal()
        {
            var ie = new ImportExport();
            return ie.DownloadAsICAL(M.TimetableFields,Config.Instance.SemesterStart,Config.Instance.SemesterEnd);
        }

        public ActionResult ExportToXML()
        {
            var ie = new ImportExport();
            return ie.DownloadAsXML(M.TimetableFields);
        }

        public PartialViewResult Filter(Model returnedModel, string degreeYear, string specialization)
        {

            if (!isNull(degreeYear))
                M.FilterDegreeYear2Specialization(degreeYear);

            if (!isNull(specialization))
                M.FilterSpecialization2Groups(specialization);

            if (returnedModel.SelectedDepartments.Any())
                M.FilterLecturersByDepartments(returnedModel.SelectedDepartments.ConvertAll(d => d.ToString()));

            if (returnedModel.SelectedBuildings.Any())
                M.FilterClassroomsByBuildings(returnedModel.SelectedBuildings.ConvertAll(b => b.ToString()));

            return PartialView("Vyber", M);
        }

        public PartialViewResult FilterAll(List<string> groups, List<string> departments, List<string> lecturers, List<string> classrooms, List<string> days, List<string> times)
        {
            removeEmptyElement(groups);
            removeEmptyElement(departments);
            removeEmptyElement(lecturers);
            removeEmptyElement(classrooms);
            removeEmptyElement(days);
            removeEmptyElement(times);

            if (groups.Any() || departments.Any() || lecturers.Any() || classrooms.Any() || days.Any() || times.Any())
                M.FilterAll2TimetableFields(groups, departments, lecturers, classrooms, days, times);

            return PartialView("VyfiltrovaneLekce", M);
        }

        private bool isNull(string text)
        {
            return text == null || text == "null";
        }

        /// <summary>
        /// Checks if the list contains only one element which is empty string. If so, the empty element is removed.
        /// </summary>
        /// <param name="list">List to be checked and modified</param>
        /// <returns></returns>
        private void removeEmptyElement(List<string> list)
        {
            if (list.Count() == 1 && list[0] == "")
                list.RemoveAt(0);
        }

    }
}
