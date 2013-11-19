using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class HodinyKruhu
    {
        public int id { get; private set; }
        public int kruhId { get; private set; }
        public int hodinaId { get; private set; }
        public HodinyKruhu(int id, int kruhId, int hodinaId)
        {
            this.id = id;
            this.kruhId = kruhId;
            this.hodinaId = hodinaId;
        }
    }//- je vybráno z part elementů

}