
namespace Rozvrh.Models.Timetable
{
    public class Day
    {
        public string id            { get; private set; }
        public string name          { get; private set; }
        public string daysOrder     { get; private set; }
       
        public Day(string id, string name, string daysOrder)
        {
            this.id = id;
            this.name = tempColorRepair(daysOrder);
            this.daysOrder = daysOrder;
        }

        private string tempColorRepair(string daysOrder)
        {
            switch (daysOrder)
            {
                case "0":
                    return "Pondělí";
                case "1":
                    return "Úterý";
                case "2":
                    return "Středa";
                case "3":
                    return "Čtvrtek";
                case "4":
                    return "Pátek";
                default:
                    return "";
            }

        }
    }//= day v xml
}