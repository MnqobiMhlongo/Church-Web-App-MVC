using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ChurchSystem.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.AspNet.Identity;
using PayPal.Api;
using Microsoft.Ajax.Utilities;

namespace ChurchSystem.Controllers
{
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }
         public ActionResult EnrollUser(int SID, int CID)
        {
            StudentCourse studentCourse = new StudentCourse();

            studentCourse.StudentId = SID;
            studentCourse.CourseId = CID;
            studentCourse.enrollmentDate = DateTime.Now;
            studentCourse.completed = false;

            db.StudentCourse.Add(studentCourse);
            return View();
        }

        public ActionResult AdmminIndex()
        {
            return View(db.Courses.ToList());
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseId,CourseName,CourseDescription,estDuration,Level")] Course course)
        {
            if (ModelState.IsValid)
            {             
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseId,CourseName,CourseDescription,estDuration,Level")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }



        public ActionResult BibleStudyPortal(string Level)
        {

            var groups = db.Courses.Where(x => x.Level == Level).ToList();
            ViewBag.Level = Level;
            
            return View(groups);
        }

        public ActionResult Announcements(int id)
        {
            var AM = db.Announcements.Where(x => x.CourseId == id).ToList();
            ViewBag.AM = AM;
            return View();
        }

        public ActionResult VideoStream(int id)
        {
            var material = db.CoursesMaterial.Find(id);

           
            return View(material);
        }
        private string GenerateBlobSasUrl(string fileName)
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=rosarychurchfiles;AccountKey=BUuUsCLUGlcQCUfkjKKoZpVxFK4u6X1qr2ddFSSEAcIC1yeaBHIL9vVXVFeDX58jic/NcsVpY7dp+AStqtP6xQ==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("uploads");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            // Set the expiry time and permissions for the SAS
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1),
                Permissions = SharedAccessBlobPermissions.Read
            };

            // Generate the SAS token
            string sasBlobToken = blockBlob.GetSharedAccessSignature(sasConstraints);

            // Return the URL to the file including the SAS token
            return blockBlob.Uri + sasBlobToken;
        }

        public ActionResult PdfStream(int id)
        {
            var material = db.CoursesMaterial.Find(id);

            return View(material);
        }

        public async Task<ActionResult> AudioStream(int id)
        {
            var material = db.CoursesMaterial.Find(id);

            // Check if the material and the file URL exist
            if (material != null && !string.IsNullOrEmpty(material.FileUrl))
            {
                // Check if the URL is an absolute URL (external link)
                if (Uri.IsWellFormedUriString(material.FileUrl, UriKind.Absolute))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // Attempt to download the audio file from the URL
                        try
                        {
                            var response = await client.GetAsync(material.FileUrl);

                            // Ensure the response is successful
                            if (response.IsSuccessStatusCode)
                            {
                                var stream = await response.Content.ReadAsStreamAsync();
                                return new FileStreamResult(stream, "audio/mp3");
                            }
                            else
                            {
                                // Handle case where the file is not found or there is a network issue
                                return HttpNotFound();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions (e.g., network issues)
                            return new HttpStatusCodeResult(500, $"Error: {ex.Message}");
                        }
                    }
                }
                else
                {
                    // If it's a relative path, use the existing logic
                    var filePath = Server.MapPath(material.FileUrl);

                    if (System.IO.File.Exists(filePath))
                    {
                        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        return new FileStreamResult(stream, "audio/mp3");
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
            }
            else
            {
                // Handle material not found or missing audio file URL
                return HttpNotFound();
            }
        }

        public ActionResult GroupChat(int id)
        {
            return View();
        }
         
        public ActionResult BibleStudy(int id)
        {

            /*Student student = new Student
            {
                UserId = int.Parse(User.Identity.GetUserId())
            };

            db.Students.Add(student);
            db.SaveChanges();

            var studentid = db.Students.Where(a => a.UserId == int.Parse(User.Identity.GetUserId())).Select(a => a.StudentId).FirstOrDefault();

            StudentCourse studentCourse = new StudentCourse
            {
                StudentId = studentid,
                CourseId = id,
                enrollmentDate = DateTime.Now,
                completed = false
            };

           db.StudentCourse.Add(studentCourse);
           db.SaveChanges();*/

            string student = User.Identity.GetUserName();
            List<StudentQuizResult> allattempts = db.StudentQuizResult.Where(c=>c.user == student).ToList(); 
            int allPassed = db.StudentQuizResult.Where(b => b.Passed == "true").Count();
            int allQuizzes = db.Quizzes.Where(a => a.CourseId == id).Count();
            int passedattempts = 0;

            foreach (var item in allattempts.Where(m=>m.Passed == "true"))
            {
                passedattempts++;
            } 

            if(id == 9)
            {
                ViewBag.comp = true;
            }

            var lvl = db.Courses.Where(x => x.CourseId == id).Select(x=>x.Level).FirstOrDefault();
            var AM = db.Announcements.Where(x => x.CourseId == id).ToList();
            var CM = db.CoursesMaterial.Where(x => x.CourseId == id).ToList();
            var AMC = db.Announcements.Where(x => x.CourseId == id).Count();
            var N = db.Courses.Where(x => x.CourseId == id).Select(v=> v.CourseName).FirstOrDefault();
            var qz = db.Quizzes.Where(a=> a.CourseId == id).ToList();

            ViewBag.CID = id;
            ViewBag.CM = CM;
            ViewBag.AM = AM;
            ViewBag.AMC = AMC;
            ViewBag.NM = N; 
            ViewBag.qz = qz;  
            ViewBag.lvl = lvl;

            return View();
        }





        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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
