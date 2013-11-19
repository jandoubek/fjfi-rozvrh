using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Building
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public Building(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }//= building v xml
}