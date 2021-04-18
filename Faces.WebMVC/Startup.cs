using Faces.WebMVC.RestClients;
using Faces.WebMVC.Services;
using MassTransit;
//using Messaging.InterfacesConstant.Dapr;
//using Messaging.InterfacesConstant.Dapr.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Faces.WebMVC
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
            services.Configure<AppSettings>(Configuration);            
            services.AddMassTransit();
            // services.AddDaprClient();
            // services.AddScoped<IEventBus, DaprEventBus>();
            services.AddHttpClient<IOrderManagementAPI, OrderManagementAPI>();
            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("rabbitmq", "/", h => { });
                services.AddSingleton<IBusControl>(provider => provider.GetRequiredService<IBusControl>());
                services.AddSingleton<IHostedService, BusService>();

            }));
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
