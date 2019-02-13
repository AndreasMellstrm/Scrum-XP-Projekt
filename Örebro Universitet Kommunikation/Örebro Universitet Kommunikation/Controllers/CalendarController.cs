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
using Örebro_Universitet_Kommunikation.Helpers;

using Ical.Net;
using Ical.Net.DataTypes;
//using Ical.Net.Serialization.iCalendar.Serializers;
using Ical.Net.Serialization;
using System.Text;

namespace Örebro_Universitet_Kommunikation.Controllers {
    [Authorize]
    public class CalendarController : Controller {
        public ApplicationDbContext Ctx { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }

        // GET: 
        public CalendarController() {
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }




        public ActionResult Index() {
            return View(UserManager.FindById(User.Identity.GetUserId()));
        }


        public ActionResult GetEvents() {
            using (Ctx) {
                var events = Ctx.CalendarEvents.ToList();
                List<CalenderItemViewModel> list = new List<CalenderItemViewModel>();
                var canDelete = false;
                var currentUser = UserManager.FindById(User.Identity.GetUserId());
                foreach(var e in events) {
                    var connections = (from c in Ctx.ApplicationUserCalendarEvents
                                 where c.EventId == e.EventId
                                 && c.CanCome == true
                                 select c).ToList();
                    List<string> users = new List<string>(); 
                    foreach(var u in connections) {
                        var user = UserManager.FindById(u.UserId);
                        var userEmail = user.Email;
                        if (user.IsInactive) {
                            userEmail = "Inaktiverad användare";
                        }
                        var name = user.FirstName + " " + user.LastName + " (" + userEmail + ")";
                        users.Add(name);
                    }
                    var creator = UserManager.FindById(e.CreatorId);
                    if(creator.Id == currentUser.Id || currentUser.Admin) {
                        canDelete = true;
                    }
                    else {
                        canDelete = false;
                    }
                    var creatorMail = creator.Email;
                    if (creator.IsInactive) {
                        creatorMail = "Inaktiverad användare";
                    }
                    string creatorName = creator.FirstName + " " + creator.LastName + " (" + creatorMail + ")";
                    var item = new CalenderItemViewModel {
                        Title = e.Title,
                        ThemeColor = e.ThemeColor,
                        Start = e.Start,
                        End = e.End,
                        IsFullDay = e.IsFullDay,
                        EventId = e.EventId,
                        Desc = e.Desc,
                        CreatorName = creatorName,
                        Users = users.ToArray(),
                        CanDelete = canDelete
                    };
                    list.Add(item);
                }
                var result = new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

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
        public ActionResult Delete(int EventId) {
            var remove = Ctx.CalendarEvents.Find(EventId);
            Ctx.CalendarEvents.Remove(remove);
            Ctx.SaveChanges();

            return RedirectToAction("Index");
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

        public List<SelectListItem> GetAllUsersSelectList() {
            var listUsers = (from u in Ctx.Users
                             where u.IsInactive == false
                             select u).ToList();
            List < SelectListItem > ListUsers = new List<SelectListItem>();
            foreach (var u in listUsers) {
                if (u.Id != User.Identity.GetUserId()) {
                    var UserItem = new SelectListItem {
                        Value = u.Id,
                        Text = u.FirstName + " " + u.LastName + " (" + u.Email + ")"
                    };
                    ListUsers.Add(UserItem);
                }
            }
            return ListUsers;
        }

        public ActionResult CreateTempEvent() {


            return View(new CreateTempEventViewModel {
                NewList = GetAllUsersSelectList(),
                ErrorMessage = ""
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTempEvent(CreateTempEventViewModel m) {
            if (ModelState.IsValid) {
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
                    var emailHelper = new EmailHelper("orukommunikation@gmail.com", "Kakan1210");
                    emailHelper.SendEmailMeeting("Nytt preliminärt möte", "Du har blivit inbjuden till ett möte. Gå in och rösta på en tid", u);
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
                var result = Ctx.SaveChanges();
                if (result > 0) {
                    ModelState.Clear();
                    return View(new CreateTempEventViewModel {
                        ErrorMessage = "Preliminärt event är nu skapat",
                        NewList = GetAllUsersSelectList()
                    });
                }
            }
            return View(new CreateTempEventViewModel {
                ErrorMessage = "Det gick inte att skapa det preliminära eventet",
                NewList = GetAllUsersSelectList()
            });
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
        public ActionResult CreateEvent(int EventId) {
            var currentEvent = Ctx.TempEvents.FirstOrDefault(e => e.Id == EventId);
            
            return View(new CreateEventViewModel { EventId = EventId, EventTitle = currentEvent.Title });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEvent(CreateEventViewModel m) {
            var currentEvent = Ctx.TempEvents.FirstOrDefault(e => e.Id == m.EventId);
            var userList = Ctx.TempEventUsers.Where(e => e.TempEventId == m.EventId).ToList();

            Ctx.CalendarEvents.Add(new CalendarEvent {
                CreatorId = currentEvent.CreatorId,
                Start = m.Start,
                End = m.End,
                Title = currentEvent.Title,
                Desc = currentEvent.Description
            });
            Ctx.SaveChanges();
            var eventId = Ctx.CalendarEvents.ToList().Last();
            foreach(var u in userList) {
                var item = new ApplicationUserCalendarEvents {
                    EventId = eventId.EventId,
                    UserId = u.UserId,
                    CanCome = false
                };
                var emailHelper = new EmailHelper("orukommunikation@gmail.com", "Kakan1210");
                emailHelper.SendEmailMeeting("Nytt möte", "", u.UserId);
                Ctx.ApplicationUserCalendarEvents.Add(item);
                Ctx.TempEventUsers.Remove(u);
            }
            var suggestions = Ctx.TempEventSuggestions.Where(s => s.TempEvenId == m.EventId);
            foreach(var s in suggestions) { 
                Ctx.TempEventSuggestions.Remove(s);
            }
            var result = Ctx.TempEventTimes.Where(t => t.TempEventId == m.EventId);
            foreach(var r in result) {
                Ctx.TempEventTimes.Remove(r);
            }
            Ctx.TempEvents.Remove(currentEvent);
            Ctx.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult AcceptEvent(int eventId) {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var events = (from e in Ctx.ApplicationUserCalendarEvents
                          where e.UserId == currentUser.Id
                          && e.EventId == eventId
                          select e).ToList();
            events[0].CanCome = true;
            Ctx.SaveChanges();
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult DeclineEvent(int eventId) {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var events = (from e in Ctx.ApplicationUserCalendarEvents
                          where e.UserId == currentUser.Id
                          && e.EventId == eventId
                          select e).ToList();
            Ctx.ApplicationUserCalendarEvents.Remove(events[0]);
            Ctx.SaveChanges();
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult ExportCal() {

            var cal = new Ical.Net.Calendar();
            var calEvents = Ctx.CalendarEvents;

            foreach(var item in calEvents) {
                cal.Events.Add(new Ical.Net.CalendarComponents.CalendarEvent {
                    Class = "public",
                    Summary = item.Title,
                    Created = new CalDateTime(DateTime.Now),
                    Description = item.Desc,
                    Start = new CalDateTime(Convert.ToDateTime(item.Start)),
                    End = new CalDateTime(Convert.ToDateTime(item.End)),
                    Sequence = 0,
                    Uid = Guid.NewGuid().ToString(),
                    Location = "N/A"

                });
            }
            var serializer = new CalendarSerializer(new SerializationContext());
            var serializedCalendar = serializer.SerializeToString(cal);
            var bytesCalendar = Encoding.UTF8.GetBytes(serializedCalendar);

            return File(bytesCalendar, "text/calendar", "kalender.ics");
        }

    }
}