using System;

namespace Rozvrh.Models.Timetable
{
    public class Time
    {
        public string id { get; private set; }
        public string hours { get; private set; }
        public string minutes { get; private set; }
        public string timesOrder { get; private set; }
        public string acronym { get; private set; }

        public Time(string id, string hours, string minutes, string timesOrder)
        {

            this.id = id;
            this.hours = hours;
            this.minutes = minutes;
            this.timesOrder = timesOrder;
            this.acronym = hours + ":" + Convert.ToInt32(minutes).ToString("00");
        }
    }//= time v xml
}