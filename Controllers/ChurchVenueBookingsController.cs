using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChurchSystem.Models;
using Microsoft.AspNet.Identity;

namespace ChurchSystem.Controllers
{
    public class ChurchVenueBookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ChurchVenueBookings
        public ActionResult Index()
        {
            var churchVenueBookings = db.ChurchVenueBookings.Include(c => c.ChurchVenue);
            return View(churchVenueBookings.ToList());
        }

        // GET: ChurchVenueBookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchVenueBooking churchVenueBooking = db.ChurchVenueBookings.Find(id);
            if (churchVenueBooking == null)
            {
                return HttpNotFound();
            }
            return View(churchVenueBooking);
        }

        [Authorize]
        // GET: ChurchVenueBookings/Create
        public ActionResult Create(int VenueId)
        {
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName");
            return View();
        }

        // POST: ChurchVenueBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingId,VenueId,Username,BookingDateTime,Decor,EventType")] ChurchVenueBooking churchVenueBooking)
        {
            if (ModelState.IsValid)
            {
                
                churchVenueBooking.Username = User.Identity.GetUserName();
                churchVenueBooking.Decor = false;

                db.ChurchVenueBookings.Add(churchVenueBooking);
                db.SaveChanges();
                //shot full of love
                return RedirectToAction("Create", "VenueBookingEvents", new {BookingId = churchVenueBooking.BookingId });
            }

            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", churchVenueBooking.VenueId);
            return View(churchVenueBooking);
        }

        // GET: ChurchVenueBookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchVenueBooking churchVenueBooking = db.ChurchVenueBookings.Find(id);
            if (churchVenueBooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", churchVenueBooking.VenueId);
            return View(churchVenueBooking);
        }

        // POST: ChurchVenueBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingId,VenueId,Username,BookingDateTime,Decor,EventType")] ChurchVenueBooking churchVenueBooking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(churchVenueBooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", churchVenueBooking.VenueId);
            return View(churchVenueBooking);
        }

        public ActionResult venueBookingsCalender()
        {

            var eventsData = db.ChurchVenueBookings.ToList() // Retrieve all events
              .Select(e => new
              {
                  title = e.EventType,
                  start = e.BookingDateTime.ToString("yyyy-MM-dd"), // Format the date as needed
                  location = db.ChurchVenues.Where(a => a.VenueId == e.VenueId).Select(a => a.VenueName).FirstOrDefault(),
                  EventId = e.BookingId,
              })
              .ToList();


            ViewBag.EventsData = Newtonsoft.Json.JsonConvert.SerializeObject(eventsData);

            return View();
        }

        public ActionResult admindetails(int id)
        {
            int bookingRecordId = db.VenueBookingEvent.Where(a=> a.BookingId == id).Select(a=>a.VenueEventId).FirstOrDefault();

            DateTime bookingDate = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.BookingDateTime).FirstOrDefault();
            ViewBag.BookingDate = bookingDate;

            string EventType = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.EventType).FirstOrDefault();
            ViewBag.EventType = EventType;

            int VenueId = db.ChurchVenueBookings.Where(a => a.BookingId == id).Select(a => a.VenueId).FirstOrDefault();

            int Vid = db.VenueBookingEvent.Where(a => a.BookingId == id).Select(a => a.VenueEventId).FirstOrDefault();


            //YoungTHUG-WyclefJean
            string imageurl = db.ChurchVenues.Where(a => a.VenueId == VenueId).Select(a => a.PanoramaUrl).FirstOrDefault();
            ViewBag.Imageurl = imageurl;
            string description = db.ChurchVenues.Where(a => a.VenueId == VenueId).Select(a => a.VenueDescription).FirstOrDefault();
            ViewBag.Description = description;
            string VenueName = db.ChurchVenues.Where(c => c.VenueId == VenueId).Select(v => v.VenueName).FirstOrDefault();
            ViewBag.VN = VenueName;

            //XOTOURLLIF3

            string Eventtype = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.EventType).FirstOrDefault();
            ViewBag.ET = Eventtype;



            if (Vid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueBookingEvent venueBookingEvent = db.VenueBookingEvent.Find(Vid);
            if (venueBookingEvent == null)
            {
                return HttpNotFound();
            }


            return View(venueBookingEvent);           
        }






        // GET: ChurchVenueBookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchVenueBooking churchVenueBooking = db.ChurchVenueBookings.Find(id);
            if (churchVenueBooking == null)
            {
                return HttpNotFound();
            }
            return View(churchVenueBooking);
        }

        // POST: ChurchVenueBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChurchVenueBooking churchVenueBooking = db.ChurchVenueBookings.Find(id);
            db.ChurchVenueBookings.Remove(churchVenueBooking);
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
