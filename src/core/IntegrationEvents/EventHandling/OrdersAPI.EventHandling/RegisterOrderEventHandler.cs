using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using EventBus.Abstractions;
using Events;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrdersAPI.Database.Models;
using OrdersAPI.Database.Repositories;
using OrdersAPI.StateStore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrdersAPI.EventHandling
{
    public class RegisterOrderEventHandler : IIntegrationEventHandler<RegisterOrderIntegrationEvent>
    {
        readonly IOrderRepository _orderRepository;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IEventBus _eventBus;
        public RegisterOrderEventHandler(IOrderRepository orderRepository, IHttpClientFactory clientFactory, IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _clientFactory = clientFactory;
            _eventBus = eventBus;
        }
        public async Task Handle(RegisterOrderIntegrationEvent result)
        {
            if (result.OrderId != Guid.Empty
                && result.PictureUrl != null
                && result.ImageData != null
                && result.UserEmail != null)
            {
                SaveOrder(result);
                var client = DaprClient.CreateInvokeHttpClient(appId: "facesapi");
                Tuple<List<byte[]>, Guid> orderDetail = await GetFacesFromFacesAPIAsync(client, result.ImageData, result.OrderId);
                List<byte[]> faces = orderDetail.Item1;
                Guid orderId = orderDetail.Item2;
                SaveOrderDetails(orderId, faces);
                var processedOrder = new OrderProcessedIntegrationEvent
                {
                    OrderId = result.OrderId,
                    Faces = faces,
                    UserEmail = result.UserEmail,
                    PictureUrl = result.PictureUrl
                };
                await _eventBus.PublishAsync<OrderProcessedIntegrationEvent>(processedOrder);
            }
        }

        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFacesAPIAsync(HttpClient client, byte[] imageData, Guid orderId)
        {
            var byteContent = new ByteArrayContent(imageData);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            using var res = await client.PostAsync("/api/faces?orderId=" + orderId, byteContent);
            string apiResponse = await res.Content.ReadAsStringAsync();
            Tuple<List<byte[]>, Guid> orderDetail = JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);
            return orderDetail;
        }

        private void SaveOrderDetails(Guid orderId, List<byte[]> faces)
        {
            var order = _orderRepository.GetOrderAsync(orderId).Result;
            if (order != null)
            {
                order.Status = Status.Processed;
                foreach (var face in faces)
                {
                    OrderDetail orderDetail = new()
                    {
                        FaceData = face,
                        OrderId = orderId
                    };
                    order.OrderDetails.Add(orderDetail);
                }
                _orderRepository.UpdateOrder(order);
                UpdateOrderStatusAsync(orderId, Status.Processed);
            }
        }

        private void SaveOrder(RegisterOrderIntegrationEvent result)
        {
            Order order = new()
            {
                OrderId = result.OrderId,
                UserEmail = result.UserEmail,
                ImageData = result.ImageData,
                PictureUrl = result.PictureUrl,
                Status = Status.Registered
            };
            _orderRepository.RegisterOrder(order);
            UpdateOrderStatusAsync(result.OrderId, Status.Registered);
        }

        private static async void UpdateOrderStatusAsync(Guid orderId, Status status)
        {
            var orderingProcessActor = GetOrderingProcessActorAsync(orderId);

            var orderStatus = new OrderStatus
            {
                OrderId = orderId,
                Status = status.ToString()
            };
            await orderingProcessActor.UpdateOrderStatus(orderStatus);
        }

        private static IOrderingProcessActor GetOrderingProcessActorAsync(Guid orderId)
        {
            var actorId = new ActorId(orderId.ToString());
            return ActorProxy.Create<IOrderingProcessActor>(actorId, nameof(OrderingProcessActor));
        }
    }
}
