using System;
using System.Collections.Generic;
using Rozrvh;
using System.Linq;
// Docasny import kvuli kompilaci, dokud se nerozhodne, co s tim
using Rozrvh.Exporters.ICal;
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
                
                var DepartmentsAsString =
                    from d in XmlLoader.m_departments
                    orderby d.acronym
                select d.acronym;
                Departments = DepartmentsAsString.ToList();
                
                
                var YearsAsString =
                    from y in XmlLoader.m_degreeyears
                    orderby y.acronym
                    select y.acronym;
               DegreeYears = YearsAsString.ToList();

                var BuildingsAsString =
                    from b in XmlLoader.m_buildings
                    orderby b.name
                    select b.name;
                Buildings = BuildingsAsString.ToList();
                
                var DaysAsString = 
                    from d in XmlLoader.m_days
                    orderby d.name
                    select d.name;
                Days = DaysAsString.ToList();

                Specializations = new List<string>();

                Groups = new List<string>();
                Lecturers = new List<string>();
                Classrooms = new List<string>();
            
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when parsing timetable XML.");
                Console.WriteLine(e.StackTrace);
            }
            
            //Departments = new List<string> { "KJCH", "KJR" };
        }

        public void FilterDegreeYear2Specialization(String degreeYearFilterValue)
        {
            var degreeYearIds =
                from dy in XmlLoader.m_degreeyears
                where dy.acronym.Equals(degreeYearFilterValue)
                select dy.id;
            
            //ze zobrazených (visible) zaměření vybere ta, které přísluší vybranému ročníku
            var scopesAsString =
                from s in XmlLoader.m_zamerenis
                where degreeYearIds.ToList().Contains(s.degreeYearId)
                orderby s.acronym
                select s.acronym;

            Specializations = scopesAsString.ToList();
        }

        //filter 2 -  z vybrané položky zaměření vybere příslušné kruhy
        public void FilterSpecialization2Groups(String scopeFilterValue)
        {

            var scopeIds =
                from s in XmlLoader.m_zamerenis
                where s.acronym.Equals(scopeFilterValue)
                select s.id;

            //ze všech kruhů vybere ty, které přísluší vybranému zaměření
            var partsAsInt =
                from p in XmlLoader.m_kruhy
                where scopeIds.ToList().Contains(p.id)
                orderby p.cisloKruhu
                select p.cisloKruhu;
            Groups = partsAsInt.ToList().ConvertAll<string>(p => p.ToString());
        }

       

        //Olda: Metoda, která podle nastavení filtrů vrátí seznam TimetableFieldů, by měla být v Controlleru.
        //      V tuhle chvíli tedy v souboru HomeController, v metodě Filter.
        //      Jedná se totiž o aplikační logiku, nikoliv datovou.

        /// <summary>
        /// Seznam vyfiltrovaných hodin, určení pro zobrazení v tabulce pod filtry
        /// </summary>
        public List<IExportHodina> FiltredLectures { get; private set; }

        public List<string> Departments { get; private set; }

        public List<string> Specializations { get; private set; }

        public List<string> Groups { get; private set; }

        public List<string> DegreeYears { get; private set; }

        public List<string> Lecturers { get; private set; }

        public List<string> Buildings { get; private set; }

        public List<string> Classrooms { get; private set; }

        public List<string> Days { get; private set; }

        public List<string> Times { get; private set; }
    }
}