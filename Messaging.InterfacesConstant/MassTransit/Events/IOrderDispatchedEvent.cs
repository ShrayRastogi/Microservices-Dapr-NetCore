using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.InterfacesConstant.MassTransit.Events
{
    public interface IOrderDispatchedEvent
    {
        Guid OrderId { get; }
        DateTime DispachtedDateTime { get; }
    }
}
