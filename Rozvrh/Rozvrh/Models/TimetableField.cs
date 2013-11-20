using System;
using Rozvrh.Models.Timetable;

namespace Rozvrh.Models
{
    public class TimetableField
    {
        public string department { get; private set; }//název katedry
        public string department_acr { get; private set; }//zkratka katedry
        public string predmet { get; private set; }  //název předmětu
        public string predmet_acr { get; private set; } //zkratka predmetu
        public ulong color { get; private set; }     //barva políčka
        public int period { get; private set; }      //perioda opakování hodiny
        public int duration { get; private set; }    //délka lekce v hodinách
        public string tag { get; private set; }      //tag
        public bool practice { get; private set; }   //cvičení?
        public string lecturer { get; private set; } //složené jméno učitele
        public string day { get; private set; }      //název dne v týdnu   
        public int day_order { get; private set; }   //pořadí dne v týdnu
        public int time_hours { get; private set; }  //čas hodin
        public int time_minutes { get; private set; }//čas minut
        public int time_order { get; private set; }  //čas pořadí časového slotu
        public string building { get; private set; } //budova název
        public string classroom { get; private set; }//místnost název

        public TimetableField(Department dep, Course c, Lecture lec, Lecturer ler, Day d, Time t, Building b, Classroom cr)
        {
            department = dep.name;
            department_acr = dep.acronym;
            predmet = c.name;
            predmet_acr = c.acronym;
            if (lec.practice == 1)
                predmet_acr += "cv";
            color = dep.color;
            period = lec.period;
            duration = lec.duration;
            tag = lec.tag;
            practice = lec.practice > 0;
            lecturer = ler.name;
            if (ler.forname.Length != 0)
                lecturer = ler.forname[0] + ". " + lecturer;
            day = d.name;
            day_order = d.daysOrder;
            time_hours = t.hours;
            time_minutes = t.minutes;
            time_order = t.timesOrder;
            building = b.name;
            classroom = cr.name;
            if (predmet == "Jazyky")
            {
                predmet_acr = "JAZ";
                classroom = "-";
                building = "-";
            }
        }
    }

}