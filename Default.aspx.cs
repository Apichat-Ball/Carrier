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

            Session["_UserID"] = "942";

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
                loadtable();
            }
        }
        public void loadtable()
        {
            var user = Convert.ToInt32(Session["_UserID"].ToString());
            var permission = carrier_Entities.Users.Where(w => w.UserID == user).Select(s => s.Permission).FirstOrDefault();
            if (permission != null && permission == "Admin")
            {
                var orderList = (from orderItem in carrier_Entities.Order_Item
                                 join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                                 where orderItem.Status != "C"
                                 select new
                                 {
                                     Docno = orderItem.Docno,
                                     pno = orderItem.pno,
                                     srcName = order.srcName,
                                     dstName = order.dstName,
                                     dateCreate = orderItem.Date_Success,
                                     ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                     TrackingPickup = orderItem.ticketPickupId,
                                     TimeTracking = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.TimeoutAtText).ToList().FirstOrDefault() ?? "",
                                     TrackingMessage = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.TicketMessage).ToList().FirstOrDefault() ?? ""
                                 }).ToList();
                gv_OrderAll.DataSource = orderList;
                gv_OrderAll.DataBind();
                foreach(GridViewRow row in gv_OrderAll.Rows)
                {
                    CheckBox cbItem = (CheckBox)row.FindControl("cbItem");
                    Label lbStatus = (Label)row.FindControl("lbStatus");
                    ImageButton imgbtnCancelOrder = (ImageButton)row.FindControl("imgbtnCancelOrder");
                    if(lbStatus.Text != "") 
                    {
                        cbItem.Visible = false;
                        imgbtnCancelOrder.Visible = false;
                    }
                }
               
            }
            else if (permission == null)
            {
                Response.Redirect("Home_Carrier.aspx");
            }
        }
        protected void lkbpno_Click(object sender, EventArgs e)
        {
            LinkButton lkbpno = (LinkButton)sender;
            Label lbDocno = (Label)lkbpno.NamingContainer.FindControl("lbDocno");
            var lbDocnoss = lbDocno.Text;

            #region Open File pdf
            //String originalPath = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
            //string filePath = originalPath.Substring(0, originalPath.LastIndexOf("/Default")) + "/PDFFile/"+lbDocnoss+".pdf";
            //string fileExtention = Path.GetExtension(filePath);
            //WebClient client = new WebClient();
            //Byte[] buffer = client.DownloadData(filePath);
            //Response.ContentType = service_Flashs.ReturnExtension(fileExtention);

            //Response.AddHeader("content-length", buffer.Length.ToString());
            //Response.BinaryWrite(buffer);
            #endregion
            Response.Redirect("Transport_Form.aspx?Docno=" + lbDocnoss);
            //service_Flashs.Get_Docment(lbDocno.Text);
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
                LinkButton lkbpno = (LinkButton)row.FindControl("lkbpno");
                if (cbItem.Checked)
                {
                    tracking.Add(lkbpno.Text);
                }
            }
            if (tracking.Count == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่ได้เลือก Order ที่ต้องการเรียกรถมารับพัสดุ')", true);
            }
            else
            {
                var responseNotify = service_Flashs.Notify(tracking);

                if (responseNotify.code == 1)
                {
                    foreach (var pno in responseNotify.pno)
                    {
                        var orderItem = carrier_Entities.Order_Item.Where(w => w.pno == pno).FirstOrDefault();
                        orderItem.Status = "A";
                        orderItem.CodeResponse = responseNotify.code;
                        orderItem.ticketPickupId = responseNotify.ticketPickupId;
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
                }
                else
                {
                    foreach (var pno in responseNotify.pno)
                    {
                        var orderItem = carrier_Entities.Order_Item.Where(w => w.pno == pno).FirstOrDefault();
                        orderItem.Status = "F";
                        orderItem.CodeResponse = responseNotify.code;
                        carrier_Entities.SaveChanges();
                    }
                }


                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('เรียกรถมารับพัสดุเรียบร้อย โปรดเช็ครายละเอียดการเรียกรถได้ที่ตาราง')", true);
                loadtable();
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
            LinkButton lkbpno = (LinkButton)row.FindControl("lkbpno");
            Label lbDocno = (Label)row.FindControl("lbDocno");
            var res = service_Flashs.CancelOrder(lbDocno.Text, lkbpno.Text);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('succes : " + res + "')", true);
            loadtable();
        }
    }
}