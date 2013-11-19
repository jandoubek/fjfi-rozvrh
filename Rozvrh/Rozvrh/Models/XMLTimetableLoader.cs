using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rozvrh.Models.Timetable;
using System.Xml.Linq;
using System.IO;

namespace Rozvrh.Models
{
    public class XMLTimetableLoader
    {
        public XMLTimetableLoader(string xmlPath)
        {
            //cesta k xml souboru s podkladovými daty
            m_rozvhXmlFilePath = xmlPath;

            //model data - všechny courses, lectures, ...
            m_courses = new List<Course>();
            m_lectures = new List<Lecture>();
            m_lecturers = new List<Lecturer>();
            m_days = new List<Day>();
            m_times = new List<Time>();
            m_buildings = new List<Building>();
            m_classrooms = new List<Classroom>();
            m_degreeyears = new List<DegreeYear>();
            m_zamerenis = new List<Zamereni>();
            m_hodiny = new List<Hodina>();
            m_kruhy = new List<Kruh>();
            m_hodinyKruhu = new List<HodinyKruhu>();

            // FIX ME !!! - proper exception handling in LoadXML and InitModelData()
            if (!this.LoadXml() || !this.InitModelData()) {
                throw new Exception("Timetable XML could not be parsed.");
            }
        }

        private string m_rozvhXmlFilePath;
        private XElement m_xelDefinitions;

        //model data - všechna data v modelu. courses, lectures, ...
        public List<Department> m_departments { get; private set; }
        public List<Course> m_courses { get; private set; }
        public List<Lecture> m_lectures { get; private set; }
        public List<Lecturer> m_lecturers { get; private set; }
        public List<Day> m_days { get; private set; }
        public List<Time> m_times { get; private set; }
        public List<Building> m_buildings { get; private set; }
        public List<Classroom> m_classrooms { get; private set; }
        public List<Hodina> m_hodiny { get; private set; }
        public List<DegreeYear> m_degreeyears { get; private set; }
        public List<Zamereni> m_zamerenis { get; private set; }
        public List<Kruh> m_kruhy { get; private set; }
        public List<HodinyKruhu> m_hodinyKruhu { get; private set; }

        public bool LoadXml()
        {
            try
            {
                Console.WriteLine("RozvrhDataProvider::LoadXML: pouším se načíst XML soubor '{0}' ...", m_rozvhXmlFilePath);
                m_xelDefinitions = XElement.Load(m_rozvhXmlFilePath);
            }
            catch (IOException e)
            {
                Console.WriteLine("RozvrhDataProvider::LoadXML: CHYBA - nepovedlo se načíst XML soubor: {0}", m_rozvhXmlFilePath);
                return false;
            }
            Console.WriteLine("RozvrhDataProvider::LoadXML: XML soubor '{0}' načten", m_rozvhXmlFilePath);
            return true;
        }

        public bool InitModelData()
        {
            try
            {
                Console.WriteLine("RozvrhDataProvider::InitData: pokouším se načíst data...");
                //init Departments
                //!!! vyhodí katedry, které nemají jméno (tzn. s acronymem "REZERVA", a "PROPAGACE")
                m_departments = new List<Department>();
                var enumDepartment =
                    from dep in m_xelDefinitions.Element("departments").Descendants("department")
                    where !dep.Element("name").IsEmpty  //vybere jen katedry se skutečným jménem
                    orderby dep.Element("code").Value
                    select dep;
                foreach (XElement dep in enumDepartment)
                    m_departments.Add(new Department(Convert.ToInt32(dep.Attribute("id").Value), Convert.ToInt32(dep.Element("code").Value), dep.Element("name").Value, dep.Element("acronym").Value, Convert.ToUInt64(dep.Element("color").Value)));

                //init Courses
                //!!! vyhodí předměty, které jsou jen z relevantních (těch co už jsou načtené) kateder               
                var enumCourse =
                    from dep in m_departments
                    from c in m_xelDefinitions.Element("courses").Descendants("course")
                    where  dep.id == (int)c.Element("department").Attribute("ref")
                    select c;
                foreach (XElement c in enumCourse)
                    m_courses.Add(new Course(Convert.ToInt32(c.Attribute("id").Value), Convert.ToInt32(c.Element("department").Attribute("ref").Value), c.Element("name").Value, c.Element("acronym").Value));

                //init Lectures
                //!!! načte lekce, které jsou jen z relevantních (těch co už jsou načtené) kurzů                
                var enumLecture =
                    from c in m_courses
                    from lec in m_xelDefinitions.Element("lectures").Descendants("lecture")
                    where c.id == (int)lec.Element("course").Attribute("ref")
                    select lec;
                foreach (XElement lec in enumLecture)
                    m_lectures.Add(new Lecture(Convert.ToInt32(lec.Attribute("id").Value), Convert.ToInt32(lec.Element("course").Attribute("ref").Value),
                                               Convert.ToInt32(lec.Element("practice").Value), lec.Element("tag").Value, Convert.ToInt32(lec.Element("duration").Value),
                                               Convert.ToInt32(lec.Element("period").Value)));

                //init Lecturers
                //!!!vybere vyučující jen z relevantních kateder 
                var enumLecturer =
                    from c in m_courses
                    from ler in m_xelDefinitions.Element("lecturers").Descendants("lecturer")
                    where c.id == (int)ler.Element("department").Attribute("ref")  //vybere vyučující ze skutečných kateder
                    select ler;
                foreach (XElement ler in enumLecturer)
                    m_lecturers.Add(new Lecturer(Convert.ToInt32(ler.Attribute("id").Value), ler.Element("name").Value,
                                                 ler.Element("forename").Value, Convert.ToInt32(ler.Element("department").Attribute("ref").Value)));
                m_lecturers.Add(new Lecturer(0, "", "", 0));

                //init Days
                IEnumerable<XElement> enumEl =
                  from d in m_xelDefinitions.Element("days").Descendants("day")
                  orderby d.Element("daysorder").Value
                  select d;
                foreach (XElement d in enumEl)
                    m_days.Add(new Day(Convert.ToInt32(d.Attribute("id").Value), d.Element("czech").Value, Convert.ToInt32(d.Element("daysorder").Value)));

                //init Times                
                enumEl =
                  from el in m_xelDefinitions.Element("times").Descendants("time")
                  orderby Convert.ToInt32(el.Element("timesorder").Value)
                  select el;
                foreach (XElement el in enumEl)
                    m_times.Add(new Time(Convert.ToInt32(el.Attribute("id").Value), Convert.ToInt32(el.Element("hours").Value),
                                         Convert.ToInt32(el.Element("minutes").Value), Convert.ToInt32(el.Element("timesorder").Value)));

                //init Buildings              
                enumEl =
                  from el in m_xelDefinitions.Element("buildings").Descendants("building")
                  orderby el.Attribute("id").Value
                  select el;
                foreach (XElement el in enumEl)
                    m_buildings.Add(new Building(Convert.ToInt32(el.Attribute("id").Value), el.Element("name").Value));

                //init Classrooms
                enumEl =
                    from el in m_xelDefinitions.Element("classrooms").Descendants("classroom")
                    select el;
                foreach (XElement el in enumEl)
                    m_classrooms.Add(new Classroom(Convert.ToInt32(el.Attribute("id").Value), el.Element("name").Value, Convert.ToInt32(el.Element("building").Attribute("ref").Value)));

                //init DegreeYear               
                enumEl =
                    from el in m_xelDefinitions.Element("degrees").Descendants("degree")
                    select el;

                //vyber groups
                var groups =
                    from g in m_xelDefinitions.Element("groups").Descendants("group")
                    select g;

                var pairs =
                    from g in groups
                    select new { degree = g.Element("degree").Attribute("ref").Value, year = g.Element("schoolyear").Value };
                pairs = pairs.Distinct();
                int i = 1;
                foreach (var p in pairs)
                {
                    m_degreeyears.Add(new DegreeYear(i,
                        enumEl.Single(x => (int?)x.Attribute("id") == Convert.ToInt32(p.degree)).Element("name").Value + " " + p.year + ". ročník",
                        enumEl.Single(x => (int?)x.Attribute("id") == Convert.ToInt32(p.degree)).Element("acronym").Value + ". " + p.year + "."));
                    var zamereni =
                        from ak in groups
                        where ak.Element("degree").Attribute("ref").Value == p.degree && ak.Element("schoolyear").Value == p.year
                        select new Zamereni(Convert.ToInt32(ak.Attribute("id").Value), ak.Element("name").Value, ak.Element("acronym").Value, i);
                    foreach (var z in zamereni)
                        m_zamerenis.Add(z);
                    i++;
                }
                m_degreeyears = m_degreeyears.OrderBy(d => d.acronym).ToList();  //seřadit, aby s tim pak nebyla práce;

                //init Hodiny
                // pokud "lecturer" nebo "Classroom" "ref" není vyplněno
                //!!!vybere hodiny kde je vyplněn čas, den a má relevantní odkaz na lekci
                var lecIds = from lec in m_lectures
                             select lec.id;

                enumEl =
                    from el in m_xelDefinitions.Element("cards").Descendants("card")
                    where lecIds.Contains((int)el.Element("lecture").Attribute("ref")) &&  //vybere vyučující lekci ze skutečných kateder
                          !el.Element("time").IsEmpty && !el.Element("day").IsEmpty
                    select el;
                int lerId, crId;
                foreach (XElement el in enumEl)
                {
                    if (el.Element("lecturer").HasAttributes)
                        lerId = Convert.ToInt32(el.Element("lecturer").Attribute("ref").Value);
                    else
                        lerId = 0;

                    if (el.Elements("classroom").Count() > 0 && el.Element("classroom").HasAttributes)
                        crId = Convert.ToInt32(el.Element("classroom").Attribute("ref").Value);
                    else
                        crId = 0;

                    m_hodiny.Add(new Hodina(Convert.ToInt32(el.Attribute("id").Value), Convert.ToInt32(el.Element("lecture").Attribute("ref").Value),
                                            lerId, Convert.ToInt32(el.Element("day").Attribute("ref").Value),
                                            Convert.ToInt32(el.Element("time").Attribute("ref").Value), crId,
                                            el.Element("tag").Value));
                }

                //init Kruhy a HodinyKruhu
                var idsHodin =
                    from h in m_hodiny
                    select h.id;

                enumEl =
                    from el in m_xelDefinitions.Element("parts").Descendants("part")
                    select el;
                int j, idHodiny;
                foreach (XElement el in enumEl)
                {
                    m_kruhy.Add(new Kruh(Convert.ToInt32(el.Attribute("id").Value), Convert.ToInt32(el.Element("number").Value), Convert.ToInt32(el.Element("group").Attribute("ref").Value)));
                    j = 1;
                    foreach (XElement cardEl in el.Descendants("card"))
                    {
                        idHodiny = Convert.ToInt32(cardEl.Attribute("ref").Value);
                        if (idsHodin.Contains(idHodiny))
                            m_hodinyKruhu.Add(new HodinyKruhu(j++, Convert.ToInt32(el.Attribute("id").Value), idHodiny));
                    }
                }
                m_kruhy = m_kruhy.OrderBy(k => k.cisloKruhu).ToList();  //seřadit, aby s tim pak nebyla práce;


            }
            catch (IOException e)
            {
                Console.WriteLine("RozvrhDataProvider::InitData: CHYBA - nepovedlo se načíst data do modelu");
                return false;
            }
            Console.WriteLine("RozvrhDataProvider::InitData: data načtena");
            return true;
        }
    }
}