using Örebro_Universitet_Kommunikation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Örebro_Universitet_Kommunikation.Controllers
{
    public class CalendarController : Controller
    {
        // GET: Calendar
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetEvents() {
            using (CalendarDataModel dc = new CalendarDataModel()) {
                var events = dc.CalendarEvents.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        [HttpPost]
        public JsonResult SaveEvent(CalendarEvents e) {
            var status = false;
            using (CalendarDataModel dc = new CalendarDataModel()) {
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
                    dc.CalendarEvents.Add(e);
                }
                dc.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID) {
            var status = false;
            using (CalendarDataModel dc = new CalendarDataModel()) {
                var v = dc.CalendarEvents.Where(a => a.EventId == eventID).FirstOrDefault();
                if (v != null) {
                    dc.CalendarEvents.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }


    }
}