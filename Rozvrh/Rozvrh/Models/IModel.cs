using System;
using System.Collections.Generic;

namespace Rozvrh.Models
{
    interface IModel
    {
        List<String> Departments { get; } 
        List<String> Specializations { get; }  // Scope of study program
        List<String> Groups { get; } // Group of students
        List<String> DegreeYears { get; } 
        List<String> Lecturers { get; } 
        List<String> Buildings { get; } 
        List<String> Classrooms { get; }
        List<String> Days { get; }
        List<String> Times { get; }

    }   
}
