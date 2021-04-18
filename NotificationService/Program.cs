using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using EmailService;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using NotificationService.Consumers;
using System;
using GreenPipes;
using Messaging.InterfacesConstant.MassTransit.Constants;

namespace NotificationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var host = CreateHostBuilder(args).Build();
            //await host.RunAsync();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost => {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile($"appSettings.json", optional: false);
                    configHost.AddEnvironmentVariables();
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile($"appSettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var emailConfig = hostContext.Configuration.
                    GetSection("EmailConfiguration").Get<EmailConfig>();
                    services.AddSingleton(emailConfig);
                    services.AddScoped<IEmailSender, EmailSender>();
                    services.AddMassTransit(c =>
                    {
                        c.AddConsumer<OrderProcessedEventConsumer>();
                    });
                    services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        cfg.Host("rabbitmq", "/", h => { });
                        cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.NotificationServiceQueue, e =>
                        {
                            e.PrefetchCount = 16;
                            e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
                            e.Consumer<OrderProcessedEventConsumer>(provider);
                        });
                    }));

                    services.AddSingleton<IHostedService, BusService>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
