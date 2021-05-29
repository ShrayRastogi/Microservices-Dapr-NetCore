using EventBus;
using EventBus.Events;
using System;
using System.Collections.Generic;

namespace Events
{
    public class OrderProcessedIntegrationEvent: IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string PictureUrl { get; set; }
        public List<byte[]> Faces { get; set; }
        public string UserEmail { get; set; }
    }
}
