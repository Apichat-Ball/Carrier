using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Carrier.Model.Carrier;

namespace Carrier.Info
{
    public class CreateOrderInfo
    {

    }
    public class OrderToNoti : Order
    {
        public int estimateParcelNumber { get; set; }
        public string mchId { get; set; }
        public string pno { get; set; }
        public string TypeSendKo { get; set; }
    }
    public class responseNotify
    {
        public List<string> pno { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public string ticketPickupId { get; set; }
        public int staffInfoId { get; set; }
        public string staffInfoName { get; set; }
        public string staffInfoPhone { get; set; }
        public string upCountryNote { get; set; }
        public string timeoutAtText { get; set; }
        public string ticketMessage { get; set; }
        public DateTime dateSuccess { get; set; }
        public string warehouseNo { get; set; }
        public responseNotify()
        {
            pno = new List<string>();
        }
    }
    public class Warehouse
    {
        public string warehouseNo { get; set; }
        public string name { get; set; }
        public string countryName { get; set; }
        public string provinceName { get; set; }
        public string cityName { get; set; }
        public string districtName { get; set; }
        public string postalCode { get; set; }
        public string detailAddress { get; set; }
        public string phone { get; set; }
        public string srcName { get; set; }
    }
    public class ReportBrand 
    {
        public string Docno { get; set; }
        public DateTime? Date_send { get; set; }
        public string status { get; set; }
        public string SDpart { get; set; }
        public string SDpart_Name { get; set; }
        public string SDpart_Name_Full { get; set; }
        public string saleOn { get; set; }
        public string siteStorage { get; set; }
        public string saleChannel { get; set; }
        public int? TypeSend { get; set; }
        public int Qty { get; set; }
    }
}