using Events;
using FacesWebApp.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FacesWebApp.RestClients
{
    public class OrderManagementAPI : IOrderManagementAPI
    {
        private readonly IOrderManagementAPI _orderManagementRestClient;
        private readonly IOptions<AppSettings> _appSettings;
        public OrderManagementAPI(IConfiguration config, HttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            string ordersAPILocation = _appSettings.Value.OrdersAPIUrl;
            httpClient.BaseAddress = new Uri($"{ordersAPILocation}");
            _orderManagementRestClient = RestService.For<IOrderManagementAPI>(httpClient);

        }

        public Task CreateOrderAsync(RegisterOrderIntegrationEvent message)
        {
            return _orderManagementRestClient.CreateOrderAsync(message);
        }

        public async Task<OrderViewModel> GetOrderById(Guid orderId)
        {
            try
            {
                return await _orderManagementRestClient.GetOrderById(orderId);
            }
            catch (ApiException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<List<OrderViewModel>> GetOrders()
        {
            return await _orderManagementRestClient.GetOrders();
        }
    }
}
