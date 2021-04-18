using Dapr;
using Messaging.InterfacesConstant.Dapr.Abstractions;
using Messaging.InterfacesConstant.Dapr.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OrdersAPI.IntegrationEvents.EventHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationEventController : ControllerBase
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";
        private readonly IServiceProvider _serviceProvider;

        public IntegrationEventController(IServiceProvider serviceProvider, IEventBus eventBus)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost("RegisterOrder")]
        [Topic(DAPR_PUBSUB_NAME, "RegisterOrderIntegrationEvent")]
        public async Task RegisterOrder(RegisterOrderIntegrationEvent message)
        {
            var handler = _serviceProvider.GetRequiredService<RegisterOrderEventHandler>();
            await handler.Handle(message);
        }

        [HttpPost("OrderDispatched")]
        [Topic(DAPR_PUBSUB_NAME, "OrderDispatchedIntegrationEvent")]
        public async Task OrderDispatched(OrderDispatchedIntegrationEvent message)
        {
            var handler = _serviceProvider.GetRequiredService<OrderDispatchedEventHandler>();
            await handler.Handle(message);
        }
    }
}
