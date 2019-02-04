using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models
{
    public class FormalBlogCommentsViewModel
    {
        public List<Comment> Comments { get; set; }
        public int BlogId { get; set; }
        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public string CreaterMail { get; set; }
        public string AttachedFile { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string CommentContent { get; set; }
    }
    public class Comment
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