using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using ChurchSystem.Models;
using Microsoft.AspNet.Identity;

namespace ChurchSystem.Controllers
{
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult BookLoanOverview()
        {
            SendDueReminder();
            UpdateOverdueFines();
            RemoveExpiredReservations();
            var allBorrowedBooks = db.bookLoan
                .Where(a=>a.status == "Waiting Collection" || a.status == "Book Collected")
                .OrderByDescending(b => b.reservedate)
                                   .Include(b => b.Books).ToList();
            var awaitingCollection = db.bookLoan.Where(a => a.status == "Waiting Collection")
                .OrderByDescending(b => b.reservedate).Include(b => b.Books).ToList();
            var awaitingReturn = db.bookLoan.Where(a => a.status == "Book Collected" || a.status == "Overdue").OrderByDescending(b => b.reservedate).Include(b => b.Books).ToList();db.bookLoan.Where(a => a.status == "Book Collected").Include(b => b.Books).ToList();

            var model = new BookLoanOverviewViewModel
            {
                AllBorrowedBooks = allBorrowedBooks,
                AwaitingCollection = awaitingCollection,
                AwaitingReturn = awaitingReturn
            };

            return View(model);
        }
        

        public ActionResult AdminIndex()
        {          
            SendDueReminder();
            UpdateOverdueFines();
            RemoveExpiredReservations();
            return View(db.books.ToList());
        }



        // GET: Books
        public ActionResult Index()
        {
            SendDueReminder();
            UpdateOverdueFines();
            RemoveExpiredReservations();
            return View(db.books.ToList());
        }

        // GET: Books/Details/5
        //[Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookId,Title,Description,Author,Genre,Status,imageurl,published")] Books books)
        {
            if (ModelState.IsValid)
            {
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
                    books.imageurl = base64String;
                }

                
                books.Status = "Available";

                db.books.Add(books);
                db.SaveChanges();
                return RedirectToAction("AdminIndex", new { successMessage = "Book Added To Collection" });
            }

            return View(books);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookId,Title,Description,Author,Genre,Status,imageurl,published")] Books books)
        {
            if (ModelState.IsValid)
            {
                db.Entry(books).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(books);
        }

        public void SendDueReminder()
        {
            DateTime td = DateTime.Today;

            DateTime startOfDay = DateTime.Today; // 00:00:00
            DateTime endOfDay = startOfDay.AddDays(1); // 00:00:00 of the next day

            List<BookLoan> dueToday = db.bookLoan
                .Where(a => a.expectedReturn >= startOfDay && a.expectedReturn < endOfDay)
                .ToList();

          


            foreach (var item in dueToday) 
            {

                Books bk = db.books.Find(item.BookId);

                var usermail = item.username;
                MailMessage mail = new MailMessage();
                mail.To.Add(usermail);
                mail.From = new MailAddress("GRACEOFGODCHURCH@gmail.com");
                mail.Subject = "Book Return Reminder: " + bk.Title;
                string Body = "This is a reminder that the book: " + bk.Title + " is due today. Kindly ensure it is returned promptly to avoid incurring any late fees.";

                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("graceofgodchurch0@gmail.com", "wwiaiouwesmobmrd"); // Enter seders User name and password   
                smtp.EnableSsl = true;
                smtp.Send(mail);

               /* //smsNow
                string number = "0634032279";
                string myURI = "https://api.bulksms.com/v1/messages";
                string myUsername = "guardianangels2024";
                string myPassword = "Bhebhe@2002";
                string myData = $"{{to: \"{number}\", body:\"{Body}\"}}";

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
                }*/
            }
        }



        //RIPYOUNG-ZAY
        public ActionResult AddToWishList(int id)
        {
            BookWishList bookWishList = new BookWishList();

            bookWishList.username = User.Identity.GetUserName();
            bookWishList.bookid = id;
            db.bookWishList.Add(bookWishList);
            db.SaveChanges();

            return RedirectToAction("Index", new { successMessage = "Added To WishList" });
        }

        public ActionResult AwaitingCollection()
        {
            return View(db.bookLoan.Where(a=>a.status == "Waiting Collection").Include(b=>b.Books).ToList());
        }

        public ActionResult AwaitingReturn()
        {
            return View(db.bookLoan.Where(a => a.status == "Book Collected").Include(b => b.Books).ToList());
        }

        public ActionResult AdminLoanDetails(int id)
        {

            var bookLoan = db.bookLoan
  .Include(b => b.Books)

  .FirstOrDefault(bl => bl.bookloanId == id);

            return View(bookLoan);
        }

        public void UpdateOverdueFines()
        {
            // Get the current date and time
            DateTime currentDate = DateTime.Now;

            // Find all loans that are overdue and have not been returned
            var overdueLoans = db.bookLoan
                .Where(bl => bl.signedout == true // Check if the book was signed out
                              && bl.status == "Book Collected"// Not yet returned
                             && currentDate > bl.expectedReturn) // Past the expected return date
                .ToList();

            // Loop through the overdue loans and calculate the fine
            foreach (var loan in overdueLoans)
            {
                // Calculate the number of overdue days
                int overdueDays = (currentDate - loan.expectedReturn).Days;

                // Update the book fine (assuming it's 10 per day)
                loan.Bookfine = overdueDays * 10;

                // Optionally, update the status to indicate it's overdue
                loan.status = "Overdue";
            }

            // Save the changes to the database
            db.SaveChanges();
        }

        public void RemoveExpiredReservations()
        {
            // Get the current date and time
            DateTime currentDate = DateTime.Now;

            // Find all records where status is "Waiting Collection" and two days have passed since reservedate
            var expiredLoans = db.bookLoan
                .Where(bl => bl.status == "Waiting Collection"
                             && DbFunctions.DiffDays(bl.reservedate, currentDate) >= 2)
                .ToList();

            // Remove the records from the database
            if (expiredLoans.Any())
            {
                db.bookLoan.RemoveRange(expiredLoans);
                db.SaveChanges();
            }
        }


        public ActionResult extendrequest(int bookloanId, DateTime newDate)
        {
            BookLoan bookLoanRequest = db.bookLoan.Where(a => a.bookloanId == bookloanId).FirstOrDefault();
            int bookWishListCount = db.bookWishList.Where(w => w.bookid == bookLoanRequest.BookId).Count();

            if (bookWishListCount > 0)
            {
                return Json(new { redirectUrl = Url.Action("BooksReserved", new { errorMessage = "Your request is denied as this book is also on another user's wishlist." }) });
            }
            else
            {
                bookLoanRequest.expectedReturn = newDate;
                db.SaveChanges();
                return Json(new { redirectUrl = Url.Action("BooksReserved", new { successMessage = "Extension Approved. Due Date is Now "+newDate.ToString("d MMMM") }) });
            }
        }


        public ActionResult AllBorrowedBooks()
        {
            //CelineDion-calltheman
            UpdateOverdueFines();
            RemoveExpiredReservations();
            var books = db.bookLoan.Where(a => a.status == "Book Collected" || a.status == "Book Delivered" || a.status == "Overdue")
                                   .Include(b => b.Books).ToList();
            return PartialView("_AllBorrowedBooks", books);
        }

        public ActionResult LibHub()
        {
            return View();
        }


        public ActionResult SignCollect(int bookLoanId, string signature)
        {
            BookLoan bookLoan = db.bookLoan.Find(bookLoanId);
            signature = signature.Split(',')[1]; // Extract the Base64 part
            bookLoan.signedout = true;
            bookLoan.signedDate = DateTime.Now;
            bookLoan.signoutImageUrl = signature;
            bookLoan.status = "Book Collected";

            db.SaveChanges();

            // Return a JSON result with the redirect URL
            return Json(new { redirectUrl = Url.Action("BookLoanOverview", new { successMessage = "Book Collection Signed. Now Awaiting Return" }) });
        }

        public ActionResult SignReturn(int bookLoanId, string signature)
        {
            BookLoan bookLoan = db.bookLoan.Find(bookLoanId);
            signature = signature.Split(',')[1];
            bookLoan.signreturnImageUrl = signature;
            bookLoan.actualReturn = DateTime.Now;
            if (bookLoan.Bookfine > 0)
            {
                bookLoan.status = "Late Return";
            }
            else
            {
                bookLoan.status = "Book Returned";
            }

            Books books = db.books.Find(bookLoan.BookId);
            books.Status = "Available";
            
            foreach(var item in db.bookWishList.Where(a=>a.bookid == bookLoan.BookId))
            {
                notifyWishlistItem(item.bookid, item.username);
            }
            db.SaveChanges();
            //db.savechanges()
            return Json(new { redirectUrl = Url.Action("BookLoanOverview", new { successMessage = "Book Return Signed." }) });
        }



       



        public ActionResult Reserve(int id, string handover, DateTime returndateTime)
        {
            BookLoan bl= new BookLoan();

            bl.username = User.Identity.GetUserName();
            bl.BookId = id;
            bl.handover = handover;
            bl.signedout = false;
            bl.expectedReturn = returndateTime;
            //bl.actualReturn = DateTime.Now;
            //bl.signedDate = DateTime.Now;
            bl.actualReturn = returndateTime;
            bl.Bookfine = 0;
            bl.reservedate = DateTime.Now;
            bl.status = "Waiting Collection";
            Books books = db.books.Find(id);

            books.Status = "Unavailable";


            //ALLCAPSMFDOOM
            var usermail = User.Identity.Name;

            MailMessage mail = new MailMessage();
            mail.To.Add(usermail);
            mail.From = new MailAddress("GRACEOFGODCHURCH@gmail.com");
            mail.Subject = "Book Reserved( " + handover +" )";
            string Body = "Book: " + bl.Books + " has been reserved and is prepared for " + handover + "<br>" +
                          "Please collect it within the next 2 days." + "<br>" +
                          "View Details Under Your Email On Our Site";// Example with a new line

            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("graceofgodchurch0@gmail.com", "wwiaiouwesmobmrd"); // Enter seders User name and password   
            smtp.EnableSsl = true;
            smtp.Send(mail);

            db.bookLoan.Add(bl);
            db.SaveChanges();
            return RedirectToAction("ReservationDetails", new { id = bl.bookloanId, successMessage = "Book Reserved" });
        }



        public void notifyWishlistItem(int id,string usermail)
        {
            Books book = db.books.Find(id);

           
            MailMessage mail = new MailMessage();
            mail.To.Add(usermail);
            mail.From = new MailAddress("GRACEOFGODCHURCH@gmail.com");
            mail.Subject = "WishList Item( " + book.Title + ") has returned" ;
            string Body = " The Book '" + book.Title + "' has been returned. Hurry now to reserve a copy at the library on our site!";

            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("graceofgodchurch0@gmail.com", "wwiaiouwesmobmrd"); // Enter seders User name and password   
            smtp.EnableSsl = true;
            smtp.Send(mail);

            //sms
            string number = "0634032279";
            string myURI = "https://api.bulksms.com/v1/messages";
            string myUsername = "guardianangels2024";
            string myPassword = "Bhebhe@2002";
            string myData = $"{{to: \"{number}\", body:\"{Body}\"}}";

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


        public ActionResult BooksReserved()
        {
            var sortedBooks = db.bookLoan
                     .Include(b => b.Books)
                     .OrderByDescending(b => b.reservedate)
                     .ToList();
            return View(sortedBooks);
        }

        public ActionResult ReservationDetails(int id)
        {
            var bookLoan = db.bookLoan
    .Include(b => b.Books)
    
    .FirstOrDefault(bl => bl.bookloanId == id);

            return View(bookLoan);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Books books = db.books.Find(id);
            db.books.Remove(books);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize]
        public ActionResult WishList()
        {
            var user = User.Identity.GetUserName();

            List<int> wishListBookIds = db.bookWishList.Select(w => w.bookid).ToList();
        
            List<Books> books = db.books.Where(b => wishListBookIds.Contains(b.BookId)).ToList();

            return View(books);
        }

        public void RemoveFromList(int id)
        {
            BookWishList wishList = db.bookWishList.Where(a => a.bookid == id).FirstOrDefault();

            if (wishList != null)
            {
                db.bookWishList.Remove(wishList);
                db.SaveChanges();
            }
        }

        public ActionResult Remove(int id)
        {
            BookWishList wishList = db.bookWishList.Where(a => a.bookid == id).FirstOrDefault();
            db.bookWishList.Remove(wishList);
            db.SaveChanges();
            return RedirectToAction("WishList", "Books", new { successMessage = "Book Removed From WishList" });
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
