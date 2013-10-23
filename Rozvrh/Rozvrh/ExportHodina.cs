using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rozrvh
{
    interface ExportHodina
    {
        string getName();
        System.DayOfWeek getDay();
        DateTime getStartTime();
        TimeSpan getLength();
        string getLecturer();
        string getRoom();
    }
}
