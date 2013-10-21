using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;
using System.Threading;
using System.Reflection;
using System.IO.Packaging;

namespace TestParsing
{
    //bude sloužit pro přenos do .ical, .pdf exportu a na grafický výstup rozvrhu na obrazovku kam přijde List<Policko>
    public class Policko
    {
        string predmet;
        string vyucujici;
        string misto;
        ulong barva;
        int cas;
        int den;
        int delka;
        int perioda;
    }

    //chybí doplnit některé objekty pro zobrazení filtrovací sekce např. čísla kruhů 

    public class Department
    {
        public int id { get; set; }
        public int code { get; set; }
        public string name { get; set; }
        public string acronym { get; set; }
        public ulong color { get; set; }
        public Department(int id, int code, string name, string acronym, ulong color)
        {
            this.id = id;
            this.code = code;
            this.name = name;
            this.acronym = acronym;
            this.color = color;
        }
    }//= department v xml
    public class Course
    {
        public int id { get; set; }
        public int departmentId { get; set; }
        public string name { get; set; }
        public string acronym { get; set; }
        public Course(int id, int departmentId, string name, string acronym)
        {
            this.id = id;
            this.departmentId = departmentId;
            this.name = name;
            this.acronym = acronym;
        }
    }//= course v xml
    public class Lecture
    {
        public int id { get; set; }
        public int courseId { get; set; }
        public int practice { get; set; }
        public string tag { get; set; }
        public int duration { get; set; }
        public int period { get; set; }
        public Lecture(int id, int courseId, int practice, string tag, int duration, int period)
        {
            this.id = id;
            this.courseId = courseId;
            this.practice = practice;
            this.tag = tag;
            this.duration = duration;
            this.period = period;
        }
    }//= lecture v xml
    public class Lecturer
    {
        public int id { get; set; }
        public string name { get; set; }
        public string forname { get; set; }
        public int departmentId { get; set; }
        public Lecturer(int id, string name, string forname, int departmentId)
        {
            this.id = id;
            this.name = name;
            this.forname = forname;
            this.departmentId = departmentId;
        }
    }//= lecturer v xml
    public class Day
    {
        public int id { get; set; }
        public string name { get; set; }
        public int daysOrder { get; set; }
        public Day(int id, string name, int daysOrder)
        {
            this.id = id;
            this.name = name;
            this.daysOrder = daysOrder;
        }
    }//= day v xml
    public class Time
    {
        public int id { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public int timesOrder { get; set; }
        public Time(int id, int hours, int minutes, int timesOrder)
        {
            this.id = id;
            this.hours = hours;
            this.minutes = minutes;
            this.timesOrder = timesOrder;
        }
    }//= time v xml
    public class Building
    {
        public int id { get; set; }
        public string name { get; set; }
        public Building(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }//= building v xml
    public class Classroom
    {
        public int id { get; set; }
        public string name { get; set; }
        public int buildingId { get; set; }
        public Classroom(int id, string name, int buildingId)
        {
            this.id = id;
            this.name = name;
            this.buildingId = buildingId;
        }
    }//= classroom v xml
    public class Hodina
    {
        public int id { get; set; }
        public int lectureId { get; set; }
        public int lecturerId { get; set; }
        public int dayId { get; set; }
        public int timeId { get; set; }
        public int classroomId { get; set; }
        public string tag { get; set; }
        public Hodina(int id, int lectureId, int lecturerId, int dayId, int timeId, int classroomId, string tag)
        {
            this.id = id;
            this.lectureId = lectureId;
            this.lecturerId = lecturerId;
            this.dayId = dayId;
            this.timeId = timeId;
            this.classroomId = classroomId;
            this.tag = tag;
        }
    }//= hodina v xml
    public class DegreeYear
    {
        public int id { get; set; }
        public string name { get; set; }
        public string acronym { get; set; }
        public DegreeYear(int id, string name, string acronym)
        {
            this.id = id;
            this.name = name;
            this.acronym = acronym;
        }
    }//= degree+year v xml
    public class Zamereni
    {
        public int id { get; set; }
        public string name { get; set; }
        public string acronym { get; set; }
        public int degreeYearId { get; set; }
        public Zamereni(int id, string name, string acronym, int degreeYearId)
        {
            this.id = id;
            this.name = name;
            this.acronym = acronym;
            this.degreeYearId = degreeYearId;
        }
    }//= group v xml
    public class Kruh
    {
        public int id { get; set; }
        public int cisloKruhu { get; set; }
        public int zamereniId { get; set; }
        public Kruh(int id, int cisloKruhu, int zamereniId)
        {
            this.id = id;
            this.cisloKruhu = cisloKruhu;
            this.zamereniId = zamereniId;
        }
    }//= part v xml
    public class HodinyKruhu
    {
        public int id { get; set; }
        public int kruhId { get; set; }
        public int hodinaId { get; set; }
        public HodinyKruhu(int id, int kruhId, int hodinaId)
        {
            this.id = id;
            this.kruhId = kruhId;
            this.hodinaId = hodinaId;
        }
    }//- je vybráno z part elementů


    public class RozvrhDataProvider
    {
        public RozvrhDataProvider(string xmlPath)
        {
            m_rozvhXmlFilePath = xmlPath;
            //hotovo
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

            //možná nebude třeba
            v_courses = new List<Course>();
            v_lecturers = new List<Lecturer>();
            v_buildings = new List<Building>();
            v_degreeyears = new List<DegreeYear>();
            v_zamerenis = new List<Zamereni>();
            v_hodiny = new List<Hodina>();
            v_kruhy = new List<Kruh>();

            s_degreeyears = new List<DegreeYear>();
        }

        private string m_rozvhXmlFilePath;
        private XElement m_xelDefinitions;

        //všechna data v modelu
        //hotovo
        public List<Department> m_departments { get; set; }
        public List<Course> m_courses { get; set; }
        public List<Lecture> m_lectures { get; set; }
        public List<Lecturer> m_lecturers { get; set; }
        public List<Day> m_days { get; set; }
        public List<Time> m_times { get; set; }
        public List<Building> m_buildings { get; set; }
        public List<Classroom> m_classrooms { get; set; }
        public List<Hodina> m_hodiny { get; set; }
        public List<DegreeYear> m_degreeyears { get; set; }
        public List<Zamereni> m_zamerenis { get; set; }
        public List<Kruh> m_kruhy { get; set; }
        public List<HodinyKruhu> m_hodinyKruhu { get; set; }

        //data zobrazená v příslučných objektech view - visible
        //možná nebude třeba
        public List<Department> v_departments { get; set; }
        public List<Course> v_courses { get; set; }
        public List<Lecturer> v_lecturers { get; set; }
        public List<Building> v_buildings { get; set; }
        public List<Hodina> v_hodiny { get; set; }
        public List<DegreeYear> v_degreeyears { get; set; }
        public List<Zamereni> v_zamerenis { get; set; }
        public List<Kruh> v_kruhy { get; set; }

        //označené kolonky v příslučných objektech view - selected
        //možná nebude třeba
        public List<Department> s_departments { get; set; }
        public List<Course> s_courses { get; set; }
        public List<Lecturer> s_lecturers { get; set; }
        public List<Building> s_buildings { get; set; }
        public List<Hodina> s_hodiny { get; set; }
        public List<DegreeYear> s_degreeyears { get; set; }
        public List<Zamereni> s_zamerenis { get; set; }
        public List<Kruh> s_kruhy { get; set; }

        //hotovo
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

        //hotovo
        public bool InitData()
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
                    select dep;
                foreach (XElement dep in enumDepartment)
                {
                    m_departments.Add(new Department(Convert.ToInt32(dep.Attribute("id").Value), Convert.ToInt32(dep.Element("code").Value), dep.Element("name").Value, dep.Element("acronym").Value, Convert.ToUInt64(dep.Element("color").Value)));
                    //Console.WriteLine(dep);
                }
                //Console.WriteLine(m_departments.Count);


                //init Courses
                //!!! vyhodí předměty, které jsou jen z relevantních (těch co už jsou načtené) kateder               
                var depsIds =
                   from dep in m_departments
                   select dep.id;

                var enumCourse =
                    from c in m_xelDefinitions.Element("courses").Descendants("course")
                    where depsIds.Contains((int)c.Element("department").Attribute("ref"))
                    select c;
                foreach (XElement c in enumCourse)
                {
                    m_courses.Add(new Course(Convert.ToInt32(c.Attribute("id").Value), Convert.ToInt32(c.Element("department").Attribute("ref").Value), c.Element("name").Value, c.Element("acronym").Value));
                    //Console.WriteLine(c);
                }

                //init Lectures
                //!!! vyhodí lekce, které jsou jen z relevantních (těch co už jsou načtené) kurzů                
                
                var coursesIds =
                   from c in m_courses
                   select c.id;

                var enumLecture =
                    from lec in m_xelDefinitions.Element("lectures").Descendants("lecture")
                    where coursesIds.Contains((int)lec.Element("course").Attribute("ref"))
                    select lec;
                foreach (XElement lec in enumLecture)
                {
                    m_lectures.Add(new Lecture(Convert.ToInt32(lec.Attribute("id").Value), Convert.ToInt32(lec.Element("course").Attribute("ref").Value),
                                               Convert.ToInt32(lec.Element("practice").Value), lec.Element("tag").Value, Convert.ToInt32(lec.Element("duration").Value),
                                               Convert.ToInt32(lec.Element("period").Value)));
                    //Console.WriteLine(lec);
                }

                //init Lecturers
                //!!!vybere vyučující jen z relevantních kateder 
                var enumLecturer =
                    from ler in m_xelDefinitions.Element("lecturers").Descendants("lecturer")
                    where depsIds.Contains((int)ler.Element("department").Attribute("ref"))   //vybere vyučující ze skutečných kateder
                    select ler;
                foreach (XElement ler in enumLecturer)
                {
                    m_lecturers.Add(new Lecturer(Convert.ToInt32(ler.Attribute("id").Value), ler.Element("name").Value,
                                                 ler.Element("forename").Value, Convert.ToInt32(ler.Element("department").Attribute("ref").Value)));
                    //Console.WriteLine(ler);
                }
                //Console.WriteLine(enumLecturer.Count<XElement>());

                //init Days
                IEnumerable<XElement> enumEl =
                  from d in m_xelDefinitions.Element("days").Descendants("day")
                  select d;
                foreach (XElement d in enumEl)
                {
                    m_days.Add(new Day(Convert.ToInt32(d.Attribute("id").Value), d.Element("czech").Value, Convert.ToInt32(d.Element("daysorder").Value)));
                    //Console.WriteLine(d);
                }
                //Console.WriteLine(enumEl.Count<XElement>());

                //init Times                
                enumEl =
                  from el in m_xelDefinitions.Element("times").Descendants("time")
                  select el;
                foreach (XElement el in enumEl)
                {
                    m_times.Add(new Time(Convert.ToInt32(el.Attribute("id").Value), Convert.ToInt32(el.Element("hours").Value),
                                         Convert.ToInt32(el.Element("minutes").Value), Convert.ToInt32(el.Element("timesorder").Value)));
                    //Console.WriteLine(el);
                }
                //Console.WriteLine(enumEl.Count<XElement>());

                //init Buildings              
                enumEl =
                  from el in m_xelDefinitions.Element("buildings").Descendants("building")
                  select el;
                foreach (XElement el in enumEl)
                {
                    m_buildings.Add(new Building(Convert.ToInt32(el.Attribute("id").Value), el.Element("name").Value));
                    //Console.WriteLine(el);
                }

                //init Classrooms
                enumEl =
                    from el in m_xelDefinitions.Element("classrooms").Descendants("classroom")
                    select el;
                foreach (XElement el in enumEl)
                {
                    m_classrooms.Add(new Classroom(Convert.ToInt32(el.Attribute("id").Value), el.Element("name").Value, Convert.ToInt32(el.Element("building").Attribute("ref").Value)));
                    //Console.WriteLine(el);
                }

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

                    // Console.WriteLine(el);
                }
                //Console.WriteLine(enumLecturer.Count<XElement>());

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
                    //Console.WriteLine(el);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("RozvrhDataProvider::InitData: CHYBA - nepovedlo se načíst data do modelu");
                return false;
            }
            Console.WriteLine("RozvrhDataProvider::InitData: data načtena");
            return true;
        }

        //možná nebude třeba
        public List<int> GetHodinyIdsFromKruhyIds(List<int> kruhyIds)
        {
            var hodinyIds =
                from hId in m_hodinyKruhu
                where kruhyIds.Contains(hId.kruhId)
                select hId.hodinaId;
            return hodinyIds.ToList();
        }

        //možná nebude třeba        
        public List<Hodina> GetHodinyFromHodinyIds(List<int> hodinyIds)
        {
            var hodiny =
                from h in m_hodiny
                where hodinyIds.Contains(h.id)
                select h;
            return hodiny.ToList();
        }

        //možná nebude třeba
        public string GetLecturerNameFromLecturerId(int lecturerId)
        {

            var name =
                from l in m_lecturers
                where lecturerId == l.id
                select l;
            if (name.Count() == 0)
            {
                string noLecturer = "-";
                return noLecturer;
            }
            else
            {
                string lecturerName = "";
                Lecturer result = name.First();
                if (result.forname != "")
                    lecturerName = result.forname[0].ToString() + ". ";
                lecturerName += result.name;
                return lecturerName;
            }
        }

        //možná nebude třeba
        public string GetZamereniNameFromZamereniId(int zamerenisIds)
        {

            var names =
                from z in m_zamerenis
                where zamerenisIds == z.id
                select z.acronym + " - " + z.name;
            if (names.Count() == 0)
            {
                return "";
            }
            else
                return names.First();
        }

        //možná nebude třeba
        public List<Hodina> Filter1(List<int> kruhyIds)
        {
            var vybraneHodinyId = //vybere id hodiny na kterou se odkazuje m_hodinyKruhu 
                from hk in m_hodinyKruhu 
                where kruhyIds.Contains(hk.kruhId)                                        
                select hk.hodinaId;

            var vybraneHodiny =
                from h in m_hodiny
                where vybraneHodinyId.Contains(h.id)
                select h;

            return vybraneHodiny.ToList();
        }

        //pro debug
        public void PrintHodiny(List<Hodina> hodiny)
        {
            foreach (Hodina h in hodiny)
            {
                string lecturerName = GetLecturerNameFromLecturerId(h.lecturerId);
                Console.WriteLine("{0} {1} {2} {3} {4}", lecturerName, h.lectureId, h.dayId, h.timeId, h.classroomId);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            XElement config = XElement.Load("..\\..\\rozvrh_config.xml");
            RozvrhDataProvider R = new RozvrhDataProvider( config.Element("datafilepath").Value);
            
            if (!R.LoadXml())
                goto Finish;

            if (!R.InitData())
                goto Finish;

            //
            List<int> kruhyIds = new List<int>();
            kruhyIds.Add(118);//118 = druhák magistr FTTF

            int zamereniId = 100;

            //List<int> degreeyearsIds = new List<int>();
            //degreeyearsIds.Add(3);

            List<Hodina> vybraneHodiny = R.Filter1(kruhyIds);

            Console.WriteLine("{0} ", R.GetZamereniNameFromZamereniId(zamereniId));
            //R.PrintHodiny(R.GetHodinyFromHodinyIds(R.GetHodinyIdsFromKruhyIds(kruhyIds)));
            R.PrintHodiny(vybraneHodiny);

            Finish:
                Console.WriteLine("Stiskněte libovolnou klávesu...");
                Console.ReadLine();
        }

    }
}
