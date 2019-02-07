using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class TempEventSuggestionModel {
        [Key]
        public int Id { get; set; }
        public int TempEvenId { get; set; }
        public string Suggestion { get; set; }
    }
}