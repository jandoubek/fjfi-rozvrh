
namespace Rozvrh.Models.Timetable
{
    public class GroupLessonBinder
    {
        public string id             { get; private set; }
        public string groupId        { get; private set; }
        public string lessonId     { get; private set; }

        public GroupLessonBinder(string id, string groupId, string lessonId)
        {
            this.id = id;
            this.groupId = groupId;
            this.lessonId = lessonId;
        }
    }//- je vybráno z part elementů xml

}