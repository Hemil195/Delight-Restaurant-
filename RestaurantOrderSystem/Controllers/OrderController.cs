using System;
using System.Linq;
using System.Web.Mvc;
using RestaurantOrderSystem.Data;
using RestaurantOrderSystem.Models;
using RestaurantOrderSystem.Models.ViewModels;
using RestaurantOrderSystem.Helpers;

namespace RestaurantOrderSystem.Controllers
{
    public class OrderController : Controller
    {
        private RestaurantDbContext db = new RestaurantDbContext();

        // Customer checkout page
        public ActionResult Checkout()
        {
            var cart = SessionHelper.GetCart(Session);
            if (cart.Items.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
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
            model.Cart = cart;

            if (cart.Items.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Menu");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create order
                    var order = new Order
                    {
                        CustomerName = model.CustomerName,
                        CustomerEmail = model.CustomerEmail,
                        CustomerPhone = model.CustomerPhone,
                        SpecialInstructions = model.SpecialInstructions,
                        TotalAmount = cart.GetTotal(),
                        OrderDate = DateTime.Now,
                        Status = OrderStatus.Pending
                    };

                    int orderId = db.CreateOrder(order);

                    // Create order details
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

        // Customer order tracking
        public ActionResult Track(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.CartItemCount = SessionHelper.GetCartItemCount(Session);
                return View(new OrderHistoryViewModel { CustomerEmail = email });
            }

            var orders = db.GetOrders().Where(o => o.CustomerEmail.ToLower() == email.ToLower()).ToList();
            var model = new OrderHistoryViewModel
            {
                CustomerEmail = email,
                Orders = orders
            };

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