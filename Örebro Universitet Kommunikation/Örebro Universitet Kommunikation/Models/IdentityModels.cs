using System.ComponentModel;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Örebro_Universitet_Kommunikation.Models {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser {

        [DefaultValue(false)]
        public bool Admin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        [DefaultValue("None")]
        public string Notifications { get; set; }
        public virtual ProjectModel Project { get; set; }
        


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public DbSet<FormalBlogEntry> FormalBlogEntries { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<FormalBlogCommentsModel> BlogComments { get; set; }
        public DbSet<ResearchBlogModel> ResearchBlogs { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<CalendarViewModel> Calendar { get; set; }
        public DbSet<EducationBlogModel> EducationBlogs { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) {


            
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }
    }
}