using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApplication1.Models
{
    interface IModel
    {
        // Getters to get filters values
        List<String> departments { get; }
        List<String> courses { get; }
        List<String> groups { get; }
        List<String> years { get; }
        List<String> lecturers { get; }
        List<String> classrooms { get; }
        List<DayOfWeek> days { get; }
        List<TimeSpan> times { get; }

        List<String> getParts(String group);

        // most important core function to get the timetable to render
        List<TimetableField> getTimeTable(List<String> filterValues);

    }   
}
