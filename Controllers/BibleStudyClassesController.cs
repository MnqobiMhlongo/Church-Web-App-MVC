using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ChurchSystem.Migrations;
using ChurchSystem.Models;
using Microsoft.AspNet.Identity;

namespace ChurchSystem.Controllers
{
    public class BibleStudyClassesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BibleStudyClasses
        public ActionResult Index()
        {
            var bibleStudyClasses = db.BibleStudyClasses.Include(b => b.Course);
            return View(bibleStudyClasses.ToList());
        }

        // GET: BibleStudyClasses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BibleStudyClasses bibleStudyClasses = db.BibleStudyClasses.Find(id);
            if (bibleStudyClasses == null)
            {
                return HttpNotFound();
            }
            return View(bibleStudyClasses);
        }

        // GET: BibleStudyClasses/Create
        public ActionResult Create(int CourseId)
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            return View();
        }

        // POST: BibleStudyClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "classId,CourseId,DateTime,classdecription,venueId,churchLeader")] BibleStudyClasses bibleStudyClasses)
        {
            if (ModelState.IsValid)
            {
                var check = db.BibleStudyClasses.Where(a=>a.DateTime == bibleStudyClasses.DateTime && a.venueId == bibleStudyClasses.venueId).Select(a=> a.classdecription).FirstOrDefault();

                if (check != null)
                {
                    return RedirectToAction("Create", new { CourseId = bibleStudyClasses.CourseId, successMessage = "Venue Already Scheduled For Time Slot, Please Schedule Another" });
                }


                Announcement announcement = new Announcement();

                announcement.author = bibleStudyClasses.churchLeader;
                announcement.CourseId = bibleStudyClasses.CourseId;
                announcement.DateTimeSent = DateTime.Now;
                announcement.Message = "Bible Study Class Has been Scheduled on " + bibleStudyClasses.DateTime.ToString("dddd d MMMM yyyy") + ". Please View Details On Calender.";
                

                db.Announcements.Add(announcement);  
                db.BibleStudyClasses.Add(bibleStudyClasses);
                db.SaveChanges();
                return RedirectToAction("AdmminIndex", "Courses", new {successMessage = "Class Scheduled And Announcement Posted "});
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", bibleStudyClasses.CourseId);
            return View(bibleStudyClasses);
        }

        public ActionResult adminCourseSessionsCalender(int CourseID)
        {
            var eventsData = db.BibleStudyClasses.Where(a => a.CourseId == CourseID).ToList() // Retrieve all events
             .Select(e => new
             {
                 title = e.classdecription,
                 start = e.DateTime.ToString("yyyy-MM-dd"), // Format the date as needed
                 location = db.ChurchVenues.Where(a => a.VenueId == e.venueId).Select(a => a.VenueName).FirstOrDefault(),
                 EventId = e.classId,
             })
             .ToList();


            ViewBag.EventsData = Newtonsoft.Json.JsonConvert.SerializeObject(eventsData);

            return View();
        }

        public ActionResult CourseSessionsCalender(int CourseID)
        {
            var eventsData = db.BibleStudyClasses.Where(a=>a.CourseId == CourseID).ToList() // Retrieve all events
             .Select(e => new
             {
                 title = e.classdecription,
                 start = e.DateTime.ToString("yyyy-MM-dd"), // Format the date as needed
                 location = db.ChurchVenues.Where(a => a.VenueId == e.venueId).Select(a => a.VenueName).FirstOrDefault(),
                 EventId = e.classId,
             })
             .ToList();


            ViewBag.EventsData = Newtonsoft.Json.JsonConvert.SerializeObject(eventsData);

            return View();       
        }

        public ActionResult Attend(int? id)
        {
            return View();
        }


        [HttpPost]
        public ActionResult SaveImage(classAttendance model)
        {
            // Convert Base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(model.attendeeImagedata.Replace("data:image/png;base64,", ""));
            int count = 1;


            // Save the image to the database
            var image = new classAttendance
            {
                classId = 9,
                attendeeImagedata = model.attendeeImagedata,
                Name = "Student " + count.ToString()
            };

            db.classAttendance.Add(image);
            db.SaveChanges();

            return Json(new { success = true, imageId = image.attendeeId });
        }


        public ActionResult attendance()
        {
            return View(db.classAttendance.ToList());
        }





        public ActionResult SendInvites()
        {
            return View();
        }

        [Authorize]
        public ActionResult ChatBot()
        {
            ViewBag.name = User.Identity.GetUserName();   
             return View();
        }


        public ActionResult StudyGroupChat(string gn)
        {
            ViewBag.nm = gn;
            ViewBag.name = User.Identity.GetUserName();
            return View();
        }


        // GET: BibleStudyClasses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BibleStudyClasses bibleStudyClasses = db.BibleStudyClasses.Find(id);
            if (bibleStudyClasses == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", bibleStudyClasses.CourseId);
            return View(bibleStudyClasses);
        }

        // POST: BibleStudyClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "classId,CourseId,DateTime,classdecription,venueId,churchLeader")] BibleStudyClasses bibleStudyClasses)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bibleStudyClasses).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", bibleStudyClasses.CourseId);
            return View(bibleStudyClasses);
        }

        // GET: BibleStudyClasses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BibleStudyClasses bibleStudyClasses = db.BibleStudyClasses.Find(id);
            if (bibleStudyClasses == null)
            {
                return HttpNotFound();
            }
            return View(bibleStudyClasses);
        }

        // POST: BibleStudyClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BibleStudyClasses bibleStudyClasses = db.BibleStudyClasses.Find(id);
            db.BibleStudyClasses.Remove(bibleStudyClasses);
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
    }
}
