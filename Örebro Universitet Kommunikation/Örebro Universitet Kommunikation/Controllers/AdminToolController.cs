using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Örebro_Universitet_Kommunikation.Models;

namespace Örebro_Universitet_Kommunikation.Controllers
{
    public class AdminToolController : Controller
    {
        // GET: AdminTool
        public ActionResult Index(bool admin)
        {
            if (admin) {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}