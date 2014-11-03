using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rozvrh.Models.Timetable;
using System.Xml.Linq;

namespace Rozvrh.Models
{
    public class OneXMLTimetable: IXMLTimetable
    {
        #region Log
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        /// <summary>
        /// Holds path to the data source file, title (E.g. 2013/2014 - letní semestr) of the timetable and its id.
        /// </summary>
        public TimetableInfo m_timetableInfo { get; set; }

        /// <summary>
        /// Holds parsed xml document. For internal use.
        /// </summary>
        private XElement m_xelDefinitions;

        /// <summary>
        /// Holds relevant departments from the xml. Only depertments with specified name.
        /// </summary>
        public List<Department> m_departments { get; set; }

        /// <summary>
        /// Holds relevant courses from the xml. Only courses of departments specified in m_departments.
        /// </summary>
        public List<Course> m_courses { get; set; }

        /// <summary>
        /// Holds relevant lectures from the xml. Only lectures of courses specified in m_courses.
        /// </summary>
        public List<Lecture> m_lectures { get; set; }

        /// <summary>
        /// Holds relevant lecturers from the xml. Only lecturers of departments specified in m_departments.
        /// </summary>
        public List<Lecturer> m_lecturers { get; set; }

        /// <summary>
        /// Holds days from the xml.
        /// </summary>
        public List<Day> m_days { get; set; }

        /// <summary>
        /// Holds times from the xml.
        /// </summary>
        public List<Time> m_times { get; set; }

        /// <summary>
        /// Holds buildings from the xml.
        /// </summary>
        public List<Building> m_buildings { get; set; }

        /// <summary>
        /// Holds classrooms from the xml.
        /// </summary>
        public List<Classroom> m_classrooms { get; set; }

        /// <summary>
        /// Holds relevant lessons from the xml. Only lessons with specified time, day and lecture specified in m_lectures.
        /// </summary>
        public List<Lesson> m_lessons { get; set; }

        /// <summary>
        /// Holds relevant degree and year info. Extracted from 'degrees' and 'groups' elements.
        /// </summary>
        public List<DegreeYear> m_degreeyears { get; set; }

        /// <summary>
        /// Holds specializations from the xml. Extracted from the 'groups' element.
        /// </summary>
        public List<Specialization> m_specializations { get; set; }

        /// <summary>
        /// Holds relevant groups (kruhy) from the xml. Extracted from the 'parts' element.
        /// </summary>
        public List<Group> m_groups { get; set; }

        /// <summary>
        /// Holds relevant pairs of group and lesson from the xml. Extracted from cards element.
        /// </summary>
        public List<GroupLessonBinder> m_groupLessonBinder { get; set; }

        /// <summary>
        /// Reloads xml file, parses and initializes properties which holds base data of the application. This method should be used for refreshing the model after updating thxml with timetable.
        /// </summary>
        /// <param name="xmlPath">Path to the source xml file.</param>
        /// <returns>True if the databaze was successfuly loaded</returns>
        public bool Load(string xmlPath)
        {
            log.Debug("Method entry.");

            m_departments = new List<Department>();
            m_courses = new List<Course>();
            m_lectures = new List<Lecture>();
            m_lecturers = new List<Lecturer>();
            m_days = new List<Day>();
            m_times = new List<Time>();
            m_buildings = new List<Building>();
            m_classrooms = new List<Classroom>();
            m_degreeyears = new List<DegreeYear>();
            m_specializations = new List<Specialization>();
            m_lessons = new List<Lesson>();
            m_groups = new List<Group>();
            m_groupLessonBinder = new List<GroupLessonBinder>();
            m_timetableInfo = new TimetableInfo();

            if (!this.LoadFile(xmlPath))
            {
                log.Debug("Method exit.");
                return false;
            }
            if (!this.ParseData())
            {
                log.Debug("Method exit.");
                return false;
            }
            log.Debug("Method exit.");
            return true;
        }

        /// <summary>
        /// Method which open and parse the xml file.
        /// </summary>
        /// <returns>Bool result.</returns>
        private bool LoadFile(string xmlPath)
        {
            log.Debug("Method entry.");
            try
            {
                log.Debug("Trying to load XML database file: '" + xmlPath + "'.");

                string pathString;
                //if the path starts with ~ - use local server loading
                if (String.CompareOrdinal("~", 0, xmlPath, 0, 1) == 0)
                {
                    log.Info("Loading XML file from a server map path: '" + xmlPath + "'.");
                    pathString = System.Web.HttpContext.Current.Server.MapPath(xmlPath);
                }
                else
                {
                    log.Info("Loading XML file from an absolute path (like http): '" + xmlPath + "'.");
                    pathString = xmlPath;
                }
                m_xelDefinitions = XElement.Load(pathString);
                log.Debug("XML loaded.");
                log.Debug("Method exit.");
                return true;
            }
            catch
            {
                log.Error("Unable to load and parse the XML data file: '" + xmlPath + "'.");
                log.Debug("Method exit.");
                return false;
            }
        }

        /// <summary>
        /// Init class properties by xml data.
        /// </summary>
        /// <returns>Bool result.</returns>
        private bool ParseData()
        {
            log.Debug("Method entry.");
            try
            {
                // Init m_departments. Only depertments with code number less than 50. e.g. departments with acronym "REZERVA", a "PROPAGACE" are ommited.
                log.Debug("Loading departments data.");
                var enumDepartment =
                    from dep in m_xelDefinitions.Element("departments").Descendants("department")
                    where Convert.ToInt32(dep.Element("code").Value) < 50
                    orderby dep.Element("code").Value
                    select dep;
                foreach (XElement dep in enumDepartment)
                    m_departments.Add(new Department(dep.Attribute("id").Value, dep.Element("code").Value, dep.Element("name").Value, dep.Element("acronym").Value, dep.Element("color").Value));
                m_departments.Add(new Department("0", "-1", "---", "---", "0"));
                log.Debug("Loaded" + m_departments.Count + "departments data.");
                //--------------------------------------------------------------------------

                // Init m_courses. Only courses of the departments specified in m_departments.
                log.Debug("Loading course data.");
                var enumCourse =
                    from dep in m_departments
                    from c in m_xelDefinitions.Element("courses").Descendants("course")
                    where dep.id.Equals(c.Element("department").Attribute("ref").Value)
                    select c;
                foreach (XElement c in enumCourse)
                    m_courses.Add(new Course(c.Attribute("id").Value, c.Element("department").Attribute("ref").Value, c.Element("name").Value, c.Element("acronym").Value));
                log.Debug("Loaded" + m_courses.Count + "courses data.");
                //--------------------------------------------------------------------------

                // Init m_lectures. Only lectures of the courses specified in m_courses.
                log.Debug("Loading lectures data.");
                var enumLecture =
                    from c in m_courses
                    from lec in m_xelDefinitions.Element("lectures").Descendants("lecture")
                    where c.id.Equals(lec.Element("course").Attribute("ref").Value)
                    select lec;
                foreach (XElement lec in enumLecture)
                    m_lectures.Add(new Lecture(lec.Attribute("id").Value, lec.Element("course").Attribute("ref").Value,
                                               lec.Element("practice").Value, lec.Element("tag").Value, lec.Element("duration").Value,
                                               lec.Element("period").Value));
                log.Debug("Loaded" + m_lectures.Count + "lecturers data.");
                //--------------------------------------------------------------------------

                // Init m_lessons. Only lessons with specified time, day and lecture specified in m_lectures. If the lecturer and classroom not given in xml, '0' ids are filed
                log.Debug("Loading lessons data.");
                var lecIds =
                    from lec in m_lectures
                    select lec.id;

                var enumCard =
                    from card in m_xelDefinitions.Element("cards").Descendants("card")
                    where lecIds.Contains(card.Element("lecture").Attribute("ref").Value) &&
                          !card.Element("time").IsEmpty && !card.Element("day").IsEmpty
                    select card;

                string lerId, crId;
                foreach (XElement card in enumCard)
                {
                    if (card.Element("lecturer").HasAttributes)
                        lerId = card.Element("lecturer").Attribute("ref").Value;
                    else
                        lerId = "0";

                    if (card.Elements("classroom").Count() > 0 && card.Element("classroom").HasAttributes)
                        crId = card.Element("classroom").Attribute("ref").Value;
                    else
                        crId = "0";

                    m_lessons.Add(new Lesson(card.Attribute("id").Value, card.Element("lecture").Attribute("ref").Value,
                                            lerId, card.Element("day").Attribute("ref").Value,
                                            card.Element("time").Attribute("ref").Value, crId,
                                            card.Element("tag").Value));
                }
                log.Debug("Loaded" + m_lessons.Count + "lessons data.");
                //--------------------------------------------------------------------------

                //lets get ids of lessons, times, lecturers and classrooms from all lessons
                List<string> lessonIds = new List<string>();
                List<string> timeIds = new List<string>();
                List<string> lecturerIds = new List<string>();
                List<string> classroomIds = new List<string>();

                var lessonPartsIds = //extract ids of lessons, times, lecturers and classrooms
                    from les in m_lessons
                    select new { les.id, les.timeId, les.lecturerId, les.classroomId };

                foreach (var e in lessonPartsIds)
                {
                    lessonIds.Add(e.id);
                    timeIds.Add(e.timeId);
                    lecturerIds.Add(e.lecturerId);
                    classroomIds.Add(e.classroomId);
                }
                //--------------------------------------------------------------------------

                // Init m_lecturers. Only lecturers with at least one lesson
                log.Debug("Loading lecturers data.");
                var enumLecturer =    //only lecturers who give one or more lessons  
                    (
                    from ler in m_xelDefinitions.Element("lecturers").Descendants("lecturer")
                    where lecturerIds.Contains(ler.Attribute("id").Value)
                    select ler
                    ).Distinct();

                foreach (XElement ler in enumLecturer)
                    m_lecturers.Add(new Lecturer(ler.Attribute("id").Value, ler.Element("name").Value,
                                                 ler.Element("forename").Value, ler.Element("department").Attribute("ref").Value));
                m_lecturers.Add(new Lecturer("0", "---", "", "0")); //adds an general null lecturer - need in the JAZ course
                log.Debug("Loaded" + m_lecturers.Count + "lecturers data.");
                //--------------------------------------------------------------------------

                // Init m_times.
                log.Debug("Loading times data.");
                var enumTimes =
                    from el in m_xelDefinitions.Element("times").Descendants("time")
                    where timeIds.Contains(el.Attribute("id").Value)  //get all times when at least on lesson take place
                    orderby Convert.ToInt32(el.Element("timesorder").Value)
                    select el;
                foreach (XElement el in enumTimes)
                    m_times.Add(new Time(el.Attribute("id").Value, el.Element("hours").Value,
                                         el.Element("minutes").Value, el.Element("timesorder").Value));
                log.Debug("Loaded" + m_times.Count + "times data.");
                //--------------------------------------------------------------------------

                // Init m_days.
                log.Debug("Loading days data.");
                var enumDays =
                    from d in m_xelDefinitions.Element("days").Descendants("day")
                    orderby d.Element("daysorder").Value
                    select d;
                foreach (XElement d in enumDays)
                    m_days.Add(new Day(d.Attribute("id").Value, d.Element("czech").Value, d.Element("daysorder").Value));
                log.Debug("Loaded" + m_days.Count + "days data.");
                //--------------------------------------------------------------------------

                // Init m_classrooms.
                log.Debug("Loading classrooms data.");
                var enumClassrooms =
                    from cl in m_xelDefinitions.Element("classrooms").Descendants("classroom")
                    where classroomIds.Contains(cl.Attribute("id").Value)
                    select cl;
                foreach (XElement cl in enumClassrooms)
                    m_classrooms.Add(new Classroom(cl.Attribute("id").Value, cl.Element("name").Value, cl.Element("building").Attribute("ref").Value));
                m_classrooms.Add(new Classroom("0", "---", "0")); //adds an general null classroom
                log.Debug("Loaded " + m_classrooms.Count + " classrooms data.");
                //--------------------------------------------------------------------------

                // Init m_buildings.
                log.Debug("Loading buildings data.");
                var buildingsUsed =
                    (
                    from cl in m_classrooms
                    select cl.buildingId
                    ).Distinct();

                var enumBuildings =
                    from b in m_xelDefinitions.Element("buildings").Descendants("building")
                    where buildingsUsed.Contains(b.Attribute("id").Value)
                    orderby b.Attribute("id").Value
                    select b;
                foreach (XElement b in enumBuildings)
                    m_buildings.Add(new Building(b.Attribute("id").Value, b.Element("name").Value));
                m_buildings.Add(new Building("0", "---")); //adds an general null building
                log.Debug("Loaded  " + m_buildings.Count + "  buildings data.");
                //--------------------------------------------------------------------------

                // Init m_degreeYears and m_specializations.
                log.Debug("Loading 'degreeYears' and specializations data.");
                //get all degrees
                var enumDegrees =
                    from el in m_xelDefinitions.Element("degrees").Descendants("degree")
                    select el;

                //get all groups (=zaměření, specialization)
                var groups =
                    from g in m_xelDefinitions.Element("groups").Descendants("group")
                    select g;

                //get all possible pairs of {degree, year} from groups(=zaměření, specialization)
                var pairs =
                    from g in groups
                    select new { degree = g.Element("degree").Attribute("ref").Value, year = g.Element("schoolyear").Value };

                //get pairs that every pair is only once mentioned. probably 6 (or 5) pairs
                pairs = pairs.Distinct();

                int i = 1;
                foreach (var p in pairs)
                {
                    m_degreeyears.Add(new DegreeYear(i.ToString(),
                        enumDegrees.Single(x => x.Attribute("id").Value.Equals(p.degree)).Element("name").Value + " " + p.year + ". ročník",
                        enumDegrees.Single(x => x.Attribute("id").Value.Equals(p.degree)).Element("acronym").Value + ". " + p.year + "."));
                    var specials =
                        from ak in groups
                        where ak.Element("degree").Attribute("ref").Value == p.degree && ak.Element("schoolyear").Value == p.year
                        select new Specialization(ak.Attribute("id").Value, ak.Element("name").Value, ak.Element("acronym").Value, i.ToString());
                    foreach (var z in specials)
                        m_specializations.Add(z);
                    i++;
                }
                m_degreeyears = m_degreeyears.OrderBy(d => d.acronym).ToList();
                log.Debug("Loaded " + m_degreeyears.Count + " 'degreeYears' and " + m_specializations.Count + " specializations data.");
                //--------------------------------------------------------------------------

                // Init m_groups and m_groupLessonBinder.
                log.Debug("Loading groups and 'groupLessonBinder' data.");
                var enumParts =
                    from el in m_xelDefinitions.Element("parts").Descendants("part")
                    select el;
                int j;
                string idLesson;
                foreach (XElement el in enumParts)
                {
                    m_groups.Add(new Group(el.Attribute("id").Value, el.Element("number").Value, el.Element("group").Attribute("ref").Value));
                    j = 1;
                    foreach (XElement cardEl in el.Descendants("card"))
                    {
                        idLesson = cardEl.Attribute("ref").Value;
                        if (lessonIds.Contains(idLesson))
                            m_groupLessonBinder.Add(new GroupLessonBinder((j++).ToString(), el.Attribute("id").Value, idLesson));
                    }
                }
                m_groups = m_groups.OrderBy(k => k.groupNo).ToList();
                log.Debug("Loaded " + m_groups.Count + " groups and " + m_groupLessonBinder.Count + " 'groupLessonBinder' data.");
                log.Debug("Method exit.");
                return true;
            }

            catch
            {
                log.Error("XML data file parsing error.");
                log.Debug("Method exit.");
                return false;
            }

        }

        /// <summary>
        /// Method filtering specializations (zaměření) by given degreeYears. Specializations are visible just only one degreeYear is selected.
        /// </summary>
        /// <param name="degreeYearIds"></param>
        /// <returns>List of Specializations</returns>
        public List<Specialization> FilterSpecializationsByDegreeYears(List<string> degreeYearIds)
        {
            log.Debug("Method entry.");
            if (anyId(degreeYearIds) && degreeYearIds.Count == 1)
            {
                var filteredSpecializations =
                    from s in m_specializations
                    where degreeYearIds.Contains(s.degreeYearId)
                    orderby s.acronym
                    select s;

                return filteredSpecializations.ToList();
            }
            log.Debug("Method exit.");
            return new List<Specialization>();
        }

        /// <summary>
        /// Method filtering groups (kruhy) by given specializations (zaměření). Groups are visible when just one degreeYear is selected.
        /// Result held in 'Groups' property of Model.
        /// </summary>
        /// <param name="specializationIds"></param>
        /// <returns>List of Groups filtered out.</returns>
        public List<Group> FilterGroupsBySpecializations(List<string> specializationIds)
        {
            log.Debug("Method entry.");
            if (anyId(specializationIds) && specializationIds.Count == 1)
            {
                var filteredGroups =
                   from g in m_groups
                   where specializationIds.Contains(g.specializationId)
                   select g;

                return filteredGroups.ToList();
            }
            log.Debug("Method exit.");
            return new List<Group>();
        }

        /// <summary>
        /// Method filtering lecturers by given departments, where employed. Result held in 'Lecturers' property of Model.
        /// </summary>
        /// <param name="departmentIds">Ids of the given departments</param>
        /// <returns>List of Lecturers filtered out.</returns>
        public List<Lecturer> FilterLecturersByDepartments(List<string> departmentIds)
        {
            log.Debug("Method entry.");
            if (anyId(departmentIds))
            {
                var filteredLecturersByDepartments =
                    from l in m_lecturers
                    where departmentIds.Contains(l.departmentId)
                    orderby l.name
                    select l;



                return filteredLecturersByDepartments.ToList();
            }
            log.Debug("Method exit.");
            return new List<Lecturer>();
        }

        /// <summary>
        /// Method filtering classrooms by given buildings. Result held in 'Classrooms' property of Model.
        /// </summary>
        /// <param name="buildingIds">Ids of the given buildings</param>
        /// <returns>List of Classrooms filtered out.</returns>
        public List<Classroom> FilterClassroomsByBuildings(List<string> buildingIds)
        {
            log.Debug("Method entry.");
            if (anyId(buildingIds))
            {
                var filteredClassrooms =
                    from c in m_classrooms
                    where buildingIds.Contains(c.buildingId)
                    orderby c.name
                    select c;

                return filteredClassrooms.ToList();
            }
            log.Debug("Method exit.");
            return new List<Classroom>();
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
        /// <returns>List of TimetableFields filtered out.</returns>
        public List<TimetableField> FilterTimetableFieldsByAll(List<string> degreeYearIds, List<string> specializationIds, List<string> groupIds,
                                               List<string> departmentIds, List<string> lecturerIds, List<string> buildingIds,
                                               List<string> classroomIds, List<string> dayIds, List<string> timeIds, string searchedString)
        {
            log.Debug("Method entry.");
            var lessonsFromAllFilters = new List<IEnumerable<Lesson>>();

            //by groups, specializations, degreeYears
            if (anyId(groupIds)) //if there is some group selected, filter by groups
                filterLessonsByGroups(groupIds, lessonsFromAllFilters);
            else
                if (anyId(specializationIds)) //when there is no group selected, try to filter by a specialization
                    filterLessonsBySpecializations(specializationIds, lessonsFromAllFilters);
                else
                    filterLessonsByDegreeYears(degreeYearIds, lessonsFromAllFilters); //if no group selected and no specialization selected, try filter by degreeYear

            //by lecturers,  departments
            if (anyId(lecturerIds)) //allows lessons of other depertments which the lecturer is member of, but given by the lecturer
                filterLessonsByLecturers(lecturerIds, lessonsFromAllFilters);
            else
                filterLessonsByDepartments(departmentIds, lessonsFromAllFilters);


            //by classrooms, buildings
            if (anyId(classroomIds))
                filterLessonsByClassrooms(classroomIds, lessonsFromAllFilters);
            else
                filterLessonsByBuildings(buildingIds, lessonsFromAllFilters);

            //by days
            filterLessonsByDays(dayIds, lessonsFromAllFilters);

            //by times
            filterLessonsByTimes(timeIds, lessonsFromAllFilters);

            //by search string
            filterLessonsBySearchString(searchedString, lessonsFromAllFilters);

            var resultLessons = intersect(lessonsFromAllFilters);

            var filteredTimetableFields =
                from h in resultLessons
                join lec in m_lectures on h.lectureId equals lec.id
                join c in m_courses on lec.courseId equals c.id
                join dep in m_departments on c.departmentId equals dep.id
                join ler in m_lecturers on h.lecturerId equals ler.id
                join d in m_days on h.dayId equals d.id
                join t in m_times on h.timeId equals t.id
                join cr in m_classrooms on h.classroomId equals cr.id
                join b in m_buildings on cr.buildingId equals b.id
                orderby dep.code, c.acronym, lec.practice, ler.name, d.daysOrder, t.timesOrder, b.name, cr.name
                select new TimetableField(dep, c, lec, ler, d, t, b, cr, m_timetableInfo);

            log.Debug("Method exit.");
            return filteredTimetableFields.ToList();
        }

        /// <summary>
        /// Method filtering lessons by given degreeYear.
        /// </summary>
        /// <param name="degreeYearIds">Ids of the given degreeYears</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByDegreeYears(List<string> degreeYearIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(degreeYearIds))
            {
                var specializationIdsByDegreeYear =
                    from s in m_specializations
                    where degreeYearIds.Contains(s.degreeYearId)
                    select s.id;

                filterLessonsBySpecializations(specializationIdsByDegreeYear.ToList(), lessonsFromAllFilters);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given specializations.
        /// </summary>
        /// <param name="specializationIds">Ids of the given specializations</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsBySpecializations(List<string> specializationIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(specializationIds))
            {
                var groupIdsBySpecializations =
                    from g in m_groups
                    where specializationIds.Contains(g.specializationId)
                    select g.id;

                filterLessonsByGroups(groupIdsBySpecializations.ToList(), lessonsFromAllFilters);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given groups.
        /// </summary>
        /// <param name="groupIds">Ids of the given groups.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByGroups(List<string> groupIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(groupIds))
            {
                var lessonsFilteredByGroups =
                    (
                     from hk in m_groupLessonBinder
                     where groupIds.Contains(hk.groupId)
                     from h in m_lessons
                     where hk.lessonId == h.id
                     select h
                     ).Distinct();
                lessonsFromAllFilters.Add(lessonsFilteredByGroups);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given departments.
        /// </summary>
        /// <param name="departmentIds">Ids of the given departments.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByDepartments(List<string> departmentIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(departmentIds))
            {
                var lessonsFilteredByDepartments =
                     from c in m_courses
                     where departmentIds.Contains(c.departmentId)
                     from l in m_lectures
                     where c.id == l.courseId
                     from h in m_lessons
                     where l.id == h.lectureId
                     select h;
                lessonsFromAllFilters.Add(lessonsFilteredByDepartments);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given lecturers.
        /// </summary>
        /// <param name="lecturerIds">Ids of the given lecturers.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByLecturers(List<string> lecturerIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(lecturerIds))
            {
                var lessonsFilteredByLecturers =
                     from h in m_lessons
                     where lecturerIds.Contains(h.lecturerId)
                     select h;
                lessonsFromAllFilters.Add(lessonsFilteredByLecturers);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given buildings, used only when no classroom is selected.
        /// </summary>
        /// <param name="buildingIds">Ids of the given buildings.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByBuildings(List<string> buildingIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(buildingIds))
            {
                var classroomIdsFilteredByBuildings =
                    from cr in m_classrooms
                    where buildingIds.Contains(cr.buildingId)
                    select cr.id;

                filterLessonsByClassrooms(classroomIdsFilteredByBuildings.ToList(), lessonsFromAllFilters);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given classrooms.
        /// </summary>
        /// <param name="classroomIds">Ids of the given classrooms.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByClassrooms(List<string> classroomIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(classroomIds))
            {
                var lessonsFilteredByClassrooms =
                     from h in m_lessons
                     where classroomIds.Contains(h.classroomId)
                     select h;
                lessonsFromAllFilters.Add(lessonsFilteredByClassrooms);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given days.
        /// </summary>
        /// <param name="dayIds">Ids of the given days.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByDays(List<string> dayIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(dayIds))
            {
                var lessonsFilteredByDays =
                     from h in m_lessons
                     where dayIds.Contains(h.dayId)
                     select h;
                lessonsFromAllFilters.Add(lessonsFilteredByDays);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given times.
        /// </summary>
        /// <param name="timeIds">Ids of the given times.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByTimes(List<string> timeIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(timeIds))
            {
                var lessonsFilteredByTime =
                     from h in m_lessons
                     where timeIds.Contains(h.timeId)
                     select h;
                lessonsFromAllFilters.Add(lessonsFilteredByTime);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given lectures.
        /// </summary>
        /// <param name="lectureIds">Ids of the given lectures.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByLectures(List<string> lectureIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(lectureIds))
            {
                var lessonsFilteredByLecture =
                     from l in m_lessons
                     where lectureIds.Contains(l.lectureId)
                     select l;
                lessonsFromAllFilters.Add(lessonsFilteredByLecture);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given courses.
        /// </summary>
        /// <param name="courseIds">Ids of the given courses.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByCourses(List<string> courseIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (anyId(courseIds))
            {
                var lectureIdsFilteredByCourses =
                    from lecture in m_lectures
                    where courseIds.Contains(lecture.courseId)
                    select lecture.id;

                filterLessonsByLectures(lectureIdsFilteredByCourses.ToList(), lessonsFromAllFilters);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Method filtering lessons by given search string representing acronym or name of a course.
        /// </summary>
        /// <param name="searchString">Search string representing acronym or name of a course.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsBySearchString(string searchString, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            log.Debug("Method entry.");
            if (searchString.Length > 0)
            {
                var courseIdsFilteredBySearchString =
                    from c in m_courses
                    where RemoveDiacriticsAndLower(c.acronym).Contains(RemoveFirstTwoDigits(RemoveDiacriticsAndLower(searchString))) || RemoveDiacriticsAndLower(c.name).Contains(RemoveDiacriticsAndLower(searchString))
                    select c.id;

                filterLessonsByCourses(courseIdsFilteredBySearchString.ToList(), lessonsFromAllFilters);
            }
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Makes set intersection of the given lists of lessons.
        /// </summary>
        /// <param name="lessonsFromAllFilters">"Lists of lessons in one list."</param>
        /// <returns>List of common lessons.</returns>
        private static IEnumerable<Lesson> intersect(IEnumerable<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (lessonsFromAllFilters.Any())
                return lessonsFromAllFilters.Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToList());
            else
                return new List<Lesson>();
        }

        /// <summary>
        ///Removes diacritics and gets the characters to lower case.
        /// </summary>
        /// <param name="s">Input string</param>
        /// <returns>Resulting string</returns>
        private static string RemoveDiacriticsAndLower(string s)
        {
            // oddělení znaků od modifikátorů (háčků, čárek, atd.)
            s = s.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                // do řetězce přidá všechny znaky kromě modifikátorů
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(s[i]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[i]);
                }
            }

            // vrátí řetězec bez diakritiky
            return sb.ToString().ToLower();
        }

        /// <summary>
        /// Removes first two digits of the given string.
        /// </summary>
        /// <param name="s">Input string</param>
        /// <returns>Resulting string</returns>
        private static string RemoveFirstTwoDigits(string s)
        {
            string trimmed = s.TrimStart('0','1','2','3','4','5','6','7','8','9');
            return trimmed;
        }

        /// <summary>
        /// Return true when there is any id, false for empty list.
        /// </summary>
        /// <param name="ids">List of ids</param>
        /// <returns>true when nonempty, false otherwise</returns>
        private static bool anyId(List<string> ids)
        {
            if (ids != null && ids.Count > 0)
                return true;
            return false;
        }
    }
}