using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using ChurchSystem.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using PayPal.Api;
using Microsoft.AspNetCore.Mvc;

namespace ChurchSystem.Controllers
{
    public class DonationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Donations
        public ActionResult Index()
        {
            var donations = db.Donations.Include(d => d.ChurchFunds);
            return View(donations.ToList());
        }

        // GET: Donations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donations donations = db.Donations.Find(id);
            if (donations == null)
            {
                return HttpNotFound();
            }
            return View(donations);
        }

        // GET: Donations/Create
        public ActionResult Create()
        {
            ViewBag.FundId = new SelectList(db.ChurchFunds, "FundId", "FundName");
            return View();
        }

        // POST: Donations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DonationId,DonorName,DonationAmount,DonationDate,FundId")] Donations donations)
        {
            if (ModelState.IsValid)
            {

                //donations.DonorName = User.Identity.Name;

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var user = userManager.FindById(User.Identity.GetUserId());

                if (user != null)
                {
                    string fullName = user.NameSurname; // Assuming your user model has a property for the full name
                    donations.DonorName = fullName;
                }
                donations.DonationDate = DateTime.Now;


                db.Donations.Add(donations);
                db.SaveChanges();
                return RedirectToAction("CreatePayment");
            }

            ViewBag.FundId = new SelectList(db.ChurchFunds, "FundId", "FundName", donations.FundId);
            return View(donations);
        }

        // GET: Donations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donations donations = db.Donations.Find(id);
            if (donations == null)
            {
                return HttpNotFound();
            }
            ViewBag.FundId = new SelectList(db.ChurchFunds, "FundId", "FundName", donations.FundId);
            return View(donations);
        }

        // POST: Donations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DonationId,DonorName,DonationAmount,DonationDate,FundId")] Donations donations)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FundId = new SelectList(db.ChurchFunds, "FundId", "FundName", donations.FundId);
            return View(donations);
        }

        // GET: Donations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donations donations = db.Donations.Find(id);
            if (donations == null)
            {
                return HttpNotFound();
            }
            return View(donations);
        }

        // POST: Donations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Donations donations = db.Donations.Find(id);
            db.Donations.Remove(donations);
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

        public ActionResult CreatePayment()
        {

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
        {
            new Transaction
            {
                amount = new Amount
                {
                    total = "5",
                    currency = "USD"
                },
                description = "Example payment"
            }
        },
                redirect_urls = new RedirectUrls
                {
                    return_url = Url.Action("ExecutePayment", "Donations", null, Request.Url.Scheme),
                    cancel_url = Url.Action("CancelPayment", "Donations", null, Request.Url.Scheme)
                }
            };

            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(
                config["clientId"], config["clientSecret"])
                .GetAccessToken();
            var apiContext = new APIContext(accessToken);

            var createdPayment = payment.Create(apiContext);

            var approvalUrl = createdPayment.links.FirstOrDefault(link => link.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase));
            if (approvalUrl != null)
            {
                return Redirect(approvalUrl.href);
            }

            return RedirectToAction("Index", "Home");
        }


        public ActionResult ExecutePayment(string paymentId, string token, string PayerID)
        {
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(
                config["clientId"], config["clientSecret"])
                .GetAccessToken();
            var apiContext = new APIContext(accessToken);

            var paymentExecution = new PaymentExecution { payer_id = PayerID };
            var payment = new Payment { id = paymentId };
            var executedPayment = payment.Execute(apiContext, paymentExecution);

            // TODO: Handle the successful payment










            return RedirectToAction("ThankYou");
        }

        public ActionResult CancelPayment()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ThankYou()
        {

            var usermail = User.Identity.Name;

            MailMessage mail = new MailMessage();
            mail.To.Add(usermail);
            mail.From = new MailAddress("GRACEOFGODCHURCH@gmail.com");
            mail.Subject = "THANK YOU FOR YOUR DONATION";
            string Body = "Thank YOU FOR YOUR DONATION";

            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("graceofgodchurch0@gmail.com", "wwiaiouwesmobmrd"); // Enter seders User name and password   
            smtp.EnableSsl = true;
            smtp.Send(mail);

            return View();
        }

        //KanyeWest-Lonely
        public ActionResult Donations()
        {
            var donations = db.Donations.Include(d => d.ChurchFunds).ToList();

            List<(int DonationId,string Message, DateTime date)> Messages = new List<(int DonationId, string Message, DateTime date)>();

            foreach(var item in donations )
            {
                string text = item.DonorName + " Donated R" +item.DonationAmount + " To The " + item.ChurchFunds.FundName + " Fund";
                Messages.Add((item.DonationId,text, item.DonationDate));
            }


            //JojiDieForYou
            var fundsTotals = db.Donations
               .GroupBy(d => new { d.ChurchFunds.FundName })
               .Select(g => new
               {
                   ChurchFund = g.Key.FundName,
                   DonationTotal = g.Sum(d => d.DonationAmount)
               })
               .ToList()
               .Select(x => (x.ChurchFund, x.DonationTotal))
               .ToList();


            var total = db.Donations.Select(t => t.DonationAmount).Sum();

            ViewBag.DonationFunds = fundsTotals;
            ViewBag.DonationTotal = total;  
            
            ViewBag.Messages = Messages;    
            return View();
        }


        //kabzadesmallSponono
        public ActionResult ChurchFunds()
        {
            //marvinGaye-Iwantyou
            var fundsTotals = db.Donations
               .GroupBy(d => new { d.ChurchFunds.FundName })
               .Select(g => new
               {
                   ChurchFund = g.Key.FundName,
                   DonationTotal = g.Sum(d => d.DonationAmount)
               })
               .ToList()
               .Select(x => (x.ChurchFund, x.DonationTotal))
               .ToList();


            //YebbasHeartBreak
            double DonatedSales = ((double)db.OrderItems.Where(i => i.item == "Donated Item").Select(t => t.Price).Sum());
            double ProductSales = ((double)db.OrderItems.Where(i => i.item == "Sale Product").Select(t => t.Price).Sum());
            double Total = DonatedSales + ProductSales;

            foreach (var total in fundsTotals)
            {
                Total += total.DonationTotal;
            }



            ViewBag.Total = Total;
            ViewBag.DonatedSales = DonatedSales;
            ViewBag.Products = ProductSales;
            ViewBag.DonationFunds = fundsTotals;
            return View();
        }


        [HttpPost]
        public ActionResult TransferToMusic()
        {
            double DonatedSales = ((double)db.OrderItems.Where(i => i.item == "Sale Product").Select(t => t.Price).Sum());

            var musicDonations = db.Donations.Where(v => v.FundId == 9).ToList();

            var donatedorders = db.OrderItems.Where(v => v.item == "Sale Product").ToList();


            foreach (var cops in donatedorders)
            {
                cops.Price = 0;
                db.Entry(cops).State = EntityState.Modified;
            }

            var transfer = new Donations
            {
                DonorName = "Product Sales",
                DonationAmount = DonatedSales,
                DonationDate = DateTime.Now,
                FundId = 9
            };

            db.Donations.Add(transfer);
            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult TransferToSunday()
        {
            double DonatedSales = ((double)db.OrderItems.Where(i => i.item == "Sale Product").Select(t => t.Price).Sum());

            var musicDonations = db.Donations.Where(v => v.FundId == 9).ToList();

            var donatedorders = db.OrderItems.Where(v => v.item == "Sale Product").ToList();


            foreach (var cops in donatedorders)
            {
                cops.Price = 0;
                db.Entry(cops).State = EntityState.Modified;
            }

            var transfer = new Donations
            {
                DonorName = "Product Sales",
                DonationAmount = DonatedSales,
                DonationDate = DateTime.Now,
                FundId = 3
            };

            db.Donations.Add(transfer);
            db.SaveChanges();
            return Json(new { success = true });


        }

        [HttpPost]
        public ActionResult TransferToYouth()
        {
            double DonatedSales = ((double)db.OrderItems.Where(i => i.item == "Sale Product").Select(t => t.Price).Sum());

            var musicDonations = db.Donations.Where(v => v.FundId == 9).ToList();

            var donatedorders = db.OrderItems.Where(v => v.item == "Sale Product").ToList();


            foreach (var cops in donatedorders)
            {
                cops.Price = 0;
                db.Entry(cops).State = EntityState.Modified;
            }

            var transfer = new Donations
            {
                DonorName = "Product Sales",
                DonationAmount = DonatedSales,
                DonationDate = DateTime.Now,
                FundId = 5
            };

            db.Donations.Add(transfer);
            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult TransferToScholarShips()
        {
            double DonatedSales = ((double)db.OrderItems.Where(i => i.item == "Sale Product").Select(t => t.Price).Sum());

            var musicDonations = db.Donations.Where(v => v.FundId == 9).ToList();

            var donatedorders = db.OrderItems.Where(v => v.item == "Sale Product").ToList();


            foreach (var cops in donatedorders)
            {
                cops.Price = 0;
                db.Entry(cops).State = EntityState.Modified;
            }

            var transfer = new Donations
            {
                DonorName = "Product Sales",
                DonationAmount = DonatedSales,
                DonationDate = DateTime.Now,
                FundId = 8
            };
            db.Donations.Add(transfer);
            db.SaveChanges();
            return Json(new { success = true });
        }







        //luthervandrossyoursecretlove


        [HttpPost]
        public ActionResult chTransferToMusic()
        {
            double DonatedSales = ((double)db.OrderItems.Where(i => i.item == "Donated Item").Select(t => t.Price).Sum());

            var musicDonations = db.Donations.Where(v => v.FundId == 9).ToList();

            var donatedorders = db.OrderItems.Where(v => v.item == "Donated Item").ToList();


            foreach (var cops in donatedorders)
            {
                cops.Price = 0;
                db.Entry(cops).State = EntityState.Modified;
            }

            var transfer = new Donations
            {
                DonorName = "Charity Sales",
                DonationAmount = DonatedSales,
                DonationDate = DateTime.Now,
                FundId = 9
            };

            db.Donations.Add(transfer);
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult chTransferToSunday()
        {
            double DonatedSales = ((double)db.OrderItems.Where(i => i.item == "Donated Item").Select(t => t.Price).Sum());

            var musicDonations = db.Donations.Where(v => v.FundId == 9).ToList();

            var donatedorders = db.OrderItems.Where(v => v.item == "Donated Item").ToList();


            foreach (var cops in donatedorders)
            {
                cops.Price = 0;
                db.Entry(cops).State = EntityState.Modified;
            }

            var transfer = new Donations
            {
                DonorName = "Charity Sales",
                DonationAmount = DonatedSales,
                DonationDate = DateTime.Now,
                FundId = 3
            };

            db.Donations.Add(transfer);
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult chTransferToYouth()
        {
            double DonatedSales = ((double)db.OrderItems.Where(i => i.item == "Donated Item").Select(t => t.Price).Sum());

            var musicDonations = db.Donations.Where(v => v.FundId == 9).ToList();

            var donatedorders = db.OrderItems.Where(v => v.item == "Donated Item").ToList();


            foreach (var cops in donatedorders)
            {
                cops.Price = 0;
                db.Entry(cops).State = EntityState.Modified;
            }

            var transfer = new Donations
            {
                DonorName = "Charity Sales",
                DonationAmount = DonatedSales,
                DonationDate = DateTime.Now,
                FundId = 5
            };

            db.Donations.Add(transfer);
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult chTransferToScholarShips()
        {
            double DonatedSales = ((double)db.OrderItems.Where(i => i.item == "Donated Item").Select(t => t.Price).Sum());

            var musicDonations = db.Donations.Where(v => v.FundId == 9).ToList();

            var donatedorders = db.OrderItems.Where(v => v.item == "Donated Item").ToList();


            foreach (var cops in donatedorders)
            {
                cops.Price = 0;
                db.Entry(cops).State = EntityState.Modified;
            }

            var transfer = new Donations
            {
                DonorName = "Charity Sales",
                DonationAmount = DonatedSales,
                DonationDate = DateTime.Now,
                FundId = 8
            };

            db.Donations.Add(transfer);
            db.SaveChanges();

            return Json(new { success = true });

        }
    }
}
