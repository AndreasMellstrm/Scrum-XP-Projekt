using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Örebro_Universitet_Kommunikation.Models {
    public class IndexViewModel {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel {
        [Required]
        [StringLength(100, ErrorMessage = "Ditt {0} måste vara minst {2} tecken.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta nytt lösenord")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nuvarande lösenord")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Ditt {0} måste vara minst {2} tecken.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta nytt lösenord")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeNotificationsViewModel {

        [Required]
        [Display(Name = "Notifikationer")]
        public string Notifications { get; set; }
    }

    public class AddPhoneNumberViewModel {
        [Required]
        [Phone]
        [Display(Name = "Telefonnummer")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    public class BlockCategoriesViewModel {

        public List<CategoryModel> CategoriesFormal { get; set; }
        public List<CategoryModel> BlockedCategoriesFormal { get; set; }
        public List<CategoryModel> CategoriesInformal { get; set; }
        public List<CategoryModel> BlockedCategoriesInformal { get; set; }
        public List<SelectListItem> CategoryTypes { get; set; }
        public ApplicationUser User { get; set; }
        public CategoryModel Category { get; set; }
        public string CategoryType { get; set; }

    }
}