using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class SearchViewModel {
        public List<FormalBlogEntry> FormalEntriesList;
        public List<FormalBlogItem> FormalBlogItems;
        public string SearchString { get; set; }
        public string Category { get; set; }
        public List<String> CategoryList { get; set; }
        
    }
}   