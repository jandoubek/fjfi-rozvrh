using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rozrvh
{
    public interface IExportHodina
    {
        string Name {get; set;}
        System.DayOfWeek Day { get; set;}
        DateTime StartTime { get; set; }
        TimeSpan Length { get; set; }
        string Lecturer { get; set; }
        string Room { get; set; }
    }
}
