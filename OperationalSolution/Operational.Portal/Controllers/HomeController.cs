using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Operational.Portal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("OperatorReport");
        }

        public ActionResult About()
        {            
            return View();
        }

        

        public ActionResult OperatorReport()
        {            
            return View();
        }

       
    }
}