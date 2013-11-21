using System;
using System.Collections.Generic;
using System.Linq;
using Rozvrh.Models.Timetable;

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
        /// Konstruktor třídy.
        /// </summary>
        public Model()
        {

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

                Specializations = new List<Specialization>();
                Groups = new List<Group>();
                Lecturers = new List<Lecturer>();
                Classrooms = new List<Classroom>();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error when parsing timetable XML.");
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Metoda filtrující zaměření podle zadaného ročníku (DegreeYear). Výsledek ukládá do 'Specializations' vlastnosti Modelu.
        /// </summary>
        /// <param name="buildingIds">Id ročníku (DegreeYear)</param>
        public void FilterDegreeYear2Specialization(String degreeYearId)
        {
            var filteredSpecializations =
                from s in XmlLoader.m_specializations
                where s.degreeYearId.Equals(degreeYearId)
                orderby s.acronym
                select s;

            Specializations = filteredSpecializations.ToList();
        }

        /// <summary>
        /// Metoda filtrující kruhy podle zadaného (jednoho) zaměření. Výsledek ukládá do 'Groups' vlastnosti Modelu.
        /// </summary>
        /// <param name="specializationId">Id zaměření</param>
        public void FilterSpecialization2Groups(String specializationId)
        {
            var filteredGroups =
               from g in XmlLoader.m_groups
               where g.specializationId.Equals(specializationId)
               select g;

            Groups = filteredGroups.ToList();
        }

        /// <summary>
        /// Metoda filtrující vyučující podle zadaných kateder (katedra, která ho zaměstnává). Výsledek ukládá do 'Lecturers' vlastnosti Modelu.
        /// </summary>
        /// <param name="departmentIds">Id kateder</param>
        public void FilterDepartments2Lecturers(String departmentId)
        {
            var filteredLecturers =
                from l in XmlLoader.m_lecturers
                where l.departmentId.Equals(departmentId)
                orderby l.name
                select l;

            Lecturers = filteredLecturers.ToList();
        }

        /// <summary>
        /// Metoda filtrující místnosti podle zadaných budov. Výsledek ukládá do 'Classrooms' vlastnosti Modelu.
        /// </summary>
        /// <param name="buildingIds">Id budov</param>
        public void FilterBuildings2Classrooms(String buildingId)
        {
            var filteredClassrooms =
                from c in XmlLoader.m_classrooms
                where c.buildingId.Equals(buildingId)
                orderby c.name
                select c;

            Classrooms = filteredClassrooms.ToList();
        }

        /// <summary>
        /// Metoda filtrující všechny hodiny (Teachings) podle zadaných parametrů. Výsledek ukládá do 'TimetableFields' vlastnosti Modelu.
        /// </summary>
        /// <param name="groupIds">Id kruhů</param>
        /// <param name="departmentIds">Id kateder</param>
        /// <param name="lecturerIds">Id vyučujících</param>
        /// <param name="classroomIds">Id místnosti</param>
        /// <param name="dayIds">Id dnů</param>
        /// <param name="timeIds">Id časů</param>
        public void FilterAll2TimetableFields(string groupId, string departmentId, string lecturerId, string classroomId, string dayId, string timeId)
        {
            const string NULL = "null";
            var teachingsFromAllFilters = new List<IEnumerable<Teaching>>();

            filterByGroup(groupId, NULL, teachingsFromAllFilters);          //získej Teachings zadaných kruhů
            filterByDepartment(departmentId, NULL, teachingsFromAllFilters);//získej Teachings zadaných kateder
            filterByLecturer(lecturerId, NULL, teachingsFromAllFilters);    //získej Teachings zadaných vyučujících
            filterByClassroom(classroomId, NULL, teachingsFromAllFilters);  //získej Teachings zadaných místností
            filterByDay(dayId, NULL, teachingsFromAllFilters);              //získej Teachings zadaných dnů
            filterByTime(timeId, NULL, teachingsFromAllFilters);            //získej Teachings zadaných časů

            //udělej množinový průnik dílčích filterů
            var resultTeachings = intersect(teachingsFromAllFilters);

            var filteredTimetableFields =
                from h      in resultTeachings
                join lec    in XmlLoader.m_lectures     on h.lectureId      equals lec.id
                join c      in XmlLoader.m_courses      on lec.courseId     equals c.id
                join dep    in XmlLoader.m_departments  on c.departmentId   equals dep.id
                join ler    in XmlLoader.m_lecturers    on h.lecturerId     equals ler.id
                join d      in XmlLoader.m_days         on h.dayId          equals d.id
                join t      in XmlLoader.m_times        on h.timeId         equals t.id
                join cr     in XmlLoader.m_classrooms   on h.classroomId    equals cr.id
                join b      in XmlLoader.m_buildings    on cr.buildingId    equals b.id
                orderby dep.code, c.acronym, lec.practice, ler.name, d.daysOrder, t.timesOrder, b.name, cr.name
                select new TimetableField(dep, c, lec, ler, d, t, b, cr);

            TimetableFields = filteredTimetableFields.ToList();
        }

        private void filterByGroup(string groupId, string NULL, ICollection<IEnumerable<Teaching>> teachingsFromAllFilters)
        {
            if (!groupId.Contains(NULL))
            {
                var timeTableFieldsFromGroupFilter =
                    (from hk in XmlLoader.m_groupTeachingBinder
                     where hk.groupId.Equals(groupId)
                     from h in XmlLoader.m_teaching
                     where hk.teachingId == h.id
                     select h).ToList();
                teachingsFromAllFilters.Add(timeTableFieldsFromGroupFilter);
            }
        }

        private void filterByDepartment(string departmentId, string NULL, ICollection<IEnumerable<Teaching>> teachingsFromAllFilters)
        {
            if (!departmentId.Contains(NULL))
            {
                var teachings2 =
                    (from c in XmlLoader.m_courses
                     where c.departmentId.Equals(departmentId)
                     from l in XmlLoader.m_lectures
                     where c.id == l.courseId
                     from h in XmlLoader.m_teaching
                     where l.id == h.lectureId
                     select h).ToList();
                teachingsFromAllFilters.Add(teachings2);
            }
        }

        private void filterByLecturer(string lecturerId, string NULL, ICollection<IEnumerable<Teaching>> teachingsFromAllFilters)
        {
            if (!lecturerId.Contains(NULL))
            {
                var teachings3 =
                    (from h in XmlLoader.m_teaching
                     where h.lecturerId.Equals(lecturerId)
                     select h).ToList();
                teachingsFromAllFilters.Add(teachings3);
            }
        }

        private void filterByClassroom(string classroomId, string NULL, ICollection<IEnumerable<Teaching>> teachingsFromAllFilters)
        {
            if (!classroomId.Contains(NULL))
            {
                var teachings4 =
                    (from h in XmlLoader.m_teaching
                     where h.classroomId.Equals(classroomId)
                     select h).ToList();
                teachingsFromAllFilters.Add(teachings4);
            }
        }

        private void filterByDay(string dayId, string NULL, ICollection<IEnumerable<Teaching>> teachingsFromAllFilters)
        {
            if (!dayId.Contains(NULL))
            {
                var teachings5 =
                    (from h in XmlLoader.m_teaching
                     where h.dayId.Equals(dayId)
                     select h).ToList();
                teachingsFromAllFilters.Add(teachings5);
            }
        }

        private void filterByTime(string timeId, string NULL, ICollection<IEnumerable<Teaching>> teachingsFromAllFilters)
        {
            if (!timeId.Contains(NULL))
            {
                var teachings6 =
                    (from h in XmlLoader.m_teaching
                     where h.timeId.Equals(timeId)
                     select h).ToList();
                teachingsFromAllFilters.Add(teachings6);
            }
        }

        private static IEnumerable<Teaching> intersect(IEnumerable<IEnumerable<Teaching>> teachingsFromAllFilters)
        {
            return teachingsFromAllFilters.Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToList());
        }

        //Olda: Metoda, která podle nastavení filtrů vrátí seznam TimetableFieldů, by měla být v Controlleru.
        //      V tuhle chvíli tedy v souboru HomeController, v metodě Filter.
        //      Jedná se totiž o aplikační logiku, nikoliv datovou.


        public List<Department>     Departments         { get; private set; }
        public List<Specialization> Specializations     { get; private set; }
        public List<Group>          Groups              { get; private set; }
        public List<DegreeYear>     DegreeYears         { get; private set; }
        public List<Lecturer>       Lecturers           { get; private set; }
        public List<Building>       Buildings           { get; private set; }
        public List<Classroom>      Classrooms          { get; private set; }
        public List<Day>            Days                { get; private set; }
        public List<Time>           Times               { get; private set; }
        public List<TimetableField> TimetableFields     { get; private set; }
    }
}