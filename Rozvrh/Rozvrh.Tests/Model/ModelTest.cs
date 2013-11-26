using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rozvrh.Models;
using Rozvrh.Models.Timetable;

namespace Rozvrh.Tests.Model
{
    /// <summary>
    /// Summary description for ModelTest
    /// </summary>
    [TestClass]
    public class ModelTest
    {
        private Mock<IXMLTimetable> timetable;
        private Models.Model model;

        private TestContext testContextInstance;

        public ModelTest()
        {
            initializeTimetable();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestInitialize]
        public void ModelTestInitialize()
        {
            model = new Models.Model(timetable.Object);
        }

        #region Initialization
        private void initializeTimetable()
        {
            timetable = new Mock<IXMLTimetable>();
            timetable.SetupGet(t => t.m_departments).Returns(departments);
            timetable.Setup(t => t.m_courses).Returns(courses);
            timetable.Setup(t => t.m_lectures).Returns(lectures);
            timetable.Setup(t => t.m_lessons).Returns(lessons);
            timetable.Setup(t => t.m_days).Returns(days);
            timetable.Setup(t => t.m_times).Returns(times);
            timetable.Setup(t => t.m_buildings).Returns(buildings);
            timetable.Setup(t => t.m_classrooms).Returns(classrooms);
            timetable.Setup(t => t.m_lecturers).Returns(lecturers);
            timetable.Setup(t => t.m_degreeyears).Returns(degreeyears);
            timetable.Setup(t => t.m_specializations).Returns(specializations);
            timetable.Setup(t => t.m_groups).Returns(groups);
            timetable.Setup(t => t.m_groupLessonBinder).Returns(groupLessonBinder);
        }

        private static List<Department> departments()
        {
            var departments = new List<Department>
            {
                new Department("1", "01", "K. matematiky", "KM", "1822536"),
                new Department("7", "15", "K. jaderné chemie", "KJCH", "4748754"),
                new Department("10", "18", "K. softwarového inženýrství v ekonomii", "KSE", "13674")
            };
            return departments;
        }

        private static List<Course> courses()
        {
            var courses = new List<Course>
            {
                new Course("55", "1", "Seminár matematické analýzy B1", "SMB1"),
                new Course("56", "1", "Softwarový seminár 1", "SOS1"),
                new Course("16", "1", "Lineární algebra plus", "LAP"),
                new Course("3", "1", "Aperiodické struktury", "APST"),

                new Course("260", "7", "Tuhé látky", "TL"),
                new Course("205", "7", "Praktikum z fyzikální chemie", "FYPRN"),
                new Course("261", "7", "Výpocty a fyzikální chemie 2", "VYC2"),

                new Course("130", "10", "Objektove orientované programování", "OOP"),
                new Course("206", "10", "Makroekonomie 1", "MAK1"),
                new Course("207", "10", "Matematická ekonomie 1", "EKO1"),
                new Course("434", "10", String.Empty, "PVS")
            };
            return courses;
        }

        private static List<Lecture> lectures()
        {
            var lectures = new List<Lecture>
            {
                new Lecture("28", "55", "0", String.Empty, "2", "0"),
                new Lecture("31", "56", "0", String.Empty, "2", "0"),
                new Lecture("6", "16", "0", String.Empty, "2", "1"),
                new Lecture("8", "16", "1", "cv", "2", "1"),
                new Lecture("72", "3", "0", String.Empty, "2", "0"),

                new Lecture("160", "260", "0", String.Empty, "2", "1"),
                new Lecture("563", "205", "0", String.Empty, "3", "0"),
                new Lecture("85", "205", "0", String.Empty, "5", "0"),
                new Lecture("161", "261", "0", String.Empty, "2", "0"),

                new Lecture("52", "130", "0", String.Empty, "2", "0"),
                new Lecture("93", "206", "0", String.Empty, "2", "0"),
                new Lecture("94", "206", "1", "cv", "2", "0"),
                new Lecture("95", "207", "1", "cv", "2", "0"),
                new Lecture("96", "207", "0", String.Empty, "2", "0"),
                new Lecture("512", "434", "0", String.Empty, "2", "0")
            };
            return lectures;
        }

        private static List<Lecturer> lecturers()
        {
            var lecturers = new List<Lecturer>
            {
                new Lecturer("15", "Krbalek", String.Empty, "1"),
                new Lecturer("13", "Culik", String.Empty, "1"),
                new Lecturer("159", "Balkova", String.Empty, "1"),
                new Lecturer("17", "Mikyska", String.Empty, "1"),
                new Lecturer("326", "Karasek", String.Empty, "1"),
                new Lecturer("10", "Masakova", String.Empty, "1"),
                new Lecturer("226", "Mucka", String.Empty, "7"),
                new Lecturer("341", "Vysoký", "Jan", "2"),
                new Lecturer("232", "Silber", String.Empty, "7"),
                new Lecturer("25", "Virius", String.Empty, "10"),
                new Lecturer("362", "Tran", String.Empty, "10"),
                new Lecturer("392", "Zouhar", String.Empty, "10"),
                new Lecturer("420", "Doubek", String.Empty, "10")
            };
            return lecturers;
        }

        private static List<Day> days()
        {
            var days = new List<Day>
            {
                new Day("1", "Monday", "0"),
                new Day("2", "Tuesday", "1"),
                new Day("3", "Wednesday", "2"),
                new Day("4", "Thursday", "3"),
                new Day("5", "Friday", "4"),
            };
            return days;
        }

        private static List<Time> times()
        {
            var times = new List<Time>
            {
                new Time("1", "7", "30", "0"),
                new Time("2", "8", "30", "1"),
                new Time("3", "9", "30", "2"),
                new Time("4", "10", "30", "3"),
                new Time("5", "11", "30", "4"),
                new Time("6", "12", "30", "5"),
                new Time("7", "13", "30", "6"),
                new Time("8", "14", "40", "7"),
                new Time("9", "15", "30", "8"),
                new Time("10", "16", "30", "9"),
                new Time("11", "17", "30", "10"),
                new Time("12", "18", "30", "11"),
                new Time("13", "19", "30", "12"),
            };
            return times;
        }

        private static List<Building> buildings()
        {
            var buildings = new List<Building>
            {
                new Building("1", "Brehová"),
                new Building("2", "Trojanova"),
                new Building("3", "Hlavova")
            };
            return buildings;
        }

        private static List<Classroom> classrooms()
        {
            var classrooms = new List<Classroom>
            {
                new Classroom("14", "T-201", "2"),
                new Classroom("23", "T-115", "2"),
                new Classroom("5", "B-115", "1"),
                new Classroom("12", "T-101", "2"),
                new Classroom("4", "B-115", "1"),
                new Classroom("21", "T-112", "2"),
                new Classroom("7", "B-314", "1"),
                new Classroom("35", "PrakFCH", "3"),
                new Classroom("22", "T-105", "2"),
                new Classroom("24", "T-014", "2"),

            };
            return classrooms;
        }

        private static List<Lesson> lessons()
        {
            var lessons = new List<Lesson>
            {
                new Lesson("65", "28", "15", "3", "3", "14", String.Empty),
                new Lesson("69", "31", "13", "4", "9", "23", String.Empty),
                new Lesson("147", "31", "13", "1", "7", "23", String.Empty),
                new Lesson("26", "6", "159", "2", "7", "14", String.Empty),
                new Lesson("44", "8", "159", "1", "9", "5", String.Empty),
                new Lesson("28", "8", "159", "3", "7", "12", String.Empty),
                new Lesson("39", "8", "17", "3", "3", "5", String.Empty),
                new Lesson("649", "8", "326", "2", "3", "4", String.Empty),
                new Lesson("799", "8", "326", "2", "5", "5", String.Empty),
                new Lesson("131", "72", "10", "2", "5", "21", String.Empty),

                new Lesson("281", "160", "226", String.Empty, String.Empty, "7", String.Empty),
                new Lesson("144", "563", "341", "1", "6", "35", String.Empty),
                new Lesson("655", "161", "232", String.Empty, String.Empty, "7", String.Empty),

                new Lesson("98", "52", "25", "2", "9", "22", String.Empty),
                new Lesson("159", "93", "362", "3", "5", "24", String.Empty),
                new Lesson("882", "94", "362", "3", "9", "24", String.Empty),
                new Lesson("162", "95", "392", "4", "11", "14", String.Empty),
                new Lesson("131", "96", "392", "4", "9", "14", String.Empty),
                new Lesson("919", "512", "420", "4", "5", "23", String.Empty)
            };
            return lessons;
        }

        private static List<DegreeYear> degreeyears()
        {
            var degreeyears = new List<DegreeYear>
            {
                new DegreeYear("1", "Bakalárské studium 1. ročník", "Bc. 1."),
                new DegreeYear("2", "Bakalárské studium 2. ročník", "Bc. 2."),
                new DegreeYear("3", "Bakalárské studium 3. ročník", "Bc. 3."),
                new DegreeYear("4", "Magisterské studium 1. ročník", "Ing. 1."),
                new DegreeYear("5", "Magisterské studium 2. ročník", "Ing. 2."),
                new DegreeYear("6", "Magisterské studium 3. ročník", "Ing. 3.")
            };
            return degreeyears;
        }

        private static List<Specialization> specializations()
        {
            var specializations = new List<Specialization>
            {
                new Specialization("39", "Matematické inženýrství", "MI", "4"),
                new Specialization("108", "Aplikované matematicko-stochastické metody", "AMSM", "1"),
                new Specialization("109", "Aplikované matematicko-stochastické metody", "AMSM", "2"),

                new Specialization("38", "Jaderne chemické inženýrství", "JCHI", "3"),
                new Specialization("40", "Jaderne chemické inženýrství", "JCHI", "4"),

                new Specialization("41", "Aplikace softwarového inženýrství", "ASI", "1"),
                new Specialization("42", "Aplikace softwarového inženýrství", "ASI", "2"),
                new Specialization("43  ", "Aplikace softwarového inženýrství", "ASI", "4"),
            };
            return specializations;
        }

        private static List<Group> groups()
        {
            var groups = new List<Group>
            {
                new Group("1", "1", "39"),
                new Group("2", "2", "39"),
                new Group("3", "1", "108"),
                new Group("4", "2", "108"),
                new Group("5", "3", "108"),
                new Group("6", "1", "38"),
                new Group("7", "1", "40"),
                new Group("9", "1", "109"),
                new Group("10", "1", "41"),
                new Group("11", "1", "42"),
                new Group("12", "1", "43")
            };
            return groups;
        }

        private static List<GroupLessonBinder> groupLessonBinder()
        {
            var groupLessonBinder = new List<GroupLessonBinder>
            {
                new GroupLessonBinder("1", "9", "65"), //The 1st group of the AMSM (2nd year) has SMB1
                new GroupLessonBinder("2", "2", "69"), //The 2nd group of MI has SOS1
                new GroupLessonBinder("3", "3", "26"), //The 1st group of the AMSM (1nd year) has LAP
                new GroupLessonBinder("3", "3", "44"), //The 1st group of the AMSM (1nd year) has LAPcv
                new GroupLessonBinder("3", "4", "26"), //The 2nd group of the AMSM (1nd year) has LAP
                new GroupLessonBinder("3", "4", "28"), //The 2nd group of the AMSM (1nd year) has LAPcv
                new GroupLessonBinder("3", "5", "26"), //The 3rd group of the AMSM (1nd year) has LAP
                new GroupLessonBinder("3", "5", "39"), //The 3rd group of the AMSM (1nd year) has LAPcv
            };
            return groupLessonBinder;
        }

        #endregion

        [TestMethod]
        public void Amsm2NdYearHasOnlySmb1Lesson()
        {
            const string amsm2NdYearGroupId = "9";
            model.FilterAll2TimetableFields(new List<string> { amsm2NdYearGroupId }, new List<string>(), new List<string>(),
                new List<string>(), new List<string>(), new List<string>());

            const int expectedCount = 1;
            const string expectedLessonAcronym = "SMB1";

            Assert.AreEqual(expectedCount, model.TimetableFields.Count);
            Assert.AreEqual(expectedLessonAcronym, model.TimetableFields[0].lecture_acr);
        }

        [TestMethod]
        public void Mi2NdYear2NdGroupHasOnlySos1Lesson()
        {
            const string mi2NdYear2NdGroupGroupId = "2";
            model.FilterAll2TimetableFields(new List<string> { mi2NdYear2NdGroupGroupId }, new List<string>(), new List<string>(),
                new List<string>(), new List<string>(), new List<string>());

            const int expectedCount = 1;
            const string expectedLessonAcronym = "SOS1";

            Assert.AreEqual(expectedCount, model.TimetableFields.Count);
            Assert.AreEqual(expectedLessonAcronym, model.TimetableFields[0].lecture_acr);
        }

        [TestMethod]
        public void TimetableFieldsOfAllDepartments()
        {
            const string kmDepartmentId = "1";
            const string kjchDepartmentId = "7";
            const string kseDepartmentId = "10";
            model.FilterAll2TimetableFields(new List<string>(), new List<string> { kmDepartmentId, kjchDepartmentId, kseDepartmentId }, new List<string>(),
                new List<string>(), new List<string>(), new List<string>());

            const int expectedCount = 17; //19 Lessons but 2 of them have't specified Day and Time property

            Assert.AreEqual(expectedCount, model.TimetableFields.Count);
        }

        [TestMethod]
        public void TimetableFieldsOfAmsm1StYear()
        {
            model.FilterAll2TimetableFields(new List<string> { "3", "4", "5" }, new List<string>(), new List<string>(),
                new List<string>(), new List<string>(), new List<string>());

            const int expectedCount = 4;

            Assert.AreEqual(expectedCount, model.TimetableFields.Count);
        }
    }
}
