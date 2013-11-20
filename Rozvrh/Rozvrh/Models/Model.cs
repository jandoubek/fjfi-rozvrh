using System;
using System.Collections.Generic;
using Rozrvh;
using Rozvrh.Models.Timetable;
using System.Linq;
// Docasny import kvuli kompilaci, dokud se nerozhodne, co s tim
using Rozrvh.Exporters.Common;
using System.Xml.Linq;

namespace Rozvrh.Models
{
    /// <summary>
    /// Dummy data
    /// </summary>
    public class Model:IModel
    {
        // Temporary, see comment in constructor
        XMLTimetableLoader XmlLoader = new XMLTimetableLoader("C:\\Aktualni_databaze.xml");
                
      
        /// <summary>
        /// Konstruktor třídy
        /// </summary>
        public Model()
        {
// FIX ME !!! - autor: Richard
// Micha se export a model, pokud od sbe neco potrebuji, musi pres controller. 
// Pokud exporter potrebuje cokoli od modelu, controller si vse vytahne z modelu pred(!) zavooanim exporteru a rovnou mu posle vse potrebne.
// Proc je vubec v modelu IExportHodina? Pokud vidim dobre, pouziva tato trida jako nas datovy "typ" pro vypis vfiltrovanych hodin ve view.
// V tom pripade, je to nas zakladni datovy typ a mel by byt soucasti modelu a jmenovat se jinak, napr. FilteredLecture
// Nejsem si jisty, jestli je v tomto pripade nutny interface - pro zakladni datovy typ, ktery nema zadne funkce (vyjma get a set) - nebyla by vhodnejsi struktura?
// Navic mi prijde, ze ExportHodina se funkcne kryje s TimeTableField a slo by je sjednotit a zbavit se tak jedne datove tridy.
// Navrhuji refaktoring ve smyslu vyse uvedeneho, pokud ma nekdo jiny nazor - sem (resp. na FB nebo na SCRUM) s nim.
            FiltredLectures = new List<IExportHodina>();

            System.Diagnostics.Debug.WriteLine("Model constructor");
            // Richard: This is only a temporary solution. In next phase the XmlLoader is gonna be modofied to be a singleton and these linq queries placed there.
            // Model constructor will be designed to only set references for all lists.
            try
            {
                Departments = XmlLoader.m_departments;
                DegreeYears = XmlLoader.m_degreeyears;
                Buildings = XmlLoader.m_buildings;
                Days = XmlLoader.m_days;
                Times = XmlLoader.m_times;

                Specializations = new List<Zamereni>();
                Groups = new List<Kruh>();
                Lecturers = new List<Lecturer>();
                Classrooms = new List<Classroom>();
            
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when parsing timetable XML.");
                Console.WriteLine(e.StackTrace);
            }
            
            //Departments = new List<string> { "KJCH", "KJR" };
        }

        public void FilterDegreeYear2Specialization(String degreeYearId)
        {
            int degreeYearIdAsInt = Convert.ToInt32(degreeYearId);  
  
            //ze zobrazených (visible) zaměření vybere ta, které přísluší vybranému ročníku
            var filteredSpecializations =
                from s in XmlLoader.m_zamerenis
                where s.degreeYearId.Equals(degreeYearIdAsInt)
                orderby s.acronym
                select s;

            Specializations = filteredSpecializations.ToList();
        }

        public void FilterSpecialization2Groups(String specializationId)
        {
            int specializationIdAsInt = Convert.ToInt32(specializationId); 

            //ze všech kruhů vybere ty, které přísluší vybranému zaměření
            var filteredGroups =
               from g in XmlLoader.m_kruhy
               where g.zamereniId.Equals(specializationIdAsInt)
               select g;

            Groups = filteredGroups.ToList();
        }

        public void FilterDepartments2Lecturers(String departmentId)
        {
            int departmentIdAsInt = Convert.ToInt32(departmentId);

            //ze všech učitelů vybere ty, kteří jsou z vybraných kateder
            var filteredLecturers =
                from l in XmlLoader.m_lecturers
                where l.departmentId.Equals(departmentIdAsInt)
                orderby l.name
                select l;

            Lecturers = filteredLecturers.ToList();
        }

        public void FilterBuildings2Classrooms(String buildingId)
        {
            int buildingIdAsInt = Convert.ToInt32(buildingId);
            //ze všech místností vybere ty, které jsou z vybraných budov
            var filteredClassrooms =
                from c in XmlLoader.m_classrooms
                where c.buildingId.Equals(buildingIdAsInt)
                orderby c.name
                select c;

            Classrooms = filteredClassrooms.ToList();
        }

        //filter 5 - hlavní filter
        public void FilterAll2TimetableFields(String groupId, String departmentId, 
            String lecturerId, String classroomId, String dayId, String timeId)
        {

            int groupIdAsInt = Convert.ToInt32(groupId);
            int departmentAsInt = Convert.ToInt32(departmentId);
            int lecturerIdAsInt = Convert.ToInt32(lecturerId);
            int classroomIdAsInt = Convert.ToInt32(classroomId);
            int dayIdAsInt = Convert.ToInt32(dayId);
            int timeIdAsInt = Convert.ToInt32(timeId);

            List<IEnumerable<Hodina>> HodinyDilci = new List<IEnumerable<Hodina>>();

            //1.podle kruhu - získá id hodin, které májí v rozvrhu označené kruhy    
            //získej id hodin vybraných kruhů
            var Hodiny1 =
                from hk in XmlLoader.m_hodinyKruhu
                where hk.kruhId.Equals(groupId)
                from h in XmlLoader.m_hodiny
                where hk.hodinaId == h.id
                select h;
            if (Hodiny1.Count() != 0)
                HodinyDilci.Add(Hodiny1);

            //2.podle katedry, která vypisuje kurz (katedra -> kurz -> lekce -> hodina)
            //získej kurzy, které jsou vypisovány těmito katedrami             
            var Hodiny2 =
                from c in XmlLoader.m_courses
                where c.departmentId.Equals(departmentAsInt)
                from l in XmlLoader.m_lectures
                where c.id == l.courseId
                from h in XmlLoader.m_hodiny
                where l.id == h.lectureId
                select h;
            if (Hodiny2.Count() != 0)
                HodinyDilci.Add(Hodiny2);

            //3. podle vyučujícího
            //získej hodiny, které vedou vybraní vyučující
            var Hodiny3 =
                from h in XmlLoader.m_hodiny
                where h.lecturerId.Equals(lecturerIdAsInt)
                select h;
            if (Hodiny3.Count() != 0)
                HodinyDilci.Add(Hodiny3);

            //4. podle vybrané místnosti
            //získej hodiny, které jsou vedeny ve vybraných místnostech

            var Hodiny4 =
                from h in XmlLoader.m_hodiny
                where h.classroomId.Equals(classroomId)
                select h;
            if (Hodiny4.Count() != 0)
                HodinyDilci.Add(Hodiny4);

            //5.podle dne v týdnu
            //získej hodiny vedené ve vybraných dnech v týdnu
            var Hodiny5 =
                from h in XmlLoader.m_hodiny
                where h.dayId.Equals(dayId)
                select h;
            if (Hodiny5.Count() != 0)
                HodinyDilci.Add(Hodiny5);

            //6.podle času
            //získej hodiny vedené ve vybraných časech
            var Hodiny6 =
                from h in XmlLoader.m_hodiny
                where h.timeId.Equals(timeId)
                select h;
            if (Hodiny6.Count() != 0)
                HodinyDilci.Add(Hodiny6);

            //udělej množinový průnik dílčích filterů
            var HodinyPrunik = HodinyDilci.Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToList());

            var TimetableFields =
                from h in HodinyPrunik
                join lec in XmlLoader.m_lectures on h.lectureId equals lec.id
                join c in XmlLoader.m_courses on lec.courseId equals c.id
                join dep in XmlLoader.m_departments on c.departmentId equals dep.id
                join ler in XmlLoader.m_lecturers on h.lecturerId equals ler.id
                join d in XmlLoader.m_days on h.dayId equals d.id
                join t in XmlLoader.m_times on h.timeId equals t.id
                join cr in XmlLoader.m_classrooms on h.classroomId equals cr.id
                join b in XmlLoader.m_buildings on cr.buildingId equals b.id
                orderby dep.code, c.acronym, lec.practice, ler.name, d.daysOrder, t.timesOrder, b.name, cr.name
                select new TimetableField(dep, c, lec, ler, d, t, b, cr);

            TimetableFields = TimetableFields.ToList();
        }

        //Olda: Metoda, která podle nastavení filtrů vrátí seznam TimetableFieldů, by měla být v Controlleru.
        //      V tuhle chvíli tedy v souboru HomeController, v metodě Filter.
        //      Jedná se totiž o aplikační logiku, nikoliv datovou.

        /// <summary>
        /// Seznam vyfiltrovaných hodin, určení pro zobrazení v tabulce pod filtry
        /// </summary>
        public List<IExportHodina> FiltredLectures { get; private set; }

        public List<Department> Departments { get; private set; }
        public List<Zamereni> Specializations { get; private set; }
        public List<Kruh> Groups { get; private set; }
        public List<DegreeYear> DegreeYears { get; private set; }
        public List<Lecturer> Lecturers { get; private set; }
        public List<Building> Buildings { get; private set; }
        public List<Classroom> Classrooms { get; private set; }
        public List<Day> Days { get; private set; }
        public List<Time> Times { get; private set; }

        public List<TimetableField> TimetableFields { get; private set; }
    }
}