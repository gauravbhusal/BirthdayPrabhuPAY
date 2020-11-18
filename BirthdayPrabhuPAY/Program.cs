using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;

namespace BirthdayPrabhuPAY
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IBirthdayService, BirthdayService>();
                    services.AddSingleton<ISmsService, SmsService>();
                    services.AddHostedService<Worker>();
                });
    }
}
