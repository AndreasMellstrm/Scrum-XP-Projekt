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
        [Required]
        [Display(Name ="Bjud in användare")]
        public List<String> ListToSend { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Ditt {0} måste vara minst {2} tecken.", MinimumLength = 6)]
        [Display(Name = "Ämne")]
        public string Title { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Ditt {0} måste vara minst {2} tecken.", MinimumLength = 6)]
        [Display(Name = "Beskrivning")]
        public string Content { get; set; }
        [Display(Name = "Förslag nummer ett")]
        [Required]
        public string Suggestion1 { get; set; }
        public string Suggestion2 { get; set; }
        public string Suggestion3 { get; set; }
        public string Suggestion4 { get; set; }
        public string ErrorMessage { get; set; }

    }
    public class ShowTempEventViewModel
    {
        public int Id { get; set; }
        public int TempEventId { get; set; }
        public int SId1 { get; set; }
        public int SId2 { get; set; }
        public int SId3 { get; set; }
        public int SId4 { get; set; }
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
    public class ShowResultEventViewModel {
        public string Title { get; set; }
        public string Description { get; set; }
        public string S1Name { get; set; }
        public string S2Name { get; set; }
        public string S3Name { get; set; }
        public string S4Name { get; set; }
        public decimal S1Result { get; set; }
        public decimal S2Result { get; set; }
        public decimal S3Result { get; set; }
        public decimal S4Result { get; set; }
        public decimal S1Procent { get; set; }
        public decimal S2Procent { get; set; }
        public decimal S3Procent { get; set; }
        public decimal S4Procent { get; set; }
        public int EventId { get; set; }
        public decimal Votes { get; set; }
        public decimal Invites { get; set; }
    }
    public class ListTempEventViewModel {
        public List<TempEventModel> TempEventList { get; set; }
    }
}