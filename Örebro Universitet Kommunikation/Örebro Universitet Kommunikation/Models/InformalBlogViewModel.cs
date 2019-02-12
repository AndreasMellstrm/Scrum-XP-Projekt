using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Örebro_Universitet_Kommunikation.Models.InformalBlogViewModel;

namespace Örebro_Universitet_Kommunikation.Models
{
    public class InformalBlogViewModel
    {
        public List<InformalBlogModel> InformalBlogEntries { get; set; }
        public List<InformalBlogItem> InformalBlogItems { get; set; }
        //public List<String> CategoryItems { get; set; }
        //public InformalBlogEditItem InformalEdit { get; set; }

    }
        public class InformalBlogItem
        {
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
    }
