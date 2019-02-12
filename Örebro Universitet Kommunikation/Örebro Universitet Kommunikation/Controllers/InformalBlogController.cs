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

                var blogItem = new InformalBlogItem {
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

                InformalBlogItemList.Add(blogItem);

            };

            return View(new InformalBlogViewModel {
                InformalBlogItems = InformalBlogItemList

            });
        }
        public ActionResult CreateInformalEntry()
        {
            var CategoryList = Ctx.Categories.Where(c => c.CategoryType == "Formal").ToList();
            List<string> CategoryListName = new List<string>();
            foreach (var c in CategoryList)
            {
                CategoryListName.Add(c.CategoryName);
            }
            var Id = User.Identity.GetUserId();
            return View(new CreateEntryViewModel {
                CategoryList = CategoryListName
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInformalEntry(CreateEntryViewModel model, string Category)
        {

            var user = UserManager.FindById(User.Identity.GetUserId());
            //var fileString = FileUpload(File);
            Ctx.InformalBlogEntries.Add(new InformalBlogModel {
                AttachedFile = " ",
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
    }
}
