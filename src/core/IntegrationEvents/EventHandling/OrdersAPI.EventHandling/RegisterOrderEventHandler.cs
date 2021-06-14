using Binding.Abstractions;
using Binding.Messages;
using Dapr.Client;
using EventBus.Abstractions;
using Events;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrdersAPI.Database.Models;
using OrdersAPI.Database.Repositories;
using StateStore;
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
        private readonly ICommonStateStore _stateStore;
        private readonly IBinding _binding;
        public RegisterOrderEventHandler(IOrderRepository orderRepository, IHttpClientFactory clientFactory, IEventBus eventBus, ICommonStateStore stateStore, IBinding binding)
        {
            _orderRepository = orderRepository;
            _clientFactory = clientFactory;
            _stateStore = stateStore;
            _eventBus = eventBus;
            _binding = binding;
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
                var bindingEvent = new BindingEvent();
                await _eventBus.PublishAsync<OrderProcessedIntegrationEvent>(processedOrder);
                await _binding.CreateBindingAsync<BindingEvent>(bindingEvent);
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

        private async void UpdateOrderStatusAsync(Guid orderId, Status status)
        { 
            var prevOrderStatus = await _stateStore.GetStateAsync<OrderStatus>(orderId.ToString());
            bool shouldUpdateOrderStatus = (prevOrderStatus == null) || status.ToString() switch
            {
                "Registered" => !prevOrderStatus.Status.Equals("Processed") &&
                              !prevOrderStatus.Status.Equals("Sent"),
                "Processed" => !prevOrderStatus.Status.Equals("Sent"),
                "Sent" => !prevOrderStatus.Status.Equals("Sent"),
                _ => false,
            };
            if (shouldUpdateOrderStatus)
            {
                if(prevOrderStatus == null)
                {
                    prevOrderStatus = new OrderStatus
                    {
                        OrderId = orderId,
                        Status = status.ToString()
                    };
                } else
                {
                    prevOrderStatus.Status = status.ToString();
                }          
                await _stateStore.UpdateStateAsync(prevOrderStatus, orderId.ToString());
            }
            else
            {
                throw new Exception("Invalid Order Status");
            }
        }
    }
}
