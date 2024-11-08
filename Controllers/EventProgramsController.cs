using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using ChurchSystem.Models;
using Newtonsoft.Json;

namespace ChurchSystem.Controllers
{
    public class EventProgramsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EventPrograms
        public ActionResult Index()
        {
            var eventsProgram = db.EventsProgram.Include(e => e.Events);
            return View(eventsProgram.ToList());
        }

        // GET: EventPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventProgram eventProgram = db.EventsProgram.Find(id);
            if (eventProgram == null)
            {
                return HttpNotFound();
            }
            return View(eventProgram);
        }

        // GET: EventPrograms/Create
        public ActionResult Create(int? EventId)
        {
            ViewBag.EventId = new SelectList(db.Events, "EventId", "Title");
            return View();
        }

        // POST: EventPrograms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "programId,EventId,activity,finished")] EventProgram eventProgram)
        {
            if (ModelState.IsValid)
            {
              
                eventProgram.finished = false;


                string activitiesJson = Request.Form["Activities"];
                List<string> activities = JsonConvert.DeserializeObject<List<string>>(activitiesJson);

                getActivitiesList(eventProgram.EventId, activities);


                db.SaveChanges();
                return RedirectToAction("Events4Register","Events", new { successMessage = "Event Set & Invitations Sent" });
            }

            ViewBag.EventId = new SelectList(db.Events, "EventId", "Title", eventProgram.EventId);
            return View(eventProgram);
        }

        // GET: EventPrograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventProgram eventProgram = db.EventsProgram.Find(id);
            if (eventProgram == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventId = new SelectList(db.Events, "EventId", "Title", eventProgram.EventId);
            return View(eventProgram);
        }

        // POST: EventPrograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "programId,EventId,activity,finished")] EventProgram eventProgram)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventProgram).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventId = new SelectList(db.Events, "EventId", "Title", eventProgram.EventId);
            return View(eventProgram);
        }

        // GET: EventPrograms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventProgram eventProgram = db.EventsProgram.Find(id);
            if (eventProgram == null)
            {
                return HttpNotFound();
            }
            return View(eventProgram);
        }

        // POST: EventPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventProgram eventProgram = db.EventsProgram.Find(id);
            db.EventsProgram.Remove(eventProgram);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        //KnayeWestLonely
        public void getActivitiesList(int Eventid,List<string>activities)
        {
            // Save each activity as a record with the eventId and finished = false
            foreach (var activity in activities)
            {
                EventProgram newActivity = new EventProgram
                {
                    EventId = Eventid,
                    activity = activity,
                    finished = false
                };
                db.EventsProgram.Add(newActivity);
            }

            db.SaveChanges();
        }

        public ActionResult eventProgramView(int? EventId)
        {
       

            //var EventDetails = db.Events.Where(v => v.EventId == 5).ToList();
            var Program = db.EventsProgram.Where(v => v.EventId == EventId).ToList();
       
            ViewBag.Program = Program;  
            return View();
        }


        public ActionResult ActivityUpdate(int? EventId)
        {
            var eventActivities = db.EventsProgram.Where(v => v.EventId == EventId).ToList();

            return View(eventActivities);
        }

        public ActionResult SetFinished(int? programId)
        {
            var activity = db.EventsProgram.Find(programId);

            activity.finished = true;

            db.Entry(activity).State = EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("ActivityUpdate","EventPrograms", new { successMessage = "Consultation Requested" });
        }


    }
}
