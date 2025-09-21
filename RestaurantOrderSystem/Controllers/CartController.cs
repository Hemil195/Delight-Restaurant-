using System.Web.Mvc;
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

        // Update item quantity in cart
        [HttpPost]
        public ActionResult UpdateQuantity(int itemId, int quantity)
        {
            var cart = SessionHelper.GetCart(Session);
            cart.UpdateQuantity(itemId, quantity);
            SessionHelper.SetCart(Session, cart);
            
            return RedirectToAction("Index");
        }

        // Remove item from cart
        [HttpPost]
        public ActionResult RemoveItem(int itemId)
        {
            var cart = SessionHelper.GetCart(Session);
            cart.RemoveItem(itemId);
            SessionHelper.SetCart(Session, cart);
            
            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        // Clear entire cart
        [HttpPost]
        public ActionResult ClearCart()
        {
            SessionHelper.ClearCart(Session);
            TempData["SuccessMessage"] = "Cart cleared.";
            return RedirectToAction("Index");
        }

        // Get cart item count for AJAX calls
        public JsonResult GetCartItemCount()
        {
            var count = SessionHelper.GetCartItemCount(Session);
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
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