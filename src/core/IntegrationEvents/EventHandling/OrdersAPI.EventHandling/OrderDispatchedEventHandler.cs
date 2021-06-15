using EventBus.Abstractions;
using Events;
using OrdersAPI.Database.Models;
using OrdersAPI.Database.Repositories;
using StateStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.EventHandling
{
    public class OrderDispatchedEventHandler : IIntegrationEventHandler<OrderDispatchedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICommonStateStore _stateStore;
        public OrderDispatchedEventHandler(IOrderRepository orderRepository, ICommonStateStore stateStore)
        {
            _orderRepository = orderRepository;
            _stateStore = stateStore;
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
            UpdateOrderStatusAsync(orderId, order.Status);
        }

        private async void UpdateOrderStatusAsync(Guid orderId, Status status)
        {
            var prevOrderStatus = await _stateStore.GetStateAsync<OrderStatus>(orderId.ToString());
            bool shouldUpdateOrderStatus = (prevOrderStatus != null) && status.ToString() switch
            {
                "Registered" => !prevOrderStatus.Status.Equals("Processed") &&
                              !prevOrderStatus.Status.Equals("Sent"),
                "Processed" => !prevOrderStatus.Status.Equals("Sent"),
                "Sent" => !prevOrderStatus.Status.Equals("Sent"),
                _ => false,
            };
            if (shouldUpdateOrderStatus)
            {
                prevOrderStatus.Status = Status.Sent.ToString();
                await _stateStore.UpdateStateAsync(prevOrderStatus, orderId.ToString());
            }
            else
            {
                throw new Exception("Invalid Order Status");
            }
        }
    }
}
