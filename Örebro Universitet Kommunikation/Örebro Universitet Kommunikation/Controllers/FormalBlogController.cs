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
using System.Data.Entity;

namespace Örebro_Universitet_Kommunikation.Controllers {
    public class FormalBlogController : Controller {

        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }


        public FormalBlogController() {
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }

        public async Task<ActionResult> Index(string searchString, string Category) {

            var user = UserManager.FindById(User.Identity.GetUserId());

            var blockedCategories = (from bc in Ctx.BlockedCategories
                                     where bc.UserId == user.Id
                                     && bc.CategoryType == "Formal"
                                     select bc).ToList();

            var items = from m in Ctx.FormalBlogEntries orderby m.BlogEntryTime descending select m;



            if (String.IsNullOrEmpty(searchString)) {
                searchString = "";


            }
            if (Category == "Välj en kategori" || Category == null) {
                items = items.Where(s => s.Title.Contains(searchString)).OrderByDescending(s => s.BlogEntryTime);
            }
            else if (Category != "Välj en kategori") {
                items = from item in items
                        where item.Title.Contains(searchString)
                        where item.Category == Category
                        orderby item.BlogEntryTime descending
                        select item;
            }





            var profileList = Ctx.Users.ToList();

            var BlogEntries = items.ToList();
            if (blockedCategories.Count > 0) {
                foreach (var bc in blockedCategories) {
                    foreach (var i in items) {
                        if (bc.CategoryName == i.Category && bc.CategoryType == "Formal") {
                            BlogEntries.Remove(i);
                        }
                    }
                }
            }
            List<FormalBlogItem> FormalBlogItemList = new List<FormalBlogItem>();


            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentUserId = currentUser.Id;
            var CurrentUserAdmin = currentUser.Admin;

            foreach (var item in BlogEntries) {
                user = await UserManager.FindByIdAsync(item.CreatorId);
                bool CanDelete = false;
                if (currentUserId.Equals(item.CreatorId) || CurrentUserAdmin) {
                    CanDelete = true;
                }
                string CreatorMail = user.Email;
                if (user.IsInactive) {
                    CreatorMail = "Inaktiverad användare";
                }
                var comments = Ctx.BlogComments.Where(b => b.BlogId == item.Id);
                var blogItem = new FormalBlogItem {
                    Id = item.Id,
                    CreatorId = item.CreatorId,
                    CreatorFirstName = user.FirstName,
                    CreatorLastName = user.LastName,
                    AttachedFile = item.AttachedFile,
                    Comments = comments.Count(),
                    Date = item.BlogEntryTime,
                    Content = item.Content,
                    Category = item.Category,
                    Title = item.Title,
                    CanDelete = CanDelete,
                    CreaterMail = CreatorMail
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
            string subject = "Nytt inlägg från " + user.FirstName + ".";
            string emailText = "Inlägg med rubrik: " + model.Title + " finns nu att läsa.";
            var emailHelper = new EmailHelper("orukommunikation@gmail.com", "Kakan1210");
            emailHelper.SendEmailFormalBlog(subject, emailText, user.Id, Category, "Formal");
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
                if(Filestring != null)
                {
                    entry.AttachedFile = Filestring;
                }
                else
                {
                    entry.AttachedFile = model.AttachedFile;
                }
                
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
            FormalBlogEntry blogEntry = Ctx.FormalBlogEntries.Find(EntryId);

            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentUserId = currentUser.Id;

            // currentUserId.Equals(CreatorId)
            Ctx.FormalBlogEntries.Remove(blogEntry);
            Ctx.SaveChanges();

            return RedirectToAction("Index", "FormalBlog");
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


        public async Task<ActionResult> ShowComments(int BlogId) {
            var BlogEntry = Ctx.FormalBlogEntries.FirstOrDefault(b => b.Id == BlogId);
            if (BlogEntry != null) {
                bool canDelete = false;
                var currentUser = UserManager.FindById(User.Identity.GetUserId());
                bool isAdmin = currentUser.Admin;
                var CommentList = Ctx.BlogComments.Where(c => c.BlogId == BlogId).OrderByDescending(c => c.BlogId);
                var BloggUser = await UserManager.FindByIdAsync(BlogEntry.CreatorId);
                List<Comment> Comments = new List<Comment>();
                foreach (var c in CommentList) {

                    if (isAdmin || currentUser.Id == c.CreatorId || BlogEntry.CreatorId == currentUser.Id) {
                        canDelete = true;
                    }
                    else {
                        canDelete = false;
                    }
                    var User = await UserManager.FindByIdAsync(c.CreatorId);
                    string CreaterMail = User.Email;
                    if (User.IsInactive) {
                        CreaterMail = "Inaktiverad användare";
                    }

                    var CommentItem = new Comment {
                        Content = c.Content,
                        Time = c.Time,
                        Email = CreaterMail,
                        FirstName = User.FirstName,
                        LastName = User.LastName,
                        CanDelete = canDelete,
                        Id = c.Id
                    };
                    Comments.Add(CommentItem);
                }
                string CreatorMail = BloggUser.Email;
                if (BloggUser.IsInactive) {
                    CreatorMail = "Inaktiverad användare";
                }
                return View(new FormalBlogCommentsViewModel {
                    AttachedFile = BlogEntry.AttachedFile,
                    BlogId = BlogEntry.Id,
                    Category = BlogEntry.Category,
                    Comments = Comments,
                    Content = BlogEntry.Content,
                    Date = BlogEntry.BlogEntryTime,
                    Title = BlogEntry.Title,
                    CreaterMail = CreatorMail,
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

            return RedirectToAction("ShowComments", new { newComment.BlogId });
        }

        public ActionResult _SearchAndFilterPartial() {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var CategoryList = Ctx.Categories.Where(c => c.CategoryType == "Formal").ToList();
            var blockedCategories = (from bc in Ctx.BlockedCategories
                                     where bc.CategoryType == "Formal"
                                     && bc.UserId == user.Id
                                     select bc).ToList();
            List<string> CategoryListName = new List<string>();
            foreach (var c in CategoryList) {
                CategoryListName.Add(c.CategoryName);
            }
            foreach (var bc in blockedCategories) {
                CategoryListName.Remove(bc.CategoryName);
            }
            CategoryListName.Add("Välj en kategori");
            CategoryListName.Reverse();
            var Id = User.Identity.GetUserId();
            return View(new SearchViewModel {
                CategoryList = CategoryListName
            });
        }

        [HttpPost]
        public ActionResult _SearchAndFilterPartial(SearchViewModel model) {
            return RedirectToAction("Index", new { model.SearchString, model.Category });
        }


        public ActionResult DeleteComment(int EntryId, int BlogId) {
            FormalBlogCommentsModel blogComment = Ctx.BlogComments.Find(EntryId);

            Ctx.BlogComments.Remove(blogComment);
            Ctx.SaveChanges();

            return RedirectToAction("ShowComments", new { BlogId });
        }

    }
}