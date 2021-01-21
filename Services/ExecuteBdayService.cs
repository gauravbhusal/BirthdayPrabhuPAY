using BirthdayPrabhuPAY.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ExecuteBdayService : IExecuteBdayService
    {
        private readonly IBirthdayService _birthdayService;
        private readonly ISmsService _smsService;


        public ExecuteBdayService(IBirthdayService birthdayService, ISmsService smsService)
        {
            _birthdayService = birthdayService;
            _smsService = smsService;
        }
        public async Task SendSms()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine("************Exception Error****************");
                Console.WriteLine(ex.Message);
            }
        }
    }

    public interface IExecuteBdayService
    {
        Task SendSms();
    }
}
