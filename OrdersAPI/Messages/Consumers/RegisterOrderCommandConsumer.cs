using MassTransit;
using Messaging.InterfacesConstant.MassTransit.Commands;
using Messaging.InterfacesConstant.MassTransit.Events;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrdersAPI.Models;
using OrdersAPI.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OrdersAPI.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<OrderSettings> _orderSettings;
        public RegisterOrderCommandConsumer(IOrderRepository orderRepository, IHttpClientFactory clientFactory, IOptions<OrderSettings> orderSettings)
        {
            _orderRepository = orderRepository;
            _clientFactory = clientFactory;
            _orderSettings = orderSettings;
        }
        public async Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            var result = context.Message;
            if (result.OrderId != Guid.Empty
                && result.PictureUrl != null
                && result.ImageData != null 
                && result.UserEmail != null)
            {
                SaveOrder(result);
                var client = _clientFactory.CreateClient();
                Tuple<List<byte[]>, Guid> orderDetail = await GetFacesFromFacesAPIAsync(client, result.ImageData, result.OrderId);
                List<byte[]> faces = orderDetail.Item1;
                Guid orderId = orderDetail.Item2;
                SaveOrderDetails(orderId, faces);
                await context.Publish<IOrderProcessedEvent>(new
                {
                    OrderId = orderId,
                    result.UserEmail,
                    Faces = faces,
                    result.PictureUrl
                });
            }
        }

        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFacesAPIAsync(HttpClient client, byte[] imageData, Guid orderId)
        {
            var urlAddress = _orderSettings.Value.FacesAPIUrl + "/api/faces/?orderId=" + orderId;
            var uri = new Uri(urlAddress);
            var byteContent = new ByteArrayContent(imageData);
            Tuple<List<byte[]>, Guid> orderDetail = null;
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            using var res = await client.PostAsync(uri, byteContent);
            string apiResponse = await res.Content.ReadAsStringAsync();
            orderDetail = JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);
            return orderDetail;
        }

        private void SaveOrderDetails(Guid orderId, List<byte[]> faces)
        {
            var order = _orderRepository.GetOrderAsync(orderId).Result;
            if(order != null)
            {
                order.Status = Status.Processed;
                foreach(var face in faces)
                {
                    OrderDetail orderDetail = new()
                    {
                        FaceData = face,
                        OrderId = orderId
                    };
                    order.OrderDetails.Add(orderDetail);
                }
                _orderRepository.UpdateOrder(order);
            }
        }

        private void SaveOrder(IRegisterOrderCommand result)
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
        }
    }
}
