using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChurchSystem.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using ChurchSystem.Migrations;
using System.IO;
using Microsoft.AspNet.Identity;

namespace ChurchSystem.Controllers
{
    public class OnlineSermonsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OnlineSermons
        public ActionResult Index()
        {
            return View(db.OnlineSermons.ToList());
        }

        [Authorize]
        public ActionResult UserIndex()
        {
            return View(db.OnlineSermons.ToList());
        }

        public ActionResult AudioStream(int? id)
        {
            var sermon = db.OnlineSermons.Include(s => s.Comments).FirstOrDefault(s => s.SermonId == id);
                
        

            // Pass the URL of the audio file to the view
            return View(sermon);
        }


        public ActionResult AudioStreams(int? id)
        {
            var sermon = db.OnlineSermons.Find(id);

            // Assuming the audio file path is stored in the FileUrl property of the sermon model
            if (sermon != null && !string.IsNullOrEmpty(sermon.FileUrl))
            {
                // Get the file path (replace this with your logic to get the file path)
                var filePath = Server.MapPath(sermon.FileUrl); // Assuming FileUrl is a relative path

                if (System.IO.File.Exists(filePath))
                {
                    // Open the audio file as a stream
                    var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                    // Return the audio file as a FileStreamResult
                    return new FileStreamResult(stream, "audio/mp3");
                }
                else
                {
                    // Handle file not found error
                    return HttpNotFound();
                }
            }
            else
            {
                // Handle sermon not found or missing audio file URL
                return HttpNotFound();
            }
        }


        public ActionResult VideoStream(int? id)
        {
            var sermon = db.OnlineSermons.Find(id);

            // Pass the URL of the audio file to the view
            return View(sermon);
        }

        public ActionResult GetPdf()
        {
            // Replace "yourBlobUrl" with the URL of your PDF file
            var pdfFileStream = new WebClient().OpenRead("https://churchfiles.blob.core.windows.net/churchuploads/wtc.docx");

            // Set the content type to application/pdf
            Response.ContentType = "application/pdf";

            // Set the content disposition to inline
            Response.AddHeader("Content-Disposition", "inline; filename=yourPdfFileName.pdf");

            // Return the file content
            return File(pdfFileStream, "application/pdf");
        }

        public ActionResult SermonNotes(int? id)
        {
            var sermon = db.OnlineSermons.Find(id);

            // Pass the URL of the audio file to the view
            return View(sermon);
        }




        // GET: OnlineSermons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineSermons onlineSermons = db.OnlineSermons.Find(id);
            if (onlineSermons == null)
            {
                return HttpNotFound();
            }
            return View(onlineSermons);
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


        // GET: OnlineSermons/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OnlineSermons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SermonId,SermonTitle,SermonDescription,FileType,FileUrl")] OnlineSermons onlineSermons)
        {
            if (ModelState.IsValid)
            {
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
                    onlineSermons.FileUrl = blob.Uri.ToString();
                }

                db.OnlineSermons.Add(onlineSermons);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(onlineSermons);
        }

        // GET: OnlineSermons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineSermons onlineSermons = db.OnlineSermons.Find(id);
            if (onlineSermons == null)
            {
                return HttpNotFound();
            }
            return View(onlineSermons);
        }

        // POST: OnlineSermons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SermonId,SermonTitle,SermonDescription,FileType,FileUrl")] OnlineSermons onlineSermons)
        {
            if (ModelState.IsValid)
            {
                db.Entry(onlineSermons).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(onlineSermons);
        }

        // GET: OnlineSermons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineSermons onlineSermons = db.OnlineSermons.Find(id);
            if (onlineSermons == null)
            {
                return HttpNotFound();
            }
            return View(onlineSermons);
        }

        // POST: OnlineSermons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OnlineSermons onlineSermons = db.OnlineSermons.Find(id);
            db.OnlineSermons.Remove(onlineSermons);
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

        [HttpPost]
        public ActionResult AudioAddComment(Comment comment)
        {
            if (ModelState.IsValid)
            {

                comment.Timestamp = DateTime.Now;
                comment.UserName = User.Identity.GetUserName();

                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("AudioStream", new { id = comment.SermonId });
            }
            // If the model state is not valid, return back to the sermon details view
            var sermon = db.OnlineSermons.Include(s => s.Comments).FirstOrDefault(s => s.SermonId == comment.SermonId);
            return View("Details", sermon);
        }

        [HttpPost]
        public ActionResult VideoAddComment(Comment comment)
        {
            if (ModelState.IsValid)
            {

                comment.Timestamp = DateTime.Now;
                comment.UserName = User.Identity.GetUserName();

                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("VideoStream", new { id = comment.SermonId });
            }
            // If the model state is not valid, return back to the sermon details view
            var sermon = db.OnlineSermons.Include(s => s.Comments).FirstOrDefault(s => s.SermonId == comment.SermonId);
            return View("Details", sermon);
        }

        [HttpPost]
        public ActionResult NotesAddComment(Comment comment)
        {
            if (ModelState.IsValid)
            {

                comment.Timestamp = DateTime.Now;
                comment.UserName = User.Identity.GetUserName();

                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("SermonNotes", new { id = comment.SermonId });
            }
            // If the model state is not valid, return back to the sermon details view
            var sermon = db.OnlineSermons.Include(s => s.Comments).FirstOrDefault(s => s.SermonId == comment.SermonId);
            return View("Details", sermon);
        }


    }
}
