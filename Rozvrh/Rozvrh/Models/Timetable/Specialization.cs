
namespace Rozvrh.Models.Timetable
{
    public class Specialization
    {
        public string id            { get; private set; }
        public string name          { get; private set; }
        public string acronym       { get; private set; }
        public string degreeYearId  { get; private set; }

        public Specialization(string id, string name, string acronym, string degreeYearId)
        {
            this.id = id;
            this.name = name;
            this.acronym = acronym;
            this.degreeYearId = degreeYearId;
        }
    }//= group v xml
}