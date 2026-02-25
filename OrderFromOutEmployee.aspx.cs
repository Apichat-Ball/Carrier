using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Service;

namespace Carrier
{
    public partial class OrderFromOutEmployee : System.Web.UI.Page
    {
        CarrierEntities carrier_Entities = new CarrierEntities();
        InsideSFG_WFEntities insideSFG_WF_Entities = new InsideSFG_WFEntities();

        Service_Flash service_Flash = new Service_Flash();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_UserID"] == null)
            {
                service_Flash.Check_UserID();
            }
            if (Session["_UserID"] == null)
            {
                Response.Redirect("https://www.sfg-th.com/Login/Default.aspx?Page=Carrier/");
            }
            lbuserid.Text = Session["_UserID"].ToString();

            if (!IsPostBack)
            {
                txtDateStart.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDateEnd.Text = DateTime.Now.ToString("dd/MM/yyyy");

                var userHaveList = carrier_Entities.User_View.Where(w => w.UserID_H == lbuserid.Text).ToList() ;
                if (userHaveList.Any())
                {
                    userHaveList.Insert(0, new User_View
                    {
                        UserID_H = "-",
                        UserID_S = "ทั้งหมด"
                    });
                    List<listDDLUser> user = new List<listDDLUser>();
                    foreach(var uh in userHaveList)
                    {
                        if(uh.UserID_H != "-")
                        {
                            var userint = Convert.ToInt32(uh.UserID_S);
                            var userstr = insideSFG_WF_Entities.Employees.Where(w => w.userID == userint).FirstOrDefault();

                            var userstrFull = userstr.name + " " + userstr.surname;
                            user.Add(new listDDLUser { userid = uh.UserID_S, username = userstrFull });
                        }
                        else
                        {
                            user.Add(new listDDLUser { userid = uh.UserID_S, username = uh.UserID_S });
                        }
                        
                    }
                    ddlUserS.DataSource = user;
                    ddlUserS.DataBind();
                }
            }
        }
        private DataTable LoadData()
        {
            // แปลงวันที่จาก TextBox
            if (string.IsNullOrEmpty(txtDateStart.Text) || string.IsNullOrEmpty(txtDateEnd.Text))
            {
                // จัดการกรณีที่ TextBox ว่างเปล่า
                return new DataTable();
            }

            // ใช้ try-catch หรือ TryParse เพื่อป้องกันการแปลงวันที่ผิดรูปแบบ
            if (!DateTime.TryParseExact(txtDateStart.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime datest))
            {
                datest = DateTime.MinValue;
            }
            if (!DateTime.TryParseExact(txtDateEnd.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateed))
            {
                dateed = DateTime.MaxValue;
            }

            var dateStTime = new DateTime(datest.Year, datest.Month, datest.Day, 0, 0, 1);
            var dateEdTime = new DateTime(dateed.Year, dateed.Month, dateed.Day, 23, 59, 59); // ให้ถึงสิ้นสุดวัน

            var BFID = txtDocnoSearch.Text;

            // สร้าง DataTable Structure
            DataTable dt = new DataTable();
            dt.Columns.Add("BFID");
            dt.Columns.Add("UserCreate");
            dt.Columns.Add("Brand");
            dt.Columns.Add("NameSend");
            dt.Columns.Add("AddressSend");
            dt.Columns.Add("NameRecieve");
            dt.Columns.Add("AddressRecieve");
            dt.Columns.Add("DateCreate");
            dt.Columns.Add("TypeSend");
            dt.Columns.Add("Status");


            // ดึงข้อมูลจาก Entity Framework
            var userHave = carrier_Entities.User_View.Where(w => w.UserID_H == lbuserid.Text && (w.UserID_S == ddlUserS.SelectedValue || ddlUserS.SelectedValue == "ทั้งหมด")).ToList();

            List<listModel> CarrierList = new List<listModel>();
            foreach(var user in userHave)
            {
                var userSubInt = Convert.ToInt32(user.UserID_S);
                var userInSFG = insideSFG_WF_Entities.Employees.Where(w => w.userID == userSubInt).FirstOrDefault();
                var userSTR = userInSFG.name + " " + userInSFG.surname + "("+ userInSFG.nick + ")";
                var carrierHaveFromUser = (from bf in carrier_Entities.Order_Big_Box
                                           join o in carrier_Entities.Orders on bf.Docno equals o.Docno
                                           join so in carrier_Entities.Lalamove_Car_Status on bf.Lala_Car_Status equals so.Status_Lala_Code into sta
                                           from staLa in sta.DefaultIfEmpty()
                                           where o.UserID == userSubInt && bf.Status == "A"  && (o.Date_send >= dateStTime && o.Date_send <= dateEdTime) 
                                           && (bf.BFID.Contains(BFID) || BFID == "") 
                                           select new
                                           {
                                               bfid = bf.BFID,
                                               depart = o.SDpart,
                                               UserCreate = userSTR,
                                               Date_send = o.Date_send,
                                               srcName = o.srcName,
                                               SRCAddress = o.srcDetailAddress + " " + o.srcDistrictName + " " + o.srcCityName + " " + o.srcProvinceName + " " + o.srcPostalCode,
                                               dstName = o.dstName,
                                               DSTAddress = o.dstDetailAddress + " " + o.dstDistrictName + " " + o.dstCityName + " " + o.dstProvinceName + " " + o.dstPostalCode,
                                               Transport_Type = o.Transport_Type,
                                               Status = bf.Lala_Car_Key == null ? bf.StatusNotifyText : staLa.Status_Lala_Name_TH
                                           }).GroupBy(g => g.bfid)
                                           .Select(s => new listModel
                                           {
                                               BFID = s.Key,
                                               UserCreate = s.FirstOrDefault().UserCreate,
                                               Brand = s.FirstOrDefault().depart,
                                               NameSend = s.FirstOrDefault().srcName,
                                               AddressSend = s.FirstOrDefault().SRCAddress,
                                               NameRecieve = s.FirstOrDefault().dstName,
                                               AddressRecieve = s.FirstOrDefault().DSTAddress,
                                               DateCreate = s.FirstOrDefault().Date_send,
                                               TypeSend = s.FirstOrDefault().Transport_Type == 2 ? "Lalamove" : "Flash",
                                               Status = s.FirstOrDefault().Status
                                           }).ToList();
                CarrierList.AddRange(carrierHaveFromUser);
            }
            if (CarrierList.Any())
            {
                CarrierList = CarrierList.OrderByDescending(o => o.DateCreate).ToList();
            }

            // แปลง List<T> เป็น DataTable
            foreach (var et in CarrierList)
            {
                var brand = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.departmentID == et.Brand).FirstOrDefault();


                dt.Rows.Add(
                    et.BFID,
                    et.UserCreate,
                    brand.department_,
                    et.NameSend,
                    et.AddressSend,
                    et.NameRecieve,
                    et.AddressRecieve,
                    et.DateCreate?.ToString("dd/MM/yyyy"),
                    et.TypeSend,
                    et.Status
                );
            }
            return dt;
        }
        private void BindGrid()
        {
            DataTable dt = LoadData();
            gv_data.DataSource = dt;
            gv_data.DataBind();

            // เก็บ DataTable ไว้ใน ViewState เพื่อใช้ในการอัปเดตสถานะ "IsUploading" ใน RowCommand
            ViewState["CurrentTaxData"] = dt;
        }
        protected void gv_data_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // รับค่า BFID จาก CommandArgument
            string BFID = e.CommandArgument.ToString();


            if (e.CommandName == "Open")
            {
                // สร้าง URL
                string url = $"Transport_Form.aspx?Docno={BFID}&PM=6";

                // ✅ ใช้ ScriptManager เพื่อเปิด URL ในแท็บใหม่
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenNewTab",
                    $"window.open('{url}', '_blank');", true);
            }
        }

        public class listModel
        {
            public string BFID { get; set; }
            public string UserCreate { get; set; }
            public string Brand { get; set; }
            public string NameSend { get; set; }
            public string NameRecieve { get; set; }
            public string AddressSend { get; set; }
            public string AddressRecieve { get; set; }
            public DateTime? DateCreate { get; set; }
            public string TypeSend { get; set; }
            public string Status { get; set; }


        }
        public class listDDLUser
        {
            public string userid { get; set; }
            public string username { get; set; }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void ddlUserS_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}