using System;
using System.Text.RegularExpressions;

namespace Rozvrh.Models
{
    public class FakeTimetableField : TimetableField
    {
        /// <summary>
        /// Fake timetable field is used during the generation of the users timetable. Contains only starting time and duration details.
        /// </summary>
        /// <param name="timeHours">Hours part of the starting time</param>
        /// <param name="timeMinutes">Minutes part of the starting time</param>
        /// <param name="duration">Length in whole hours</param>
        public FakeTimetableField(string timeHours, string timeMinutes, string duration)
        {
            if (isInt(timeHours))
                time_hours = timeHours;
            if (isInt(timeMinutes))
                time_minutes = timeMinutes;
            if (isInt(duration))
                this.duration = duration;
        }

        /// <summary>
        /// Checks if the text is only numbers => is integer
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool isInt(string text)
        {
            return Regex.IsMatch(text, @"^\d+$");
        }
    }
}
