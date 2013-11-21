using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Hodina
    {
        public string id            { get; private set; }
        public string lectureId     { get; private set; }
        public string lecturerId    { get; private set; }
        public string dayId         { get; private set; }
        public string timeId        { get; private set; }
        public string classroomId   { get; private set; }
        public string tag           { get; private set; }
       
        public Hodina(string id, string lectureId, string lecturerId, string dayId, string timeId, string classroomId, string tag)
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