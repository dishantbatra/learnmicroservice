namespace Shopping.Aggregator.Models
{
    // This is a class which we send when client application send user name
    public class ShoppingModel
    {
        public string UserName { get; set; }
        public BasketModel BasketWithProducts { get; set; }
        public IEnumerable<OrderResponseModel> Orders { get; set; }
    }
}
