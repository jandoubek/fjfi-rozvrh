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
    public class Model : IModel
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
            //ze zobrazených (visible) zaměření vybere ta, které přísluší vybranému ročníku
            var filteredSpecializations =
                from s in XmlLoader.m_zamerenis
                where s.degreeYearId.Equals(degreeYearId)
                orderby s.acronym
                select s;

            Specializations = filteredSpecializations.ToList();
        }

        public void FilterSpecialization2Groups(String specializationId)
        {
            //ze všech kruhů vybere ty, které přísluší vybranému zaměření
            var filteredGroups =
               from g in XmlLoader.m_kruhy
               where g.zamereniId.Equals(specializationId)
               select g;

            Groups = filteredGroups.ToList();
        }

        public void FilterDepartments2Lecturers(String departmentId)
        {
            //ze všech učitelů vybere ty, kteří jsou z vybraných kateder
            var filteredLecturers =
                from l in XmlLoader.m_lecturers
                where l.departmentId.Equals(departmentId)
                orderby l.name
                select l;

            Lecturers = filteredLecturers.ToList();
        }

        public void FilterBuildings2Classrooms(String buildingId)
        {
            //ze všech místností vybere ty, které jsou z vybraných budov
            var filteredClassrooms =
                from c in XmlLoader.m_classrooms
                where c.buildingId.Equals(buildingId)
                orderby c.name
                select c;

            Classrooms = filteredClassrooms.ToList();
        }

        /// <summary>
        /// Metoda filtrující nad všemi daty podle zadaných parametrů.
        /// </summary>
        /// <param name="groupId">Id kruhu</param>
        /// <param name="departmentId">Id katedry</param>
        /// <param name="lecturerId">Id vyučujícího</param>
        /// <param name="classroomId">Id třídy</param>
        /// <param name="dayId">Id dne</param>
        /// <param name="timeId">Id času</param>
        public void FilterAll2TimetableFields(String groupId, String departmentId,
            String lecturerId, String classroomId, String dayId, String timeId)
        {
            const string NULL = "null";
            var timetableFieldsFromEachFilter = new List<IEnumerable<Hodina>>();

            filterByGroup(groupId, NULL, timetableFieldsFromEachFilter);
            filterByDepartment(departmentId, NULL, timetableFieldsFromEachFilter);
            filterByLecturer(lecturerId, NULL, timetableFieldsFromEachFilter);
            filterByClassroom(classroomId, NULL, timetableFieldsFromEachFilter);
            filterByDay(dayId, NULL, timetableFieldsFromEachFilter);
            filterByTime(timeId, NULL, timetableFieldsFromEachFilter);

            //udělej množinový průnik dílčích filterů
            var timetableFields = intersect(timetableFieldsFromEachFilter);

            var filteredTimetableFields =
                from h in timetableFields
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

            TimetableFields = filteredTimetableFields.ToList();
        }

        private void filterByGroup(string groupId, string NULL, ICollection<IEnumerable<Hodina>> timetableFieldsFromEachFilter)
        {
            if (!groupId.Contains(NULL))
            {
                var timeTableFieldsFromGroupFilter =
                    (from hk in XmlLoader.m_hodinyKruhu
                     where hk.kruhId.Equals(groupId)
                     from h in XmlLoader.m_hodiny
                     where hk.hodinaId == h.id
                     select h).ToList();
                timetableFieldsFromEachFilter.Add(timeTableFieldsFromGroupFilter);
            }
        }

        private void filterByDepartment(string departmentId, string NULL, ICollection<IEnumerable<Hodina>> timetableFieldsFromEachFilter)
        {
            if (!departmentId.Contains(NULL))
            {
                var hodiny2 =
                    (from c in XmlLoader.m_courses
                     where c.departmentId.Equals(departmentId)
                     from l in XmlLoader.m_lectures
                     where c.id == l.courseId
                     from h in XmlLoader.m_hodiny
                     where l.id == h.lectureId
                     select h).ToList();
                timetableFieldsFromEachFilter.Add(hodiny2);
            }
        }

        private void filterByLecturer(string lecturerId, string NULL, ICollection<IEnumerable<Hodina>> timetableFieldsFromEachFilter)
        {
            if (!lecturerId.Contains(NULL))
            {
                var hodiny3 =
                    (from h in XmlLoader.m_hodiny
                     where h.lecturerId.Equals(lecturerId)
                     select h).ToList();
                timetableFieldsFromEachFilter.Add(hodiny3);
            }
        }

        private void filterByClassroom(string classroomId, string NULL, ICollection<IEnumerable<Hodina>> timetableFieldsFromEachFilter)
        {
            if (!classroomId.Contains(NULL))
            {
                var hodiny4 =
                    (from h in XmlLoader.m_hodiny
                     where h.classroomId.Equals(classroomId)
                     select h).ToList();
                timetableFieldsFromEachFilter.Add(hodiny4);
            }
        }

        private void filterByDay(string dayId, string NULL, ICollection<IEnumerable<Hodina>> timetableFieldsFromEachFilter)
        {
            if (!dayId.Contains(NULL))
            {
                var hodiny5 =
                    (from h in XmlLoader.m_hodiny
                     where h.dayId.Equals(dayId)
                     select h).ToList();
                timetableFieldsFromEachFilter.Add(hodiny5);
            }
        }

        private void filterByTime(string timeId, string NULL, ICollection<IEnumerable<Hodina>> timetableFieldsFromEachFilter)
        {
            if (!timeId.Contains(NULL))
            {
                var hodiny6 =
                    (from h in XmlLoader.m_hodiny
                     where h.timeId.Equals(timeId)
                     select h).ToList();
                timetableFieldsFromEachFilter.Add(hodiny6);
            }
        }

        private static IEnumerable<Hodina> intersect(IEnumerable<IEnumerable<Hodina>> timetableFieldsFromEachFilter)
        {
            return timetableFieldsFromEachFilter.Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToList());
        }

        //Olda: Metoda, která podle nastavení filtrů vrátí seznam TimetableFieldů, by měla být v Controlleru.
        //      V tuhle chvíli tedy v souboru HomeController, v metodě Filter.
        //      Jedná se totiž o aplikační logiku, nikoliv datovou.

        /// <summary>
        /// Seznam vyfiltrovaných hodin, určení pro zobrazení v tabulce pod filtry
        /// </summary>
        public List<IExportHodina>  FiltredLectures     { get; private set; }

        public List<Department>     Departments         { get; private set; }
        public List<Zamereni>       Specializations     { get; private set; }
        public List<Kruh>           Groups              { get; private set; }
        public List<DegreeYear>     DegreeYears         { get; private set; }
        public List<Lecturer>       Lecturers           { get; private set; }
        public List<Building>       Buildings           { get; private set; }
        public List<Classroom>      Classrooms          { get; private set; }
        public List<Day>            Days                { get; private set; }
        public List<Time>           Times               { get; private set; }
        public List<TimetableField> TimetableFields     { get; private set; }
    }
}