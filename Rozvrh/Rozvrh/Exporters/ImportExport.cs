using Rozvrh.Exporters.Common;
using Rozvrh.Exporters.Generators;
using Rozvrh.Exporters.Generators;
using Rozvrh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Rozvrh.Exporters
{
    /// <summary>
    ///  Class exporting to ICal and SVG format via ActionResult File. It also imports and exports lectures using XML serialization.
    /// </summary>
    public class ImportExport
    {

        private static ImportExport instance;
        private List<TimetableField> lectures;

        private ImportExport() 
        {
            lectures = new List<TimetableField>();
        }

        /// <summary>
        ///  Singleton instance.
        /// </summary>
        public static ImportExport Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImportExport();
                }
                return instance;
            }
        }

        /// <summary> 
        /// Sets current lectures ready export.
        /// </summary> 
        /// <param name="l">List of lectures.</param>
        public void setLectures(List<TimetableField> l)
        {
            lectures = l;
        }
        

        /// <summary> 
        /// Exports lectures to SVG file. 
        /// </summary> 
        /// <returns> SVG file in XML syntax. </returns>
        /// <param name="lectures">List of lectures.</param>
        /// <param name="title">Title displayed at top-left corner of the SVG.</param>
        public ActionResult DownloadAsSVG(string title)
        {
            SvgGenerator gen = new SvgGenerator();
            string text = gen.generateSVG(convertToExportFormat(lectures), title);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            var res = new FileContentResult(buffer, "text/plain");
            res.FileDownloadName = "FJFIrozvrh.svg";
            return res;
        }

        /// <summary>
        /// Serializes lectures to XML file.
        /// </summary>
        /// <returns> XML serialization as text file. </returns>
        public ActionResult DownloadAsXML()
        {
            var ser = new XmlSerializer(typeof(List<TimetableField>));
            using (var ms = new MemoryStream())
            {
                ser.Serialize(ms, lectures);
                var bytes = ms.ToArray();
                var res = new FileContentResult(bytes, "text/xml");
                res.FileDownloadName = "FJFIRozvrh.XML" ;
                return res;
            }
        }

        /// <summary>
        /// Exports lectures to ICal format.
        /// </summary>
        /// <returns> Text file in ICal format. </returns>
        /// <param name="semStart">Beginning of the semester. Only Year, Month and Day are used. </param>
        /// <param name="semEnd">End of the semester. Only Year, Month and Day are used. </param>
        public ActionResult DownloadAsICAL(DateTime semStart, DateTime semEnd)
        {
            ICalGenerator gen = new ICalGenerator();
            string text = gen.generateICal(convertToExportFormat(lectures), semStart, semEnd);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            var res = new FileContentResult(buffer, "text/plain");
            res.FileDownloadName = "FJFIRozvrh.ical";
            return res;
        }

        /// <summary>
        /// Deserialize XML of lectures.
        /// </summary>
        /// <returns> List of lectures of TimetableField class. </returns>
        /// <param name="file">File posted by HTTP POST.</param>
        /// <exception cref="InvalidDataException">The XML cannot be deserialized.</exception>
        public List<TimetableField> ImportXML(HttpPostedFileBase file)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<TimetableField>));
            List<TimetableField> hodiny = new List<TimetableField>();
            try
            {
                StreamReader f = new StreamReader(file.InputStream);
                hodiny = (List<TimetableField>)ser.Deserialize(f);
            }
            catch (InvalidOperationException ioe)
            {
                throw new System.IO.InvalidDataException("Failed to load XML, the file was corrupted.");
            }

            return hodiny;
        }

        private List<ExportLecture> convertToExportFormat(List<TimetableField> lectures)
        {
            var res = new List<ExportLecture>();
            if (lectures != null)
            {
                foreach (var lecture in lectures)
                {
                    res.Add(new ExportLecture(lecture));
                }
            }
            
            return res;
        }

    }
}
