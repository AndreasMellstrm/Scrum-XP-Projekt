using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Controllers
{
    public class InformalBlogController : Controller
    {
        // GET: InformalBlog
        public ActionResult Index()
        {
            return View();
        }
    }
}