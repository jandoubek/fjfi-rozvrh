using System;
using System.Collections.Generic;
using System.Linq;
using Rozvrh.Models.Timetable;
using System.Xml.Linq;
using System.IO;

namespace Rozvrh.Models
{
    /// <summary>
    /// Class providing data load from xml file and storing of the base data after aplication start. This method is implemented as singleton.
    /// </summary>
    public class XMLTimetable : IXMLTimetable
    {

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<XMLTimetable> _instance = new Lazy<XMLTimetable>(() => new XMLTimetable());
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///  Accessor for instance.
        /// </summary>
        public static XMLTimetable Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private XMLTimetable()
        {
            Refresh(Config.Instance.XMLTimetableFilePath);
        }


        /// <summary>
        /// Reloads xml file, parses and initializes properties which holds base data of the application. This method should be used for refreshing the model after updating thxml with timetable.
        /// </summary>
        /// <param name="xmlPath">Path to the source xml file.</param>
        public bool Refresh(string xmlPath)
        {
            log.Debug("Method entry.");
            m_dataXmlFilePath = xmlPath;

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

            if (!this.LoadXml())
            {
                log.Debug("Method exit.");
                return false;
            }
            if (!this.InitData())
            {
                log.Debug("Method exit.");
                return false;
            }
            log.Debug("Method exit.");
            return true;
        }

        /// <summary>
        /// Holds path to the data source file.
        /// </summary>
        private string m_dataXmlFilePath;

        /// <summary>
        /// Holds parsed xml document. For internal use.
        /// </summary>
        private XElement m_xelDefinitions;

        /// <summary>
        /// Holds relevant departments from the xml. Only depertments with specified name.
        /// </summary>
        public List<Department> m_departments { get; private set; }

        /// <summary>
        /// Holds relevant courses from the xml. Only courses of departments specified in m_departments.
        /// </summary>
        public List<Course> m_courses { get; private set; }

        /// <summary>
        /// Holds relevant lectures from the xml. Only lectures of courses specified in m_courses.
        /// </summary>
        public List<Lecture> m_lectures { get; private set; }

        /// <summary>
        /// Holds relevant lecturers from the xml. Only lecturers of departments specified in m_departments.
        /// </summary>
        public List<Lecturer> m_lecturers { get; private set; }

        /// <summary>
        /// Holds days from the xml.
        /// </summary>
        public List<Day> m_days { get; private set; }

        /// <summary>
        /// Holds times from the xml.
        /// </summary>
        public List<Time> m_times { get; private set; }

        /// <summary>
        /// Holds buildings from the xml.
        /// </summary>
        public List<Building> m_buildings { get; private set; }

        /// <summary>
        /// Holds classrooms from the xml.
        /// </summary>
        public List<Classroom> m_classrooms { get; private set; }

        /// <summary>
        /// Holds relevant lessons from the xml. Only lessons with specified time, day and lecture specified in m_lectures.
        /// </summary>
        public List<Lesson> m_lessons { get; private set; }

        /// <summary>
        /// Holds relevant degree and year info. Extracted from 'degrees' and 'groups' elements.
        /// </summary>
        public List<DegreeYear> m_degreeyears { get; private set; }

        /// <summary>
        /// Holds specializations from the xml. Extracted from the 'groups' element.
        /// </summary>
        public List<Specialization> m_specializations { get; private set; }

        /// <summary>
        /// Holds relevant groups (kruhy) from the xml. Extracted from the 'parts' element.
        /// </summary>
        public List<Group> m_groups { get; private set; }

        /// <summary>
        /// Holds relevant pairs of group and lesson from the xml. Extracted from cards element.
        /// </summary>
        public List<GroupLessonBinder> m_groupLessonBinder { get; private set; }

        /// <summary>
        /// Method which open and parse the xml file.
        /// </summary>
        /// <returns>Bool result.</returns>
        private bool LoadXml()
        {
            log.Debug("Method entry.");
            try
            {
                log.Debug("Trying to load XML database file: '" + m_dataXmlFilePath + "'.");

                //if the path starts with ~ - use local server loading
                if (String.CompareOrdinal("~", 0, m_dataXmlFilePath, 0, 1) == 0)
                {
                    log.Info("Loading XML file from a server map path: '" + m_dataXmlFilePath + "'.");
                    m_xelDefinitions = XElement.Load(System.Web.HttpContext.Current.Server.MapPath(m_dataXmlFilePath));
                }
                else
                {
                    log.Info("Loading XML file from an absolute path (like http): '" + m_dataXmlFilePath + "'.");
                    m_xelDefinitions = XElement.Load(m_dataXmlFilePath);
                }
                log.Info("XML loaded.");
                log.Debug("Method exit.");
                return true;
            }
            catch
            {
                log.Error("Unable to load and parse the XML data file: '" + m_dataXmlFilePath + "'.");
                log.Debug("Method exit.");
                return false;
            }
        }


        /// <summary>
        /// Init class properties by xml data.
        /// </summary>
        /// <returns>Bool result.</returns>
        private bool InitData()
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

    }
}