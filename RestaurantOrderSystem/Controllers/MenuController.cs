using System.Linq;
using System.Web.Mvc;
using RestaurantOrderSystem.Data;
using RestaurantOrderSystem.Models;
using RestaurantOrderSystem.Models.ViewModels;
using RestaurantOrderSystem.Helpers;

namespace RestaurantOrderSystem.Controllers
{
    public class MenuController : Controller
    {
        private RestaurantDbContext db = new RestaurantDbContext();

        // Customer-facing menu page
        public ActionResult Index(int? categoryId, string search)
        {
            var model = new MenuViewModel
            {
                Categories = db.GetCategories(),
                SelectedCategoryId = categoryId,
                SearchTerm = search
            };

            if (categoryId.HasValue)
            {
                model.MenuItems = db.GetMenuItemsByCategory(categoryId.Value);
            }
            else if (!string.IsNullOrEmpty(search))
            {
                model.MenuItems = db.GetMenuItems()
                    .Where(m => m.Name.ToLower().Contains(search.ToLower()) || 
                               (m.Description != null && m.Description.ToLower().Contains(search.ToLower())))
                    .Where(m => m.IsAvailable)
                    .ToList();
            }
            else
            {
                model.MenuItems = db.GetMenuItems().Where(m => m.IsAvailable).ToList();
            }

            ViewBag.CartItemCount = SessionHelper.GetCartItemCount(Session);
            return View(model);
        }

        // Menu item details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            
            MenuItem menuItem = db.GetMenuItem(id.Value);
            if (menuItem == null || !menuItem.IsAvailable)
            {
                return HttpNotFound();
            }

            ViewBag.CartItemCount = SessionHelper.GetCartItemCount(Session);
            return View(menuItem);
        }

        // Add item to cart
        [HttpPost]
        public ActionResult AddToCart(int itemId, int quantity = 1)
        {
            var menuItem = db.GetMenuItem(itemId);
            if (menuItem != null && menuItem.IsAvailable)
            {
                var cart = SessionHelper.GetCart(Session);
                cart.AddItem(menuItem, quantity);
                SessionHelper.SetCart(Session, cart);
                
                TempData["SuccessMessage"] = $"{menuItem.Name} added to cart!";
            }
            else
            {
                TempData["ErrorMessage"] = "Item not available.";
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
    }
}