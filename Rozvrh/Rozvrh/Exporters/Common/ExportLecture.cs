using Rozvrh.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;

namespace Rozvrh.Exporters.Common
{
    /// <summary>
    /// Class used for export to SVG and ICal.
    /// </summary>
    public class ExportLecture
    {
        /// <summary>
        /// Short name of the lecture - abbreviation.
        /// </summary>
        private string name;

        /// <summary>
        /// Day of the week of the lecture.
        /// </summary>
        private DayOfWeek day;

        /// <summary>
        /// Start time of the lecture.
        /// </summary>
        /// <remarks>Only Hour and Minute fields are used.</remarks>
        private DateTime startTime;

        /// <summary>
        /// Length of the lecture.
        /// </summary>
        /// <remarks>
        /// This is added to StartTime in ICalExport to determine the end time from which only Hour and Minute are used. 
        /// In SVG export it's continously mapped on pixels.
        /// </remarks>
        private TimeSpan length;

        /// <summary>
        /// Last name of the lecturer.
        /// </summary>
        private string lecturer;

        /// <summary>
        /// Abbreviation of the lecture's room.
        /// </summary>
        private string room;

        /// <summary>
        /// Color code of the departement in Hex code.
        /// </summary>
        /// <remarks>
        /// E.g. #999999
        /// </remarks>
        private string departementColor;

        public ExportLecture(string n,DayOfWeek dow,DateTime st, TimeSpan len, string lec,
            string r,string dep) {
            name = n;
            day = dow;
            startTime = st;
            length = len;
            lecturer = lec;
            room = r;
            departementColor = dep;
        }

        public ExportLecture(TimetableField ttf)
        {
            name = ttf.lecture_acr;
            //day order ranges from 0 to 4, DayOfWeek from 1 to 5
            day = (DayOfWeek)Enum.ToObject(typeof(DayOfWeek), Int32.Parse(ttf.day_order) + 1);
            startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
                Int32.Parse(ttf.time_hours), Int32.Parse(ttf.time_minutes), 0);
            length = new TimeSpan(Int32.Parse(ttf.duration),0,0);
            lecturer = ttf.lecturer;
            room = ttf.classroom;
            departementColor = "#"+Int32.Parse(ttf.color).ToString("X6");
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

        public long LengthTicks
        {
            get { return length.Ticks; }
            set { length = new TimeSpan(value); }
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


        public string DepartementColor
        {
            get
            {
                return departementColor;
            }

            set
            {
                departementColor = value;
            }
        }

    }
}