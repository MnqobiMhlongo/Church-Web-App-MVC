using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using ChurchSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Owin.Security;
using PayPal.Api;


namespace ChurchSystem.Controllers
{
    public class EventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Events
        public ActionResult Index()
        {
           
            

            return View(db.Events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }

        public ActionResult SubmitSurvey(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitSurvey(EventSurvey survey)
        {
            if (ModelState.IsValid)
            {
               db.EventSurveys.Add(survey);
               db.SaveChanges();
            }
            return RedirectToAction("HRC", "Home", new { successMessage = "Thank You, Your Feedback is Appreciated" });
        }




        public ActionResult Invite(int? id)
        {
            
            Events events = db.Events.Find(id);

            //lmaoo
            string user = User.Identity.GetUserName();
            bool accepted = db.Attendance.Any(v => v.EventId == id && v.Attandee == user);


            string verification = db.Attendance.Where(v => v.Attandee == user && v.EventId == id)
                .Select(u => u.verification).FirstOrDefault(); 


            ViewBag.Verification = verification;    
            ViewBag.Accepted = accepted;    
            return View(events);
        }

        public ActionResult RSVP(int Eventid)
        {

            Attendance attendance = new Attendance
            {
                EventId = Eventid,
                Attandee = User.Identity.GetUserName(),
                verification = Eventid.ToString()+User.Identity.GetUserName(),
                attended = false
            };

            db.Attendance.Add(attendance);
            db.SaveChanges();   
            return RedirectToAction("HRC","Home", new { successMessage = "Invitation Accepted" });
        }


        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventId,Title,Description,dateTime,Location,Organizer")] Events events)
        {
            if (ModelState.IsValid)
            {
                events.DatePosted = DateTime.Now;
                events.Status = "Upcoming";
                db.Events.Add(events);
                db.SaveChanges();
                return RedirectToAction("Create","EventPrograms", new { EventId = events.EventId});
            }

            return View(events);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventId,Title,Description,dateTime,Location,Organizer")] Events events)
        {
            if (ModelState.IsValid)
            {
                db.Entry(events).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(events);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Events events = db.Events.Find(id);
            db.Events.Remove(events);
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

        public ActionResult map()
        { return View(); }




        public ActionResult Calender()
        {

            var eventsData = db.Events.ToList() // Retrieve all events
              .Select(e => new
              {
                  title = e.Title,
                  start = e.dateTime.ToString("yyyy-MM-dd"), // Format the date as needed
                  location = e.Location,
                  EventId = e.EventId
              })
              .ToList();


            ViewBag.EventsData = Newtonsoft.Json.JsonConvert.SerializeObject(eventsData);



            return View(); 
        }


        public ActionResult Event()
        {
            return View(db.Events.Where(e => e.Status == "Finished").ToList());         
        }



        public ActionResult Attendance(int? eventId)
        {
            var attendees = db.Attendance.Where(e => e.EventId == eventId).ToList();
            var attendeesCount = db.Attendance.Where(e => e.EventId == eventId).Select(e => e.Attandee).Count();
            var attended = db.Attendance.Where(e => e.EventId == eventId && e.attended == true).Select(e => e.Attandee).Count();


            ViewBag.Attendees = attendees;
            ViewBag.Count = attendeesCount;
            ViewBag.Attended = attended;
           
            return View();

        }




        public ActionResult Rating(int? eventId)
        {
            var record = db.Events.Find(eventId);

            //ViewBag.record = record.Rating;
            return View(record);  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult rate(int? eventId, int rating, string comment)
        {
            if (eventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var finishedevent = db.Events.Find(eventId);
            if (finishedevent == null)
            {
                return HttpNotFound();
            }

            finishedevent.Rating = rating;
            finishedevent.Comments = comment;

            db.Entry(finishedevent).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("HRC", "Home", new { successMessage = "FeedBack Submitted ;)" });
        }


        public ActionResult Events4Register()
        {
            var events = db.Events.Where(e => e.Status == "Upcoming").ToList();


            return View(events);
        }


        public ActionResult EventRegister(int? EventId)
        {

            var register = db.Attendance.Where(a => a.EventId == EventId).ToList();
            var CountTotal  = db.Attendance.Where(a => a.EventId == EventId).Count();
            var CountTrue = db.Attendance.Where(a => a.EventId == EventId && a.attended == true ).Count();


            ViewBag.Register = register;
            ViewBag.Count = CountTotal;
            ViewBag.CountTrue = CountTrue;
            return View();
        }

        public void Arrived(string verification)
        {

            Attendance attendance = db.Attendance.Where(a => a.verification == verification).FirstOrDefault();


            attendance.attended = true;

            db.Entry(attendance).State = EntityState.Modified;
            db.SaveChanges();


            //return RedirectToAction("EventRegister", "Events", new { successMessage = "Consultation Requested" });
           // return RedirectToAction("ActivityUpdate", "EventPrograms", new { successMessage = "Consultation Requested" });
        }


        public ActionResult Finish(int id)
        {

            //Blxst-KeepCominBack
            Events ev = db.Events.Where(e => e.EventId == id).FirstOrDefault();

            ev.Status = "Finished";


            var register = db.Attendance.Where(a => a.EventId == id).ToList();

            foreach(var item in register)
            {
                emailwithLinkTest(id, item.Attandee);
            }


            db.Entry(ev).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Events4Register", new { successMessage = "Event Finished" });
        }

        public void emailwithLinkTest(int id,string usermail)
        {
            
            MailMessage mail = new MailMessage();
            mail.To.Add(usermail);
            mail.From = new MailAddress("GRACEOFGODCHURCH@gmail.com");
            mail.Subject = "Please Review Our Event";

            // Use Url.Action to generate the link
            string surveyLink = Url.Action("SubmitSurvey", "Events", new { id = id }, Request.Url.Scheme); // Replace "Survey" and "Events" with your action and controller names

            string Body = $"Thank you for attending our event" +
                          "Your Attendance was highly appreciated. We Also Appreciate Your Feedback <br/><br/>" +
                          $"<a href='{surveyLink}'>Survey Here!</a>";

            mail.Body = Body;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("graceofgodchurch0@gmail.com", "wwiaiouwesmobmrd"); // Enter sender's User name and password   
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        public ActionResult ReviewIndex()
        {
            return View(db.EventSurveys.ToList());
        }

        public ActionResult Review(int id)
        {
            EventSurvey eventSurvey = db.EventSurveys.Find(id);

            ViewBag.CR = eventSurvey.ContentReview;
            ViewBag.VR = eventSurvey.VenueReview;
            ViewBag.PR = eventSurvey.PresentersReview;
            ViewBag.LR = eventSurvey.LocationReview;
            ViewBag.SR = eventSurvey.SatisfactionReview;
            ViewBag.R = eventSurvey.Recommend;
            ViewBag.AA = eventSurvey.AttendAgain;
            ViewBag.LS = eventSurvey.LengthAndSchedule;
            ViewBag.C = ViewBag.Comment;



            return View(eventSurvey);
        }


    }
}
