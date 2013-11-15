using System;
using System.Collections.Generic;
using Rozrvh;
// Docasny import kvuli kompilaci, dokud se nerozhodne, co s tim
using Rozrvh.Exporters.ICal;

namespace Rozvrh.Models
{
    /// <summary>
    /// Dummy data
    /// </summary>
    public class Model : IModel
    {
        /// <summary>
        /// Konstruktor třídy
        /// </summary>
        public Model()
        {
// FIX ME !!! - autor: Richard
// Micha se export a model, pokud od sbe neco potrebuji, musi pres controller. 
// Pokud exporter potrebuje cokoli od modelu, controller si vse vytahne z modelu pred(!) zavooanim exporteru a rovnou mu posle vse potrebne.
// Proc je vubec v modelu IExportHodina? Pokud vidim dobre, pouziva tato trida jako nas datovy "typ" pro vypis vfiltrovanych hodin ve view.
// V tom pripade, je to nas zakladni datovy typ a mel by byt soucasti modelu a jmenovat se jinak, napr. FilteredLecture
// Nejsem si jisty, jestli je v tomto pripade nutny interface - pro zakladni datovy typ, ktery nema zadne funkce (vyjma get a set) - nebyla by vhodnejsi struktura?
// Navic mi prijde, ze ExportHodina se funkcne kryje s TimeTableField a slo by je sjednotit a zbavit se tak jedne datove tridy.
// Navrhuji refaktoring ve smyslu vyse uvedeneho, pokud ma nekdo jiny nazor - sem (resp. na FB nebo na SCRUM) s nim.
            FiltredLectures = new List<IExportHodina>();
        }

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

        //Olda: Metoda, která podle nastavení filtrů vrátí seznam TimetableFieldů, by měla být v Controlleru.
        //      V tuhle chvíli tedy v souboru HomeController, v metodě Filter.
        //      Jedná se totiž o aplikační logiku, nikoliv datovou.

        /// <summary>
        /// Seznam vyfiltrovaných hodin, určení pro zobrazení v tabulce pod filtry
        /// </summary>
        public List<IExportHodina> FiltredLectures { get; set; }
    }
}