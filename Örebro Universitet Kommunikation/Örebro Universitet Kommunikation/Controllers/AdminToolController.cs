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
        public ApplicationDbContext Ctx { get; set; }

        public AdminToolController() {
            Ctx = new ApplicationDbContext();
        }
        // GET: AdminTool
        public ActionResult Index(ApplicationUser user)
        {
            var admins = (from a in Ctx.Users
                          where a.Admin == true
                          select a).ToList();
            if (admins.Contains(user)) {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}