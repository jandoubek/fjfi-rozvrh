using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Rozrvh.Exporters.Common
{
    /// <summary>
    /// Implementation of IExportHodina which is also XML serializable.
    /// </summary>
    public class ExportHodina : IExportHodina
    {
        private string name;
        private DayOfWeek day;
        private DateTime startTime;
        private TimeSpan length;
        private string lecturer;
        private string room;

        public ExportHodina(string n,DayOfWeek dow,DateTime st, TimeSpan len, string lec,
            string r) {
            name = n;
            day = dow;
            startTime = st;
            length = len;
            lecturer = lec;
            room = r;
        }


        [XmlElement("NazevPredmetu")]
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

        [XmlElement("Den")]
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

        [XmlElement("ZacatekHodiny")]
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

        [XmlIgnore]
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

        [XmlElement("Delka")]
        public long LengthTicks
        {
            get { return length.Ticks; }
            set { length = new TimeSpan(value); }
        }

        [XmlElement("Prednasejici")]
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

        [XmlElement("Mistonost")]
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