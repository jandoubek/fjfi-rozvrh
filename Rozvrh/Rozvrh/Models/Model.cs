using System;
using System.Collections.Generic;
using Rozrvh;
using Rozvrh.Models.Timetable;
using System.Linq;
// Docasny import kvuli kompilaci, dokud se nerozhodne, co s tim
using Rozrvh.Exporters.Common;
using System.Xml.Linq;

namespace Rozvrh.Models
{
    /// <summary>
    /// Dummy data
    /// </summary>
    public class Model:IModel
    {
        // Temporary, see comment in constructor
        XMLTimetableLoader XmlLoader = new XMLTimetableLoader("C:\\Aktualni_databaze.xml");
                
      
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

            System.Diagnostics.Debug.WriteLine("Model constructor");
            // Richard: This is only a temporary solution. In next phase the XmlLoader is gonna be modofied to be a singleton and these linq queries placed there.
            // Model constructor will be designed to only set references for all lists.
            try
            {
                Departments = XmlLoader.m_departments;
                DegreeYears = XmlLoader.m_degreeyears;
                Buildings = XmlLoader.m_buildings;
                Days = XmlLoader.m_days;
                Times = XmlLoader.m_times;

                Specializations = new List<Zamereni>();
                Groups = new List<Kruh>();
                Lecturers = new List<Lecturer>();
                Classrooms = new List<Classroom>();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error when parsing timetable XML.");
                Console.WriteLine(e.StackTrace);
            }

            //Departments = new List<string> { "KJCH", "KJR" };
        }

        //Olda: Metoda, která podle nastavení filtrů vrátí seznam TimetableFieldů, by měla být v Controlleru.
        //      V tuhle chvíli tedy v souboru HomeController, v metodě Filter.
        //      Jedná se totiž o aplikační logiku, nikoliv datovou.

        /// <summary>
        /// Seznam vyfiltrovaných hodin, určení pro zobrazení v tabulce pod filtry
        /// </summary>
        public List<IExportHodina> FiltredLectures { get; private set; }

        public List<Department> Departments { get; private set; }
        public List<Zamereni> Specializations { get; private set; }
        public List<Kruh> Groups { get; private set; }
        public List<DegreeYear> DegreeYears { get; private set; }
        public List<Lecturer> Lecturers { get; private set; }
        public List<Building> Buildings { get; private set; }
        public List<Classroom> Classrooms { get; private set; }
        public List<Day> Days { get; private set; }
        public List<Time> Times { get; private set; }
    }
}