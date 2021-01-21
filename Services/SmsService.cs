using ApiCallMethods;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PrabhuPayProdSms;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _config;
        readonly string NcellUserName = "";
        readonly string NcellPassword = "";
        readonly string NcellsmsSuccessResponse = "";


        BasicHttpBinding basicHttpBinding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IUtility> factory = null;

        private readonly IUtility client;
        //private readonly IHostingEnvironment _hostingEnvironment;
        public SmsService(IConfiguration config/*, IHostingEnvironment hostingEnvironment*/)
        {
            _config = config;
            NcellUserName = _config.GetSection("NCell").GetSection("NcellUserName").Value;
            NcellPassword = _config.GetSection("NCell").GetSection("NcellPassword").Value;
            NcellsmsSuccessResponse = _config.GetSection("NCell").GetSection("SmsSuccessResponse").Value;

            basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.MaxReceivedMessageSize = 2147483647;
            endpointAddress = new EndpointAddress("https://merchant.prabhupay.com/Api/Utility.svc?wsdl");
            factory = new ChannelFactory<IUtility>(basicHttpBinding, endpointAddress);
            client = factory.CreateChannel();
            //_hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> SendSMS(string mobileNumber, string message)
        {
            try
            {
                if (mobileNumber.Substring(0, 3).Equals("980") || mobileNumber.Substring(0, 3).Equals("981") || mobileNumber.Substring(0, 3).Equals("982"))
                {
                    return await SmsViaNcell(mobileNumber, message);
                }
                else
                {
                    return await Sms(mobileNumber, message);
                }
            }
            catch { }
            return "99";
        }

        private async Task<string> SmsViaNcell(string mobileNumber, string message)
        {
            try
            {
                WebApiRequestMethods<string, string> apiRequestMethod = new WebApiRequestMethods<string, string>("https://sms.prabhupay.com:13013/cgi-bin/sendsms");
                var response = await apiRequestMethod.GetAsync(string.Format("user=" + NcellUserName + "&pass=" + NcellPassword + "&to=977{0}&text={1}&smsc=NCELL31041", mobileNumber, message));
                if (response != null && response.Equals(NcellsmsSuccessResponse))
                {
                    return "00";
                }
            }
            catch { }
            return "99";
        }

        private async Task<string> Sms(string mobileNumber, string message)
        {
            try
            {
                if(!string.IsNullOrEmpty(mobileNumber) && !string.IsNullOrEmpty(message))
                {
                    var response = await client.SendSMSAsync(new InputSendSMS()
                    {
                        Message = message,
                        MobileNumber = mobileNumber,
                        UserName = "PrabhuTech1",
                        Password = "PjuWQ#Tv"
                    });
                    if (response.Code == "000")
                        return "00";
                    else
                        return "99";
                }
            }
            catch {}
            return "99";
        }
    }

    public interface ISmsService
    {
        Task<string> SendSMS(string mobileNumber, string message);
    }
}
