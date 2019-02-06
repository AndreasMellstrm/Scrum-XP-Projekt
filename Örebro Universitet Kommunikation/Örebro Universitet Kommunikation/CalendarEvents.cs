namespace Örebro_Universitet_Kommunikation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CalendarEvents
    {
        [Key]
        public int EventId { get; set; }

        public string Title { get; set; }

        public string Desc { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string ThemeColor { get; set; }

        public bool IsFullDay { get; set; }
    }
}
