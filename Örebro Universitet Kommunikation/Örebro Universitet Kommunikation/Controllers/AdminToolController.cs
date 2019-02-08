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

        public List<ProjectModel> GetAllProjects() {
            List<ProjectModel> ProjectList = Ctx.Projects.ToList();
            return ProjectList;
        }

        public List<ApplicationUser> GetAllUsers() {
            List<ApplicationUser> UserList = Ctx.Users.ToList();
            return UserList;
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
                    PhoneNumber = model.PhoneNumber,
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
            return View(new AsignUserToProjectViewModel{
                UserList = GetAllUsers(),
                ProjectList = GetAllProjects(),
                ErrorMessage = ""
                    });
        }

        [HttpPost]
        public async Task<ActionResult> AsignUserToProject(AsignUserToProjectViewModel model) {
            var user = UserManager.FindById(model.UserId);          
            var projects = (from p in Ctx.Projects
                            where p.ProjectId == model.ProjectId
                            select p).ToList();
            user.Project = projects[0];
            var result = await Ctx.SaveChangesAsync();
            if(result > 0) {
                return View(new AsignUserToProjectViewModel {
                    ProjectList = GetAllProjects(),
                    UserList = GetAllUsers(),
                    ErrorMessage = "Användaren blev tilldelad projektet " + projects[0].ProjectName
                });
            }
            return View(new AsignUserToProjectViewModel {
                ProjectList = GetAllProjects(),
                UserList = GetAllUsers(),
                ErrorMessage = "Användaren kunde inte tilldelas projektet " + projects[0].ProjectName
            });
        }

        public ActionResult EditUser() {
            return View(new EditUserViewModel {
                UserList = GetAllUsers(),
                UserId = User.Identity.GetUserId()
            });
        }

        public ActionResult _EditUserPartial(string userId) {
            var user = UserManager.FindById(userId);
            return PartialView(new _EditUserPartialViewModel {
                userId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Position = user.Position,
                ErrorMessage = ""
            });
        }

        public async Task<ActionResult> SaveUserChanges(_EditUserPartialViewModel model) {
            if (model.Password == null || model.Password == "") {
                var user = UserManager.FindById(model.userId);
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.Position = model.Position;
                var ErrorMessage = "";
            }
            else {
                var user = UserManager.FindById(model.userId);
                await UserManager.ResetPasswordAsync(user.Id, await UserManager.GeneratePasswordResetTokenAsync(user.Id), model.Password);
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.Position = model.Position;
                var ErrorMessage = "";
            }
            var result = await Ctx.SaveChangesAsync();
            if (result > 0) {
                return RedirectToAction("EditUser","AdminTool", new _EditUserPartialViewModel {
                    ErrorMessage = "Användaren har redigerats"
                });
            }
            return RedirectToAction("EditUser", "AdminTool", new _EditUserPartialViewModel {
                ErrorMessage = "Användaren har redigerats"
            });
        }
    }
}