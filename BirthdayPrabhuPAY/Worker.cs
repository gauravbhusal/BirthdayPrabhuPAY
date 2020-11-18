using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;
using Services.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BirthdayPrabhuPAY
{
    public class Worker : BackgroundService
    {
        private readonly IBirthdayService _birthdayService;
        private readonly ISmsService _smsService;


        public Worker(IBirthdayService birthdayService, ISmsService smsService)
        {
            _birthdayService = birthdayService;
            _smsService = smsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var go = true;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (go)
                {
                    await SendBirthdaySMS();
                    go = false;
                }
            }
        }

        private async Task SendBirthdaySMS()
        {
            var response = _birthdayService.GetCustomers();

            if (response.Item2.Count() > 0)
            {
                foreach (var item in response.Item2)
                {
                    string mobNo = item.MobileNumber, message = response.Item1;
                    var smsResp = await _smsService.SendSMS(mobNo, message);
                    _birthdayService.Log(new LogInfo { Message = message, MobileNumber = mobNo, Status = smsResp });
                }
            }
        }
    }
}
