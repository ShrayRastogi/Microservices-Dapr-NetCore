using Faces.WebMVC.ViewModels;
using Messaging.InterfacesConstant.Dapr.Events;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMVC.RestClients
{
    public interface IOrderManagementAPI
    {
        [Get("/o/api/orders")]
        Task<List<OrderViewModel>> GetOrders();
        [Get("/o/api/orders/{orderId}")]
        Task<OrderViewModel> GetOrderById(Guid orderId);
        [Post("/o/api/orders/createorder")]
        Task CreateOrderAsync([Body] RegisterOrderIntegrationEvent message);
    }
}
