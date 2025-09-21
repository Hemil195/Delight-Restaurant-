using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantOrderSystem.Models
{
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Preparing = 2,
        Ready = 3,
        Completed = 4,
        Cancelled = 5
    }

    public class Order
    {
        public int OrderID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string CustomerPhone { get; set; }

        [StringLength(500)]
        [Display(Name = "Special Instructions")]
        public string SpecialInstructions { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Order Status")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Navigation property
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            OrderDetails = new List<OrderDetail>();
            OrderDate = DateTime.Now;
        }
    }
}