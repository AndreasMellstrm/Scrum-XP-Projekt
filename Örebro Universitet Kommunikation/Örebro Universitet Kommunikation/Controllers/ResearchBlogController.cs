using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Örebro_Universitet_Kommunikation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult> ShowResearch(int ResearchProject) {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentUserProject = currentUser.Project.ProjectId;
            var currentProject = Ctx.Projects.FirstOrDefault(p => p.ProjectId == ResearchProject);

            List<ResearchBlogItem> researchList = new List<ResearchBlogItem>();
            bool canEdit = false;
            if (currentUserProject == ResearchProject) {
                var ResearchList = Ctx.ResearchBlogs.Where(c => c.ProjectId == ResearchProject);
                foreach (var r in ResearchList) {
                    var user = await UserManager.FindByIdAsync(r.CreatorId);
                    
                    if(currentUser.Id == r.CreatorId || currentUser.Admin) {
                        canEdit = true;
                    }
                    var blogItem = new ResearchBlogItem {
                        CreatorId = r.CreatorId,
                        AttachedFile = r.AttachedFile,
                        CanDelete = canEdit,
                        Content = r.Content,
                        CreaterMail = user.Email,
                        CreatorFirstName = user.FirstName,
                        CreatorLastName = user.LastName,
                        Date = r.BlogEntryTime,
                        Title = r.Title
                    };
                    researchList.Add(blogItem);
                }
                researchList.OrderByDescending(r => r.Date);

                return View(new ResearchBlogViewModel { ResearchBlogList = researchList, ResearchName = currentProject.ProjectName });
            }
            return RedirectToAction("Index", "ResearchBlog");
        }
        
    }
}