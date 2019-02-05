using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class ResearchBlogModel {
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
        public int ProjectId { get; set; }
        public string AttachedFile { get; set; }
    }
}