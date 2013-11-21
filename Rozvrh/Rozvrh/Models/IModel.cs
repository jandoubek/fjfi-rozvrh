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

        List<TimetableField> TimetableFields { get; }
    }   
}
