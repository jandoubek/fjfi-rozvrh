using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Rozvrh.Models.Timetable;

namespace Rozvrh.Models
{
    public class Model : IModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        /// <summary>
        /// Class constructor. Inits the properties which are used in View components.
        /// </summary>
        public Model()
        {
            log.Debug("Method entry.");

            initialize();
            SelectedTimetable = XMLTimetable.Instance;
            loadData();
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Class constructor. Inits the properties which are used in View components from given instance XMLTimetable - should be used only for unit testing.
        /// </summary>
        /// <param name="timetableData"></param>
        public Model(OneXMLTimetable timetableData)
        {
            log.Debug("Method entry.");

            initialize();
            SelectedTimetable = timetableData;
            loadData();

            log.Debug("Method exit.");
        }

        /// <summary>
        /// Initializes fields to empty lists.
        /// </summary>
        private void initialize()
        {
            log.Debug("Method entry.");

            WelcomeMessage = LoadWelcomeMessangeFromFile(Config.Instance.WelcomeMessageFilePath);

            Specializations = new List<Specialization>();
            Groups = new List<Group>();
            Lecturers = new List<Lecturer>();
            Classrooms = new List<Classroom>();


            SelectedDegreeYears = new List<int>();
            SelectedSpecializations = new List<int>();
            SelectedGroups = new List<int>();
            SelectedDepartments = new List<int>();
            SelectedLecturers = new List<int>();
            SelectedBuildings = new List<int>();
            SelectedClassrooms = new List<int>();
            SelectedDays = new List<int>();
            SelectedTimes = new List<int>();

            FiltredTimetableFields = new List<TimetableField>();
            CustomTimetableFields = new List<TimetableField>();

            log.Debug("Method exit.");
        }

        /// <summary>
        /// Loads data from XMLTimetable class and initializes all property fields.
        /// </summary>
        private void loadData()
        {
            log.Debug("Method entry.");

            try
            {
                Departments = SelectedTimetable.m_departments;
                DegreeYears = SelectedTimetable.m_degreeyears;
                Buildings = SelectedTimetable.m_buildings;
                Days = SelectedTimetable.m_days;
                Times = SelectedTimetable.m_times;
            }
            catch (Exception e)
            {
                log.Error("Error when parsing timetable XML." + e.StackTrace);
            }

            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering specializations (zaměření) by given degreeYears. Specializations are visible just only one degreeYear is selected.
        /// Result held in 'Specializations' property of Model.
        /// </summary>
        /// <param name="degreeYearIds"></param>
        public void FilterSpecializationsByDegreeYears(List<string> degreeYearIds)
        {
            Specializations = SelectedTimetable.FilterSpecializationsByDegreeYears(degreeYearIds);
        }

        /// <summary>
        /// Method filtering groups (kruhy) by given specializations (zaměření). Groups are visible when just one degreeYear is selected.
        /// Result held in 'Groups' property of Model.
        /// </summary>
        /// <param name="specializationIds"></param>
        public void FilterGroupsBySpecializations(List<string> specializationIds)
        {
            Groups = SelectedTimetable.FilterGroupsBySpecializations(specializationIds);
        }

        /// <summary>
        /// Method filtering lecturers by given departments, where employed. Result held in 'Lecturers' property of Model.
        /// </summary>
        /// <param name="departmentIds">Ids of the given departments</param>
        public void FilterLecturersByDepartments(List<string> departmentIds)
        {
            Lecturers = SelectedTimetable.FilterLecturersByDepartments(departmentIds);
        }

        /// <summary>
        /// Method filtering classrooms by given buildings. Result held in 'Classrooms' property of Model.
        /// </summary>
        /// <param name="buildingIds">Ids of the given buildings</param>
        public void FilterClassroomsByBuildings(List<string> buildingIds)
        {
            Classrooms = SelectedTimetable.FilterClassroomsByBuildings(buildingIds);
        }

        /// <summary>
        /// Method filtering lessons (vyučovací hodiny) by given degreeYears, specializations zaměření, groups (kruhy), 
        /// departments (dep. of the course), lecturers, classrooms, days and times.
        /// Result held in 'TimetableFields' property of Model.
        /// </summary>
        /// <param name="degreeYearIds">Ids of the given degreeYear.</param>
        /// <param name="specializationIds">Ids of the given specialization.</param>
        /// <param name="groupIds">Ids of the given groups.</param>
        /// <param name="departmentIds">Ids of the given departments.</param>
        /// <param name="lecturerIds">Ids of the given lecturers.</param>
        /// <param name="buildingIds">Ids of the given buildings.</param>
        /// <param name="classroomIds">Ids of the given classrooms.</param>
        /// <param name="dayIds">Ids of the given days.</param>
        /// <param name="timeIds">Ids of the given times.</param>
        public void FilterTimetableFieldsByAll(List<string> degreeYearIds,  List<string> specializationIds,  List<string> groupIds,
                                               List<string> departmentIds,  List<string> lecturerIds,        List<string> buildingIds,
                                               List<string> classroomIds,   List<string> dayIds,             List<string> timeIds,      string searchedString)
        {
            
            FiltredTimetableFields = SelectedTimetable.FilterTimetableFieldsByAll(degreeYearIds, specializationIds, groupIds, departmentIds, lecturerIds, buildingIds, classroomIds, dayIds, timeIds, searchedString);
            
        }

        private string LoadWelcomeMessangeFromFile(string welcomeMessageFilePath)
        {
            log.Debug("Method entry.");
            string result = "";
            try
            {
                log.Debug("Trying to load config from file: '" + welcomeMessageFilePath + "'");

                TextReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(welcomeMessageFilePath));
                if (reader == null)
                {
                    log.Error("Unable to load config file (wrong filepath).");
                }
                result = reader.ReadToEnd();
                reader.Close();
                log.Info("Welcome message successfully loaded from file: '" + welcomeMessageFilePath + "'");
                log.Debug("Method exit.");
                return result;
            }
            catch
            {
                log.Debug("Welcome message does not loaded from file: '" + welcomeMessageFilePath + "'");
                log.Debug("Method exit");
                return result;
            }
        }


        public List<Department> Departments { get; private set; }
        public List<Specialization> Specializations { get; private set; }
        public List<Group> Groups { get; private set; }
        public List<DegreeYear> DegreeYears { get; private set; }
        public List<Lecturer> Lecturers { get; private set; }
        public List<Building> Buildings { get; private set; }
        public List<Classroom> Classrooms { get; private set; }
        public List<Day> Days { get; private set; }
        public List<Time> Times { get; private set; }
        /// <summary>
        /// List of TimetableFields fitred using settings in filtrs on Vyber.cshtml
        /// </summary>
        public List<TimetableField> FiltredTimetableFields { get; private set; }

        /// <summary>
        /// List of TimetableFields selected by user to be part of his personal timetable
        /// </summary>
        public List<TimetableField> CustomTimetableFields { get; set; }

        public List<int> SelectedDegreeYears { get; set; }
        public List<int> SelectedSpecializations { get; set; }
        public List<int> SelectedGroups { get; set; }
        public List<int> SelectedDepartments { get; set; }
        public List<int> SelectedLecturers { get; set; }
        public List<int> SelectedBuildings { get; set; }
        public List<int> SelectedClassrooms { get; set; }
        public List<int> SelectedDays { get; set; }
        public List<int> SelectedTimes { get; set; }
        public string SearchedString { get; set; }

        /// <summary>
        /// Message to be shown on the empty filtering result list.
        /// </summary>
        public string WelcomeMessage { get; set; }

        /// <summary>
        /// List of timetables in archive.
        /// </summary>
        public List<TimetableInfo> TimetablesInfoList
        {
            get
            {
                return XMLTimetable.TimetableArchive.Select(t => t.m_timetableInfo).OrderByDescending(t=>t.Created).ToList();
            }
        }

        /// <summary>
        /// The property holding the selected timetable id.
        /// </summary>
        public string SelectedTimetableId { get; set; }

        /// <summary>
        /// The instance of currently selected timetable.
        /// </summary>
        public OneXMLTimetable SelectedTimetable { get; set; }
    }
}