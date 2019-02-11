using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class MeetingRequestViewModel {

        public List<CalendarEvent> MeetingRequests { get; set; }
        public ApplicationUser User { get; set; }
        public List<TempEventModel> TempEvents { get; set; }
    }
}