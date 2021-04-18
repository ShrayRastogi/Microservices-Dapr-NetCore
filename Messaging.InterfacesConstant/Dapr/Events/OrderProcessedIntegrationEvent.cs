using System;
using System.Collections.Generic;

namespace Messaging.InterfacesConstant.Dapr.Events
{
    public class OrderProcessedIntegrationEvent: IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string PictureUrl { get; set; }
        public List<byte[]> Faces { get; set; }
        public string UserEmail { get; set; }
    }
}
