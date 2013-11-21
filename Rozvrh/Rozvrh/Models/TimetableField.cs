using Rozvrh.Models.Timetable;

namespace Rozvrh.Models
{
    public class TimetableField
    {
        public string department        { get; private set; }//název katedry
        public string department_acr    { get; private set; }//zkratka katedry
        public string lecture_name      { get; private set; }//název předmětu
        public string lecture_acr       { get; private set; }//zkratka předmětu
        public string color             { get; private set; }//barva políčka
        public string period            { get; private set; }//perioda opakování hodiny - znamená spíš pravidelně (0) x nepravidelně (1)
        public string duration          { get; private set; }//délka lekce v hodinách, běžné přednášky mají duration 2
        public string tag               { get; private set; }//tag
        public string practice          { get; private set; }//cvičení - hodnoty 0 nebo 1
        public string lecturer          { get; private set; }//složené jméno učitele
        public string day               { get; private set; }//název dne v týdnu   
        public string day_order         { get; private set; }//pořadí dne v týdnu od 0 (pondělí) do 4 (pátek)
        public string time_hours        { get; private set; }//čas hodin
        public string time_minutes      { get; private set; }//čas minut
        public string time_acronym      { get; private set; }//složenina h(h):mm
        public string time_order        { get; private set; }//čas pořadí časového slotu od 0 do 12
        public string building          { get; private set; }//budova název
        public string classroom         { get; private set; }//místnost název

        public TimetableField(Department dep, Course c, Lecture lec, Lecturer ler, Day d, Time t, Building b, Classroom cr)
        {
            department = dep.name;
            department_acr = dep.acronym;
            lecture_name = c.name;
            lecture_acr = c.acronym;
            if (lec.practice.Equals("1"))
                lecture_acr += "cv";
            color = dep.color;
            period = lec.period;
            duration = lec.duration;
            tag = lec.tag;
            practice = lec.practice;
            lecturer = ler.name;
            if (ler.forname.Length != 0)
                lecturer = ler.forname[0] + ". " + lecturer;
            day = d.name;
            day_order = d.daysOrder;
            time_hours = t.hours;
            time_minutes = t.minutes;
            time_acronym = t.acronym;
            time_order = t.timesOrder;
            building = b.name;
            classroom = cr.name;
            if (lecture_name == "Jazyky")
            {
                lecture_acr = "JAZ";
                classroom = "-";
                building = "-";
            }
        }
    }

}