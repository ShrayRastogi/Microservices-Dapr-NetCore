using Messaging.InterfacesConstant.Dapr.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OrdersAPI.IntegrationEvents.EventHandling;
using OrdersAPI.Models;
using OrdersAPI.Persistence;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IServiceProvider _serviceProvider;

        public OrdersController(IServiceProvider serviceProvider, IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepository.GetOrdersAsync();
            foreach (var order in orders)
            {
                order.OrderStatus = Enum.GetName(typeof(Status), order.Status);
            }
            return Ok(orders);
        }

        [HttpGet]
        [Route("{orderId}", Name = "GetOrderById")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var order = await _orderRepository.GetOrderAsync(Guid.Parse(orderId));
            order.OrderStatus = Enum.GetName(typeof(Status), order.Status);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        [Route("createorder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task CreateOrderAsync([FromBody] RegisterOrderIntegrationEvent message)
        {
            var handler = _serviceProvider.GetRequiredService<RegisterOrderEventHandler>();
           await handler.Handle(message);
        }
    }
}
