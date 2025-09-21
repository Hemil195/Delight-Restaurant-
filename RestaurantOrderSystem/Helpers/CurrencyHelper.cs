using System;
using System.Globalization;

namespace RestaurantOrderSystem.Helpers
{
    public static class CurrencyHelper
    {
        /// <summary>
        /// Formats decimal amount to Indian Rupee currency format
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <returns>Formatted currency string like ?280.00</returns>
        public static string ToIndianRupee(this decimal amount)
        {
            return $"?{amount:N2}";
        }

        /// <summary>
        /// Formats decimal amount to Indian Rupee currency format without decimals for whole numbers
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <returns>Formatted currency string like ?280 or ?280.50</returns>
        public static string ToIndianRupeeShort(this decimal amount)
        {
            if (amount % 1 == 0)
                return $"?{amount:N0}";
            else
                return $"?{amount:N2}";
        }

        /// <summary>
        /// Creates a user-friendly price display with currency symbol
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <returns>User-friendly price string</returns>
        public static string ToUserFriendlyPrice(this decimal amount)
        {
            return $"? {amount:N2}";
        }

        /// <summary>
        /// Formats amount for display in cart and orders
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <returns>Formatted price for cart display</returns>
        public static string ToCartPrice(this decimal amount)
        {
            return amount % 1 == 0 ? $"?{amount:F0}" : $"?{amount:F2}";
        }
    }

    public static class UserExperienceHelper
    {
        /// <summary>
        /// Creates user-friendly status messages
        /// </summary>
        /// <param name="status">Order status enum</param>
        /// <returns>User-friendly status text</returns>
        public static string ToUserFriendlyStatus(this RestaurantOrderSystem.Models.OrderStatus status)
        {
            switch (status)
            {
                case Models.OrderStatus.Pending:
                    return "Order Received ??";
                case Models.OrderStatus.Confirmed:
                    return "Order Confirmed ?";
                case Models.OrderStatus.Preparing:
                    return "Preparing Your Food ?????";
                case Models.OrderStatus.Ready:
                    return "Ready for Pickup/Delivery ??";
                case Models.OrderStatus.Completed:
                    return "Order Completed ?";
                case Models.OrderStatus.Cancelled:
                    return "Order Cancelled ?";
                default:
                    return status.ToString();
            }
        }

        /// <summary>
        /// Gets appropriate CSS class for order status
        /// </summary>
        /// <param name="status">Order status enum</param>
        /// <returns>Bootstrap CSS class</returns>
        public static string GetStatusBadgeClass(this RestaurantOrderSystem.Models.OrderStatus status)
        {
            switch (status)
            {
                case Models.OrderStatus.Pending:
                    return "badge bg-warning text-dark";
                case Models.OrderStatus.Confirmed:
                    return "badge bg-info text-white";
                case Models.OrderStatus.Preparing:
                    return "badge bg-primary text-white";
                case Models.OrderStatus.Ready:
                    return "badge bg-success text-white";
                case Models.OrderStatus.Completed:
                    return "badge bg-secondary text-white";
                case Models.OrderStatus.Cancelled:
                    return "badge bg-danger text-white";
                default:
                    return "badge bg-light text-dark";
            }
        }

        /// <summary>
        /// Creates a user-friendly item count display
        /// </summary>
        /// <param name="count">Number of items</param>
        /// <returns>User-friendly count text</returns>
        public static string ToItemCountText(this int count)
        {
            if (count == 0)
                return "No items";
            else if (count == 1)
                return "1 item";
            else
                return $"{count} items";
        }

        /// <summary>
        /// Creates a user-friendly time display
        /// </summary>
        /// <param name="dateTime">DateTime to format</param>
        /// <returns>User-friendly time text</returns>
        public static string ToUserFriendlyTime(this DateTime dateTime)
        {
            var now = DateTime.Now;
            var timeSpan = now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            else if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            else if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            else if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days ago";
            else
                return dateTime.ToString("MMM dd, yyyy");
        }
    }
}