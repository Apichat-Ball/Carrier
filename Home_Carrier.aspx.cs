using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Carrier
{
    public partial class Home_Carrier : System.Web.UI.Page
    {
        Service_Flash service_Flashs;
        CarrierEntities carrier_Entities;
        InsideSFG_WFEntities insideSFG_WF_Entities;
        public Home_Carrier()
        {
            service_Flashs = new Service_Flash();
            carrier_Entities = new CarrierEntities();
            insideSFG_WF_Entities = new InsideSFG_WFEntities();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            //User ทั่วไป
            if (Session["_UserID"] == null)
            {
                service_Flashs.Check_UserID();
            }
            if (Session["_UserID"] == null)
            {
                Response.Redirect("https://www.sfg-th.com/Login/");
            }
            lbuserid.Text = Session["_UserID"].ToString();
            if (!IsPostBack)
            {
                
                txtDateStart.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDateEnd.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                lbstatusSearch.Text = "First";
                loadtable(1);
                loadComment();
            }
        }
        public void loadtable(int page)
        {
            var user = Convert.ToInt32(lbuserid.Text);
            var maxrow = 10;
            /*allFavorite.Insert(0, new newList { val = "select", text = "เลือกผู้ส่งที่ใช้บ่อย" });
                    allFavorite.Insert(1, new newList { val = "SFG", text = "Star Fashion Group" });
                    allFavorite.Insert(2, new newList { val = "SDC1", text = "SDC1" });
                    ddlFavorites.DataSource = allFavorite;
                    ddlFavorites.DataBind();
                    ddlFavorites.Items.Insert(3, new ListItem{ Value = "ROX", Text = "R.O.X.Flagship store" });*/
            var orderList = (from orderItem in carrier_Entities.Order_Item
                             join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                             where order.UserID == user // && orderItem.Status != "C"
                             select new
                             {
                                 Docno = orderItem.Docno,
                                 pno = orderItem.pno,
                                 TypesendKO = orderItem.TypeSendKO == "SDC1" ? "SDC1" : orderItem.TypeSendKO == "ROX" ? "R.O.X.Flagship store" : "Star Fashion Group",
                                 srcName = order.srcName,
                                 dstName = order.dstName,
                                 ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                 dateCreate = orderItem.Date_Success,
                                 TrackingPickup = orderItem.ticketPickupId,
                                 TimeTracking = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.DateNotify).ToList().FirstOrDefault(),
                                 TimeTrackingText = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.TimeoutAtText).ToList().FirstOrDefault() ?? "",
                                 Brand = order.SDpart,
                                 status = orderItem.Status,
                                 Remark = order.remark
                             }).ToList();
           
            var format = "dd/MM/yyyy";
                var enUS = new CultureInfo("en-US");
            if (txtDateStart.Text != "" && txtDateEnd.Text != "")
            {
                var start = DateTime.ParseExact(txtDateStart.Text, format, enUS, DateTimeStyles.None);
                var end = DateTime.ParseExact(txtDateEnd.Text, format, enUS, DateTimeStyles.None);
                if(lbstatusSearch.Text == "First")
                {
                    orderList = orderList.Where(w =>w.status != "C" && w.status != "A" && w.status != "SP" && w.status != "SL").ToList();
                }
                else
                {
                    if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "")
                    {
                        if (txtPnoSearch.Text != "")
                        {
                            orderList = orderList.Where(w =>  w.pno != null).ToList();
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
                }

                #region Test
                var BigBox = carrier_Entities.Order_Big_Box.ToList();
                if(txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "" || lbstatusSearch.Text != "First")
                {
                    BigBox = BigBox.ToList();
                }
                else
                {
                    BigBox = BigBox.Where(w => w.Status == "A").ToList();
                }
                var DocnoGroup = BigBox.Select(s => s.Docno).ToList();
                var orderListCloon = orderList.ToList();
                var orderGroup = orderList.FindAll(f => DocnoGroup.Contains(f.Docno));
                foreach (var gro in orderGroup)
                {

                    orderList.Remove(gro);
                }
                var groupBox = BigBox.GroupBy(g => new { g.BFID }).Select(s => new { s.Key.BFID, docno = s.Select(d => d.Docno).FirstOrDefault() }).ToList();

                foreach (var Box in groupBox)
                {
                    var orderget = orderListCloon.Where(w => w.Docno == Box.docno).FirstOrDefault();
                    if (orderget != null)
                    {
                        orderList.Add(new
                        {
                            Docno = Box.BFID,
                            pno = orderget.pno,
                            TypesendKO = orderget.TypesendKO,
                            srcName = orderget.srcName,
                            dstName = orderget.dstName,
                            ArticleCategory = orderget.ArticleCategory,
                            dateCreate = orderget.dateCreate,
                            TrackingPickup = orderget.TrackingPickup,
                            TimeTracking = orderget.TimeTracking,
                            TimeTrackingText = orderget.TimeTrackingText,
                            Brand = orderget.Brand,
                            status = orderget.status,
                            Remark = "รายละเอียดอยู่ในเอกสาร"
                        });
                    }
                }

                #endregion

                double maxdata_gvData = (double)((decimal)Convert.ToDecimal(orderList.Count()) / Convert.ToDecimal(maxrow));
                int pageCount_gvData = (int)Math.Ceiling(maxdata_gvData);
                gv_OrderAll.DataSource = orderList.OrderByDescending(x => x.dateCreate).Skip((page - 1) * maxrow).Take(maxrow);
                gv_OrderAll.DataBind();
                
                Page_gv(page, pageCount_gvData);

                foreach (GridViewRow row in gv_OrderAll.Rows)
                {
                    LinkButton lkDocno = (LinkButton)row.FindControl("lkDocno");
                    Label lbDateCreate = (Label)row.FindControl("lbDateCreate");
                    Label lbStatus = (Label)row.FindControl("lbStatus");
                    Label lbTimeTrackingText = (Label)row.FindControl("lbTimeTrackingText");
                    Label lbTimeTracking = (Label)row.FindControl("lbTimeTracking");
                    Label lbStatusItem = (Label)row.FindControl("lbStatusItem");
                    ImageButton imgbtnCancelOrder = (ImageButton)row.FindControl("imgbtnCancelOrder");
                    ImageButton imgbtnEdit = (ImageButton)row.FindControl("imgbtnEdit");
                    lbDateCreate.Text = DateTime.Parse(lbDateCreate.Text).ToString("dd/MM/yyyy");

                    var checkBigBox = carrier_Entities.Order_Big_Box.Where(w => w.BFID == lkDocno.Text).ToList();
                    if (checkBigBox.Count() == 0)
                    {
                        if (lbStatusItem.Text == "A")
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
                                    var a = service_Flashs.CheckNotify(lkDocno.Text);
                                    if (a != "")
                                    {
                                        lbTimeTrackingText.Text = a;
                                    }
                                }
                                else
                                {
                                    lbTimeTrackingText.Text = "วันนี้" + lbTimeTrackingText.Text.Substring(8);
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
                                    var a = service_Flashs.CheckNotify(lkDocno.Text);
                                    if (a != "")
                                    {
                                        lbTimeTrackingText.Text = a;
                                    }
                                }
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
                            imgbtnCancelOrder.Visible = false;
                        }
                        else if(lbStatusItem.Text == "C")
                        {
                            lbTimeTrackingText.Text = "ยกเลิกแล้ว";
                            lbTimeTrackingText.BackColor = System.Drawing.Color.PaleVioletRed;
                            lbTimeTrackingText.ForeColor = System.Drawing.Color.White;
                            lbTimeTrackingText.CssClass = "status-tracking";
                        }
                            row.Cells.RemoveAt(10);
                    }
                    else
                    {
                        List<Model_Chack_Noti> resCheck = new List<Model_Chack_Noti>();
                        foreach (var b in checkBigBox)
                        {
                            if (lbStatusItem.Text == "A")
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
                                        var a = service_Flashs.CheckNotifyBigBox(b.Docno);
                                        if (a.message != "")
                                        {
                                            resCheck.Add(a);
                                        }
                                    }
                                    else
                                    {
                                        resCheck.Add(new Model_Chack_Noti { code = "0", message = "วันนี้" + lbTimeTrackingText.Text.Substring(8) });
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
                                        var a = service_Flashs.CheckNotifyBigBox(b.Docno);
                                        if (a.message != "")
                                        {
                                            resCheck.Add(a);
                                        }
                                    }
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
                                imgbtnCancelOrder.Visible = false;
                            }
                            else if(lbStatusItem.Text == "C")
                            {
                                lbTimeTrackingText.Text = "ยกเลิกแล้ว";
                                lbTimeTrackingText.BackColor = System.Drawing.Color.PaleVioletRed;
                                lbTimeTrackingText.ForeColor = System.Drawing.Color.White;
                                lbTimeTrackingText.CssClass = "status-tracking";
                                imgbtnCancelOrder.Visible = false;
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
                        
                    if (lbStatusItem.Text == "SP")
                    {
                        lbTimeTrackingText.Text = "ส่งผ่านไปรษณีย์แล้ว";
                        imgbtnCancelOrder.Visible = false;
                        lbTimeTrackingText.BackColor = System.Drawing.Color.Gray;
                        lbTimeTrackingText.CssClass = "status-tracking";
                    }
                    if (lbStatusItem.Text == "SL")
                    {
                        lbTimeTrackingText.Text = "ส่งผ่าน Lalamove";
                        imgbtnCancelOrder.Visible = false;
                        lbTimeTrackingText.BackColor = System.Drawing.Color.Orange;
                        lbTimeTrackingText.CssClass = "status-tracking";
                    }
                    Label lbBrand = (Label)row.FindControl("lbBrand");
                    var Brand = (from BG_HA in insideSFG_WF_Entities.BG_HApprove
                                 join BG_HAPF in insideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
                                 where BG_HA.departmentID == lbBrand.Text
                                 select new { Brand = BG_HA.department_ , BrandShort = BG_HAPF.Depart_Short }).ToList().FirstOrDefault();
                    if(Brand != null)
                    {
                        lbBrand.Text = Brand.Brand;
                    }
                    else
                    {
                        lbBrand.Text = "";
                    }

                    var orderListnull = (from orderItem in carrier_Entities.Order_Item
                                     join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                                     where orderItem.Status == null
                                     select order.Docno).ToList();

                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเลือกวันที่เริ่มและสิ้นสุดการค้นหา')", true);
            }


        }
        public void loadComment()
        {
            var datenowOld = DateTime.Now.AddDays(-7);
            var cm = carrier_Entities.Comment_System.Where(w => w.CM_DateCreate > datenowOld).OrderByDescending(o=>o.CM_DateCreate).ToList();
            if(cm.Count == 0)
            {
                dv_Comment.Visible = false;
            }
            else
            {
                dv_Comment.Visible = true;
            }

            gv_UpdateComment.DataSource = cm;
            gv_UpdateComment.DataBind();
            foreach(GridViewRow row in gv_UpdateComment.Rows)
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
                if (lbUrlFile.Text != "")
                {
                    imgGuie.Visible = true;
                    String originalPath = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
                    string filePath = originalPath.Substring(0, originalPath.LastIndexOf("/Home_Carrier")) + "/FilePatchUpdate/" + lbUrlFile.Text;
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
        protected void Page_gv(int pageselect, int pageCount)
        {
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
            if (pageselect - 1 <= 0) { lkPrevious.CssClass = "btn btn-outline-secondary disabled"; }
            lkNext.CssClass = "btn btn-outline-primary";
            if (pageselect + 1 > pageCount) { lkNext.CssClass = "btn btn-outline-secondary disabled"; }

            
            if (lkFirst.Text == pageselect.ToString()) { lkFirst.CssClass = "btn btn-outline-primary active"; } else { lkFirst.CssClass = "btn btn-outline-primary"; }
            if (lkLast.Text == pageselect.ToString()) { lkLast.CssClass = "btn btn-outline-primary active"; } else { lkLast.CssClass = "btn btn-outline-primary"; }
            if (lk1.Text == pageselect.ToString()) { lk1.CssClass = "btn btn-outline-primary active"; } else { lk1.CssClass = "btn btn-outline-primary"; }
            if (lk2.Text == pageselect.ToString()) { lk2.CssClass = "btn btn-outline-primary active"; } else { lk2.CssClass = "btn btn-outline-primary"; }
            if (lk3.Text == pageselect.ToString()) { lk3.CssClass = "btn btn-outline-primary active"; } else { lk3.CssClass = "btn btn-outline-primary"; }

            lk1.CommandArgument = lk1.Text; lk2.CommandArgument = lk2.Text; lk3.CommandArgument = lk3.Text;

        }



        protected void lkDocno_Click(object sender, EventArgs e)
        {
            LinkButton lkDocno = (LinkButton)sender;
            var lbDocnoss = lkDocno.Text;

            Response.Redirect("Transport_Form.aspx?Docno=" + lbDocnoss);
        }
        protected void imgbtnCancelOrder_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnCancelOrder = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnCancelOrder.NamingContainer;
            Label lbpno = (Label)row.FindControl("lbpno");
            LinkButton lkDocno = (LinkButton)row.FindControl("lkDocno");
            var loadFL = carrier_Entities.Order_Big_Box.Where(w => w.BFID == lkDocno.Text).ToList();
            var res = "";
            if (loadFL.Count() != 0)
            {
                List<string> resall = new List<string>();
                foreach (var i in loadFL)
                {
                    var pno = carrier_Entities.Order_Item.Where(w => w.Docno == i.Docno).FirstOrDefault().pno;
                    res = service_Flashs.CancelOrder(i.Docno, pno);
                    resall.Add(res);
                    i.Status = "C";
                    carrier_Entities.SaveChanges();
                }

            }
            else
            {
                res = service_Flashs.CancelOrder(lkDocno.Text, lbpno.Text);
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('succes : " + res + "')", true);
            loadtable(1);
        }
        protected void btnCreateOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("Transport_Form.aspx");
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            lbstatusSearch.Text = "Second";
            btnClear.Visible = true;
            loadtable(1);
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtDateStart.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtDateEnd.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            lbstatusSearch.Text = "First";
            btnClear.Visible = false;
            txtDocnoSearch.Text = "";
            txtPnoSearch.Text = "";
            txtDstNameSearch.Text = "";
            txtArticleSearch.Text = "";
            loadtable(1);
        }

        protected void selectPage(object sender, CommandEventArgs e)
        {
            loadtable(Convert.ToInt32(e.CommandArgument));
        }

    }
}