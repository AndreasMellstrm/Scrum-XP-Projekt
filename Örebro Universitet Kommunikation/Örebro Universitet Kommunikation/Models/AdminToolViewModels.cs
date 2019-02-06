using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Örebro_Universitet_Kommunikation.Models {

    public class AdminToolViewModel {
        public string Id { get; set; }
    }

    public class CreateUserViewModel {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Ditt {0} måste vara minst {2} tecken.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression("([a-öA-Ö0-9/s.&'-]+)", ErrorMessage = "Vänligen använd endast giltiga tecken")]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression("([a-öA-Ö0-9/s.&'-]+)", ErrorMessage = "Vänligen använd endast giltiga tecken")]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression("([a-öA-Ö0-9/s.&'-]+)", ErrorMessage = "Vänligen använd endast giltiga tecken")]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Administratör")]
        public bool Admin { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class CreateCategoryViewModel {

        [Required]
        [RegularExpression("([a-öA-Ö0-9/s.&'-]+)", ErrorMessage = "Vänligen använd endast giltiga tecken")]
        [Display(Name = "Namn")]
        public string CategoryName { get; set; }

        [Required]
        [Display(Name = "Typ")]
        public string CategoryType { get; set; }

        public string ErrorMessage { get; set; }

        public List<string> CategoryTypes { get; set; }

        public CreateCategoryViewModel() {

        }

        public CreateCategoryViewModel(string ErrorMessage = "") {
            CategoryTypes = new List<string> {
                "Formal",
                "Informal"
            };
            this.ErrorMessage = ErrorMessage;
        }
    }

    public class CreateProjectViewModel {

        [Required]
        [RegularExpression("([a-öA-Ö0-9/s.&'-]+)", ErrorMessage = "Vänligen använd endast giltiga tecken")]
        [Display(Name = "Namn")]
        public string ProjectName { get; set; }
        
        public string ErrorMessage { get; set; }

    }

    public class AsignUserToProjectViewModel {
        
        public List<ProjectModel> ProjectList { get; set; }
        public List<ApplicationUser> UserList { get; set; }
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public string ErrorMessage { get; set; }

    }
}