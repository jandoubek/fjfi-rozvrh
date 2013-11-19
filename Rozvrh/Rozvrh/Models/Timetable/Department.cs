using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Department
    {
        public int id { get; private set; }
        public int code { get; private set; }
        public string name { get; private set; }
        public string acronym { get; private set; }
        public ulong color { get; private set; }
        public Department(int id, int code, string name, string acronym, ulong color)
        {
            this.id = id;
            this.code = code;
            this.name = name;
            this.acronym = acronym;
            this.color = color;
        }
    }//= department v xml
}