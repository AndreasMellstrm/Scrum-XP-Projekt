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

namespace Örebro_Universitet_Kommunikation.Controllers {
    [Authorize]
    public class ResearchBlogController : Controller {
        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        public ResearchBlogController() {
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }

        // GET: ResearchBlog
        public ActionResult Index() {
            var researchProjects = Ctx.Projects;
            List<ProjectItem> researchList = new List<ProjectItem>();
            var isMember = false;
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            foreach (var p in researchProjects) {
                var listOfBlogs = Ctx.ResearchBlogs.Where(b => b.ProjectId == p.ProjectId);
                if (p.ProjectId == currentUser.ProjectId) {
                    isMember = true;
                }
                else {
                    isMember = false;
                }
                var item = new ProjectItem {
                    ProjectName = p.ProjectName,
                    Id = p.ProjectId,
                    BlogAmount = listOfBlogs.Count(),
                    IsMember = isMember
                };
                researchList.Add(item);
            }

            return View(new ProjectViewModel { ProjectItems = researchList });
        }
        public async Task<ActionResult> ShowResearch(int ResearchProject) {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentProject = Ctx.Projects.FirstOrDefault(p => p.ProjectId == ResearchProject);
            bool canCreate = false;
            if (currentUser.ProjectId != 0) {
                if (ResearchProject == currentUser.ProjectId) {
                    canCreate = true;
                }
            }
            List<ResearchBlogItem> researchList = new List<ResearchBlogItem>();
            bool canEdit = false;
            var ResearchList = Ctx.ResearchBlogs.Where(c => c.ProjectId == ResearchProject);
            foreach (var r in ResearchList) {
                var user = await UserManager.FindByIdAsync(r.CreatorId);

                if (currentUser.Id == r.CreatorId || currentUser.Admin) {
                    canEdit = true;
                }
                string CreatorMail = user.Email;
                if (user.IsInactive) {
                    CreatorMail = "Inaktiverad användare";
                }
                var blogItem = new ResearchBlogItem {
                    Id = r.Id,
                    CreatorId = r.CreatorId,
                    AttachedFile = r.AttachedFile,
                    CanDelete = canEdit,
                    Content = r.Content,
                    CreaterMail = CreatorMail,
                    CreatorFirstName = user.FirstName,
                    CreatorLastName = user.LastName,
                    Date = r.BlogEntryTime,
                    Title = r.Title
                };
                researchList.Add(blogItem);
            }
            researchList.OrderByDescending(r => r.Date);

            return View(new ResearchBlogViewModel { ResearchBlogList = researchList, ResearchName = currentProject.ProjectName, CanCreateEntry = canCreate });
        }

        public ActionResult CreateEntry() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEntry(CreateResearchViewModel model, HttpPostedFileBase File) {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var fileString = FileUpload(File);
            Ctx.ResearchBlogs.Add(new ResearchBlogModel {
                AttachedFile = fileString,
                ProjectId = user.ProjectId ?? default(int),
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
            return RedirectToAction("ShowResearch", new { ResearchProject = user.ProjectId });

        }
        public string FileUpload(HttpPostedFileBase File) {

            //Vi kollar att det finns en fil att spara
            if (File != null && File.ContentLength > 0) {
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
            else {
                return null;
            }
        }
        public ActionResult WriteComment(ResearchBlogCommentsViewModel newComment) {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            Ctx.ResearchBlogComments.Add(new ResearchBlogCommentsModel {
                BlogId = newComment.BlogId,
                Content = newComment.CommentContent,
                Time = DateTime.Now,
                CreatorId = currentUser.Id
            });
            Ctx.SaveChanges();

            return RedirectToAction("ShowComments", new { newComment.BlogId });
        }
        public async Task<ActionResult> ShowComments(int BlogId) {
            var BlogEntry = Ctx.ResearchBlogs.FirstOrDefault(b => b.Id == BlogId);
            if (BlogEntry != null) {
                bool canDelete = false;
                var CommentList = Ctx.ResearchBlogComments.Where(c => c.BlogId == BlogId).OrderByDescending(c => c.BlogId);
                var BloggUser = await UserManager.FindByIdAsync(BlogEntry.CreatorId);
                List<ResearchComment> Comments = new List<ResearchComment>();
                var currentUser = UserManager.FindById(User.Identity.GetUserId());
                var currentProject = Ctx.Projects.FirstOrDefault(p => p.ProjectId == BlogEntry.ProjectId);
                bool isAdmin = currentUser.Admin;

                foreach (var c in CommentList) {
                    if (isAdmin || currentUser.Id == c.CreatorId || BlogEntry.CreatorId == currentUser.Id) {
                        canDelete = true;
                    }
                    else {
                        canDelete = false;
                    }
                }
                foreach (var co in CommentList) {
                    var User = await UserManager.FindByIdAsync(co.CreatorId);
                    string CreaterMail = User.Email;
                    if (User.IsInactive) {
                        CreaterMail = "Inaktiverad användare";
                    }
                    var CommentItem = new ResearchComment {
                        Content = co.Content,
                        Time = co.Time,
                        Email = CreaterMail,
                        FirstName = User.FirstName,
                        LastName = User.LastName,
                        CanDelete = canDelete,
                        Id = co.BlogId

                    };
                    Comments.Add(CommentItem);
                }
                string CreatorMail = BloggUser.Email;
                if (BloggUser.IsInactive) {
                    CreatorMail = "Inaktiverad användare";
                }
                return View(new ResearchBlogCommentsViewModel {
                    AttachedFile = BlogEntry.AttachedFile,
                    BlogId = BlogEntry.Id,

                    Comments = Comments,
                    Content = BlogEntry.Content,
                    Date = BlogEntry.BlogEntryTime,
                    Title = BlogEntry.Title,
                    CreatorMail = CreatorMail,
                    CreatorFirstName = BloggUser.FirstName,
                    CreatorLastName = BloggUser.LastName,
                    ProjectName = currentProject.ProjectName,
                    ProjectId = currentProject.ProjectId
                });
            }

            return RedirectToAction("Index", "ResearchBlog");
        }

        public ActionResult DeleteComment(int EntryId, int BlogId) {
            ResearchBlogCommentsModel researchComments = Ctx.ResearchBlogComments.Find(EntryId);

            Ctx.ResearchBlogComments.Remove(researchComments);
            Ctx.SaveChanges();

            return RedirectToAction("ShowComments", new { BlogId });
        }
    }
}