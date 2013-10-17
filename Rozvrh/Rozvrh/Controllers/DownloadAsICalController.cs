using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rozrvh.Controllers
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
            string text = generateICal();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            return File(buffer, "text/plain", "FJFIrozvrh.ical");
        }

        private string generateICal() 
        {
            string header = @"BEGIN:VCALENDAR 
VERSION:2.0
PRODID:-//hacksw/handcal//NONSGML v1.0//EN";
            string tail = "END:VCALENDAR";
            string datum = "20131015";
            string timeStart = "T093000";
            string timeEnd = "T113000";
            string konecSemestru = "20131115";
            string body = @"BEGIN:VEVENT
UID:honzik.vaclav@gmail.com
DTSTAMP:"+datum+timeStart+@"
DTSTART:"+datum+timeStart+@"
DTEND:"+datum+timeEnd+@"
RRULE:FREQ=WEEKLY;UNTIL="+konecSemestru+@"T000000Z
SUMMARY:Prednaska z MAB 01
END:VEVENT";
            return header + System.Environment.NewLine + body + System.Environment.NewLine + tail; 
        }

    }
}
