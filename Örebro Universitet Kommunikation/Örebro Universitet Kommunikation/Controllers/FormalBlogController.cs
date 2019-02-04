using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Örebro_Universitet_Kommunikation.Helpers;
using Örebro_Universitet_Kommunikation.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Örebro_Universitet_Kommunikation.Controllers {
    public class FormalBlogController : Controller {

        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }


        public FormalBlogController(){
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }

        public async Task<ActionResult> Index() {
            Ctx = new ApplicationDbContext();
            var profileList = Ctx.Users.ToList();

            var BlogEntries = (from BE in Ctx.FormalBlogEntries
                               orderby BE.Id descending
                               select BE).ToList();

            List<FormalBlogItem> FormalBlogItemList = new List<FormalBlogItem>();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentUserId = currentUser.Id;
            bool CanDelete = false;
            var CurrentUserAdmin = currentUser.Admin;

            foreach (var item in BlogEntries) {
                var user = await UserManager.FindByIdAsync(item.CreatorId);
                
                if (currentUserId.Equals(item.CreatorId) || CurrentUserAdmin) {
                    CanDelete = true;
                }

                var blogItem = new FormalBlogItem {
                    Id = item.Id,
                    CreatorId = item.CreatorId,
                    CreatorFirstName = user.FirstName,
                    CreatorLastName = user.LastName,
                    AttachedFile = item.AttachedFile,
                    Comments = 0,
                    Date = item.BlogEntryTime,
                    Content = item.Content,
                    Category = item.Category,
                    Title = item.Title,
                    CanDelete = CanDelete
                };

                FormalBlogItemList.Add(blogItem);
            };
        
            return View(new FormalBlogViewModel {
                FormalBlogItems = FormalBlogItemList
            });
        }
        public ActionResult CreateEntry() {
            var CategoryList = Ctx.Categories.Where(c => c.CategoryType == "Formal").ToList();
            List<string> CategoryListName = new List<string>();
            foreach (var c in CategoryList) {

                CategoryListName.Add(c.CategoryName);
            }
            var Id = User.Identity.GetUserId();
            return View(new CreateEntryViewModel {
                CategoryList = CategoryListName
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEntry(CreateEntryViewModel model, HttpPostedFileBase File, string Category) {
            
            var user = UserManager.FindById(User.Identity.GetUserId());
            var fileString = FileUpload(File);
            Ctx.FormalBlogEntries.Add(new FormalBlogEntry {
                AttachedFile = fileString,
                Category = Category,
                BlogEntryTime = DateTime.Now,
                Title = model.Title,
                Content = model.Content,
                CreatorId = user.Id
            }
            );
            Ctx.SaveChanges();
            var EmailRecipients = (from U in Ctx.Users
                               where U.Notifications == "Email"
                               || U.Notifications == "EmailSms"
                               where U.Id != user.Id
                               select U).ToList();
            string subject = "Nytt inlägg från " + user.FirstName + ".";
            string emailText = "Inlägg med rubrik: " + model.Title + " finns nu att läsa.";
            foreach (var appUser in EmailRecipients) {
                var emailHelper = new EmailHelper("orukommunikation@gmail.com", "Kakan1210", appUser.Email);
                emailHelper.SendEMail(appUser.Email, subject, emailText);
            }
            return RedirectToAction("Index", "FormalBlog");
        }

        [HttpGet]
        public ActionResult EditEntry(int EntryId) {
            var BlogEntry = Ctx.FormalBlogEntries.FirstOrDefault(b => b.Id == EntryId);
            var CategoryList = Ctx.Categories.Where(c => c.CategoryType == "Formal").ToList();
            List<string> CategoryListName = new List<string>();
            foreach (var c in CategoryList) {

                CategoryListName.Add(c.CategoryName);
            }
            var blogItem1 = new EditEntryViewModel() {
                Id = BlogEntry.Id,
                AttachedFile = BlogEntry.AttachedFile,
                Category = BlogEntry.Category,
                Content = BlogEntry.Content,
                Title = BlogEntry.Title,
                CategoryItems = CategoryListName

            };
            return View(blogItem1);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditEntryViewModel model, HttpPostedFileBase File, int Id) {
            if (ModelState.IsValid) {
                var entry = Ctx.FormalBlogEntries.FirstOrDefault(b => b.Id == Id);
                var Filestring = FileUpload(File);


                entry.AttachedFile = Filestring;
                entry.Category = model.Category;


                entry.Content = model.Content;
                entry.Title = model.Title;

                Ctx.SaveChanges();
            }


            return RedirectToAction("Index", "FormalBlog");
        }
        
        public ActionResult DeleteLink(int EntryId) {
            var entry = Ctx.FormalBlogEntries.FirstOrDefault(b => b.Id == EntryId);
            if (ModelState.IsValid) {
                entry.AttachedFile = null;
                Ctx.SaveChanges();
            }

            return RedirectToAction("Index", "FormalBlog");
        }

        public ActionResult DeleteEntry(int EntryId, string CreatorId) {
            FormalBlogEntry blogEntry =  Ctx.FormalBlogEntries.Find(EntryId);
            
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentUserId = currentUser.Id;

            // currentUserId.Equals(CreatorId)
            Ctx.FormalBlogEntries.Remove(blogEntry);
            Ctx.SaveChanges();

            return RedirectToAction("Index", "FormalBlog");
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
        
        public ActionResult TempUpload(HttpPostedFileBase File)
        {
            var fileString = FileUpload(File);
            Debug.WriteLine(fileString);
            return View();
        }
        
       public async Task <ActionResult> ShowComments(int BlogId)
        {
            var BlogEntry = Ctx.FormalBlogEntries.FirstOrDefault(b => b.Id == BlogId);
            if (BlogEntry != null) { 
                var CommentList = Ctx.BlogComments.Where(c => c.BlogId == BlogId).OrderByDescending(c => c.BlogId);
                var BloggUser = await UserManager.FindByIdAsync(BlogEntry.CreatorId);
                List<Comment> Comments = new List<Comment>();
                foreach(var c in CommentList)
                {
                    var User = await UserManager.FindByIdAsync(c.CreatorId);

                    var CommentItem = new Comment
                    {
                        Content = c.Content,
                        Time = c.Time,
                        Email = User.Email,
                        FirstName = User.FirstName,
                        LastName = User.LastName
                    };
                    Comments.Add(CommentItem);
                }
                return View(new FormalBlogCommentsViewModel
                {
                    AttachedFile = BlogEntry.AttachedFile,
                    BlogId = BlogEntry.Id,
                    Category = BlogEntry.Category,
                    Comments = Comments,
                    Content = BlogEntry.Content,
                    Date = BlogEntry.BlogEntryTime,
                    Title = BlogEntry.Title,
                    CreaterMail = BloggUser.Email,
                    CreatorFirstName = BloggUser.FirstName,
                    CreatorLastName = BloggUser.LastName
                });
            }
            return RedirectToAction("Index", "FormalBlog");
        }
        public ActionResult WriteComment(FormalBlogCommentsViewModel newComment) {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            Ctx.BlogComments.Add(new FormalBlogCommentsModel {
                BlogId = newComment.BlogId,
                Content = newComment.CommentContent,
                Time = DateTime.Now,
                CreatorId = currentUser.Id
            });
            Ctx.SaveChanges();

            return RedirectToAction("ShowComments", new { BlogId = newComment.BlogId});
        }
    }
}