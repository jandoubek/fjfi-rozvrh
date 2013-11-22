
namespace Rozvrh.Models.Timetable
{
    public class Lecturer
    {
        public string id            { get; private set; }
        public string name          { get; private set; }
        public string departmentId  { get; private set; }
        
        public Lecturer(string id, string name, string forname, string departmentId)
        {
            this.id = id;
            this.name = name;
            if (forname.Length != 0)               
                this.name = name + " " + forname[0] + ".";
            this.departmentId = departmentId;
        }
    }//= lecturer v xml
}