using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Building
    {
        public string id        { get; private set; }
        public string name      { get; private set; }
        
        public Building(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }//= building v xml
}