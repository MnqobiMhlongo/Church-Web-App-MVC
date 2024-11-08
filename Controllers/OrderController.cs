using ChurchSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchSystem.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        public ActionResult Index()
        {
            // Get the current user's orders
            var userId = User.Identity.Name; // Assuming you use usernames for user identification
            var orders = db.Orders.Where(o => o.UserId == userId).ToList();

            return View(orders);
        }

        // Display order details
        public ActionResult Details(int id)
        {
            var order = db.Orders.SingleOrDefault(o => o.OrderId == id);

            // Ensure the order belongs to the current user (security check)
            if (order == null )
            {
                return HttpNotFound();
            }

            // Load the associated OrderItems for the order
            db.Entry(order).Collection(o => o.OrderItems).Load();

            return View(order);
        }

        // Display a list of all orders (for administrators)
        public ActionResult AdminIndex()
        {
            var orders = db.Orders.ToList();
            return View(orders);
        }

        // Update the order status
        [HttpGet]
        public ActionResult UpdateStatus(int id)
        {
            var order = db.Orders.SingleOrDefault(o => o.OrderId == id);

            // Ensure the order exists
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateStatus(int id, string newStatus)
        {
            var order = db.Orders.SingleOrDefault(o => o.OrderId == id);

            // Ensure the order exists
            if (order == null)
            {
                return HttpNotFound();
            }

            // Update the order status
            order.OrderStatus = newStatus;
            db.SaveChanges();

            // Redirect back to the Admin Index after updating
            return RedirectToAction("AdminIndex", new { successMessage = "STATUS UPDATED" });
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