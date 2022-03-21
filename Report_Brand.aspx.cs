using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Service;
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Carrier;
using System.Globalization;

namespace Carrier
{
    public partial class Report_Brand : System.Web.UI.Page
    {
        Service_Flash service_Flashs = new Service_Flash();
        InsideSFG_WFEntities InsideSFG_WF_Entities = new InsideSFG_WFEntities();
        CarrierEntities Carrier_Entities = new CarrierEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
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
                txtDateStart.Text = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
                txtDateEnd.Text = DateTime.Now.ToString("dd/MM/yyyy");
                loadtable();
            }
        }
        public void loadtable()
        {
            var FC = InsideSFG_WF_Entities.BG_ForeCast.Where(w => w.ActiveStatus == 1).GroupBy(g => g.DepartmentID).Select(s => new Forecasts { DepartmentID = s.Key });
            var depart = (from BG_HA in InsideSFG_WF_Entities.BG_HApprove
                          join BG_HAPF in InsideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
                          where FC.Select(s => s.DepartmentID).Contains(BG_HA.departmentID) && (BG_HA.Sta == "B" || BG_HA.Sta == "S" || BG_HA.Sta == "N")
                          select new { departmentID = BG_HA.departmentID, depart_Short = BG_HAPF.Depart_Short }
                          ).OrderBy(r => r.depart_Short).ToList();
            var order = (from or in Carrier_Entities.Orders
                         join orItem in Carrier_Entities.Order_Item on or.Docno equals orItem.Docno
                         where (orItem.Status != "C" && orItem.Status != null) && or.TypeSend == 1 && or.saleOn == "OFFLINE"
                         select new OrderItem { Docno = or.Docno, siteStorage = or.siteStorage, Date_send = or.Date_send, qty = orItem.Qty ,brand = or.SDpart}).ToList();

            foreach(var i in order)
            {
                i.brand = depart.Where(w => w.departmentID == i.brand).Select(s => s.depart_Short).FirstOrDefault();
                var datesend = Carrier_Entities.History_Notify_Order.Where(w => w.Docno == i.Docno).Select(s => s.Date_Notify).FirstOrDefault();
                if(datesend != null)
                {
                    i.Date_send = DateTime.Parse(datesend.Value.ToShortDateString());
                }
                else
                {
                    i.Date_send = DateTime.Parse(i.Date_send.Value.ToShortDateString());
                }
            }
            order = order.GroupBy(g => new { siteStorage = g.siteStorage, Date_send = g.Date_send, brand = g.brand })
                         .Select(s => new OrderItem { siteStorage = s.Key.siteStorage, brand = s.Key.brand, Date_send = s.Key.Date_send, qty = s.Sum(ss => ss.qty) }).ToList();
            var profit = Carrier_Entities.Site_Profit.Where(w => w.Channel == "OFFLINE" && !w.Site_Stroage.Contains("CENTER") && (w.Sale_Channel == "Depart" || w.Sale_Channel == "Shop"))
                .GroupBy(g => new{ Site_Stroage = g.Site_Stroage ,SaleChannel = g.Sale_Channel}).ToList();
            var format = "dd/MM/yyyy";
            var enUS = new CultureInfo("en-US");
            var dateStart = DateTime.ParseExact(txtDateStart.Text, format, enUS, DateTimeStyles.None);
            var dateEnd = DateTime.ParseExact(txtDateEnd.Text, format, enUS, DateTimeStyles.None);
            var proCheck = profit.Select(s => s.Key.Site_Stroage).ToList();
            order = order.Where(w => proCheck.Contains(w.siteStorage) && (dateStart <= w.Date_send && w.Date_send <= dateEnd)).ToList();
            foreach(var i in order)
            {
                var sale = profit.Where(w => w.Key.Site_Stroage == i.siteStorage).Select(s => s.Key.SaleChannel).FirstOrDefault();
                i.saleChannel = sale;
            }
            List<headItem> head = new List<headItem>();
            head.Add(new headItem
            {
                saleChannel = "Depart",
                saleBrand = order.Where(w=>w.saleChannel == "Depart").ToList()
            });
            head.Add(new headItem
            {
                saleChannel = "Shop",
                saleBrand = order.Where(w => w.saleChannel == "Shop").ToList()
            });
            gv_Head.DataSource = head;
            gv_Head.DataBind();
            foreach(GridViewRow rowh in gv_Head.Rows)
            {
                GridView gv_Brand = (GridView)rowh.FindControl("gv_Table");
                foreach(GridViewRow row in gv_Brand.Rows)
                {
                    Label lbDateSend = (Label)row.FindControl("lbDateSend");
                    lbDateSend.Text = DateTime.Parse(lbDateSend.Text).ToString("dd/MM/yyyy");
                }
            }
        }

        protected void imgbtnAdd_Click(object sender, ImageClickEventArgs e)
        {
            loadtable();
        }
    }
    public class OrderItem
    {
        public string Docno { get; set; }
        public string siteStorage { get; set; }
        public DateTime? Date_send { get; set; }
        public int? qty { get; set; }
        public string brand { get; set; }
        public string saleChannel { get; set; }
    }
    public class headItem
    {
        public string saleChannel { get; set; }
        public List<OrderItem> saleBrand { get; set; }
        public headItem()
        {
            saleBrand = new List<OrderItem>();
        }
    }
}