using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
    [Authorize]
    public class EducationBlogController : Controller
    {
        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        public EducationBlogController() {
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }
        // GET: EducationBlog
        public async Task<ActionResult> Index()
        {
            var EducationBlog = Ctx.EducationBlogs.OrderByDescending(e => e.Time);
            List<EducationBlogItem> educationList = new List<EducationBlogItem>();
            foreach(var e in EducationBlog) {
                var user = await UserManager.FindByIdAsync(e.CreatorId);
                var comments = Ctx.EducationBlogComments.Where(b => b.BlogId == e.Id);
                var item = new EducationBlogItem {
                    AttachedFile = e.AttachedFile,
                    Content = e.Content,
                    Comments = comments.Count(),
                    Date = e.Time,
                    Id = e.Id,
                    CreatorId = e.CreatorId,
                    Title = e.Title,
                    CreatorFirstName = user.FirstName,
                    CreatorLastName = user.LastName,
                    CreatorMail = user.Email
                };
                educationList.Add(item);
            }
            return View(new EducationBlogViewModel { EducationBlogList = educationList});
        }
        public ActionResult CreateEntry() {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEntry(CreateEducationViewModel model, HttpPostedFileBase File) {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var fileString = FileUpload(File);
            Ctx.EducationBlogs.Add(new EducationBlogModel {
                AttachedFile = fileString,
                Time = DateTime.Now,
                Title = model.Title,
                Content = model.Content,
                CreatorId = user.Id
            });
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
            return RedirectToAction("Index");
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
        public ActionResult WriteComment(EducationBlogCommentsViewModel newComment)
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            Ctx.EducationBlogComments.Add(new EducationBlogCommentsModel
            {
                BlogId = newComment.BlogId,
                Content = newComment.CommentContent,
                Time = DateTime.Now,
                CreatorId = currentUser.Id
            });
            Ctx.SaveChanges();

            return RedirectToAction("ShowComments", new { newComment.BlogId });
        }
        public async Task<ActionResult> ShowComments(int BlogId)
        {
            var BlogEntry = Ctx.EducationBlogs.FirstOrDefault(b => b.Id == BlogId);
            if (BlogEntry != null)
            {
                var CommentList = Ctx.EducationBlogComments.Where(c => c.BlogId == BlogId).OrderByDescending(c => c.BlogId);
                var BloggUser = await UserManager.FindByIdAsync(BlogEntry.CreatorId);
                List<EducationComment> Comments = new List<EducationComment>();
                foreach (var c in CommentList)
                {
                    var User = await UserManager.FindByIdAsync(c.CreatorId);

                    var CommentItem = new EducationComment
                    {
                        Content = c.Content,
                        Time = c.Time,
                        Email = User.Email,
                        FirstName = User.FirstName,
                        LastName = User.LastName

                    };
                    Comments.Add(CommentItem);
                }
                return View(new EducationBlogCommentsViewModel
                {
                    AttachedFile = BlogEntry.AttachedFile,
                    BlogId = BlogEntry.Id,
                    
                    Comments = Comments,
                    Content = BlogEntry.Content,
                    Date = BlogEntry.Time,
                    Title = BlogEntry.Title,
                    CreatorMail = BloggUser.Email,
                    CreatorFirstName = BloggUser.FirstName,
                    CreatorLastName = BloggUser.LastName

                });
            }
            return RedirectToAction("Index", "EducationBlog");
        }
    }

}