using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Rozvrh.Models.Timetable
{
    public class TimetableInfo
    {
        #region Log
        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

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
        [XmlIgnore]
        public String TimetableXMLFilePath { get; set; }

        /// <summary> 
        /// Label of the current timetable xml to be shown in list of timetables. 
        /// </summary>
        [XmlElement("TimetableLabel")]
        public String TimetableLabel { get; set; }

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

        /// <summary>
        /// Id of the timetable in list
        /// </summary>
        [XmlIgnore]
        public string Id { get; set; }

        /// <summary>
        /// Label of the timetable coumpounded from Label and date of creation.
        /// </summary>
        [XmlIgnore]
        public string CompoundLabel
        {
            get
            {
                return TimetableLabel + " (" + Created.ToShortDateString() + ")";
            }
        }

        public TimetableInfo(string xmlTimetableFilePath, string id)
        {
            TimetableInfo ti = LoadFromFile(xmlTimetableFilePath);
            SetValues(ti);
            Id = id;
            TimetableXMLFilePath = xmlTimetableFilePath;
        }

        public TimetableInfo()
        { }

        private void SetValues(TimetableInfo ti)
        {
            Id = ti.Id;
            SufixPoolLink = ti.SufixPoolLink;
            PrefixPoolLink = ti.PrefixPoolLink;
            TimetableLabel = ti.TimetableLabel;
            TimetableXMLFilePath = ti.TimetableXMLFilePath;
            SemesterEnd = ti.SemesterEnd;
            SemesterStart = ti.SemesterStart;
            Created = ti.Created;
            LinkToAdditionalInformation = ti.LinkToAdditionalInformation;
        }

        private static TimetableInfo LoadFromFile(string timetableFilePath)
        {
            log.Debug("Method entry.");
            string infoFilePath =  Path.GetDirectoryName(timetableFilePath) + @"\" + Path.GetFileNameWithoutExtension(timetableFilePath) + "-info.xml";
            try
            {
                log.Debug("Trying to load timetable info from file: '" + infoFilePath + "'");
                XmlSerializer serializer = new XmlSerializer(typeof(TimetableInfo));

                if (String.CompareOrdinal("~", 0, infoFilePath, 0, 1) == 0)
                {
                    log.Info("Loading XML file from a server map path: '" + infoFilePath + "'.");
                    infoFilePath = System.Web.HttpContext.Current.Server.MapPath(infoFilePath);
                }
                else
                {
                    log.Info("Loading XML file from an absolute path (like http): '" + infoFilePath + "'.");
                }
                
                TextReader reader = new StreamReader(infoFilePath);
                if (reader == null)
                {
                    log.Error("Unable to load timetable info file (wrong filepath).");
                }
                TimetableInfo loadedTimetableInfo = (TimetableInfo)serializer.Deserialize(reader);
                reader.Close();


                log.Debug("Timetable info successfully loaded from file: '" + infoFilePath + "'");
                log.Debug("Method exit.");
                return loadedTimetableInfo;
            }
            catch (Exception e)
            {
                log.Error("Configuration does not loaded from file: '" + infoFilePath + "'. Exception: " + e.Message );
                log.Debug("Method exit");
                return null;
            }
        }
    }
}