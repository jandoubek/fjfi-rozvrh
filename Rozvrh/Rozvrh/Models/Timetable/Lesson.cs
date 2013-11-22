
namespace Rozvrh.Models.Timetable
{
    /// <summary>
    /// Class representing teaching lesson. (Former Teaching, Hodina, Card in xml file)
    /// </summary>
    public class Lesson
    {
        public string id            { get; private set; }
        public string lectureId     { get; private set; }
        public string lecturerId    { get; private set; }
        public string dayId         { get; private set; }
        public string timeId        { get; private set; }
        public string classroomId   { get; private set; }
        public string tag           { get; private set; }

        public Lesson(string id, string lectureId, string lecturerId, string dayId, string timeId, string classroomId, string tag)
        {
            this.id = id;
            this.lectureId = lectureId;
            this.lecturerId = lecturerId;
            this.dayId = dayId;
            this.timeId = timeId;
            this.classroomId = classroomId;
            this.tag = tag;
        }
    }//= card v xml
}