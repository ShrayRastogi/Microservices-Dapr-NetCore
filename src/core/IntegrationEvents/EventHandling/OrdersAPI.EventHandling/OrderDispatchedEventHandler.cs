﻿using Dapr.Actors;
using Dapr.Actors.Client;
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

        private static async void UpdateOrderStatusAsync(Guid orderId, Status status)
        {
            var commonActor = GetCommonActorAsync(orderId);
            var orderStatus = new OrderStatus
            {
                OrderId = orderId,
                Status = status.ToString()
            };
            await commonActor.UpdateOrderStatus(orderStatus);
        }

        private static ICommonActor GetCommonActorAsync(Guid orderId)
        {
            var actorId = new ActorId(orderId.ToString());
            return ActorProxy.Create<ICommonActor>(actorId, nameof(CommonActor));
        }
    }
}
