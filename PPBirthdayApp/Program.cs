using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PPBirthdayApp
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            var bar = serviceProvider.GetService<IExecuteBdayService>();
            await bar.SendSms();
            Console.ReadLine();
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            // add services:
            services.AddSingleton<IConfiguration>(configuration);
            services.AddTransient<IBirthdayService, BirthdayService>();
            services.AddTransient<ISmsService, SmsService>();
            services.AddSingleton<IExecuteBdayService, ExecuteBdayService>();

        }
    }
}
