using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Classroom
    {
        public string id            { get; private set; }
        public string name          { get; private set; }
        public string buildingId    { get; private set; }
        
        public Classroom(string id, string name, string buildingId)
        {
            this.id = id;
            this.name = name;
            this.buildingId = buildingId;
        }
    }//= classroom v xml
}