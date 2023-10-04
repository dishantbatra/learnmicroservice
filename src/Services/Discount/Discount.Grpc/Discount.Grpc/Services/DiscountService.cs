using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services
{
    public class DiscountService : Discount.DiscountBase
    {
        private IDiscountRepository _discountRepository;
        private ILogger<DiscountService> _logger;
        private IMapper _mapper;
        public DiscountService(IDiscountRepository discountRepository, ILogger<DiscountService> logger, IMapper mapper) { 
            _discountRepository = discountRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async override Task<CouponModel> CreateDiscountRequest(CouponModel request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request);

            var result =  await this._discountRepository.CreateDiscount(coupon);
            _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);


            var couponModel = _mapper.Map<CouponModel>(result);
            return couponModel;
        }

        public async override Task<SuccessResponse> DeleteDiscountRequest(CouponModel request, ServerCallContext context)
        {
            var deleted = await _discountRepository.DeleteDiscount(request.ProductName);
            var response = new SuccessResponse
            {
                Success = deleted
            };

            return response;
        }

        public async override Task<CouponModel> GetDiscountRequest(ProductName request, ServerCallContext context)
        {
            var coupon = await this._discountRepository.GetDiscount(request.Name);

            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with product name {request.Name} not found"));
            }
            _logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}", coupon.ProductName, coupon.Amount);

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public async override Task<CouponModel> UpdateDiscountRequest(CouponModel request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request);

            await _discountRepository.UpdateDiscount(coupon);
            _logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }
    }
}
