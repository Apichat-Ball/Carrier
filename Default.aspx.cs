using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Service;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using System.IO;
using System.Net;
using System.Globalization;

namespace Carrier
{
    public partial class _Default : Page
    {
        Service_Flash service_Flashs;
        CarrierEntities carrier_Entities;
        InsideSFG_WFEntities insideSFG_WF_Entities;
        public _Default()
        {
            service_Flashs = new Service_Flash();
            carrier_Entities = new CarrierEntities();
            insideSFG_WF_Entities = new InsideSFG_WFEntities();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();
            //HttpContext.Current.Session["_UserID"] = "101635";
            if (Session["_UserID"] == null)
            {
                service_Flashs.Check_UserID();
            }
            if (Session["_UserID"] == null)
            {
                Response.Redirect("http://www.sfg-th.com/Login/");
            }
            lbuserid.Text = Session["_UserID"].ToString();
            if (!IsPostBack)
            {
                lbStatusSearch.Text = "First";
                txtDateStart.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
                txtDateEnd.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                loadtable(1);
            }
        }
        public void loadtable(int page)
        {
            var user = Convert.ToInt32(Session["_UserID"].ToString());
            var permission = carrier_Entities.Users.Where(w => w.UserID == user).FirstOrDefault();
            if (permission != null && permission.Permission == "Admin")
            {
                var maxrow = 10;
                var orderList = (from orderItem in carrier_Entities.Order_Item
                                 join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                                 select new
                                 {
                                     Docno = orderItem.Docno,
                                     nameCreate = order.UserID,
                                     pno = orderItem.pno,
                                     srcName = order.srcName,
                                     dstName = order.dstName,
                                     ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                     dateCreate = orderItem.Date_Success,
                                     TrackingPickup = orderItem.ticketPickupId,
                                     TimeTracking = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.DateNotify).ToList().FirstOrDefault(),
                                     TimeTrackingText = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.TimeoutAtText).ToList().FirstOrDefault() ?? "",
                                     Brand = order.SDpart,
                                     status = orderItem.Status,
                                     Remark = order.remark,
                                     TypeSend = order.TypeSend,
                                     StaffInfoName = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.StaffInfoName).ToList().FirstOrDefault(),
                                     Transport_Type = order.Transport_Type,
                                     TypeSendKa = orderItem.TypeSendKO
                                 }).ToList();

                var format = "dd/MM/yyyy";
                var enUS = new CultureInfo("en-US");
                if(permission.TypeWarehouse != null)
                {
                    if(permission.TypeWarehouse == "SFG")
                    {
                        orderList = orderList.Where(w => w.TypeSendKa != "SDC1").ToList();
                    }
                    else
                    {

                    orderList = orderList.Where(w => w.TypeSendKa == permission.TypeWarehouse).ToList();
                    }
                }
                if (txtDateStart.Text != "" && txtDateEnd.Text != "")
                {
                    var start = DateTime.ParseExact(txtDateStart.Text, format, enUS, DateTimeStyles.None);
                    var end = DateTime.ParseExact(txtDateEnd.Text, format, enUS, DateTimeStyles.None);
                    switch (ddlStatusOrder.SelectedValue)
                    {
                        case "1":
                            orderList = orderList.Where(w => w.status != "A" && w.status != "SP" && w.status != "SL" && w.status != "C").ToList();
                            if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "")
                            {
                                //orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                                orderList = orderList.Where(w => (w.Docno.Contains(txtDocnoSearch.Text) || txtDocnoSearch.Text == "")
                                && (w.pno.Contains(txtPnoSearch.Text.ToUpper()) || txtPnoSearch.Text == "")
                                && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")).ToList();

                            }
                            else if(lbStatusSearch.Text != "First")
                            {
                                orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                            }
                            
                            break;
                        case "2":
                            orderList = orderList.Where(w => w.status != null && w.status != "C").ToList();
                            if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "")
                            {
                                //orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                                if (txtPnoSearch.Text != "")
                                {
                                    orderList = orderList.Where(w => w.pno != null).ToList();
                                    orderList = orderList.Where(w => (w.Docno.Contains(txtDocnoSearch.Text) || txtDocnoSearch.Text == "")
                                    && (w.pno.StartsWith(txtPnoSearch.Text.ToUpper()) || txtPnoSearch.Text == "")
                                    && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                    && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")).ToList();
                                }
                                else
                                {
                                    orderList = orderList.Where(w => (w.Docno.Contains(txtDocnoSearch.Text) || txtDocnoSearch.Text == "")
                                    && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                    && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")).ToList();
                                }


                            }
                            else
                            {
                                orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                            }
                            break;
                        case "3":
                            orderList = orderList.Where(w =>w.status == "C").ToList();
                            if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "")
                            {
                                //orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                                if (txtPnoSearch.Text != "")
                                {
                                    orderList = orderList.Where(w => w.pno != null).ToList();
                                    orderList = orderList.Where(w => (w.Docno.Contains(txtDocnoSearch.Text) || txtDocnoSearch.Text == "")
                                    && (w.pno.StartsWith(txtPnoSearch.Text.ToUpper()) || txtPnoSearch.Text == "")
                                    && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                    && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")).ToList();
                                }
                                else
                                {
                                    orderList = orderList.Where(w => (w.Docno.Contains(txtDocnoSearch.Text) || txtDocnoSearch.Text == "")
                                    && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                    && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")).ToList();
                                }


                            }
                            else
                            {
                                orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                            }
                            break;
                    }

                    double maxdata_gvData = (double)((decimal)Convert.ToDecimal(orderList.Count()) / Convert.ToDecimal(maxrow));
                    int pageCount_gvData = (int)Math.Ceiling(maxdata_gvData);
                    gv_OrderAll.DataSource = orderList.OrderByDescending(x => x.dateCreate).Skip((page - 1) * maxrow).Take(maxrow);
                    gv_OrderAll.DataBind();
                    Page_gv(page, pageCount_gvData);

                    foreach (GridViewRow row in gv_OrderAll.Rows)
                    {
                        LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");
                        CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                        Label lbDateCreate = (Label)row.FindControl("lbDateCreate");
                        Label lbStatus = (Label)row.FindControl("lbStatus");
                        Label lbTimeTrackingText = (Label)row.FindControl("lbTimeTrackingText");
                        Label lbTimeTracking = (Label)row.FindControl("lbTimeTracking");
                        Label lbStatusItem = (Label)row.FindControl("lbStatusItem");
                        ImageButton imgbtnCancelOrder = (ImageButton)row.FindControl("imgbtnCancelOrder");
                        Label lbTypeSend = (Label)row.FindControl("lbTypeSend");
                        Label lbTransport_Type = (Label)row.FindControl("lbTransport_Type");
                        lbDateCreate.Text = DateTime.Parse(lbDateCreate.Text).ToString("dd/MM/yyyy");
                        Label lbUserCreate = (Label)row.FindControl("lbUserCreate");
                        
                        var userid = Convert.ToInt32(lbUserCreate.Text);
                        var emp = insideSFG_WF_Entities.Employees.Where(w => w.userID == userid).FirstOrDefault();
                        lbUserCreate.Text = emp.name+" "+ emp.surname+"("+ emp.nick+ ")";
                        if (lbTransport_Type.Text == "1")
                        {
                            lbTransport_Type.Text = "FlashExpress";
                        }
                        else if (lbTransport_Type.Text == "2")
                        {
                            lbTransport_Type.Text = "Lalamove";
                        }
                        if (lbStatus.Text != "")
                        {
                            var dateNotiDate = new DateTime();
                            if (lbTimeTrackingText.Text.Contains("พรุ่งนี้"))
                            {
                                var dateRaw = DateTime.Parse(lbTimeTracking.Text).AddDays(1);
                                dateNotiDate = DateTime.Parse(dateRaw.ToShortDateString());
                                var dateToUpdate = dateNotiDate.AddHours(16).AddMinutes(45);
                                if (DateTime.Now >= dateToUpdate)
                                {
                                    var date = DateTime.Parse(lbTimeTracking.Text);
                                    var a  = service_Flashs.CheckNotify(lkbDocno.Text);
                                    if(a != "")
                                    {
                                        lbTimeTrackingText.Text = service_Flashs.CheckNotify( lkbDocno.Text);
                                    }
                                }
                                else
                                {
                                    if (DateTime.Now.ToShortDateString() == dateToUpdate.ToShortDateString()) { 
                                        lbTimeTrackingText.Text = "วันนี้" + lbTimeTrackingText.Text.Substring(8);
                                    }
                                }
                            }
                            else if (lbTimeTrackingText.Text.Contains("วันนี้"))
                            {
                                var dateRaw = DateTime.Parse(lbTimeTracking.Text);
                                dateNotiDate = DateTime.Parse(dateRaw.ToShortDateString());
                                var dateToUpdate = dateNotiDate.AddHours(17).AddMinutes(30);
                                if (DateTime.Now >= dateToUpdate)
                                {
                                    var date = DateTime.Parse(lbTimeTracking.Text);
                                    var a = service_Flashs.CheckNotify(lkbDocno.Text);
                                    if (a != "")
                                    {
                                        lbTimeTrackingText.Text = service_Flashs.CheckNotify(lkbDocno.Text);
                                    }
                                    else
                                    {
                                        lbTimeTrackingText.Text = "ยังไม่ได้มารับของ";
                                        
                                    }
                                }
                            }
                            if (lbTimeTrackingText.Text.Contains("ยกเลิกแล้ว"))
                            {
                                lbTimeTrackingText.BackColor = System.Drawing.Color.PaleVioletRed;
                                lbTimeTrackingText.ForeColor = System.Drawing.Color.White;
                                lbTimeTrackingText.CssClass = "status-tracking";
                            }
                            else if (lbTimeTrackingText.Text.Contains("ยังไม่ได้มารับของ"))
                                {
                                lbTimeTrackingText.BackColor = System.Drawing.Color.Orange;
                                lbTimeTrackingText.ForeColor = System.Drawing.Color.White;
                                lbTimeTrackingText.CssClass = "status-tracking";
                            }
                            else
                            {
                                lbTimeTrackingText.BackColor = System.Drawing.Color.LimeGreen;
                                lbTimeTrackingText.ForeColor = System.Drawing.Color.White;
                                lbTimeTrackingText.CssClass = "status-tracking";
                            }
                            cbItem.Visible = false;
                            imgbtnCancelOrder.Visible = false;
                        }
                        else
                        {
                            if (lbStatusItem.Text == "A")
                            {
                                var a = service_Flashs.CheckNotify(lkbDocno.Text);
                                if (a != "")
                                {
                                    lbTimeTrackingText.Text = service_Flashs.CheckNotify(lkbDocno.Text);
                                }
                                if (lbTimeTrackingText.Text.Contains("ยกเลิกแล้ว"))
                                {
                                    lbTimeTrackingText.BackColor = System.Drawing.Color.PaleVioletRed;
                                    lbTimeTrackingText.ForeColor = System.Drawing.Color.White;
                                    lbTimeTrackingText.CssClass = "status-tracking";
                                }
                                else
                                {
                                    lbTimeTrackingText.BackColor = System.Drawing.Color.LimeGreen;
                                    lbTimeTrackingText.ForeColor = System.Drawing.Color.White;
                                    lbTimeTrackingText.CssClass = "status-tracking";
                                }
                                cbItem.Visible = false;
                                imgbtnCancelOrder.Visible = false;
                            }
                        }
                        gv_OrderAll.Columns[0].Visible = true;
                        gv_OrderAll.Columns[10].Visible = true;
                        gv_OrderAll.Columns[11].Visible = true;
                        gv_OrderAll.Columns[12].Visible = true;
                        if (lbStatusItem.Text == "SP")
                        {
                            lbTimeTrackingText.Text = "ส่งผ่านไปรษณีย์แล้ว";
                            cbItem.Visible = false;
                            lbTimeTrackingText.BackColor = System.Drawing.Color.Gray;
                            lbTimeTrackingText.CssClass = "status-tracking";
                            imgbtnCancelOrder.Visible = false;
                        }
                        if (lbStatusItem.Text == "SL")
                        {
                            lbTimeTrackingText.Text = "ส่งผ่าน Lalamove";
                            cbItem.Visible = false;
                            lbTimeTrackingText.BackColor = System.Drawing.Color.Orange;
                            lbTimeTrackingText.CssClass = "status-tracking";
                            imgbtnCancelOrder.Visible = false;
                        }
                        if(lbStatusItem.Text == "C")
                        {
                            cbItem.Visible = false;
                            imgbtnCancelOrder.Visible = false;
                            gv_OrderAll.Columns[0].Visible = false;
                            gv_OrderAll.Columns[10].Visible = false;
                            gv_OrderAll.Columns[11].Visible = false;
                            gv_OrderAll.Columns[12].Visible = false;

                        }
                        if (lbTypeSend.Text == "2")
                        {
                            cbItem.Visible = false;
                            imgbtnCancelOrder.Visible = false;
                        }
                        Label lbBrand = (Label)row.FindControl("lbBrand");
                        var Brand = (from BG_HA in insideSFG_WF_Entities.BG_HApprove
                                     join BG_HAPF in insideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
                                     where BG_HA.departmentID == lbBrand.Text
                                     select new { Brand = BG_HA.department_, BrandShort = BG_HAPF.Depart_Short }).ToList().FirstOrDefault();
                        if (Brand != null)
                        {
                            lbBrand.Text = Brand.Brand;
                            lbBrand.ToolTip = Brand.Brand;
                        }
                        else
                        {
                            lbBrand.Text = "";
                        }
                    }

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเลือกวันที่เริ่มและสิ้นสุดการค้นหา')", true);
                }

            }
            else if (permission == null)
            {
                Response.Redirect("Home_Carrier.aspx");
            }
        }
        public void LoadAllTableCheckAll()
        {
            var user = Convert.ToInt32(Session["_UserID"].ToString());
            var permission = carrier_Entities.Users.Where(w => w.UserID == user).FirstOrDefault();
            var orderList = (from orderItem in carrier_Entities.Order_Item
                             join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                             where orderItem.Status != "C"
                             select new
                             {
                                 Docno = orderItem.Docno,
                                 pno = orderItem.pno,
                                 nameCreate = order.UserID,
                                 srcName = order.srcName,
                                 dstName = order.dstName,
                                 ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                 dateCreate = orderItem.Date_Success,
                                 TrackingPickup = orderItem.ticketPickupId,
                                 TimeTracking = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.DateNotify).ToList().FirstOrDefault(),
                                 TimeTrackingText = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.TimeoutAtText).ToList().FirstOrDefault() ?? "",
                                 Brand = order.SDpart,
                                 status = orderItem.Status,
                                 Remark = order.remark,
                                 TypeSend = order.TypeSend,
                                 StaffInfoName = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.StaffInfoName).ToList().FirstOrDefault(),
                                 Transport_Type = order.Transport_Type,
                                 TypeSendKa = orderItem.TypeSendKO
                             }).ToList();
            if(permission.TypeWarehouse != null)
            {
                orderList = orderList.Where(w => w.TypeSendKa == permission.TypeWarehouse).ToList();
            }
            orderList = orderList.Where(w => w.status != "A" && w.status != "SP" && w.status != "SL").ToList();
            gv_OrderAll.DataSource = orderList.OrderByDescending(x => x.dateCreate);
            gv_OrderAll.DataBind();
            CheckBox cbAll = (CheckBox)gv_OrderAll.HeaderRow.FindControl("cbAll");
            cbAll.Checked = true;
            foreach (GridViewRow row in gv_OrderAll.Rows)
            {
                LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");
                CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                Label lbDateCreate = (Label)row.FindControl("lbDateCreate");
                Label lbStatus = (Label)row.FindControl("lbStatus");
                Label lbTimeTrackingText = (Label)row.FindControl("lbTimeTrackingText");
                Label lbTimeTracking = (Label)row.FindControl("lbTimeTracking");
                Label lbStatusItem = (Label)row.FindControl("lbStatusItem");
                ImageButton imgbtnCancelOrder = (ImageButton)row.FindControl("imgbtnCancelOrder");
                Label lbTypeSend = (Label)row.FindControl("lbTypeSend");
                Label lbTransport_Type = (Label)row.FindControl("lbTransport_Type");
                lbDateCreate.Text = DateTime.Parse(lbDateCreate.Text).ToString("dd/MM/yyyy");
                Label lbUserCreate = (Label)row.FindControl("lbUserCreate");
                var userid = Convert.ToInt32(lbUserCreate.Text);
                var emp = insideSFG_WF_Entities.Employees.Where(w => w.userID == userid).FirstOrDefault();
                lbUserCreate.Text = emp.name + " " + emp.surname + "(" + emp.nick + ")";
                cbItem.Checked = true;
                if (lbTransport_Type.Text == "1")
                {
                    lbTransport_Type.Text = "FlashExpress";
                }
                else if (lbTransport_Type.Text == "2")
                {
                    lbTransport_Type.Text = "Lalamove";
                }
                if (lbStatus.Text != "")
                {
                    var dateNoti = new DateTime();
                    if (lbTimeTrackingText.Text.Contains("พรุ่งนี้"))
                    {
                        dateNoti = DateTime.Parse((DateTime.Parse(lbTimeTracking.Text).AddDays(1)).ToShortDateString());
                        if (dateNoti <= DateTime.Now)
                        {
                            var date = DateTime.Parse(lbTimeTracking.Text);
                            var a = service_Flashs.CheckNotify(lkbDocno.Text);
                            if (a != "")
                            {
                                lbTimeTrackingText.Text = service_Flashs.CheckNotify(lkbDocno.Text);
                            }
                        }
                        else
                        {
                            lbTimeTrackingText.Text = "วันนี้" + lbTimeTrackingText.Text.Substring(8);
                        }
                    }
                    else if (lbTimeTrackingText.Text.Contains("วันนี้"))
                    {
                        dateNoti = DateTime.Parse(lbTimeTracking.Text);
                        if (dateNoti.ToShortDateString() != DateTime.Now.ToShortDateString())
                        {
                            var date = DateTime.Parse(lbTimeTracking.Text);
                            var a = service_Flashs.CheckNotify(lkbDocno.Text);
                            if (a != "")
                            {
                                lbTimeTrackingText.Text = service_Flashs.CheckNotify(lkbDocno.Text);
                            }
                        }
                    }
                    cbItem.Visible = false;
                    imgbtnCancelOrder.Visible = false;
                }
                if (lbStatusItem.Text == "SP")
                {
                    lbTimeTrackingText.Text = "ส่งผ่านไปรษณีย์แล้ว";
                    cbItem.Visible = false;
                    imgbtnCancelOrder.Visible = false;
                }
                if (lbStatusItem.Text == "SL")
                {
                    lbTimeTrackingText.Text = "ส่งผ่าน Lalamove";
                    cbItem.Visible = false;
                    imgbtnCancelOrder.Visible = false;
                }
                if (lbTypeSend.Text == "2")
                {
                    cbItem.Visible = false;
                    imgbtnCancelOrder.Visible = false;
                }
                Label lbBrand = (Label)row.FindControl("lbBrand");
                var Brand = (from BG_HA in insideSFG_WF_Entities.BG_HApprove
                             join BG_HAPF in insideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
                             where BG_HA.departmentID == lbBrand.Text
                             select new { Brand = BG_HA.department_, BrandShort = BG_HAPF.Depart_Short }).ToList().FirstOrDefault();
                if (Brand != null)
                {
                    lbBrand.Text = Brand.Brand;
                }
                else
                {
                    lbBrand.Text = "";
                }
            }
        }
        protected void Page_gv(int pageselect, int pageCount)
        {
            //จัดจำนวน row in table เพื่อแสดง
            lkPrevious.Visible = true;
            lkNext.Visible = true;
            lk1.Visible = false;
            lk2.Visible = false;
            lk3.Visible = false;
            lkFirst.Visible = false;
            lkLast.Visible = false;
            lkFirst.Text = "1";
            lkFirst.CommandArgument = "1";
            lkLast.Text = pageCount.ToString();
            lkLast.CommandArgument = pageCount.ToString();
            if (pageCount <= 3)
            {
                switch (pageCount)
                {
                    case 1:
                        lk1.Visible = true; lk1.Text = Convert.ToString(pageCount);
                        break;
                    case 2:
                        lk1.Visible = true;
                        lk2.Visible = true;
                        lk1.Text = Convert.ToString(pageCount - 1);
                        lk2.Text = Convert.ToString(pageCount);
                        break;
                    case 3:
                        lk1.Visible = true; lk1.Text = Convert.ToString(pageCount - 2);
                        lk2.Visible = true; lk2.Text = Convert.ToString(pageCount - 1);
                        lk3.Visible = true; lk3.Text = Convert.ToString(pageCount);
                        break;
                }


            }
            else
            {
                lk1.Visible = true; lk2.Visible = true; lk3.Visible = true;
                switch (pageselect)
                {
                    case 1:
                        lk1.Text = Convert.ToString(pageselect); lk2.Text = Convert.ToString(pageselect + 1); lk3.Text = Convert.ToString(pageselect + 2);
                        break;
                    case 2:
                        lk1.Text = Convert.ToString(pageselect - 1); lk2.Text = Convert.ToString(pageselect); lk3.Text = Convert.ToString(pageselect + 1);
                        break;
                    case 3:
                        lk1.Text = Convert.ToString(pageselect - 2); lk2.Text = Convert.ToString(pageselect - 1); lk3.Text = Convert.ToString(pageselect);
                        break;

                    default:
                        lkFirst.Visible = true;
                        switch (pageselect == pageCount)
                        {
                            case false:
                                lk1.Text = Convert.ToString(pageselect - 1); lk2.Text = Convert.ToString(pageselect); lk3.Text = Convert.ToString(pageselect + 1);
                                break;

                            case true:
                                lk1.Text = Convert.ToString(pageselect - 2); lk2.Text = Convert.ToString(pageselect - 1); lk3.Text = Convert.ToString(pageselect);
                                break;
                        }
                        break;


                }

                //Last

            }
            lkPrevious.CommandArgument = Convert.ToString(pageselect - 1);
            lkNext.CommandArgument = Convert.ToString(pageselect + 1);
            var last = Convert.ToInt32(lkLast.Text);
            if (pageselect <= (pageCount - (last - pageselect == 1 ? 1 : 2)) && pageCount != 2) { lkLast.Visible = true; }
            lkPrevious.CssClass = "btn btn-outline-primary";
            lkNext.CssClass = "btn btn-outline-primary";
            if (pageselect - 1 <= 0) { lkPrevious.CssClass = "btn btn-outline-secondary disabled"; }
            if (pageselect + 1 > pageCount) { lkNext.CssClass = "btn btn-outline-secondary disabled"; }


            if (lkFirst.Text == pageselect.ToString()) { lkFirst.CssClass = "btn btn-outline-primary active"; } else { lkFirst.CssClass = "btn btn-outline-primary"; }
            if (lkLast.Text == pageselect.ToString()) { lkLast.CssClass = "btn btn-outline-primary active"; } else { lkLast.CssClass = "btn btn-outline-primary"; }
            if (lk1.Text == pageselect.ToString()) { lk1.CssClass = "btn btn-outline-primary active"; } else { lk1.CssClass = "btn btn-outline-primary"; }
            if (lk2.Text == pageselect.ToString()) { lk2.CssClass = "btn btn-outline-primary active"; } else { lk2.CssClass = "btn btn-outline-primary"; }
            if (lk3.Text == pageselect.ToString()) { lk3.CssClass = "btn btn-outline-primary active"; } else { lk3.CssClass = "btn btn-outline-primary"; }

            lk1.CommandArgument = lk1.Text; lk2.CommandArgument = lk2.Text; lk3.CommandArgument = lk3.Text;

        }
        protected void cbAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbAll = (CheckBox)sender;
            if (cbAll.Checked)
            {
                LoadAllTableCheckAll();
                div_Page_Bar.Visible = false;
                foreach (GridViewRow row in gv_OrderAll.Rows)
                {
                    CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                    if (cbItem.Visible == true)
                    {
                        cbItem.Checked = true;
                    }
                }
            }
            else
            {
                foreach (GridViewRow row in gv_OrderAll.Rows)
                {
                    CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                    cbItem.Checked = false;
                }
            }
        }

        protected void imgbtnCancelOrder_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnCancelOrder = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnCancelOrder.NamingContainer;
            Label lbpno = (Label)row.FindControl("lbpno");
            LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");
            var res = service_Flashs.CancelOrder(lkbDocno.Text, lbpno.Text);
            
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('succes : " + res + "')", true);

            
            loadtable(1);
        }
        protected void lkbDocno_Click(object sender, EventArgs e)
        {
            LinkButton lkbDocno = (LinkButton)sender;
            var lbDocnoss = lkbDocno.Text;
            Response.Redirect("Transport_Form.aspx?Docno=" + lbDocnoss);
        }
        protected void selectPage(object sender, CommandEventArgs e)
        {
            loadtable(Convert.ToInt32(e.CommandArgument));
        }

        protected void btnNotifications_Click(object sender, ImageClickEventArgs e)
        {
            List<string> tracking = new List<string>();
            foreach (GridViewRow row in gv_OrderAll.Rows)
            {
                CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                Label lbpno = (Label)row.FindControl("lbpno");
                if (cbItem.Checked)
                {
                    tracking.Add(lbpno.Text);
                }
            }
            if (tracking.Count == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่ได้เลือก Order ที่ต้องการเรียกรถมารับพัสดุ')", true);
            }
            else
            {
                var responseNotifyList = service_Flashs.Notify(tracking);
                List<messageNotify> messageNoti = new List<messageNotify>();
                DateTime dateSuccess = DateTime.Now;
                List<History_Notify_Order> his = new List<History_Notify_Order>();
                var lastNolist = carrier_Entities.History_Notify_Order.ToList();
                var lastNo = "";
                if (lastNolist.Count == 0)
                {
                    lastNo = "HIS00001";
                }
                else
                {
                    lastNo = carrier_Entities.History_Notify_Order.OrderByDescending(o => o.History_ID).FirstOrDefault().History_NO;
                }
                var lenght = (Convert.ToInt32(lastNo.Substring(3, 5)) + 1).ToString().Length;
                var newNo = lastNo.Substring(0, 8 - lenght) + (Convert.ToInt32(lastNo.Substring(3, 5)) + 1).ToString();
                foreach (var responseNotify in responseNotifyList)
                {
                    if (responseNotify.code == 1)
                    {
                        var mess = "เลขที่พัสดุที่ : ";
                        var lastpno = responseNotify.pno.LastOrDefault();
                        foreach (var pno in responseNotify.pno)
                        {
                            var orderItem = carrier_Entities.Order_Item.Where(w => w.pno == pno).FirstOrDefault();
                            orderItem.Status = "A";
                            orderItem.CodeResponse = responseNotify.code;
                            orderItem.ticketPickupId = responseNotify.ticketPickupId;
                            if (lastpno == pno)
                            {
                                mess += pno + " เรียกรถเรียบร้อยแล้ว";
                            }
                            else
                            {
                                mess += pno + ",";
                            }
                            his.Add(new History_Notify_Order
                            {
                                History_NO = newNo,
                                Docno = orderItem.Docno,
                                Date_Notify = dateSuccess,
                                pno = pno,
                                Type_Send_KA = orderItem.TypeSendKO
                            });
                        }
                        carrier_Entities.Notifies.Add(new Notify
                        {
                            TicketPickupId = responseNotify.ticketPickupId,
                            StaffInfoId = responseNotify.staffInfoId,
                            StaffInfoName = responseNotify.staffInfoName,
                            StaffInfoPhone = responseNotify.staffInfoPhone,
                            UpCountryNote = responseNotify.upCountryNote,
                            TimeoutAtText = responseNotify.timeoutAtText,
                            TicketMessage = responseNotify.ticketMessage,
                            DateNotify = responseNotify.dateSuccess,
                            warehouseNo = responseNotify.warehouseNo
                        });
                        carrier_Entities.SaveChanges();
                        messageNoti.Add(new messageNotify { code = 1, message = mess });
                    }
                    else
                    {
                        if (responseNotify.code == 1010)
                        {
                            var mess = "เลขที่พัสดุที่ : ";
                            var notiOld = carrier_Entities.Notifies.Where(w => w.warehouseNo == responseNotify.warehouseNo).OrderByDescending(r => r.DateNotify).ToList();
                            foreach (var pno in responseNotify.pno)
                            {
                                var lastpno = responseNotify.pno.LastOrDefault();
                                var orderItem = carrier_Entities.Order_Item.Where(w => w.pno == pno).FirstOrDefault();
                                if (notiOld == null)
                                {
                                    orderItem.Status = "A";
                                    orderItem.CodeResponse = 1;
                                    //orderItem.ticketPickupId = ;
                                    carrier_Entities.SaveChanges();
                                }
                                else
                                {
                                    orderItem.Status = "A";
                                    orderItem.CodeResponse = 1;
                                    orderItem.ticketPickupId = notiOld.Select(s => s.TicketPickupId).FirstOrDefault();
                                    carrier_Entities.SaveChanges();
                                }
                                if (lastpno == pno)
                                {
                                    mess += pno + " เรียกรถเรียบร้อยแล้ว";
                                }
                                else
                                {
                                    mess += pno + ",";
                                }
                                var hisNoti = carrier_Entities.History_Notify_Order.Where(w => w.pno == pno).FirstOrDefault();
                                if(hisNoti == null)
                                {
                                    his.Add(new History_Notify_Order
                                    {
                                        History_NO = newNo,
                                        Docno = orderItem.Docno,
                                        Date_Notify = dateSuccess,
                                        pno = pno,
                                        Type_Send_KA = orderItem.TypeSendKO
                                    });
                                }
                            }
                        }
                        else
                        {

                            var mess = "เลขที่พัสดุที่ : ";
                            var lastpno = responseNotify.pno.LastOrDefault();
                            foreach (var pno in responseNotify.pno)
                            {
                                var orderItem = carrier_Entities.Order_Item.Where(w => w.pno == pno).FirstOrDefault();
                                orderItem.CodeResponse = responseNotify.code;
                                carrier_Entities.SaveChanges();
                                if (lastpno == pno)
                                {
                                    mess += pno + " ไม่สามารถเรียกรถได้";
                                }
                                else
                                {
                                    mess += pno + ",";
                                }
                            }
                            messageNoti.Add(new messageNotify { code = 1, message = mess });
                        }
                    }
                }
                if(his.Count != 0)
                {
                    carrier_Entities.History_Notify_Order.AddRange(his);
                    carrier_Entities.SaveChanges();
                }
                
                var messageAlert = "";
                foreach (var listmess in messageNoti)
                {
                    messageAlert = listmess.message + "\n";
                }


                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + messageAlert + "')", true);
                div_Page_Bar.Visible = true;
                Response.Redirect("Default.aspx");
            }

        }

        protected void btnCreateOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("Transport_Form.aspx");
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            lbStatusSearch.Text = "Second";
            btnClear.Visible = true;
            loadtable(1);
            div_Page_Bar.Visible = true;
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            lbStatusSearch.Text = "First";
            txtDateStart.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            txtDateEnd.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            btnClear.Visible = false;
            txtDocnoSearch.Text = "";
            txtPnoSearch.Text = "";
            txtDstNameSearch.Text = "";
            txtArticleSearch.Text = "";

            loadtable(1);
            div_Page_Bar.Visible = true;
        }



        protected void btnSendFree_Click(object sender, EventArgs e)
        {
            List<string> Docno = new List<string>();
            foreach (GridViewRow row in gv_OrderAll.Rows)
            {
                CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");
                if (cbItem.Checked)
                {
                    Docno.Add(lkbDocno.Text);
                }
            }
            if (Docno.Count == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ก่อนส่งไปรษณีย์กรุณาเลือกรายการที่ต้องการส่งครับ')", true);
            }
            carrier_Entities.Order_Item.Where(w => Docno.Contains(w.Docno)).ToList().ForEach(f => f.Status = "SP");
            carrier_Entities.SaveChanges();
            loadtable(1);
            div_Page_Bar.Visible = true;
        }


        protected void btnLalamove_Click(object sender, ImageClickEventArgs e)
        {
            List<string> Docno = new List<string>();
            foreach (GridViewRow row in gv_OrderAll.Rows)
            {
                CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");
                if (cbItem.Checked)
                {
                    Docno.Add(lkbDocno.Text);
                }
            }
            if (Docno.Count == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ก่อนเรียกรถ Lalamove กรุณาเลือกรายการที่ต้องการส่งครับ')", true);
            }
            carrier_Entities.Order_Item.Where(w => Docno.Contains(w.Docno)).ToList().ForEach(f => f.Status = "SL");
            carrier_Entities.SaveChanges();
            loadtable(1);
            div_Page_Bar.Visible = true;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var dateYesterday = DateTime.Now.AddDays(-1);
            var dateNow = DateTime.Now;
            var dateOld = new DateTime(dateYesterday.Year, dateYesterday.Month, dateYesterday.Day,16,30,1 );
            var dateNew = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 16, 30, 0);
            var user = Convert.ToInt32(Session["_UserID"].ToString());
            var permission = carrier_Entities.Users.Where(w => w.UserID == user).FirstOrDefault();
            List<History_Notify_Order> his = new List<History_Notify_Order>();
            if(permission.TypeWarehouse == "SFG")
            {
                his = carrier_Entities.History_Notify_Order.Where(w =>w.SaveFrom == null && w.Date_Notify >= dateOld && w.Date_Notify <= dateNew &&( w.Type_Send_KA == permission.TypeWarehouse||w.Type_Send_KA == null)).ToList();
            }
            else if(permission.TypeWarehouse == "SDC1")
            {
                his = carrier_Entities.History_Notify_Order.Where(w => w.SaveFrom == null && w.Date_Notify >= dateOld && w.Date_Notify <= dateNew && w.Type_Send_KA == permission.TypeWarehouse).ToList();
            }
            else
            {
                var ss = carrier_Entities.History_Notify_Order.ToList();
                his = carrier_Entities.History_Notify_Order.Where(w => w.SaveFrom == null && w.Date_Notify >= dateOld && w.Date_Notify <= dateNew ).ToList();
            }
            var listHistory = his.Select(s=>new
                {
                    History_NO = s.History_NO,
                    Date_Notify = s.Date_Notify,
                    Pno = s.pno,
                    Docno = s.Docno,
                    Type_Send_KA = s.Type_Send_KA
                }).OrderBy(o=>o.Docno).ToList();
            GridView history = new GridView();
            
            if (listHistory.Count == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('วันนี้ไม่พบการเรียกรถ Flash มารับตั้งแต่ 16.30 น. ของเมื่อวานถึง 16.30 น. ของวันนี้')", true);
            }
            else
            {
                history.DataSource = listHistory;
                history.DataBind();
                //GridViewRow rowHead = (GridViewRow)history.HeaderRow;
                //Label History_NO = (Label)rowHead.FindControl("History_NO");
                //Label Date_Notify = (Label)rowHead.FindControl("Date_Notify");
                //Label Pno = (Label)rowHead.FindControl("Pno");
                //Label Docno = (Label)rowHead.FindControl("Docno");
                //Label Type_Send_KA = (Label)rowHead.FindControl("Type_Send_KA");
                //Export Excel
                Page.Response.ClearContent();
                Page.Response.AddHeader("Content-Disposition", "attachment;filename=" + "KA_Per_Day_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".xls");
                Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Page.Response.Charset = "utf-8";
                Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-874");
                Page.Response.ContentType = "application/vnd.ms-excel";
                using (StringWriter strwritter = new StringWriter())
                {
                    HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
                    htmltextwrtter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                    htmltextwrtter.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=windows-874'>");
                    history.AllowPaging = false;
                    history.HeaderRow.BackColor = System.Drawing.Color.Yellow;
                    history.RenderControl(htmltextwrtter);
                    Page.Response.Output.Write(strwritter.ToString());
                    Page.Response.End();
                }
            }
            
        }

        protected void ddlStatusOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadtable(1);
            div_Page_Bar.Visible = true;
        }

        protected void btnUpdatePno_Click(object sender, EventArgs e)
        {
            var orderList = (from orderItem in carrier_Entities.Order_Item
                             join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                             where orderItem.Status == null
                             select order.Docno).ToList();
            foreach(var i in orderList)
            {
                var hi = carrier_Entities.History_Notify_Order.Where(w => w.Docno == i).ToList();
                var res = service_Flashs.CheckNotify(i);
                if(res != "" && hi.Count == 0)
                {
                    var order = carrier_Entities.Order_Item.Where(w => w.Docno == i).ToList();
                    order.FirstOrDefault().Status = "A";
                    order.FirstOrDefault().CodeResponse = 1;
                    var his = carrier_Entities.History_Notify_Order.Where(w => w.Docno == i).ToList();
                    var lastNolist = carrier_Entities.History_Notify_Order.ToList();
                    var lastNo = "";
                    if (lastNolist.Count == 0)
                    {
                        lastNo = "HIS00001";
                    }
                    else
                    {
                        lastNo = carrier_Entities.History_Notify_Order.OrderByDescending(o => o.History_ID).FirstOrDefault().History_NO;
                    }
                    var lenght = (Convert.ToInt32(lastNo.Substring(3, 5)) + 1).ToString().Length;
                    var newNo = lastNo.Substring(0, 8 - lenght) + (Convert.ToInt32(lastNo.Substring(3, 5)) + 1).ToString();
                    if (his.Count == 0)
                    {
                        carrier_Entities.History_Notify_Order.Add(new History_Notify_Order { Date_Notify = DateTime.Now, Docno = order.FirstOrDefault().Docno, pno = order.FirstOrDefault().pno, Type_Send_KA = order.FirstOrDefault().TypeSendKO, History_NO = newNo ,SaveFrom = "Update"});
                        carrier_Entities.SaveChanges();
                    }

                }
            }
            ClientScript.RegisterStartupScript(this.GetType(), "Success", "<script type='text/javascript'>alert('อัพเดท Order สำเร็จ');window.location='Default';</script>'");
        }

    }
    public class messageNotify
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}