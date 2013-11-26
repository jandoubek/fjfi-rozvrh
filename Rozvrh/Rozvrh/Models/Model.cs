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

        /// <summary>
        /// Class constructor. Inits the properties which are used in View components.
        /// </summary>
        public Model()
        {
            xmlTimetable = XMLTimetable.Instance;
            loadData();
        }

        /// <summary>
        /// Class constructor. Inits the properties which are used in View components from given instance XMLTimetable - should be used only for unit testing.
        /// </summary>
        /// <param name="timetableData"></param>
        public Model(IXMLTimetable timetableData)
        {
            xmlTimetable = timetableData;
            loadData();
        }

        /// <summary>
        /// Loads data from XMLTimetable class and initializes all property fields.
        /// </summary>
        private void loadData()
        {
            try
            {
                Departments = xmlTimetable.m_departments;
                DegreeYears = xmlTimetable.m_degreeyears;
                Buildings = xmlTimetable.m_buildings;
                Days = xmlTimetable.m_days;
                Times = xmlTimetable.m_times;

                Specializations = new List<Specialization>();
                Groups = new List<Group>();
                Lecturers = new List<Lecturer>();
                Classrooms = new List<Classroom>();


                SelectedDegreeYears = new List<int>();
                SelectedSpecializations = new List<int>();
                SelectedGroups = new List<int>();
                SelectedDepartments = new List<int>();
                SelectedLecturers = new List<int>();
                SelectedBuildings = new List<int>();
                SelectedClassrooms = new List<int>();
                SelectedDays = new List<int>();
                SelectedTimes = new List<int>();
            }
            catch (Exception e)
            {
                // FIX ME !!! - implement proper logging
                System.Diagnostics.Debug.WriteLine("Error when parsing timetable XML.");
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }
        }

        private IXMLTimetable xmlTimetable { get; set; }

        /// <summary>
        /// Method filtering specializations (zaměření) by given degreeYears. Specializations are visible just only one degreeYear is selected.
        /// Result held in 'Specializations' property of Model.
        /// </summary>
        /// <param name="degreeYearIds"></param>
        public void FilterSpecializationsByDegreeYears(List<string> degreeYearIds)
        {
            if (anyId(degreeYearIds) && degreeYearIds.Count == 1)
            {
                var filteredSpecializations =
                    from s in xmlTimetable.m_specializations
                    where degreeYearIds.Contains(s.degreeYearId)
                    orderby s.acronym
                    select s;

                Specializations = filteredSpecializations.ToList();
            }
        }

        /// <summary>
        /// Method filtering groups (kruhy) by given specializations (zaměření). Groups are visible when just one degreeYear is selected.
        /// Result held in 'Groups' property of Model.
        /// </summary>
        /// <param name="specializationIds"></param>
        public void FilterGroupsBySpecializations(List<string> specializationIds)
        {
            if (anyId(specializationIds) && specializationIds.Count == 1)
            {
                var filteredGroups =
                   from g in xmlTimetable.m_groups
                   where specializationIds.Contains(g.specializationId)
                   select g;

                Groups = filteredGroups.ToList();
            }
        }

        /// <summary>
        /// Method filtering lecturers by given departments, where employed. Result held in 'Lecturers' property of Model.
        /// </summary>
        /// <param name="departmentIds">Ids of the given departments</param>
        public void FilterLecturersByDepartments(List<string> departmentIds)
        {
            if (anyId(departmentIds))
            {
                var filteredLecturersByDepartments =
                    from l in xmlTimetable.m_lecturers
                    where departmentIds.Contains(l.departmentId)
                    orderby l.name
                    select l;



                Lecturers = filteredLecturersByDepartments.ToList();
            }
        }

        /// <summary>
        /// Method filtering classrooms by given buildings. Result held in 'Classrooms' property of Model.
        /// </summary>
        /// <param name="buildingIds">Ids of the given buildings</param>
        public void FilterClassroomsByBuildings(List<string> buildingIds)
        {
            if (anyId(buildingIds))
            {
                var filteredClassrooms =
                    from c in xmlTimetable.m_classrooms
                    where buildingIds.Contains(c.buildingId)
                    orderby c.name
                    select c;

                Classrooms = filteredClassrooms.ToList();
            }
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
        public void FilterTimetableFieldsByAll(List<string> degreeYearIds,  List<string> specializationIds,  List<string> groupIds,
                                               List<string> departmentIds,  List<string> lecturerIds,        List<string> buildingIds,
                                               List<string> classroomIds,   List<string> dayIds,             List<string> timeIds)
        {
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

            
            var resultLessons = intersect(lessonsFromAllFilters);

            var filteredTimetableFields =
                from h in resultLessons
                join lec in xmlTimetable.m_lectures on h.lectureId equals lec.id
                join c in xmlTimetable.m_courses on lec.courseId equals c.id
                join dep in xmlTimetable.m_departments on c.departmentId equals dep.id
                join ler in xmlTimetable.m_lecturers on h.lecturerId equals ler.id
                join d in xmlTimetable.m_days on h.dayId equals d.id
                join t in xmlTimetable.m_times on h.timeId equals t.id
                join cr in xmlTimetable.m_classrooms on h.classroomId equals cr.id
                join b in xmlTimetable.m_buildings on cr.buildingId equals b.id
                orderby dep.code, c.acronym, lec.practice, ler.name, d.daysOrder, t.timesOrder, b.name, cr.name
                select new TimetableField(dep, c, lec, ler, d, t, b, cr);

            TimetableFields = filteredTimetableFields.ToList();
        }

        /// <summary>
        /// Method filtering lessons by given degreeYear.
        /// </summary>
        /// <param name="degreeYearIds">Ids of the given degreeYears</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByDegreeYears(List<string> degreeYearIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (anyId(degreeYearIds))
            {
                var specializationIdsByDegreeYear =
                    from s in xmlTimetable.m_specializations
                    where degreeYearIds.Contains(s.degreeYearId)
                    select s.id;

                filterLessonsBySpecializations(specializationIdsByDegreeYear.ToList(), lessonsFromAllFilters);
            }
        }

        /// <summary>
        /// Method filtering lessons by given specializations.
        /// </summary>
        /// <param name="specializationIds">Ids of the given specializations</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsBySpecializations(List<string> specializationIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (anyId(specializationIds))
            {
                var groupIdsBySpecializations =
                    from g in xmlTimetable.m_groups
                    where specializationIds.Contains(g.specializationId)
                    select g.id;

                filterLessonsByGroups(groupIdsBySpecializations.ToList(), lessonsFromAllFilters);
            }
        }

        /// <summary>
        /// Method filtering lessons by given groups.
        /// </summary>
        /// <param name="groupIds">Ids of the given groups.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByGroups(List<string> groupIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (anyId(groupIds))
            {
                var lessonsFilteredByGroups =
                    (
                     from hk in xmlTimetable.m_groupLessonBinder
                     where groupIds.Contains(hk.groupId)
                     from h in xmlTimetable.m_lessons
                     where hk.lessonId == h.id
                     select h
                     ).Distinct();
                lessonsFromAllFilters.Add(lessonsFilteredByGroups);
            }
        }

        /// <summary>
        /// Method filtering lessons by given departments.
        /// </summary>
        /// <param name="departmentIds">Ids of the given departments.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByDepartments(List<string> departmentIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (anyId(departmentIds))
            {
                var lessonsFilteredByDepartments =
                     from c in xmlTimetable.m_courses
                     where departmentIds.Contains(c.departmentId)
                     from l in xmlTimetable.m_lectures
                     where c.id == l.courseId
                     from h in xmlTimetable.m_lessons
                     where l.id == h.lectureId
                     select h;
                lessonsFromAllFilters.Add(lessonsFilteredByDepartments);
            }
        }

        /// <summary>
        /// Method filtering lessons by given lecturers.
        /// </summary>
        /// <param name="lecturerIds">Ids of the given lecturers.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByLecturers(List<string> lecturerIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (anyId(lecturerIds))
            {
                var lessonsFilteredByLecturers =
                     from h in xmlTimetable.m_lessons
                     where lecturerIds.Contains(h.lecturerId)
                     select h;
                lessonsFromAllFilters.Add(lessonsFilteredByLecturers);
            }
        }

        /// <summary>
        /// Method filtering lessons by given buildings, used only when no classroom is selected.
        /// </summary>
        /// <param name="buildingIds">Ids of the given buildings.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByBuildings(List<string> buildingIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (anyId(buildingIds))
            {
                var classroomIdsFilteredByBuildings =
                    from cr in xmlTimetable.m_classrooms
                    where buildingIds.Contains(cr.buildingId)
                    select cr.id;

                filterLessonsByClassrooms(classroomIdsFilteredByBuildings.ToList(), lessonsFromAllFilters);
            }
        }

        /// <summary>
        /// Method filtering lessons by given classrooms.
        /// </summary>
        /// <param name="classroomIds">Ids of the given classrooms.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByClassrooms(List<string> classroomIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (anyId(classroomIds))
            {
                var lessonsFilteredByClassrooms =
                     from h in xmlTimetable.m_lessons
                     where classroomIds.Contains(h.classroomId)
                     select h;
                lessonsFromAllFilters.Add(lessonsFilteredByClassrooms);
            }
        }

        /// <summary>
        /// Method filtering lessons by given days.
        /// </summary>
        /// <param name="dayIds">Ids of the given days.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByDays(List<string> dayIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if (anyId(dayIds))
            {
                var lessonsFilteredByDays =
                     from h in xmlTimetable.m_lessons
                     where dayIds.Contains(h.dayId)
                     select h;
                lessonsFromAllFilters.Add(lessonsFilteredByDays);
            }
        }

        /// <summary>
        /// Method filtering lessons by given times.
        /// </summary>
        /// <param name="timeIds">Ids of the given times.</param>
        /// <param name="lessonsFromAllFilters">Collection where add partial filter result.</param>
        private void filterLessonsByTimes(List<string> timeIds, ICollection<IEnumerable<Lesson>> lessonsFromAllFilters)
        {
            if ( anyId(timeIds) )
            {
                var lessonsFilteredByTime =
                     from h in xmlTimetable.m_lessons
                     where timeIds.Contains(h.timeId)
                     select h;
                lessonsFromAllFilters.Add(lessonsFilteredByTime);
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

        /// <summary>
        /// Return true when there is any id, false for empty list.
        /// </summary>
        /// <param name="Ids">list of ids</param>
        /// <returns>true when nonempty, false otherwise</returns>
        private static bool anyId(List<string> ids)
        {
            if (ids != null && ids.Count > 0)
                return true;
            else
                return false;
        }

        //Olda: Metoda, která podle nastavení filtrů vrátí seznam TimetableFieldů, by měla být v Controlleru.
        //      V tuhle chvíli tedy v souboru HomeController, v metodě Filter.
        //      Jedná se totiž o aplikační logiku, nikoliv datovou.

        public List<Department> Departments { get; private set; }
        public List<Specialization> Specializations { get; private set; }
        public List<Group> Groups { get; private set; }
        public List<DegreeYear> DegreeYears { get; private set; }
        public List<Lecturer> Lecturers { get; private set; }
        public List<Building> Buildings { get; private set; }
        public List<Classroom> Classrooms { get; private set; }
        public List<Day> Days { get; private set; }
        public List<Time> Times { get; private set; }
        public List<TimetableField> TimetableFields { get; private set; }

        public List<int> SelectedDegreeYears { get; set; }
        public List<int> SelectedSpecializations { get; set; }
        public List<int> SelectedGroups { get; set; }
        public List<int> SelectedDepartments { get; set; }
        public List<int> SelectedLecturers { get; set; }
        public List<int> SelectedBuildings { get; set; }
        public List<int> SelectedClassrooms { get; set; }
        public List<int> SelectedDays { get; set; }
        public List<int> SelectedTimes { get; set; }
    }
}