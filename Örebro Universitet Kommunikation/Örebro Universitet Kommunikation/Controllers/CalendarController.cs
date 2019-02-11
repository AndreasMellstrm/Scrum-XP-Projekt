using Microsoft.AspNet.Identity;
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
using System.Threading;

namespace Örebro_Universitet_Kommunikation.Controllers {

    public class CalendarController : Controller {
        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }

        // GET: 
        public CalendarController() {
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }




        public ActionResult Index() {
            return View();
        }


        public ActionResult GetEvents() {
            using (Ctx) {
                var events = Ctx.CalendarEvents.ToList();
                var result = new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                return result;
            }
        }
        [HttpPost]
        public async Task<JsonResult> SaveEvent(CalendarEvent e) {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            var currentUserId = currentUser.Id;
            var CurrentUserAdmin = currentUser.Admin;
            var events = Ctx.CalendarEvents.ToList();


            foreach (var item in events) {
                var user = await UserManager.FindByIdAsync(item.CreatorId);
                bool CanDelete = false;
                if (currentUserId.Equals(item.CreatorId) || CurrentUserAdmin) {
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
                }
                else {
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
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            var currentUserId = currentUser.Id;
            var CurrentUserAdmin = currentUser.Admin;
            var events = (from e in Ctx.CalendarEvents
                          where e.EventId == eventID
                          select e).ToList();
            var creatorId = events[0].CreatorId;

            bool status = false;

            using (Ctx) {
                var v = Ctx.CalendarEvents.Where(a => a.EventId == eventID).FirstOrDefault();
                if (v != null && (currentUserId == creatorId || CurrentUserAdmin)) {
                    Ctx.CalendarEvents.Remove(v);
                    Ctx.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
        public ActionResult CreateTempEvent() {
            var listUsers = Ctx.Users.ToList();
            List<SelectListItem> ListUsers = new List<SelectListItem>();
            foreach (var u in listUsers) {
                if (u.Id != User.Identity.GetUserId()) {
                    var UserItem = new SelectListItem {
                        Value = u.Id,
                        Text = u.FirstName + " " + u.LastName + " (" + u.Email + ")"
                    };
                    ListUsers.Add(UserItem);
                }
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
        public ActionResult InviteTempEvent(int TEI) {
            var CurrentUserId = User.Identity.GetUserId();
            var EventSuggestions = Ctx.TempEventSuggestions.Where(s => s.TempEvenId == TEI).ToList();
            string Sg1 = null;
            string Sg2 = null;
            string Sg3 = null;
            string Sg4 = null;
            int SI1 = 0;
            int SI2 = 0;
            int SI3 = 0;
            int SI4 = 0;
            if (EventSuggestions.Count() != 0) {
                var Sg1obj = EventSuggestions.ElementAt(0);
                Sg1 = Sg1obj.Suggestion;
                SI1 = Sg1obj.Id;
                if (EventSuggestions.Count() > 1) {
                    var Sg2obj = EventSuggestions.ElementAt(1);
                    Sg2 = Sg2obj.Suggestion;
                    SI2 = Sg2obj.Id;
                }
                if (EventSuggestions.Count() > 2) {
                    var Sg3obj = EventSuggestions.ElementAt(2);
                    Sg3 = Sg3obj.Suggestion;
                    SI3 = Sg3obj.Id;
                }
                if (EventSuggestions.Count() > 3) {
                    var Sg4obj = EventSuggestions.ElementAt(3);
                    Sg4 = Sg4obj.Suggestion;
                    SI4 = Sg4obj.Id;
                }
            }
            var EventInfo = Ctx.TempEvents.FirstOrDefault(e => e.Id == TEI);
            var Creator = UserManager.FindById(EventInfo.CreatorId);
            var CreatorName = Creator.FirstName + " " + Creator.LastName + " (" + Creator.Email + ")";


            return View(new ShowTempEventViewModel {
                Suggestion1 = Sg1,
                Suggestion2 = Sg2,
                Suggestion3 = Sg3,
                Suggestion4 = Sg4,
                SId1 = SI1,
                SId2 = SI2,
                SId3 = SI3,
                SId4 = SI4,
                Creator = CreatorName,
                Title = EventInfo.Title,
                Content = EventInfo.Description,
                TempEventId = TEI
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InviteTempEvent(ShowTempEventViewModel m) {
            var CurrentUserId = User.Identity.GetUserId();
            if (m.s1) {
                Ctx.TempEventTimes.Add(new TempEventTimeModel {
                    TempEventId = m.TempEventId,
                    SuggestionId = m.SId1,
                    UserId = CurrentUserId
                });
            }
            if (m.s2) {
                Ctx.TempEventTimes.Add(new TempEventTimeModel {
                    TempEventId = m.TempEventId,
                    SuggestionId = m.SId2,
                    UserId = CurrentUserId
                });
            }
            if (m.s3) {
                Ctx.TempEventTimes.Add(new TempEventTimeModel {
                    TempEventId = m.TempEventId,
                    SuggestionId = m.SId3,
                    UserId = CurrentUserId
                });
            }
            if (m.s4) {
                Ctx.TempEventTimes.Add(new TempEventTimeModel {
                    TempEventId = m.TempEventId,
                    SuggestionId = m.SId4,
                    UserId = CurrentUserId
                });
            }
            Ctx.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult ShowEventResult(int EventId) {
            var currentUserId = User.Identity.GetUserId();
            var currentEvent = Ctx.TempEvents.FirstOrDefault(e => e.Id == EventId);
            var suggestions = Ctx.TempEventSuggestions.Where(s => s.TempEvenId == EventId).ToList();
            string Sg1name = null;
            string Sg2name = null;
            string Sg3name = null;
            string Sg4name = null;
            int Sg1result = 0;
            int Sg2result = 0;
            int Sg3result = 0;
            int Sg4result = 0;
            if (currentEvent.CreatorId == currentUserId) {
                if (suggestions.Count() != 0) {
                    var Sg1obj = suggestions.ElementAt(0);
                    var Sg1sug = Sg1obj.Id;
                    Sg1name = Sg1obj.Suggestion;
                    Sg1result = Ctx.TempEventTimes.Where(s => s.SuggestionId == Sg1sug).Count();
                    if (suggestions.Count() > 1) {
                        var Sg2obj = suggestions.ElementAt(1);
                        var Sg2sug = Sg2obj.Id;
                        Sg2name = Sg2obj.Suggestion;
                        Sg2result = Ctx.TempEventTimes.Where(s => s.SuggestionId == Sg2sug).Count();
                    }
                    if (suggestions.Count() > 2) {
                        var Sg3obj = suggestions.ElementAt(2);
                        var Sg3sug = Sg3obj.Id;
                        Sg3name = Sg3obj.Suggestion;
                        Sg3result = Ctx.TempEventTimes.Where(s => s.SuggestionId == Sg3sug).Count();
                    }
                    if (suggestions.Count() > 3) {
                        var Sg4obj = suggestions.ElementAt(3);
                        var Sg4sug = Sg4obj.Id;
                        Sg4name = Sg4obj.Suggestion;
                        Sg4result = Ctx.TempEventTimes.Where(s => s.SuggestionId == Sg4sug).Count();
                    }
                }
                return View(new ShowResultEventViewModel {
                    Description = currentEvent.Description,
                    Title = currentEvent.Title,
                    S1Name = Sg1name,
                    S1Result = Sg1result,
                    S2Name = Sg2name,
                    S2Result = Sg2result,
                    S3Name = Sg3name,
                    S3Result = Sg3result,
                    S4Name = Sg4name,
                    S4Result = Sg4result,
                    EventId = EventId
                });
            }
            return RedirectToAction("Index");
        }
        public ActionResult ListTempEvents() {
            var currentId = User.Identity.GetUserId();
            var TempEvent = Ctx.TempEvents.Where(e => e.CreatorId == currentId).ToList();

            return View(new ListTempEventViewModel { TempEventList = TempEvent });
        }
    }
}