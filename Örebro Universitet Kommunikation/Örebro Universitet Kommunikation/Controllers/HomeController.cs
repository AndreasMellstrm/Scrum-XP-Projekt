﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Controllers {
    [Authorize]
    public class HomeController : Controller {
        public ActionResult Index() {




            return RedirectToAction("Index", "FormalBlog");
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

    }
}