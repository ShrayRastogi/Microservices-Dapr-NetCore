using Messaging.InterfacesConstant.Dapr.Events;
using System.Threading.Tasks;

namespace Messaging.InterfacesConstant.Dapr.Abstractions
{
    public interface IEventBus
    {
        Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event)
            where TIntegrationEvent : IntegrationEvent;
    }
}
