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
        public responseNotify()
        {
            pno = new List<string>();
        }
    }
}