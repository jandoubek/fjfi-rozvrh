using Rozrvh;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace MvcApplication1.Controllers
{
    public class DownloadAsXMLController : Controller
    {
        //
        // GET: /DownloadAsXML/

        public String Index()
        {
            return "To download as XML go to Download/";
        }

        public ActionResult Download()
        {
            var hodiny = new List<ExportHodina>();
            var ser = new XmlSerializer(typeof(List<ExportHodina>));
            using (var ms = new MemoryStream())
            {
                ser.Serialize(ms, hodiny);
                var bytes = ms.ToArray();
                return File(bytes, "text/xml", "FJFIrozvrh.xml");               
            }
        }

    }
}
