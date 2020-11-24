using ApiCallMethods;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _config;
        string NcellUserName = "";
        string NcellPassword = "";
        string NcellsmsSuccessResponse = "";
        public SmsService(IConfiguration config)
        {
            _config = config;
            NcellUserName = _config.GetSection("NCell").GetSection("NcellUserName").Value;
            NcellPassword = _config.GetSection("NCell").GetSection("NcellPassword").Value;
            NcellsmsSuccessResponse = _config.GetSection("NCell").GetSection("SmsSuccessResponse").Value;
        }

        public async Task<string> SendSMS(string mobileNumber, string message)
        {
            try
            {
                WebApiRequestMethods<string, string> apiRequestMethod = new WebApiRequestMethods<string, string>("https://sms.prabhupay.com:13013/cgi-bin/sendsms");
                var response = await apiRequestMethod.GetAsync(string.Format("user=" + NcellUserName + "&pass=" + NcellPassword + "&to=977{0}&text={1}&smsc=NCELL31041", mobileNumber, message));
                if (response != null && response.Equals(NcellsmsSuccessResponse))
                {
                    return "00";
                }
                return "99";
            }
            catch (Exception ex)
            {
                return "99";
            }
        }
    }

    public interface ISmsService
    {
        Task<string> SendSMS(string mobileNumber, string message);
    }
}
