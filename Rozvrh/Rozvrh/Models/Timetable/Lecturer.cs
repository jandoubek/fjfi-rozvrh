using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Lecturer
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public string forname { get; private set; }
        public int departmentId { get; private set; }
        public Lecturer(int id, string name, string forname, int departmentId)
        {
            this.id = id;
            this.name = name;
            this.forname = forname;
            this.departmentId = departmentId;
        }
    }//= lecturer v xml
}