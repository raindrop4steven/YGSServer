using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YGSServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("index");
        }

        public ActionResult ApplyForm()
        {
            return View("applyForm");
        }

        public ActionResult Demo()
        {
            return View("index");
        }

        public ActionResult Download()
        {
            return View("Download");
        }

        public ActionResult Check()
        {
            return View("Check");
        }

        public ActionResult Cred()
        {
            return View("Cred");
        }

        public ActionResult CertificatesDetail()
        {
            return View("CertificatesDetail");
        }
    }
}