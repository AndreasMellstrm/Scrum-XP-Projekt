using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Örebro_Universitet_Kommunikation.Models;

namespace Örebro_Universitet_Kommunikation.Controllers {
    [Authorize]
    public class AdminToolController : Controller {
        public ApplicationDbContext Ctx { get; set; }

        public UserManager<ApplicationUser> UserManager { get; set; }

        public AdminToolController() {
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }

        public async Task<bool> IsAdmin(string userId) {
            var admins = (from a in Ctx.Users
                          where a.Admin == true
                          select a).ToList();
            var admin = await UserManager.FindByIdAsync(userId);
            if (admins.Contains(admin)) {
                return true;
            }
            return false;
        }
        // GET: AdminTool
        public async Task<ActionResult> Index(string Id) {
            if (await IsAdmin(Id)) {
                return View(new AdminToolViewModel {
                    Id = Id
                });
            }
            return RedirectToAction("Index", "FormalBlog");
        }

        public async Task<ActionResult> CreateUser(string Id) {
            if (await IsAdmin(Id)) {
                return View(new CreateUserViewModel {
                    ErrorMessage = ""
                });
            }
            return RedirectToAction("Index", "FormalBlog");
        }

        // POST: /AdminTool/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(CreateUserViewModel model) {
            if (ModelState.IsValid) {
                var user = new ApplicationUser {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Position = model.Position,
                    Admin = model.Admin
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded) {
                    ModelState.Clear();
                    return View(new CreateUserViewModel {
                        ErrorMessage = "Användare " + model.Email + " har skapats"
                    });
                }
            }
            return View(new CreateUserViewModel {
                ErrorMessage = "Skapandet av " + model.Email + " misslyckades."
            });

        }

        public async Task<ActionResult> CreateCategory(string Id) {
            if (await IsAdmin(Id)) {
                return View(new CreateCategoryViewModel(""));
            }
            return RedirectToAction("Index", "FormalBlog");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCategory(CreateCategoryViewModel model) {
            var Category = new CategoryModel {
                CategoryName = model.CategoryName,
                CategoryType = model.CategoryType,

            };
            try {
                Ctx.Categories.Add(Category);
                var result = await Ctx.SaveChangesAsync();
                if (result > 0) {
                    ModelState.Clear();
                    var ErrorMessageSuccess = "Kategorin " + model.CategoryName + " har skapats";
                    return View(new CreateCategoryViewModel(ErrorMessageSuccess));
                }
            }
            catch {

                var ErrorMessageFail = "Skapandet av kategorin: " + model.CategoryName + " misslyckades.";
                return View(new CreateCategoryViewModel(ErrorMessageFail));
            }
            return View();
        }

        public async Task<ActionResult> CreateProject(string Id) {
            if (await IsAdmin(Id)) {
                return View(new CreateProjectViewModel{
                    ErrorMessage = ""
                    });
            }
            return RedirectToAction("Index", "FormalBlog");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProject(CreateProjectViewModel model) {
            var Project = new ProjectModel {
                ProjectName = model.ProjectName
            };
            try {
                Ctx.Projects.Add(Project);
                var result = await Ctx.SaveChangesAsync();
                if (result > 0) {
                    ModelState.Clear();
                    var ErrorMessageSuccess = "Projektet " + model.ProjectName + " har skapats";
                    return View(new CreateProjectViewModel {
                        ErrorMessage = ErrorMessageSuccess
                    });
                }
            }
            catch {
                var ErrorMessageFail = "Skapandet av projektet: " + model.ProjectName + " misslyckades.";
                return View(new CreateProjectViewModel{
                    ErrorMessage = ErrorMessageFail
                });
            }
            return View();
        }

        public ActionResult AsignUserToProject() {
            List<ApplicationUser> UserList = (from u in Ctx.Users
                                              select u).ToList();
            List<ProjectModel> ProjectList = (from p in Ctx.Projects
                                              select p).ToList();
            return View(new AsignUserToProjectViewModel{
                UserList = UserList,
                ProjectList = ProjectList,
                ErrorMessage = ""
                    });
        }
    }
}