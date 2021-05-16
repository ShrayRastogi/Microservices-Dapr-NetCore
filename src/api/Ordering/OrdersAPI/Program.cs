using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrdersAPI.Database;

namespace OrdersAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // CreateHostBuilder(args).Build().Run();
            var configuration = GetConfiguration();

            // Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                var host = CreateHostBuilder(configuration, args);
                using (var scope = host.Services.CreateScope())
                {
                    // Get the DbContext instance
                    var myDbContext = scope.ServiceProvider.GetRequiredService<OrdersContext>();

                    //Do the migration
                    myDbContext.MigrateDB();
                }
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in OrdersAPI - Program terminated unexpectedly" + ex.Message);
            }
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });

        private static IWebHost CreateHostBuilder(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .CaptureStartupErrors(false)
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .Build();

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
