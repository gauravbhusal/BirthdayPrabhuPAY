using BirthdayPrabhuPAY.ViewModel;
using Microsoft.Extensions.Configuration;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Services
{
    public class BirthdayService : IBirthdayService
    {
        private readonly IConfiguration _config;
        readonly string dbConn = "";
        readonly string LogDbConn = "";

        public BirthdayService(IConfiguration config)
        {
            _config = config;
            dbConn = _config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            LogDbConn = _config.GetSection("ConnectionStrings").GetSection("LogDb").Value;
        }

        public Tuple<string, List<CustomerInfo>> GetCustomers()
        {
            try
            {
                List<CustomerInfo> lists = new List<CustomerInfo>();
                string message = "";

                using (SqlConnection con = new SqlConnection(dbConn))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "GetCustomerBirthday";
                        con.Open();

                        DataSet ds = new DataSet();
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        sda.Fill(ds);

                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            CustomerInfo customer = new CustomerInfo();
                            customer.Name = item["CustomerName"].ToString();
                            customer.MobileNumber = item["MobileNumber"].ToString();
                            lists.Add(customer);
                        }

                        message = ds.Tables[1].Rows[0].ItemArray[0].ToString();
                    }
                }

                return Tuple.Create(message, lists);
            }
            catch (Exception ex)
            {
                return Tuple.Create("99", new List<CustomerInfo>());
            }
        }

        public string Log(LogInfo log)
        {
            try
            {
                SqlConnection con = new SqlConnection(LogDbConn);
                con.Open();
                SqlCommand cmd = new SqlCommand("Insert Into Table_1(Id, MobileNumber, Message, Status, CreatedDate) values(NEWID(), @mobileNumber, @message, @status, @createdDate)", con);
                cmd.Parameters.AddWithValue("status", log.Status);
                cmd.Parameters.AddWithValue("message", log.Message);
                cmd.Parameters.AddWithValue("mobileNumber", log.MobileNumber);
                cmd.Parameters.AddWithValue("createdDate", DateTime.UtcNow);
                SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                con.Close();

                return "00";
            }
            catch (Exception ex)
            {
                return "99";
            }
        }

        public void InsertSmsDataLogs(SmsLogViewModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(LogDbConn))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "InsertSmsLogs";

                        cmd.Parameters.AddWithValue("mobileNumber", model.MobileNumber);
                        cmd.Parameters.AddWithValue("customerId", model.CustomerId);
                        cmd.Parameters.AddWithValue("message", model.Message);
                        cmd.Parameters.AddWithValue("messageType", model.MessageType);
                        cmd.Parameters.AddWithValue("transactionId", model.TransactionId);
                        cmd.Parameters.AddWithValue("responseCode", model.ResponseCode);
                        cmd.Parameters.AddWithValue("responseMessage", model.ResponseMessage);
                        cmd.Parameters.AddWithValue("remarks", model.Remarks);
                        cmd.Parameters.AddWithValue("lastUpdatedById", model.LastUpdatedById);
                        cmd.Parameters.AddWithValue("flag", model.Flag);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

    public interface IBirthdayService
    {
        Tuple<string, List<CustomerInfo>> GetCustomers();
        string Log(LogInfo log);
        void InsertSmsDataLogs(SmsLogViewModel model);
    }
}
