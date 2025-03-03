﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Budget ;
using Carrier.Service;
using System.Globalization;
using System.IO;
using System.Web.UI.HtmlControls;

namespace Carrier
{
    public partial class Report_ACC : System.Web.UI.Page
    {
        InsideSFG_WFEntities insideSFG_WF_Entities;
        CarrierEntities carrier_Entities;
        Service_Flash service_Flashs;
        BudgetEntities budget_Entities = new BudgetEntities();
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

                var HistoryNoti = carrier_Entities.History_Notify_Order.Where(w => w.Date_Notify >= datestart && w.Date_Notify <= dateend).ToList();
                List<string> docno = new List<string>();
                if (HistoryNoti.Count != 0)
                {
                    docno = HistoryNoti.Select(s => s.Docno).Distinct().ToList();
                }
                var orderList = (from order in carrier_Entities.Orders
                                 join order_Item in carrier_Entities.Order_Item on order.Docno equals order_Item.Docno
                                 where order_Item.Status != "C" && order_Item.Status != "A" && order_Item.Status != null && order_Item.Date_Success >= datestart && order_Item.Date_Success <= dateend && order.Ref_Order != "ECommerce"
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
                                     FlashLa = order_Item.Status == "SL" ? "Lalamove" : "FlashExpress",
                                     Brand = order.SDpart,
                                     site = order.siteStorage == "" || order.siteStorage == "-" ? "CENTER" : order.siteStorage,
                                     srcAddress = order.srcDetailAddress,
                                     dstAddress = order.dstDetailAddress

                                 }).ToList();

                var orderListTrue = (from order in carrier_Entities.Orders
                                     join order_Item in carrier_Entities.Order_Item on order.Docno equals order_Item.Docno
                                     where order_Item.Status != "C" && order_Item.Status != null && docno.Contains(order_Item.Docno) && order.Ref_Order != "ECommerce"
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
                                         FlashLa = order_Item.Status == "SL" ? "Lalamove" : "FlashExpress",
                                         Brand = order.SDpart,
                                         site = order.siteStorage == "" || order.siteStorage == "-" ? "CENTER" : order.siteStorage,
                                         srcAddress = order.srcDetailAddress,
                                         dstAddress = order.dstDetailAddress
                                     }).ToList();
                orderList.AddRange(orderListTrue);
                var maxrow = 8;
                double maxdata_gvData = (double)((decimal)Convert.ToDecimal(orderList.Count()) / Convert.ToDecimal(maxrow));
                int pageCount_gvData = (int)Math.Ceiling(maxdata_gvData);
                gv_Report.DataSource = orderList.OrderBy(x => x.dateCreate).Skip((page - 1) * maxrow).Take(maxrow);
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
                        //var ShotBand = (from BG_HA in insideSFG_WF_Entities.BG_HApprove
                        //                join BG_HAPF in insideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
                        //                where BG_HA.departmentID == lbBrand.Text
                        //                select new BrandPro
                        //                {
                        //                    DepartmentID = BG_HA.departmentID,
                        //                    Brand = BG_HA.department_,
                        //                    Depart_Short = BG_HAPF.Depart_Short,
                        //                    ComCode = BG_HAPF.ComCode,
                        //                    CostCenter_Offline = BG_HAPF.CostCenter_Offline,
                        //                    CostCenter_Online = BG_HAPF.CostCenter_Online,
                        //                    Profit_Offline = BG_HAPF.Profit_Offline,
                        //                    Profit_Online = BG_HAPF.Profit_Online
                        //                }
                        //         ).ToList().FirstOrDefault();
                        //if(lbBrand.Text == "1619")
                        //{
                        //    var budget = budget_Entities.Departments.Where(w => w.Department_Name.Contains("SEEK") && !new string[] { "1619", "1508" }.Contains(w.Department_ID)).ToList();
                        //    if(budget.Count() != 0)
                        //    {
                                
                        //    }
                        //}
                        Label lbComcode = (Label)row.FindControl("lbComcode");
                        Label lbProfit = (Label)row.FindControl("lbProfit");
                        Label lbCostCenter = (Label)row.FindControl("lbCostCenter");
                        Label lbSaleOn = (Label)row.FindControl("lbSaleOn");
                        Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
                        var ShotBand = budget_Entities.Departments.Where(w => w.Department_ID == lbBrand.Text).FirstOrDefault();
                        if (ShotBand != null)
                        {
                            lbBrand.Text = ShotBand.Department_Name;
                            lbBrandShort.Text = ShotBand.ShortBrand;
                            if (lbSiteStorage.Text == "CENTER")
                            {
                                var centerSite = carrier_Entities.Site_Center.Where(w => ShotBand.ShortBrand == w.Brand_Center_Short).ToList().FirstOrDefault();
                                if (centerSite == null)
                                {
                                    var pro = carrier_Entities.Site_Profit.Where(w => w.Channel == lbSaleOn.Text && w.Brand == ShotBand.ShortBrand
                                    && (w.Sale_Channel == "Depart" || w.Sale_Channel == "Shop" || w.Sale_Channel == "WebSite")).ToList().FirstOrDefault();
                                    if (pro != null)
                                    {
                                        lbProfit.Text = pro.Profit;
                                        lbComcode.Text = pro.COMCODE;
                                        lbCostCenter.Text = pro.Costcenter;
                                    }
                                }
                                else
                                {
                                    var pro = carrier_Entities.Site_Profit.Where(w => w.Brand == centerSite.Brand_Center_Name_Full).FirstOrDefault();
                                    lbProfit.Text = pro.Profit;
                                    lbComcode.Text = pro.COMCODE;
                                    lbCostCenter.Text = pro.Costcenter;
                                }
                            }
                            else
                            {
                                var Profit = (from pro in carrier_Entities.Site_Profit
                                              where pro.Site_Stroage == lbSiteStorage.Text && pro.Channel == lbSaleOn.Text && pro.Brand.Contains(ShotBand.ShortBrand)
                                              select new { ComCode = pro.COMCODE, profit = pro.Profit, CostCenter = pro.Costcenter }).ToList().FirstOrDefault();
                                if (Profit != null)
                                {

                                    lbComcode.Text = Profit.ComCode;
                                    lbProfit.Text = Profit.profit;
                                    lbCostCenter.Text = Profit.CostCenter;
                                }
                                else
                                {
                                    if (lbSiteStorage.Text == "CENTER_ONLINE" || lbSiteStorage.Text == "CENTER_OFFLINE")
                                    {
                                        var ProfitNoSiteCenter = (from pro in carrier_Entities.Site_Profit
                                                                  where pro.Channel == lbSaleOn.Text && pro.Brand.Contains(ShotBand.ShortBrand) && (pro.Sale_Channel == "Depart" || pro.Sale_Channel == "Shop" || pro.Sale_Channel == "WebSite")
                                                                  select new { ComCode = pro.COMCODE, profit = pro.Profit, CostCenter = pro.Costcenter }).ToList().FirstOrDefault();
                                        if (ProfitNoSiteCenter != null)
                                        {
                                            lbComcode.Text = ProfitNoSiteCenter.ComCode;
                                            lbProfit.Text = ProfitNoSiteCenter.profit;
                                            lbCostCenter.Text = ProfitNoSiteCenter.CostCenter;
                                        }
                                    }
                                    else if (lbSiteStorage.Text.StartsWith("EV"))
                                    {
                                        var site = lbSiteStorage.Text;
                                        var eventCheck = carrier_Entities.Event_Shop.Where(w => w.Shop_Code == site).FirstOrDefault();
                                        if (eventCheck != null)
                                        {
                                            var ProfitNoSiteCenter = (from pro in carrier_Entities.Site_Profit
                                                                      where pro.Channel == "OFFLINE" && pro.Brand.Contains(ShotBand.ShortBrand) && (pro.Sale_Channel == "Depart" || pro.Sale_Channel == "Shop")
                                                                      select new { ComCode = pro.COMCODE, profit = pro.Profit, CostCenter = pro.Costcenter }).ToList().FirstOrDefault();
                                            if (ProfitNoSiteCenter != null)
                                            {
                                                lbComcode.Text = ProfitNoSiteCenter.ComCode;
                                                lbProfit.Text = ProfitNoSiteCenter.profit;
                                                lbCostCenter.Text = ProfitNoSiteCenter.CostCenter;
                                            }
                                        }
                                    }
                                }
                            }
                            #region OLD
                            //var Profit = (from pro in carrier_Entities.Site_Profit
                            //              where pro.Site_Stroage == lbSiteStorage.Text && pro.Channel == lbSaleOn.Text && pro.Brand.Contains(ShotBand.Depart_Short+"%")
                            //              select new { ComCode = pro.COMCODE, profit = pro.Profit, CostCenter = pro.Costcenter }).ToList().FirstOrDefault();
                            //if (Profit != null)
                            //{

                            //    lbComcode.Text = Profit.ComCode;
                            //    lbProfit.Text = Profit.profit;
                            //    lbCostCenter.Text = Profit.CostCenter;
                            //}
                            //else
                            //{
                            //    lbComcode.Text = ShotBand.ComCode;
                            //    if (lbSaleOn.Text == "ONLINE")
                            //    {
                            //        lbProfit.Text = ShotBand.Profit_Online;
                            //        lbCostCenter.Text = ShotBand.CostCenter_Online;
                            //    }
                            //    else if (lbSaleOn.Text == "OFFLINE")
                            //    {
                            //        lbProfit.Text = ShotBand.Profit_Offline;
                            //        lbCostCenter.Text = ShotBand.CostCenter_Offline;
                            //    }
                            //}
                            #endregion

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
            btnClear.Visible = true;
            btnExport.Visible = true;
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
        protected void btnClear_Click(object sender, EventArgs e)
        {
            lbFirstLoad.Text = "first";
            txtDateStart.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtDateEnd.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            loadTable(1);
            btnClear.Visible = false;
            btnExport.Visible = false;
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            var format = "dd/MM/yyyy";
            var enUS = new CultureInfo("en-US");
            var datestart = DateTime.ParseExact(txtDateStart.Text, format, enUS, DateTimeStyles.None);
            var dateend = DateTime.ParseExact(txtDateEnd.Text, format, enUS, DateTimeStyles.None);
            var HistoryNoti = carrier_Entities.History_Notify_Order.Where(w => w.Date_Notify >= datestart && w.Date_Notify <= dateend).ToList();
            if (HistoryNoti.Count != 0)
            {
                var docno = HistoryNoti.Select(s => s.Docno).Distinct();
                var orderList = (from order in carrier_Entities.Orders
                                 join order_Item in carrier_Entities.Order_Item on order.Docno equals order_Item.Docno
                                 where order_Item.Status != "C" && order_Item.Status != "A" && order_Item.Status != null && order_Item.Date_Success >= datestart && order_Item.Date_Success <= dateend && order.Ref_Order != "ECommerce"
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
                                     FlashLa = order_Item.Status == "SL" ? "Lalamove" : "FlashExpress",
                                     SaleOn = order.saleOn,
                                     Brand = order.SDpart,
                                     site = order.siteStorage == "" || order.siteStorage == "-" ? "CENTER" : order.siteStorage,
                                     srcAddress = order.srcDetailAddress,
                                     dstAddress = order.dstDetailAddress
                                 }).ToList();
                var orderListNotNoti = (from order in carrier_Entities.Orders
                                        join order_Item in carrier_Entities.Order_Item on order.Docno equals order_Item.Docno
                                        where order_Item.Status == null && order_Item.Date_Success >= datestart && order_Item.Date_Success <= dateend && order.Ref_Order != "ECommerce"
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
                                            FlashLa = order_Item.Status == "SL" ? "Lalamove" : "FlashExpress",
                                            SaleOn = order.saleOn,
                                            Brand = order.SDpart,
                                            site = order.siteStorage == "" || order.siteStorage == "-" ? "CENTER" : order.siteStorage,
                                            srcAddress = order.srcDetailAddress,
                                            dstAddress = order.dstDetailAddress
                                        }).ToList();
                List<string> doc = new List<string>();
                foreach (var i in orderListNotNoti)
                {
                    var res = service_Flashs.CheckNotify(i.Docno);
                    if (res != "")
                    {
                        doc.Add(i.Docno);
                    }
                }
                orderListNotNoti = orderListNotNoti.Where(w => doc.Contains(w.Docno)).ToList();
                var orderListTrue = (from order in carrier_Entities.Orders
                                     join order_Item in carrier_Entities.Order_Item on order.Docno equals order_Item.Docno
                                     where order_Item.Status != "C" && order_Item.Status != null && docno.Contains(order_Item.Docno) && order.Ref_Order != "ECommerce"
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
                                         FlashLa = order_Item.Status == "SL" ? "Lalamove" : "FlashExpress",
                                         SaleOn = order.saleOn,
                                         Brand = order.SDpart,
                                         site = order.siteStorage == "" || order.siteStorage == "-" ? "CENTER" : order.siteStorage,
                                         srcAddress = order.srcDetailAddress,
                                         dstAddress = order.dstDetailAddress
                                     }).ToList();
                orderList.AddRange(orderListNotNoti);
                orderList.AddRange(orderListTrue);
                orderList = orderList.OrderBy(o => o.Docno).ToList();
                gv_Report.DataSource = orderList;
                gv_Report.DataBind();
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
                                        select new BrandPro
                                        {
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
                        Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");

                        if (ShotBand != null)
                        {
                            lbBrand.Text = ShotBand.Brand;
                            lbBrandShort.Text = ShotBand.Depart_Short;
                            if (lbSiteStorage.Text == "CENTER")
                            {
                                var centerSite = carrier_Entities.Site_Center.Where(w => ShotBand.Depart_Short == w.Brand_Center_Short).ToList().FirstOrDefault();
                                if (centerSite == null)
                                {
                                    var pro = carrier_Entities.Site_Profit.Where(w => w.Channel == lbSaleOn.Text && w.Brand == ShotBand.Depart_Short
                                    && (w.Sale_Channel == "Depart" || w.Sale_Channel == "Shop" || w.Sale_Channel == "WebSite")).ToList().FirstOrDefault();
                                    if (pro != null)
                                    {
                                        lbProfit.Text = pro.Profit;
                                        lbComcode.Text = pro.COMCODE;
                                        lbCostCenter.Text = pro.Costcenter;
                                    }
                                }
                                else
                                {
                                    var pro = carrier_Entities.Site_Profit.Where(w => w.Brand == centerSite.Brand_Center_Name_Full).FirstOrDefault();
                                    lbProfit.Text = pro.Profit;
                                    lbComcode.Text = pro.COMCODE;
                                    lbCostCenter.Text = pro.Costcenter;
                                }
                            }
                            else
                            {
                                var Profit = (from pro in carrier_Entities.Site_Profit
                                              where pro.Site_Stroage == lbSiteStorage.Text && pro.Channel == lbSaleOn.Text && pro.Brand.Contains(ShotBand.Depart_Short)
                                              select new { ComCode = pro.COMCODE, profit = pro.Profit, CostCenter = pro.Costcenter }).ToList().FirstOrDefault();
                                if (Profit != null)
                                {

                                    lbComcode.Text = Profit.ComCode;
                                    lbProfit.Text = Profit.profit;
                                    lbCostCenter.Text = Profit.CostCenter;
                                }
                                else
                                {
                                    if (lbSiteStorage.Text == "CENTER_ONLINE" || lbSiteStorage.Text == "CENTER_OFFLINE")
                                    {
                                        var ProfitNoSiteCenter = (from pro in carrier_Entities.Site_Profit
                                                                  where pro.Channel == lbSaleOn.Text && pro.Brand.Contains(ShotBand.Depart_Short) && (pro.Sale_Channel == "Depart" || pro.Sale_Channel == "Shop" || pro.Sale_Channel == "WebSite")
                                                                  select new { ComCode = pro.COMCODE, profit = pro.Profit, CostCenter = pro.Costcenter }).ToList().FirstOrDefault();
                                        if (ProfitNoSiteCenter != null)
                                        {
                                            lbComcode.Text = ProfitNoSiteCenter.ComCode;
                                            lbProfit.Text = ProfitNoSiteCenter.profit;
                                            lbCostCenter.Text = ProfitNoSiteCenter.CostCenter;
                                        }
                                    }
                                    else if (lbSiteStorage.Text.StartsWith("EV"))
                                    {
                                        var site = lbSiteStorage.Text;
                                        var eventCheck = carrier_Entities.Event_Shop.Where(w => w.Shop_Code == site).FirstOrDefault();
                                        if(eventCheck != null)
                                        {
                                            var ProfitNoSiteCenter = (from pro in carrier_Entities.Site_Profit
                                                                      where pro.Channel == "OFFLINE" && pro.Brand.Contains(ShotBand.Depart_Short) && (pro.Sale_Channel == "Depart" || pro.Sale_Channel == "Shop" )
                                                                      select new { ComCode = pro.COMCODE, profit = pro.Profit, CostCenter = pro.Costcenter }).ToList().FirstOrDefault();
                                            if (ProfitNoSiteCenter != null)
                                            {
                                                lbComcode.Text = ProfitNoSiteCenter.ComCode;
                                                lbProfit.Text = ProfitNoSiteCenter.profit;
                                                lbCostCenter.Text = ProfitNoSiteCenter.CostCenter;
                                            }
                                        }
                                        
                                    }
                                }
                            }
                            #region OLD
                            //lbBrand.Text = ShotBand.Brand;
                            //lbBrandShort.Text = ShotBand.Depart_Short;
                            //var Profit = (from pro in carrier_Entities.Site_Profit
                            //              where pro.Site_Stroage == lbSiteStorage.Text && pro.Channel == lbSaleOn.Text && pro.Brand.Contains(ShotBand.Depart_Short + "%")
                            //              select new { ComCode = pro.COMCODE, profit = pro.Profit, CostCenter = pro.Costcenter }).ToList().FirstOrDefault();
                            //if (Profit != null)
                            //{

                            //    lbComcode.Text = Profit.ComCode;
                            //    lbProfit.Text = Profit.profit;
                            //    lbCostCenter.Text = Profit.CostCenter;
                            //}
                            //else
                            //{
                            //    lbComcode.Text = ShotBand.ComCode;
                            //    if (lbSaleOn.Text == "ONLINE")
                            //    {
                            //        lbProfit.Text = ShotBand.Profit_Online;
                            //        lbCostCenter.Text = ShotBand.CostCenter_Online;
                            //    }
                            //    else if (lbSaleOn.Text == "OFFLINE")
                            //    {
                            //        lbProfit.Text = ShotBand.Profit_Offline;
                            //        lbCostCenter.Text = ShotBand.CostCenter_Offline;
                            //    }
                            //}
                            #endregion

                        }

                    }

                }
            }
            var fileName = "Report_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".xls";
            ExportExel(gv_Report, fileName);
        }
        public void ExportExel(GridView gv_Report, string FileName)
        {
            if (gv_Report.Rows.Count == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('วันที่คุณเลือกไม่ได้มีข้อมูลการสร้างรายการใดๆ หรือรูปแบบวันที่เลือกไม่ถูกต้อง')", true);
            }
            else
            {
                Page.Response.ClearContent();
                Page.Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
                Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Page.Response.Charset = "utf-8";
                Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-874");
                Page.Response.ContentType = "application/vnd.ms-excel";
                using (StringWriter strwritter = new StringWriter())
                {
                    HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
                    htmltextwrtter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                    htmltextwrtter.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=windows-874'>");
                    gv_Report.AllowPaging = false;
                    gv_Report.HeaderRow.BackColor = System.Drawing.Color.White;
                    gv_Report.RenderControl(htmltextwrtter);
                    Page.Response.Output.Write(strwritter.ToString());
                    Page.Response.End();
                }
            }

        }
        public override void VerifyRenderingInServerForm(Control control)
        {

        }
        public class BrandPro : BG_HApprove_Profitcenter
        {
            public string Brand { get; set; }
        }

    }
}