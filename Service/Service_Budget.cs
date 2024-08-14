using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Web.UI;

namespace Carrier.Service
{
    public class Service_Budget
    {
        public Return_Create_MainExpense Create_MainExpense(modelToCreate filter)
        {
            var client = new RestClient("https://www.sfg-th.com/API_Budget/POST_Create_Expense");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(filter);
            IRestResponse response = client.Execute(request);
            JObject j = JObject.Parse(response.Content);
            Return_Create_MainExpense return_model = new Return_Create_MainExpense();

            if (j["code"].ToString() == "200")
            {
                return_model.code = "S";
                return_model.message = j["message"].ToString();
                return_model.docno = j["docno"].ToString();
                return_model.datesend = Convert.ToDateTime(j["datesend"]);
            }
            else
            {
                return_model.code = "E";
                return_model.message = j["message"].ToString();
            }
            return return_model;
        }

        public string Insert_CutBudget(cuttemp filter)
        {

            var client = new RestClient("https://www.sfg-th.com/API_Budget/Insert_Cut_Budget");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { detail_id = filter.detail_id, depart_id = filter.depart_id, date_use = filter.date_use, money = filter.money, typeBudget_id = filter.typeBudget_id, userId = filter.userId, remark = filter.remark });
            IRestResponse response = client.Execute(request);
            JObject j = JObject.Parse(response.Content);
            if (j["code"].ToString() == "200")
            {
                JSAlert("S", j["message"].ToString());
            }
            return j["message"].ToString();
        }


        public string Encrypt(string clearText)
        {
            string EncryptionKey = "Staradmin2009";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
                {
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    };
                };
            }
            return clearText;
        }
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "Staradmin2009";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
                {
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    };
                };
            }
            return cipherText;
        }
        public void JSAlert(string typemsg, string alertText)
        {
            string typetoastr = "";
            switch (typemsg)
            {
                case "S":
                    typetoastr = "success";
                    break;

                case "E":
                    typetoastr = "error";
                    break;

                case "I":
                    typetoastr = "info";
                    break;
            }
            Page myPage = (Page)HttpContext.Current.Handler;
            string scriptText = (Convert.ToString("toastr." + typetoastr + "('") + alertText.Replace("'", "")) + "');";
            ScriptManager.RegisterStartupScript(myPage, myPage.GetType(), "script_ref_name", scriptText, true);
        }

        //Email

        readonly System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
        System.Net.Mail.MailMessage objMail;
        public string SendMail(string addressTo, string[] addressCC, string subject, string body)
        {
            var res = "";
            try
            {
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential("mis.starfashiongroup@gmail.com", "vabpxykmhsoldksk");
                using (objMail = new System.Net.Mail.MailMessage("mis.starfashiongroup@gmail.com", addressTo))
                {
                    objMail.From = new System.Net.Mail.MailAddress("mis.starfashiongroup@gmail.com", "Starfashion Group");
                    if (addressCC != null)
                    {
                        foreach (var i in addressCC)
                        {
                            objMail.Bcc.Add(i);
                        }
                    }
                    objMail.Priority = MailPriority.Normal;
                    objMail.IsBodyHtml = true;
                    objMail.SubjectEncoding = Encoding.GetEncoding("windows-874");
                    objMail.BodyEncoding = Encoding.GetEncoding("windows-874");
                    objMail.Subject = subject;
                    objMail.Body = body;
                    smtp.Send(objMail);
                }
                res = "success";
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;

        }

    }
    public class Return_Create_MainExpense
    {
        public string code { get; set; }
        public string message { get; set; }
        public string docno { get; set; }
        public DateTime datesend { get; set; }
    }
    public class modelToCreate
    {
        public DateTime Date_Send { get; set; }
        public string Department_ID { get; set; }
        public DateTime Date_Use { get; set; }
        public string OD_Docno { get; set; }
        public string Remark { get; set; }
        public string SAPRemark { get; set; }
        public int TypeBudget_ID { get; set; }
        public int UserID { get; set; }
        public string Detail { get; set; }
        public string Detail_ID { get; set; }
        public double Estimate { get; set; }
        public double Actual { get; set; }
        public string JobNo { get; set; }
        public string UserId_Approve { get; set; }
    }
}