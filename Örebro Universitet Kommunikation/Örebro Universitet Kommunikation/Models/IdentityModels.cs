using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int ProjectId { get; set; }
        public virtual ICollection<ApplicationUserCalendarEvents> EventRelationships { get; set; }





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
        public DbSet<EducationBlogModel> EducationBlogs { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<EducationBlogCommentsModel> EducationBlogComments { get; set; }
        public DbSet<ResearchBlogCommentsModel> ResearchBlogComments { get; set; }
        public DbSet<TempEventModel> TempEvents { get; set; }
        public DbSet<TempEventSuggestionModel> TempEventSuggestions { get; set; }
        public DbSet<TempEventTimeModel> TempEventTimes { get; set; }
        public DbSet<TempEventUserModel> TempEventUsers { get; set; }
        public DbSet<ApplicationUserCalendarEvents> ApplicationUserCalendarEvents { get; set; }
        public DbSet<InformalBlogModel> InformalBlogEntries { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) {
            //Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;




        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUserCalendarEvents>().HasKey(c => new { c.EventId, c.UserId });

            modelBuilder.Entity<ApplicationUser>()
               .HasMany(c => c.EventRelationships)
               .WithRequired(cc => cc.User)
               .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<CalendarEvent>()
               .HasMany(c => c.EventRelationships)
               .WithRequired(cc => cc.Event)
               .HasForeignKey(c => c.EventId);
        }

            public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }
    }
}