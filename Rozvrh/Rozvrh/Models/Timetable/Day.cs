using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Day
    {
        public string id            { get; private set; }
        public string name          { get; private set; }
        public string daysOrder     { get; private set; }
       
        public Day(string id, string name, string daysOrder)
        {
            this.id = id;
            this.name = name;
            this.daysOrder = daysOrder;
        }
    }//= day v xml
}