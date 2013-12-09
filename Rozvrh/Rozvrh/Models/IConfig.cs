using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Rozvrh.Models
{
    [Serializable]
    public class IConfig
    {
        /// <summary> 
        /// Hyperlink to webpage with additional information about timetable
        /// </summary>
        /// <remarks>Used in SVG export.</remarks>
        [XmlElement("LinkToInfo")]
        public string LinkToAdditionalInformation { get; set; }

        /// <summary> 
        /// Date the timetable was created
        /// </summary>
        /// <remarks>Used in SVG export.</remarks>
        [XmlElement("Created")]
        public DateTime Created { get; set; }

        /// <summary> 
        /// When does the semester starts.
        /// </summary>
        [XmlElement("SemesterStart")]
        public DateTime SemesterStart { get; set; }


        /// <summary> 
        /// When does the semester start.
        /// </summary>
        [XmlElement("SemesterEnd")]
        public DateTime SemesterEnd { get; set; }

        /// <summary> 
        /// File path to XML with timetables. 
        /// </summary>
        [XmlElement("XMLTimeTablePath")]
        public String XMLTimetableFilePath { get; set; }

        /// <summary> 
        /// Prefix of the link to course popularity pool.
        /// </summary>
        [XmlElement("PrefixPoolLink")]
        public string PrefixPoolLink { get; set; }

        /// <summary> 
        /// Sufix of the link to course popularity pool.
        /// </summary>
        [XmlElement("SufixPoolLink")]
        public string SufixPoolLink { get; set; }
    }
}
