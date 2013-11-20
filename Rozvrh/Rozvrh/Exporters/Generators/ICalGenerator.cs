using Rozrvh;
using Rozrvh.Exporters.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozrvh.Exporters.Generators
{
    public class ICalGenerator
    {
        public ICalGenerator()
        {

        }

        public string generateICal(List<IExportHodina> hodiny,DateTime semesterStart,DateTime semesterEnd)
        {
            string header = "BEGIN:VCALENDAR" + System.Environment.NewLine + "VERSION:2.0" +
                System.Environment.NewLine + "PRODID:-//hacksw/handcal//NONSGML v1.0//EN" +
                System.Environment.NewLine;
            string body = "";

            foreach (IExportHodina hodina in hodiny)
            {
                body += generateEventFromExportHodina(hodina, semesterStart, semesterEnd);
            }

            string tail = "END:VCALENDAR";


            return header + body + tail;
        }


        private string generateEventFromExportHodina(IExportHodina hodina,DateTime semesterStart,DateTime semesterEnd)
        {
            string semStart = dateTimeDateToICalString(
                closestDayFromDateTime(semesterStart, hodina.Day));
            string semEnd = dateTimeDateToICalString(semesterEnd);

            string head = "BEGIN:VEVENT" + System.Environment.NewLine;
            string stamp = "DTSTAMP:" + dateTimeDateToICalString(DateTime.Today) +
                hourToICalString(DateTime.Now.Hour, DateTime.Now.Minute) + System.Environment.NewLine;
            string start = "DTSTART:" + semStart +
                hourToICalString(hodina.StartTime.Hour, hodina.StartTime.Minute) +
                System.Environment.NewLine;
            string end = "DTEND:" + semStart +
                hourToICalString((hodina.StartTime + hodina.Length).Hour,
                (hodina.StartTime + hodina.Length).Minute) +
                System.Environment.NewLine;
            string repeat = "RRULE:FREQ=WEEKLY;UNTIL=" + semEnd + "T000000Z" +
                System.Environment.NewLine;
            string summarry = "SUMMARY:" + hodina.Name + System.Environment.NewLine;
            string loc = "LOCATION:" + hodina.Room + System.Environment.NewLine;
            string tail = "END:VEVENT" + System.Environment.NewLine;

            return head + stamp + start + end + repeat + summarry + loc + tail;
        }

        public DateTime closestDayFromDateTime(DateTime date, System.DayOfWeek target)
        {
            int delta = target - date.DayOfWeek;
            return date.AddDays(delta);
        }

         public string dateTimeDateToICalString(DateTime dt)
        {
            return dt.Year.ToString() + dt.Month.ToString("D2") +
                    dt.Day.ToString("D2");
        }

        public string hourToICalString(int hour, int min)
        {
            return "T" + hour.ToString("D2") + min.ToString("D2") + "00";
        }
    }
    
}