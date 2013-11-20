using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Rozrvh.Exporters.Common;
using Rozvrh.Exporters;

namespace Rozvrh.Controllers
{
    public class ExportController : Controller
    {
        public ActionResult ToICal()
        {
            var ie = new ImportExport();
            return ie.DownloadAsICAL(generateTestExport(), new DateTime(2013, 9, 23), new DateTime(2013, 12, 22));

        }

        public ActionResult ToSvg()
        {
            var ie = new ImportExport();
            return ie.DownloadAsSVG(generateTestExport());
        }

        private List<IExportHodina> generateTestExport()
        {
            var testExport = new List<IExportHodina>();
            testExport.Add(new ExportHodina("VSBP", DayOfWeek.Monday, new DateTime(2000, 1, 1, 14, 30, 0), new TimeSpan(2, 0, 0), "Vopálka", "B-314"));
            testExport.Add(new ExportHodina("TPC", DayOfWeek.Tuesday, new DateTime(2000, 1, 1, 8, 30, 0), new TimeSpan(2, 0, 0), "Štamberg", "--"));
            testExport.Add(new ExportHodina("RAO", DayOfWeek.Tuesday, new DateTime(2000, 1, 1, 13, 30, 0), new TimeSpan(4, 0, 0), "Vrba", "B-115"));
            testExport.Add(new ExportHodina("CHRP", DayOfWeek.Wednesday, new DateTime(2000, 1, 1, 10, 30, 0), new TimeSpan(2, 0, 0), "John", "B-314"));
            return testExport;
        }

    }
}
