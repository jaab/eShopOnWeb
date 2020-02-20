using MediatR;
using Microsoft.eShopWeb.Web.ViewModels;

namespace Microsoft.eShopWeb.Web.Pages.Admin.AllOrderDetails
{
    public class GetAllOrderDetails : IRequest<OrderViewModel>
    {
        public int OrderId { get; set; }

        public GetAllOrderDetails(int orderId)
        {
            OrderId = orderId;
        }
    }
}
