using System.Web.Mvc;
using System.Linq;
using RestaurantOrderSystem.Helpers;
using RestaurantOrderSystem.Data;

namespace RestaurantOrderSystem.Controllers
{
    public class CartController : Controller
    {
        private RestaurantDbContext db = new RestaurantDbContext();

        // View shopping cart
        public ActionResult Index()
        {
            var cart = SessionHelper.GetCart(Session);
            ViewBag.CartItemCount = cart.GetItemCount();
            return View(cart);
        }

        // Add item to cart (called from Menu)
        [HttpPost]
        public ActionResult AddToCart(int itemId, int quantity = 1)
        {
            try
            {
                var menuItem = db.GetMenuItem(itemId);
                if (menuItem == null)
                {
                    TempData["ErrorMessage"] = "Item not found.";
                    return RedirectToAction("Index", "Menu");
                }

                if (!menuItem.IsAvailable)
                {
                    TempData["ErrorMessage"] = $"{menuItem.Name} is currently not available.";
                    return RedirectToAction("Index", "Menu");
                }

                var cart = SessionHelper.GetCart(Session);
                cart.AddItem(menuItem, quantity);
                SessionHelper.SetCart(Session, cart);

                TempData["SuccessMessage"] = $"{menuItem.Name} added to cart!";
                return RedirectToAction("Index", "Menu");
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding item to cart: " + ex.Message;
                return RedirectToAction("Index", "Menu");
            }
        }

        // Update item quantity in cart (called from Cart view)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCart(int itemId, int quantity)
        {
            try
            {
                var cart = SessionHelper.GetCart(Session);
                
                if (quantity <= 0)
                {
                    cart.RemoveItem(itemId);
                    TempData["SuccessMessage"] = "Item removed from cart.";
                }
                else if (quantity > 99)
                {
                    TempData["ErrorMessage"] = "Maximum quantity allowed is 99.";
                }
                else
                {
                    cart.UpdateQuantity(itemId, quantity);
                    TempData["SuccessMessage"] = "Cart updated successfully.";
                }
                
                SessionHelper.SetCart(Session, cart);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating cart: " + ex.Message;
            }
            
            return RedirectToAction("Index");
        }

        // Remove item from cart (called from Cart view)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromCart(int itemId)
        {
            try
            {
                var cart = SessionHelper.GetCart(Session);
                var item = cart.Items.FirstOrDefault(x => x.ItemID == itemId);
                
                if (item != null)
                {
                    cart.RemoveItem(itemId);
                    SessionHelper.SetCart(Session, cart);
                    TempData["SuccessMessage"] = $"{item.Name} removed from cart.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Item not found in cart.";
                }
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while removing item: " + ex.Message;
            }
            
            return RedirectToAction("Index");
        }

        // Clear entire cart (called from Cart view)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClearCart()
        {
            try
            {
                var cart = SessionHelper.GetCart(Session);
                if (cart.Items.Any())
                {
                    SessionHelper.ClearCart(Session);
                    TempData["SuccessMessage"] = "Cart cleared successfully.";
                }
                else
                {
                    TempData["InfoMessage"] = "Cart is already empty.";
                }
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while clearing cart: " + ex.Message;
            }
            
            return RedirectToAction("Index");
        }

        // Get cart item count for AJAX calls
        public JsonResult GetCartItemCount()
        {
            try
            {
                var count = SessionHelper.GetCartItemCount(Session);
                return Json(new { success = true, count = count }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Get cart total for AJAX calls
        public JsonResult GetCartTotal()
        {
            try
            {
                var cart = SessionHelper.GetCart(Session);
                return Json(new { 
                    success = true, 
                    total = cart.GetTotal(),
                    formattedTotal = cart.FormattedTotal,
                    itemCount = cart.GetItemCount()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Update quantity via AJAX (for better user experience)
        [HttpPost]
        public JsonResult UpdateQuantityAjax(int itemId, int quantity)
        {
            try
            {
                var cart = SessionHelper.GetCart(Session);
                var item = cart.Items.FirstOrDefault(x => x.ItemID == itemId);
                
                if (item == null)
                {
                    return Json(new { success = false, error = "Item not found in cart." });
                }

                if (quantity <= 0)
                {
                    cart.RemoveItem(itemId);
                    SessionHelper.SetCart(Session, cart);
                    return Json(new { 
                        success = true, 
                        removed = true, 
                        message = $"{item.Name} removed from cart.",
                        cartTotal = cart.GetTotal(),
                        cartCount = cart.GetItemCount()
                    });
                }
                else if (quantity > 99)
                {
                    return Json(new { success = false, error = "Maximum quantity allowed is 99." });
                }
                else
                {
                    cart.UpdateQuantity(itemId, quantity);
                    SessionHelper.SetCart(Session, cart);
                    
                    var updatedItem = cart.Items.FirstOrDefault(x => x.ItemID == itemId);
                    return Json(new { 
                        success = true, 
                        removed = false,
                        itemSubtotal = updatedItem.Subtotal,
                        formattedSubtotal = updatedItem.FormattedSubtotal,
                        cartTotal = cart.GetTotal(),
                        cartCount = cart.GetItemCount(),
                        message = "Cart updated successfully."
                    });
                }
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, error = "An error occurred: " + ex.Message });
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