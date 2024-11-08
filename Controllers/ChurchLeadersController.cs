using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ChurchSystem.Migrations;
using ChurchSystem.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ChurchSystem.Controllers
{
    public class ChurchLeadersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ChurchLeaders
        public ActionResult Index()
        {
            return View(db.ChurchLeaders.ToList());
        }

        // GET: ChurchLeaders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchLeaders churchLeaders = db.ChurchLeaders.Find(id);
            if (churchLeaders == null)
            {
                return HttpNotFound();
            }
            return View(churchLeaders);
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=peopleintransitimages;AccountKey=Z2XXwa4RsEK7/lUpSk43c8HXKrkT6rhnusH4WLmb1o+XOGjRhJODmeKT75UGH3d/DfzIyl21SWQa+ASt/B8fIA==;EndpointSuffix=core.windows.net";
            string containerName = "peopleintransitimages";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            return container;
        }

      

        // GET: ChurchLeaders/Create
        public ActionResult Create()
        {

            //List<DateTime> bookedTimes = GetBookedTimes(id, bookingD);

            // Pass the booked times to the view
           // ViewBag.BookedTimes = bookedTimes;

            return View();
        }

        // POST: ChurchLeaders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "leaderId,LeaderName,Role,ImageUrl,Email,PhoneNumber")] ChurchLeaders churchLeaders, string ImageData)
        {
            if (ModelState.IsValid)
            {
               HttpPostedFileBase file = Request.Files["ImageFile"];

                /* if (file != null && file.ContentLength > 0)
                 {
                     CloudBlobContainer container = GetCloudBlobContainer();
                     string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                     CloudBlockBlob blob = container.GetBlockBlobReference(filename);
                     blob.Properties.ContentType = file.ContentType;
                     using (Stream stream = file.InputStream)
                     {
                         blob.UploadFromStream(stream);
                     }
                     churchLeaders.ImageUrl = blob.Uri.ToString();
                 }*/

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
                    churchLeaders.ImageUrl = base64String;
                }



                db.ChurchLeaders.Add(churchLeaders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(churchLeaders);
        }

        // GET: ChurchLeaders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchLeaders churchLeaders = db.ChurchLeaders.Find(id);
            if (churchLeaders == null)
            {
                return HttpNotFound();
            }
            return View(churchLeaders);
        }

        // POST: ChurchLeaders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "leaderId,LeaderName,Role,ImageUrl,Email,PhoneNumber")] ChurchLeaders churchLeaders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(churchLeaders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(churchLeaders);
        }

        // GET: ChurchLeaders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChurchLeaders churchLeaders = db.ChurchLeaders.Find(id);
            if (churchLeaders == null)
            {
                return HttpNotFound();
            }
            return View(churchLeaders);
        }

        // POST: ChurchLeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChurchLeaders churchLeaders = db.ChurchLeaders.Find(id);
            db.ChurchLeaders.Remove(churchLeaders);
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
