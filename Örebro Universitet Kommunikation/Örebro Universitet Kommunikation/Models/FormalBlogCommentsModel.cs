using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models
{
    public class FormalBlogCommentsModel
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public string CreatorId { get; set; }
        public int BlogId { get; set; }
        public DateTime Time { get; set; }
    }
}