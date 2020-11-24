using System;
using System.Collections.Generic;
using System.Text;

namespace BirthdayPrabhuPAY.ViewModel
{
    public class SmsLogViewModel
    {
        public string MobileNumber { get; set; }
        public string CustomerId { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string LastUpdatedById { get; set; }
        public string Remarks { get; set; }
        public string Flag { get; set; } = "INSERT";
    }
}
