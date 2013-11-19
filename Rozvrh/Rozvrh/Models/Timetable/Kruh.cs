using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Kruh
    {
        public int id { get; private set; }
        public int cisloKruhu { get; private set; }
        public int zamereniId { get; private set; }
        public Kruh(int id, int cisloKruhu, int zamereniId)
        {
            this.id = id;
            this.cisloKruhu = cisloKruhu;
            this.zamereniId = zamereniId;
        }
    }//= part v xml
}