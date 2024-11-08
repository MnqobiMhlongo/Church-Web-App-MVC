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
using System.IO;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ChurchSystem.Migrations;

namespace ChurchSystem.Controllers
{
    public class ItemDonationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ItemDonations
        public ActionResult Index()
        {
            return View(db.ItemDonations.ToList());
        }

        // GET: ItemDonations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemDonations itemDonations = db.ItemDonations.Find(id);
            if (itemDonations == null)
            {
                return HttpNotFound();
            }
            return View(itemDonations);
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

        // GET: ItemDonations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemDonations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DonationId,DonorName,Item,itemDescription,Quantity,ImageUrl,Price")] ItemDonations itemDonations)
        {
            if (ModelState.IsValid)
            {
                /* if (!string.IsNullOrEmpty(ImageData))
                 {
                     byte[] imageBytes = Convert.FromBase64String(ImageData);

                     CloudBlobContainer container = GetCloudBlobContainer();
                     string filename = Guid.NewGuid().ToString() + ".jpg"; // Change the extension as needed
                     CloudBlockBlob blob = container.GetBlockBlobReference(filename);
                     blob.Properties.ContentType = "image/jpeg"; // Change the content type as needed

                     using (MemoryStream stream = new MemoryStream(imageBytes))
                     {
                         blob.UploadFromStream(stream);
                     }

                     itemDonations.ImageUrl = blob.Uri.ToString();
                 */

                HttpPostedFileBase file = Request.Files["ImageFile"];
              
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
                    itemDonations.ImageUrl = base64String;
                }



                // Assuming you have a UserManager set up, you can get the current user's information
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var user = userManager.FindById(User.Identity.GetUserId());

                if (user != null)
                {
                    string fullName = user.NameSurname; // Assuming your user model has a property for the full name
                    itemDonations.DonorName = fullName;
                }

                db.ItemDonations.Add(itemDonations);
                db.SaveChanges();
                return RedirectToAction("Index", new { successMessage = "Item Added To Marketplace" });
            }

            return View(itemDonations);
        }

        // GET: ItemDonations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemDonations itemDonations = db.ItemDonations.Find(id);
            if (itemDonations == null)
            {
                return HttpNotFound();
            }
            return View(itemDonations);
        }

        // POST: ItemDonations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DonationId,DonorName,Item,itemDescription,Quantity,ImageUrl")] ItemDonations itemDonations)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemDonations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(itemDonations);
        }

        // GET: ItemDonations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemDonations itemDonations = db.ItemDonations.Find(id);
            if (itemDonations == null)
            {
                return HttpNotFound();
            }
            return View(itemDonations);
        }

        // POST: ItemDonations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemDonations itemDonations = db.ItemDonations.Find(id);
            db.ItemDonations.Remove(itemDonations);
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






        [Authorize(Roles = "admin")]
        public ActionResult G()
        {
            return View(db.Users.ToList());
        }

    }
}
