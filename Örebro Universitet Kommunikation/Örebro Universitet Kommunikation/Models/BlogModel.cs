using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class BlogModel {
        public int CreatorUserId { get; set; }
        public int EntryId { get; set; }
        public string CreatorName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime BlogEntryTime { get; set; }
        public string Category { get; set; }
        public string AttachedFile { get; set; }

        public BlogModel(int CreatorUserId, int EntryId, string CreatorName, string Title, string Content, DateTime BlogEntryTime, string Category, string AttachedFIle) {
            this.CreatorUserId = CreatorUserId;
            this.EntryId = EntryId;
            this.CreatorName = CreatorName;
            this.Title = Title;
            this.Content = Content;
            this.BlogEntryTime = BlogEntryTime;
            this.Category = Category;
            this.AttachedFile = AttachedFile;

        }



    }
}