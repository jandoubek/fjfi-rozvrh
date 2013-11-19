using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Course
    {
        public int id { get; private set; }
        public int departmentId { get; private set; }
        public string name { get; private set; }
        public string acronym { get; private set; }
        public Course(int id, int departmentId, string name, string acronym)
        {
            this.id = id;
            this.departmentId = departmentId;
            this.name = name;
            this.acronym = acronym;
        }
    }//= course v xml
}