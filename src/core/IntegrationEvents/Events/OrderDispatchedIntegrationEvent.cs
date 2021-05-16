using EventBus;
using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
    public class OrderDispatchedIntegrationEvent: IntegrationEvent
    {
        public Guid OrderId { get; set; }
        public DateTime DispachtedDateTime { get; set; }
    }
}
