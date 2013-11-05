using System;
using System.Collections.Generic;

namespace MvcApplication1.Models
{
    /// <summary>
    /// Dummy data
    /// </summary>
    public class Model : IModel
    {
        /// <summary>
        /// Returns all departmens
        /// </summary>
        public List<string> Departments
        {
            get { return new List<string> { "KJCH", "KJR" }; }
        }

        /// <summary>
        /// Returns all courses
        /// </summary>
        public List<string> Courses
        {
            get { return new List<string> { "JCH" }; }
        }

        /// <summary>
        /// Returns all groups
        /// </summary>
        public List<string> Groups
        {
            get { return new List<string> { "1" }; }
        }

        /// <summary>
        /// Returns all yeas of studies
        /// </summary>
        public List<string> Years
        {
            get { return new List<string> { "První BS", "Druhý BS", "Třetí BS", "První MS", "Druhý MS", "Třetí MS" }; }
        }

        /// <summary>
        /// Returns all lecturers
        /// </summary>
        public List<string> Lecturers
        {
            get { return new List<string> { "John", "Štamberg", "Vopálka", "Vrba" }; }
        }

        /// <summary>
        /// Returns all classrooms
        /// </summary>
        public List<string> Classrooms
        {
            get { return new List<string> { "B-314", "B-115" }; }
        }

        /// <summary>
        /// Returns all days of the workweek
        /// </summary>
        public List<DayOfWeek> Days
        {
            get
            {
                return new List<DayOfWeek>
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday
                };
            }
        }

        /// <summary>
        /// Returns all start times of lectures
        /// </summary>
        public List<TimeSpan> Times
        {
            get
            {
                return new List<TimeSpan>
                {
                    new TimeSpan(0, 8, 30, 0),
                    new TimeSpan(0, 10, 30, 0),
                    new TimeSpan(0, 13, 30, 0),
                    new TimeSpan(0, 14, 30, 0)
                };
            }
        }

        //TODO Autor doplní komentář!
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<string> GetParts(string group)
        {
            throw new NotImplementedException();
        }

        //TODO Autor doplní komentář!
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterValues"></param>
        /// <returns></returns>
        public List<TimetableField> GetTimeTable(List<string> filterValues)
        {
            throw new NotImplementedException();
        }
    }
}