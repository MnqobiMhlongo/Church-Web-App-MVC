using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChurchSystem.Migrations;
using ChurchSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs; 

namespace ChurchSystem.Controllers
{
    public class BookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Bookings
        public ActionResult Index(string username)
        {
            int Lid = db.ChurchLeaders.Where(c => c.Email == username).Select(i => i.leaderId).FirstOrDefault();

            var bookings = db.Bookings
                .Where(b => b.leaderId == Lid && b.isApproved == false);
                
            return View(bookings.ToList());
        }

        public ActionResult MyBookings()
        {
            string user = User.Identity.GetUserName();

            int leader = db.ChurchLeaders.Where(c => c.Email == user).Select(c => c.leaderId).FirstOrDefault();
            var bookings = db.Bookings.Include(b => b.ChurchLeaders);
            var mybookings = db.Bookings
                                    .Where(b => b.leaderId == leader)
                                    .ToList();

            return View(mybookings);
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        public ActionResult CLList()
        {
            var bookings = db.Bookings.Include(b => b.ChurchLeaders);
            //return View(bookings.ToList());
            return View(db.ChurchLeaders.ToList());
        }


        // GET: Bookings/Create
        [System.Web.Mvc.Authorize]
        public ActionResult Create(int? leaderId)
        {
            ViewBag.leaderId = new SelectList(db.ChurchLeaders, "leaderId", "LeaderName");

            Booking booking = new Booking();
            return View(booking);
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,user,BookingDate,Purpose,leaderId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var user = userManager.FindById(User.Identity.GetUserId());

                if (user != null)
                {
                    string fullName = user.NameSurname; // Assuming your user model has a property for the full name
                    booking.user = fullName;
                }


                booking.isApproved = false;
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("HRC","Home",new { successMessage = "Consultation Requested" });
            }

            ViewBag.leaderId = new SelectList(db.ChurchLeaders, "leaderId", "LeaderName", booking.leaderId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.leaderId = new SelectList(db.ChurchLeaders, "leaderId", "LeaderName", booking.leaderId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,user,BookingDate,Purpose,leaderId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.leaderId = new SelectList(db.ChurchLeaders, "leaderId", "LeaderName", booking.leaderId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
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


        public ActionResult MyCalender(string username)
        {
            int Lid = db.ChurchLeaders.Where(c => c.Email == username).Select(i => i.leaderId).FirstOrDefault();

            var bookings = db.Bookings
                .Where(b => b.leaderId == Lid && b.isApproved == true)
                .ToList();

            var eventsData = db.Bookings.ToList().Where(b => b.leaderId == Lid && b.isApproved == true) // Retrieve all events
          .Select(e => new
          {
              title = e.Purpose,
              start = e.BookingDate.ToString("yyyy-MM-dd"), // Format the date as needed
              purpose = e.Purpose,
              BookingId = e.Id  
          })
          .ToList();


            ViewBag.EventsData = Newtonsoft.Json.JsonConvert.SerializeObject(eventsData);

            return View(bookings);
        }

        //private IHubContext<ChatHub> HC;

        public BookingsController()
        {
            var hubContext = GlobalHost.ConnectionManager;

            //HC = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        }


       /* public ActionResult send()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.All.addNewMessageToPage("Message1", "Message2");
            return RedirectToAction("Index","Bookings", new {username = User.Identity.GetUserName()});
        }*/

        public ActionResult ApproveBooking(int id)
        {
            Booking booking = db.Bookings.Find(id);
            booking.isApproved = true;
            db.SaveChanges();

            return RedirectToAction("Index", "Bookings", new { successMessage = "Consultation Approved & Added To Your Calender" });
        }

        public ActionResult DeclineBooking(int id)
        {
            Booking booking = db.Bookings.Find(id);
            booking.isApproved = false;
            db.Bookings.Remove(booking);           
            db.SaveChanges();

            return RedirectToAction("Index", "Bookings", new { successMessage = " Consultation Request Declined" });
        }

    }
}
