using System;

namespace Messaging.InterfacesConstant.Dapr.Events
{
    public class RegisterOrderIntegrationEvent: IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public string PictureUrl { get; set; }
        public string UserEmail { get; set; }
        public byte[] ImageData { get; set; }
    }
}
