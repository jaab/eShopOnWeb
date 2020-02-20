using MediatR;
using Microsoft.eShopWeb.Web.ViewModels;
using System.Collections.Generic;

namespace Microsoft.eShopWeb.Web.Pages.Admin.AllOrders
{
    public class GetAllOrders : IRequest<IEnumerable<OrderViewModel>>
    {
        

        public GetAllOrders()
        {
           
        }
    }
}
