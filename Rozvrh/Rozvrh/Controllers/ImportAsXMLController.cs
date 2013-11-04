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
    public class ImportAsXMLController : Controller
    {
        //
        // GET: /ImportAsXML/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /ImportAsXML/Import

        [HttpPost]
        public String Import(HttpPostedFileBase file)
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
                return "Failed to load XML, the file is corrupted.";
            }
            string s = "";

            foreach (ExportHodina h in hodiny)
            {
                s += h.Name + " " + h.Day.ToString() + System.Environment.NewLine;
            }

            return s;
        }

    }
}
