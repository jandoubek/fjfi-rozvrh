
namespace Rozvrh.Models.Timetable
{
    public class Lecture
    {
        public string id        { get; private set; }
        public string courseId  { get; private set; }
        public string practice  { get; private set; }
        public string tag       { get; private set; }
        public string duration  { get; private set; }
        public string period    { get; private set; }
        
        public Lecture(string id, string courseId, string practice, string tag, string duration, string period)
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