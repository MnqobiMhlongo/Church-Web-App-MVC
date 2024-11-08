using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChurchSystem.Migrations;
using ChurchSystem.Models;

namespace ChurchSystem.Controllers
{
    public class ChurchVenuesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ChurchVenues
        public ActionResult Index()
        {
            var venues = db.ChurchVenues.Include(v => v.VenueImages).ToList();
            return View(venues);
        }

        //freddie gibbs&madlib tinydesk
        public ActionResult Venues()
        {
            return View();
        }

        public ActionResult ConfirmVenue(int id)
        {
            

            // Find the venue by ID
            var venue = db.ChurchVenues.FirstOrDefault(v => v.VenueId == id);

            if (venue == null)
            {
                return HttpNotFound();
            }

            return View(venue);
        }
        



        // GET: ChurchVenues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchVenue churchVenue = db.ChurchVenues.Find(id);
            if (churchVenue == null)
            {
                return HttpNotFound();
            }
            return View(churchVenue);
        }

        // GET: ChurchVenues/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChurchVenues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VenueId,VenueName,VenueDescription,ImageUrl,VenueCapacity")] ChurchVenue churchVenue)
        {
            if (ModelState.IsValid)
            {
                
                db.ChurchVenues.Add(churchVenue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(churchVenue);
        }

        // GET: ChurchVenues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchVenue churchVenue = db.ChurchVenues.Find(id);
            if (churchVenue == null)
            {
                return HttpNotFound();
            }
            return View(churchVenue);
        }

        // POST: ChurchVenues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VenueId,VenueName,VenueDescription,ImageUrl,VenueCapacity")] ChurchVenue churchVenue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(churchVenue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(churchVenue);
        }

        // GET: ChurchVenues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchVenue churchVenue = db.ChurchVenues.Find(id);
            if (churchVenue == null)
            {
                return HttpNotFound();
            }
            return View(churchVenue);
        }

        // POST: ChurchVenues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChurchVenue churchVenue = db.ChurchVenues.Find(id);
            db.ChurchVenues.Remove(churchVenue);
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
