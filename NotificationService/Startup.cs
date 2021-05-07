using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;
using SixLabors.ImageSharp;
using Messaging.InterfacesConstant.Dapr.Events;
using Messaging.InterfacesConstant.Dapr.Abstractions;
using Dapr.Client;

namespace NotificationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures Services.
        /// </summary>
        /// <param name="services">Service Collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDaprClient();            
            services.AddSingleton(new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, JsonSerializerOptions serializerOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCloudEvents();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapPost("OrderProcessed", HandleOrderProcessedEvent).WithTopic("pubsub", "OrderProcessedIntegrationEvent");
            });

            async Task HandleOrderProcessedEvent(HttpContext context)
            {
                Console.WriteLine("Enter HandleOrderProcessedEvent");
                var client = context.RequestServices.GetRequiredService<DaprClient>();
                var rootFolder = AppContext.BaseDirectory;
                Console.WriteLine("Root Directory is - " + rootFolder);
                var result = await JsonSerializer.DeserializeAsync<OrderProcessedIntegrationEvent>(context.Request.Body, serializerOptions);
                var facesData = result.Faces;
                if (facesData.Count < 1)
                {
                    await Console.Out.WriteLineAsync($"No Faces Detected");
                }
                else
                {
                    int j = 0;
                    foreach (var face in facesData)
                    {
                        MemoryStream ms = new(face);
                        var image = Image.Load(ms.ToArray());
                        image.Save(rootFolder + "face" + j + ".jpg");
                        j++;
                    }
                }

                // string[] mailAddress = { result.UserEmail };
                //await _emailSender.SendEmailAsync(new Message
                //    (mailAddress, "Your Order " + result.OrderId, "From Faces and Faces", facesData));
                var dispatchedOrder = new OrderDispatchedIntegrationEvent
                {
                    DispachtedDateTime = DateTime.UtcNow,
                    OrderId = result.OrderId
                };
                await client.PublishEventAsync<OrderDispatchedIntegrationEvent>("pubsub", "OrderDispatchedIntegrationEvent", dispatchedOrder);
            }
        }
    }
}
