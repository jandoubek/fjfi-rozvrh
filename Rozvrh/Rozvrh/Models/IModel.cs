using System;
using System.Collections.Generic;

namespace Rozvrh.Models
{
    interface IModel
    {
        List<String> Departments { get; }
        List<String> Courses { get; }
        List<String> Groups { get; }
        List<String> Years { get; }
        List<String> Lecturers { get; }
        List<String> Classrooms { get; }
        List<String> Days { get; }
        List<String> Times { get; }

        List<String> GetParts(String group);
    }   
}
