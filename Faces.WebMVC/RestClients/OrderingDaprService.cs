using Faces.WebMVC.ViewModels;
using Messaging.InterfacesConstant.Dapr.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Faces.WebMVC.RestClients
{
    public class OrderingDaprService : IOrderingDaprService
    {
        //private readonly HttpClient _httpClient;
        //public async Task CreateOrderAsync(RegisterOrderIntegrationEvent message)
        //{
        //    var requestUri = "api/v1/orders/createorder";
        //    var response = await _httpClient.PostAsJsonAsync(requestUri, message);

        //    response.EnsureSuccessStatusCode();
        //}

        //public Task<OrderViewModel> GetOrderById(Guid orderId)
        //{
        //    var requestUri = "api/v1/orders/createorder";
        //   return _httpClient.GetFromJsonAsync<OrderViewModel>(requestUri);
        //}

        //public Task<List<OrderViewModel>> GetOrders()
        //{
        //    var requestUri = $"api/v1/orders/items?ids";
        //    return _httpClient.GetFromJsonAsync<List<OrderViewModel>>(requestUri);
        //}
    }
}
