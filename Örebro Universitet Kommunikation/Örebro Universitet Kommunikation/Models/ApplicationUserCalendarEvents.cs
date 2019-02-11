using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class ApplicationUserCalendarEvents {

        [Key, Column(Order = 0)]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        public int EventId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("EventId")]
        public virtual CalendarEvent Event { get; set; }

        public bool CanCome { get; set; }

    }
}