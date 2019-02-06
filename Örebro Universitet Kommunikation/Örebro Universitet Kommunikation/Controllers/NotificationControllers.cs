using Örebro_Universitet_Kommunikation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Örebro_Universitet_Kommunikation.Controllers
{
    public class EmailController
    {

        public ApplicationDbContext Ctx { get; set; }

        ApplicationUser[] EmailRecipients = new ApplicationUser[] { };

        public EmailController() {
            Ctx = new ApplicationDbContext();
            EmailRecipients = (from U in Ctx.Users
                               where U.Notifications == "Blog"
                               || U.Notifications == "Event"
                               || U.Notifications == "BlogEvent"
                               select U).ToArray();
        }
        
    }
}
