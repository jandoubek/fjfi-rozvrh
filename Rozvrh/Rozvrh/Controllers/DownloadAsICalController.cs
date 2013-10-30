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
            var hodiny = new List<IExportHodina>();
            prototypeData(hodiny);
            string text = generateICal(hodiny);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            return File(buffer, "text/plain", "FJFIrozvrh.ical");
        }

        public static void prototypeData(List<IExportHodina> dataToFill)
        {
            var vsbp = new PrototypeExportHodina("VSBP", DayOfWeek.Monday, 
                new DateTime(1,1,1,14,30,0), new TimeSpan(2,0,0), "Vopalka", "B-314");            
            var tpc = new PrototypeExportHodina("TPC", DayOfWeek.Tuesday, 
                new DateTime(1,1,1,8,30,0), new TimeSpan(2,0,0), "Stamberg", "---");
            var rao = new PrototypeExportHodina("RAO", DayOfWeek.Tuesday,
                new DateTime(1, 1, 1, 13, 30, 0), new TimeSpan(4, 0, 0), "Vrba", "B-115");
            var chrp = new PrototypeExportHodina("CHRP", DayOfWeek.Wednesday,
                new DateTime(1, 1, 1, 10, 30, 0), new TimeSpan(2, 0, 0), "John", "B-314");

            dataToFill.Add(vsbp);
            dataToFill.Add(tpc);
            dataToFill.Add(rao);
            dataToFill.Add(chrp);
        }

        private string generateICal(List<IExportHodina> hodiny) 
        {
            string header = "BEGIN:VCALENDAR" + System.Environment.NewLine + "VERSION:2.0" +
                System.Environment.NewLine + "PRODID:-//hacksw/handcal//NONSGML v1.0//EN" +
                System.Environment.NewLine;
            string body = "";

            foreach (IExportHodina hodina in hodiny)
            {
                body += generateEventFromExportHodina(hodina);
            }

            string tail = "END:VCALENDAR";


            return header + body + tail; 
        }


        private string generateEventFromExportHodina(IExportHodina hodina)
        {
            string semStart = dateTimeDateToICalString(
                closestDayFromDateTime(semesterStart(),hodina.Day));
            string semEnd = dateTimeDateToICalString(semesterEnd());

            string head = "BEGIN:VEVENT" + System.Environment.NewLine;
            string stamp = "DTSTAMP:" + dateTimeDateToICalString(DateTime.Today) +
                hourToICalString(DateTime.Now.Hour,DateTime.Now.Minute)+System.Environment.NewLine;
            string start = "DTSTART:" + semStart + 
                hourToICalString(hodina.StartTime.Hour, hodina.StartTime.Minute) +
                System.Environment.NewLine;
            string end = "DTEND:" + semStart +
                hourToICalString((hodina.StartTime+hodina.Length).Hour, 
                (hodina.StartTime+hodina.Length).Minute) +
                System.Environment.NewLine;
            string repeat = "RRULE:FREQ=WEEKLY;UNTIL=" + semEnd + "T000000Z" +
                System.Environment.NewLine;
            string summarry = "SUMMARY:" + hodina.Name + System.Environment.NewLine;
            string loc = "LOCATION:" + hodina.Room + System.Environment.NewLine;
            string tail = "END:VEVENT" + System.Environment.NewLine;

            return head + stamp + start + end + repeat + summarry + loc + tail;
        }

        private DateTime closestDayFromDateTime(DateTime date, System.DayOfWeek target)
        {
            int delta = target - date.DayOfWeek;
            return date.AddDays(delta);
        }

        private DateTime semesterStart()
        {
            DateTime result = new DateTime(0);
            DateTime now = DateTime.Today;
            DateTime winterToSummer = new DateTime(now.Year, 2, 16);
            DateTime summerToWinter = new DateTime(now.Year,9, 20);
            DateTime nextYear = new DateTime(now.Year+1,1,1);
            DateTime thisYear = new DateTime(now.Year,1,1);
            if (now >= summerToWinter && now <= nextYear)
            {
                result = summerToWinter;
            }
            else if(now >= thisYear && now <= winterToSummer)
            {
                result = new DateTime(summerToWinter.Year - 1,summerToWinter.Month,summerToWinter.Day);
            }
            else if (now >= winterToSummer && now <= summerToWinter)
            {
                result = winterToSummer;
            }

            if (result == new DateTime(0))
            {
                throw new System.Exception("Problem with computing semester start " +
                    "when exporting do iCal format. Today: " + now.ToShortDateString());
            }
            return result;
        }

        private DateTime semesterEnd()
        {
            DateTime result = new DateTime(0);
            DateTime now = DateTime.Today;
            DateTime winterToSummer = new DateTime(now.Year, 2, 16);
            DateTime summerToWinter = new DateTime(now.Year, 9, 20);
            DateTime nextYear = new DateTime(now.Year + 1, 1, 1);
            DateTime thisYear = new DateTime(now.Year, 1, 1);
            if (now >= summerToWinter && now <= nextYear)
            {
                result = new DateTime(winterToSummer.Year + 1, winterToSummer.Month, winterToSummer.Day);
            }
            else if (now >= thisYear && now <= winterToSummer)
            {
                result = winterToSummer;
            }
            else if (now >= winterToSummer && now <= summerToWinter)
            {
                result = summerToWinter;
            }

            if (result == new DateTime(0))
            {
                throw new System.Exception("Problem with computing semester end " +
                    "when exporting do iCal format. Today: " + now.ToShortDateString());
            }
            return result;
        }

        private string dateTimeDateToICalString(DateTime dt)
        {
            return dt.Year.ToString() + dt.Month.ToString("D2") +
                    dt.Day.ToString("D2");
        }

        private string hourToICalString(int hour, int min)
        {
            return "T"+hour.ToString("D2") + min.ToString("D2") + "00";
        }

    }
}
