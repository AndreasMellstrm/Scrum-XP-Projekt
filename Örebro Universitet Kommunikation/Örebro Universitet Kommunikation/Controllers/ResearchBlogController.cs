using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
//using Örebro_Universitet_Kommunikation.Helpers;
using Örebro_Universitet_Kommunikation.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        
        public ActionResult CreateEntry()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEntry(CreateResearchViewModel model, HttpPostedFileBase File)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var fileString = FileUpload(File);
            Ctx.ResearchBlogs.Add(new ResearchBlogModel
            {
                AttachedFile = fileString,
                ProjectId = model.ProjectId,
                BlogEntryTime = DateTime.Now,
                Title = model.Title,
                Content = model.Content,
                CreatorId = user.Id
            }
            );
            Ctx.SaveChanges();
            //var EmailRecipients = (from U in Ctx.Users
            //                       where U.Notifications == "Email"
            //                       || U.Notifications == "EmailSms"
            //                       where U.Id != user.Id
            //                       select U).ToList();
            //string subject = "Nytt inlägg från " + user.FirstName + ".";
            //string emailText = "Inlägg med rubrik: " + model.Title + " finns nu att läsa.";
            //foreach (var appUser in EmailRecipients)
            //{
            //    var emailHelper = new EmailHelper("orukommunikation@gmail.com", "Kakan1210", appUser.Email);
            //    emailHelper.SendEMail(appUser.Email, subject, emailText);
            //}
            return RedirectToAction("ShowResearch", "ResearchBlog", model.ProjectId);
          
        }
        public string FileUpload(HttpPostedFileBase File)
        {

            //Vi kollar att det finns en fil att spara
            if (File != null && File.ContentLength > 0)
            {
                //Hämtar filnamnet utan filändelse
                var NoExtension = Path.GetFileNameWithoutExtension(File.FileName);
                //Hämtar filändelsen
                var Extension = Path.GetExtension(File.FileName);
                //Lägger ihop de båda, men med nuvarande tidpukt i namnet.
                //Det görs för att filnamnet ska bli unikt.
                var NameOfFile = NoExtension + DateTime.Now.ToString("yyyy-MM-dd-fff") + Extension;
                //Filen där filerna sparas och filnamnet slås ihop till en sträng
                var NameOfPath = "/Content/Files/" + NameOfFile;
                string FilePath = Path.Combine(Server.MapPath("~/Content/Files/"), NameOfFile);
                //Filen sparas
                File.SaveAs(FilePath);

                return NameOfPath;
            }
            else
            {
                return null;
            }
        }
    }
}