using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Kruh
    {
        public string id            { get; private set; }
        public string cisloKruhu    { get; private set; }
        public string zamereniId    { get; private set; }
        
        public Kruh(string id, string cisloKruhu, string zamereniId)
        {
            this.id = id;
            this.cisloKruhu = cisloKruhu;
            this.zamereniId = zamereniId;
        }
    }//= part v xml
}