using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rozrvh.Exporters.ICal
{
    public class DownloadAsICalController : Controller
    {
        //
        // GET: /DownloadAsICal/

        public String Index()
        {
            return "Download as iCal file.";
        }

        //
        // GET: /DownloadAsICal/Download/

        public ActionResult Download() 
        {
            var hodiny = new List<IExportHodina>();
            //For testing
            prototypeData(hodiny);

            DateTime semStart = new DateTime();
            DateTime semEnd = new DateTime();
            ICalGenerator gen = new ICalGenerator();
            string text = gen.generateICal(hodiny, semStart, semEnd);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            return File(buffer, "text/plain", "FJFIrozvrh.ical");
        }

        public static void prototypeData(List<IExportHodina> dataToFill)
        {
            var vsbp = new ExportHodina("VSBP", DayOfWeek.Monday, 
                new DateTime(1,1,1,14,30,0), new TimeSpan(2,0,0), "Vopalka", "B-314");            
            var tpc = new ExportHodina("TPC", DayOfWeek.Tuesday, 
                new DateTime(1,1,1,8,30,0), new TimeSpan(2,0,0), "Stamberg", "---");
            var rao = new ExportHodina("RAO", DayOfWeek.Tuesday,
                new DateTime(1, 1, 1, 13, 30, 0), new TimeSpan(4, 0, 0), "Vrba", "B-115");
            var chrp = new ExportHodina("CHRP", DayOfWeek.Wednesday,
                new DateTime(1, 1, 1, 10, 30, 0), new TimeSpan(2, 0, 0), "John", "B-314");

            dataToFill.Add(vsbp);
            dataToFill.Add(tpc);
            dataToFill.Add(rao);
            dataToFill.Add(chrp);
        }

        
    }
}
