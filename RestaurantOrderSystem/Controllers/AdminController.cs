using System.Linq;
using System.Web.Mvc;
using RestaurantOrderSystem.Data;
using RestaurantOrderSystem.Models;
using RestaurantOrderSystem.Models.ViewModels;

namespace RestaurantOrderSystem.Controllers
{
    public class AdminController : Controller
    {
        private RestaurantDbContext db = new RestaurantDbContext();

        // Admin dashboard
        public ActionResult Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                TotalMenuItems = db.GetTotalMenuItems(),
                TotalCategories = db.GetCategories().Count,
                TotalOrders = db.GetTotalOrders(),
                PendingOrders = db.GetPendingOrdersCount(),
                TodaysRevenue = db.GetTodaysRevenue(),
                RecentOrders = db.GetOrders().Take(10).ToList()
            };

            return View(model);
        }

        #region Menu Item Management
        // List all menu items for admin
        public ActionResult MenuItems()
        {
            var menuItems = db.GetMenuItems();
            return View(menuItems);
        }

        // Create new menu item
        public ActionResult CreateMenuItem()
        {
            ViewBag.Categories = new SelectList(db.GetCategories(), "CategoryID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMenuItem(MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                db.AddMenuItem(menuItem);
                TempData["SuccessMessage"] = "Menu item created successfully!";
                return RedirectToAction("MenuItems");
            }

            ViewBag.Categories = new SelectList(db.GetCategories(), "CategoryID", "Name", menuItem.CategoryID);
            return View(menuItem);
        }

        // Edit menu item
        public ActionResult EditMenuItem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            MenuItem menuItem = db.GetMenuItem(id.Value);
            if (menuItem == null)
            {
                return HttpNotFound();
            }

            ViewBag.Categories = new SelectList(db.GetCategories(), "CategoryID", "Name", menuItem.CategoryID);
            return View(menuItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMenuItem(MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                db.UpdateMenuItem(menuItem);
                TempData["SuccessMessage"] = "Menu item updated successfully!";
                return RedirectToAction("MenuItems");
            }

            ViewBag.Categories = new SelectList(db.GetCategories(), "CategoryID", "Name", menuItem.CategoryID);
            return View(menuItem);
        }

        // Delete menu item
        public ActionResult DeleteMenuItem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            MenuItem menuItem = db.GetMenuItem(id.Value);
            if (menuItem == null)
            {
                return HttpNotFound();
            }

            return View(menuItem);
        }

        [HttpPost, ActionName("DeleteMenuItem")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMenuItemConfirmed(int id)
        {
            try
            {
                db.DeleteMenuItem(id);
                TempData["SuccessMessage"] = "Menu item deleted successfully!";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting menu item: " + ex.Message;
            }
            return RedirectToAction("MenuItems");
        }
        #endregion

        #region Category Management
        // List all categories
        public ActionResult Categories()
        {
            var categories = db.GetCategories();
            return View(categories);
        }

        // Create new category
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                db.AddCategory(category);
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction("Categories");
            }

            return View(category);
        }

        // Edit category
        public ActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Category category = db.GetCategory(id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                db.UpdateCategory(category);
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("Categories");
            }

            return View(category);
        }

        // Delete category
        public ActionResult DeleteCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Category category = db.GetCategory(id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategoryConfirmed(int id)
        {
            try
            {
                db.DeleteCategory(id);
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting category: " + ex.Message;
            }
            return RedirectToAction("Categories");
        }
        #endregion

        #region Order Management
        // List all orders
        public ActionResult Orders()
        {
            var orders = db.GetOrders();
            return View(orders);
        }

        // View order details
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Order order = db.GetOrder(id.Value);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.OrderDetails = db.GetOrderDetails(id.Value);
            return View(order);
        }

        // Update order status
        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, OrderStatus status)
        {
            try
            {
                db.UpdateOrderStatus(orderId, status);
                TempData["SuccessMessage"] = "Order status updated successfully!";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating order status: " + ex.Message;
            }

            return RedirectToAction("OrderDetails", new { id = orderId });
        }
        #endregion

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