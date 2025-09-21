using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestaurantOrderSystem.Data;
using RestaurantOrderSystem.Models.ViewModels;
using RestaurantOrderSystem.Helpers;

namespace RestaurantOrderSystem.Controllers
{
    public class HomeController : Controller
    {
        private RestaurantDbContext db = new RestaurantDbContext();

        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                FeaturedItems = db.GetFeaturedMenuItems(),
                Categories = db.GetCategories(),
                TotalMenuItems = db.GetTotalMenuItems(),
                TotalOrders = db.GetTotalOrders()
            };
            
            // Set cart count for layout
            ViewBag.CartItemCount = SessionHelper.GetCartItemCount(Session);
            
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Restaurant Order System - A comprehensive solution for restaurant management and online ordering.";
            ViewBag.CartItemCount = SessionHelper.GetCartItemCount(Session);
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us for support or inquiries about the Restaurant Order System.";
            ViewBag.CartItemCount = SessionHelper.GetCartItemCount(Session);
            return View();
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