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
            get { throw new NotImplementedException(); }
        }

        public List<string> courses
        {
            get { throw new NotImplementedException(); }
        }

        public List<string> groups
        {
            get { throw new NotImplementedException(); }
        }

        public List<string> years
        {
            get { throw new NotImplementedException(); }
        }

        public List<string> lecturers
        {
            get { throw new NotImplementedException(); }
        }

        public List<string> classrooms
        {
            get { throw new NotImplementedException(); }
        }

        public List<DayOfWeek> days
        {
            get { throw new NotImplementedException(); }
        }

        public List<TimeSpan> times
        {
            get { throw new NotImplementedException(); }
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