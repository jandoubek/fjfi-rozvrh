using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class HodinyKruhu
    {
        public string id            { get; private set; }
        public string kruhId        { get; private set; }
        public string hodinaId      { get; private set; }
        
        public HodinyKruhu(string id, string kruhId, string hodinaId)
        {
            this.id = id;
            this.kruhId = kruhId;
            this.hodinaId = hodinaId;
        }
    }//- je vybráno z part elementů

}