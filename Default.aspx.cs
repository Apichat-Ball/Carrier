using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Service;
using Carrier.Model.Carrier;
using System.IO;
using System.Net;
using System.Globalization;

namespace Carrier
{
    public partial class _Default : Page
    {
        Service_Flash service_Flashs;
        CarrierEntities carrier_Entities;
        public _Default()
        {
            service_Flashs = new Service_Flash();
            carrier_Entities = new CarrierEntities();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();

            //Session["_UserID"] = "942";

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
                txtDateStart.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDateEnd.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                loadtable(1);
            }
        }
        public void loadtable(int page)
        {
            var user = Convert.ToInt32(Session["_UserID"].ToString());
            var permission = carrier_Entities.Users.Where(w => w.UserID == user).Select(s => s.Permission).FirstOrDefault();
            if (permission != null && permission == "Admin")
            {
                var maxrow = 10;
                var orderList = (from orderItem in carrier_Entities.Order_Item
                                 join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                                 where  orderItem.Status != "C"
                                 select new
                                 {
                                     Docno = orderItem.Docno,
                                     pno = orderItem.pno,
                                     srcName = order.srcName,
                                     dstName = order.dstName,
                                     ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                     dateCreate = orderItem.Date_Success,
                                     TrackingPickup = orderItem.ticketPickupId,
                                     TimeTracking = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.TimeoutAtText).ToList().FirstOrDefault() ?? "",
                                 }).ToList();

                    var format = "dd/MM/yyyy";
                    var enUS = new CultureInfo("en-US");
                if(txtDateStart.Text != "" && txtDateEnd.Text != "")
                {
                    var start = DateTime.ParseExact(txtDateStart.Text, format, enUS, DateTimeStyles.None);
                    var end = DateTime.ParseExact(txtDateEnd.Text, format, enUS, DateTimeStyles.None);
                    if (txtDocnoSearch.Text != "" || txtPnoSearch.Text != "" || txtDstNameSearch.Text != "" || txtArticleSearch.Text != "")
                    {
                        orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();
                        if (txtDocnoSearch.Text != "")
                        {
                            orderList = orderList.Where(w => w.Docno == txtDocnoSearch.Text).ToList();
                        }
                        if (txtPnoSearch.Text != "")
                        {
                            orderList = orderList.Where(w => w.pno == txtPnoSearch.Text).ToList();
                        }
                        if (txtDstNameSearch.Text != "")
                        {
                            orderList = orderList.Where(w => w.dstName == txtDstNameSearch.Text).ToList();
                        }
                        if (txtArticleSearch.Text != "")
                        {
                            orderList = orderList.Where(w => w.ArticleCategory == txtArticleSearch.Text).ToList();
                        }

                    }
                    else
                    {

                        orderList = orderList.Where(w => w.dateCreate >= start && w.dateCreate <= end).ToList();

                    }
                    double maxdata_gvData = (double)((decimal)Convert.ToDecimal(orderList.Count()) / Convert.ToDecimal(maxrow));
                    int pageCount_gvData = (int)Math.Ceiling(maxdata_gvData);
                    gv_OrderAll.DataSource = orderList.OrderByDescending(x => x.dateCreate).Skip((page - 1) * maxrow).Take(maxrow);
                    gv_OrderAll.DataBind();
                    Page_gv(page, pageCount_gvData);

                    foreach (GridViewRow row in gv_OrderAll.Rows)
                    {
                        CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                        Label lbDateCreate = (Label)row.FindControl("lbDateCreate");
                        Label lbStatus = (Label)row.FindControl("lbStatus");
                        ImageButton imgbtnCancelOrder = (ImageButton)row.FindControl("imgbtnCancelOrder");
                        lbDateCreate.Text = DateTime.Parse(lbDateCreate.Text).ToString("dd/MM/yyyy");
                        if (lbStatus.Text != "")
                        {
                            cbItem.Visible = false;
                            imgbtnCancelOrder.Visible = false;
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
            if (pageselect <= (pageCount - 2)) { lkLast.Visible = true; }
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
                foreach (GridViewRow row in gv_OrderAll.Rows)
                {
                    CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                    cbItem.Checked = true;
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
                foreach(var responseNotify in responseNotifyList)
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
                            if(lastpno == pno)
                            {
                                mess += pno + " เรียกรถเรียบร้อยแล้ว";
                            }
                            else
                            {
                                mess += pno + ",";
                            }
                            
                        }
                        carrier_Entities.Notifies.Add(new Notify
                        {
                            TicketPickupId = responseNotify.ticketPickupId,
                            StaffInfoId = responseNotify.staffInfoId,
                            StaffInfoName = responseNotify.staffInfoName,
                            StaffInfoPhone = responseNotify.staffInfoPhone,
                            UpCountryNote = responseNotify.upCountryNote,
                            TimeoutAtText = responseNotify.timeoutAtText,
                            TicketMessage = responseNotify.ticketMessage
                        });

                        carrier_Entities.SaveChanges();
                        messageNoti.Add(new messageNotify { code = 1, message = mess });
                    }
                    else
                    {
                        var mess = "เลขที่พัสดุที่ : ";
                        var lastpno = responseNotify.pno.LastOrDefault();
                        foreach (var pno in responseNotify.pno)
                        {
                            var orderItem = carrier_Entities.Order_Item.Where(w => w.pno == pno).FirstOrDefault();
                            orderItem.Status = "F";
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
                var messageAlert = "";
                foreach (var listmess in messageNoti)
                {
                    messageAlert = listmess.message + "\n";
                }
                

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('"+messageAlert+"')", true);
                loadtable(1);
            }

        }

        protected void btnCreateOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("Transport_Form.aspx");
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            loadtable(1);
        }
        protected void selectPage(object sender, CommandEventArgs e)
        {
            loadtable(Convert.ToInt32(e.CommandArgument));
        }
        protected void lkbDocno_Click(object sender, EventArgs e)
        {
            LinkButton lkbDocno = (LinkButton)sender;
            var lbDocnoss = lkbDocno.Text;
            Response.Redirect("Transport_Form.aspx?Docno=" + lbDocnoss);
        }
    }
    public class messageNotify
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}