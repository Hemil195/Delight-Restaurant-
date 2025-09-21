using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RestaurantOrderSystem.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        // Navigation property
        public virtual ICollection<MenuItem> MenuItems { get; set; }

        public Category()
        {
            MenuItems = new List<MenuItem>();
        }
    }
}