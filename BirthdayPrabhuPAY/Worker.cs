using BirthdayPrabhuPAY.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;
using Services.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
                    //_birthdayService.Log(new LogInfo { Message = message, MobileNumber = mobNo, Status = smsResp });
                    SmsLogViewModel smsModel = new SmsLogViewModel()
                    {
                        MobileNumber = mobNo,
                        Message = message,
                        ResponseCode = smsResp,
                        Remarks = "Birthday Sms"
                    };
                    _birthdayService.InsertSmsDataLogs(smsModel);
                    //LogToTextFile(mobNo, message, smsResp);
                    Console.WriteLine($"\n SMS to {item.MobileNumber}. Status: {smsResp}");
                }
                Console.WriteLine("****************************");
                Console.WriteLine("Completed. Number of customer found: " + response.Item2.Count());
            }
        }

        private void LogToTextFile(string mobNo, string message, string smsResp)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{mobNo} :: {message} :: {smsResp}");
            File.AppendAllText("./" + "log.txt", sb.ToString());
            sb.Clear();
        }
    }
}
