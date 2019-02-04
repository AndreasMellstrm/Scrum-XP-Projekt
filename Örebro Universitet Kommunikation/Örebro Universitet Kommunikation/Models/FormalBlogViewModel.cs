using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Örebro_Universitet_Kommunikation.Models {
    public class FormalBlogViewModel {

        public List<FormalBlogEntry> FormalBlogEntries { get; set; }
        public List<FormalBlogItem> FormalBlogItems { get; set; }
        public List<String> CategoryItems { get; set; }
        public FormalBlogEditItem FormalEdit { get; set; }
    }
    public class FormalBlogItem {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public string CreaterMail { get; set; }
        public string AttachedFile { get; set; }
        public int Comments { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public bool CanDelete { get; set; }
    }

    public class FormalBlogEditItem {
        [Key]
        public int Id { get; set; }
        public string CreatorId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Din {0} måste vara minst {2} tecken.", MinimumLength = 6)]
        [Display(Name = "Titel")]
        public string Title { get; set; }
        [Display(Name = "Inlägg")]
        public string Content { get; set; }
        public DateTime BlogEntryTime { get; set; }
        public string Category { get; set; }
        public string AttachedFile { get; set; }
    }

    public class EditEntryViewModel {

        public int Id { get; set; }
        [Display(Name = "Titel")]
        public string Title { get; set; }
        [Display(Name = "Inlägg")]
        public string Content { get; set; }
        public string Category { get; set; }
        public string AttachedFile { get; set; }
        public List<String> CategoryItems { get; set; }
    }
}