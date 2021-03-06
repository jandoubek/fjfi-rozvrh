﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Rozvrh.Models
{
    /// <summary>
    /// 'Interface' class of the config. Only the data part.
    /// </summary>
    [Serializable]
    public class IConfig
    {
        /// <summary>
        ///  Path to the file with message to be shown on the empty filtering result list.
        /// </summary>
        [XmlElement("WelcomeMessageFilePath")]
        public string WelcomeMessageFilePath { get; set; }

        /// <summary> 
        /// Path to the folder with timetable files (.xml and .info) 
        /// </summary>
        [XmlElement("ArchivePath")]
        public String ArchivePath { get; set; }

        /// <summary>
        /// Welcome message to be shown at startup page.
        /// </summary>
        [XmlIgnore]
        public String WelcomeMessage { get; set; }
    }
}
