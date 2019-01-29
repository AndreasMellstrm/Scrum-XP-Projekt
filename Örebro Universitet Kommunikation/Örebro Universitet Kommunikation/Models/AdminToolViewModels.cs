using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
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
    }
}