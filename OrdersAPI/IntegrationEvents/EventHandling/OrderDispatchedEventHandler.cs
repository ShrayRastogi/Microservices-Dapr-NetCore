using Messaging.InterfacesConstant.Dapr.Abstractions;
using Messaging.InterfacesConstant.Dapr.Events;
using OrdersAPI.Models;
using OrdersAPI.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.IntegrationEvents.EventHandling
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
