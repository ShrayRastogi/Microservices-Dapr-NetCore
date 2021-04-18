using Messaging.InterfacesConstant.Dapr.Events;
using System.Threading.Tasks;

namespace Messaging.InterfacesConstant.Dapr.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
