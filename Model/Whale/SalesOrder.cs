//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Carrier.Model.Whale
{
    using System;
    using System.Collections.Generic;
    
    public partial class SalesOrder
    {
        public string Docno { get; set; }
        public Nullable<System.DateTime> Date_Send { get; set; }
        public string Status { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<System.DateTime> POS_Date { get; set; }
        public Nullable<System.DateTime> Transaction_Date { get; set; }
        public Nullable<int> Owner_ID { get; set; }
        public Nullable<int> StorageWM_ID { get; set; }
        public Nullable<int> Channel_ID { get; set; }
        public Nullable<int> Transaction_Type { get; set; }
        public string Ref_Order { get; set; }
        public string c_Name { get; set; }
        public string c_Phone { get; set; }
        public string c_Email { get; set; }
        public string s_Name { get; set; }
        public string s_Address1 { get; set; }
        public string s_Address2 { get; set; }
        public string s_PostCode { get; set; }
        public Nullable<int> s_Province_ID { get; set; }
        public Nullable<int> s_Amphur_ID { get; set; }
        public Nullable<int> s_District_ID { get; set; }
        public string s_Phone1 { get; set; }
        public string s_Phone2 { get; set; }
        public Nullable<bool> same_shipping { get; set; }
        public string b_Email { get; set; }
        public string b_IDCard { get; set; }
        public string b_BranchID { get; set; }
        public string b_Name { get; set; }
        public string b_Address1 { get; set; }
        public string b_Address2 { get; set; }
        public string b_PostCode { get; set; }
        public Nullable<int> b_Province_ID { get; set; }
        public string b_Province_Name { get; set; }
        public Nullable<int> b_Amphur_ID { get; set; }
        public Nullable<int> b_District_ID { get; set; }
        public string b_Phone1 { get; set; }
        public string b_Phone2 { get; set; }
        public Nullable<int> InventoryDocID { get; set; }
        public Nullable<System.DateTime> Date_Print { get; set; }
        public Nullable<int> UserID_Print { get; set; }
        public Nullable<System.DateTime> Date_Tracking { get; set; }
        public Nullable<int> UserID_Tracking { get; set; }
        public Nullable<System.DateTime> Date_Boxed { get; set; }
        public Nullable<int> UserID_Boxed { get; set; }
        public string Remark { get; set; }
        public string Note { get; set; }
        public string TypeDocument_ID { get; set; }
        public string WODocno { get; set; }
        public Nullable<double> Shipping_Price { get; set; }
        public string Code_Commission { get; set; }
        public Nullable<bool> Insert_Account { get; set; }
        public string Ref_Packaging { get; set; }
        public string RecheckEtax { get; set; }
        public Nullable<System.DateTime> Shipbydate { get; set; }
        public string Claim_Docno { get; set; }
    }
}