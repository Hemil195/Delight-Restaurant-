using System.Web.Mvc;

namespace RestaurantOrderSystem.Filters
{
    public class AdminAuthenticationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check if admin is logged in
            if (filterContext.HttpContext.Session["IsAdminLoggedIn"] == null || 
                !(bool)filterContext.HttpContext.Session["IsAdminLoggedIn"])
            {
                // Redirect to login page
                filterContext.Result = new RedirectResult("~/Login");
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}