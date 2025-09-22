using System;
using System.Linq;
using System.Web.Mvc;
using RestaurantOrderSystem.Data;
using RestaurantOrderSystem.Helpers;
using RestaurantOrderSystem.Models;
using RestaurantOrderSystem.Models.ViewModels;

namespace RestaurantOrderSystem.Controllers
{
    public class OrderController : Controller
    {
        private RestaurantDbContext db = new RestaurantDbContext();

        // Customer checkout page
        public ActionResult Checkout()
        {
            var cart = SessionHelper.GetCart(Session);
            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items before checkout.";
                return RedirectToAction("Index", "Menu");
            }

            var model = new CheckoutViewModel
            {
                Cart = cart
            };

            ViewBag.CartItemCount = cart.GetItemCount();
            return View(model);
        }

        // Process checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            var cart = SessionHelper.GetCart(Session);
            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items before checkout.";
                return RedirectToAction("Index", "Menu");
            }

            model.Cart = cart;

            if (ModelState.IsValid)
            {
                try
                {
                    // Calculate total with tax
                    var subtotal = cart.GetTotal();
                    var tax = subtotal * 0.08m; // 8% tax
                    var total = subtotal + tax;

                    // Create order
                    var order = new Order
                    {
                        CustomerName = model.CustomerName,
                        CustomerEmail = model.CustomerEmail,
                        CustomerPhone = model.CustomerPhone,
                        SpecialInstructions = model.SpecialInstructions,
                        TotalAmount = total,
                        OrderDate = DateTime.Now,
                        Status = OrderStatus.Pending
                    };

                    // Save order to database
                    var orderId = db.CreateOrder(order);

                    // Add order details
                    foreach (var item in cart.Items)
                    {
                        var orderDetail = new OrderDetail
                        {
                            OrderID = orderId,
                            ItemID = item.ItemID,
                            Quantity = item.Quantity,
                            UnitPrice = item.Price,
                            Subtotal = item.Subtotal
                        };
                        db.AddOrderDetail(orderDetail);
                    }

                    // Clear cart
                    SessionHelper.ClearCart(Session);

                    TempData["SuccessMessage"] = $"Order #{orderId} placed successfully!";
                    return RedirectToAction("OrderConfirmation", new { id = orderId });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while processing your order: " + ex.Message;
                }
            }

            ViewBag.CartItemCount = cart.GetItemCount();
            return View(model);
        }

        // Order confirmation page
        public ActionResult OrderConfirmation(int id)
        {
            var order = db.GetOrder(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.OrderDetails = db.GetOrderDetails(id);
            ViewBag.CartItemCount = SessionHelper.GetCartItemCount(Session);
            return View(order);
        }

        // Customer order tracking - GET request for direct access
        public ActionResult Track(string email)
        {
            var model = new OrderHistoryViewModel
            {
                CustomerEmail = email
            };

            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    var orders = db.GetOrdersByEmail(email.Trim());
                    model.Orders = orders;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while retrieving your orders: " + ex.Message;
                    model.Orders = new System.Collections.Generic.List<Order>();
                }
            }

            ViewBag.CartItemCount = SessionHelper.GetCartItemCount(Session);
            return View(model);
        }

        // Customer order tracking - POST request from form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Track(FormCollection form)
        {
            var customerEmail = form["customerEmail"];
            
            var model = new OrderHistoryViewModel
            {
                CustomerEmail = customerEmail
            };

            if (!string.IsNullOrEmpty(customerEmail))
            {
                try
                {
                    var orders = db.GetOrdersByEmail(customerEmail.Trim());
                    model.Orders = orders;
                    
                    if (orders == null || !orders.Any())
                    {
                        TempData["InfoMessage"] = $"No orders found for email address: {customerEmail}";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while retrieving your orders: " + ex.Message;
                    model.Orders = new System.Collections.Generic.List<Order>();
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please enter a valid email address.";
                model.Orders = new System.Collections.Generic.List<Order>();
            }

            ViewBag.CartItemCount = SessionHelper.GetCartItemCount(Session);
            return View(model);
        }

        // Get order status for AJAX
        public JsonResult GetOrderStatus(int orderId)
        {
            var order = db.GetOrder(orderId);
            if (order != null)
            {
                return Json(new { 
                    status = order.Status.ToString(), 
                    statusText = GetStatusText(order.Status) 
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "NotFound" }, JsonRequestBehavior.AllowGet);
        }

        private string GetStatusText(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Pending: return "Order Received";
                case OrderStatus.Confirmed: return "Order Confirmed";
                case OrderStatus.Preparing: return "Being Prepared";
                case OrderStatus.Ready: return "Ready for Pickup";
                case OrderStatus.Completed: return "Completed";
                case OrderStatus.Cancelled: return "Cancelled";
                default: return "Unknown";
            }
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