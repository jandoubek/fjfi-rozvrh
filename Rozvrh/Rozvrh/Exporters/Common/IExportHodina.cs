using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rozrvh.Exporters.Common
{
    /// <summary>
    /// Interface used for import and export.
    /// </summary>
    public interface IExportHodina
    {
        /// <summary>
        /// Short name of the lecture - abbreviation.
        /// </summary>
        string Name {get; set;}

        /// <summary>
        /// Day of the week of the lecture.
        /// </summary>
        System.DayOfWeek Day { get; set;}

        /// <summary>
        /// Start time of the lecture.
        /// </summary>
        /// <remarks>Only Hour and Minute fields are used.</remarks>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Length of the lecture.
        /// </summary>
        /// <remarks>
        /// This is added to StartTime in ICalExport to determine the end time from which only Hour and Minute are used. 
        /// In SVG export it's continously mapped on pixels.
        /// </remarks>
        TimeSpan Length { get; set; }

        /// <summary>
        /// Last name of the lecturer.
        /// </summary>
        string Lecturer { get; set; }

        /// <summary>
        /// Abbreviation of the lecture's room.
        /// </summary>
        string Room { get; set; }
    }
}
