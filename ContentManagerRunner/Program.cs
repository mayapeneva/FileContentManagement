using FileContentManagement.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace ContentManagerRunner
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json",
                    optional: false,
                    reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.RegisterContentManager<Guid>(hostContext.Configuration);
                });
    }
}
