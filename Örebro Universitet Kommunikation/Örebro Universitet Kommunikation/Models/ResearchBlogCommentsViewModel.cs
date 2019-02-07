using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models
{
    public class ResearchBlogCommentsViewModel
    {
        public List<ResearchComment> Comments { get; set; }
        public int BlogId { get; set; }
        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public string CreatorMail { get; set; }
        public string AttachedFile { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [Required(ErrorMessage = "Var vänlig ange en kommentar")]
        [StringLength(200, ErrorMessage = "Din {0} måste vara minst {2} tecken.", MinimumLength = 3)]
        public string CommentContent { get; set; }
        public string ProjectName { get; set; }
        public int ProjectId { get; set; }

    }
    public class ResearchComment
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public DateTime Time { get; set; }

    }
}