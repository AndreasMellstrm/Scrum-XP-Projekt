using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class TempEventUserModel {
        [Key]
        public int Id { get; set; }
        public int TempEventId { get; set; }
        public string UserId { get; set; }
    }
}