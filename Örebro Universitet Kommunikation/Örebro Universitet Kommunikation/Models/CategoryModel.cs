using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Models {
    public class CategoryModel {
        [Key, Column(Order = 0)]
        public string CategoryName { get; set; }
        [Key, Column(Order = 1)]
        public string CategoryType { get; set; }
    }
}