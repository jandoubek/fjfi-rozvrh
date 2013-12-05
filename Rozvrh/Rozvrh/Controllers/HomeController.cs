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

        public HomeController()
        {
            System.Diagnostics.Debug.WriteLine("Controller constructor");
            M = new Model();
        }

        public ActionResult Index()
        {
            return View(M);
        }

        public ActionResult ExportToSVG()
        {
            //FIX ME (VASEK): Misto retezce rozvrh prijde z UI od uzivatele nadpis rozvrhu. 
            return ImportExport.Instance.DownloadAsSVG("Rozvrh",Config.Instance.Created,Config.Instance.LinkToAdditionalInformation);
        }

        public ActionResult ExportToICal()
        {
            return ImportExport.Instance.DownloadAsICAL(Config.Instance.SemesterStart, Config.Instance.SemesterEnd);
        }

        public ActionResult ExportToXML()
        {
            return ImportExport.Instance.DownloadAsXML();
        }

        public PartialViewResult Filter(Model returnedModel)
        {
            if (returnedModel.SelectedDegreeYears.Any())
                M.FilterSpecializationsByDegreeYears(returnedModel.SelectedDegreeYears.ConvertAll(d => d.ToString()));

            if (returnedModel.SelectedSpecializations.Any())
                M.FilterGroupsBySpecializations(returnedModel.SelectedSpecializations.ConvertAll(d => d.ToString()));

            if (returnedModel.SelectedDepartments.Any())
                M.FilterLecturersByDepartments(returnedModel.SelectedDepartments.ConvertAll(d => d.ToString()));

            if (returnedModel.SelectedBuildings.Any())
                M.FilterClassroomsByBuildings(returnedModel.SelectedBuildings.ConvertAll(b => b.ToString()));

            ImportExport.Instance.setLectures(M.FiltredTimetableFields);
            return PartialView("Vyber", M);
        }

        public PartialViewResult FilterAll(List<string> degreeYears, List<string> specializations, List<string> groups,
            List<string> departments, List<string> lecturers, List<string> buildings,
            List<string> classrooms, List<string> days, List<string> times)
        {
            removeEmptyElement(degreeYears);
            removeEmptyElement(specializations);
            removeEmptyElement(groups);
            removeEmptyElement(departments);
            removeEmptyElement(buildings);
            removeEmptyElement(lecturers);
            removeEmptyElement(classrooms);
            removeEmptyElement(days);
            removeEmptyElement(times);

            if (degreeYears.Any() || specializations.Any() || groups.Any() || departments.Any() ||
                lecturers.Any() || buildings.Any() || classrooms.Any() || days.Any() || times.Any())

                M.FilterTimetableFieldsByAll(degreeYears, specializations, groups, departments, lecturers, buildings, classrooms, days, times);

            ImportExport.Instance.setLectures(M.FiltredTimetableFields);

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

        public ActionResult SelectAll()
        {
            var opacity = new double[] {0.3114, 0.4583, 0.6338};
            M.FilterTimetableFieldsByAll(new List<string> {"6"}, new List<string> {"9"}, new List<string> {"25"}, new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>());
            M.CustomTimetableFields = M.FiltredTimetableFields;
            foreach (var tf in M.CustomTimetableFields)
            {
                var hexColor = int.Parse(tf.color).ToString("X6");
                tf.color = "";
                for (int i = 0; i < 3; i++)
                {
                    double x = Convert.ToDouble(Int32.Parse(hexColor.Substring(i*2, 2), NumberStyles.HexNumber));
                    x = (1 - opacity[i])*255 + opacity[i]*x;
                    tf.color += Convert.ToInt32(Math.Floor(x)).ToString("X2");
                }
            }
            return PartialView("Rozvrh", M);
        }
    }
}
