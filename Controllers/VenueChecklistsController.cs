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
    public class VenueChecklistsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VenueChecklists
        public ActionResult Index()
        {
            var venueChecklist = db.VenueChecklist.Include(v => v.ChurchVenue);
            return View(venueChecklist.ToList());
        }

        // GET: VenueChecklists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueChecklist venueChecklist = db.VenueChecklist.Find(id);
            if (venueChecklist == null)
            {
                return HttpNotFound();
            }
            return View(venueChecklist);
        }

        // GET: VenueChecklists/Create
        public ActionResult Create(int VenueId)
        {
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName");
            if(VenueId == 4)
            {
                ViewBag.outside = true; 
            }
            return View();
        }

        private void SendSms()
        {
            string myURI = "https://api.bulksms.com/v1/messages";
           //JheneAiko-TheWorst
            string myUsername = "andilembatha";
            string myPassword = "Password@01";
            //string myData2 = $"{{to: \"{phoneNumber}\", body:\"{message}\"}}";
            string myData = "{to: \"27712403287\", body:\"Dear Client, we’re pleased to inform you that your venue at is fully prepared and ready for your event. If you have any further requests, feel free to contact us. Looking forward to hosting you - Holy Rosary Church \"}";


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

        // POST: VenueChecklists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,VenueId,Cleaned,SoundEquipemnt,Chairs,Lights,Tents,SafetyEquipment")] VenueChecklist venueChecklist)
        {
            if (ModelState.IsValid)
            {
                db.VenueChecklist.Add(venueChecklist);
                db.SaveChanges();
                SendSms();
                return RedirectToAction("venueBookingsCalender", "ChurchVenueBookings", new { successMessage = "Checklist complete, Venue Ready For Client" });
            }

            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", venueChecklist.VenueId);
            return View(venueChecklist);
        }

        // GET: VenueChecklists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueChecklist venueChecklist = db.VenueChecklist.Find(id);
            if (venueChecklist == null)
            {
                return HttpNotFound();
            }
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", venueChecklist.VenueId);
            return View(venueChecklist);
        }

        // POST: VenueChecklists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,VenueId,Cleaned,SoundEquipemnt,Chairs,Lights,Tents,SafetyEquipment")] VenueChecklist venueChecklist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(venueChecklist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", venueChecklist.VenueId);
            return View(venueChecklist);
        }

        // GET: VenueChecklists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueChecklist venueChecklist = db.VenueChecklist.Find(id);
            if (venueChecklist == null)
            {
                return HttpNotFound();
            }
            return View(venueChecklist);
        }

        // POST: VenueChecklists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VenueChecklist venueChecklist = db.VenueChecklist.Find(id);
            db.VenueChecklist.Remove(venueChecklist);
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
