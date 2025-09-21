using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderSystem.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<MenuItem> FeaturedItems { get; set; }
        public List<Category> Categories { get; set; }
        public int TotalMenuItems { get; set; }
        public int TotalOrders { get; set; }
    }

    public class MenuViewModel
    {
        public List<Category> Categories { get; set; }
        public List<MenuItem> MenuItems { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string SearchTerm { get; set; }
    }

    public class CheckoutViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string CustomerEmail { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string CustomerPhone { get; set; }

        [StringLength(500)]
        [Display(Name = "Special Instructions")]
        public string SpecialInstructions { get; set; }

        public ShoppingCart Cart { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int TotalMenuItems { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TodaysRevenue { get; set; }
        public List<Order> RecentOrders { get; set; }
        public List<MenuItem> PopularItems { get; set; }
    }

    public class OrderHistoryViewModel
    {
        public string CustomerEmail { get; set; }
        public List<Order> Orders { get; set; }
    }
}