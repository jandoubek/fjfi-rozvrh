using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Course
    {
        public string id            { get; private set; }
        public string departmentId  { get; private set; }
        public string name          { get; private set; }
        public string acronym       { get; private set; }
        
        public Course(string id, string departmentId, string name, string acronym)
        {
            this.id = id;
            this.departmentId = departmentId;
            this.name = name;
            this.acronym = acronym;
        }
    }//= course v xml
}