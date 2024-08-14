using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Online_Lazada;
using Carrier.Model.Whale;
using Carrier.Model.Online_NonAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web.UI.WebControls;

namespace Carrier.Service
{
    public class Service_Whale
    {
        //Entities
        WhaleEntities entities_Whale;
        Online_LazadaEntities entities_Online_Lazada;
        Online_NonAPIEntities entities_Online_NonAPI;
        InsideSFG_WFEntities entities_InsideSFG_WF;

        //Service_NetworkShare service_NetworkShare;
        public Service_Whale()
        {
            //Entities
            entities_Whale = new WhaleEntities();
            entities_Online_Lazada = new Online_LazadaEntities();
            entities_Online_NonAPI = new Online_NonAPIEntities();
            entities_InsideSFG_WF = new InsideSFG_WFEntities();

        }

        //Order
        public class Model_Order
        {
            public DateTime? Date_Send { get; set; }
            public string Docno { get; set; }
            public int? UserID_Create { get; set; }
            public string Status { get; set; }
            public string Status_Name { get; set; }
            public string Ref_Order { get; set; }
            public DateTime? Transaction_Date { get; set; }
            public int Transaction_Type { get; set; }
            public string Type_Transaction { get; set; }
            public string Channel_ID { get; set; }
            public string Channel_Name { get; set; }
            public string Channel_Group { get; set; }
            public string Channel_refCode { get; set; }
            public string c_Name { get; set; }
            public string s_Name { get; set; }
            public string s_Address1 { get; set; }
            public string s_Address2 { get; set; }
            public string s_District_ID { get; set; }
            public string s_City_ID { get; set; }
            public string s_Province_ID { get; set; }
            public string s_PostCode { get; set; }
            public string s_Phone1 { get; set; }
            public string s_Phone2 { get; set; }
            public string b_IDCard { get; set; }
            public string b_Email { get; set; }
            public string b_Name { get; set; }
            public int? StorageWM_ID { get; set; }
            public DateTime? Date_Print { get; set; }
            public int? Owner_ID { get; set; }
            public string TypeDocument_ID { get; set; }
            public string TypeDocument_Name { get; set; }
            public string Code_Commission { get; set; }
            public string No { get; set; }
            public string SKU { get; set; }
            public string Barcode { get; set; }
            public string Product_Name { get; set; }
            public string Location_Name { get; set; }
            public int? QTY { get; set; }
            public double? RetailPrice { get; set; }
            public double? NetPrice { get; set; }
            public string Status_Item { get; set; }
            public string Status_Item_Name { get; set; }
            public string Trackingno { get; set; }
            public bool? Boxed { get; set; }
            public DateTime? Date_Boxed { get; set; }
            public string Vouchercode { get; set; }
            public double? Shippingfee { get; set; }
            public double? Paymentfee { get; set; }
            public double? Commissionfee { get; set; }
            public string Remark { get; set; }
            public DateTime? Date_Tracking { get; set; }
            public string Ref_Packaging { get; set; }
            public DateTime? Shipbydate { get; set; }
        }
        public class Filter_Order
        {
            public string docno { get; set; }
            

        }
        public IQueryable<Model_Order> Get_Order(string docno)
        {

            var objData = (from tSalesOrders in entities_Whale.SalesOrders
                           where (docno == null || docno == ""
                                   || tSalesOrders.Docno == docno || tSalesOrders.Ref_Order == docno
                                   || (tSalesOrders.Ref_Packaging == docno && !new string[] { "AB", "F" }.Contains(tSalesOrders.Status)))
                           join tChannel in entities_Whale.Channels on tSalesOrders.Channel_ID equals tChannel.Channel_ID
                           join tItems in entities_Whale.SalesOrder_Item on tSalesOrders.Docno equals tItems.Docno
                           select new Model_Order
                           {
                               Date_Send = tSalesOrders.Date_Send,
                               Docno = tSalesOrders.Docno,
                               //UserID_Create = tSalesOrders.UserID,
                               Status = tSalesOrders.Status,
                               //Status_Name =
                               //             tSalesOrders.Transaction_Type == 1 ? tStatus.Status_Name :
                               //             tSalesOrders.Transaction_Type == 2 ? (tSalesOrders.Status == "F" ? "คืนสินค้า" : tStatus.Status_Name) : "ERROR",
                               //Ref_Order = tSalesOrders.Ref_Order,
                               //Transaction_Date = tSalesOrders.Transaction_Date,
                               //Transaction_Type = (int)tSalesOrders.Transaction_Type,
                               Type_Transaction = tChannel.Type_Transaction,
                               Channel_ID = tSalesOrders.Channel_ID.ToString(),
                               //Channel_Name = tChannel.Channel_Name,
                               //Channel_Group = tChannel.Channel_Group,
                               Channel_refCode = tChannel.refCode,
                               //c_Name = tSalesOrders.c_Name,
                               //s_Name = tSalesOrders.s_Name,
                               //s_Address1 = tSalesOrders.s_Address1,
                               //s_Address2 = tSalesOrders.s_Address2,
                               //s_District_ID = tSalesOrders.s_District_ID.ToString(),
                               //s_City_ID = tSalesOrders.s_Amphur_ID.ToString(),
                               //s_Province_ID = tSalesOrders.s_Province_ID.ToString(),
                               //s_PostCode = tSalesOrders.s_PostCode,
                               //s_Phone1 = tSalesOrders.s_Phone1,
                               //s_Phone2 = tSalesOrders.s_Phone2,
                               //b_Name = tSalesOrders.b_Name,
                               //b_Email = tSalesOrders.b_Email,
                               b_IDCard = tSalesOrders.b_IDCard,
                               //StorageWM_ID = tSalesOrders.StorageWM_ID,
                               //Date_Print = tSalesOrders.Date_Print,
                               //Owner_ID = (int)tSalesOrders.Owner_ID,
                               //TypeDocument_ID = tSalesOrders.TypeDocument_ID,
                               //TypeDocument_Name = tTypeDocument.TypeDocument_Name,
                               //Code_Commission = tSalesOrders.Code_Commission
                               SKU = tItems.SKU,
                           });
                           
            return objData;
        }

        public IQueryable<Model_Order> Get_Order2( string docno)
        {
            var objData = (from tNonAPI in entities_Online_NonAPI.Form_NonAPI
                           join tChannel in entities_Online_NonAPI.Type_Channel on tNonAPI.Channel_ID equals tChannel.Channel_ID
                           join tNonAPI_Item in entities_Online_NonAPI.Form_NonAPI_Item on tNonAPI.Docno equals tNonAPI_Item.Docno
                           where tNonAPI.Docno == docno
                           select new Model_Order
                           {
                               Date_Send = tNonAPI.Date_Send,
                               Docno = tNonAPI.Docno,
                               UserID_Create = tNonAPI.UserID,
                               Status = tNonAPI.DocStatus,
                               Status_Name =
                                    tNonAPI.Transaction_Type == "0001" ?
                                    (
                                        tNonAPI.DocStatus == "S" ? "สินค้าไม่เพียงพอ" :
                                        tNonAPI.DocStatus == "C" || tNonAPI.DocStatus == "R" ? "ยกเลิก" :
                                        tNonAPI.DocStatus == "F" ? "ขนส่งเข้ารับสินค้าแล้ว" :
                                        tNonAPI.DocStatus == "CC" ? "ยกเลิกจากระบบ WMK เพื่อไป Whale" :
                                        "ERROR"
                                    ) :
                                    tNonAPI.Transaction_Type == "0002" ?
                                    (
                                        tNonAPI.DocStatus == "F" ? "คืนสินค้า" :
                                        "ERROR"
                                    ) :
                                    "ERROR"
                                    ,
                               Ref_Order =
                                    tNonAPI.Transaction_Type == "0001" ? tNonAPI.Ref_Order :
                                    tNonAPI.Transaction_Type == "0002" ? tNonAPI.Ref_Order_Sales :
                                    "ERROR",
                               Transaction_Date = tNonAPI.Transaction_Date,
                               Transaction_Type = tNonAPI.Transaction_Type == "0001" ? 1 :
                                                tNonAPI.Transaction_Type == "0002" ? 2 :
                                                0,
                               Channel_ID = tNonAPI.Channel_ID,
                               Channel_Name = tChannel.Channel_Name,
                               c_Name = tNonAPI.CustomerFirstName + " " + tNonAPI.CustomerLastName,
                               s_Name = tNonAPI.AddressShipping_FirstName + " " + tNonAPI.AddressShipping_LastName,
                               s_Address1 = tNonAPI.AddressShipping_Address1,
                               s_Address2 = tNonAPI.AddressShipping_Address2,
                               s_District_ID = tNonAPI.AddressShipping_District_ID,
                               s_City_ID = tNonAPI.AddressShipping_Amphur_ID,
                               s_Province_ID = tNonAPI.AddressShipping_Province_ID,
                               s_PostCode = tNonAPI.AddressShipping_PostCode,
                               s_Phone1 = tNonAPI.AddressShipping_Tel1,
                               s_Phone2 = tNonAPI.AddressShipping_Tel2,
                               b_Name = tNonAPI.AddressBilling_FirstName + " " + tNonAPI.AddressBilling_LastName,
                               StorageWM_ID = 0,
                               Date_Print = tNonAPI.Date_Print,
                               Owner_ID = 1,
                               TypeDocument_ID = "NORMAL",
                               TypeDocument_Name = "ทั่วไป",
                               No = tNonAPI_Item.No,
                               SKU = tNonAPI_Item.SKU

                           });

            return objData;
        }

    }

}