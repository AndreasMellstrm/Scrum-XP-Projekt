﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Örebro_Universitet_Kommunikation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Örebro_Universitet_Kommunikation.Controllers
{

    public class CalendarController : Controller
    {
        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }

        // GET: 
        public CalendarController() {
        Ctx = new ApplicationDbContext();
        UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }

        


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetEvents() {
            using (ApplicationDbContext dc = new ApplicationDbContext()) {
                var events = dc.CalendarEvents.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        [HttpPost]
        public async Task<JsonResult> SaveEvent(CalendarEvent e) {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var currentUserId = currentUser.Id;
            var CurrentUserAdmin = currentUser.Admin;
            var events = Ctx.CalendarEvents.ToList();

            foreach (var item in events)
            {
                var user = await UserManager.FindByIdAsync(item.CreatorId);
                bool CanDelete = false;
                if (currentUserId.Equals(item.CreatorId) || CurrentUserAdmin)
                {
                    CanDelete = true;
                }
            }



            var status = false;

            using (ApplicationDbContext dc = new ApplicationDbContext()) {
                if (e.EventId > 0) {
                    //Update the event
                    var v = dc.CalendarEvents.Where(a => a.EventId == e.EventId).FirstOrDefault();
                    if (v != null) {
                        v.Title = e.Title;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Desc = e.Desc;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                    }
                } else {
                    e.CreatorId = currentUserId;
                    dc.CalendarEvents.Add(e);
                }
                await dc.SaveChangesAsync();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID) {
            var status = false;
            using (ApplicationDbContext dc = new ApplicationDbContext()) {
                var v = dc.CalendarEvents.Where(a => a.EventId == eventID).FirstOrDefault();
                if (v != null) {
                    dc.CalendarEvents.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
        public ActionResult CreateTempEvent() {
            var listUsers = Ctx.Users.ToList();
            List<SelectListItem> ListUsers = new List<SelectListItem>();
            foreach (var u in listUsers) {
                var UserItem = new SelectListItem {
                    Value = u.Id,
                    Text = u.FirstName + " " + u.LastName + " (" + u.Email + ")"
                };
                ListUsers.Add(UserItem);
            }

            return View(new CreateTempEventViewModel { NewList = ListUsers });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTempEvent(CreateTempEventViewModel m) {
            var CurrentUserId = User.Identity.GetUserId();
            // Create Event
            Ctx.TempEvents.Add(new TempEventModel {
                CreatorId = CurrentUserId,
                Description = m.Content,
                Title = m.Title
            });
            Ctx.SaveChanges();
            var TempEvent = Ctx.TempEvents.ToList().Last();
            // Create EventUsers
            foreach (var u in m.ListToSend) {
                Ctx.TempEventUsers.Add(new TempEventUserModel {
                    TempEventId = TempEvent.Id,
                    UserId = u
                });
            }
            // Create EvenSuggestion
            if (m.Suggestion1 != null) {
                Ctx.TempEventSuggestions.Add(new TempEventSuggestionModel {
                    Suggestion = m.Suggestion1,
                    TempEvenId = TempEvent.Id
                });
            }
            if (m.Suggestion2 != null) {
                Ctx.TempEventSuggestions.Add(new TempEventSuggestionModel {
                    Suggestion = m.Suggestion2,
                    TempEvenId = TempEvent.Id
                });
            }
            if (m.Suggestion3 != null) {
                Ctx.TempEventSuggestions.Add(new TempEventSuggestionModel {
                    Suggestion = m.Suggestion3,
                    TempEvenId = TempEvent.Id
                });
            }
            if (m.Suggestion4 != null) {
                Ctx.TempEventSuggestions.Add(new TempEventSuggestionModel {
                    Suggestion = m.Suggestion4,
                    TempEvenId = TempEvent.Id
                });
            }
            Ctx.SaveChanges();
            return RedirectToAction("Index");
        }
        public  ActionResult InviteTempEvent(int TEI)
        {
            var CurrentUserId = User.Identity.GetUserId();
            var EventSuggestions = Ctx.TempEventSuggestions.Where(s => s.TempEvenId == TEI).ToList();
            string Sg1 = null;
            string Sg2 = null;
            string Sg3 = null;
            string Sg4 = null;
            if(EventSuggestions.Count() != 0) { 
                var Sg1obj = EventSuggestions.ElementAt(0);
                Sg1 = Sg1obj.Suggestion;
                if(EventSuggestions.Count() > 1) {
                    var Sg2obj = EventSuggestions.ElementAt(1);
                    Sg2 = Sg2obj.Suggestion;
                }
                if (EventSuggestions.Count() > 2) {
                    var Sg3obj = EventSuggestions.ElementAt(2);
                    Sg3 = Sg3obj.Suggestion;
                }
                if (EventSuggestions.Count() > 3) {
                    var Sg4obj = EventSuggestions.ElementAt(3);
                    Sg4 = Sg4obj.Suggestion;
                }
            }
            var EventInfo = Ctx.TempEvents.FirstOrDefault(e => e.Id == TEI);
            var Creator = UserManager.FindById(EventInfo.CreatorId);
            var CreatorName = Creator.FirstName + " " + Creator.LastName + " (" + Creator.Email + ")";


            return View(new ShowTempEventViewModel {Suggestion1 = Sg1, Suggestion2 = Sg2,
                Suggestion3 = Sg3, Suggestion4 = Sg4,
                Creator = CreatorName, Title = EventInfo.Title, Content = EventInfo.Description});
        }
    }
}