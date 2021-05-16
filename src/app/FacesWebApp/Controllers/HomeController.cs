using Events;
using FacesWebApp.Models;
using FacesWebApp.RestClients;
using FacesWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FacesWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // private readonly IEventBus _eventBus;
        private readonly IOrderManagementAPI _orderManagementAPI;
        public HomeController(ILogger<HomeController> logger, IOrderManagementAPI orderManagementAPI)
        {
            _logger = logger;
            _orderManagementAPI = orderManagementAPI;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterOrder(OrderViewModel model)
        {
            MemoryStream ms = new();
            using var uploadedFile = model.ImageFile.OpenReadStream();
            await uploadedFile.CopyToAsync(ms);
            model.ImageData = ms.ToArray();
            model.PictureUrl = model.ImageFile.FileName;
            model.OrderId = Guid.NewGuid();
            var message = new RegisterOrderIntegrationEvent
            {
                OrderId = model.OrderId,
                UserEmail = model.UserEmail,
                ImageData = model.ImageData,
                PictureUrl = model.PictureUrl
            };
            //var sendToUri = new Uri($"{RabbitMqMassTransitConstants.RabbitMqUri}"
            //    + $"{RabbitMqMassTransitConstants.RegisterOrderCommandQueue }");
            //var endpoint = await _busControl.GetSendEndpoint(sendToUri);
            //await endpoint.Send<IRegisterOrderCommand>(new
            //{
            //    model.OrderId,
            //    model.UserEmail,
            //    model.ImageData,
            //    model.PictureUrl
            //});
            // await _eventBus.PublishAsync<RegisterOrderIntegrationEvent>(message);
            await _orderManagementAPI.CreateOrderAsync(message);
            ViewData["OrderId"] = model.OrderId;
            return View("Thanks");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
