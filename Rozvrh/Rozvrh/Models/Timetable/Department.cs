
namespace Rozvrh.Models.Timetable
{
    public class Department
    {
        public string id        { get; private set; }
        public string code      { get; private set; }
        public string name      { get; private set; }
        public string acronym   { get; private set; }
        public string color     { get; private set; }
        
        public Department(string id, string code, string name, string acronym, string color)
        {
            this.id = id;
            this.code = code;
            this.name = name;
            this.acronym = acronym;
            this.color = color;
        }
    }//= department v xml
}