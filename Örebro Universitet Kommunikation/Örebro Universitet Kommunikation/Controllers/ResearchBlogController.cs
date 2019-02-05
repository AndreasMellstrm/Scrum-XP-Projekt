using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Örebro_Universitet_Kommunikation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Controllers
{
    public class ResearchBlogController : Controller
    {
        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        public ResearchBlogController() {
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }

        // GET: ResearchBlog
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult ShowResearch(int ResearchProject) {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            if(currentUser.ProjectId == ResearchProject) {
                var ResearchList = Ctx.ResearchBlogs.Where(c => c.ProjectId == ResearchProject);
                foreach(var r in ResearchList) {

                }

            }

            return View(); // Redirect till index
        }
        
    }
}