using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Day
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public int daysOrder { get; private set; }
        public Day(int id, string name, int daysOrder)
        {
            this.id = id;
            this.name = name;
            this.daysOrder = daysOrder;
        }
    }//= day v xml
}