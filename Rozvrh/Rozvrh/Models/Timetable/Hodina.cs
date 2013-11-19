using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Hodina
    {
        public int id { get; private set; }
        public int lectureId { get; private set; }
        public int lecturerId { get; private set; }
        public int dayId { get; private set; }
        public int timeId { get; private set; }
        public int classroomId { get; private set; }
        public string tag { get; private set; }
        public Hodina(int id, int lectureId, int lecturerId, int dayId, int timeId, int classroomId, string tag)
        {
            this.id = id;
            this.lectureId = lectureId;
            this.lecturerId = lecturerId;
            this.dayId = dayId;
            this.timeId = timeId;
            this.classroomId = classroomId;
            this.tag = tag;
        }
    }//= hodina v xml
}