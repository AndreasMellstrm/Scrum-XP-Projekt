using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Models {
    public class CreateTempEventViewModel {
        public List<SelectListItem> NewList { get; set; }
        public List<String> ListToSend { get; set; }
        [Display(Name = "Ämne")]
        public string Title { get; set; }
        [Display(Name = "Beskrivning")]
        public string Content { get; set; }
        [Display(Name = "Förslag")]
        [Required]
        public string Suggestion1 { get; set; }
        public string Suggestion2 { get; set; }
        public string Suggestion3 { get; set; }
        public string Suggestion4 { get; set; }

    }
}