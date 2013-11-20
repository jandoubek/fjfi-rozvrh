using System;
using System.Collections.Generic;
using Rozvrh.Models.Timetable;

namespace Rozvrh.Models
{
    interface IModel
    {
        List<Department> Departments { get; } 
        List<Zamereni> Specializations { get; }  // Scope of study program
        List<Kruh> Groups { get; } // Group of students
        List<DegreeYear> DegreeYears { get; } 
        List<Lecturer> Lecturers { get; } 
        List<Building> Buildings { get; } 
        List<Classroom> Classrooms { get; }
        List<Day> Days { get; }
        List<Time> Times { get; }

    }   
}
