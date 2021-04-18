using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.InterfacesConstant.Dapr.Events
{
    public class OrderDispatchedIntegrationEvent: IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public DateTime DispachtedDateTime { get; set; }
    }
}
