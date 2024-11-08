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
    public class EventDecorLayOutsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EventDecorLayOuts
        public ActionResult Index()
        {
            return View(db.EventDecorLayOuts.ToList());
        }

        // GET: EventDecorLayOuts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventDecorLayOuts eventDecorLayOuts = db.EventDecorLayOuts.Find(id);
            if (eventDecorLayOuts == null)
            {
                return HttpNotFound();
            }
            return View(eventDecorLayOuts);
        }

        // GET: EventDecorLayOuts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventDecorLayOuts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Layoutid,layoutName,layoutImageUrl")] EventDecorLayOuts eventDecorLayOuts)
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
                    eventDecorLayOuts.layoutImageUrl = base64String;
                }

                db.EventDecorLayOuts.Add(eventDecorLayOuts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eventDecorLayOuts);
        }

        // GET: EventDecorLayOuts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventDecorLayOuts eventDecorLayOuts = db.EventDecorLayOuts.Find(id);
            if (eventDecorLayOuts == null)
            {
                return HttpNotFound();
            }
            return View(eventDecorLayOuts);
        }

        // POST: EventDecorLayOuts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Layoutid,layoutName,layoutImageUrl")] EventDecorLayOuts eventDecorLayOuts)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventDecorLayOuts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eventDecorLayOuts);
        }

        // GET: EventDecorLayOuts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventDecorLayOuts eventDecorLayOuts = db.EventDecorLayOuts.Find(id);
            if (eventDecorLayOuts == null)
            {
                return HttpNotFound();
            }
            return View(eventDecorLayOuts);
        }

        // POST: EventDecorLayOuts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventDecorLayOuts eventDecorLayOuts = db.EventDecorLayOuts.Find(id);
            db.EventDecorLayOuts.Remove(eventDecorLayOuts);
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
