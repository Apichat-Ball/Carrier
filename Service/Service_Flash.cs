using Carrier.Info;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Online_Lazada;
using Carrier.Model.Whale;
using Carrier.Model.Budget;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
        BudgetEntities budget_Entities;

        public Service_Flash()
        {
            entities_Online_Lazada = new Online_LazadaEntities();
            entities_Whale = new WhaleEntities();
            entities_Carrier = new CarrierEntities();
            entities_InsideSFG_WF = new InsideSFG_WFEntities();
            budget_Entities = new BudgetEntities();
        }
        public Model_Trackingno CreateOrderFLASH(string docno, string favor)
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
                                        + (objOrder.dstProvinceName != null && objOrder.dstProvinceName != "" ? " " + objOrder.dstProvinceName : "");
                if (objOrder.dstPhone == "" || objOrder.dstPhone == "-" || objOrder.dstPhone == " ")
                {
                    headerpara += "&dstName=" + objOrder.dstName +
                                    "&dstPhone=" + objOrder.dstHomePhone;
                }
                else
                {
                    if (objOrder.dstHomePhone == "" || objOrder.dstHomePhone == "-" || objOrder.dstHomePhone == " ")
                    {
                        headerpara += "&dstName=" + objOrder.dstName + "&dstPhone=" + objOrder.dstPhone;
                    }
                    else
                    {
                        headerpara += "&dstHomePhone=" + objOrder.dstHomePhone + "&dstName=" + objOrder.dstName + "&dstPhone=" + objOrder.dstPhone;

                    }
                }
                headerpara += "&dstPostalCode=" + (objOrder.dstPostalCode != null ? objOrder.dstPostalCode.ToString() : "") +
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
                try
                {
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
                            upcountryCharge = j["data"]["upcountryCharge"].ToString(),
                            TypeSendKO = favor == "select" ? "SFG" : favor
                        };

                        entities_Carrier.Order_Item.Add(order);
                        entities_Carrier.SaveChanges();
                        entities_Carrier.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Create_Transport", request = "{https://api.flashexpress.com/open/v3/orders?" + headerpara + "&sign=" + sign + "}", status = j["code"].ToString(), fromFlash = Newtonsoft.Json.JsonConvert.SerializeObject(j), respon = Newtonsoft.Json.JsonConvert.SerializeObject(new Model_Trackingno { success = true, trackingno = j["data"]["pno"].ToString() }) });
                        entities_Carrier.SaveChanges();
                        return new Model_Trackingno { success = true, trackingno = j["data"]["pno"].ToString() };
                    }
                    else
                    {
                        var selectOrder = entities_Carrier.Orders.Where(w => w.Docno == docno).FirstOrDefault();
                        entities_Carrier.Orders.Remove(selectOrder);
                        entities_Carrier.SaveChanges();
                        entities_Carrier.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Create_Transport", request = "{https://api.flashexpress.com/open/v3/orders?" + headerpara + "&sign=" + sign + "}", status = j["code"].ToString(), fromFlash = Newtonsoft.Json.JsonConvert.SerializeObject(j), respon = Newtonsoft.Json.JsonConvert.SerializeObject(new Model_Trackingno { success = false, trackingno = j["message"].ToString() }) });
                        entities_Carrier.SaveChanges();
                        return new Model_Trackingno { success = false, trackingno = j["message"].ToString() };
                    }
                }
                catch (Exception ex)
                {
                    entities_Carrier.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Create_Transport", request = "{https://api.flashexpress.com/open/v3/orders?" + headerpara + "&sign=" + sign + "}", status = "500", fromFlash = response.Content, respon ="Fail" });
                    entities_Carrier.SaveChanges();
                    return new Model_Trackingno { success = false, trackingno = "Order นี้ไม่สามารถบันทึกได้ โปรดบันทึกหน้าจอการกรอกข้อมูลและแจ้งเจ้าหน้าที่ครับ" };
                }
                
            }
            else
            {
                return new Model_Trackingno { success = false, trackingno = "Order นี้ไม่ได้มีการบันทึกไว้" };
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

        #region convert string to passKey
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
        #endregion

        public string Get_Docment(string docno, string lastfile)
        {
            var orderItem = entities_Carrier.Order_Item.Where(w => w.Docno == docno).FirstOrDefault();
            string trackingno = orderItem.pno;

            Model_Key model_key = Get_Key("FLASH", "FLASH");

            string headerpara = "mchId=" + model_key.mchId + "&nonceStr=" + orderItem.Docno;
            string sign = sha256_hash(headerpara + "&key=" + model_key.key).ToUpper();
            try
            {
                HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create("https://api.flashexpress.com/open/v1/orders/" + trackingno + "/pre_print?" + headerpara + "&sign=" + sign);
                fileReq.Method = "POST";
                var resp = fileReq.GetResponse();
                Stream input = resp.GetResponseStream();
                string filepath = HttpContext.Current.Server.MapPath("PDFFile/");
                DirectoryInfo d = new DirectoryInfo(filepath);

                String originalPath = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
                String parentDirectory = originalPath.Substring(0, originalPath.LastIndexOf(lastfile)) + "/PDFFile/" + docno + ".pdf?" + DateTime.Now.ToString();
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
            catch(Exception ex)
            {
                SendMail("apichat.f@sfg-th.com", null, "Error GetDoccument",
                    "<html>" +
                    "<body>" +
                    "<p>message:"+ex.Message+"</p>" +
                    "</body>" +
                    "</html>");
                return "";
            }
            
        }
        public string GetDocumentAll(string DocBig, string pathRef)
        {
            var docno = entities_Carrier.Order_Big_Box.Where(w => w.BFID == DocBig).Select(w => w.Docno).ToList();
            List<string> path = new List<string>();
            foreach (var i in docno)
            {
                string dataDir = HttpContext.Current.Server.MapPath("PDFFile/") + i + ".pdf";
                path.Add(dataDir);
            }
            #region test
            //path.Add(HttpContext.Current.Server.MapPath("PDFFile/") + "FL2200000111.pdf");
            //path.Add(HttpContext.Current.Server.MapPath("PDFFile/") + "FL2200000112.pdf");
            //path.Add(HttpContext.Current.Server.MapPath("PDFFile/") + "FL2200000129.pdf");
            //path.Add(HttpContext.Current.Server.MapPath("PDFFile/") + "FL2200000135.pdf");
            //path.Add(HttpContext.Current.Server.MapPath("PDFFile/") + "FL2200000136.pdf");
            #endregion
            string filepath = HttpContext.Current.Server.MapPath("MergePDF/");
            DirectoryInfo d = new DirectoryInfo(filepath);
            var pathMerge = MergeFile(path,DocBig);
            String originalPath = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
            String parentDirectory = originalPath.Substring(0, originalPath.LastIndexOf(pathRef)) + "/MergePDF/" + DocBig + ".pdf?" + DateTime.Now.ToString();
            string dataDirMerge = HttpContext.Current.Server.MapPath("MergePDF/") + DocBig + ".pdf";
            
            return pathMerge.pathname;
        }
        public pathFile MergeFile(List<string> filepath, string docno)
        {
            string[] path = new string[filepath.Count()];
            var i = 0;
            foreach (var file in filepath)
            {
                path[i] = file;
                i++;
            }

            var res = CombineMultiplePDFs(path, docno);
            return res;
        }
        public pathFile CombineMultiplePDFs(string[] fileNames, string docno)
        {
            string dataDir = HttpContext.Current.Server.MapPath("MergePDF/");
            //if (File.Exists(dataDir + docno + ".pdf"))
            //{
            //    File.Delete(dataDir + docno + ".pdf");
            //}
            pathFile res = new pathFile();


            Document sourceDocument = new Document();
            PdfCopy pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(dataDir + docno + ".pdf", System.IO.FileMode.Create));
            sourceDocument.Open();
            foreach(var i in fileNames)
            {
                PdfReader reader = new PdfReader(i);
                int pages = TotalPageCount(i);
                for (int a = 1; a <= pages; a++)
                {
                    PdfImportedPage importedPage = pdfCopyProvider.GetImportedPage(reader, a);
                    pdfCopyProvider.AddPage(importedPage);
                }
                reader.Close();
            }
            sourceDocument.Close();
            res.pathname = dataDir + docno + ".pdf";
            return res;

        }
        private static int TotalPageCount(string file)
        {
            using (StreamReader sr = new StreamReader(System.IO.File.OpenRead(file)))
            {
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(sr.ReadToEnd());

                return matches.Count;
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
            List<Warehouse> warehouse = new List<Warehouse>();
            var keyFlash = Get_Key("FLASH", "FLASH");
            //var param = "mchId=" + keyFlash.mchId + "&nonceStr=" + tracking.FirstOrDefault();
            //string signWare = sha256_hash(param + "&key=" + keyFlash.key).ToUpper();
            //var clientWare = new RestClient("https://api.flashexpress.com/open/v1/warehouses?" + param + "&sign=" + signWare);
            //clientWare.Timeout = -1;
            //var requestWare = new RestRequest(Method.POST);
            //requestWare.AlwaysMultipartFormData = true;
            //IRestResponse responseWare = clientWare.Execute(requestWare);
            //JObject jW = JObject.Parse(responseWare.Content);
            //if (Convert.ToInt32(jW["code"]) == 1)
            //{
            //    foreach (var r in jW["data"])
            //    {
            //        warehouse.Add(new Warehouse
            //        {
            //            warehouseNo = r["warehouseNo"].ToString(),
            //            name = r["name"].ToString(),
            //            countryName = r["countryName"].ToString(),
            //            provinceName = r["provinceName"].ToString(),
            //            cityName = r["cityName"].ToString(),
            //            districtName = r["districtName"].ToString(),
            //            postalCode = r["postalCode"].ToString(),
            //            detailAddress = r["detailAddress"].ToString(),
            //            phone = r["phone"].ToString(),
            //            srcName = r["srcName"].ToString()
            //        });
            //    }
            //}

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
                                     remark = order.remark != "" ? order.remark : "-",
                                     TypeSendKo = orderItem.TypeSendKO
                                 }).ToList().FirstOrDefault();
                var check = CheckNotify(orderData.Docno);
                if (check == "")
                {
                    Order_Noti.Add(orderData);
                }
            }

            List<responseNotify> listResnotify = new List<responseNotify>();
            if (Order_Noti.Count != 0)
            {
                //SDC1
                var SDC1 = Order_Noti.Where(w => w.TypeSendKo == "SDC1").ToList();
                foreach (var sd in SDC1)
                {
                    Order_Noti.Remove(sd);
                }
                responseNotify resnotity = new responseNotify();
                if (SDC1.Count != 0)
                {
                    try
                    {
                        var last = SDC1.LastOrDefault();
                        var random = "mdchId=" + last.mchId + "&sendTime=" + DateTime.Now;
                        var Md5 = MD5_hash(random);
                        var header =
                            "estimateParcelNumber=" + SDC1.Count() +
                            "&mchId=" + last.mchId +
                            "&nonceStr=" + Md5 +
                            "&warehouseNo=" + "Wangnoi";

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
                            resnotity.dateSuccess = DateTime.Now;
                            resnotity.warehouseNo = "Wangnoi";

                            entities_Carrier.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Service_Flash/Notify", request = "https://api.flashexpress.com/open/v1/notify?" + header + "&sign=" + sign, status = resnotity.code.ToString(), fromFlash = Newtonsoft.Json.JsonConvert.SerializeObject(j), respon = Newtonsoft.Json.JsonConvert.SerializeObject(resnotity) });
                            entities_Carrier.SaveChanges();
                        }
                        else
                        {
                            resnotity.pno = pno;
                            resnotity.code = Convert.ToInt32(j["code"]);
                            resnotity.message = j["message"].ToString();
                            resnotity.warehouseNo = "Wangnoi";

                            entities_Carrier.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Service_Flash/Notify", request = "https://api.flashexpress.com/open/v1/notify?" + header + "&sign=" + sign, status = resnotity.code.ToString(), fromFlash = Newtonsoft.Json.JsonConvert.SerializeObject(j), respon = Newtonsoft.Json.JsonConvert.SerializeObject(resnotity) });
                            entities_Carrier.SaveChanges();

                        }
                        listResnotify.Add(resnotity);
                    }
                    catch(Exception ex)
                    {
                        SendMail("apichat.f@sfg-th.com", null, "Error",
                            "<Html><body>" +
                            "<p>SDC1</p>" +
                            "<p>Path : ServiceFlash/Notify</p>" +
                            "<p>"+ Newtonsoft.Json.JsonConvert.SerializeObject(SDC1) + "</p>" +
                            "<p>Error : "+ex.Message+"</p>" +
                            "</body></Html>");
                    }
                    
                }
                resnotity = new responseNotify();

                //ROX
                var rox = Order_Noti.Where(w => w.TypeSendKo == "ROX").ToList();
                if (rox.Count != 0)
                {
                    try
                    {
                        foreach (var r in rox)
                        {
                            Order_Noti.Remove(r);
                        }
                        var last = rox.LastOrDefault();
                        var random = "mdchId=" + last.mchId + "&sendTime=" + DateTime.Now;
                        var Md5 = MD5_hash(random);
                        var header =
                            "estimateParcelNumber=" + rox.Count() +
                            "&mchId=" + last.mchId +
                            "&nonceStr=" + Md5 +
                            "&warehouseNo=" + "RXSFRXM1";

                        string sign = sha256_hash(header + "&key=" + keyFlash.key).ToUpper();
                        var client = new RestClient("https://api.flashexpress.com/open/v1/notify?" + header + "&sign=" + sign);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AlwaysMultipartFormData = true;
                        IRestResponse response = client.Execute(request);
                        JObject j = JObject.Parse(response.Content);
                        var pno = rox.Select(s => s.pno).ToList();
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
                            resnotity.dateSuccess = DateTime.Now;
                            resnotity.warehouseNo = "RXSFRXM1";
                            entities_Carrier.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Service_Flash/Notify", request = "https://api.flashexpress.com/open/v1/notify?" + header + "&sign=" + sign, status = resnotity.code.ToString(), fromFlash = Newtonsoft.Json.JsonConvert.SerializeObject(j), respon = Newtonsoft.Json.JsonConvert.SerializeObject(resnotity) });
                            entities_Carrier.SaveChanges();
                        }
                        else
                        {
                            resnotity.pno = pno;
                            resnotity.code = Convert.ToInt32(j["code"]);
                            resnotity.message = j["message"].ToString();
                            resnotity.warehouseNo = "RXSFRXM1";
                            entities_Carrier.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Service_Flash/Notify", request = "https://api.flashexpress.com/open/v1/notify?" + header + "&sign=" + sign, status = resnotity.code.ToString(), fromFlash = Newtonsoft.Json.JsonConvert.SerializeObject(j), respon = Newtonsoft.Json.JsonConvert.SerializeObject(resnotity) });
                            entities_Carrier.SaveChanges();

                        }
                        listResnotify.Add(resnotity);
                    }
                    catch(Exception ex)
                    {
                        SendMail("apichat.f@sfg-th.com", null, "Error",
                            "<Html><body>" +
                            "<p>ROX</p>" +
                            "<p>Path : ServiceFlash/Notify</p>" +
                            "<p>" + Newtonsoft.Json.JsonConvert.SerializeObject(rox) + "</p>" +
                            "<p>Error : " + ex.Message + "</p>" +
                            "</body></Html>");
                    }
                    
                }

                resnotity = new responseNotify();

                //SFG
                if (Order_Noti.Count != 0)
                {
                    try
                    {
                        var last = Order_Noti.LastOrDefault();
                        var random = "mdchId=" + last.mchId + "&sendTime=" + DateTime.Now;
                        var Md5 = MD5_hash(random);
                        var header =
                            "estimateParcelNumber=" + Order_Noti.Count() +
                            "&mchId=" + last.mchId +
                            "&nonceStr=" + Md5 +
                            "&warehouseNo=" + "SFG";

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
                            resnotity.dateSuccess = DateTime.Now;
                            resnotity.warehouseNo = "SFG";
                            entities_Carrier.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Service_Flash/Notify", request = "https://api.flashexpress.com/open/v1/notify?" + header + "&sign=" + sign, status = resnotity.code.ToString(), fromFlash = Newtonsoft.Json.JsonConvert.SerializeObject(j), respon = Newtonsoft.Json.JsonConvert.SerializeObject(resnotity) });
                            entities_Carrier.SaveChanges();
                        }
                        else
                        {
                            resnotity.pno = pno;
                            resnotity.code = Convert.ToInt32(j["code"]);
                            resnotity.message = j["message"].ToString();
                            resnotity.warehouseNo = "SFG";
                            entities_Carrier.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Service_Flash/Notify", request = "https://api.flashexpress.com/open/v1/notify?" + header + "&sign=" + sign, status = resnotity.code.ToString(), fromFlash = Newtonsoft.Json.JsonConvert.SerializeObject(j), respon = Newtonsoft.Json.JsonConvert.SerializeObject(resnotity) });
                            entities_Carrier.SaveChanges();

                        }
                        listResnotify.Add(resnotity);
                    }
                    catch(Exception ex)
                    {
                        SendMail("apichat.f@sfg-th.com", null, "Error",
                            "<Html><body>" +
                            "<p>SFG</p>" +
                            "<p>Path : ServiceFlash/Notify</p>" +
                            "<p>" + Newtonsoft.Json.JsonConvert.SerializeObject(Order_Noti) + "</p>" +
                            "<p>Error : " + ex.Message + "</p>" +
                            "</body></Html>");
                    }
                    

                }

            }
                return listResnotify;
        }
        public string CheckNotify(string Docno)
        {
            #region v1
            //var mchId = entities_Carrier.Order_Item.Where(w => w.Docno == Docno).Select(s => s.mchId).ToList().FirstOrDefault();
            //var keyFlash = Get_Key("FLASH", "FLASH");
            //var date = datesend.ToString("yyyy-MM-dd");
            //var random = "date=" + date + "mchId=" + mchId;
            //var Md5 = MD5_hash(random);
            //var header = "date=" + date + "&mchId=" + mchId + "&nonceStr=" + Md5;
            //string sign = sha256_hash(header + "&key=" + keyFlash.key).ToUpper();
            //var client = new RestClient("https://api.flashexpress.com/open/v1/notifications?" + header + "&sign=" + sign);
            //client.Timeout = -1;
            //var request = new RestRequest(Method.POST);
            //request.AlwaysMultipartFormData = true;
            //IRestResponse response = client.Execute(request);
            //JObject j = JObject.Parse(response.Content);
            //var d = "";
            //var booking = (from OI in entities_Carrier.Order_Item
            //                 join NT in entities_Carrier.Notifies on OI.ticketPickupId equals NT.TicketPickupId
            //                 where OI.Docno == Docno
            //                 select new { warehouseNo = NT.warehouseNo  , dateNotify = NT.DateNotify}).FirstOrDefault() ;
            //if (Convert.ToInt32(j["code"]) == 1)
            //{
            //    foreach (var i in j["data"])
            //    {

            //        var dtf = (booking.dateNotify ?? DateTime.Now).AddHours(-7);
            //        var dtfNew = dtf.AddMinutes(+15);
            //        dtf = dtf.AddMinutes(-15);
            //        //var dateUnix = new DateTimeOffset(dtf).ToUnixTimeSeconds();
            //        var createAt = Convert.ToInt32(i["createdAt"].ToString());
            //        var createAtFromEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(createAt);
            //        if (dtf <= createAtFromEpoch && createAtFromEpoch <= dtfNew && (booking.warehouseNo == i["kaWarehouseNo"].ToString() || booking.warehouseNo == " "))
            //        {
            //            d = i["stateText"].ToString();
            //        }

            //    }
            //    if(d == "")
            //    {

            //    }
            //}
            #endregion
            #region V2
            var d = "";

            var mchId = entities_Carrier.Order_Item.Where(w => w.Docno == Docno).ToList().FirstOrDefault();
            var keyFlash = Get_Key("FLASH", "FLASH");
            var random = "pno=" + mchId.pno + "mchId=" + mchId.mchId;
            var Md5 = MD5_hash(random);
            var header = "mchId=" + mchId.mchId + "&nonceStr=" + Md5;
            string sign = sha256_hash(header + "&key=" + keyFlash.key).ToUpper();
            try
            {
                var client = new RestClient("https://api.flashexpress.com/open/v1/orders/" + mchId.pno + "/routes?" + header + "&sign=" + sign);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                IRestResponse response = client.Execute(request);
                JObject j = JObject.Parse(response.Content);
                if (j["code"].ToString() == "1")
                {
                    var code = Convert.ToInt32(j["data"]["state"].ToString());
                    var a = entities_Carrier.Status_Notify_Order.Where(w => w.statusId == code).ToList();
                    if (a.Count() == 0)
                    {
                        entities_Carrier.Status_Notify_Order.Add(new Status_Notify_Order { statusId = code, statusName = j["data"]["stateText"].ToString() });
                        entities_Carrier.SaveChanges();
                    }
                    if (j["data"]["state"].ToString() != "0")
                    {
                        d = j["data"]["stateText"].ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                SendMail("apichat.f@sfg-th.com", null, "Error Service CheckNotify " + Docno, "<html>" +
                    "<body>" +
                    "<p>Path : " + "https://api.flashexpress.com/open/v1/orders/" + mchId.pno + "/routes?" + header + "&sign=" + sign + "</p>" +
                    "<p>Error:" + ex.Message + "</p>" +
                    "</body" +
                    "</html>");
            }
            
            #endregion
            return d;
        }

        //V2
        public Model_Chack_Noti CheckNotifyBigBox(string Docno)
        {
            Model_Chack_Noti d = new Model_Chack_Noti();
            var mchId = entities_Carrier.Order_Item.Where(w => w.Docno == Docno).ToList().FirstOrDefault();
            var keyFlash = Get_Key("FLASH", "FLASH");
            var random = "pno=" + mchId.pno + "mchId=" + mchId.mchId;
            var Md5 = MD5_hash(random);
            var header = "mchId=" + mchId.mchId + "&nonceStr=" + Md5;
            string sign = sha256_hash(header + "&key=" + keyFlash.key).ToUpper();
            try
            {
                var client = new RestClient("https://api.flashexpress.com/open/v1/orders/" + mchId.pno + "/routes?" + header + "&sign=" + sign);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                IRestResponse response = client.Execute(request);
                JObject j = JObject.Parse(response.Content);
                if (j["code"].ToString() == "1")
                {
                    var code = Convert.ToInt32(j["data"]["state"].ToString());
                    var a = entities_Carrier.Status_Notify_Order.Where(w => w.statusId == code).ToList();
                    if (a.Count() == 0)
                    {
                        entities_Carrier.Status_Notify_Order.Add(new Status_Notify_Order { statusId = code, statusName = j["data"]["stateText"].ToString() });
                        entities_Carrier.SaveChanges();
                    }
                    if (j["data"]["state"].ToString() != "0")
                    {
                        d.code = j["data"]["state"].ToString();
                        d.message = j["data"]["stateText"].ToString();

                    }
                    else
                    {
                        d.code = j["data"]["state"].ToString();
                        d.message = "";
                    }
                    var bigbox = entities_Carrier.Order_Big_Box.Where(w => w.Docno == Docno).FirstOrDefault();
                    bigbox.StatusNotifyCode = j["data"]["state"].ToString();
                    bigbox.StatusNotifyText = j["data"]["stateText"].ToString();
                    entities_Carrier.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                SendMail("apichat.f@sfg-th.com", null, "Error Service CheckNotiBigBox "+ Docno, "<html>" +
                    "<body>" +
                    "<p>Path : "+ "https://api.flashexpress.com/open/v1/orders/" + mchId.pno + "/routes?" + header + "&sign=" + sign +"</p>" +
                    "<p>Error:"+ex.Message+"</p>" +
                    "</body" +
                    "</html>");
            }
            
            return d;
        }
        public string Validate_Transport(Order item, string receive, string favorites)
        {
            if (item.srcName == "")
            {
                return "กรุุณากรอชื่อผู้ส่ง";
            }
            else if (item.srcPhone == "" || item.srcPhone == "-")
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
                if (item.dstHomePhone == "" || item.dstHomePhone == "-")
                {
                    return "กรุณากรอกเบอร์โทรศัพท์ผู้รับอย่างใดอย่างหนึ่งหรือทั้ง 2 เบอร์";
                }
            }
            else if (item.dstProvinceName == "เลือกจังหวัด")
            {
                return "กรุณาเลือกจังหวัดผู้รับ";
            }
            else if (item.dstCityName == "" || item.dstCityName == "เลือกอำเภอ")
            {
                return "กรุณาเลือกอำเภอผู้รับ";
            }
            else if (item.dstDistrictName == "" || item.dstDistrictName == "เลือกตำบล")
            {
                return "กรุณาเลือกตำบลผู้รับ";
            }
            else if (item.dstDetailAddress.Length > 200)
            {
                return "รายละเอียดที่อยู่ผู้รับต้องไม่เกิน 200 ตัวอักษร";
            }
            else if (item.dstDetailAddress.Contains("&"))
            {
                return "รายละเอียดที่อยู่ผู้รับต้องไม่มีตัวอักษร &";
            }
            else if (item.SDpart == "Select")
            {
                return "กรุณาเลือกแผนกที่ต้องการเบิก";
            }
            else if (item.articleCategory == 1111)
            {
                return "กรุณาเลือกประเภทพัสดุ";
            }
            if (receive == "select")
            {
                return "กรุณาเลือกปลายทาง";
            }
            if (item.Transport_Type == 1)
            {
                if (favorites == "select")
                {
                    return "กรุณาเลือกผุ้ส่ง";
                }
            }
            if (item.srcName.Length > 50)
            {
                return "ชื่อผู้ส่งต้องมีความยาวไม่เกิน 50 ตัวอักษร";
            }
            else if (item.srcName.Contains("&"))
            {
                return "ชื่อผู้ส่งไม่สามารถใส่อักษรพิเศษได้แก่ &";
            }
            if (item.dstName.Length > 50)
            {
                return "ชื่อผู้รับต้องมีความยาวไม่เกิน 50 ตัวอักษร";
            }
            else if (item.dstName.Contains("&"))
            {
                return "ชื่อผู้รับไม่สามารถใส่อักษรพิเศษได้แก่ &";
            }
            if (item.remark.Contains("+"))
            {
                return "ช่องหมายเหตุห้ามใส่เครื่องหมาย +";
            }

            if (item.dstDetailAddress.Contains("#") || item.dstDetailAddress.Contains("*") || item.dstDetailAddress.Contains("+"))
            {
                return "ที่อยู่รายละเอียดผู้รับห้ามใช้ตัวอักษรดังนี้ # * +";
            }

            if (item.siteStorage.Length < 6)
            {
                return "กรุณาใส่ SiteStorage ไม่ต่ำกว่า 6 ตัวครับ";
            }
            else if (item.siteStorage.Contains(" ") || item.siteStorage.Contains("-"))
            {
                return "SiteStorage ห้ามเว้นวรรคหรือเครื่องหมายต่างๆครับ";
            }
            else
            {
                //if (item.SDpart == "1619")
                //{
                //    var seek = budget_Entities.Departments.Where(w => w.Department_Name.Contains("SEEK") && w.Flag == "F" && w.Department_ID != "1619" && w.ShortBrand != "" ).Select(s=>s.ShortBrand).ToList();
                //    var siteCheck = entities_Carrier.Site_Profit.Where(w => w.Site_Stroage.StartsWith(item.siteStorage) && w.Channel == item.saleOn && seek.Contains(w.Brand)).ToList();
                //    if(siteCheck.Count() == 0)
                //    {
                //        return "ไม่พบ SiteStorage นี้ครับ";
                //    }
                //}
                //else
                //{
                    if (item.saleChannel == "Event")
                    {
                        var eventPro = entities_Carrier.Event_Shop.Where(w => w.Shop_Code.StartsWith(item.siteStorage)).ToList();
                        if (eventPro.Count == 0)
                        {
                            var eventProfit = entities_Carrier.Site_Profit.Where(w => w.Sale_Channel == item.saleChannel && w.Site_Stroage.StartsWith(item.siteStorage)).ToList();
                            if(eventProfit.Count() == 0)
                            {
                                return "ไม่พบ SiteStorage นี้ใน Event ครับ";
                            }
                            
                        }
                    }
                    else if (item.saleChannel == "Depart")
                    {
                        if (item.siteStorage.StartsWith("CENTER"))
                        {
                            return "หน้าร้าน จำเป็นต้องใส่ SiteStorage ที่ไม่ใช่ CENTER";
                        }
                        else
                        {
                            var BG = budget_Entities.Departments.Where(w => w.Department_ID == item.SDpart).Select(s => s.ShortBrand).ToList();

                            var SaleChannel = entities_Carrier.Site_Profit.Where(w => w.Site_Stroage.StartsWith(item.siteStorage) && w.Channel == item.saleOn).Select(s => s.Sale_Channel).FirstOrDefault();

                            var siteCenter = entities_Carrier.Site_Center.Where(w => BG.Contains(w.Brand_Center_Short)).FirstOrDefault();
                            if (siteCenter == null)
                            {
                                if (item.siteStorage.StartsWith("CENTER"))
                                {
                                    return "SiteStorage CENTER ใช้ไม่ได้";
                                }
                            }
                            else
                            {
                                if (item.siteStorage != "CENTER")
                                {
                                    return "SiteStorage ใช้ได้แค่ CENTER ";
                                }
                            }
                            if (SaleChannel != "Shop")
                            {
                                var brandProfit = entities_Carrier.Site_Profit.Where(w => w.Site_Stroage.StartsWith(item.siteStorage) && w.Channel == item.saleOn).Select(s => s.Brand).Distinct().ToList();
                                if (brandProfit.Count == 0)
                                {
                                    return "ไม่พบ SiteStorage นี้ครับ";
                                }
                                BG = BG.Where(w => brandProfit.Contains(w)).ToList();
                                if (BG.Count() == 0)
                                {
                                    return "SiteStorage พบ Profit แต่ Brand ไม่ตรงกับใน Profit ";
                                }
                            }
                        }

                    }
                    else
                    {
                        var BG = budget_Entities.Departments.Where(w => w.Department_ID == item.SDpart).Select(s => s.ShortBrand).ToList();

                        var siteCenter = entities_Carrier.Site_Center.Where(w => BG.Contains(w.Brand_Center_Short)).FirstOrDefault();
                        if (siteCenter == null)
                        {
                            if (item.siteStorage.StartsWith("CENTER"))
                            {
                                return "SiteStorage CENTER ใช้ไม่ได้";
                            }
                            else
                            {
                                var pro = entities_Carrier.Site_Profit.Where(w => w.Channel == item.saleOn && BG.Contains(w.Brand)).ToList();
                                if (pro.Count == 0)
                                {
                                    return "Brand นี้ไม่พบ Profit ";
                                }
                                else
                                {
                                    var SaleChannel = entities_Carrier.Site_Profit.Where(w => w.Site_Stroage.StartsWith(item.siteStorage) && w.Channel == item.saleOn).Select(s => s.Sale_Channel).FirstOrDefault();
                                    if (SaleChannel != "Shop")
                                    {
                                        var brandProfit = entities_Carrier.Site_Profit.Where(w => w.Site_Stroage.StartsWith(item.siteStorage) && w.Channel == item.saleOn).Select(s => s.Brand).Distinct().ToList();
                                        if (brandProfit.Count == 0)
                                        {
                                            return "ไม่พบ SiteStorage นี้ครับ";
                                        }
                                        BG = BG.Where(w => brandProfit.Contains(w)).ToList();
                                        if (BG.Count() == 0)
                                        {
                                            return "SiteStorage พบ Profit แต่ Brand ไม่ตรงกับใน Profit ";
                                        }
                                    }
                                    else
                                    {
                                        var brandProfit = entities_Carrier.Site_Profit.Where(w => w.Site_Stroage.StartsWith(item.siteStorage) && w.Channel == item.saleOn).Select(s => s.Brand).Distinct().ToList();
                                        BG = BG.Where(w => brandProfit.Contains(w)).ToList();
                                        if (BG.Count() == 0)
                                        {
                                            return "SiteStorage พบ Profit แต่ Brand ไม่ตรงกับใน Profit ";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (item.siteStorage != "CENTER")
                            {
                                return "SiteStorage ใช้ได้แค่ CENTER ";
                            }
                            else
                            {
                                var pro = entities_Carrier.Site_Profit.Where(w => w.Channel == item.saleOn && w.Brand == siteCenter.Brand_Center_Name_Full).ToList();
                                if (pro.Count == 0)
                                {
                                    return "Brand นี้ไม่พบ Profit ";
                                }
                            }

                        }

                    }
                //}
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
                                   where (tEmployee.username_ == username || tEmployee.uCode == username)
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
            else { return null; }
        }
        public string CancelOrder(string lbDocno, string lkbpno)
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
            var head = "mchId=" + query.mchId + "&nonceStr=" + query.nonceStr;
            var keyFlash = Get_Key("FLASH", "FLASH");
            var sign = sha256_hash(head + "&key=" + keyFlash.key).ToUpper();
            var client = new RestClient("https://api.flashexpress.com/open/v1/orders/" + query.pno + "/cancel?" + head + "&sign=" + sign);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
            JObject j = JObject.Parse(response.Content);
            if (Convert.ToInt32(j["code"]) == 1)
            {
                var order = entities_Carrier.Order_Item.Where(w => w.Docno == lbDocno).ToList().FirstOrDefault();
                order.Status = "C";
                entities_Carrier.SaveChanges();
                return "Cancel Order Success.";
            }
            else
            {
                var order = entities_Carrier.Order_Item.Where(w => w.Docno == lbDocno).ToList().FirstOrDefault();
                order.Status = "C";
                entities_Carrier.SaveChanges();
                return "Cancel Order Success.";
            }
        }
        public string EditOrder(Order item, string pno, string typeSentKo)
        {
            Model_Key model_key = Get_Key("FLASH", "FLASH");
            string headerpara = "articleCategory=" + item.articleCategory +
                                "&codEnabled=0" +
                                "&dstCityName=" + item.dstCityName +
                                "&dstDetailAddress=" + (item.dstDetailAddress == null ? "" : item.dstDetailAddress)
                                    + (item.dstDistrictName != null && item.dstDistrictName != "" ? " " + item.dstDistrictName : "")
                                    + (item.dstCityName != null && item.dstCityName != "" ? " " + item.dstCityName : "")
                                    + (item.dstProvinceName != null && item.dstProvinceName != "" ? " " + item.dstProvinceName : "");
            if (item.dstPhone == "" || item.dstPhone == "-" || item.dstPhone == " ")
            {
                headerpara += "&dstName=" + item.dstName +
                                "&dstPhone=" + item.dstHomePhone;
            }
            else
            {
                if (item.dstHomePhone == "" || item.dstHomePhone == "-" || item.dstHomePhone == " ")
                {
                    headerpara += "&dstName=" + item.dstName + "&dstPhone=" + item.dstPhone;
                }
                else
                {
                    headerpara += "&dstHomePhone" + item.dstHomePhone + "&dstName=" + item.dstName + "&dstPhone=" + item.dstPhone;

                }
            }
            headerpara += "&dstPostalCode=" + (item.dstPostalCode != null ? item.dstPostalCode.ToString() : "") +
                                "&dstProvinceName=" + item.dstProvinceName +
                                "&expressCategory=" + item.ExpressCategory +
                                "&insured=0" +
                                "&mchId=" + model_key.mchId +
                                "&nonceStr=" + item.Docno +
                                "&outTradeNo=" + item.Docno +
                                "&pno=" + pno +
                                "&remark=" + (item.remark != "" ? item.remark : "-") +
                                "&srcDetailAddress=" + (item.srcDetailAddress != null && item.srcDetailAddress != "" ? item.srcDetailAddress : "")
                                + (item.srcDistrictName != null && item.srcDistrictName != "" ? " " + item.srcDistrictName : "")
                                + (item.srcCityName != null && item.srcCityName != "" ? " " + item.srcCityName : "")
                                + (item.srcProvinceName != null && item.srcProvinceName != "" ? " " + item.srcProvinceName : "") +
                                "&srcName=" + item.srcName +
                                "&srcPhone=" + item.srcPhone +
                                "&weight=1";
            string sign = sha256_hash(headerpara + "&key=" + model_key.key).ToUpper();
            var client = new RestClient("https://api.flashexpress.com/open/v1/orders/modify?" + headerpara + "&sign=" + sign);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
            JObject j = JObject.Parse(response.Content);
            if (Convert.ToInt32(j["code"]) == 1)
            {
                var order = entities_Carrier.Orders.Where(w => w.Docno == item.Docno).FirstOrDefault();
                order.dstName = item.dstName;
                order.dstPhone = item.dstPhone;
                order.dstHomePhone = item.dstHomePhone;
                order.dstProvinceName = item.dstProvinceName;
                order.dstCityName = item.dstCityName;
                order.dstDistrictName = item.dstDistrictName;
                order.dstDetailAddress = item.dstDetailAddress;
                order.dstPostalCode = item.dstPostalCode;
                order.srcName = item.srcName;
                order.srcPhone = item.srcPhone;
                order.srcProvinceName = item.srcProvinceName;
                order.srcCityName = item.srcCityName;
                order.srcDistrictName = item.srcDistrictName;
                order.srcDetailAddress = item.srcDistrictName;
                order.srcPostalCode = item.srcPostalCode;
                order.remark = item.remark;
                order.saleOn = item.saleOn;
                order.saleChannel = item.saleChannel;
                order.SDpart = item.SDpart;
                order.Transport_Type = item.Transport_Type;
                order.TypeSend = item.TypeSend;
                order.siteStorage = item.siteStorage;
                order.ExpressCategory = item.ExpressCategory;
                var orderItem = entities_Carrier.Order_Item.Where(w => w.Docno == item.Docno).FirstOrDefault();
                orderItem.TypeSendKO = typeSentKo;
                entities_Carrier.SaveChanges();
                return "สำเร็จ";
            }
            else
            {
                return "ไม่สำเร็จ";
            }

        }
        public List<ReportBrand> Get_Report_Brand(string departOrShop, string SDpart)
        {

            var order = (from or in entities_Carrier.Orders
                         join orItem in entities_Carrier.Order_Item on or.Docno equals orItem.Docno
                         where orItem.Status == "A" || orItem.Status == "SL"
                         select new ReportBrand
                         {
                             Docno = or.Docno,
                             Date_send = or.Date_send,
                             saleOn = or.saleOn,
                             SDpart = or.SDpart,
                             siteStorage = or.siteStorage,
                             status = or.status,
                             TypeSend = or.TypeSend,
                             saleChannel = entities_Carrier.Site_Profit.Where(w => w.Site_Stroage.StartsWith(or.siteStorage)).FirstOrDefault().Sale_Channel,
                             Qty = orItem.Qty ?? 0
                         }).ToList();
            foreach (var i in order)
            {
                var BG = (from ha in entities_InsideSFG_WF.BG_HApprove
                          join haP in entities_InsideSFG_WF.BG_HApprove_Profitcenter on ha.departmentID equals haP.DepartmentID
                          where ha.departmentID == i.SDpart
                          select new { ha = ha, haP = haP }).ToList();
                i.SDpart_Name = BG.FirstOrDefault().haP.Depart_Short;
                i.SDpart_Name_Full = BG.FirstOrDefault().ha.department_;
            }
            var ss = order.Where(w => w.saleChannel == departOrShop && w.saleOn == "OFFLINE" && w.SDpart == SDpart).ToList();
            return ss;

        }


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
                smtp.Credentials = new NetworkCredential("mis.starfashiongroup@gmail.com", "cljsefqhkviuckfl");
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
    public class Model_Chack_Noti
    {
        public string code { get; set; }
        public string message { get; set; }
    }

}