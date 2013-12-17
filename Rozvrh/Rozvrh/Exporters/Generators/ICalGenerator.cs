using Rozvrh;
using Rozvrh.Exporters.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Exporters.Generators
{
    /// <summary>
    ///  Class generating string in ICal format from list of lectures.
    /// </summary>
    public class ICalGenerator
    {
        public ICalGenerator()
        {

        }

        /// <summary> 
        /// Generates string in iCal format.
        /// </summary> 
        /// <returns> String following ICal format. </returns>
        /// <param name="lectures">List of lectures with IExportHodina interface to export.</param>
        /// <param name="semesterStart">Beginning of the semester. Only Year, Month and Day are used. </param>
        /// <param name="semesterEnd">End of the semester. Only Year, Month and Day are used. </param>
        public string generateICal(List<ExportLecture> lectures, DateTime semesterStart, DateTime semesterEnd)
        {
            string header = "BEGIN:VCALENDAR" + System.Environment.NewLine + "VERSION:2.0" +
                System.Environment.NewLine + "PRODID:-//hacksw/handcal//NONSGML v1.0//EN" +
                System.Environment.NewLine;
            string body = "";

            foreach (ExportLecture lecture in lectures)
            {
                body += generateEventFromExportHodina(lecture, semesterStart, semesterEnd);
            }

            string tail = "END:VCALENDAR";


            return header + body + tail;
        }


        private string generateEventFromExportHodina(ExportLecture lecture, DateTime semesterStart, DateTime semesterEnd)
        {
            string semStart = dateTimeDateToICalString(
                closestDayFromDateTime(semesterStart, lecture.Day));
            string semEnd = dateTimeDateToICalString(semesterEnd);

            string head = "BEGIN:VEVENT" + System.Environment.NewLine;
            string stamp = "DTSTAMP:" + dateTimeDateToICalString(DateTime.Today) +
                hourToICalString(DateTime.Now.Hour, DateTime.Now.Minute) + System.Environment.NewLine;
            string start = "DTSTART:" + semStart +
                hourToICalString(lecture.StartTime.Hour, lecture.StartTime.Minute) +
                System.Environment.NewLine;
            string end = "DTEND:" + semStart +
                hourToICalString((lecture.StartTime + lecture.Length).Hour,
                (lecture.StartTime + lecture.Length).Minute) +
                System.Environment.NewLine;
            string repeat = "RRULE:FREQ=WEEKLY;UNTIL=" + semEnd + "T000000Z" +
                System.Environment.NewLine;
            string summarry = "SUMMARY:" + lecture.Name + System.Environment.NewLine;
            string loc = "LOCATION:" + lecture.Room + System.Environment.NewLine;
            string tail = "END:VEVENT" + System.Environment.NewLine;

            return head + stamp + start + end + repeat + summarry + loc + tail;
        }

        /// <summary> 
        /// Returns closest date to input date which is target day. 
        /// </summary> 
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