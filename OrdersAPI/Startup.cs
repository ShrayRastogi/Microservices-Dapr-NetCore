using GreenPipes;
using MassTransit;
using Messaging.InterfacesConstant.Dapr;
using Messaging.InterfacesConstant.Dapr.Abstractions;
using Messaging.InterfacesConstant.MassTransit.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OrdersAPI.IntegrationEvents.EventHandling;
using OrdersAPI.Messages.Consumers;
using OrdersAPI.Persistence;
using OrdersAPI.Services;
using System;

namespace OrdersAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<OrderSettings>(Configuration);
            services.AddDbContext<OrdersContext>(options => options.UseSqlServer(
                Configuration["OrdersContext"]));
            services.AddHttpClient();
            services.AddDaprClient();
            services.AddScoped<IEventBus, DaprEventBus>();
            services.AddTransient<RegisterOrderEventHandler>();
            services.AddTransient<OrderDispatchedEventHandler>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddMassTransit(c =>
            {
                c.AddConsumer<RegisterOrderCommandConsumer>();
                c.AddConsumer<OrderEventDispatchedConsumer>();
            });
            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("rabbitmq", "/", h => { });
                cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
                    e.Consumer<RegisterOrderCommandConsumer>(provider);
                });
                cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.OrderDispatchedServiceQueue, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
                    e.Consumer<OrderEventDispatchedConsumer>(provider);
                });

            }));
            services.AddSingleton<IHostedService, BusService>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials());
            });
            services.AddControllers().AddDapr();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrdersAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrdersAPI v1"));
            }
            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseCloudEvents();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
            });

            //Migrate DB
            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetService<OrdersContext>().MigrateDB();
        }
    }
}
