using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Örebro_Universitet_Kommunikation.Models {
    public class FormalBlogEntry {

        [Key]
        public int EntryId { get; set; }
        public int CreatorUserId { get; set; }
        public string CreatorName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime BlogEntryTime { get; set; }
        public string Category { get; set; }
        public string AttachedFile { get; set; }

    }
}