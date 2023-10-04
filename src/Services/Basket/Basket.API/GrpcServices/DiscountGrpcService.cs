namespace Basket.API.GrpcServices
{
    using Discount.Grpc;
    public class DiscountGrpcService
    {
        Discount.DiscountClient _discountServiceClient { get; set; }
        public DiscountGrpcService(Discount.DiscountClient discountServiceClient)
        {
            _discountServiceClient = discountServiceClient ?? throw new ArgumentNullException(nameof(discountServiceClient));
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var request = new ProductName { Name = productName };

            return await _discountServiceClient.GetDiscountRequestAsync(request);
        }
    }
}
