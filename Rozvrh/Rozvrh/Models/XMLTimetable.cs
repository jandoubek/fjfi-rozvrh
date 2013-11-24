using System;
using System.Collections.Generic;
using System.Linq;
using Rozvrh.Models.Timetable;
using System.Xml.Linq;
using System.IO;


//FIX ME !!! Richard: Needs to be abstracted to an interface becouse of unit testing.

namespace Rozvrh.Models
{
    /// <summary>
    /// Class providing data load from xml file and storing of the base data after aplication start. This method is implemented as singleton.
    /// </summary>
    public class XMLTimetable
    {

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<XMLTimetable> _instance = new Lazy<XMLTimetable>(() => new XMLTimetable());

        // accessor for instance
        public static XMLTimetable Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private XMLTimetable() {
            Refresh(Config.Instance.XMLTimetableFilePath);
        }


        /// <summary>
        /// Reloads xml file, parses and initializes properties which holds base data of the application. This method should be used for refreshing the model after updating thxml with timetable.
        /// </summary>
        /// <param name="xmlPath">Path to the source xml file.</param>
        public void Refresh(string xmlPath)
        {
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

            this.LoadXml();
            this.InitData();
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
        private void LoadXml()
        {
            try
            {
                //LOG Console.WriteLine("RozvrhDataProvider::LoadXML: pouším se načíst XML soubor '{0}' ...", m_rozvhXmlFilePath);
                m_xelDefinitions = XElement.Load(m_dataXmlFilePath);
            }
            catch (IOException e)
            {
                throw new Exception("Unable to load and parse the xml data file.");
                //LOG Console.WriteLine("RozvrhDataProvider::LoadXML: CHYBA - nepovedlo se načíst XML soubor: {0}", m_rozvhXmlFilePath);
            }
            //LOG Console.WriteLine("RozvrhDataProvider::LoadXML: XML soubor '{0}' načten", m_rozvhXmlFilePath);
        }

        /// <summary>
        /// Init class properties by xml data.
        /// </summary>
        private void InitData()
        {
            try
            {
                //LOG Console.WriteLine("RozvrhDataProvider::InitData: pokouším se načíst data...");

                //must be called in this order
                initDepartments();
                initCourses();
                initLectures();
                initLessons();
                initLecturers();
                initDays();
                initTimes();
                initBuildings();
                initClassrooms();
                initDegreeYearAndSpecialization();
                initGroupsAndGroupLessonBinders();
            }
            catch (IOException e)
            {
                //LOG Console.WriteLine("RozvrhDataProvider::InitData: CHYBA - nepovedlo se načíst data do modelu");
                throw new Exception("XML data file parsing error.");
            }
            //LOG Console.WriteLine("RozvrhDataProvider::InitData: data načtena");
        }

        /// <summary>
        /// Init m_departments. Only depertments with name filled in the xml. e.g. departments with acronym "REZERVA", a "PROPAGACE" are ommited.
        /// </summary>
        private void initDepartments()
        {
            var enumDepartment =
                from dep in m_xelDefinitions.Element("departments").Descendants("department")
                where !dep.Element("name").IsEmpty
                orderby dep.Element("code").Value
                select dep;
            foreach (XElement dep in enumDepartment)
                m_departments.Add(new Department(dep.Attribute("id").Value, dep.Element("code").Value, dep.Element("name").Value, dep.Element("acronym").Value, dep.Element("color").Value));
        }

        /// <summary>
        /// Init m_courses. Only courses of the departments specified in m_departments.
        /// </summary>
        private void initCourses()
        {
            var enumCourse =
                    from dep in m_departments
                    from c in m_xelDefinitions.Element("courses").Descendants("course")
                    where dep.id.Equals(c.Element("department").Attribute("ref").Value)
                    select c;
            foreach (XElement c in enumCourse)
                m_courses.Add(new Course(c.Attribute("id").Value, c.Element("department").Attribute("ref").Value, c.Element("name").Value, c.Element("acronym").Value));
        }

        /// <summary>
        /// Init m_lectures. Only lectures of the courses specified in m_courses.
        /// </summary>
        private void initLectures()
        {
            var enumLecture =
                from c in m_courses
                from lec in m_xelDefinitions.Element("lectures").Descendants("lecture")
                where c.id.Equals(lec.Element("course").Attribute("ref").Value)
                select lec;
            foreach (XElement lec in enumLecture)
                m_lectures.Add(new Lecture(lec.Attribute("id").Value, lec.Element("course").Attribute("ref").Value,
                                           lec.Element("practice").Value, lec.Element("tag").Value, lec.Element("duration").Value,
                                           lec.Element("period").Value));
        }

        /// <summary>
        /// Init m_lessons. Only lessons with specified time, day and lecture specified in m_lectures. If the lecturer and classroom not given in xml, '0' ids are filed
        /// </summary>
        private void initLessons()
        {
            var lecIds = from lec in m_lectures
                         select lec.id;

            var enumEl =
                from el in m_xelDefinitions.Element("cards").Descendants("card")
                where lecIds.Contains(el.Element("lecture").Attribute("ref").Value) &&
                      !el.Element("time").IsEmpty && !el.Element("day").IsEmpty
                select el;

            string lerId, crId;
            foreach (XElement el in enumEl)
            {
                if (el.Element("lecturer").HasAttributes)
                    lerId = el.Element("lecturer").Attribute("ref").Value;
                else
                    lerId = "0";

                if (el.Elements("classroom").Count() > 0 && el.Element("classroom").HasAttributes)
                    crId = el.Element("classroom").Attribute("ref").Value;
                else
                    crId = "0";

                m_lessons.Add(new Lesson(el.Attribute("id").Value, el.Element("lecture").Attribute("ref").Value,
                                        lerId, el.Element("day").Attribute("ref").Value,
                                        el.Element("time").Attribute("ref").Value, crId,
                                        el.Element("tag").Value));
            }
        }

        /// <summary>
        /// Init m_lecturers. Only lecturers of the departments specified in m_departments.
        /// </summary>
        private void initLecturers()
        {
            var enumLecturerDep =      //all from departments in m_departments          
               (
                from d in m_departments
                from ler in m_xelDefinitions.Element("lecturers").Descendants("lecturer")
                where d.id == ler.Element("department").Attribute("ref").Value
                select ler
               ).Distinct();

            var enumLecturerTeaching =    //only lecturers who give one or more lessons  
                (
                from ler in enumLecturerDep
                from les in m_lessons
                where ler.Attribute("id").Value == les.lecturerId
                select ler
                ).Distinct();

            foreach (XElement ler in enumLecturerTeaching)
                m_lecturers.Add(new Lecturer(ler.Attribute("id").Value, ler.Element("name").Value,
                                             ler.Element("forename").Value, ler.Element("department").Attribute("ref").Value));
            m_lecturers.Add(new Lecturer("0", "", "", "0")); //adds an general null lecturer - need in the JAZ course
        }

        /// <summary>
        /// Init m_days.
        /// </summary>
        private void initDays()
        {
            var enumEl =
              from d in m_xelDefinitions.Element("days").Descendants("day")
              orderby d.Element("daysorder").Value
              select d;
            foreach (XElement d in enumEl)
                m_days.Add(new Day(d.Attribute("id").Value, d.Element("czech").Value, d.Element("daysorder").Value));
        }

        /// <summary>
        /// Init m_times.
        /// </summary>
        private void initTimes()
        {
            var enumEl =
                  from el in m_xelDefinitions.Element("times").Descendants("time")
                  orderby Convert.ToInt32(el.Element("timesorder").Value)
                  select el;
            foreach (XElement el in enumEl)
                m_times.Add(new Time(el.Attribute("id").Value, el.Element("hours").Value,
                                     el.Element("minutes").Value, el.Element("timesorder").Value));
        }

        /// <summary>
        /// Init m_buildings.
        /// </summary>
        private void initBuildings()
        {
            var enumEl =
                  from el in m_xelDefinitions.Element("buildings").Descendants("building")
                  orderby el.Attribute("id").Value
                  select el;
            foreach (XElement el in enumEl)
                m_buildings.Add(new Building(el.Attribute("id").Value, el.Element("name").Value));
        }

        /// <summary>
        /// Init m_classrooms.
        /// </summary>
        private void initClassrooms()
        {
            var enumEl =
                    from el in m_xelDefinitions.Element("classrooms").Descendants("classroom")
                    select el;
            foreach (XElement el in enumEl)
                m_classrooms.Add(new Classroom(el.Attribute("id").Value, el.Element("name").Value, el.Element("building").Attribute("ref").Value));
        }

        /// <summary>
        /// Init m_degreeYears and m_specializations.
        /// </summary>
        private void initDegreeYearAndSpecialization()
        {
            //get all degrees
            var enumEl =
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
                    enumEl.Single(x => x.Attribute("id").Value.Equals(p.degree)).Element("name").Value + " " + p.year + ". ročník",
                    enumEl.Single(x => x.Attribute("id").Value.Equals(p.degree)).Element("acronym").Value + ". " + p.year + "."));
                var specials =
                    from ak in groups
                    where ak.Element("degree").Attribute("ref").Value == p.degree && ak.Element("schoolyear").Value == p.year
                    select new Specialization(ak.Attribute("id").Value, ak.Element("name").Value, ak.Element("acronym").Value, i.ToString());
                foreach (var z in specials)
                    m_specializations.Add(z);
                i++;
            }
            m_degreeyears = m_degreeyears.OrderBy(d => d.acronym).ToList();
        }

        /// <summary>
        /// Init m_groups and m_groupLessonBinder.
        /// </summary>
        private void initGroupsAndGroupLessonBinders()
        {
            var idsLessons =
                from h in m_lessons
                select h.id;

            var enumEl =
                from el in m_xelDefinitions.Element("parts").Descendants("part")
                select el;
            int j;
            string idLesson;
            foreach (XElement el in enumEl)
            {
                m_groups.Add(new Group(el.Attribute("id").Value, el.Element("number").Value, el.Element("group").Attribute("ref").Value));
                j = 1;
                foreach (XElement cardEl in el.Descendants("card"))
                {
                    idLesson = cardEl.Attribute("ref").Value;
                    if (idsLessons.Contains(idLesson))
                        m_groupLessonBinder.Add(new GroupLessonBinder((j++).ToString(), el.Attribute("id").Value, idLesson));
                }
            }
            m_groups = m_groups.OrderBy(k => k.groupNo).ToList();
        }

    }
}