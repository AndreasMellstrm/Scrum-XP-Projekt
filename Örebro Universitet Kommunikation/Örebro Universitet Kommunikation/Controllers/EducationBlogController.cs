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

namespace Örebro_Universitet_Kommunikation.Controllers {
    [Authorize]
    public class EducationBlogController : Controller {
        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        public EducationBlogController() {
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }
        // GET: EducationBlog
        public async Task<ActionResult> Index() {
            var EducationBlog = Ctx.EducationBlogs.OrderByDescending(e => e.Time);
            List<EducationBlogItem> educationList = new List<EducationBlogItem>();
            bool canDelete = false;
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            bool isAdmin = currentUser.Admin;
            foreach (var e in EducationBlog) {
                if (isAdmin || currentUser.Id == e.CreatorId)
                {
                    canDelete = true;
                }
                else
                {
                    canDelete = false;
                }
                var user = await UserManager.FindByIdAsync(e.CreatorId);
                var creatorMail = user.Email;
                if (user.IsInactive) {
                    creatorMail = "Inaktiverad användare";
                }
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
                    CreatorMail = creatorMail,
                    CanDelete = canDelete
                };
                educationList.Add(item);
            }
            return View(new EducationBlogViewModel { EducationBlogList = educationList });
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
        public ActionResult WriteComment(EducationBlogCommentsViewModel newComment) {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            Ctx.EducationBlogComments.Add(new EducationBlogCommentsModel {
                BlogId = newComment.BlogId,
                Content = newComment.CommentContent,
                Time = DateTime.Now,
                CreatorId = currentUser.Id
            });
            Ctx.SaveChanges();

            return RedirectToAction("ShowComments", new { newComment.BlogId });
        }
        public async Task<ActionResult> ShowComments(int BlogId) {
            var BlogEntry = Ctx.EducationBlogs.FirstOrDefault(b => b.Id == BlogId);
            if (BlogEntry != null) {
                bool canDelete = false;
                var CommentList = Ctx.EducationBlogComments.Where(c => c.BlogId == BlogId).OrderByDescending(c => c.BlogId);
                var BloggUser = await UserManager.FindByIdAsync(BlogEntry.CreatorId);
                var BloggUserMail = BloggUser.Email;
                if (BloggUser.IsInactive) {
                    BloggUserMail = "Inaktiverad användare";
                }
                bool isAdmin = BloggUser.Admin;
                List<EducationComment> Comments = new List<EducationComment>();
                foreach (var c in CommentList) {
                    if (isAdmin || BloggUser.Id == c.CreatorId || BlogEntry.CreatorId == BloggUser.Id) {
                        canDelete = true;
                    }
                    else {
                        canDelete = false;
                    }
                }
                foreach (var co in CommentList) {
                    var User = await UserManager.FindByIdAsync(co.CreatorId);
                    var creatorMail = User.Email;
                    if (User.IsInactive) {
                        creatorMail = "Inaktiverad användare";
                    }
                    var CommentItem = new EducationComment {
                        Content = co.Content,
                        Time = co.Time,
                        Email = creatorMail,
                        FirstName = User.FirstName,
                        LastName = User.LastName,
                        CanDelete = canDelete,
                        Id = co.Id,
                        BlogId = co.BlogId

                    };
                    Comments.Add(CommentItem);
                }
                return View(new EducationBlogCommentsViewModel {
                    AttachedFile = BlogEntry.AttachedFile,
                    BlogId = BlogEntry.Id,

                    Comments = Comments,
                    Content = BlogEntry.Content,
                    Date = BlogEntry.Time,
                    Title = BlogEntry.Title,
                    CreatorMail = BloggUserMail,
                    CreatorFirstName = BloggUser.FirstName,
                    CreatorLastName = BloggUser.LastName

                });
            }
            return RedirectToAction("Index", "EducationBlog");
        }

        public ActionResult DeleteComment(int Id, int BlogId) {
            EducationBlogCommentsModel educationComments = Ctx.EducationBlogComments.Find(Id);

            Ctx.EducationBlogComments.Remove(educationComments);
            Ctx.SaveChanges();

            return RedirectToAction("ShowComments", new { BlogId });
        }
        [HttpGet]
        public ActionResult EditEntry(int EntryId)
        {
            var BlogEntry = Ctx.EducationBlogs.FirstOrDefault(b => b.Id == EntryId);
            //var CategoryList = Ctx.Categories.Where(c => c.CategoryType == "Formal").ToList();
            //List<string> CategoryListName = new List<string>();
            //foreach (var c in CategoryList)
            //{

            //    CategoryListName.Add(c.CategoryName);
            //}
            var blogItem1 = new EditEducationViewModel()
            {
                Id = BlogEntry.Id,
                AttachedFile = BlogEntry.AttachedFile,
                //Category = BlogEntry.Category,
                Content = BlogEntry.Content,
                Title = BlogEntry.Title,
               
                //CategoryItems = CategoryListName

            };
            return View(blogItem1);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditEducationViewModel model, HttpPostedFileBase File, int Id)
        {
           
            if (ModelState.IsValid)
            {
                var entry = Ctx.EducationBlogs.FirstOrDefault(b => b.Id == Id);

                var Filestring = FileUpload(File);
                if (Filestring != null)
                {
                    entry.AttachedFile = Filestring;
                }
                else
                {
                    entry.AttachedFile = model.AttachedFile;
                }

                //entry.Category = model.Category;
                //entry.ProjectId = model.ProjectId;

                entry.Content = model.Content;
                entry.Title = model.Title;
                
                Ctx.SaveChanges();
            }


            return RedirectToAction("Index");
        }
        public ActionResult DeleteEntry(int EntryId, string CreatorId)
        {
            EducationBlogModel blogEntry = Ctx.EducationBlogs.Find(EntryId);

            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentUserId = currentUser.Id;

            // currentUserId.Equals(CreatorId)
            Ctx.EducationBlogs.Remove(blogEntry);
            Ctx.SaveChanges();

            return RedirectToAction("Index", "EducationBlog");
        }
        public ActionResult DeleteLink(int EntryId)
        {
            var entry = Ctx.EducationBlogs.FirstOrDefault(b => b.Id == EntryId);
            if (ModelState.IsValid)
            {
                entry.AttachedFile = null;
                Ctx.SaveChanges();
            }

            return RedirectToAction("Index", "EducationBlog");
        }
    }

}
