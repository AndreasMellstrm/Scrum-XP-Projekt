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

namespace Örebro_Universitet_Kommunikation.Controllers
{

    public class CalendarController : Controller {
        // GET: 
        public UserManager<ApplicationUser> UserManager { get; set; }
        public ApplicationDbContext Ctx { get; set; }
        public CalendarController() {
            Ctx = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Ctx));
        }
        public ActionResult Index() {
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
    }
}