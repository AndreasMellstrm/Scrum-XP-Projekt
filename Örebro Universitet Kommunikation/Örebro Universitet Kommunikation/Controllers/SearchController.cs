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
    public class SearchController : Controller {
        
        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }

        public SearchController() {
            Ctx = new ApplicationDbContext();
        }

        // GET: Search
        public async Task<ActionResult> Index(String searchString) {

            Ctx = new ApplicationDbContext();




            var items = from m in Ctx.FormalBlogEntries select m;
            if (!String.IsNullOrEmpty(searchString)) {
                items = items.Where(s => s.Title.Contains(searchString));
            }



            var profileList = Ctx.Users.ToList();


            var BlogEntries = (from BE in Ctx.FormalBlogEntries
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


            return View(new SearchViewModel {
                FormalBlogItems = FormalBlogItemList
            });
        }
    }
}