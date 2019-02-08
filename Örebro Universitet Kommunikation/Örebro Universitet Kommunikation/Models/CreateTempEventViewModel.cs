using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Models {
    public class CreateTempEventViewModel {
        [Display(Name = "Bjud in användare")]
        public List<SelectListItem> NewList { get; set; }
        public List<String> ListToSend { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Din {0} måste vara minst {2} tecken.", MinimumLength = 6)]
        [Display(Name = "Ämne")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Beskrivning")]
        public string Content { get; set; }
        [Display(Name = "Förslag")]
        [Required]
        public string Suggestion1 { get; set; }
        public string Suggestion2 { get; set; }
        public string Suggestion3 { get; set; }
        public string Suggestion4 { get; set; }

    }
    public class ShowTempEventViewModel
    {
        public string Creator { get; set; }
        [Display(Name = "Ämne")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Beskrivning")]
        public string Content { get; set; }
        [Display(Name = "Förslag")]
        [Required]
        public string Suggestion1 { get; set; }
        public string Suggestion2 { get; set; }
        public string Suggestion3 { get; set; }
        public string Suggestion4 { get; set; }
        public bool s1 { get; set; }
        public bool s2 { get; set; }
        public bool s3 { get; set; }
        public bool s4 { get; set; }
        public bool s5 { get; set; }

    }
    }