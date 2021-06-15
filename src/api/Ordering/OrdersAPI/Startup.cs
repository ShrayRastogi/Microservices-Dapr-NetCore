using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Binding;
using Binding.Abstractions;
using EventBus;
using EventBus.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OrdersAPI.Database;
using OrdersAPI.Database.Repositories;
using OrdersAPI.EventHandling;
using StateStore;

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
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials());
            });
            services.Configure<OrderSettings>(Configuration);
            services.AddDbContext<OrdersContext>(options => options.UseSqlServer(
                Configuration["OrdersContext"]));
            services.AddHttpClient();
            services.AddScoped<IEventBus, DaprEventBus>();
            services.AddScoped<IBinding, DaprBinding>();
            services.AddTransient<RegisterOrderEventHandler>();
            services.AddTransient<OrderDispatchedEventHandler>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<ICommonStateStore, CommonStateStore>();
            services.AddControllers().AddDapr();
            services.AddActors(options =>
            {
                options.Actors.RegisterActor<CommonActor>();
            });
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
            app.UseHttpsRedirection();            
            app.UseRouting();
            app.UseCloudEvents();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapActorsHandlers();
                endpoints.MapControllers();
            });
        }

        // If database context is in another library project, then need to implement IDesignTimeDbContextFactory
        // so that .Net Core looks for it and ignores default initialization of db context.
        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<OrdersContext>
        {
            public OrdersContext CreateDbContext(string[] args)
            {
                var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

                var optionsBuilder = new DbContextOptionsBuilder<OrdersContext>();

                var connectionString = configuration["OrdersContext"];

                optionsBuilder.UseSqlServer(connectionString);

                return new OrdersContext(optionsBuilder.Options);
            }
        }
    }
}
