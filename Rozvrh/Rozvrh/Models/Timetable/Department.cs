
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
            this.color = tempColorRepair();
        }

        private string tempColorRepair()
        {
            switch(code)
            {
                case "01":
                    return "7590422";
                case "02":
                    return "16153724";
                case "04":
                    return "11184810";
                case "11":
                    return "13369344";
                case "12":
                    return "16087296";
                case "14":
                    return "52411";
                case "15":
                    return "7688315";
                case "16":
                    return "3433892";
                case "17":
                    return "15586304";
                case "18":
                    return "12680465";
                default:
                    return "11184810";
            }
            
        }
    }//= department v xml
}