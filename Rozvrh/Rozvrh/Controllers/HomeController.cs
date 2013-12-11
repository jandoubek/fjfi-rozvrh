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
using System.IO;

namespace Rozvrh.Controllers
{
    public class HomeController : Controller
    {
        private Model M;

        public HomeController()
        {
            System.Diagnostics.Debug.WriteLine("Controller constructor");
            M = new Model();
            LoadFromSession();
        }

        public ActionResult Index()
        {
            return View(M);
        }

        public ActionResult ExportToSVG()
        {
            //FIX ME (VASEK): Misto retezce rozvrh prijde z UI od uzivatele nadpis rozvrhu. 
            ImportExport instance = new ImportExport();
            return instance.DownloadAsSVG(M.CustomTimetableFields,"Rozvrh", Config.Instance.Created, Config.Instance.LinkToAdditionalInformation);
        }

        public ActionResult ExportToICal()
        {
            ImportExport instance = new ImportExport();
            return instance.DownloadAsICAL(M.CustomTimetableFields,Config.Instance.SemesterStart, Config.Instance.SemesterEnd);
        }

        public ActionResult ExportToXML()
        {
            ImportExport instance = new ImportExport();
            return instance.DownloadAsXML(M.CustomTimetableFields);
        }

        [HttpPost]
        public ActionResult ImportFromXML(HttpPostedFileBase file)
        {
            ImportExport instance = new ImportExport();
            try
            {
                List<TimetableField> result;
                result = instance.ImportXML(file);
                M.CustomTimetableFields = result;
                SaveToSession();
            }
            catch (InvalidDataException ex)
            {
                M.ImportErrorMessage = ex.Message;
            }

            return View("Index", M);
            
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
            //2. Bc, ASI, 1 - 1,9,25
            //2. Bc, APIN, 1 - 1,15,32
            //2. Bc, FYT, 1 - 1,121,133
            M.FilterTimetableFieldsByAll(new List<string> { "1" }, new List<string> { "121" }, new List<string> { "133" }, new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>());
            M.CustomTimetableFields = M.FiltredTimetableFields;
            foreach (var tf in M.CustomTimetableFields)
            {
                tf.color = int.Parse(tf.color).ToString("X6");
            }
            SaveToSession();

            return PartialView("Rozvrh", M);
        }

        /// <summary>
        /// Loads data from session to model.
        /// </summary>
        private void LoadFromSession() {
                M.CustomTimetableFields = (List<TimetableField>)System.Web.HttpContext.Current.Session["CustomTimetableFields"];
                if (M.CustomTimetableFields == null) {
                    M.CustomTimetableFields = new List<TimetableField>();
                }
       }

        /// <summary>
        /// Saves data from model to session.
        /// </summary>
        private void SaveToSession() {
            System.Web.HttpContext.Current.Session["CustomTimetableFields"] = M.CustomTimetableFields;
        }

        /// <summary>
        /// Returns 1.1.2001 date with hours and minutes of the TimetableField
        /// </summary>
        /// <param name="field">Field to be processed</param>
        /// <returns></returns>
        private static DateTime getStartTime(TimetableField field)
        {
            return new DateTime(2001, 1, 1, int.Parse(field.time_hours), int.Parse(field.time_minutes), 0);
        }

        /// <summary>
        /// Returns fields duration
        /// </summary>
        /// <param name="field">Field to be processed</param>
        /// <returns></returns>
        private static TimeSpan getDuration(TimetableField field)
        {
            return new TimeSpan(int.Parse(field.duration), 0, 0);
        }

        public static List<List<TimetableField>> getGroups(List<TimetableField> fields)
        {
            fields.Sort((x, y) => DateTime.Compare(getStartTime(x), getStartTime(y)));
            var groups = new List<List<TimetableField>> { new List<TimetableField>() };

            foreach (TimetableField l in fields)
            {
                var lastgroup = groups[groups.Count - 1];
                if (lastgroup.Count < 1)
                {
                    groups[groups.Count - 1].Add(l);
                }
                //Is lecture l starting sooner than end time of the last lecture in last group (with 10 minutes toleration)?
                else if (DateTime.Compare(getStartTime(l), lastgroup.Max(lec => getStartTime(lec) + getDuration(lec) - new TimeSpan(0, 10, 0))) < 0)
                {
                    //All lectures intersecting with l
                    List<TimetableField> intersecting = lastgroup.Where(lec => (DateTime.Compare(getStartTime(l), getStartTime(lec) + getDuration(lec) - new TimeSpan(0, 10, 0)) < 0)).ToList();
                    //Only have to worry about 3 and more lectures in one group
                    //If lecture l is intersecting with all of lectures in lastgroup then it belongs to the group
                    if (lastgroup.Count <= 1 || intersecting.Count() == lastgroup.Count)
                    {
                        lastgroup.Add(l);
                    }
                    else
                    {
                        //Otherwise create a fake lectures from all intersecting lectures
                        var newGroup = intersecting.Select(interLec => new FakeTimetableField(interLec.time_hours, interLec.time_minutes, interLec.duration))
                            .Cast<TimetableField>().ToList();

                        //And create a new last group from l and the fake lectures
                        newGroup.Add(l);
                        groups.Add(newGroup);
                    }

                }
                else
                {
                    groups.Add(new List<TimetableField>());
                    lastgroup = groups[groups.Count - 1];
                    lastgroup.Add(l);
                }
            }
            return groups;
        }


    }
}
