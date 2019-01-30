using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Örebro_Universitet_Kommunikation.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Controllers {
    public class FormalBlogController : Controller {

        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }


        public FormalBlogController(){
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }

        public ActionResult Index() {

            Ctx = new ApplicationDbContext();

            var BlogEntries = (from BE in Ctx.FormalBlogEntreis
                               select BE).ToList();


            return View(new FormalBlogViewModel {

                FormalBlogEntries = BlogEntries

            });
        }
        public ActionResult CreateEntry() {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEntry(FormalBlogEntry model, HttpPostedFileBase File) {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var fileString = FileUpload(File);
            Ctx.FormalBlogEntreis.Add(new FormalBlogEntry {
                AttachedFile = fileString,
                Category = null,
                BlogEntryTime = DateTime.Now,
                Title = model.Title,
                Content = model.Content,
                Creator = user
            }
            );
            Ctx.SaveChanges();

            return View();
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
        
    }
}