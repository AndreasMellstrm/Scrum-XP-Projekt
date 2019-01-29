using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class CategoryModel {
        [Key]
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryType { get; set; }
    }
    public enum CategoryTypes {
        
    }
}