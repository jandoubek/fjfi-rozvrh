using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rozrvh;
using System.Collections.Generic;
using MvcApplication1.Controllers;

namespace Rozvrh.Tests
{
    [TestClass]
    public class ICalGeneratorTest
    {
        [TestMethod]
        public void TestGenerate()
        {

            var mockA = new Moq.Mock<IExportHodina>();
            var mockB = new Moq.Mock<IExportHodina>();

            mockA.Setup(m => m.Day).Returns(DayOfWeek.Monday);
            mockA.Setup(m => m.Lecturer).Returns("OsobaA");
            mockA.Setup(m => m.Length).Returns(new TimeSpan(2, 0, 0));
            mockA.Setup(m => m.Name).Returns("PredmetA");
            mockA.Setup(m => m.Room).Returns("MistnostA");
            mockA.Setup(m => m.StartTime).Returns(new DateTime(1, 1, 1, 7, 30, 0));

            mockB.Setup(m => m.Day).Returns(DayOfWeek.Tuesday);
            mockB.Setup(m => m.Lecturer).Returns("OsobaB");
            mockB.Setup(m => m.Length).Returns(new TimeSpan(1, 0, 0));
            mockB.Setup(m => m.Name).Returns("PredmetB");
            mockB.Setup(m => m.Room).Returns("MistnostB");
            mockB.Setup(m => m.StartTime).Returns(new DateTime(1, 1, 1, 17, 0, 0));

            var hodiny = new List<IExportHodina>();
            hodiny.Add(mockA.Object);
            hodiny.Add(mockB.Object);

            DateTime semesterStart = new DateTime(2013, 9, 19);
            DateTime semesterEnd = new DateTime(2014, 2, 19);

            ICalGenerator gen = new ICalGenerator();

            string actualOutput = gen.generateICal(hodiny,semesterStart,semesterEnd);
            string stamp = gen.dateTimeDateToICalString(DateTime.Today) +
                gen.hourToICalString(DateTime.Now.Hour, DateTime.Now.Minute);
            string semStartA = gen.dateTimeDateToICalString(gen.closestDayFromDateTime(semesterStart,mockA.Object.Day));
            string semStartB = gen.dateTimeDateToICalString(gen.closestDayFromDateTime(semesterStart,mockB.Object.Day));
            string semEnd = gen.dateTimeDateToICalString(semesterEnd);

            string goodOutput = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//hacksw/handcal//NONSGML v1.0//EN
BEGIN:VEVENT
DTSTAMP:" + stamp + @"
DTSTART:" + semStartA + @"T073000
DTEND:" + semStartA + @"T093000
RRULE:FREQ=WEEKLY;UNTIL=" + semEnd + @"T000000Z
SUMMARY:PredmetA
LOCATION:MistnostA
END:VEVENT
BEGIN:VEVENT
DTSTAMP:" + stamp + @"
DTSTART:" + semStartB + @"T170000
DTEND:" + semStartB + @"T180000
RRULE:FREQ=WEEKLY;UNTIL=" + semEnd + @"T000000Z
SUMMARY:PredmetB
LOCATION:MistnostB
END:VEVENT
END:VCALENDAR";
            Assert.AreEqual<string>(goodOutput, actualOutput);

        }

        [TestMethod]
        public void TestEmpty()
        {
            var hodiny = new List<IExportHodina>();
            DateTime semesterStart = new DateTime();
            DateTime semesterEnd = new DateTime();
            ICalGenerator gen = new ICalGenerator();
            string actualOutput = gen.generateICal(hodiny, semesterStart, semesterEnd);

            string goodOutput = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//hacksw/handcal//NONSGML v1.0//EN
END:VCALENDAR";
            Assert.AreEqual<string>(goodOutput, actualOutput);
        }
    }
}
