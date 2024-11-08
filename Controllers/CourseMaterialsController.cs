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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ChurchSystem.Controllers
{
    public class CourseMaterialsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CourseMaterials
        public ActionResult Index()
        {
            var coursesMaterial = db.CoursesMaterial.Include(c => c.Course);
            return View(coursesMaterial.ToList());
        }

        public ActionResult CourseMaterial(int CourseId)
        {
            var CM = db.CoursesMaterial.Where(x => x.CourseId== CourseId).ToList();
            ViewBag.CID = CourseId;
            return View(CM);
        }


        public ActionResult UserIndex(int CId)
        {
            var coursesMaterial = db.CoursesMaterial.Include(c => c.Course).Where(c => c.CourseId ==  CId);
            return View(coursesMaterial.ToList());
        }


        // GET: CourseMaterials/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseMaterial courseMaterial = db.CoursesMaterial.Find(id);
            if (courseMaterial == null)
            {
                return HttpNotFound();
            }
            return View(courseMaterial);
        }


        //Still OMW
        private CloudBlobContainer GetCloudBlobContainer()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=rosarychurchfiles;AccountKey=BUuUsCLUGlcQCUfkjKKoZpVxFK4u6X1qr2ddFSSEAcIC1yeaBHIL9vVXVFeDX58jic/NcsVpY7dp+AStqtP6xQ==;EndpointSuffix=core.windows.net";
            string containerName = "uploads";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            return container;
        }

        // GET: CourseMaterials/Create
        public ActionResult Create(int CourseId)
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            return View();
        }


        // POST: CourseMaterials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaterialId,CourseId,Name,DatePosted,Description,FileType,FileUrl")] CourseMaterial courseMaterial)
        {
            if (ModelState.IsValid)
            {
                //SiyayaPhambili
                //Anytime you need a friend
                HttpPostedFileBase file = Request.Files["ImageFile"];
                if (file != null && file.ContentLength > 0)
                {
                    CloudBlobContainer container = GetCloudBlobContainer();
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    CloudBlockBlob blob = container.GetBlockBlobReference(filename);
                    blob.Properties.ContentType = file.ContentType;
                    using (Stream stream = file.InputStream)
                    {
                        blob.UploadFromStream(stream);
                    }
                    courseMaterial.FileUrl = blob.Uri.ToString();
                }

                courseMaterial.DatePosted = DateTime.Now;   
                db.CoursesMaterial.Add(courseMaterial);
                db.SaveChanges();
                return RedirectToAction("CourseMaterial", new { CourseId = courseMaterial.CourseId, successMessage = "Uploaded ;)" });
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", courseMaterial.CourseId);
            return View(courseMaterial);
        }

        // GET: CourseMaterials/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseMaterial courseMaterial = db.CoursesMaterial.Find(id);
            if (courseMaterial == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", courseMaterial.CourseId);
            return View(courseMaterial);
        }

        // POST: CourseMaterials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaterialId,CourseId,Name,DatePosted,Description,FileType,FileUrl")] CourseMaterial courseMaterial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseMaterial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", courseMaterial.CourseId);
            return View(courseMaterial);
        }

        // GET: CourseMaterials/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseMaterial courseMaterial = db.CoursesMaterial.Find(id);
            if (courseMaterial == null)
            {
                return HttpNotFound();
            }
            return View(courseMaterial);
        }

        // POST: CourseMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseMaterial courseMaterial = db.CoursesMaterial.Find(id);
            db.CoursesMaterial.Remove(courseMaterial);
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
