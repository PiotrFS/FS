using System;
using System.Web;
using System.Web.Mvc;
using FS.Business;

namespace FS.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file, DateTime? reportToDate)
        {
            // very basic guards against missing input
            if (file == null) return Content("No file provided");
            if (reportToDate == null) return Content("No Report Date provided");

            var reportManager = new ReportManager(reportToDate.Value, file.InputStream);
            reportManager.GenerateReports();

            return View("ShowReports", reportManager);
        }
    }
}