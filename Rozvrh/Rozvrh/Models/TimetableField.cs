using Rozvrh.Models.Timetable;

namespace Rozvrh.Models
{
    /// <summary>
    /// Class representing all information needed to render one lesson (field) on timetable
    /// </summary>
    public class TimetableField
    {
        /// <summary>
        /// Department name.
        /// </summary>
        public string department        { get; set; }
        
        /// <summary>
        /// Department acronym. Just letters, no numbers.
        /// </summary>
        public string department_acr    { get; set; }
        
        /// <summary>
        /// Lecture name.
        /// </summary>
        public string lecture_name      { get; set; }
        
        /// <summary>
        /// Lecture acronym. 'cv' at the end of string when practise.
        /// </summary>
        public string lecture_acr       { get; set; }
        
        /// <summary>
        /// Color of the department. Integer number in decimal base representing hex color. e.g. 15724527 is EFEFEF grey
        /// </summary>
        public string color             { get; set; }
        
        /// <summary>
        /// Week regularity of lessons. "Ano" for regular every week lesson and "No" for irregular lesson.
        /// </summary>
        public string period            { get; set; }
        
        /// <summary>
        /// Lecture duration in hours. Always an integer.
        /// </summary>
        public string duration          { get; set; }

        /// <summary>
        /// address to the www pages of the previous year school pool 
        /// </summary>
        public string course_href       { get; set; }

        /// <summary>
        /// Miscellaneous info about lecture.
        /// </summary>
        public string tag               { get; set; }
        
        /// <summary>
        /// Is the lecture practice? '1' for practice lecture, '0' for lecture/reading
        /// </summary>
        public string practice          { get; set; }
        
        /// <summary>
        /// Conposed name of a lecturer. Format 'Name S.' (surname).
        /// </summary>
        public string lecturer          { get; set; }
        
        /// <summary>
        /// Day name. 
        /// </summary>
        public string day               { get; set; } 
        
        /// <summary>
        /// Day order. From 0 to 4 (pondělí - pátek)
        /// </summary>
        public string day_order         { get; set; }
        
        /// <summary>
        /// Only hours of lesson time.
        /// </summary>
        public string time_hours        { get; set; }
        
        /// <summary>
        /// Only minutes of lesson time.
        /// </summary>
        public string time_minutes      { get; set; }
        
        /// <summary>
        /// Compound time. Format '[h]h:mm'.
        /// </summary>
        public string time              { get; set; }
        
        /// <summary>
        /// Order of a time in timetable. From 0 to 12 (7:30 - 19:30)
        /// </summary>
        public string time_order        { get; set; }
        
        /// <summary>
        /// Building name.
        /// </summary>
        public string building          { get; set; }
        
        /// <summary>
        /// Classroom name.
        /// </summary>
        public string classroom         { get; set; }

        /// <summary>
        /// Returns unique hash computed from lecture_acr, time_hours, time_minutes, classroom and lecturer
        /// </summary>
        public int UniqueID          { get; set; }

        /// <summary>
        /// Class constructor. Fills '-' in the classroom and building properties for 'Jazyky' lessons. 
        /// </summary>
        /// <param name="dep">Department class.</param>
        /// <param name="c">Course class.</param>
        /// <param name="lec">Lecture class.</param>
        /// <param name="ler">Lecturer class input.</param>
        /// <param name="d">Day class input.</param>
        /// <param name="t">Time class input.</param>
        /// <param name="b">Building class input.</param>
        /// <param name="cr">Classroom class input.</param>
        public TimetableField(Department dep, Course c, Lecture lec, Lecturer ler, Day d, Time t, Building b, Classroom cr, TimetableInfo ti)
        {
            department = dep.name;
            department_acr = dep.acronym;
            lecture_name = c.name;
            lecture_acr = c.acronym;
            //if (lec.practice.Equals("1") && lecture_acr != "JAZ")
            lecture_acr += lec.tag;
            color = dep.color;
            if (lec.period == "0")
                period = "Ano";
            else
                period = "Ne";
            duration = lec.duration;   //will be replaced to the config setting
            string prefix = ti.PrefixPoolLink;
            string sufix = ti.SufixPoolLink;
            course_href = prefix + dep.code + c.acronym.Replace(@"/", "") + sufix;
            tag = lec.tag; 
            practice = lec.practice;
            lecturer = ler.name;
            day = d.name;
            day_order = d.daysOrder;
            time_hours = t.hours;
            time_minutes = t.minutes;
            time = t.acronym;
            time_order = t.timesOrder;
            building = b.name;
            classroom = cr.name;
            RecalculateUniqueID();
        }

        /// <summary>
        /// Parameterless contructor for XML serialization.
        /// </summary>
        public TimetableField()
        {
 
        }

        /// <summary>
        /// Recalculates the UniqueID - need to be called after change in object data
        /// </summary>
        public void RecalculateUniqueID()
        {
            UniqueID = (lecture_acr + time + day + classroom + lecturer + department_acr + duration + period).GetHashCode();
        }
    }

}