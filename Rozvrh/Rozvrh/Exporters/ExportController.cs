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
    public class ExportController : Controller
    {
        //
        // GET: /Export/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DownloadAsSVG(List<IExportHodina> hodiny)
        {
            SvgGenerator gen = new SvgGenerator();
            string text = gen.generateSVG(hodiny,"Rozvrh");
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            return File(buffer, "text/plain", "FJFIrozvrh.svg");
        }

        public ActionResult DownloadAsXML(List<IExportHodina> hodiny)
        {
            var ser = new XmlSerializer(typeof(List<ExportHodina>));
            using (var ms = new MemoryStream())
            {
                ser.Serialize(ms, hodiny);
                var bytes = ms.ToArray();
                return File(bytes, "text/xml", "FJFIrozvrh.xml");
            }
        }

        public ActionResult DownloadAsICAL(List<IExportHodina> hodiny,DateTime semStart, DateTime semEnd)
        {
            ICalGenerator gen = new ICalGenerator();
            string text = gen.generateICal(hodiny, semStart, semEnd);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            return File(buffer, "text/plain", "FJFIrozvrh.ical");
        }

        [HttpPost]
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
            string s = "";

            return hodiny;
        }

    }
}
