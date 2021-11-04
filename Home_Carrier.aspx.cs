using Carrier.Model.Carrier;
using Carrier.Service;
using System;
using System.Collections.Generic;
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
        public Home_Carrier()
        {
            service_Flashs = new Service_Flash();
            carrier_Entities = new CarrierEntities();
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
                loadtable();
            }
        }
        public void loadtable()
        {
            var user = Convert.ToInt32(lbuserid.Text);
            var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1);
            var end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1);
            var orderList = (from orderItem in carrier_Entities.Order_Item
                             join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                             where order.UserID == user && orderItem.Status != "C" &&((order.Date_send >= start && order.Date_send <= end) || orderItem.Status == null)
                             select new { 
                                 Docno = orderItem.Docno, 
                                 pno = orderItem.pno, 
                                 srcName = order.srcName, 
                                 dstName = order.dstName,
                                 ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                 dateCreate = orderItem.Date_Success,   
                                 TrackingPickup = orderItem.ticketPickupId,
                                 TimeTracking = carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderItem.ticketPickupId).Select(s => s.TimeoutAtText).ToList().FirstOrDefault() ?? "",
                             }).ToList();
            
            gv_OrderAll.DataSource = orderList;
            gv_OrderAll.DataBind();
            foreach(GridViewRow row in gv_OrderAll.Rows)
            {
                Label lbStatus = (Label)row.FindControl("lbStatus");
                ImageButton imgbtnCancelOrder = (ImageButton)row.FindControl("imgbtnCancelOrder");
                if (lbStatus.Text != "")
                {
                    imgbtnCancelOrder.Visible = false;
                }
            }

            txtDateStart.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString();
            txtDateEnd.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToShortDateString();
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