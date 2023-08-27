namespace Basket.API.Entities
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
            this.UserName = userName;
        }

        public decimal TotalPrice
        {
            get
            {
                decimal sum = 0;
                foreach (var item in Items)
                {
                    sum += item.Price*item.Quantity;
                }
                return sum;
            }
        }
    }

    
}
