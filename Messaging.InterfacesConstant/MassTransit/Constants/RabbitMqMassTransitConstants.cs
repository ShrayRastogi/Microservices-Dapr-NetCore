using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.InterfacesConstant.MassTransit.Constants
{
    public class RabbitMqMassTransitConstants
    {
        public const string RabbitMqUri = "rabbitmq://rabbitmq:5672/";
        public const string UserName = "guest";
        public const string Password = "guest";
        public const string RegisterOrderCommandQueue = "register.order.command";
        public const string NotificationServiceQueue = "notification.service.queue";
        public const string OrderDispatchedServiceQueue = "order.dispatch.service.queue";
    }
}
