using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantOrderSystem.Models
{
    public class MenuItem
    {
        public int ItemID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Item Name")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 9999.99, ErrorMessage = "Price must be between ?0.01 and ?9999.99")]
        public decimal Price { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Featured")]
        public bool IsFeatured { get; set; } = false;

        // Navigation property
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        /// <summary>
        /// Gets formatted price in Indian Rupees
        /// </summary>
        public string FormattedPrice => $"?{Price:N0}";

        /// <summary>
        /// Gets user-friendly price for display
        /// </summary>
        public string DisplayPrice => Price % 1 == 0 ? $"?{Price:F0}" : $"?{Price:F2}";

        /// <summary>
        /// Gets availability status text
        /// </summary>
        public string AvailabilityText => IsAvailable ? "Available Now" : "Currently Unavailable";

        /// <summary>
        /// Gets CSS class for availability badge
        /// </summary>
        public string AvailabilityBadgeClass => IsAvailable ? "badge bg-success" : "badge bg-secondary";

        /// <summary>
        /// Gets the vegetarian badge (all items are vegetarian)
        /// </summary>
        public string VegBadge => "?? Vegetarian";

        /// <summary>
        /// Gets short description for cards (max 100 characters)
        /// </summary>
        public string ShortDescription
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                    return "Delicious vegetarian dish made with fresh ingredients";
                
                return Description.Length > 100 
                    ? Description.Substring(0, 97) + "..." 
                    : Description;
            }
        }

        /// <summary>
        /// Gets featured status text
        /// </summary>
        public string FeaturedText => IsFeatured ? "? Chef's Special" : "";

        /// <summary>
        /// Gets appropriate image URL or placeholder
        /// </summary>
        public string SafeImageUrl => !string.IsNullOrEmpty(ImageUrl) 
            ? ImageUrl 
            : "/images/menu/placeholder-veg.jpg";
    }
}