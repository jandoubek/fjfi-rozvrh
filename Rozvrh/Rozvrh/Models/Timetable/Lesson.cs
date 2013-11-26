
namespace Rozvrh.Models.Timetable
{
    /// <summary>
    /// Class representing teaching lesson. (Former Teaching, Hodina, Card in xml file)
    /// Is equal to a Card in the source XML file.
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
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (id != null ? id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (lectureId != null ? lectureId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (lecturerId != null ? lecturerId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (dayId != null ? dayId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (timeId != null ? timeId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (classroomId != null ? classroomId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (tag != null ? tag.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Lesson) obj);
        }

        protected bool Equals(Lesson other)
        {
            return string.Equals(id, other.id) && string.Equals(lectureId, other.lectureId) && string.Equals(lecturerId, other.lecturerId) && string.Equals(dayId, other.dayId) && string.Equals(timeId, other.timeId) && string.Equals(classroomId, other.classroomId) && string.Equals(tag, other.tag);
        }
    }
}