//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Carrier.Model.Carrier
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order_Item
    {
        public string Docno { get; set; }
        public Nullable<System.DateTime> Date_Success { get; set; }
        public string sign { get; set; }
        public string pno { get; set; }
        public string mchId { get; set; }
        public string sortCode { get; set; }
        public string dstStoreName { get; set; }
        public string sortingLineCode { get; set; }
        public string earlyFlightEnabled { get; set; }
        public string packEnabled { get; set; }
        public string upcountryCharge { get; set; }
        public Nullable<int> Qty { get; set; }
        public string Status { get; set; }
        public Nullable<int> CodeResponse { get; set; }
        public string ticketPickupId { get; set; }
        public string TypeSendKO { get; set; }
    }
}
