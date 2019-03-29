using Appkiz.Library.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YGSServer.Common;


namespace YGSServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                // 获得当前用户
                var employee = (User.Identity as AppkizIdentity).Employee;
                return View("index");
            }
            catch
            {
                return Redirect("/Frame/NeedLogon.aspx?ReturnUrl=/Apps/YGS");
            }
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
            try
            {
                // 获得当前用户
                var employee = (User.Identity as AppkizIdentity).Employee;
                return View("Download");
            }
            catch
            {
                return Redirect("/Frame/NeedLogon.aspx?ReturnUrl=/Apps/YGS");
            }
        }

        public ActionResult Check()
        {
            try
            {
                // 获得当前用户
                var employee = (User.Identity as AppkizIdentity).Employee;
                return View("Check");
            }
            catch
            {
                return Redirect("/Frame/NeedLogon.aspx?ReturnUrl=/Apps/YGS");
            }
        }

        public ActionResult Cred()
        {
            try
            {
                // 获得当前用户
                var employee = (User.Identity as AppkizIdentity).Employee;
                return View("Cred");
            }
            catch
            {
                return Redirect("/Frame/NeedLogon.aspx?ReturnUrl=/Apps/YGS");
            }
        }

        public ActionResult CertificatesDetail()
        {
            return View("CertificatesDetail");
        }
    }
}