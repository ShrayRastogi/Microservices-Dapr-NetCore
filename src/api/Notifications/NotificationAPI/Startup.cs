using Dapr.Client;
using Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationAPI.EventHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotificationAPI
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
            // services.AddDaprClient();
            services.AddTransient<OrderProcessedEventHandler>();
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
                var result = await JsonSerializer.DeserializeAsync<OrderProcessedIntegrationEvent>(context.Request.Body, serializerOptions);
                var handler = context.RequestServices.GetRequiredService<OrderProcessedEventHandler>();
                await handler.Handle(result);
            }
        }
    }
}
