using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class Model:IModel
    {
        public List<string> departments
        {
            get;
        }

        public List<string> courses
        {
            get;
        }

        public List<string> groups
        {
            get;
        }

        public List<string> years
        {
            get;
        }

        public List<string> lecturers
        {
            get;
        }

        public List<string> classrooms
        {
            get;
        }

        public List<DayOfWeek> days
        {
            get;
        }

        public List<TimeSpan> times
        {
            get;
        }

        public List<string> getParts(string group)
        {
            throw new NotImplementedException();
        }

        public List<TimetableField> getTimeTable(List<string> filterValues)
        {
            throw new NotImplementedException();
        }
    }
}