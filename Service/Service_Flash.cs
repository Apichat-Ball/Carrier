using Carrier.Info;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Online_Lazada;
using Carrier.Model.Whale;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Carrier.Service
{
    public class Service_Flash
    {
        WhaleEntities entities_Whale;
        Online_LazadaEntities entities_Online_Lazada;
        CarrierEntities entities_Carrier;
        InsideSFG_WFEntities entities_InsideSFG_WF;
        public Service_Flash()
        {
            entities_Online_Lazada = new Online_LazadaEntities();
            entities_Whale = new WhaleEntities();
            entities_Carrier = new CarrierEntities();
            entities_InsideSFG_WF = new InsideSFG_WFEntities();
        }
        public Model_Trackingno CreateOrderFLASH(string docno)
        {
            var obj = entities_Carrier.Orders.Where(w => w.Docno == docno).ToList();

            if (obj.Count > 0)
            {

                var objOrder = obj.FirstOrDefault();

                Model_Key model_key = Get_Key("FLASH", "FLASH");
                string headerpara = "articleCategory=" + objOrder.articleCategory +
                                    "&codEnabled=0" +
                                    "&dstCityName=" + objOrder.dstCityName +
                                    "&dstDetailAddress=" + (objOrder.dstDetailAddress == null ? "" : objOrder.dstDetailAddress)
                                        + (objOrder.dstDistrictName != null && objOrder.dstDistrictName != "" ? " " + objOrder.dstDistrictName : "")
                                        + (objOrder.dstCityName != null && objOrder.dstCityName != "" ? " " + objOrder.dstCityName : "")
                                        + (objOrder.dstProvinceName != null && objOrder.dstProvinceName != "" ? " " + objOrder.dstProvinceName : "") +
                                    "&dstName=" + objOrder.dstName +
                                    "&dstPhone=" + objOrder.dstPhone +
                                    "&dstPostalCode=" + (objOrder.dstPostalCode != null ? objOrder.dstPostalCode.ToString() : "") +
                                    "&dstProvinceName=" + objOrder.dstProvinceName +
                                    "&expressCategory=1" +
                                    "&insured=0" +
                                    "&mchId=" + model_key.mchId +
                                    "&nonceStr=" + docno +
                                    "&outTradeNo=" + docno +
                                    "&remark=" + (objOrder.remark != "" ? objOrder.remark : "-") +
                                    "&srcCityName=" + objOrder.srcCityName +
                                    "&srcDetailAddress=" + (objOrder.srcDetailAddress != null && objOrder.srcDetailAddress != "" ? objOrder.srcDetailAddress : "")
                                    + (objOrder.srcDistrictName != null && objOrder.srcDistrictName != "" ? " " + objOrder.srcDistrictName : "")
                                    + (objOrder.srcCityName != null && objOrder.srcCityName != "" ? " " + objOrder.srcCityName : "")
                                    + (objOrder.srcProvinceName != null && objOrder.srcProvinceName != "" ? " " + objOrder.srcProvinceName : "") +
                                    "&srcName=" + objOrder.srcName +
                                    "&srcPhone=" + objOrder.srcPhone +
                                    "&srcPostalCode=" + objOrder.srcPostalCode +
                                    "&srcProvinceName=" + objOrder.srcProvinceName +
                                    "&weight=1";
                string sign = sha256_hash(headerpara + "&key=" + model_key.key).ToUpper();
                var client = new RestClient("https://api.flashexpress.com/open/v3/orders?" + headerpara + "&sign=" + sign);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                IRestResponse response = client.Execute(request);
                JObject j = JObject.Parse(response.Content);

                if (Convert.ToInt32(j["code"]) == 1)
                {
                    Order_Item order = new Order_Item
                    {
                        Docno = docno,
                        Date_Success = DateTime.Now,
                        sign = sign,
                        pno = j["data"]["pno"].ToString(),
                        mchId = j["data"]["mchId"].ToString(),
                        sortCode = j["data"]["sortCode"].ToString(),
                        dstStoreName = j["data"]["dstStoreName"].ToString(),
                        sortingLineCode = j["data"]["sortingLineCode"].ToString(),
                        Qty = 1,
                        earlyFlightEnabled = j["data"]["earlyFlightEnabled"].ToString(),
                        packEnabled = j["data"]["packEnabled"].ToString(),
                        upcountryCharge = j["data"]["upcountryCharge"].ToString()
                    };
                    entities_Carrier.Order_Item.Add(order);
                    entities_Carrier.SaveChanges();
                    return new Model_Trackingno { success = true, trackingno = j["data"]["pno"].ToString() };
                }
                else
                {
                    var selectOrder = entities_Carrier.Orders.Where(w => w.Docno == docno).FirstOrDefault();
                    entities_Carrier.Orders.Remove(selectOrder);
                    entities_Carrier.SaveChanges();
                    return new Model_Trackingno { success = false, trackingno = j["message"].ToString() };
                }
            }
            else
            {
                return new Model_Trackingno { success = false, trackingno = "Error" };
            }

        }
        public Model_Key Get_Key(string accountapi, string appname)
        {
            Model_Key model_key = new Model_Key();

            var objAPI_Key = (from tAPI_Key in entities_Online_Lazada.API_Key
                              where tAPI_Key.Brand == accountapi
                              && tAPI_Key.App_Name == appname
                              select tAPI_Key
                              ).FirstOrDefault();
            if (objAPI_Key != null)
            {
                model_key.mchId = objAPI_Key.Shop_ID;
                model_key.key = objAPI_Key.App_Key;
            }

            return model_key;
        }
        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }


            return Sb.ToString();
        }
        public static String MD5_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();
            using (MD5 hash = MD5.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
        public string Get_Docment(string docno)
        {
            var orderItem = entities_Carrier.Order_Item.Where(w => w.Docno == docno).FirstOrDefault();
            string trackingno = orderItem.pno;

            Model_Key model_key = Get_Key("FLASH", "FLASH");

            string headerpara = "mchId=" + model_key.mchId + "&nonceStr=" + orderItem.Docno;
            string sign = sha256_hash(headerpara + "&key=" + model_key.key).ToUpper();
            HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create("https://api.flashexpress.com/open/v1/orders/" + trackingno + "/pre_print?" + headerpara + "&sign=" + sign);
            fileReq.Method = "POST";
            var resp = fileReq.GetResponse();
            Stream input = resp.GetResponseStream();
            string filepath = HttpContext.Current.Server.MapPath("PDFFile/");
            DirectoryInfo d = new DirectoryInfo(filepath);

            String originalPath = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
            String parentDirectory = originalPath.Substring(0, originalPath.LastIndexOf("/Transport_Form")) + "/PDFFile/" + docno + ".pdf?" + DateTime.Now.ToString();
            string dataDir = HttpContext.Current.Server.MapPath("PDFFile/") + docno + ".pdf";
            if (File.Exists(dataDir))
            {
                return parentDirectory;
            }
            else
            {
                byte[] data = input.ReadAsBytes();
                System.IO.File.WriteAllBytes(dataDir, data);
                return parentDirectory;

            }
        }
        public string ReturnExtension(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".htm":
                case ".html":
                case ".log":
                    return "text/HTML";
                case ".txt":
                    return "text/plain";
                case ".doc":
                    return "application/ms-word";
                case ".tiff":
                case ".tif":
                    return "image/tiff";
                case ".asf":
                    return "video/x-ms-asf";
                case ".avi":
                    return "video/avi";
                case ".zip":
                    return "application/zip";
                case ".xls":
                case ".csv":
                    return "application/vnd.ms-excel";
                case ".gif":
                    return "image/gif";
                case ".jpg":
                case "jpeg":
                    return "image/jpeg";
                case ".bmp":
                    return "image/bmp";
                case ".wav":
                    return "audio/wav";
                case ".mp3":
                    return "audio/mpeg3";
                case ".mpg":
                case "mpeg":
                    return "video/mpeg";
                case ".rtf":
                    return "application/rtf";
                case ".asp":
                    return "text/asp";
                case ".pdf":
                    return "application/pdf";
                case ".fdf":
                    return "application/vnd.fdf";
                case ".ppt":
                    return "application/mspowerpoint";
                case ".dwg":
                    return "image/vnd.dwg";
                case ".msg":
                    return "application/msoutlook";
                case ".xml":
                case ".sdxl":
                    return "application/xml";
                case ".xdp":
                    return "application/vnd.adobe.xdp+xml";
                default:
                    return "application/octet-stream";
            }
        }
        public List<responseNotify> Notify(List<string> tracking)
        {
            var keyFlash = Get_Key("FLASH", "FLASH");
            
            List<OrderToNoti> Order_Noti = new List<OrderToNoti>();
            foreach (var no in tracking)
            {
                var orderData = (from orderItem in entities_Carrier.Order_Item
                                 join order in entities_Carrier.Orders on orderItem.Docno equals order.Docno
                                 where orderItem.pno == no
                                 select new OrderToNoti
                                 {
                                     mchId = orderItem.mchId,
                                     //nonceStr = order.Docno,
                                     pno = orderItem.pno,
                                     Docno = order.Docno,
                                     srcName = order.srcName,
                                     srcPhone = order.srcPhone,
                                     srcProvinceName = order.srcProvinceName,
                                     srcCityName = order.srcCityName,
                                     srcDistrictName = order.srcDistrictName,
                                     srcPostalCode = order.srcPostalCode,
                                     srcDetailAddress = order.srcDetailAddress,
                                     estimateParcelNumber = 1,
                                     remark = order.remark != "" ? order.remark : "-"
                                 }).ToList().FirstOrDefault();
                Order_Noti.Add(orderData);

            }

            var SDC1 = Order_Noti.Where(w => w.srcName == "บริษัท เอส.ดี.ซี วัน จำกัด").ToList();
            foreach(var sd in SDC1)
            {
                Order_Noti.Remove(sd);
            }
            List<responseNotify> listResnotify = new List<responseNotify>();
            responseNotify resnotity = new responseNotify();
            if (SDC1.Count != 0)
            {
                var last = SDC1.LastOrDefault();
                var random = "mdchId=" + last.mchId + "&sendTime=" + DateTime.Now;
                var Md5 = MD5_hash(random);
                var header =
                    "estimateParcelNumber=" + SDC1.Count() +
                    "&mchId=" + last.mchId +
                    "&nonceStr=" + Md5 +
                    "&srcCityName=" + "อำเภอวังน้อย" +
                    "&srcDetailAddress=" + "59/1 ม.1 " +
                    "&srcDistrictName=" + "ลำไทร" +
                    "&srcName=" + "บริษัท เอส.ดี.ซี วัน จำกัด" +
                    "&srcPhone=" + "0944764565" +
                    "&srcPostalCode=" + "13170" +
                    "&srcProvinceName=" + "พระนครศรีอยุธยา";

                string sign = sha256_hash(header + "&key=" + keyFlash.key).ToUpper();
                var client = new RestClient("https://api.flashexpress.com/open/v1/notify?" + header + "&sign=" + sign);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                IRestResponse response = client.Execute(request);
                JObject j = JObject.Parse(response.Content);
                var pno = SDC1.Select(s => s.pno).ToList();
                if (Convert.ToInt32(j["code"]) == 1)
                {
                    resnotity.pno = pno;
                    resnotity.code = Convert.ToInt32(j["code"]);
                    resnotity.message = j["message"].ToString();
                    resnotity.ticketPickupId = j["data"]["ticketPickupId"].ToString();
                    resnotity.staffInfoId = Convert.ToInt32(j["data"]["staffInfoId"]);
                    resnotity.staffInfoName = j["data"]["staffInfoName"].ToString();
                    resnotity.staffInfoPhone = j["data"]["staffInfoPhone"].ToString();
                    resnotity.upCountryNote = j["data"]["upCountryNote"].ToString();
                    resnotity.timeoutAtText = j["data"]["timeoutAtText"].ToString();
                    resnotity.ticketMessage = j["data"]["ticketMessage"].ToString();

                }
                else
                {
                    resnotity.pno = pno;
                    resnotity.code = Convert.ToInt32(j["code"]);
                    resnotity.message = j["message"].ToString();

                }
                listResnotify.Add(resnotity);
            }
            resnotity = new responseNotify();

            if (Order_Noti.Count != 0)
            {
                var last = Order_Noti.LastOrDefault();
                var random = "mdchId=" + last.mchId + "&sendTime=" + DateTime.Now;
                var Md5 = MD5_hash(random);
                var header =
                    "estimateParcelNumber=" + Order_Noti.Count() +
                    "&mchId=" + last.mchId +
                    "&nonceStr=" + Md5 +
                    "&srcCityName=" + "เขตบางคอแหลม" +
                    "&srcDetailAddress=" + "477 พระราม 3 " +
                    "&srcDistrictName=" + "บางโคล่" +
                    "&srcName=" + "บริษัท สตาร์แฟชั่น(2551) จำกัด" +
                    "&srcPhone=" + "0873078300" +
                    "&srcPostalCode=" + "10120" +
                    "&srcProvinceName=" + "กรุงเทพมหานคร";

                string sign = sha256_hash(header + "&key=" + keyFlash.key).ToUpper();
                var client = new RestClient("https://api.flashexpress.com/open/v1/notify?" + header + "&sign=" + sign);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                IRestResponse response = client.Execute(request);
                JObject j = JObject.Parse(response.Content);
                var pno = Order_Noti.Select(s => s.pno).ToList();
                if (Convert.ToInt32(j["code"]) == 1)
                {
                    resnotity.pno = pno;
                    resnotity.code = Convert.ToInt32(j["code"]);
                    resnotity.message = j["message"].ToString();
                    resnotity.ticketPickupId = j["data"]["ticketPickupId"].ToString();
                    resnotity.staffInfoId = Convert.ToInt32(j["data"]["staffInfoId"]);
                    resnotity.staffInfoName = j["data"]["staffInfoName"].ToString();
                    resnotity.staffInfoPhone = j["data"]["staffInfoPhone"].ToString();
                    resnotity.upCountryNote = j["data"]["upCountryNote"].ToString();
                    resnotity.timeoutAtText = j["data"]["timeoutAtText"].ToString();
                    resnotity.ticketMessage = j["data"]["ticketMessage"].ToString();

                }
                else
                {
                    resnotity.pno = pno;
                    resnotity.code = Convert.ToInt32(j["code"]);
                    resnotity.message = j["message"].ToString();

                }
                listResnotify.Add(resnotity);
            }
            return listResnotify;
        }
        public string Validate_Transport(Order item,string receive)
        {
            if (item.srcName == "")
            {
                return "กรุุณากรอชื่อผู้ส่ง";
            }
            else if (item.srcPhone == ""||item.srcPhone == "-")
            {
                return "กรุณากรอกเบอร์โทรผู้ส่ง";
            }
            else if (item.srcProvinceName == "เลือกจังหวัด")
            {
                return "กรุณาเลือกจังหวัดผู้ส่ง";
            }
            else if (item.dstName == "")
            {
                return "กรุณากรอกชื่อผู้รับ";
            }
            else if (item.dstPhone == "" || item.dstPhone == "-")
            {
                return "กรุณากรอกเบอร์โทรศัพท์มือถือผู้รับ";
            }
            else if (item.dstProvinceName == "เลือกจังหวัด")
            {
                return "กรุณาเลือกจังหวัดผู้รับ";
            }
            else if(item.dstCityName == "" || item.dstCityName == "เลือกอำเภอ")
            {
                return "กรุณาเลือกอำเภอผู้รับ";
            }
            else if (item.SDpart == "Select")
            {
                return "กรุณาเลือกแผนกที่ต้องการเบิก";
            }
            else if(item.articleCategory == 1111)
            {
                return "กรุณาเลือกประเภทพัสดุ";
            }
            if(receive == "หน้าร้าน")
            {
                if(item.siteStorage.Length < 8)
                {
                    return "กรุณาใส่ Site Storage ให้ครบทั้ง 8 ตัวด้วยครับ";
                }
            }

            return "PASS";
        }
        public User Check_UserID()
        {

            if ((HttpContext.Current.Request.Cookies["sfgweb"] != null))
            {
                if ((HttpContext.Current.Request.Cookies["sfgweb"]["uname"] != null))
                {
                    string username = HttpContext.Current.Request.Cookies["sfgweb"]["uname"].Trim();
                    var objuser = (from tEmployee in entities_InsideSFG_WF.Employees
                                   where tEmployee.username_ == username
                                   && tEmployee.StatWork == "Y"
                                   select tEmployee
                                      ).FirstOrDefault();
                    if (objuser != null)
                    {
                        HttpContext.Current.Session["_UserID"] = objuser.userID.ToString();
                        return new User { UserID = objuser.userID, Username = objuser.username_ + " " + objuser.surname };
                    }
                    else { return null; }
                }
                else { return null; }
            }
            else { HttpContext.Current.Session["_UserID"] = "101635";  return null; }
        }
        public string CancelOrder(string lbDocno,string lkbpno)
        {
            var query = (from order in entities_Carrier.Orders
                         join orderItem in entities_Carrier.Order_Item on order.Docno equals orderItem.Docno
                         where order.Docno == lbDocno && orderItem.pno == lkbpno
                         select new
                         {
                             pno = orderItem.pno,
                             mchId = orderItem.mchId,
                             nonceStr = lbDocno
                         }).ToList().FirstOrDefault();
            var head = "mchId=" + query.mchId + "&nonceStr=" + query.nonceStr ;
            var keyFlash = Get_Key("FLASH", "FLASH");
            var sign = sha256_hash(head+"&key=" + keyFlash.key).ToUpper();
            var client = new RestClient("https://api.flashexpress.com/open/v1/orders/"+query.pno+"/cancel?" + head + "&sign=" + sign);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
            JObject j = JObject.Parse(response.Content);
            if(Convert.ToInt32(j["code"]) == 1)
            {
                var order = entities_Carrier.Order_Item.Where(w=>w.Docno == lbDocno).ToList().FirstOrDefault();
                order.Status = "C";
                entities_Carrier.SaveChanges();
                return "Cancel Order Success.";
            }
            else
            {
                return "Cancel Order NOT Success.";
            }
        }
        
    }
    public class Model_Key
    {
        public string mchId { get; set; }
        public string key { get; set; }
    }
    public class Model_Trackingno
    {
        public bool success { get; set; }
        public string trackingno { get; set; }
    }
    
}