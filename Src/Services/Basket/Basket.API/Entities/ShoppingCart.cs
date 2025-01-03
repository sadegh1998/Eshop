﻿namespace Basket.API.Entities
{
    public class ShoppingCart
    {
        public string UserName { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
        public ShoppingCart()
        {
        }
        public ShoppingCart(string userName)
        {
            UserName = userName;
        }

        public decimal TotalPrice
        {
            get
            {
                decimal totoalprice = 0;
                foreach (var item in Items)
                {
                    totoalprice += item.Price * item.Quantity;
                }
                return totoalprice;
            }
        }
    }
}
