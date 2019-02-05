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
            var researchProjects = Ctx.Projects;
            List<ProjectItem> researchList = new List<ProjectItem>();
            foreach(var p in researchProjects) {
                var item = new ProjectItem {
                    ProjectName = p.ProjectName,
                    Id = p.ProjectId
                };
                researchList.Add(item);
            }

            return View(new ProjectViewModel { ProjectItems = researchList });
        }
        public async Task<ActionResult> ShowResearch(int ResearchProject) {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentProject = Ctx.Projects.FirstOrDefault(p => p.ProjectId == ResearchProject);

            List<ResearchBlogItem> researchList = new List<ResearchBlogItem>();
            bool canEdit = false;
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
        
    }
}