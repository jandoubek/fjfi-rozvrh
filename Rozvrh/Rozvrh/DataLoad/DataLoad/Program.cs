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
using Microsoft.VisualStudio.TestTools.UnitTesting;


    //bude sloužit pro přenos do .ical, .pdf exportu a na grafický výstup rozvrhu na obrazovku
    public class TimetableField
    {
        public string department { get; private set; }//název katedry
        public string department_acr { get; private set; }//zkratka katedry
        public string predmet { get; private set; }  //název předmětu
        public string predmet_acr { get; private set; } //zkratka predmetu
        public ulong color { get; private set; }     //barva políčka
        public int period { get; private set; }      //perioda opakování hodiny
        public int duration { get; private set; }    //délka lekce v hodinách
        public string tag { get; private set; }      //tag
        public bool practice { get; private set; }   //cvičení?
        public string lecturer { get; private set; } //složené jméno učitele
        public string day { get; private set; }      //název dne v týdnu   
        public int day_order { get; private set; }   //pořadí dne v týdnu
        public int time_hours { get; private set; }  //čas hodin
        public int time_minutes { get; private set; }//čas minut
        public int time_order { get; private set; }  //čas pořadí časového slotu
        public string building { get; private set; } //budova název
        public string classroom { get; private set; }//místnost název

        public TimetableField(Department dep, Course c, Lecture lec, Lecturer ler, Day d, Time t, Building b, Classroom cr)
        {
            department = dep.name;
            department_acr = dep.acronym;
            predmet = c.name;
            predmet_acr = c.acronym;
            if (lec.practice == 1)
                predmet_acr += "cv";
            color = dep.color;
            period = lec.period;
            duration = lec.duration;
            tag = lec.tag;
            practice = lec.practice>0;
            lecturer = ler.name;
            if (ler.forname.Length != 0)
                lecturer = ler.forname[0] + ". " + lecturer;
            day = d.name;
            day_order = d.daysOrder;
            time_hours = t.hours;
            time_minutes = t.minutes;
            time_order = t.timesOrder;
            building = b.name;
            classroom = cr.name;
            if (predmet == "Jazyky")
            {
                predmet_acr = "JAZ";
                classroom = "-";
                building = "-";
            }
        }
    }

    public class Department
    {
        public int id { get; private set; }
        public int code { get; private set; }
        public string name { get; private set; }
        public string acronym { get; private set; }
        public ulong color { get; private set; }
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
        public int id { get; private set; }
        public int departmentId { get; private set; }
        public string name { get; private set; }
        public string acronym { get; private set; }
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
        public int id { get; private set; }
        public int courseId { get; private set; }
        public int practice { get; private set; }
        public string tag { get; private set; }
        public int duration { get; private set; }
        public int period { get; private set; }
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
        public int id { get; private set; }
        public string name { get; private set; }
        public string forname { get; private set; }
        public int departmentId { get; private set; }
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
        public int id { get; private set; }
        public string name { get; private set; }
        public int daysOrder { get; private set; }
        public Day(int id, string name, int daysOrder)
        {
            this.id = id;
            this.name = name;
            this.daysOrder = daysOrder;
        }
    }//= day v xml
    public class Time
    {
        public int id { get; private set; }
        public int hours { get; private set; }
        public int minutes { get; private set; }
        public int timesOrder { get; private set; }
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
        public int id { get; private set; }
        public string name { get; private set; }
        public Building(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }//= building v xml
    public class Classroom
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public int buildingId { get; private set; }
        public Classroom(int id, string name, int buildingId)
        {
            this.id = id;
            this.name = name;
            this.buildingId = buildingId;
        }
    }//= classroom v xml
    public class Hodina
    {
        public int id { get; private set; }
        public int lectureId { get; private set; }
        public int lecturerId { get; private set; }
        public int dayId { get; private set; }
        public int timeId { get; private set; }
        public int classroomId { get; private set; }
        public string tag { get; private set; }
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
        public int id { get; private set; }
        public string name { get; private set; }
        public string acronym { get; private set; }
        public DegreeYear(int id, string name, string acronym)
        {
            this.id = id;
            this.name = name;
            this.acronym = acronym;
        }
    }//= degree+year v xml
    public class Zamereni
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public string acronym { get; private set; }
        public int degreeYearId { get; private set; }
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
        public int id { get; private set; }
        public int cisloKruhu { get; private set; }
        public int zamereniId { get; private set; }
        public Kruh(int id, int cisloKruhu, int zamereniId)
        {
            this.id = id;
            this.cisloKruhu = cisloKruhu;
            this.zamereniId = zamereniId;
        }
    }//= part v xml
    public class HodinyKruhu
    {
        public int id { get; private set; }
        public int kruhId { get; private set; }
        public int hodinaId { get; private set; }
        public HodinyKruhu(int id, int kruhId, int hodinaId)
        {
            this.id = id;
            this.kruhId = kruhId;
            this.hodinaId = hodinaId;
        }
    }//- je vybráno z part elementů


    public class ModelData
    {
        public ModelData(string xmlPath)
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

    public class ViewData
    {
        public ViewData(ModelData md)
        {
            this.md = md;
            
            //visible
            v_degreeyears = md.m_degreeyears;
            v_zamerenis = new List<Zamereni>();
            v_kruhy = new List<Kruh>();
            v_departments = md.m_departments;
            v_lecturers = new List<Lecturer>();
            v_buildings = md.m_buildings;
            v_classrooms = new List<Classroom>();
            v_days = md.m_days;
            v_times = md.m_times;
        }

        ModelData md { get; set; }
        public List<TimetableField> v_timetablefields { get; private set; }

        //data zobrazená v příslučných objektech view - visible
        public List<DegreeYear> v_degreeyears { get; set; }
        public List<Zamereni> v_zamerenis { get; set; }
        public List<Kruh> v_kruhy { get; set; }
        public List<Department> v_departments { get; set; }
        public List<Lecturer> v_lecturers { get; set; }
        public List<Building> v_buildings { get; set; }
        public List<Classroom> v_classrooms { get; set; }
        public List<Day> v_days { get; set; }
        public List<Time> v_times { get; set; }
     
        //filter 1 - z vybraných položek ročníku zobrazí příšlušná zaměření
        public void FilterDegreeYear2Zamereni(DegreeYear s_degreeyear)
        {
            //ze zobrazených (visible) zaměření vybere ta, které přísluší vybranému ročníku
            var zamerenis =
                from z in md.m_zamerenis
                where s_degreeyear != null && s_degreeyear.id.Equals(z.degreeYearId)
                orderby z.acronym
                select z;

            v_zamerenis = zamerenis.ToList();
        }

        //filter 2 -  z vybrané položky zaměření vybere příslušné kruhy
        public void FilterZamereni2Kruhy(Zamereni s_zamereni)
        {
            //ze všech kruhů vybere ty, které přísluší vybranému zaměření
            var kruhy =
                from k in md.m_kruhy
                where s_zamereni != null && s_zamereni.id.Equals(k.zamereniId)
                orderby k.cisloKruhu
                select k;

            v_kruhy = kruhy.ToList();
        }

        //filter 3 -  z vybraných položek kateder vybere příslušné učitele
        public void FilterDepartments2Lecturers(List<Department> s_departments)
        {
            //ze všech učitelů vybere ty, kteří jsou z vybraných kateder
            var ucitele =
                from k in s_departments
                from l in md.m_lecturers
                where k.id == l.departmentId
                orderby l.name
                select l;

            v_lecturers = ucitele.ToList();
        }

        //filter 4 - z vybraných budov zobraz patřičné místnosti
        public void FilterBuildings2Classrooms(List<Building> s_buildings)
        {
            //ze všech místností vybere ty, které jsou z vybraných budov
            var mistnosti =
                from b in s_buildings
                from c in md.m_classrooms
                where b.id == c.buildingId
                orderby c.name
                select c;

            v_classrooms = mistnosti.ToList();
        }

        //filter 5 - hlavní filter
        public void FilterAll2TimetableFields(List<Kruh> s_kruhy, List<Department> s_departments, List<Lecturer> s_lecturers, List<Classroom> s_classrooms, List<Day> s_days, List<Time> s_times)
        {
            if(s_kruhy == null)
                s_kruhy = new List<Kruh>();
            if (s_departments == null)
                s_departments = new List<Department>();
            if (s_lecturers == null)
                s_lecturers = new List<Lecturer>();
            if (s_classrooms == null)
                s_classrooms = new List<Classroom>();
            if (s_days == null)
                s_days = new List<Day>();
            if (s_times == null)
                s_times = new List<Time>();

            List<IEnumerable<Hodina>> HodinyDilci = new List<IEnumerable<Hodina>>();

        //1.podle kruhu - získá id hodin, které májí v rozvrhu označené kruhy    
            //získej id hodin vybraných kruhů
            var Hodiny1 =
                from k in s_kruhy
                from hk in md.m_hodinyKruhu
                where k.id == hk.kruhId
                from h in md.m_hodiny
                where hk.hodinaId == h.id
                select h;
            if (Hodiny1.Count() != 0)
                HodinyDilci.Add(Hodiny1);

        //2.podle katedry, která vypisuje kurz (katedra -> kurz -> lekce -> hodina)
            //získej kurzy, které jsou vypisovány těmito katedrami             
            var Hodiny2 =
                from s in s_departments
                from c in md.m_courses
                where s.id == c.departmentId
                from l in md.m_lectures
                where c.id == l.courseId
                from h in md.m_hodiny
                where l.id == h.lectureId
                select h;
            if (Hodiny2.Count() != 0)
                HodinyDilci.Add(Hodiny2);
       
        //3. podle vyučujícího
            //získej hodiny, které vedou vybraní vyučující
            var Hodiny3 =
                from h in md.m_hodiny
                from l in s_lecturers
                where l.id == h.lecturerId
                select h;
            if (Hodiny3.Count() != 0)
                HodinyDilci.Add(Hodiny3);

        //4. podle vybrané místnosti
            //získej hodiny, které jsou vedeny ve vybraných místnostech

            var Hodiny4 =
                from r in s_classrooms
                from h in md.m_hodiny
                where r.id == h.classroomId
                select h;
            if (Hodiny4.Count() != 0)
                HodinyDilci.Add(Hodiny4);

        //5.podle dne v týdnu
            //získej hodiny vedené ve vybraných dnech v týdnu
            var Hodiny5 =
                from d in s_days
                from h in md.m_hodiny
                where d.id == h.dayId
                select h;
            if (Hodiny5.Count() != 0)
                HodinyDilci.Add(Hodiny5);

        //6.podle času
            //získej hodiny vedené ve vybraných časech
            var Hodiny6 =
                from t in s_times
                from h in md.m_hodiny
                where t.id == h.timeId
                select h;
            if(Hodiny6.Count() != 0)
                HodinyDilci.Add(Hodiny6);

            //udělej množinový průnik dílčích filterů
            var HodinyPrunik = HodinyDilci.Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToList());
            
            var TimetableFields =
                from h in HodinyPrunik
                join lec in md.m_lectures on h.lectureId equals lec.id
                join c in md.m_courses on lec.courseId equals c.id
                join dep in md.m_departments on c.departmentId equals dep.id
                join ler in md.m_lecturers on h.lecturerId equals ler.id
                join d in md.m_days on h.dayId equals d.id
                join t in md.m_times on h.timeId equals t.id
                join cr in md.m_classrooms on h.classroomId equals cr.id 
                join b in md.m_buildings on cr.buildingId equals b.id
                orderby dep.code, c.acronym, lec.practice, ler.name, d.daysOrder, t.timesOrder, b.name, cr.name
                select new TimetableField(dep, c, lec, ler, d, t, b, cr);

            v_timetablefields = TimetableFields.ToList();
        }

    }


    class Program
    {

        [TestMethod]
        public static void TestFilter1(ModelData MD, bool vypis)
        {
            ViewData VD = new ViewData(MD);
            VD.FilterDegreeYear2Zamereni(VD.v_degreeyears[1]);         //zobraz zaměření příslušné vybranému kruhu index 1 - BS 2.ročník
            List<string> expected = new List<string> { "APIN", "ASI", "FYT", "JCHI", "JIB", "JIC", "LPT", "RT", "ZS" };

            if (vypis)
            {
                //vypiš co je v VD.v_zamerenis
                Console.Write("\n\n zaměření v {0}: \n", VD.v_degreeyears[1].acronym);
                foreach (Zamereni z in VD.v_zamerenis)
                    Console.Write("     {0} \n", z.acronym);
            }

            CollectionAssert.AreEqual(expected, VD.v_zamerenis.Select(z => z.acronym).ToList(), "Neprošel test Filteru 1 - FilterDegreeyear2Zamereni");
        }

        [TestMethod]
        public static void TestFilter2(ModelData MD, bool vypis)
        {
            ViewData VD = new ViewData(MD);
            VD.FilterZamereni2Kruhy(MD.m_zamerenis[8]);              //zobraz kruhy příslušné vybranému zaměření index 8 - BS 2.ročník
            List<int> expected = new List<int> { 1, 2, 3, 4, 5, 6, 7};

            if (vypis)
            {
                //vypiš co je v VD.v_kruhy
                Console.Write("\n\n zaměření v {0} 2.ročník Bc: \n", MD.m_zamerenis[8].acronym);
                foreach (Kruh k in VD.v_kruhy)
                    Console.Write("     {0} \n", k.cisloKruhu);
            }

            CollectionAssert.AreEqual(expected, VD.v_kruhy.Select(k => k.cisloKruhu).ToList(), "Neprošel test Filteru 2 - FilterZamereni2Kruhy");
        }

        [TestMethod]
        public static void TestFilter3(ModelData MD, bool vypis)
        {
            ViewData VD = new ViewData(MD);
            List<Department> s_depts = new List<Department> { MD.m_departments[0], MD.m_departments[2], MD.m_departments[9] }; //KM, KJ, KSE
            VD.FilterDepartments2Lecturers(s_depts);            
            int expectedCount = 150;

            if (vypis)
            {
                //vypiš co je v VD.v_lecturers
                Console.Write("\n\n  katedra {0}, {1} a {2} mají učitele: \n", MD.m_departments[0].acronym, MD.m_departments[2].acronym, MD.m_departments[9].acronym);
                foreach (Lecturer l in VD.v_lecturers)
                    Console.Write("     {0} {1} \n", l.forname, l.name);
            }

            Assert.AreEqual(expectedCount, VD.v_lecturers.Count(), "Neprošel test Filteru 3 - FilterDepartments2Lecturers");
        }

        [TestMethod]
        public static void TestFilter4(ModelData MD, bool vypis)
        {
            ViewData VD = new ViewData(MD);
            List<Building> s_builds = new List<Building> { MD.m_buildings[8], MD.m_buildings[5], MD.m_buildings[1] }; //UTIA, Troja, Trojanova
            VD.FilterBuildings2Classrooms(s_builds);
            int expectedCount = 38;

            if (vypis)
            {
                //vypiš co je v VD.v_classrooms
                Console.Write("\n\n  budovy {0}, {1} a {2} mají místnosti: \n", MD.m_buildings[8].name, MD.m_buildings[5].name, MD.m_buildings[1].name);
                foreach (Classroom c in VD.v_classrooms)
                    Console.Write("     {0}\n", c.name);
            }

            Assert.AreEqual(expectedCount, VD.v_classrooms.Count(), "Neprošel test Filteru 4 - FilterBuildings2Classrooms");
        }

        [TestMethod]
        public static void TestFilter5(ModelData MD, bool vypis)
        {
            ViewData VD = new ViewData(MD);

            List<Kruh> s_kruhy = new List<Kruh> { MD.m_kruhy[0], MD.m_kruhy[2]}; //Bc.2 JIB kruh1   Bc.3 FYT kruh1
            //List<Department> s_depts = new List<Department> { MD.m_departments[1]}; //KF
            //List<Classroom> s_clasrms = new List<Classroom>(){ MD.m_classrooms[0], MD.m_classrooms[2] };//??
            List<Time> s_times = new List<Time> { MD.m_times[0], MD.m_times[1] }; //7:30 9:30           
            List<Day> s_days = new List<Day> { MD.m_days[4], MD.m_days[3] }; //Ct Pa      
            
            VD.FilterAll2TimetableFields(s_kruhy, null, null, null, s_days, s_times);
            List<string> expected = new List<string> { "KFcv", "JAZ", "SUPR", "ZAF1"};

            if (vypis)
            {
                Console.WriteLine("\n\n výsledná pole jsou následující: ");
                foreach (TimetableField t in VD.v_timetablefields)
                    Console.WriteLine("    {6}  {0}  {1}  {2}  {3}:{4} {5}", t.predmet_acr, t.lecturer, t.day, t.time_hours, t.time_minutes, t.classroom, t.department_acr);
            }

            CollectionAssert.AreEqual(expected, VD.v_timetablefields.Select(t => t.predmet_acr).ToList(), "Neprošel test Filteru 5 - FilterAll2TimetableFields");
        }

        static void Main(string[] args)
        {
            XElement config = XElement.Load("..\\..\\rozvrh_config.xml");
            ModelData MD = new ModelData(config.Element("datafilepath").Value);

            //načti databazi z XML
            if (!MD.LoadXml())
                goto Finish;

            //zpracuj načtená data
            if (!MD.InitModelData())
                goto Finish;


            TestFilter1(MD, true); //test FilterDegreeyear2Zamereni

            TestFilter2(MD, true); //test FilterZamereni2Kruhy

            TestFilter3(MD, true); //test FilterDepartments2Lecturers
    
            TestFilter4(MD, true); //test FilterBuildings2Classrooms

            TestFilter5(MD, true); //test FilterAll2TimetableFields

            Finish:
                Console.WriteLine("Stiskněte libovolnou klávesu...");
                Console.ReadLine();
        }

    }
