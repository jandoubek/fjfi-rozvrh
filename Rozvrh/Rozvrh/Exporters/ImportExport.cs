using Rozrvh.Exporters.Common;
using Rozrvh.Exporters.Generators;
using Rozvrh.Exporters.Generators;
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
        /// <summary> 
        /// Exports lectures to SVG file. 
        /// </summary> 
        /// <returns> SVG file in XML syntax. </returns>
        /// <param name="lectures">List of lectures with IExportHodina interface to export.</param>
        public ActionResult DownloadAsSVG(List<IExportHodina> lectures)
        {
            SvgGenerator gen = new SvgGenerator();
            string text = gen.generateSVG(lectures,"Rozvrh");
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            var res = new FileContentResult(buffer, "text/plain");
            res.FileDownloadName = "FJFIrozvrh.svg";
            return res;
        }

        /// <summary>
        /// Serializes lectures to XML file.
        /// </summary>
        /// <returns> XML serialization as text file. </returns>
        /// <param name="lectures">List of lectures with IExportHodina interface to export.</param>
        public ActionResult DownloadAsXML(List<IExportHodina> lectures)
        {
            var ser = new XmlSerializer(typeof(List<ExportHodina>));
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
        /// <param name="lectures">List of lectures with IExportHodina interface to export.</param>
        /// <param name="semStart">Beginning of the semester. Only Year, Month and Day are used. </param>
        /// <param name="semEnd">End of the semester. Only Year, Month and Day are used. </param>
        public ActionResult DownloadAsICAL(List<IExportHodina> lectures,DateTime semStart, DateTime semEnd)
        {
            ICalGenerator gen = new ICalGenerator();
            string text = gen.generateICal(lectures, semStart, semEnd);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            var res = new FileContentResult(buffer, "text/plain");
            res.FileDownloadName = "FJFIRozvrh.ical";
            return res;
        }

        /// <summary>
        /// Deserialize XML of lectures.
        /// </summary>
        /// <returns> List of lectures of ExportHodina class. </returns>
        /// <param name="file">File posted by HTTP POST.</param>
        public List<ExportHodina> ImportXML(HttpPostedFileBase file)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<ExportHodina>));
            List<ExportHodina> hodiny = new List<ExportHodina>();
            try
            {
                StreamReader f = new StreamReader(file.InputStream);
                hodiny = (List<ExportHodina>)ser.Deserialize(f);
            }
            catch (InvalidOperationException ioe)
            {
                throw new System.IO.InvalidDataException("Failed to load XML, the file was corrupted.");
            }

            return hodiny;
        }

    }
}
