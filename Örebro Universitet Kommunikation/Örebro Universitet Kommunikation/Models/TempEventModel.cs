using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class TempEventModel {
        [Key]
        public int Id { get; set; }
        public string CreatorId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Din {0} måste vara minst {2} tecken.", MinimumLength = 6)]
        [Display(Name = "Titel")]
        public string Title { get; set; }
        public string Description { get; set; }
    }
}