
namespace Rozvrh.Models.Timetable
{
    public class Lecturer
    {
        public string id            { get; private set; }
        public string name          { get; private set; }
        public string forname       { get; private set; }
        public string departmentId  { get; private set; }
        
        public Lecturer(string id, string name, string forname, string departmentId)
        {
            this.id = id;
            this.name = name;
            this.forname = forname;
            this.departmentId = departmentId;
        }
    }//= lecturer v xml
}