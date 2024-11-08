using ChurchSystem.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchSystem.Controllers
{
    public class BasketController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // Display the shopping cart
        public ActionResult Index()
        {
            var cartItems = db.Basket.ToList();
            return View(cartItems);
        }

        public ActionResult AddToCart(int productId, int quantity)
        {
            // Lookup product details based on productId (Assuming you have a Product entity)
            var product = db.Products.Find(productId);

            if (product == null)
            {
                // Handle the case where the product doesn't exist
                return RedirectToAction("Index");
            }

            // Create a new shopping cart item
            var cartItem = new basket
            {
                ProductId = productId,
                ProductName = product.productDescription,
                Price = (decimal)product.ProductPrice,
                Quantity = quantity,
                item = "Sale Product"
            };

            // Add the item to the cart and save it to the database
            db.Basket.Add(cartItem);
            db.SaveChanges();

            return RedirectToAction("Index"); // Redirect to cart display page
        }

        public ActionResult ItemAddToCart(int productId, int quantity)
        {
            // Lookup product details based on productId (Assuming you have a Product entity)
            var product = db.ItemDonations.Find(productId);

            if (product == null)
            {
                // Handle the case where the product doesn't exist
                return RedirectToAction("Index");
            }

            // Create a new shopping cart item
            var cartItem = new basket
            {
                ProductId = productId,
                ProductName = product.itemDescription,
                Price = (decimal)product.Price,
                Quantity = quantity,
                item = "Donated Item"
            };

            // Add the item to the cart and save it to the database
            db.Basket.Add(cartItem);
            db.SaveChanges();

            return RedirectToAction("Index"); // Redirect to cart display page
        }




        public ActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = db.Basket.Find(cartItemId);

            if (cartItem != null)
            {
                db.Basket.Remove(cartItem);
                db.SaveChanges();
            }

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

        public ActionResult Checkout()
        {
            // Assuming you have a user authentication system in place
            var userId = User.Identity.Name; // Get the user's ID

            // Create a new order
            var order = new Models.Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending", // Set the initial status
            };

            db.Orders.Add(order);
            db.SaveChanges(); // Save the order to the database

            // Now, you have an order created for the user

            // Iterate through the shopping cart items and create order items
            foreach (var cartItem in db.Basket)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.ProductName,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity,
                    item = cartItem.item,
                    OrderId = order.OrderId, // Associate the order item with the order
                };

                db.OrderItems.Add(orderItem);
                db.Basket.Remove(cartItem);
            }

            // Calculate the total amount of the order
            order.TotalAmount = db.Basket.Sum(cartItem => cartItem.Price * cartItem.Quantity);
            db.SaveChanges(); // Update the order's total amount           

            // You can display a confirmation page with order details
            return View(order);
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
                    return_url = Url.Action("ExecutePayment", "Basket", null, Request.Url.Scheme),
                    cancel_url = Url.Action("CancelPayment", "Basket", null, Request.Url.Scheme)
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

            return RedirectToAction("Index","Order");
        }

        public ActionResult CancelPayment()
        {
            return RedirectToAction("Index", "Home");
        }



















    }
}