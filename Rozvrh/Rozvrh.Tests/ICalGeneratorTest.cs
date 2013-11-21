using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rozrvh;
using System.Collections.Generic;
using Rozrvh.Exporters.Common;
using Rozrvh.Exporters.Generators;


namespace Rozvrh.Tests
{
    [TestClass]
    public class ICalGeneratorTest
    {
        [TestMethod]
        public void TestGenerate()
        {

            ExportLecture a = new ExportLecture("PredmetA", DayOfWeek.Monday, new DateTime(1, 1, 1, 7, 30, 0), new TimeSpan(2, 0, 0),
                "OsobaA", "MistnostA","#99999");
            ExportLecture b = new ExportLecture("PredmetB", DayOfWeek.Tuesday, new DateTime(1, 1, 1, 17, 0, 0), new TimeSpan(1, 0, 0),
                "OsobaB", "MistnostB", "#99999");

            var hodiny = new List<ExportLecture>();
            hodiny.Add(a);
            hodiny.Add(b);

            DateTime semesterStart = new DateTime(2013, 9, 19);
            DateTime semesterEnd = new DateTime(2014, 2, 19);

            ICalGenerator gen = new ICalGenerator();

            string actualOutput = gen.generateICal(hodiny,semesterStart,semesterEnd);
            string stamp = gen.dateTimeDateToICalString(DateTime.Today) +
                gen.hourToICalString(DateTime.Now.Hour, DateTime.Now.Minute);
            string semStartA = gen.dateTimeDateToICalString(gen.closestDayFromDateTime(semesterStart,a.Day));
            string semStartB = gen.dateTimeDateToICalString(gen.closestDayFromDateTime(semesterStart,b.Day));
            string semEnd = gen.dateTimeDateToICalString(semesterEnd);

            string goodOutput = "BEGIN:VCALENDAR"+System.Environment.NewLine+
                    "VERSION:2.0"+System.Environment.NewLine+
                    "PRODID:-//hacksw/handcal//NONSGML v1.0//EN"+System.Environment.NewLine+
                    "BEGIN:VEVENT"+System.Environment.NewLine+
                    "DTSTAMP:" + stamp + System.Environment.NewLine +
                    "DTSTART:" + semStartA + "T073000"+System.Environment.NewLine+
                    "DTEND:" + semStartA + "T093000"+System.Environment.NewLine+
                    "RRULE:FREQ=WEEKLY;UNTIL=" + semEnd + "T000000Z"+System.Environment.NewLine+
                    "SUMMARY:PredmetA"+System.Environment.NewLine+
                    "LOCATION:MistnostA"+System.Environment.NewLine+
                    "END:VEVENT"+System.Environment.NewLine+
                    "BEGIN:VEVENT"+System.Environment.NewLine+
                    "DTSTAMP:" + stamp + System.Environment.NewLine +
                    "DTSTART:" + semStartB + "T170000"+ System.Environment.NewLine+
                    "DTEND:" + semStartB + "T180000" + System.Environment.NewLine +
                    "RRULE:FREQ=WEEKLY;UNTIL=" + semEnd + "T000000Z"+System.Environment.NewLine+
                    "SUMMARY:PredmetB"+System.Environment.NewLine+
                    "LOCATION:MistnostB"+System.Environment.NewLine+
                    "END:VEVENT"+System.Environment.NewLine+
                    "END:VCALENDAR";
            Assert.AreEqual<string>(goodOutput, actualOutput);

        }

        [TestMethod]
        public void TestEmpty()
        {
            var hodiny = new List<ExportLecture>();
            DateTime semesterStart = new DateTime();
            DateTime semesterEnd = new DateTime();
            ICalGenerator gen = new ICalGenerator();
            string actualOutput = gen.generateICal(hodiny, semesterStart, semesterEnd);

            string goodOutput = @"BEGIN:VCALENDAR"+System.Environment.NewLine+
                                "VERSION:2.0"+System.Environment.NewLine+
                                "PRODID:-//hacksw/handcal//NONSGML v1.0//EN"+System.Environment.NewLine+
                                "END:VCALENDAR";
            Assert.AreEqual<string>(goodOutput, actualOutput);
        }
    }
}
