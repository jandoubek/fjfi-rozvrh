using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class DegreeYear
    {
        public string id            { get; private set; }
        public string name          { get; private set; }
        public string acronym       { get; private set; }
        public DegreeYear(string id, string name, string acronym)
        {
            this.id = id;
            this.name = name;
            this.acronym = acronym;
        }
    }//= degree+year v xml
}