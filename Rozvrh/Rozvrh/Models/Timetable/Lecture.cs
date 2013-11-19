using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Lecture
    {
        public int id { get; private set; }
        public int courseId { get; private set; }
        public int practice { get; private set; }
        public string tag { get; private set; }
        public int duration { get; private set; }
        public int period { get; private set; }
        public Lecture(int id, int courseId, int practice, string tag, int duration, int period)
        {
            this.id = id;
            this.courseId = courseId;
            this.practice = practice;
            this.tag = tag;
            this.duration = duration;
            this.period = period;
        }
    }//= lecture v xml
}