using System.Net;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly DiscountGrpcService _discountGrpcService;
        public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService)
        {
            this._basketRepository = basketRepository;
            this._discountGrpcService = discountGrpcService;
        }

        [HttpGet("{userName}",Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
           var items = await this._basketRepository.GetBasket(userName);
           return Ok(items?? new ShoppingCart(userName));
        }


        [HttpPost(Name = "UpdateBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart shoppingCart)
        {
            //TODO: Call discount grpc service
            // calculate price of the products
           
                foreach (var item in shoppingCart.Items)
                {
                    var coutpon = await this._discountGrpcService.GetDiscount(item.ProductName);
                    item.Price = item.Price - coutpon.Amount;

                }
           
            return Ok(await this._basketRepository.UpdateBasket(shoppingCart));
        }


        [HttpDelete("{userName}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            await this._basketRepository.DeleteBasket(userName);
            return Ok();
        }
    }
}
