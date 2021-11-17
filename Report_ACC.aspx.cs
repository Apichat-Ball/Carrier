using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Service;
using System.Globalization;
namespace Carrier
{
    public partial class Report_ACC : System.Web.UI.Page
    {
        InsideSFG_WFEntities insideSFG_WF_Entities;
        CarrierEntities carrier_Entities;
        Service_Flash service_Flashs;
        public Report_ACC()
        {
            insideSFG_WF_Entities = new InsideSFG_WFEntities();
            carrier_Entities = new CarrierEntities();
            service_Flashs = new Service_Flash();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
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
                loadTable(1);
            }
        }
        protected void loadTable(int page)
        {
            if (txtDateStart.Text != "" && txtDateEnd.Text != "")
            {
                var format = "dd/MM/yyyy";
                var enUS = new CultureInfo("en-US");
                var datestart = DateTime.ParseExact(txtDateStart.Text, format, enUS, DateTimeStyles.None);
                var dateend = DateTime.ParseExact(txtDateEnd.Text, format, enUS, DateTimeStyles.None);
                var orderList = (from order in carrier_Entities.Orders
                                 join order_Item in carrier_Entities.Order_Item on order.Docno equals order_Item.Docno
                                 where  order_Item.Status != "C"
                                 select new
                                 {
                                     Docno = order.Docno,
                                     pno = order_Item.pno,
                                     srcName = order.srcName,
                                     dstName = order.dstName,
                                     ArticleCategory = carrier_Entities.Article_Category.Where(w => w.ArticleCode == order.articleCategory).ToList().FirstOrDefault().ArticleName,
                                     dateCreate = order_Item.Date_Success,
                                     TrackingPickup = order_Item.ticketPickupId,
                                     TimeTracking = carrier_Entities.Notifies.Where(w => w.TicketPickupId == order_Item.ticketPickupId).Select(s => s.TimeoutAtText).ToList().FirstOrDefault() ?? "",
                                     SaleOn = order.saleOn,
                                     Brand = order.SDpart
                                 }).ToList();
                if(lbFirstLoad.Text != "first")
                {
                    orderList = orderList.Where(w => w.dateCreate >= datestart && w.dateCreate <= dateend).ToList();
                }
                var maxrow = 10;
                double maxdata_gvData = (double)((decimal)Convert.ToDecimal(orderList.Count()) / Convert.ToDecimal(maxrow));
                int pageCount_gvData = (int)Math.Ceiling(maxdata_gvData);
                gv_Report.DataSource = orderList.OrderByDescending(x => x.dateCreate).Skip((page - 1) * maxrow).Take(maxrow);
                gv_Report.DataBind();
                Page_gv(page, pageCount_gvData);
                foreach (GridViewRow row in gv_Report.Rows)
                {
                    Label lbBrand = (Label)row.FindControl("lbBrand");
                    Label lbBrandShort = (Label)row.FindControl("lbBrandShort");
                    Label lbDateCreate = (Label)row.FindControl("lbDateCreate");
                    lbDateCreate.Text = DateTime.Parse(lbDateCreate.Text).ToString("dd/MM/yyyy");
                    if (lbBrand.Text != "")
                    {
                        var ShotBand = (from BG_HA in insideSFG_WF_Entities.BG_HApprove
                                        join BG_HAPF in insideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
                                        where BG_HA.departmentID == lbBrand.Text
                                        select new BrandPro { 
                                            DepartmentID = BG_HA.departmentID,
                                            Brand = BG_HA.department_, 
                                            Depart_Short = BG_HAPF.Depart_Short,
                                            ComCode = BG_HAPF.ComCode,
                                            CostCenter_Offline = BG_HAPF.CostCenter_Offline,
                                            CostCenter_Online = BG_HAPF.CostCenter_Online,
                                            Profit_Offline = BG_HAPF.Profit_Offline,
                                            Profit_Online = BG_HAPF.Profit_Online
                                        }
                                 ).ToList().FirstOrDefault();
                        Label lbComcode = (Label)row.FindControl("lbComcode");
                        Label lbProfit = (Label)row.FindControl("lbProfit");
                        Label lbCostCenter = (Label)row.FindControl("lbCostCenter");
                        Label lbSaleOn = (Label)row.FindControl("lbSaleOn");

                        if(ShotBand != null)
                        {
                            lbBrand.Text = ShotBand.Brand;
                            lbBrandShort.Text = ShotBand.Depart_Short;
                            lbComcode.Text = ShotBand.ComCode;
                            if (lbSaleOn.Text == "ONLINE")
                            {
                                lbProfit.Text = ShotBand.Profit_Online;
                                lbCostCenter.Text = ShotBand.CostCenter_Online;
                            }
                            else if (lbSaleOn.Text == "OFFLINE")
                            {
                                lbProfit.Text = ShotBand.Profit_Offline;
                                lbCostCenter.Text = ShotBand.CostCenter_Offline;
                            }
                        }
                        
                    }
                    
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเลือกวันที่เริ่มและสิ้นสุดการค้นหา')", true);
            }

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            lbFirstLoad.Text = "two";
            loadTable(1);
        }
        protected void selectPage(object sender, CommandEventArgs e)
        {
            loadTable(Convert.ToInt32(e.CommandArgument));
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
        public class BrandPro : BG_HApprove_Profitcenter
        {
            public string Brand { get; set; }
        }
    }
}