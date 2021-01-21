using BirthdayPrabhuPAY.ViewModel;
using Microsoft.Extensions.Hosting;
using Services;
using System;
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
                    //await SendBirthdaySMS();
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
                    SmsLogViewModel smsModel = new SmsLogViewModel()
                    {
                        MobileNumber = mobNo,
                        Message = message,
                        ResponseCode = smsResp,
                        Remarks = "Birthday Sms"
                    };
                    _birthdayService.InsertSmsDataLogs(smsModel);
                    Console.WriteLine($"\n SMS to {item.MobileNumber}. Status: {smsResp}");
                }
                Console.WriteLine("****************************");
                Console.WriteLine("Completed. Number of customer found: " + response.Item2.Count());
            }
        }

    }
}
