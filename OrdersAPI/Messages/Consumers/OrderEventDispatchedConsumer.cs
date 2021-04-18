using MassTransit;
using Messaging.InterfacesConstant.MassTransit.Events;
using OrdersAPI.Models;
using OrdersAPI.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Messages.Consumers
{
    public class OrderEventDispatchedConsumer : IConsumer<IOrderDispatchedEvent>
    {
        private readonly IOrderRepository _orderRepository;
        public OrderEventDispatchedConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public Task Consume(ConsumeContext<IOrderDispatchedEvent> context)
        {
            var message = context.Message;
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
