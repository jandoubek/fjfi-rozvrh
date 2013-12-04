using System;
using System.Collections.Generic;
using Rozvrh.Models.Timetable;

namespace Rozvrh.Models
{
    interface IModel
    {
        List<Department>    Departments        { get; }
        List<Specialization>Specializations    { get; } // Scope of study program
        List<Group>         Groups             { get; } // Group of students
        List<DegreeYear>    DegreeYears        { get; }
        List<Lecturer>      Lecturers          { get; }
        List<Building>      Buildings          { get; }
        List<Classroom>     Classrooms         { get; }
        List<Day>           Days               { get; }
        List<Time>          Times              { get; }

        List<TimetableField> FiltredTimetableFields { get; }
        List<TimetableField> CustomTimetableFields { get; }

        List<int> SelectedDegreeYears { get; }
        List<int> SelectedSpecializations { get; }
        List<int> SelectedGroups { get; }
        List<int> SelectedDepartments { get; }
        List<int> SelectedLecturers { get; }
        List<int> SelectedBuildings { get; }
        List<int> SelectedClassrooms { get; }
        List<int> SelectedDays { get; }
        List<int> SelectedTimes { get; }
    }   
}
