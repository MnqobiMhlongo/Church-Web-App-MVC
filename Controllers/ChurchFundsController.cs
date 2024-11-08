using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChurchSystem.Models;

namespace ChurchSystem.Controllers
{
    public class ChurchFundsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ChurchFunds
        public ActionResult Index()
        {                     
            return View(db.ChurchFunds.ToList());
        }

        // GET: ChurchFunds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchFunds churchFunds = db.ChurchFunds.Find(id);
            if (churchFunds == null)
            {
                return HttpNotFound();
            }
            return View(churchFunds);
        }

        // GET: ChurchFunds/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChurchFunds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FundId,FundName")] ChurchFunds churchFunds)
        {
            if (ModelState.IsValid)
            {
                db.ChurchFunds.Add(churchFunds);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(churchFunds);
        }

        // GET: ChurchFunds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchFunds churchFunds = db.ChurchFunds.Find(id);
            if (churchFunds == null)
            {
                return HttpNotFound();
            }
            return View(churchFunds);
        }

        // POST: ChurchFunds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FundId,FundName")] ChurchFunds churchFunds)
        {
            if (ModelState.IsValid)
            {
                db.Entry(churchFunds).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(churchFunds);
        }

        // GET: ChurchFunds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchFunds churchFunds = db.ChurchFunds.Find(id);
            if (churchFunds == null)
            {
                return HttpNotFound();
            }
            return View(churchFunds);
        }

        // POST: ChurchFunds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChurchFunds churchFunds = db.ChurchFunds.Find(id);
            db.ChurchFunds.Remove(churchFunds);
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
