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
    public class InformalBlogController : Controller
    {
        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }

        public InformalBlogController()
        {
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }
        // GET: InformalBlog
        public async Task <ActionResult> Index(string searchString, string Category)
        {
            Ctx = new ApplicationDbContext();

            var items = from m in Ctx.InformalBlogEntries orderby m.BlogEntryTime descending select m;


            
            if (String.IsNullOrEmpty(searchString))
            {
                searchString = "";


            }
            if (Category == "Välj en kategori" || Category == null)
            {
                items = items.Where(s => s.Title.Contains(searchString)).OrderByDescending(s => s.BlogEntryTime);
            }
            else if (Category != "Välj en kategori")
            {
                items = from item in items
                        where item.Title.Contains(searchString)
                        where item.Category == Category
                        orderby item.BlogEntryTime descending
                        select item;
            }

    


            var profileList = Ctx.Users.ToList();

            var BlogEntries = items;


            List<InformalBlogItem> InformalBlogItemList = new List<InformalBlogItem>();


            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentUserId = currentUser.Id;
            var CurrentUserAdmin = currentUser.Admin;

            foreach (var item in BlogEntries)
            {
                var user = await UserManager.FindByIdAsync(item.CreatorId);
                bool CanDelete = false;
                if (currentUserId.Equals(item.CreatorId) || CurrentUserAdmin)
                {
                    CanDelete = true;
                }
                var comments = Ctx.InformalBlogComments.Where(b => b.BlogId == item.Id);
                var blogItem = new InformalBlogItem {
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
                    CanDelete = CanDelete
                };

                InformalBlogItemList.Add(blogItem);

            };

            return View(new InformalBlogViewModel {
                InformalBlogItems = InformalBlogItemList

            });
        }
        public ActionResult CreateInformalEntry()
        {
            var CategoryList = Ctx.Categories.Where(c => c.CategoryType == "Informal").ToList();
            List<string> CategoryListName = new List<string>();
            foreach (var c in CategoryList)
            {
                CategoryListName.Add(c.CategoryName);
            }
            var Id = User.Identity.GetUserId();
            return View(new CreateEntryViewModel {
                CategoryList = CategoryListName,
                ErrorMessage = ""
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInformalEntry(CreateEntryViewModel model, HttpPostedFileBase File, string Category)
        {

            var user = UserManager.FindById(User.Identity.GetUserId());
            string fileString = null;
            if (File != null && File.ContentLength > 0) { 
                fileString = FileUpload(File);
                if(fileString == null)
                {
                    var CategoryList = Ctx.Categories.Where(c => c.CategoryType == "Formal").ToList();
                    List<string> CategoryListName = new List<string>();
                    foreach (var c in CategoryList)
                    {
                        CategoryListName.Add(c.CategoryName);
                    }
                    return View(new CreateEntryViewModel
                    {
                        Title = model.Title,
                        Content = model.Content,
                        ErrorMessage = "Endast bildfiler tillåtna.",
                        CategoryList = CategoryListName

                    });
                }
            }
            Ctx.InformalBlogEntries.Add(new InformalBlogModel {
                AttachedFile = fileString,
                Category = Category,
                BlogEntryTime = DateTime.Now,
                Title = model.Title,
                Content = model.Content,
                CreatorId = user.Id
            }
            );
            Ctx.SaveChanges();
            //string subject = "Nytt inlägg från " + user.FirstName + ".";
            //string emailText = "Inlägg med rubrik: " + model.Title + " finns nu att läsa.";
            //var emailHelper = new EmailHelper("orukommunikation@gmail.com", "Kakan1210");
            //emailHelper.SendEmailFormalBlog(subject, emailText, user.Id);
            return RedirectToAction("Index", "InformalBlog");
        }
        [HttpGet]
        public ActionResult EditInformalEntry(int EntryId) {
            var BlogEntry = Ctx.InformalBlogEntries.FirstOrDefault(b => b.Id == EntryId);
            var CategoryList = Ctx.Categories.Where(c => c.CategoryType == "Informal").ToList();
            List<string> CategoryListName = new List<string>();
            foreach (var c in CategoryList) {

                CategoryListName.Add(c.CategoryName);
            }
            var InformalBlogItem = new EditInformalEntryViewModel() {
                Id = BlogEntry.Id,
                AttachedFile = "",      //Kanske inte funkar yao
                Category = BlogEntry.Category,
                Content = BlogEntry.Content,
                Title = BlogEntry.Title,
                CategoryItems = CategoryListName

            };
            return View(InformalBlogItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInformal(EditInformalEntryViewModel model, int Id) {
            if (ModelState.IsValid) {
                var entry = Ctx.InformalBlogEntries.FirstOrDefault(b => b.Id == Id);
                //var Filestring = FileUpload(File);


                //entry.AttachedFile = Filestring;
                entry.Category = model.Category;


                entry.Content = model.Content;
                entry.Title = model.Title;

                Ctx.SaveChanges();
            }


            return RedirectToAction("Index", "InformalBlog");
        }


        public ActionResult DeleteInformalEntry(int EntryId, string CreatorId)
        {
            InformalBlogModel blogEntry = Ctx.InformalBlogEntries.Find(EntryId);

            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentUserId = currentUser.Id;

            // currentUserId.Equals(CreatorId)
            Ctx.InformalBlogEntries.Remove(blogEntry);
            Ctx.SaveChanges();

            return RedirectToAction("Index", "InformalBlog");
        }



            public ActionResult _SearchAndFilterInformalPartial()
        {
            var CategoryList = Ctx.Categories.Where(c => c.CategoryType == "Informal").ToList();
            List<string> CategoryListName = new List<string>();
            foreach (var c in CategoryList)
            {

                CategoryListName.Add(c.CategoryName);
            }
            CategoryListName.Add("Välj en kategori");
            CategoryListName.Reverse();
            var Id = User.Identity.GetUserId();
            return View(new SearchViewModel {
                CategoryList = CategoryListName
            });
        }

        [HttpPost]
        public ActionResult _SearchAndFilterInformalPartial(SearchViewModel model)
        {
            return RedirectToAction("Index", new { model.SearchString, model.Category });
        }

        public async Task<ActionResult> ShowInformalComments(int BlogId) {
            var BlogEntry = Ctx.InformalBlogEntries.FirstOrDefault(b => b.Id == BlogId);
            if (BlogEntry != null) {
                bool canDelete = false;
                var currentUser = UserManager.FindById(User.Identity.GetUserId());
                bool isAdmin = currentUser.Admin;
                var CommentList = Ctx.InformalBlogComments.Where(c => c.BlogId == BlogId).OrderByDescending(c => c.BlogId);
                var BloggUser = await UserManager.FindByIdAsync(BlogEntry.CreatorId);
                List<InformalComment> Comments = new List<InformalComment>();
                foreach (var c in CommentList) {

                    if (isAdmin || currentUser.Id == c.CreatorId || BlogEntry.CreatorId == currentUser.Id) {
                        canDelete = true;
                    } else {
                        canDelete = false;
                    }
                    var User = await UserManager.FindByIdAsync(c.CreatorId);
                    string CreaterMail = User.Email;
                    if (User.IsInactive) {
                        CreaterMail = "Inaktiverad användare";
                    }

                    var CommentItem = new InformalComment {
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
                return View(new InformalBlogCommentsViewModel {
                    AttachedFile = BlogEntry.AttachedFile,
                    BlogId = BlogEntry.Id,
                    Category = BlogEntry.Category,
                    InformalComments = Comments,
                    Content = BlogEntry.Content,
                    Date = BlogEntry.BlogEntryTime,
                    Title = BlogEntry.Title,
                    CreaterMail = CreatorMail,
                    CreatorFirstName = BloggUser.FirstName,
                    CreatorLastName = BloggUser.LastName

                });
            }
            return RedirectToAction("Index", "InformalBlog");
        }

        public ActionResult WriteInformalComment(InformalBlogCommentsViewModel newComment) {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            Ctx.InformalBlogComments.Add(new InformalBlogCommentsModel {
                BlogId = newComment.BlogId,
                Content = newComment.CommentContent,
                Time = DateTime.Now,
                CreatorId = currentUser.Id
            });
            Ctx.SaveChanges();

            return RedirectToAction("ShowInformalComments", new { newComment.BlogId });
        }

        public ActionResult DeleteInformalComment(int EntryId, int BlogId) {
            InformalBlogCommentsModel blogComment = Ctx.InformalBlogComments.Find(EntryId);

            Ctx.InformalBlogComments.Remove(blogComment);
            Ctx.SaveChanges();

            return RedirectToAction("ShowInformalComments", new { BlogId });
        }
        public string FileUpload(HttpPostedFileBase File)
        {
            //Vi kollar att det finns en fil att spara
            if (File != null && File.ContentLength > 0)
            {
                //Hämtar filnamnet utan filändelse
                var NoExtension = Path.GetFileNameWithoutExtension(File.FileName);
                //Hämtar filändelsen
                var Extension = Path.GetExtension(File.FileName).ToLower();
                
                if (Extension == ".jpg" || Extension == ".gif" || Extension == ".png" || Extension == ".jpeg") {
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
            else
            {
                return null;
            }
        }
    }
}
