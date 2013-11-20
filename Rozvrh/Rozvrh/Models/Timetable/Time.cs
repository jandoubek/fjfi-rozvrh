using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Time
    {
        public int id { get; private set; }
        public int hours { get; private set; }
        public int minutes { get; private set; }
        public int timesOrder { get; private set; }
        public String acronym { get; private set; }
        public Time(int id, int hours, int minutes, int timesOrder)
        {
            this.id = id;
            this.hours = hours;
            this.minutes = minutes;
            this.timesOrder = timesOrder;
            this.acronym = hours.ToString() + ":" + minutes.ToString();
        }
    }//= time v xml
}