using Örebro_Universitet_Kommunikation.Models;
using System.Linq;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Controllers {
    public class FormalBlogController : Controller {

        public ApplicationDbContext Ctx { get; set; }


        public FormalBlogController(){
            Ctx = new ApplicationDbContext();

        }

        public ActionResult Index() {

            Ctx = new ApplicationDbContext();

            var BlogEntries = (from BE in Ctx.FormalBlogEntreis
                               select BE).ToList();


            return View(new FormalBlogViewModel {

                FormalBlogEntries = BlogEntries

            });
        }

        
    }
}