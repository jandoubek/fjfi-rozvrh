using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Models.Timetable
{
    public class Group
    {
        public string id                    { get; private set; }
        public string groupNo                { get; private set; }
        public string specializationId      { get; private set; }

        public Group(string id, string groupNo, string specializationId)
        {
            this.id = id;
            this.groupNo = groupNo;
            this.specializationId = specializationId;
        }
    }//= part v xml
}