using System;
using System.Collections.Generic;
using System.Linq;
using Rozvrh.Models.Timetable;

namespace Rozvrh.Models
{
    /// <summary>
    /// Richard: Dummy data
    /// </summary>
    public class Model : IModel
    {
        // Richard: Temporary, see comment in constructor
        XMLTimetableLoader XmlLoader = new XMLTimetableLoader("C:\\Aktualni_databaze.xml");

        /// <summary>
        /// Class constructor. Inits the properties which are used in View components.
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
        /// Method filtering specializations (zaměření) by given DegreeYear. Result held in 'Specializations' property of Model.
        /// </summary>
        /// <param name="degreeYearId">Id of the given degreeYear</param>
        public void FilterDegreeYear2Specialization(String degreeYearId)
        {
            if (degreeYearId != null)
            {
                var filteredSpecializations =
                    from s in XmlLoader.m_specializations
                    where s.degreeYearId.Equals(degreeYearId)
                    orderby s.acronym
                    select s;

                Specializations = filteredSpecializations.ToList();
            }
        }

        /// <summary>
        /// Method filtering groups (kruhy) by given specialization (zaměření). Result held in 'Groups' property of Model.
        /// </summary>
        /// <param name="specializationId">Id of the given specialization</param>
        public void FilterSpecialization2Groups(String specializationId)
        {
            if (specializationId != null)
            {
                var filteredGroups =
                   from g in XmlLoader.m_groups
                   where g.specializationId.Equals(specializationId)
                   select g;

                Groups = filteredGroups.ToList();
            }
        }

        /// <summary>
        /// Method filtering lecturers by given departments, where employed. Result held in 'Lecturers' property of Model.
        /// </summary>
        /// <param name="departmentIds">Ids of the given departments</param>
        public void FilterDepartments2Lecturers(List<string> departmentIds)
        {
            if (departmentIds != null && departmentIds.Count > 0)
            {
                var filteredLecturers =
                    from l in XmlLoader.m_lecturers
                    where departmentIds.Contains(l.departmentId)
                    orderby l.name
                    select l;

                Lecturers = filteredLecturers.ToList();
            }  
        }

        /// <summary>
        /// Method filtering classrooms by given buildings. Result held in 'Classrooms' property of Model.
        /// </summary>
        /// <param name="buildingIds">Ids of the given buildings</param>
        public void FilterBuildings2Classrooms(List<string> buildingIds)
        {
            if (buildingIds != null && buildingIds.Count > 0)
            {
                var filteredClassrooms =
                    from c in XmlLoader.m_classrooms
                    where buildingIds.Contains(c.buildingId)
                    orderby c.name
                    select c;

                Classrooms = filteredClassrooms.ToList();
            }
        }

        //David: preserved for back compatibility
        public void FilterDepartments2Lecturers(String departmentId)
        {
                var filteredLecturers =
                    from l in XmlLoader.m_lecturers
                    where l.departmentId.Equals(departmentId)
                    orderby l.name
                    select l;

                Lecturers = filteredLecturers.ToList();
        }

        //David: preserved for back compatibility
        public void FilterBuildings2Classrooms(String buildingId)
        {
            var filteredClassrooms =
                from c in XmlLoader.m_classrooms
                where c.buildingId.Equals(buildingId)
                orderby c.name
                select c;

            Classrooms = filteredClassrooms.ToList();
        }

        //David: preserved for back compatibility
        public void FilterAll2TimetableFields(string groupId, string departmentId, string lecturerId, string classroomId, string dayId, string timeId)
        {
            const string NULL = "null";
            var lessonsFromAllFilters = new List<IEnumerable<Lesson>>();

            //pro kompatibilitu
            List<string> groupIds = new List<string>();
            List<string> departmentIds = new List<string>();
            List<string> lecturerIds = new List<string>();
            List<string> classroomIds = new List<string>();
            List<string> dayIds = new List<string>();
            List<string> timeIds = new List<string>();

            if (groupId != NULL) groupIds.Add(groupId);
            if (departmentId != NULL) departmentIds.Add(departmentId);
            if (lecturerId != NULL) lecturerIds.Add(lecturerId);
            if (classroomId != NULL) classroomIds.Add(classroomId);
            if (dayId != NULL) dayIds.Add(dayId);
            if (timeId != NULL) timeIds.Add(timeId);
            //pro kompatibilitu

            filterLessonsByGroups(groupIds, lessonsFromAllFilters);
            if (lecturerIds == null || lecturerIds.Count == 0) //the condition allows lessons of other depertments then the lecturer is member of, but given by the lecturer
                filterLessonsByDepartments(departmentIds, lessonsFromAllFilters);
            filterLessonsByLecturers(lecturerIds, lessonsFromAllFilters);
            filterLessonsByClassrooms(classroomIds, lessonsFromAllFilters);
            filterLessonsByDays(dayIds, lessonsFromAllFilters);
            filterLessonsByTimes(timeIds, lessonsFromAllFilters);

            var resultLessons = intersect(lessonsFromAllFilters);

            var filteredTimetableFields =
                from h in resultLessons
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

        /// <summary>
        /// Method filtering lessons (vyučovací hodiny) by given groups (kruhy), departments (dep. of the course), lecturers, classrooms, days and times.
        /// Result held in 'TimetableFields' property of Model.
        /// </summary>
        /// <param name="groupIds">Ids of the given groups.</param>
        /// <param name="departmentIds">Ids of the given departments.</param>
        /// <param name="lecturerIds">Ids of the given lecturers.</param>
        /// <param name="classroomIds">Ids of the given classrooms.</param>
        /// <param name="dayIds">Ids of the given days.</param>
        /// <param name="timeIds">Ids of the given times.</param>
        public void FilterAll2TimetableFields(List<string> groupIds, List<string> departmentIds, List<string> lecturerIds, 
                                              List<string> classroomIds, List<string> dayIds, List<string> timeIds)
        {
            var lessonsFromAllFilters = new List<IEnumerable<Lesson>>();

            filterLessonsByGroups(groupIds,lessonsFromAllFilters);          
            if(lecturerIds == null || lecturerIds.Count == 0) //allows lessons of other depertments then the lecturer is member of, but given by the lecturer
                filterLessonsByDepartments(departmentIds, lessonsFromAllFilters);
            filterLessonsByLecturers(lecturerIds, lessonsFromAllFilters);    
            filterLessonsByClassrooms(classroomIds, lessonsFromAllFilters);  
            filterLessonsByDays(dayIds, lessonsFromAllFilters);             
            filterLessonsByTimes(timeIds, lessonsFromAllFilters);

            var resultLessons = intersect(lessonsFromAllFilters);

            var filteredTimetableFields =
                from h      in resultLessons
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

        /// <summary>
        /// Method filtering lessons by given groups.
        /// </summary>
        /// <param name="groupIds">Ids of the given groups.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByGroups(List<string> groupIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (groupIds != null && groupIds.Count > 0)
            {
                var timeTableFieldsFromGroupFilter =
                    (from hk in XmlLoader.m_groupLessonBinder
                     where  groupIds.Contains(hk.groupId)
                     from h in XmlLoader.m_lessons
                     where hk.lessonId == h.id
                     select h).ToList();
                lessonsFromAllFilters.Add(timeTableFieldsFromGroupFilter);
            }
        }
       
        /// <summary>
        /// Method filtering lessons by given departments.
        /// </summary>
        /// <param name="departmentIds">Ids of the given departments.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByDepartments(List<string> departmentIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (departmentIds != null && departmentIds.Count > 0)
            {
                var lessons2 =
                    (from c in XmlLoader.m_courses
                     where departmentIds.Contains(c.departmentId)
                     from l in XmlLoader.m_lectures
                     where c.id == l.courseId
                     from h in XmlLoader.m_lessons
                     where l.id == h.lectureId
                     select h).ToList();
                lessonsFromAllFilters.Add(lessons2);
            }
        }

        /// <summary>
        /// Method filtering lessons by given lecturers.
        /// </summary>
        /// <param name="lecturerId">Ids of the given lecturers.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByLecturers(List<string> lecturerIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (lecturerIds != null && lecturerIds.Count > 0)
            {
                var lessons3 =
                    (from h in XmlLoader.m_lessons
                     where lecturerIds.Contains(h.lecturerId)
                     select h).ToList();
                lessonsFromAllFilters.Add(lessons3);
            }
        }

        /// <summary>
        /// Method filtering lessons by given classrooms.
        /// </summary>
        /// <param name="classroomId">Ids of the given classrooms.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByClassrooms(List<string> classroomIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (classroomIds != null && classroomIds.Count > 0)
            {
                var lessons4 =
                    (from h in XmlLoader.m_lessons
                     where classroomIds.Contains(h.classroomId)
                     select h).ToList();
                lessonsFromAllFilters.Add(lessons4);
            }
        }

        /// <summary>
        /// Method filtering lessons by given days.
        /// </summary>
        /// <param name="dayId">Ids of the given days.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByDays(List<string> dayIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (dayIds != null && dayIds.Count > 0)
            {
                var lessons5 =
                    (from h in XmlLoader.m_lessons
                     where dayIds.Contains(h.dayId)
                     select h).ToList();
                lessonsFromAllFilters.Add(lessons5);
            }
        }

        /// <summary>
        /// Method filtering lessons by given times.
        /// </summary>
        /// <param name="timeId">Ids of the given times.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByTimes(List<string> timeIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (timeIds != null && timeIds.Count > 0)
            {
                var lessons6 =
                    (from h in XmlLoader.m_lessons
                     where timeIds.Contains(h.timeId)
                     select h).ToList();
                lessonsFromAllFilters.Add(lessons6);
            }
        }

        /// <summary>
        /// Makes set intersection of the given lists of lessons.
        /// </summary>
        /// <param name="lessonsFromAllFilters">"Lists of lessons in one list."</param>
        /// <returns>List of common lessons.</returns>
        private static IEnumerable<Lesson> intersect(IEnumerable<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            return lessonsFromAllFilters.Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToList());
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