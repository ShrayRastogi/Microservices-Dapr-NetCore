using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using EventBus.Abstractions;
using Events;
using NotificationAPI.StateStore;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotificationAPI.EventHandling
{
    public class OrderProcessedEventHandler
    {
        private readonly IEventBus _eventBus;
        public OrderProcessedEventHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        public async Task Handle(OrderProcessedIntegrationEvent result)
        {
            var orderStatus = await GetOrderingProcessActorAsync(result.OrderId).GetOrderStatus();
            if (orderStatus != null && orderStatus.Status != "Processed")
            {
                throw new Exception("Could not process order due to invalid status - " + orderStatus.Status);
            }

            var facesData = result.Faces;
            var rootFolder = AppContext.BaseDirectory;
            Console.WriteLine("Root Directory is - " + rootFolder);
            if (facesData.Count < 1)
            {
                await Console.Out.WriteLineAsync($"No Faces Detected");
            }
            else
            {
                int j = 0;
                foreach (var face in facesData)
                {
                    MemoryStream ms = new(face);
                    var image = Image.Load(ms.ToArray());
                    image.Save(rootFolder + "face" + j + ".jpg");
                    j++;
                }
            }

            var dispatchedOrder = new OrderDispatchedIntegrationEvent
            {
                DispachtedDateTime = DateTime.UtcNow,
                OrderId = result.OrderId
            };
            await _eventBus.PublishAsync<OrderDispatchedIntegrationEvent>(dispatchedOrder);
        }        

        private static INotificationProcessActor GetOrderingProcessActorAsync(Guid orderId)
        {
            var actorId = new ActorId(orderId.ToString());
            return ActorProxy.Create<INotificationProcessActor>(actorId, nameof(NotificationProcessActor));
        }
    }
}
