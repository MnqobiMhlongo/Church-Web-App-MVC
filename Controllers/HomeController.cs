using ChurchSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Text;
using ZXing.OneD.RSS;
using Microsoft.AspNet.Identity;
using System.Web.WebPages;
using ChurchSystem.Migrations;



namespace ChurchSystem.Controllers
{
    public class HomeController : Controller
    {

        //Shai-IfIEver
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {

           

            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult AdminIndex()
        {
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult Church()
        {
            return View();
        }

        [Authorize]
        public ActionResult IOF()
        {
            return View();  
        }



        public void RemindSms()
        {
            var EventTimes = db.Events.ToList();
            var atts = db.Attendance.ToList();



            foreach(var rsvp in atts)
            {
                var EventTime = db.Events.ToList();
                var users = db.Users.ToList();


            }
        }


        public void SendSms()
        {
            //DrDre-LetMeRide
            // This URL is used for sending messages
            string myURI = "https://api.bulksms.com/v1/messages";

            // change these values to match your own account
            string myUsername = "icebear2002";
            string myPassword = "Bhebhe@2002";

            // the details of the message we want to send
            string myData = "{to: \"27634032279\", body:\"TESTINGHANGFIRE!\"}";

            // build the request based on the supplied settings
            var request = WebRequest.Create(myURI);

            // supply the credentials
            request.Credentials = new NetworkCredential(myUsername, myPassword);
            request.PreAuthenticate = true;
            // we want to use HTTP POST
            request.Method = "POST";
            // for this API, the type must always be JSON
            request.ContentType = "application/json";

            // Here we use Unicode encoding, but ASCIIEncoding would also work
            var encoding = new UnicodeEncoding();
            var encodedData = encoding.GetBytes(myData);

            // Write the data to the request stream
            var stream = request.GetRequestStream();
            stream.Write(encodedData, 0, encodedData.Length);
            stream.Close();

            // try ... catch to handle errors nicely
            try
            {
                // make the call to the API
                var response = request.GetResponse();

                // read the response and print it to the console
                var reader = new StreamReader(response.GetResponseStream());
                Console.WriteLine(reader.ReadToEnd());
            }
            catch (WebException ex)
            {
                // show the general message
                Console.WriteLine("An error occurred:" + ex.Message);

                // print the detail that comes with the error
                var reader = new StreamReader(ex.Response.GetResponseStream());
                Console.WriteLine("Error details:" + reader.ReadToEnd());
            }

        }



        //WhatMyNameSnoopDog
        [HttpPost]
        public void SendEventReminders()
        {
            // Get events starting in the next 2 minutes
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddMinutes(2);

            var upcomingEvents = db.Events
                .Where(e => e.dateTime > startTime && e.dateTime <= endTime)
                .ToList();

            foreach (var eventItem in upcomingEvents)
            {
                var attendees = db.Attendance
                    .Where(a => a.EventId == eventItem.EventId)
                    .Select(a => new { a.Attandee, a.Events.Title })
                    .ToList();

                foreach (var attendee in attendees)
                {
                    var user = db.Users.FirstOrDefault(u => u.UserName == attendee.Attandee);
                    if (user != null)
                    {
                        SendSms(user.PhoneNumber, $"Grace Of God Church Event:\nEvent: {attendee.Title} will be starting in a few minutes. Please make sure to use your Invitation QR CODE to mark your attendance.");
                    }
                }
            }
        }

        private void SendSms(string phoneNumber, string message)
        {
            string myURI = "https://api.bulksms.com/v1/messages";
            string myUsername = "andilembatha";
            string myPassword = "Password@01";
            string myData = $"{{to: \"{phoneNumber}\", body:\"{message}\"}}";

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


        public ActionResult HRC()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddMinutes(2);

        /*    var upcomingEvents = db.Events
                .Where(e => e.dateTime > startTime && e.dateTime <= endTime)
                .ToList();*/


            //SendSms("27634032279", $"Reminder: The event TESTING is starting soon!");
            return View();
        }

        [Authorize]
        public ActionResult AdminHRC()
        {

            DateTime today = DateTime.Today;
            var DonationsCount = db.Donations
                                  .Where(d => d.DonationDate.Year == today.Year &&
                                              d.DonationDate.Month == today.Month &&
                                              d.DonationDate.Day == today.Day)
                                  .Count();


            ViewBag.DonationsCount = DonationsCount;
            return View();
        }

        //[Authorize]
        public ActionResult Admin()
        {
            DateTime today = DateTime.Today;
            var DonationsCount = db.Donations
                                  .Where(d => d.DonationDate.Year == today.Year &&
                                              d.DonationDate.Month == today.Month &&
                                              d.DonationDate.Day == today.Day)
                                  .Count();


            ViewBag.DonationsCount = DonationsCount;
            return View();
        }

        public ActionResult Popup()
        {

            var events = db.Events.ToList();
            var rate = db.Events.Where(e => e.Status == "Finished").ToList();
            var bookings = db.VenueBookingEvent.Where(e => e.Paid == true).ToList();


            List<(int eventId, string invite,string posted)> invite = new List<(int, string, string posted)>();

            foreach(var item in events)
            {
                string text = item.Organizer + " Has Invited You To Our " + item.Title + " On " + item.dateTime.ToString("dd MMMM yyyy HH:mm");
                string posted = item.DatePosted.ToString("dd MMMM yyyy HH:mm ");

                invite.Add((item.EventId,text,posted));

            }


            List<(int eventId, string invite )> ratinginvite = new List<(int, string)>();

            foreach (var item in rate)
            {
                //string text = item.Organizer + " Has Invited You To Our " + item.Title + " On " + item.dateTime.ToString("dd MMMM yyyy HH:mm");
                string text = "Please Rate and Comment On Our Event: " + item.Title;
                //string posted = item.DatePosted.ToString("dd MMMM yyyy HH:mm ");

                ratinginvite.Add((item.EventId, text));

            }


            List<(int id, string invite)> VenueConfirmation = new List<(int id, string invite)>();

            foreach(var item in  bookings)
            {
                string text = "Your Venue Booking Confirmation";
                VenueConfirmation.Add((item.VenueEventId, text));
            }

            ViewBag.VC = VenueConfirmation;
            ViewBag.PR = ratinginvite;
            ViewBag.Invites = invite;
            return View();
        }


        public ActionResult NewStudent()
        {
            Student student = new Student();

            student.UserId = int.Parse(User.Identity.GetUserId()); ;
            return View();
        }


        

        [HttpPost]
        public ActionResult SaveImage(images model)
        {
            // Convert Base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(model.imagedata.Replace("data:image/png;base64,", ""));
            string base64WithoutPrefix = model.imagedata.Replace("data:image/png;base64,", "");


            int count = db.images.Count() + 1;

            // Save the image to the database
            var image = new images
            {
                imagedata = base64WithoutPrefix,
                name = "Student " + count
            };

            
            db.images.Add(image);
            db.SaveChanges();

            return Json(new { success = true, imageId = image.Imageid });
        }


        //ALGreen-LoveAndHappyness
        public ActionResult Home()
        {
            return View();
        }

     


    }
}