using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Rozvrh.Exporters.Common;
using Rozvrh.Exporters;

namespace Rozvrh.Controllers
{
    public class ExportController : Controller
    {
        public ActionResult ToICal()
        {
            var ie = new ImportExport();
            //Vasek: FIX ME 
            //Misto bla bla prijde odkaz na singleton model, konkretne na jeho TimetableFields
            //return ie.DownloadAsICAL(blabla, new DateTime(2013, 9, 23), new DateTime(2013, 12, 22));
            return null;

        }

        public ActionResult ToSvg()
        {
            var ie = new ImportExport();
            //Vasek: FIX ME 
            //Misto bla bla prijde odkaz na singleton model, konkretne na jeho TimetableFields
            //return ie.DownloadAsSVG(blabla);
            return null;
        }

    }
}
