using System.Collections.Generic;
using System.Linq;

namespace RestaurantOrderSystem.Models
{
    public class CartItem
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        
        public decimal Subtotal => Price * Quantity;

        /// <summary>
        /// Gets formatted price in Indian Rupees
        /// </summary>
        public string FormattedPrice => $"?{Price:N0}";

        /// <summary>
        /// Gets formatted subtotal in Indian Rupees
        /// </summary>
        public string FormattedSubtotal => $"?{Subtotal:N0}";
    }

    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; }

        public ShoppingCart()
        {
            Items = new List<CartItem>();
        }

        public void AddItem(MenuItem menuItem, int quantity = 1)
        {
            var existingItem = Items.FirstOrDefault(x => x.ItemID == menuItem.ItemID);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    ItemID = menuItem.ItemID,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    Quantity = quantity,
                    ImageUrl = menuItem.ImageUrl
                });
            }
        }

        public void RemoveItem(int itemId)
        {
            Items.RemoveAll(x => x.ItemID == itemId);
        }

        public void UpdateQuantity(int itemId, int quantity)
        {
            var item = Items.FirstOrDefault(x => x.ItemID == itemId);
            if (item != null)
            {
                if (quantity <= 0)
                    RemoveItem(itemId);
                else
                    item.Quantity = quantity;
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        public decimal GetTotal()
        {
            return Items.Sum(x => x.Subtotal);
        }

        public int GetItemCount()
        {
            return Items.Sum(x => x.Quantity);
        }

        /// <summary>
        /// Gets formatted total in Indian Rupees
        /// </summary>
        public string FormattedTotal => $"?{GetTotal():N0}";

        /// <summary>
        /// Gets user-friendly item count text
        /// </summary>
        public string ItemCountText
        {
            get
            {
                var count = GetItemCount();
                if (count == 0)
                    return "No items in cart";
                else if (count == 1)
                    return "1 item in cart";
                else
                    return $"{count} items in cart";
            }
        }

        /// <summary>
        /// Checks if cart is empty
        /// </summary>
        public bool IsEmpty => Items.Count == 0;

        /// <summary>
        /// Gets delivery charge (free for orders above ?500)
        /// </summary>
        public decimal DeliveryCharge
        {
            get
            {
                var total = GetTotal();
                return total >= 500 ? 0 : 40; // Free delivery above ?500, otherwise ?40
            }
        }

        /// <summary>
        /// Gets formatted delivery charge
        /// </summary>
        public string FormattedDeliveryCharge => DeliveryCharge == 0 ? "FREE" : $"?{DeliveryCharge:N0}";

        /// <summary>
        /// Gets grand total including delivery
        /// </summary>
        public decimal GrandTotal => GetTotal() + DeliveryCharge;

        /// <summary>
        /// Gets formatted grand total
        /// </summary>
        public string FormattedGrandTotal => $"?{GrandTotal:N0}";
    }
}