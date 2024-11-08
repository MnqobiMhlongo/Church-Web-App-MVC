using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ChurchSystem.Models;

namespace ChurchSystem.Controllers
{
    public class VenueBookingEventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VenueBookingEvents
        public ActionResult Index()
        {
            var venueBookingEvent = db.VenueBookingEvent.Include(v => v.ChurchVenue).Include(v => v.ChurchVenueBooking);
            return View(venueBookingEvent.ToList());
        }


        // GET: VenueBookingEvents/Details/5
        public ActionResult Details(int? id)
        {

            int bookingRecordId = db.VenueBookingEvent.Where(a => a.VenueEventId == id).Select(v => v.BookingId).FirstOrDefault();
            
           
            
            DateTime bookingDate = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.BookingDateTime).FirstOrDefault();
            ViewBag.BookingDate = bookingDate;

            string EventType = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.EventType).FirstOrDefault();
            ViewBag.EventType = EventType;

            int VenueId = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.VenueId).FirstOrDefault();


            //YoungTHUG-WyclefJean
            string imageurl = db.ChurchVenues.Where(a => a.VenueId == VenueId).Select(a => a.PanoramaUrl).FirstOrDefault();
            ViewBag.Imageurl = imageurl;   
            string description =  db.ChurchVenues.Where(a=> a.VenueId == VenueId).Select(a=>a.VenueDescription).FirstOrDefault();
            ViewBag.Description = description;
            string VenueName = db.ChurchVenues.Where(c => c.VenueId == VenueId).Select(v => v.VenueName).FirstOrDefault();
            ViewBag.VN = VenueName;

            //XOTOURLLIF3

            string Eventtype = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.EventType).FirstOrDefault();
            ViewBag.ET = Eventtype;
            
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueBookingEvent venueBookingEvent = db.VenueBookingEvent.Find(id);
            if (venueBookingEvent == null)
            {
                return HttpNotFound();
            }

           
            return View(venueBookingEvent);
        }

        public ActionResult sendInvites()
        {
            return View(new SendInvites());
        }

        [HttpPost]
        public ActionResult SendInvites(SendInvites model)
        {
            if (ModelState.IsValid)
            {
                foreach (var phoneNumber in model.PhoneNumbers)
                {
                    SendSms(phoneNumber, model.Message);
                }

                ViewBag.SuccessMessage = "Invitations sent successfully!";
            }
            return View(model);
        }

        //basJcole - Tribe
        private void SendSms(string phoneNumber, string message)
        {
            string myURI = "https://api.bulksms.com/v1/messages";
            string myUsername = "andilembatha";
            string myPassword = "Password@01";
            string myData = $"{{to: \"{phoneNumber}\", body:\"{message}\"}}";

            var request = WebRequest.Create(myURI);
            request.Credentials = new NetworkCredential(myUsername, myPassword);
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.ContentType = "application/json";

            var encoding = new UnicodeEncoding();
            var encodedData = encoding.GetBytes(myData);

            var stream = request.GetRequestStream();
            stream.Write(encodedData, 0, encodedData.Length);
            stream.Close();

            try
            {
                var response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                Console.WriteLine(reader.ReadToEnd());
            }
            catch (WebException ex)
            {
                Console.WriteLine("An error occurred:" + ex.Message);
                var reader = new StreamReader(ex.Response.GetResponseStream());
                Console.WriteLine("Error details:" + reader.ReadToEnd());
            }
        }






            // GET: VenueBookingEvents/Create
            public ActionResult Create(int BookingId)
        {
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName");
            ViewBag.BookingId = new SelectList(db.ChurchVenueBookings, "BookingId", "Username");
            return View();
        }

        // POST: VenueBookingEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VenueEventId,VenueId,BookingId,EventDescription,startTime,endTime,Attendees,tent,moreSeats,PostCleanup,equipment,layoutid,AddOns,TotalCost")] VenueBookingEvent venueBookingEvent)
        {
            if (ModelState.IsValid)
            {
                int venueId = db.ChurchVenueBookings.Where(a => a.BookingId == venueBookingEvent.BookingId).Select(a => a.VenueId).FirstOrDefault();
                venueBookingEvent.VenueId = venueId;
                double price = 350;
                double addons = 0;

                bool decor = db.ChurchVenueBookings.Where(a => a.BookingId == venueBookingEvent.BookingId).Select(a => a.Decor).FirstOrDefault();
                              
                if (venueBookingEvent.moreSeats) { price += 200; addons += 200; };
                if (venueBookingEvent.PostCleanup) { price += 200; addons += 200; };
           
                venueBookingEvent.tent = false;
                venueBookingEvent.equipment = false;
                
                venueBookingEvent.TotalCost = price;
                venueBookingEvent.AddOns = addons;
                string SL = venueBookingEvent.layoutid;

                db.VenueBookingEvent.Add(venueBookingEvent);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = venueBookingEvent.VenueEventId});
            }

            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", venueBookingEvent.VenueId);
            ViewBag.BookingId = new SelectList(db.ChurchVenueBookings, "BookingId", "Username", venueBookingEvent.BookingId);
            return View(venueBookingEvent);
        }      

        // GET: VenueBookingEvents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueBookingEvent venueBookingEvent = db.VenueBookingEvent.Find(id);
            if (venueBookingEvent == null)
            {
                return HttpNotFound();
            }
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", venueBookingEvent.VenueId);
            ViewBag.BookingId = new SelectList(db.ChurchVenueBookings, "BookingId", "Username", venueBookingEvent.BookingId);
            return View(venueBookingEvent);
        }

        // POST: VenueBookingEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VenueEventId,VenueId,BookingId,EventDescription,startTime,endTime,Attendees,tent,moreSeats,PostCleanup,equipment,layoutid,AddOns,TotalCost")] VenueBookingEvent venueBookingEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(venueBookingEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", venueBookingEvent.VenueId);
            ViewBag.BookingId = new SelectList(db.ChurchVenueBookings, "BookingId", "Username", venueBookingEvent.BookingId);
            return View(venueBookingEvent);
        }

        public ActionResult BC()
        {
            return View();
        }



        // GET: VenueBookingEvents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueBookingEvent venueBookingEvent = db.VenueBookingEvent.Find(id);
            if (venueBookingEvent == null)
            {
                return HttpNotFound();
            }
            return View(venueBookingEvent);
        }

        // POST: VenueBookingEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VenueBookingEvent venueBookingEvent = db.VenueBookingEvent.Find(id);
            db.VenueBookingEvent.Remove(venueBookingEvent);
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

        public ActionResult Confirmation(int id)
        {

            
            int bookingRecordId = db.VenueBookingEvent.Where(a => a.VenueEventId == id).Select(v => v.BookingId).FirstOrDefault();
            bool decor = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.Decor).FirstOrDefault();
            if (decor)
            {
                ViewBag.Decor = 200;
            }

            DateTime bookingDate = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.BookingDateTime).FirstOrDefault();
            ViewBag.BookingDate = bookingDate;

            string EventType = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.EventType).FirstOrDefault();
            ViewBag.EventType = EventType;

            int VenueId = db.ChurchVenueBookings.Where(a => a.BookingId == bookingRecordId).Select(a => a.VenueId).FirstOrDefault();

            var images = db.VenueImages.Where(a => a.VenueId == VenueId).Select(v => v.ImageUrl).ToList();

            ViewBag.Images = images;

            string VenueName = db.ChurchVenues.Where(c => c.VenueId == VenueId).Select(v => v.VenueName).FirstOrDefault();
            ViewBag.VN = VenueName;

            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueBookingEvent venueBookingEvent = db.VenueBookingEvent.Find(id);

            ViewBag.Paid = venueBookingEvent.Paid;
            ViewBag.Verification = "Booking Confirmation";

            if (venueBookingEvent == null)
            {
                return HttpNotFound();
            }
            return View(venueBookingEvent);
        }



    }
}
