using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Classroom
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public int buildingId { get; private set; }
        public Classroom(int id, string name, int buildingId)
        {
            this.id = id;
            this.name = name;
            this.buildingId = buildingId;
        }
    }//= classroom v xml
}