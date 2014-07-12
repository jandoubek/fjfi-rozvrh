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
using Rozvrh.Models.Timetable;
using System.Text;
using System.Web.UI;

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
            return instance.DownloadAsSVG(M.CustomTimetableFields, "Rozvrh", Config.Instance.Created, Config.Instance.LinkToAdditionalInformation);
        }

        public ActionResult ViewRozvrh()
        {
            return View("RozvrhForExport", M);
        }

        public ActionResult ExportToPNG()
        {
            ImportExport instance = new ImportExport();

            return instance.DownloadAsBITMAP(M.CustomTimetableFields, "Rozvrh", Config.Instance.Created,
                Config.Instance.LinkToAdditionalInformation, Server.MapPath("~/App_Data/"), "png");
        }

        public ActionResult ExportToJPG()
        {
            ImportExport instance = new ImportExport();

            return instance.DownloadAsBITMAP(M.CustomTimetableFields, "Rozvrh", Config.Instance.Created,
                Config.Instance.LinkToAdditionalInformation, Server.MapPath("~/App_Data/"), "jpg");
        }

        public ActionResult ExportToPDF()
        {
            /*ImportExport instance = new ImportExport();

            return instance.DownloadAsBITMAP(M.CustomTimetableFields, "Rozvrh", Config.Instance.Created,
                Config.Instance.LinkToAdditionalInformation, Server.MapPath("~/App_Data/"), "pdf");*/
            return new Rotativa.ViewAsPdf("ExportToPDF", M)
            {
                FileName = "MůjRozvrh.pdf",
                //PageSize = Rotativa.Options.Size.A4,
                //PageOrientation = Rotativa.Options.Orientation.Landscape
            }; 
          
        }

        public ActionResult ExportToICal()
        {
            ImportExport instance = new ImportExport();
            return instance.DownloadAsICAL(M.CustomTimetableFields, Config.Instance.SemesterStart, Config.Instance.SemesterEnd);
        }

        public ActionResult ExportToXML()
        {
            ImportExport instance = new ImportExport();
            return instance.DownloadAsXML(M.CustomTimetableFields);
        }

        public ActionResult ExportToPNG()
        {
            ImportExport instance = new ImportExport();
           
            return instance.DownloadAsBITMAP(M.CustomTimetableFields, "Rozvrh", Config.Instance.Created, 
                Config.Instance.LinkToAdditionalInformation, Server.MapPath("~/App_Data/"),"png");
        }

        public ActionResult ExportToJPG()
        {
            ImportExport instance = new ImportExport();

            return instance.DownloadAsBITMAP(M.CustomTimetableFields, "Rozvrh", Config.Instance.Created,
                Config.Instance.LinkToAdditionalInformation, Server.MapPath("~/App_Data/"),"jpg");
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

            System.Web.HttpContext.Current.Session["FiltredTimetableFields"] = M.FiltredTimetableFields;

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

        public JsonResult GetTimetableField(string uid)
        {
            int uniqueID;
            if (int.TryParse(uid, out uniqueID))
            {

                var field = M.CustomTimetableFields.Where(f => f.UniqueID == uniqueID).ToArray()[0];
                return Json(field, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult EditTimetableField(string uid, string lecture, string lecturer, string room, string department, string day, string hours, string minutes, string duration, string period)
        {
            int uniqueID;
            if (int.TryParse(uid, out uniqueID))
            {
                minutes = Convert.ToInt32(minutes).ToString("00");

                M.CustomTimetableFields.RemoveAll(field => field.UniqueID == uniqueID);

                var newField = new TimetableField { lecturer = lecturer, lecture_acr = lecture, classroom = room, department_acr = department, day = day, time = hours + minutes, duration = duration };
                newField.period = (period.ToLower() == "true" ? "Ano" : "Ne");

                var possibleDepartments = M.Departments.Where(dep => dep.acronym == department).ToArray();
                if (possibleDepartments.Any())
                    newField.color = possibleDepartments[0].color;
                /*
                var possibleTimes = M.Times.Where(t => t.hours == hours).ToArray();
                if (possibleTimes.Any())
                {
                    newField.time_hours = possibleTimes[0].hours;
                    newField.time_minutes = possibleTimes[0].minutes;
                }
                */
                newField.time_hours = hours;
                newField.time_minutes = minutes;

                var possibleDays = M.Days.Where(d => d.name == day).ToArray();
                if (possibleDays.Any())
                    newField.day_order = possibleDays[0].daysOrder;

                newField.RecalculateUniqueID();
                M.CustomTimetableFields.Add(newField);
            }
            SaveToSession();
            return PartialView("Rozvrh", M);
        }

        public ActionResult AddAll(List<string> uids)
        {
            var filtredFields = (List<TimetableField>)System.Web.HttpContext.Current.Session["FiltredTimetableFields"];
            if (filtredFields != null)
            {
                M.CustomTimetableFields.AddRange(filtredFields);

                //Removes all duplicates according to UniqueID
                var elements = new HashSet<int>();
                M.CustomTimetableFields.RemoveAll(i => !elements.Add(i.UniqueID));
            }
            SaveToSession();

            return PartialView("Rozvrh", M);
        }

        public ActionResult AddSome(List<string> uids)
        {
            var filtredFields = (List<TimetableField>)System.Web.HttpContext.Current.Session["FiltredTimetableFields"];
            if (filtredFields != null)
            {
                M.CustomTimetableFields.AddRange(filtredFields.Where(field => uids.Contains(field.UniqueID.ToString())));

                //Removes all duplicates according to UniqueID
                var elements = new HashSet<int>();
                M.CustomTimetableFields.RemoveAll(i => !elements.Add(i.UniqueID));
            }
            
            SaveToSession();

            return PartialView("Rozvrh", M);
            }

        public ActionResult RemoveAll()
        {
            M.CustomTimetableFields.Clear();

            SaveToSession();

            return PartialView("Rozvrh", M);
        }

        public ActionResult RemoveOne(string uid)
        {
            int uniqueID;
            if(int.TryParse(uid, out uniqueID))
            {
                M.CustomTimetableFields.RemoveAll(field => field.UniqueID == uniqueID);
            }
            SaveToSession();
            return PartialView("Rozvrh", M);
        }

        /// <summary>
        /// Loads data from session to model.
        /// </summary>
        private void LoadFromSession()
        {
                M.CustomTimetableFields = (List<TimetableField>)System.Web.HttpContext.Current.Session["CustomTimetableFields"];
            if (M.CustomTimetableFields == null)
            {
                    M.CustomTimetableFields = new List<TimetableField>();
                }
       }

        /// <summary>
        /// Saves data from model to session.
        /// </summary>
        private void SaveToSession()
        {
            System.Web.HttpContext.Current.Session["CustomTimetableFields"] = M.CustomTimetableFields;
        }



        public static List<List<TimetableField>> getGroups(List<TimetableField> fields)
        {            
            List<List<TimetableField>> groups = new LecturesGroupDivider().divideToGroups(fields);
            return groups;
        }


    }
}
