using Faces.WebMVC.RestClients;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMVC.Controllers
{
    public class OrderManagementController : Controller
    {
        private readonly IOrderManagementAPI _orderManagementAPI;
        public OrderManagementController(IOrderManagementAPI orderManagementAPI)
        {
            _orderManagementAPI = orderManagementAPI;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _orderManagementAPI.GetOrders();
            foreach (var order in orders)
            {
                order.ImageString = ConvertAndFormatToString(order.ImageData);
            }
            return View(orders);
        }

        [Route("/Details/{orderId}")]
        public async Task<IActionResult> Details(string orderId)
        {
            var order = await _orderManagementAPI.GetOrderById(Guid.Parse(orderId));
            order.ImageString = ConvertAndFormatToString(order.ImageData);
            foreach(var detail in order.OrderDetails)
            {
                detail.ImageString = ConvertAndFormatToString(detail.FaceData);
            }
            return View(order);
        }

        private string ConvertAndFormatToString(byte[] imageData)
        {
            string imageBase64Data = Convert.ToBase64String(imageData);
            return string.Format("data:image/png;base64, {0}", imageBase64Data);
        }
    }
}
