using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            }
        }
        public void loadtable(int page)
        {
            var user = Convert.ToInt32(lbuserid.Text);
            var maxrow = 10;
            var orderList = (from orderItem in carrier_Entities.Order_Item
                             join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                             where order.UserID == user && orderItem.Status != "C"
                             select new
                             {
                                 Docno = orderItem.Docno,
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
                    orderList = orderList.Where(w =>w.status != "A" && w.status != "SP" && w.status != "SL").ToList();
                }
                else
                {
                    if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "")
                    {
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
                }
                
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
                    lbDateCreate.Text = DateTime.Parse(lbDateCreate.Text).ToString("dd/MM/yyyy");
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
                                    lbTimeTrackingText.Text = service_Flashs.CheckNotify(lkDocno.Text);
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
                                    lbTimeTrackingText.Text = service_Flashs.CheckNotify(lkDocno.Text);
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
                    
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเลือกวันที่เริ่มและสิ้นสุดการค้นหา')", true);
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
            Label lkbpno = (Label)row.FindControl("lbpno");
            LinkButton lbDocno = (LinkButton)row.FindControl("lkDocno");
            var res = service_Flashs.CancelOrder(lbDocno.Text, lkbpno.Text);
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

        //protected void imgbtnEditOrder_Click(object sender, ImageClickEventArgs e)
        //{
        //    ImageButton imgbtnEditOrder = (ImageButton)sender;
        //    GridViewRow row = (GridViewRow)imgbtnEditOrder.NamingContainer;
        //    LinkButton lkDocno = (LinkButton)row.FindControl("lkDocno");
        //    Label lbpno = (Label)row.FindControl("lbpno");
        //    Response.Redirect("Transport_Form.aspx?Docno=" + lkDocno.Text + "&pno=" + lbpno.Text);
        //}
    }
}