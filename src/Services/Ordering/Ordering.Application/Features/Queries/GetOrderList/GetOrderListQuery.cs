using MediatR;

namespace Ordering.Application.Features.Queries.GetOrderList
{
    public class GetOrderListQuery : IRequest<List<OrdersVm>>
    {
        public GetOrderListQuery(string userName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName)); ;
        }

        public string UserName { get; set; }
    }
}
