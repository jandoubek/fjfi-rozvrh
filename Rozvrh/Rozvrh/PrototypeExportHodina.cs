using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozrvh
{
    public class PrototypeExportHodina : IExportHodina
    {
        private string name;
        private DayOfWeek day;
        private DateTime startTime;
        private TimeSpan length;
        private string lecturer;
        private string room;

        public PrototypeExportHodina(string n,DayOfWeek dow,DateTime st, TimeSpan len, string lec,
            string r) {
            name = n;
            day = dow;
            startTime = st;
            length = len;
            lecturer = lec;
            room = r;
        }



        public string Name 
        {
            get 
            {
                return name;
            }

            set
            {
               name = value;
            }
        }

        public System.DayOfWeek Day
        {
            get
            {
                return day;
            }

            set
            {
                day = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return startTime;
            }

            set
            {
                startTime = value;
            }
        }

        public TimeSpan Length
        {
            get
            {
                return length;
            }

            set
            {
                length = value;
            }
        }

        public string Lecturer
        {
            get
            {
                return lecturer;
            }

            set
            {
                lecturer = value;
            }
        }

        public string Room
        {
            get
            {
                return room;
            }

            set
            {
                room = value;
            }
        }

    }
}