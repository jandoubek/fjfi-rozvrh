
namespace Rozvrh.Models.Timetable
{
    public class GroupTeachingBinder
    {
        public string id             { get; private set; }
        public string groupId        { get; private set; }
        public string teachingId     { get; private set; }

        public GroupTeachingBinder(string id, string groupId, string teachingId)
        {
            this.id = id;
            this.groupId = groupId;
            this.teachingId = teachingId;
        }
    }//- je vybráno z part elementů

}