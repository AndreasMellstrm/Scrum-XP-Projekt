using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class ResearchBlogViewModel {
        public List<ResearchBlogItem> ResearchBlogList { get; set; }
        public string ResearchName { get; set; }
        public bool CanCreateEntry { get; set; }
    }
    public class ResearchBlogItem {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public string CreaterMail { get; set; }
        public string AttachedFile { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool CanDelete { get; set; }
        public int ResearchComments { get; set; }

    }
}