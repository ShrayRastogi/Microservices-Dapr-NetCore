using EventBus.Abstractions;
using Events;
using OrdersAPI.Database.Models;
using OrdersAPI.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.EventHandling
{
    public class OrderDispatchedEventHandler: IIntegrationEventHandler<OrderDispatchedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;
        public OrderDispatchedEventHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task Handle(OrderDispatchedIntegrationEvent message)
        {
            Guid orderId = message.OrderId;
            UpdateDatabase(orderId);
            return Task.CompletedTask;
        }

        private void UpdateDatabase(Guid orderId)
        {
            var order = _orderRepository.GetOrder(orderId);
            order.Status = Status.Sent;
            _orderRepository.UpdateOrder(order);
        }
    }
}
