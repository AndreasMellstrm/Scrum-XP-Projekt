﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class CalendarEvent {
        [Key]
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }
        public string CreatorId { get; set; }
        public virtual ICollection<ApplicationUserCalendarEvents> EventRelationships { get; set; }

    }
    public class CreateEventViewModel {
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class CalenderItemViewModel {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }
        public string CreatorName { get; set; }
        public bool CanDelete { get; set; }
        public string[] Users { get; set; }
    }


}