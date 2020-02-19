using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Web.Features.MyOrders;
using Microsoft.eShopWeb.Web.Features.OrderDetails;
using Microsoft.eShopWeb.Web.Services;
using Microsoft.eShopWeb.Web.ViewModels;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Controllers.Api
{
    
    public class OrderController : BaseApiController
    {

        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository) => _orderRepository = orderRepository;
        
       [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderItemViewModel>> GetById(int orderId) {
            try  {
                var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
                return Ok(order);
            } catch (ModelNotFoundException) {
                return NotFound();
            }
        }

        [HttpGet("{orderId}")]
        public async Task RemoveById(int orderId) {
           
             var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
             var removeOrder = order;
             await _orderRepository.DeleteAsync(removeOrder);  
        }

    }
}