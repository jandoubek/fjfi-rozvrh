
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
            this.color = tempColorRepair(acronym);
        }

        private string tempColorRepair(string acronym)
        {
            switch(acronym)
            {
                case "KM":
                    return "7590422";
                case "KF":
                    return "16153724";
                case "KJ":
                    return "11184810";
                case "KIPL":
                    return "13369344";
                case "KFE":
                    return "16087296";
                case "KMAT":
                    return "52411";
                case "KJCH":
                    return "7688315";
                case "KDAIZ":
                    return "3433892";
                case "KJR":
                    return "15586304";
                case "KSI":
                    return "12680465";
                default:
                    return "11184810";
            }
            
        }
    }//= department v xml
}