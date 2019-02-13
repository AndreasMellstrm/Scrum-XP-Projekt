using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Örebro_Universitet_Kommunikation.Helpers;
using Örebro_Universitet_Kommunikation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Controllers
{
    public class SummaryController : Controller{
        ApplicationDbContext Ctx = new ApplicationDbContext();
        public UserManager<ApplicationUser> UserManager { get; set; }

        public SummaryController() {

            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
            Ctx = new ApplicationDbContext();
        }


        public ActionResult Index(){

            var calendarEvents = Ctx.CalendarEvents;
            var formalBlogEntries = Ctx.FormalBlogEntries;
            return View();
        }

        public ActionResult SendSummary() {
            List<CalendarEvent> MyEventsList = new List<CalendarEvent>();


            var sevenDaysAgo = DateTime.Now.AddDays(-7);
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var CalendarEvents = Ctx.ApplicationUserCalendarEvents.Where(c => c.CanCome == true && c.UserId == currentUser.Id);
            var CreatedEvents = Ctx.CalendarEvents.Where(u => u.CreatorId == currentUser.Id);

            foreach (var item in CalendarEvents) {
                var events = Ctx.CalendarEvents.Find(item.EventId);
                if(events.Start >= sevenDaysAgo && events.Start <= DateTime.Now) {
                    MyEventsList.Add(events);
                }
            }

            foreach(var item in CreatedEvents) {
                if (item.Start >= sevenDaysAgo && item.Start <= DateTime.Now) {
                    MyEventsList.Add(item);
                }
            }


            
            var FormalBlogEntries = Ctx.FormalBlogEntries.Where(f => f.BlogEntryTime >= sevenDaysAgo);
            string EventItem = "Summering av veckan: \n";
            EventItem = EventItem + "Formella bloginlägg de senaste 7 dagarna: \n";
            foreach (var item in FormalBlogEntries) {
                EventItem = EventItem + item.Title + "\n" + "Beskrivning: " + item.Content + "\n" + "Datum & tid: " + item.BlogEntryTime + "\n";
            }
            EventItem = EventItem + " \n Kalenderinlägg de senaste 7 dagarna: \n";
            foreach(var item in MyEventsList) {
                EventItem = EventItem + " \n" + item.Title + "\n" + "Beskrivning: " + item.Desc + "\n" + "Start: " + item.Start + "\n" + "Slut: " + item.End + "\n";
            }
           
            

            
            






            var emailHelper = new EmailHelper("orukommunikation@gmail.com", "Kakan1210");
            emailHelper.SendEmail(currentUser.Email.ToString(), "Summering", EventItem);
            return RedirectToAction("Index", "manage");
        }
    }
}