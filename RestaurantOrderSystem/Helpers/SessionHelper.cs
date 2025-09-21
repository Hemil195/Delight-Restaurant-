using System.Web;
using Newtonsoft.Json;
using RestaurantOrderSystem.Models;

namespace RestaurantOrderSystem.Helpers
{
    public static class SessionHelper
    {
        private const string CART_SESSION_KEY = "ShoppingCart";

        public static ShoppingCart GetCart(HttpSessionStateBase session)
        {
            var cartJson = session[CART_SESSION_KEY] as string;
            if (string.IsNullOrEmpty(cartJson))
            {
                return new ShoppingCart();
            }
            return JsonConvert.DeserializeObject<ShoppingCart>(cartJson);
        }

        public static void SetCart(HttpSessionStateBase session, ShoppingCart cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            session[CART_SESSION_KEY] = cartJson;
        }

        public static void ClearCart(HttpSessionStateBase session)
        {
            session.Remove(CART_SESSION_KEY);
        }

        public static int GetCartItemCount(HttpSessionStateBase session)
        {
            var cart = GetCart(session);
            return cart.GetItemCount();
        }
    }
}