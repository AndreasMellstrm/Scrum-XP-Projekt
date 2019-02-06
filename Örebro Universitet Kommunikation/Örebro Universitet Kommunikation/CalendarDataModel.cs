namespace Örebro_Universitet_Kommunikation {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CalendarDataModel : DbContext {
        public CalendarDataModel()
            : base("name=CalendarDataModel") {
        }

        public virtual DbSet<CalendarEvents> CalendarEvents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
        }
    }
}
