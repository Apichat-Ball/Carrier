using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Service;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Budget;
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
        BudgetEntities budget_Entities = new BudgetEntities();
        public _Default()
        {
            service_Flashs = new Service_Flash();
            carrier_Entities = new CarrierEntities();
            insideSFG_WF_Entities = new InsideSFG_WFEntities();
        }
        protected void Page_Load(object sender, EventArgs e)
        {


            //สำหรับ Admin
            //Session.Clear();
            Session["_UserID"] = null;
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
                if (Session["TypeSend"] != null)
                {
                    ddlTypsend.SelectedValue = Session["TypeSend"].ToString();
                    if(Session["TypeSend"].ToString() == "2")
                    {
                        dv_Deliver.Visible = true;
                        //dv_price.Visible = true;
                    }
                }
                loadtable(1);
                
                loadComment();
                var userSession = Convert.ToInt32( Session["_UserID"].ToString());
                var user = carrier_Entities.Users.Where(w => w.UserID == userSession && w.ImportForSAP == true).FirstOrDefault();
                if(user != null)
                {
                    dv_Announce.Visible = true;
                }
            }
            
        }

       
        public void loadtable(int page)
        {
            
            var typsend = Convert.ToInt32(ddlTypsend.SelectedValue);
            var user = Convert.ToInt32(Session["_UserID"].ToString());
            var permission = carrier_Entities.Users.Where(w => w.UserID == user).FirstOrDefault();
            if (permission != null && permission.Permission == "Admin")
            {

                var bfid = "";
                if(txtDocnoSearch.Text != "" && txtDocnoSearch.Text.StartsWith("FL"))
                {
                    bfid = carrier_Entities.Order_Big_Box.Where(w => w.Docno.Contains(txtDocnoSearch.Text)).FirstOrDefault().BFID;
                }
                else
                {
                    bfid = txtDocnoSearch.Text;
                }

                dv_DO_Search.Visible = true;
                var maxrow = 10;
                var orderList = (from orderItem in carrier_Entities.Order_Item
                                 join bg in carrier_Entities.Order_Big_Box on orderItem.Docno equals bg.Docno
                                 join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                                 where ( bg.BFID.Contains(bfid) || bfid == "") && order.Transport_Type == typsend
                                 select new
                                 {

                                     Docno = orderItem.Docno,
                                     nameCreate = order.UserID,
                                     pno = orderItem.pno,
                                     srcName = order.srcName,
                                     dstName = order.dstName,
                                     ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                     dateCreate = order.Date_send,
                                     TrackingPickup = orderItem.ticketPickupId,
                                     TimeTracking = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.DateNotify).ToList().FirstOrDefault(),
                                     TimeTrackingText = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.TimeoutAtText).ToList().FirstOrDefault() ?? "",
                                     Brand = order.SDpart,
                                     status = orderItem.Status,
                                     Remark = order.remark,
                                     TypeSend = order.TypeSend,
                                     StaffInfoName = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.StaffInfoName).ToList().FirstOrDefault(),
                                     Transport_Type = order.Transport_Type,
                                     TypeSendKa = orderItem.TypeSendKO,
                                     DateSucces = orderItem.Date_Success,
                                     dstDetailAddress = order.dstDetailAddress
                                 }).ToList();
                var orderFOC = (from orderItem in carrier_Entities.Order_Item
                                join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                                where ( order.Docno.Contains(txtDocnoSearch.Text) || txtDocnoSearch.Text == "") && !order.Docno.StartsWith("FL") && order.Transport_Type == typsend
                                select new
                                {

                                    Docno = orderItem.Docno,
                                    nameCreate = order.UserID,
                                    pno = orderItem.pno,
                                    srcName = order.srcName,
                                    dstName = order.dstName,
                                    ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                    dateCreate = order.Date_send,
                                    TrackingPickup = orderItem.ticketPickupId,
                                    TimeTracking = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.DateNotify).ToList().FirstOrDefault(),
                                    TimeTrackingText = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.TimeoutAtText).ToList().FirstOrDefault() ?? "",
                                    Brand = order.SDpart,
                                    status = orderItem.Status,
                                    Remark = order.remark,
                                    TypeSend = order.TypeSend,
                                    StaffInfoName = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.StaffInfoName).ToList().FirstOrDefault(),
                                    Transport_Type = order.Transport_Type,
                                    TypeSendKa = orderItem.TypeSendKO,
                                    DateSucces = orderItem.Date_Success,
                                    dstDetailAddress = order.dstDetailAddress
                                }).ToList();
                orderList.AddRange(orderFOC);

                var format = "dd/MM/yyyy";
                var enUS = new CultureInfo("en-US");
                if (permission.TypeWarehouse != null)
                {
                    switch (permission.TypeWarehouse)
                    {
                        case "SFG":
                            var roxLalamove = orderList.Where(w => w.TypeSendKa == "ROX" && w.Transport_Type == 2).ToList();
                            var SDC1Lalamove = orderList.Where(w => w.TypeSendKa == "SDC1" && w.Transport_Type == 2).ToList();
                            orderList = orderList.Where(w => w.TypeSendKa != "SDC1" && w.TypeSendKa != "ROX").ToList();
                            orderList.AddRange(roxLalamove);
                            orderList.AddRange(SDC1Lalamove);
                            break;
                        case "SDC1":
                        case "ROX":
                            orderList = orderList.Where(w => w.TypeSendKa == permission.TypeWarehouse && w.Transport_Type != 2).ToList();
                            break;
                    }
                }
                if (txtDateStart.Text != "" && txtDateEnd.Text != "")
                {
                    var start = Convert.ToDateTime(txtDateStart.Text);
                    var end = Convert.ToDateTime(txtDateEnd.Text);
                    switch (ddlStatusOrder.SelectedValue)
                    {
                        case "1":
                            orderList = orderList.Where(w => w.status != "A" && w.status != "SP" && w.status != "SL" && w.status != "C").ToList();
                            if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "" || txtDOSearch.Text != "")
                            {
                                //orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                                orderList = orderList.Where(w => 
                                ((w.pno ?? "").Contains(txtPnoSearch.Text.ToUpper()) || txtPnoSearch.Text == "")
                                && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                                && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text)) 
                                : false  ) : (w.Remark.Contains(txtDOSearch.Text)) ) : false)
                                || txtDOSearch.Text == "")
                                ).ToList();

                            }
                            else if (lbStatusSearch.Text != "First")
                            {
                                orderList = orderList.Where(w => w.DateSucces >= start && w.DateSucces <= end).ToList();
                            }

                            break;
                        case "2":
                            orderList = orderList.Where(w => w.status != null && w.status != "C").ToList();
                            if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "" || txtDOSearch.Text != "")
                            {
                                //orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                                if (txtPnoSearch.Text != "")
                                {
                                    orderList = orderList.Where(w => w.pno != null).ToList();
                                    orderList = orderList.Where(w => ((w.pno ?? "").Contains(txtPnoSearch.Text.ToUpper()) || txtPnoSearch.Text == "")
                                    && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                    && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                                    && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text))
                                : false) : (w.Remark.Contains(txtDOSearch.Text))) : false)
                                || txtDOSearch.Text == "")
                                    ).ToList();
                                }
                                else
                                {
                                    orderList = orderList.Where(w =>  (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                    && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                                    && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text))
                                : false) : (w.Remark.Contains(txtDOSearch.Text))) : false)
                                || txtDOSearch.Text == "")
                                    ).ToList();
                                }


                            }
                            else
                            {
                                orderList = orderList.Where(w => w.DateSucces >= start && w.DateSucces <= end).ToList();
                            }
                            break;
                        case "3":
                            orderList = orderList.Where(w => w.status == "C").ToList();
                            if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "" || txtDOSearch.Text != "")
                            {
                                //orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                                if (txtPnoSearch.Text != "")
                                {
                                    orderList = orderList.Where(w => w.pno != null).ToList();
                                    orderList = orderList.Where(w => ((w.pno ?? "").Contains(txtPnoSearch.Text.ToUpper()) || txtPnoSearch.Text == "")
                                    && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                    && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                                    && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text))
                                : false) : (w.Remark.Contains(txtDOSearch.Text))) : false)
                                || txtDOSearch.Text == "")
                                    ).ToList();
                                }
                                else
                                {
                                    orderList = orderList.Where(w => 
                                    (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                    && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                                    && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text))
                                : false) : (w.Remark.Contains(txtDOSearch.Text))) : false)
                                || txtDOSearch.Text == "")
                                    ).ToList();
                                }


                            }
                            else
                            {
                                orderList = orderList.Where(w => w.DateSucces >= start && w.DateSucces <= end).ToList();
                            }
                            break;
                        case "0":
                            if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "" || txtDOSearch.Text != "")
                            {
                                orderList = orderList.Where(w =>
                                ((w.pno ?? "").Contains(txtPnoSearch.Text.ToUpper()) || txtPnoSearch.Text == "")
                                && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                                && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text))
                                : false) : (w.Remark.Contains(txtDOSearch.Text))) : false)
                                || txtDOSearch.Text == "")
                                ).ToList(); ;
                            }
                            else
                            {

                                orderList = orderList.Where(w => w.DateSucces >= start && w.DateSucces <= end).ToList();
                            }
                            break;
                    }
                    

                    #region 
                    var BigBox = carrier_Entities.Order_Big_Box.ToList();
                    if(ddlStatusOrder.SelectedValue != "3")
                    {
                        BigBox = BigBox.Where(w => w.Status == "A").ToList();
                    }
                    else
                    {
                        BigBox = BigBox.Where(w => w.Status == "C").ToList();
                    }
                    var DocnoGroup = BigBox.Select(s => s.Docno).ToList();
                    var orderListCloon = orderList.ToList();
                    var orderGroup = orderList.FindAll(f => DocnoGroup.Contains(f.Docno));
                    foreach(var gro in orderGroup)
                    {

                        orderList.Remove(gro);
                    }

                    if(new string[] { "1","2"}.Contains(ddlStatusOrder.SelectedValue) && txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "")
                    {
                        var DocnoHaveFromClone = orderListCloon.Select(s => s.Docno).ToList();
                        BigBox = BigBox.Where(w => DocnoHaveFromClone.Contains(w.Docno)).ToList();
                    }
                    var groupBox = BigBox.GroupBy(g => new { g.BFID }).Select(s=>new { s.Key.BFID, docno = s.Select(d=>d.Docno).FirstOrDefault()}).ToList();

                    foreach(var Box in groupBox)
                    {
                        var orderget = orderListCloon.Where(w => w.Docno == Box.docno).FirstOrDefault();
                        if(orderget != null)
                        {
                            var BigBoxAll = BigBox.Where(w => w.BFID == Box.BFID).ToList();
                            var Tracking = "";
                            
                            switch (orderget.TypeSendKa)
                            {
                                case "SFG":
                                    Tracking = "รายละเอียดอยู่ในเอกสาร";
                                    break;
                                case "SDC1":
                                    foreach (var b in BigBoxAll)
                                    {
                                        var orderOne = carrier_Entities.Orders.Where(w => w.Docno == b.Docno).FirstOrDefault();
                                        if (orderOne != null)
                                        {
                                            
                                            if (b != BigBoxAll.LastOrDefault())
                                            {
                                                Tracking += orderOne.remark + " ,\n";
                                            }
                                            else
                                            {
                                                Tracking += orderOne.remark ;
                                            }
                                        }
                                    }
                                    break;
                                case "ROX":
                                    Tracking = "รายละเอียดอยู่ในเอกสาร";
                                    break;
                                default: Tracking = "รายละเอียดอยู่ในเอกสาร";
                                    break;
                            }

                            var pno = "";
                            var inbox = carrier_Entities.Order_Big_Box.Where(w => w.BFID == Box.BFID).ToList();
                            if (inbox.Count() != 0)
                            {
                                foreach (var i in inbox)
                                {
                                    var pnos = carrier_Entities.Order_Item.Where(w => w.Docno == i.Docno).FirstOrDefault().pno;
                                    if (i == inbox.LastOrDefault())
                                    {
                                        pno += pnos;
                                    }
                                    else
                                    {
                                        pno += pnos + ",\n";
                                    }
                                }
                            }
                            orderList.Add(new
                            {
                                Docno = Box.BFID,
                                nameCreate = orderget.nameCreate,
                                pno = pno == "" ? orderget.pno : pno,
                                srcName = orderget.srcName,
                                dstName = orderget.dstName,
                                ArticleCategory = orderget.ArticleCategory,
                                dateCreate = orderget.dateCreate,
                                TrackingPickup = orderget.TrackingPickup,
                                TimeTracking = orderget.TimeTracking,
                                TimeTrackingText = orderget.TimeTrackingText,
                                Brand = orderget.Brand,
                                status = orderget.status,
                                Remark = Tracking,
                                TypeSend = orderget.TypeSend,
                                StaffInfoName = orderget.StaffInfoName,
                                Transport_Type = orderget.Transport_Type,
                                TypeSendKa = orderget.TypeSendKa,
                                DateSucces = orderget.DateSucces,
                                dstDetailAddress = orderget.dstDetailAddress
                            });
                        }
                        
                    }

                    #endregion

                    double maxdata_gvData = (double)((decimal)Convert.ToDecimal(orderList.Count()) / Convert.ToDecimal(maxrow));
                    int pageCount_gvData = (int)Math.Ceiling(maxdata_gvData);
                    gv_OrderAll.DataSource = orderList.OrderByDescending(x => x.DateSucces).Skip((page - 1) * maxrow).Take(maxrow);
                    gv_OrderAll.DataBind();
                    Page_gv(page, pageCount_gvData);

                    if(orderList.Count() != 0)
                    {
                        if (ddlStatusOrder.SelectedValue == "2" || ddlStatusOrder.SelectedValue == "3")
                        {
                            gv_OrderAll.HeaderRow.Cells[0].Visible = false;
                            gv_OrderAll.HeaderRow.Cells[13].Visible = false;
                        }
                        else
                        {
                            gv_OrderAll.HeaderRow.Cells[0].Visible = true;
                        }
                    }
                    
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
                        //ImageButton imgbtnEdit = (ImageButton)row.FindControl("imgbtnEdit");
                        Label lbTypeSend = (Label)row.FindControl("lbTypeSend");
                        Label lbTransport_Type = (Label)row.FindControl("lbTransport_Type");
                        lbDateCreate.Text = DateTime.Parse(lbDateCreate.Text).ToString("dd/MM/yyyy");
                        Label lbUserCreate = (Label)row.FindControl("lbUserCreate");
                        ImageButton imgbtnGet = (ImageButton)row.FindControl("imgbtnGet");
                        Label lbStaffinfoId = (Label)row.FindControl("lbStaffinfoId");



                        if (ddlStatusOrder.SelectedValue == "2"|| ddlStatusOrder.SelectedValue == "3")
                        {
                            row.Cells[0].Visible = false;
                            row.Cells[13].Visible = false;
                        }
                        else
                        {
                            row.Cells[0].Visible = true;
                        }

                        var userid = Convert.ToInt32(lbUserCreate.Text);
                        var emp = insideSFG_WF_Entities.Employees.Where(w => w.userID == userid).FirstOrDefault();
                        lbUserCreate.Text = emp.name + " " + emp.surname + "(" + emp.nick + ")";
                        if (lbTransport_Type.Text == "1")
                        {
                            lbTransport_Type.Text = "FlashExpress";
                        }
                        else if (lbTransport_Type.Text == "2")
                        {
                            lbTransport_Type.Text = "Lalamove";
                            var calcar = carrier_Entities.Calculate_Car.Where(w => w.BFID == lkbDocno.Text).FirstOrDefault();
                            if(calcar != null)
                            {
                                lbStaffinfoId.Text = calcar.DeliveryNumber;
                            }
                        }
                        var checkBigBox = carrier_Entities.Order_Big_Box.Where(w => w.BFID == lkbDocno.Text).ToList();
                        if(checkBigBox.Count() == 0)
                        {
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
                                    else
                                    {
                                        if (DateTime.Now.ToShortDateString() == dateToUpdate.ToShortDateString())
                                        {
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
                        }
                        else
                        {
                            List<Model_Chack_Noti> resCheck = new List<Model_Chack_Noti>();
                            foreach(var BoxID in checkBigBox)
                            {
                                if (lbStatus.Text != "")
                                {
                                    var dateNotiDate = new DateTime();
                                    if (lbTimeTrackingText.Text.Contains("พรุ่งนี้"))
                                    {
                                        var dateRaw = DateTime.Parse(lbTimeTracking.Text).AddDays(1);
                                        dateNotiDate = DateTime.Parse(dateRaw.ToShortDateString());
                                        var dateToUpdate = dateNotiDate.AddHours(17).AddMinutes(30);
                                        if (DateTime.Now >= dateToUpdate)
                                        {
                                            var date = DateTime.Parse(lbTimeTracking.Text);
                                            var a = service_Flashs.CheckNotifyBigBox(BoxID.Docno);
                                            if (a.message != "")
                                            {
                                                resCheck.Add(a);
                                            }
                                            else
                                            {
                                                resCheck.Add(new Model_Chack_Noti { code = "0", message = "ยังไม่ได้มารับของ" });
                                            }
                                        }
                                        else
                                        {
                                            if (DateTime.Now.ToShortDateString() == dateToUpdate.ToShortDateString())
                                            {
                                                resCheck.Add(new Model_Chack_Noti { code = "0" , message = "วันนี้" + lbTimeTrackingText.Text.Substring(8) });
                                            }
                                            else
                                            {
                                                resCheck.Add(new Model_Chack_Noti { code = "0", message = lbTimeTrackingText.Text });
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
                                            var a = service_Flashs.CheckNotifyBigBox(BoxID.Docno);
                                            if (a.message != "")
                                            {
                                                resCheck.Add(a);
                                            }
                                            else
                                            {
                                                resCheck.Add(new Model_Chack_Noti { code = "0", message = "ยังไม่ได้มารับของ" });

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
                                        var a = service_Flashs.CheckNotifyBigBox(BoxID.Docno);
                                        if (a.message != "")
                                        {
                                            resCheck.Add(a);
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
                                    else
                                    {
                                        /*var a = service_Flashs.CheckNotify(lkbDocno.Text);
                                        if (a == "")
                                        {
                                            imgbtnEdit.Visible = true;
                                        }*/
                                    }
                                }
                            }

                            if(lbStatus.Text != "" && resCheck.Count() != 0)
                            {
                                var resday = resCheck.Where(w => w.code == "0" && w.message.StartsWith("วันนี้")).FirstOrDefault();
                                var resmessage = resCheck.Where(w => w.code == "0" && w.message == "ยังไม่ได้มารับของ").FirstOrDefault();
                                if (resday != null)
                                {
                                    lbTimeTrackingText.Text = resday.message;
                                }
                                else if (resmessage != null)
                                {
                                    lbTimeTrackingText.Text = resmessage.message;
                                }
                                else
                                {
                                    var statusNoti = resCheck.Where(w => w.code != "0").GroupBy(g => g.code).Select(s => new { code = s.Key, message = s.Select(si => si.message).FirstOrDefault() }).ToList();
                                    if (statusNoti.Count() > 1)
                                    {
                                        if (statusNoti.Where(w => w.code == "5").FirstOrDefault() != null)
                                        {
                                            lbTimeTrackingText.Text = "บางรายการเซ็นรับแล้ว";
                                        }
                                        else if (statusNoti.Where(w => w.code != "0").FirstOrDefault() != null)
                                        {
                                            lbTimeTrackingText.Text = "อยู่ในขั้นตอนการขนส่ง";
                                        }
                                    }
                                    else
                                    {
                                        lbTimeTrackingText.Text = resCheck.FirstOrDefault().message;
                                    }
                                }
                            }
                            else if(resCheck.Count() != 0)
                            {
                                var statusNoti = resCheck.Where(w => w.code != "0").GroupBy(g => g.code).Select(s => new { code = s.Key, message = s.Select(si => si.message).FirstOrDefault() }).ToList();
                                if (statusNoti.Count() > 1)
                                {
                                    if (statusNoti.Where(w => w.code == "5").FirstOrDefault() != null)
                                    {
                                        lbTimeTrackingText.Text = "บางรายการเซ็นรับแล้ว";
                                    }
                                    else if (statusNoti.Where(w => w.code != "0").FirstOrDefault() != null)
                                    {
                                        lbTimeTrackingText.Text = "อยู่ในขั้นตอนการขนส่ง";
                                    }
                                }
                                else
                                {
                                    lbTimeTrackingText.Text = resCheck.FirstOrDefault().message;
                                }
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
                        if (lbStatusItem.Text == "C")
                        {
                            cbItem.Visible = false;
                            imgbtnCancelOrder.Visible = false;
                            gv_OrderAll.Columns[0].Visible = false;
                            gv_OrderAll.Columns[11].Visible = false;
                            gv_OrderAll.Columns[12].Visible = false;
                            gv_OrderAll.Columns[13].Visible = false;

                        }
                        if (lbTypeSend.Text == "2" && lbStatusItem.Text == "")
                        {
                            cbItem.Visible = false;
                            imgbtnCancelOrder.Visible = false;
                            imgbtnGet.Visible = true;
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
                    div_Page_Bar.Visible = true;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเลือกวันที่เริ่มและสิ้นสุดการค้นหา')", true);
                }

            }
            else if (permission == null || permission.Permission == null)
            {
                Response.Redirect("Home_Carrier.aspx");
            }
            else if(permission.Permission == "ACC")
            {
                Response.Redirect("Home_Carrier.aspx");
            }
        }
        public void loadComment()
        {
            var datenowOld = DateTime.Now.AddDays(-7);
            var cm = carrier_Entities.Comment_System.Where(w=>w.CM_DateCreate > datenowOld).OrderByDescending(o => o.CM_DateCreate).ToList();
            if (cm.Count == 0)
            {
                dv_Comment.Visible = false;
            }
            else
            {
                dv_Comment.Visible = true;
            }

            gv_UpdateComment.DataSource = cm;
            gv_UpdateComment.DataBind();
            foreach (GridViewRow row in gv_UpdateComment.Rows)
            {
                Label lbStatusComment = (Label)row.FindControl("lbStatusComment");

                if (lbStatusComment.Text == "1")
                {
                    lbStatusComment.Text = "อัพเดต";
                    lbStatusComment.BackColor = System.Drawing.Color.LimeGreen;
                    lbStatusComment.ForeColor = System.Drawing.Color.White;
                }
                else if (lbStatusComment.Text == "2")
                {
                    lbStatusComment.Text = "ประกาศ";
                    lbStatusComment.BackColor = System.Drawing.Color.Red;
                    lbStatusComment.ForeColor = System.Drawing.Color.White;
                }
                Label lbDateCreate = (Label)row.FindControl("lbDateCreate");
                lbDateCreate.Text = Convert.ToDateTime(lbDateCreate.Text).ToString("dd/MM/yyyy");
                var datenow = DateTime.Now.ToString("dd/MM/yyyy");
                Label lbNew = (Label)row.FindControl("lbNew");
                if (lbDateCreate.Text == datenow)
                {
                    lbNew.Visible = true;
                }
                else
                {
                    lbNew.Visible = false;
                }

                Label lbCommerntID = (Label)row.FindControl("lbCommerntID");
                ImageButton imgGuie = (ImageButton)row.FindControl("imgGuie");
                Label lbUrlFile = (Label)row.FindControl("lbUrlFile");
                if(lbUrlFile.Text != "")
                {
                    imgGuie.Visible = true;
                    String originalPath = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
                    string filePath = originalPath.Substring(0, originalPath.LastIndexOf("/Default")) + "/FilePatchUpdate/" + lbUrlFile.Text;
                    string dataDir = HttpContext.Current.Server.MapPath("FilePatchUpdate/") + lbUrlFile.Text;
                    if (File.Exists(dataDir))
                    {
                        imgGuie.OnClientClick = "window.open('" + filePath + "', '_blank', '')";

                    }
                    else
                    {
                        imgGuie.Visible = false;
                    }
                }
               

            }
        }
        public void LoadAllTableCheckAll()
        {
            var user = Convert.ToInt32(Session["_UserID"].ToString());
            var permission = carrier_Entities.Users.Where(w => w.UserID == user).FirstOrDefault();
            dv_DO_Search.Visible = true;
            var orderList = (from orderItem in carrier_Entities.Order_Item
                             join bg in carrier_Entities.Order_Big_Box on orderItem.Docno equals bg.Docno
                             join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                             where (bg.BFID.Contains(txtDocnoSearch.Text) || txtDocnoSearch.Text == "") && orderItem.Status == null
                             select new
                             {

                                 Docno = orderItem.Docno,
                                 nameCreate = order.UserID,
                                 pno = orderItem.pno,
                                 srcName = order.srcName,
                                 dstName = order.dstName,
                                 dstDetailAddress = order.dstDetailAddress,
                                 ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                 dateCreate = orderItem.Date_Success ?? order.Date_send,
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
            var orderFOC = (from orderItem in carrier_Entities.Order_Item
                            join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                            where (order.Docno.Contains(txtDocnoSearch.Text) || txtDocnoSearch.Text == "") && !order.Docno.StartsWith("FL") && orderItem.Status == null
                            select new
                            {

                                Docno = orderItem.Docno,
                                nameCreate = order.UserID,
                                pno = orderItem.pno,
                                srcName = order.srcName,
                                dstName = order.dstName,
                                dstDetailAddress = order.dstDetailAddress,
                                ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                dateCreate = orderItem.Date_Success ?? order.Date_send,
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
            orderList.AddRange(orderFOC);

            var format = "dd/MM/yyyy";
            var enUS = new CultureInfo("en-US");
            if (permission.TypeWarehouse != null)
            {
                switch (permission.TypeWarehouse)
                {
                    case "SFG":
                        var roxLalamove = orderList.Where(w => w.TypeSendKa == "ROX" && w.Transport_Type == 2).ToList();
                        var SDC1Lalamove = orderList.Where(w => w.TypeSendKa == "SDC1" && w.Transport_Type == 2).ToList();
                        orderList = orderList.Where(w => w.TypeSendKa != "SDC1" && w.TypeSendKa != "ROX").ToList();
                        orderList.AddRange(roxLalamove);
                        orderList.AddRange(SDC1Lalamove);
                        break;
                    case "SDC1":
                    case "ROX":
                        orderList = orderList.Where(w => w.TypeSendKa == permission.TypeWarehouse && w.Transport_Type != 2).ToList();
                        break;
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
                        if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "" || txtDOSearch.Text != "")
                        {
                            //orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                            orderList = orderList.Where(w =>
                            ((w.pno ?? "").Contains(txtPnoSearch.Text.ToUpper()) || txtPnoSearch.Text == "")
                            && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                            && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                            && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text))
                            : false) : (w.Remark.Contains(txtDOSearch.Text))) : false)
                            || txtDOSearch.Text == "")
                            ).ToList();

                        }
                        else if (lbStatusSearch.Text != "First")
                        {
                            orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                        }

                        break;
                    case "2":
                        orderList = orderList.Where(w => w.status != null && w.status != "C").ToList();
                        if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "" || txtDOSearch.Text != "")
                        {
                            //orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                            if (txtPnoSearch.Text != "")
                            {
                                orderList = orderList.Where(w => w.pno != null).ToList();
                                orderList = orderList.Where(w => ((w.pno ?? "").Contains(txtPnoSearch.Text.ToUpper()) || txtPnoSearch.Text == "")
                                && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                                && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text))
                            : false) : (w.Remark.Contains(txtDOSearch.Text))) : false)
                            || txtDOSearch.Text == "")
                                ).ToList();
                            }
                            else
                            {
                                orderList = orderList.Where(w => (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                                && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text))
                            : false) : (w.Remark.Contains(txtDOSearch.Text))) : false)
                            || txtDOSearch.Text == "")
                                ).ToList();
                            }


                        }
                        else
                        {
                            orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                        }
                        break;
                    case "3":
                        orderList = orderList.Where(w => w.status == "C").ToList();
                        if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "" || txtDOSearch.Text != "")
                        {
                            //orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                            if (txtPnoSearch.Text != "")
                            {
                                orderList = orderList.Where(w => w.pno != null).ToList();
                                orderList = orderList.Where(w => ((w.pno ?? "").Contains(txtPnoSearch.Text.ToUpper()) || txtPnoSearch.Text == "")
                                && (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                                && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text))
                            : false) : (w.Remark.Contains(txtDOSearch.Text))) : false)
                            || txtDOSearch.Text == "")
                                ).ToList();
                            }
                            else
                            {
                                orderList = orderList.Where(w =>
                                (w.dstName.Contains(txtDstNameSearch.Text) || txtDstNameSearch.Text == "")
                                && (w.ArticleCategory.Contains(txtArticleSearch.Text) || txtArticleSearch.Text == "")
                                && ((w.Remark != "" && w.Remark != null ? (w.Remark.Contains(':') ? (w.Remark.Split(':')[0].EndsWith("DO") ? (w.Remark.Split(':')[1].Contains(",") ? w.Remark.Split(':')[1].Split(',').ToList().Contains(txtDOSearch.Text) : w.Remark.Split(':')[1].Contains(txtDOSearch.Text))
                            : false) : (w.Remark.Contains(txtDOSearch.Text))) : false)
                            || txtDOSearch.Text == "")
                                ).ToList();
                            }


                        }
                        else
                        {
                            orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                        }
                        break;
                    case "0":
                        orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                        break;
                }


                #region 
                var BigBox = carrier_Entities.Order_Big_Box.ToList();
                if (ddlStatusOrder.SelectedValue != "3")
                {
                    BigBox = BigBox.Where(w => w.Status == "A").ToList();
                }
                else
                {
                    BigBox = BigBox.Where(w => w.Status == "C").ToList();
                }
                var DocnoGroup = BigBox.Select(s => s.Docno).ToList();
                var orderListCloon = orderList.ToList();
                var orderGroup = orderList.FindAll(f => DocnoGroup.Contains(f.Docno));
                foreach (var gro in orderGroup)
                {

                    orderList.Remove(gro);
                }

                if (new string[] { "1", "2" }.Contains(ddlStatusOrder.SelectedValue) && txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "")
                {
                    var DocnoHaveFromClone = orderListCloon.Select(s => s.Docno).ToList();
                    BigBox = BigBox.Where(w => DocnoHaveFromClone.Contains(w.Docno)).ToList();
                }
                var groupBox = BigBox.GroupBy(g => new { g.BFID }).Select(s => new { s.Key.BFID, docno = s.Select(d => d.Docno).FirstOrDefault() }).ToList();

                foreach (var Box in groupBox)
                {
                    var orderget = orderListCloon.Where(w => w.Docno == Box.docno).FirstOrDefault();
                    if (orderget != null)
                    {
                        var BigBoxAll = BigBox.Where(w => w.BFID == Box.BFID).ToList();
                        var Tracking = "";

                        switch (orderget.TypeSendKa)
                        {
                            case "SFG":
                                Tracking = "รายละเอียดอยู่ในเอกสาร";
                                break;
                            case "SDC1":
                                foreach (var b in BigBoxAll)
                                {
                                    var orderOne = carrier_Entities.Orders.Where(w => w.Docno == b.Docno).FirstOrDefault();
                                    if (orderOne != null)
                                    {

                                        if (b != BigBoxAll.LastOrDefault())
                                        {
                                            Tracking += orderOne.remark + " ,\n";
                                        }
                                        else
                                        {
                                            Tracking += orderOne.remark;
                                        }
                                    }
                                }
                                break;
                            case "ROX":
                                Tracking = "รายละเอียดอยู่ในเอกสาร";
                                break;
                            default:
                                Tracking = "รายละเอียดอยู่ในเอกสาร";
                                break;
                        }

                        var pno = "";
                        var inbox = carrier_Entities.Order_Big_Box.Where(w => w.BFID == Box.BFID).ToList();
                        if (inbox.Count() != 0)
                        {
                            foreach(var i in inbox)
                            {
                                var pnos = carrier_Entities.Order_Item.Where(w => w.Docno == i.Docno).FirstOrDefault().pno;
                                if(i == inbox.LastOrDefault())
                                {
                                    pno += pnos;
                                }
                                else
                                {
                                    pno += pnos + ",\n";
                                }
                            }
                        }
                        orderList.Add(new
                        {
                            Docno = Box.BFID,
                            nameCreate = orderget.nameCreate,
                            pno = pno == "" ?  orderget.pno : pno,
                            srcName = orderget.srcName,
                            dstName = orderget.dstName,
                            dstDetailAddress = orderget.dstDetailAddress,
                            ArticleCategory = orderget.ArticleCategory,
                            dateCreate = orderget.dateCreate,
                            TrackingPickup = orderget.TrackingPickup,
                            TimeTracking = orderget.TimeTracking,
                            TimeTrackingText = orderget.TimeTrackingText,
                            Brand = orderget.Brand,
                            status = orderget.status,
                            Remark = Tracking,
                            TypeSend = orderget.TypeSend,
                            StaffInfoName = orderget.StaffInfoName,
                            Transport_Type = orderget.Transport_Type,
                            TypeSendKa = orderget.TypeSendKa
                        });
                    }

                }

                #endregion

                
                gv_OrderAll.DataSource = orderList.OrderByDescending(x => x.dateCreate);
                gv_OrderAll.DataBind();

                if (orderList.Count() != 0)
                {
                    if (ddlStatusOrder.SelectedValue == "2" || ddlStatusOrder.SelectedValue == "3")
                    {
                        gv_OrderAll.HeaderRow.Cells[0].Visible = false;
                        gv_OrderAll.HeaderRow.Cells[12].Visible = false;
                    }
                    else
                    {
                        gv_OrderAll.HeaderRow.Cells[0].Visible = true;
                    }
                }

                foreach (GridViewRow row in gv_OrderAll.Rows)
                {
                    LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");
                    CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                    Label lbDateCreateS = (Label)row.FindControl("lbDateCreate");
                    Label lbStatus = (Label)row.FindControl("lbStatus");
                    Label lbTimeTrackingText = (Label)row.FindControl("lbTimeTrackingText");
                    Label lbTimeTracking = (Label)row.FindControl("lbTimeTracking");
                    Label lbStatusItem = (Label)row.FindControl("lbStatusItem");
                    ImageButton imgbtnCancelOrder = (ImageButton)row.FindControl("imgbtnCancelOrder");
                    //ImageButton imgbtnEdit = (ImageButton)row.FindControl("imgbtnEdit");
                    Label lbTypeSend = (Label)row.FindControl("lbTypeSend");
                    Label lbTransport_Type = (Label)row.FindControl("lbTransport_Type");
                    lbDateCreateS.Text = DateTime.Parse(lbDateCreateS.Text).ToString("dd/MM/yyyy");
                    Label lbUserCreate = (Label)row.FindControl("lbUserCreate");
                    ImageButton imgbtnGet = (ImageButton)row.FindControl("imgbtnGet");

                    if (ddlStatusOrder.SelectedValue == "2" || ddlStatusOrder.SelectedValue == "3")
                    {
                        row.Cells[0].Visible = false;
                        row.Cells[12].Visible = false;
                    }
                    else
                    {
                        row.Cells[0].Visible = true;
                    }

                    var userid = Convert.ToInt32(lbUserCreate.Text);
                    var emp = insideSFG_WF_Entities.Employees.Where(w => w.userID == userid).FirstOrDefault();
                    lbUserCreate.Text = emp.name + " " + emp.surname + "(" + emp.nick + ")";
                    if (lbTransport_Type.Text == "1")
                    {
                        lbTransport_Type.Text = "FlashExpress";
                    }
                    else if (lbTransport_Type.Text == "2")
                    {
                        lbTransport_Type.Text = "Lalamove";
                    }
                    var checkBigBox = carrier_Entities.Order_Big_Box.Where(w => w.BFID == lkbDocno.Text).ToList();
                    if (checkBigBox.Count() == 0)
                    {
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
                                else
                                {
                                    if (DateTime.Now.ToShortDateString() == dateToUpdate.ToShortDateString())
                                    {
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
                    }
                    else
                    {
                        List<Model_Chack_Noti> resCheck = new List<Model_Chack_Noti>();
                        foreach (var BoxID in checkBigBox)
                        {
                            if (lbStatus.Text != "")
                            {
                                var dateNotiDate = new DateTime();
                                if (lbTimeTrackingText.Text.Contains("พรุ่งนี้"))
                                {
                                    var dateRaw = DateTime.Parse(lbTimeTracking.Text).AddDays(1);
                                    dateNotiDate = DateTime.Parse(dateRaw.ToShortDateString());
                                    var dateToUpdate = dateNotiDate.AddHours(17).AddMinutes(30);
                                    if (DateTime.Now >= dateToUpdate)
                                    {
                                        var date = DateTime.Parse(lbTimeTracking.Text);
                                        var a = service_Flashs.CheckNotifyBigBox(BoxID.Docno);
                                        if (a.message != "")
                                        {
                                            resCheck.Add(a);
                                        }
                                        else
                                        {
                                            resCheck.Add(new Model_Chack_Noti { code = "0", message = "ยังไม่ได้มารับของ" });
                                        }
                                    }
                                    else
                                    {
                                        if (DateTime.Now.ToShortDateString() == dateToUpdate.ToShortDateString())
                                        {
                                            resCheck.Add(new Model_Chack_Noti { code = "0", message = "วันนี้" + lbTimeTrackingText.Text.Substring(8) });
                                        }
                                        else
                                        {
                                            resCheck.Add(new Model_Chack_Noti { code = "0", message = lbTimeTrackingText.Text });
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
                                        var a = service_Flashs.CheckNotifyBigBox(BoxID.Docno);
                                        if (a.message != "")
                                        {
                                            resCheck.Add(a);
                                        }
                                        else
                                        {
                                            resCheck.Add(new Model_Chack_Noti { code = "0", message = "ยังไม่ได้มารับของ" });

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
                                    var a = service_Flashs.CheckNotifyBigBox(BoxID.Docno);
                                    if (a.message != "")
                                    {
                                        resCheck.Add(a);
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
                                else
                                {
                                    /*var a = service_Flashs.CheckNotify(lkbDocno.Text);
                                    if (a == "")
                                    {
                                        imgbtnEdit.Visible = true;
                                    }*/
                                }
                            }
                        }

                        if (lbStatus.Text != "" && resCheck.Count() != 0)
                        {
                            var resday = resCheck.Where(w => w.code == "0" && w.message.StartsWith("วันนี้")).FirstOrDefault();
                            var resmessage = resCheck.Where(w => w.code == "0" && w.message == "ยังไม่ได้มารับของ").FirstOrDefault();
                            if (resday != null)
                            {
                                lbTimeTrackingText.Text = resday.message;
                            }
                            else if (resmessage != null)
                            {
                                lbTimeTrackingText.Text = resmessage.message;
                            }
                            else
                            {
                                var statusNoti = resCheck.Where(w => w.code != "0").GroupBy(g => g.code).Select(s => new { code = s.Key, message = s.Select(si => si.message).FirstOrDefault() }).ToList();
                                if (statusNoti.Count() > 1)
                                {
                                    if (statusNoti.Where(w => w.code == "5").FirstOrDefault() != null)
                                    {
                                        lbTimeTrackingText.Text = "บางรายการเซ็นรับแล้ว";
                                    }
                                    else if (statusNoti.Where(w => w.code != "0").FirstOrDefault() != null)
                                    {
                                        lbTimeTrackingText.Text = "อยู่ในขั้นตอนการขนส่ง";
                                    }
                                }
                                else
                                {
                                    lbTimeTrackingText.Text = resCheck.FirstOrDefault().message;
                                }
                            }
                        }
                        else if (resCheck.Count() != 0)
                        {
                            var statusNoti = resCheck.Where(w => w.code != "0").GroupBy(g => g.code).Select(s => new { code = s.Key, message = s.Select(si => si.message).FirstOrDefault() }).ToList();
                            if (statusNoti.Count() > 1)
                            {
                                if (statusNoti.Where(w => w.code == "5").FirstOrDefault() != null)
                                {
                                    lbTimeTrackingText.Text = "บางรายการเซ็นรับแล้ว";
                                }
                                else if (statusNoti.Where(w => w.code != "0").FirstOrDefault() != null)
                                {
                                    lbTimeTrackingText.Text = "อยู่ในขั้นตอนการขนส่ง";
                                }
                            }
                            else
                            {
                                lbTimeTrackingText.Text = resCheck.FirstOrDefault().message;
                            }
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
                    if (lbStatusItem.Text == "C")
                    {
                        cbItem.Visible = false;
                        imgbtnCancelOrder.Visible = false;
                        gv_OrderAll.Columns[0].Visible = false;
                        gv_OrderAll.Columns[10].Visible = false;
                        gv_OrderAll.Columns[11].Visible = false;
                        gv_OrderAll.Columns[12].Visible = false;

                    }
                    if (lbTypeSend.Text == "2" && lbStatusItem.Text == "")
                    {
                        cbItem.Visible = false;
                        imgbtnCancelOrder.Visible = false;
                        imgbtnGet.Visible = true;
                    }
                    else
                    {
                        cbItem.Checked = true;
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
                //foreach (GridViewRow row in gv_OrderAll.Rows)
                //{
                //    CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                //    if (cbItem.Visible == true)
                //    {
                //        cbItem.Checked = true;
                //    }
                //}
                //cbAll.Checked = true;
                CheckBox cbAlls = (CheckBox)gv_OrderAll.HeaderRow.FindControl("cbAll");
                cbAlls.Checked = true;
            }
            else
            {
                //foreach (GridViewRow row in gv_OrderAll.Rows)
                //{
                //    CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                //    cbItem.Checked = false;
                //}
                loadtable(1);
            }
        }

        protected void imgbtnCancelOrder_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnCancelOrder = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnCancelOrder.NamingContainer;
            Label lbpno = (Label)row.FindControl("lbpno");
            LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");

            var loadFL = carrier_Entities.Order_Big_Box.Where(w => w.BFID == lkbDocno.Text).ToList();
            var res = "";
            if(loadFL.Count() != 0)
            {
                List<string> resall = new List<string>();
                foreach(var i in loadFL)
                {
                    var orItem = carrier_Entities.Order_Item.Where(w => w.Docno == i.Docno).FirstOrDefault();
                    var ord = carrier_Entities.Orders.Where(w => w.Docno == i.Docno).FirstOrDefault();
                    if(ord.Transport_Type == 2)
                    {
                        i.Status = "C";
                        orItem.Status = "C";
                        carrier_Entities.SaveChanges();
                        res = "Cancel Order Success.";
                    }
                    else
                    {
                        res = service_Flashs.CancelOrder(i.Docno, orItem.pno);
                        resall.Add(res);
                        i.Status = "C";
                        carrier_Entities.SaveChanges();
                    }
                    
                }

            }
            else
            {
                res = service_Flashs.CancelOrder(lkbDocno.Text, lbpno.Text);
            }

            
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('succes : " + res + "')", true);

            
            loadtable(1);
        }
        protected void lkbDocno_Click(object sender, EventArgs e)
        {
            LinkButton lkbDocno = (LinkButton)sender;
            var lbDocnoss = lkbDocno.Text;
            Random num = new Random();
            Response.Redirect("Transport_Form.aspx?Docno=" + lbDocnoss + "&PM="+num.Next(10));
            
        }
        protected void selectPage(object sender, CommandEventArgs e)
        {
            loadtable(Convert.ToInt32(e.CommandArgument));
        }

        protected void btnNotifications_Click(object sender, ImageClickEventArgs e)
        {
            List<string> tracking = new List<string>();
            var DocBGSL = "";
            foreach (GridViewRow row in gv_OrderAll.Rows)
            {
                LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");
                CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                Label lbpno = (Label)row.FindControl("lbpno");
                if (cbItem.Checked)
                {
                    var inbox = carrier_Entities.Order_Big_Box.Where(w => w.BFID == lkbDocno.Text).ToList();
                    if(inbox.Count() != 0)
                    {
                        foreach (var b in inbox)
                        {
                            var tran = carrier_Entities.Orders.Where(w => w.Docno == b.Docno).FirstOrDefault();
                            if (tran.Transport_Type == 2)
                            {
                                //dv_Lalamove.Visible = true;
                                //dv_Main.Style.Add("filter", "blur(50px)");
                                //dv_Main.Style.Add("pointer-events", "none");
                                btnConfirmDelivery_Click(this, EventArgs.Empty);
                                return;
                                //var orderBig = carrier_Entities.Order_Big_Box.Where(w => w.Docno == tran.Docno).FirstOrDefault().BFID;
                                //DocBGSL += orderBig + ",";
                                //var slItemslItem = carrier_Entities.Order_Item.Where(w => w.Docno == b.Docno).First();
                                //slItemslItem.Status = "SL";
                                //slItemslItem.Date_Success = DateTime.Now;
                                //carrier_Entities.SaveChanges();
                            }
                            else
                            {
                                var pno = carrier_Entities.Order_Item.Where(w => w.Docno == b.Docno).FirstOrDefault().pno;
                                tracking.Add(pno);
                            }
                            
                        }
                    }
                    else
                    {
                        tracking.Add(lbpno.Text);
                    }
                    
                }
            }
            if (tracking.Count == 0 && DocBGSL == "")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่ได้เลือก Order ที่ต้องการเรียกรถมารับพัสดุ')", true);
                return;
            }
            if(DocBGSL != "")
            {
                DocBGSL = "รายการ " + DocBGSL + " Lalamove มารับเรียบร้อยแล้วครับ";
            }

            if(tracking.Count != 0)
            {

                var ordNotPrint = (from o in carrier_Entities.Orders
                           join i in carrier_Entities.Order_Item on o.Docno equals i.Docno
                           where tracking.Contains(i.pno) && o.status != "AP"
                           select new { Docno = o.Docno }).ToList();
                if(ordNotPrint.Count() == 0)
                {
                    var responseNotifyList = service_Flashs.Notify(tracking);
                    List<messageNotify> messageNoti = new List<messageNotify>();
                    DateTime dateSuccess = DateTime.Now;
                    List<History_Notify_Order> his = new List<History_Notify_Order>();


                    #region V2
                    var lastNolist = carrier_Entities.History_Notify_Order.ToList();
                    var lastNo = "";
                    var checkNO = lastNolist.OrderByDescending(o => o.History_ID).FirstOrDefault().History_NO;
                    if (checkNO.Length == 8)
                    {
                        lastNo = "HIS" + DateTime.Now.Year.ToString().Substring(2, 2) + "00000";
                    }
                    else if (checkNO.Substring(3, 2) != DateTime.Now.Year.ToString().Substring(2, 2))
                    {
                        lastNo = "HIS" + DateTime.Now.Year.ToString().Substring(2, 2) + "00000";
                    }
                    else
                    {
                        lastNo = checkNO;
                    }
                    var lenght = (Convert.ToInt32(lastNo.Substring(5, 5)) + 1).ToString().Length;
                    var newNo = lastNo.Substring(0, 10 - lenght) + (Convert.ToInt32(lastNo.Substring(5, 5)) + 1).ToString();
                    #endregion
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
                                orderItem.Date_Success = dateSuccess;
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
                                        orderItem.Date_Success = dateSuccess;
                                        //orderItem.ticketPickupId = ;
                                        carrier_Entities.SaveChanges();
                                    }
                                    else
                                    {
                                        orderItem.Status = "A";
                                        orderItem.CodeResponse = 1;
                                        orderItem.ticketPickupId = notiOld.Select(s => s.TicketPickupId).FirstOrDefault();
                                        orderItem.Date_Success = dateSuccess;
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
                                    if (hisNoti == null)
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
                    if (his.Count != 0)
                    {
                        carrier_Entities.History_Notify_Order.AddRange(his);
                        carrier_Entities.SaveChanges();
                    }

                    var messageAlert = "";
                    foreach (var listmess in messageNoti)
                    {
                        messageAlert = listmess.message + "\n";
                    }

                    Page myPage = (Page)HttpContext.Current.Handler;
                    ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "<script type='text/javascript'>alert('"+ messageAlert + DocBGSL + "');window.location='Default';</script>'");
                    div_Page_Bar.Visible = true;
                    //Response.Redirect("Default.aspx");
                }
                else
                {

                    var doc = "";
                    foreach(var i in ordNotPrint)
                    {
                        var BigBox = carrier_Entities.Order_Big_Box.Where(w => w.Docno == i.Docno).FirstOrDefault().BFID;
                        if (!doc.Contains(BigBox))
                        {
                            doc += BigBox + ";";
                        }
                        
                    }
                    Page myPage = (Page)HttpContext.Current.Handler;
                    ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "<script type='text/javascript'>alert('เอกสารที่ยังไม่ได้มีการปะหน้ากล่อง : " +  doc + " " + DocBGSL +  "');window.location='Default';</script>'");
                    div_Page_Bar.Visible = true;
                    //Response.Redirect("Default.aspx");
                }
                
            }
            else
            {
                Page myPage = (Page)HttpContext.Current.Handler;
                ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "<script type='text/javascript'>alert('" + DocBGSL + "');window.location='Default';</script>'");
                div_Page_Bar.Visible = true;
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
            txtDOSearch.Text = "";
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
                var lalamove = carrier_Entities.Order_Item.Where(w => w.Date_Success >= dateOld && w.Date_Success <= dateNew && w.Status == "SL").ToList();
                if(lalamove != null)
                {
                    foreach(var s in lalamove)
                    {
                        his.Add(new History_Notify_Order
                        {
                            History_NO = s.Docno,
                            Date_Notify = s.Date_Success,
                            pno = s.pno,
                            Docno = s.Docno,
                            Type_Send_KA = s.TypeSendKO
                        });
                    }
                    
                }
            }
            else if(permission.TypeWarehouse == "SDC1")
            {
                his = carrier_Entities.History_Notify_Order.Where(w => w.SaveFrom == null && w.Date_Notify >= dateOld && w.Date_Notify <= dateNew && w.Type_Send_KA == permission.TypeWarehouse).ToList();
            }
            else if(permission.TypeWarehouse == "ROX")
            {
                his = carrier_Entities.History_Notify_Order.Where(w => w.SaveFrom == null && w.Date_Notify >= dateOld && w.Date_Notify <= dateNew && w.Type_Send_KA == permission.TypeWarehouse).ToList();
            }
            else
            {
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

            var typsend = Convert.ToInt32(ddlTypsend.SelectedValue);
            if(typsend == 2 && new string[] { "1","0"}.Contains(ddlStatusOrder.SelectedValue))
            {
                dv_Deliver.Visible = true;
                //dv_price.Visible = true;
            }
            else
            {
                dv_Deliver.Visible = false;
                //dv_price.Visible = false;
            }
            Session["TypeSend"] = ddlTypsend.SelectedValue;
            div_Page_Bar.Visible = true;
        }

        protected void btnUpdatePno_Click(object sender, EventArgs e)
        {
            var orderList = (from orderItem in carrier_Entities.Order_Item
                             join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                             where orderItem.Status == null && order.Transport_Type == 1
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
                    

                    #region V2
                    var checkNO = lastNolist.OrderByDescending(o => o.History_ID).FirstOrDefault().History_NO;
                    if (checkNO.Length == 8)
                    {
                        lastNo = "HIS" + DateTime.Now.Year.ToString().Substring(2, 2) + "00000";
                    }
                    else if (checkNO.Substring(3, 2) != DateTime.Now.Year.ToString().Substring(2, 2))
                    {
                        lastNo = "HIS" + DateTime.Now.Year.ToString().Substring(2, 2) + "00000";
                    }
                    else
                    {
                        lastNo = checkNO;
                    }
                    var lenght = (Convert.ToInt32(lastNo.Substring(5, 5)) + 1).ToString().Length;
                    var newNo = lastNo.Substring(0, 10 - lenght) + (Convert.ToInt32(lastNo.Substring(5, 5)) + 1).ToString();
                    #endregion
                    if (his.Count == 0)
                    {
                        carrier_Entities.History_Notify_Order.Add(new History_Notify_Order { Date_Notify = DateTime.Now, Docno = order.FirstOrDefault().Docno, pno = order.FirstOrDefault().pno, Type_Send_KA = order.FirstOrDefault().TypeSendKO, History_NO = newNo ,SaveFrom = "Update"});
                        carrier_Entities.SaveChanges();
                    }

                }
            }
            ClientScript.RegisterStartupScript(this.GetType(), "Success", "<script type='text/javascript'>alert('อัพเดท Order สำเร็จ');window.location='Default';</script>'");
        }

        protected void imgbtnEdit_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnEdit = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnEdit.NamingContainer;
            LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");
            service_Flashs.CheckNotify(lkbDocno.Text);
            Response.Redirect("Transport_Form?Docno=" + lkbDocno.Text+"&Act=Edit");
        }

        protected void imgbtnGet_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnGet = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnGet.NamingContainer;
            Label lbpno = (Label)row.FindControl("lbpno");
            LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");
            var loadFL = carrier_Entities.Order_Big_Box.Where(w => w.BFID == lkbDocno.Text).ToList();
            var res = "";
            if (txtDeliveryOrder.Text == "")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ยังไม่ได้ใส่ Delivery Order')", true);
                txtDeliveryOrder.Focus();
                return;
            }

            //if (txtPriceDelivery.Text == "")
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ยังไม่ได้ใส่จำนวนเงินครับ')", true);
            //    txtPriceDelivery.Focus();
            //    return;
            //}

            //if (!Double.TryParse(txtPriceDelivery.Text, out _))
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('จำนวนเงินไม่ถูกต้อง')", true);
            //    txtPriceDelivery.Focus();
            //    return;
            //}

            var inbox = carrier_Entities.Order_Big_Box.Where(w => w.BFID == lkbDocno.Text).ToList();
            if (inbox.Count() != 0)
            {
                foreach (var b in inbox)
                {
                    var tran = carrier_Entities.Orders.Where(w => w.Docno == b.Docno).FirstOrDefault();
                    if (tran.Transport_Type == 2)
                    {
                        var orderBig = carrier_Entities.Order_Big_Box.Where(w => w.Docno == tran.Docno).FirstOrDefault();
                        orderBig.StatusCal = "F";
                        var slItemslItem = carrier_Entities.Order_Item.Where(w => w.Docno == b.Docno).First();
                        slItemslItem.Status = "SL";
                        slItemslItem.Date_Success = DateTime.Now;
                        carrier_Entities.SaveChanges();
                        if (txtDeliveryOrder.Text != "")
                        {
                            carrier_Entities.Calculate_Car.Add(new Calculate_Car
                            {
                                DeliveryNumber = txtDeliveryOrder.Text,
                                BFID = b.BFID,
                                Docno = b.Docno,
                                QTY = 1,
                                Date_Group = DateTime.Now,
                                TypeSendKO = slItemslItem.TypeSendKO,
                                SDpart = tran.SDpart,
                                SiteStorage = tran.siteStorage
                            });
                            carrier_Entities.SaveChanges();
                            res = "ยืนยันรับสินค้าแล้วครับ และ";
                        }

                    }
                }
            }

            //var carAll = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == txtDeliveryOrder.Text).ToList();
            //foreach (var c in carAll)
            //{
            //    c.Price = Convert.ToDouble((Convert.ToDouble(txtPriceDelivery.Text) / carAll.Count()).ToString("#,##0.00"));
            //}
            //carrier_Entities.SaveChanges();

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('succes :"+ res + " บันทึกข้อมูลรถ Lalamove เรียบร้อยแล้วครับ');window.location='Default';", true);


            loadtable(1);
        }

        

        protected void btnConfirmDelivery_Click(object sender, EventArgs e)
        {
            var DocBGSL = "";

            if (txtDeliveryOrder.Text == "")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ยังไม่ได้ใส่ Delivery Order')", true);
                txtDeliveryOrder.Focus();
                return;
            }

            //if(txtPriceDelivery.Text == "")
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ยังไม่ได้ใส่จำนวนเงินครับ')", true);
            //    txtPriceDelivery.Focus();
            //    return;
            //}

            //if(!Double.TryParse(txtPriceDelivery.Text,out _))
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('จำนวนเงินไม่ถูกต้อง')", true);
            //    txtPriceDelivery.Focus();
            //    return;
            //}
            foreach (GridViewRow row in gv_OrderAll.Rows)
            {
                LinkButton lkbDocno = (LinkButton)row.FindControl("lkbDocno");
                CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                Label lbpno = (Label)row.FindControl("lbpno");
                if (cbItem.Checked)
                {
                    var inbox = carrier_Entities.Order_Big_Box.Where(w => w.BFID == lkbDocno.Text).ToList();
                    if (inbox.Count() != 0)
                    {
                        foreach (var b in inbox)
                        {
                            var tran = carrier_Entities.Orders.Where(w => w.Docno == b.Docno).FirstOrDefault();
                            if (tran.Transport_Type == 2)
                            {
                                var orderBig = carrier_Entities.Order_Big_Box.Where(w => w.Docno == tran.Docno).FirstOrDefault();
                                orderBig.StatusCal = "F";
                                DocBGSL += orderBig.BFID + ",";
                                var slItemslItem = carrier_Entities.Order_Item.Where(w => w.Docno == b.Docno).First();
                                slItemslItem.Status = "SL";
                                slItemslItem.Date_Success = DateTime.Now;
                                carrier_Entities.SaveChanges();
                                if(txtDeliveryOrder.Text != "")
                                {
                                    carrier_Entities.Calculate_Car.Add(new Calculate_Car
                                    {
                                        DeliveryNumber = txtDeliveryOrder.Text,
                                        BFID = b.BFID,
                                        Docno = b.Docno,
                                        QTY = 1,
                                        Date_Group = DateTime.Now,
                                        TypeSendKO = slItemslItem.TypeSendKO,
                                        SDpart = tran.SDpart,
                                        SiteStorage = tran.siteStorage
                                    });
                                    carrier_Entities.SaveChanges();
                                }
                                
                            }
                        }
                    }
                }
            }

            //var carAll = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == txtDeliveryOrder.Text).ToList();
            //foreach (var c in carAll)
            //{
            //    c.Price = Convert.ToDouble((Convert.ToDouble(txtPriceDelivery.Text) / carAll.Count()).ToString("#,##0"));
            //}
            //carrier_Entities.SaveChanges();

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('succes : บันทึกข้อมูล Lalamove เรียบร้อยแล้วครับ');window.location='Default';", true);
            //dv_Lalamove.Visible = false;
            //dv_Main.Style.Remove("filter");
            //dv_Main.Style.Remove("pointer-events");
            
        }

        protected void btnCloseDv_Lalamove_Click(object sender, EventArgs e)
        {
            //dv_Main.Style.Remove("filter");
            //dv_Main.Style.Remove("pointer-events");
            //dv_Lalamove.Visible = false;
            //txtDeliveryOrder.Text = "";
            //txtPriceDelivery.Text = "";
            //btnConfirmDelivery.Text = "";
        }



        //protected void Time1_Tick(object sender, EventArgs e)
        //{

        //    //var date = Convert.ToDateTime(lbTime.Text != ""? lbTime.Text : DateTime.Now.ToString("H:mm"));
        //    var date = DateTime.Now;
        //    var minute = date.Minute;
        //    if((minute%30) == 0)
        //    {
        //        btnUpdatePno_Click(this, EventArgs.Empty);
        //    }
        //}


    }
    public class messageNotify
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}