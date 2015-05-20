﻿using System;
using System.Collections.Generic;
using System.Linq;
using Rozvrh.Models.Timetable;
using Rozvrh.Utils;
using System.Xml.Linq;
using System.IO;

namespace Rozvrh.Models
{
    /// <summary>
    /// Class providing data load from xml file and storing of the base data after aplication start. This method is implemented as singleton.
    /// </summary>
    public class XMLTimetable : OneXMLTimetable
    {

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<XMLTimetable> _instance = new Lazy<XMLTimetable>(() => new XMLTimetable());

        #region Log
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public static XMLTimetable Instance
        {
            get
            {
                return _instance.Value;
            }
            
        }

        private XMLTimetable()
        {
            LoadDatabase();
        }

        public bool LoadDatabase()
        {
            bool result = false;

            TimetableArchive = new List<OneXMLTimetable>();

            //load archive timetables

            List<string> paths = xmlFilePaths();
            foreach (string path in paths)
            {
                if(!path.EndsWith("-info.xml"))
                {
                    OneXMLTimetable newTT = new OneXMLTimetable();
                    if (newTT.Load(path))
                    {
                        newTT.m_timetableInfo = new TimetableInfo(path, TimetableArchive.Count.ToString());
                        TimetableArchive.Add(newTT);
                        result = true;
                    }
                }
            }

            //get most recent timetable accordint to creation date
            OneXMLTimetable mostRecent = TimetableArchive.OrderByDescending(t => t.m_timetableInfo.SemesterStart).OrderByDescending(t => t.m_timetableInfo.Created).First();
            this.Copy(mostRecent);

            return result;
        }

        public static List<OneXMLTimetable> TimetableArchive;

        /// <summary>
        /// Get paths of all xml files in archive directory.
        /// </summary>
        /// <returns>Full file paths</returns>
        private List<string> xmlFilePaths()
        {
            log.Debug("Method entry.");
            string directoryPath = Config.Instance.ArchivePath;
            List<string> files = new List<string>();
            try
            {
                log.Debug("Trying to load index file with list of XMLs from directory " + directoryPath);
                string indexFileContent = Files.LoadFileContentFromPath(directoryPath + "/index.txt");
                files = indexFileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                for(int i = 0; i < files.Count; i++)
                {
                    files[i] = directoryPath + "/" + files[i];
                }
                return files;
            }
            catch
            {
                log.Error("Unable to get files from directory " + directoryPath);
                return files;
            }
            finally
            {
                log.Debug("Method exit.");
            }
        }

        /// <summary>
        /// Returns the timetable from TimetableArchive with specified id.
        /// </summary>
        /// <param name="id">The id of the required timetable.</param>
        /// <returns>Reference to required timetable.</returns>
        public static OneXMLTimetable getTimetableOfId(string id)
        {
            var l = TimetableArchive.Where(t => t.m_timetableInfo.Id == id);
            if (l.Any())
                return l.First();
            else if (TimetableArchive[0] != null)
                return TimetableArchive[0];
            else
                return null;
        }

    }
}