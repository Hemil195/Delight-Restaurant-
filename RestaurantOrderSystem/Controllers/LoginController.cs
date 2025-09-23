using System.Web.Mvc;

namespace RestaurantOrderSystem.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            // If already logged in, redirect to admin dashboard
            if (Session["IsAdminLoggedIn"] != null && (bool)Session["IsAdminLoggedIn"])
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string username, string password)
        {
            // Static credentials: admin / admin123
            if (username == "admin" && password == "admin123")
            {
                Session["IsAdminLoggedIn"] = true;
                Session["AdminUsername"] = username;
                TempData["SuccessMessage"] = "Welcome back, Administrator!";
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid username or password. Please try again.";
                return View();
            }
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Remove("IsAdminLoggedIn");
            Session.Remove("AdminUsername");
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}