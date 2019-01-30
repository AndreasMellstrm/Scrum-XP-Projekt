using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Models {
    public class CategoryViewModel {
        public class CategoryList {
            public int Id { get; set; }
            public IEnumerable<SelectListItem> CategoryName { get; set; }
        }
    }
}