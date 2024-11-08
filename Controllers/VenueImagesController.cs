using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChurchSystem.Models;

namespace ChurchSystem.Controllers
{
    public class VenueImagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VenueImages
        public ActionResult Index()
        {
            var venueImages = db.VenueImages.Include(v => v.ChurchVenue);
            return View(venueImages.ToList());
        }

        // GET: VenueImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueImages venueImages = db.VenueImages.Find(id);
            if (venueImages == null)
            {
                return HttpNotFound();
            }
            return View(venueImages);
        }

        // GET: VenueImages/Create
        public ActionResult Create()
        {
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName");
            return View();
        }

        // POST: VenueImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ImageId,VenueId,Imagename,ImageUrl")] VenueImages venueImages)
        {
            if (ModelState.IsValid)
            {
                //cici-iqiniso
                HttpPostedFileBase file = Request.Files["ImageFile"];
                //Simmy
                if (file != null && file.ContentLength > 0)
                {
                    // Read the file content into a byte array
                    byte[] fileBytes;
                    using (BinaryReader reader = new BinaryReader(file.InputStream))
                    {
                        fileBytes = reader.ReadBytes(file.ContentLength);
                    }

                    // Convert the byte array to a Base64 string
                    string base64String = Convert.ToBase64String(fileBytes);

                    // Set the ImageData property to the Base64 string
                    venueImages.ImageUrl = base64String;
                }

                db.VenueImages.Add(venueImages);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", venueImages.VenueId);
            return View(venueImages);
        }

        // GET: VenueImages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueImages venueImages = db.VenueImages.Find(id);
            if (venueImages == null)
            {
                return HttpNotFound();
            }
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", venueImages.VenueId);
            return View(venueImages);
        }

        // POST: VenueImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ImageId,VenueId,ImageUrl")] VenueImages venueImages)
        {
            if (ModelState.IsValid)
            {
                db.Entry(venueImages).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VenueId = new SelectList(db.ChurchVenues, "VenueId", "VenueName", venueImages.VenueId);
            return View(venueImages);
        }

        // GET: VenueImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VenueImages venueImages = db.VenueImages.Find(id);
            if (venueImages == null)
            {
                return HttpNotFound();
            }
            return View(venueImages);
        }

        // POST: VenueImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VenueImages venueImages = db.VenueImages.Find(id);
            db.VenueImages.Remove(venueImages);
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
