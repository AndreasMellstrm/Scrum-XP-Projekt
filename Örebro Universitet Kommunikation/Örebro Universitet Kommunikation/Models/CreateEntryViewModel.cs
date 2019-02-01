using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Models {
    public class CreateEntryViewModel {
        public List<String> CategoryList { get; set; }
        public List<ApplicationUser> EmailRecipients { get; set; }
        public int Id { get; set; }
        public string CreatorId{ get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Din {0} måste vara minst {2} tecken.", MinimumLength = 6)]
        [Display(Name = "Titel")]
        public string Title { get; set; }
        [Display(Name = "Inlägg")]
        public string Content { get; set; }
        public string Category { get; set; }

        public CreateEntryViewModel() {
           var Ctx = new ApplicationDbContext();
            string uID = CreatorId;
            EmailRecipients = (from U in Ctx.Users
                               where U.Notifications == "Email"
                               || U.Notifications == "EmailSms"
                               where U.Id != uID
                               select U).ToList();
        }
    }
}