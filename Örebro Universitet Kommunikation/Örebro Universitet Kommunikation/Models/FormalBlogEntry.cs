using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Örebro_Universitet_Kommunikation.Models {
    public class FormalBlogEntry {
        [Key]
        public int Id { get; set; }
        public string CreatorId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Din {0} måste vara minst {2} tecken.", MinimumLength = 3)]
        [Display(Name = "Titel")]
        public string Title { get; set; }
        [Display(Name = "Inlägg")]
        public string Content { get; set; }
        public DateTime BlogEntryTime { get; set; }
        public string Category { get; set; }
        public string AttachedFile { get; set; }
    }
}