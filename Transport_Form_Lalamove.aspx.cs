using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.Carrier;
using Carrier.Model.Budget;
using Carrier.Model.Whale;
using Carrier.Model.InsideSFG_WF;
using Carrier.Service;
using Carrier.Info;
using System.IO;
using System.Configuration;
using ExcelDataReader;
using System.Data;

namespace Carrier
{
    public partial class Transport_Form_Lalamove : System.Web.UI.Page
    {
        Service_Flash service_Flash = new Service_Flash();
        Service_Budget service_Budget = new Service_Budget();
        CarrierEntities carrier_Entities = new CarrierEntities();
        BudgetEntities budget_Entities = new BudgetEntities();
        WhaleEntities Whale_Entities = new WhaleEntities();
        InsideSFG_WFEntities InsideSFG_WF_Entities = new InsideSFG_WFEntities();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDateSt.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
                txtDateED.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                dv_App.Visible = false;
            }

        }

        public void loadData()
        {
            var dateST = Convert.ToDateTime(txtDateSt.Text);
            var dateED = Convert.ToDateTime(txtDateED.Text);
            dateST = new DateTime(dateST.Year, dateST.Month, dateST.Day, 0, 0, 1);
            dateED = new DateTime(dateED.Year, dateED.Month, dateED.Day, 23, 59, 59);
            var Delivery = new List<string>();
            if(txtDelivery.Text != "")
            {
                Delivery = carrier_Entities.Calculate_Car.Where(w =>  w.DeliveryNumber.Contains(txtDelivery.Text) || txtDelivery.Text == "").GroupBy(g => g.DeliveryNumber).Select(s => s.Key).ToList();
            }
            else
            {
                Delivery = carrier_Entities.Calculate_Car.Where(w => w.Date_Group >= dateST && w.Date_Group <= dateED && (w.DeliveryNumber.Contains(txtDelivery.Text) || txtDelivery.Text == "")).GroupBy(g => g.DeliveryNumber).Select(s => s.Key).ToList();
            }
            List<Calculate_Car> cars = new List<Calculate_Car>();
            double priceTOtal = 0;
            int qtyTOtal = 0;
            foreach (var del in Delivery)
            {
                Calculate_Car car = new Calculate_Car();
                var docno = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == del).ToList();
                var price = docno.Sum(s => s.Price);
                car.Price = price;
                car.DeliveryNumber = del;
                foreach (var s in docno.GroupBy(g => g.SiteStorage).OrderBy(o => o.Key).ToList())
                {
                    if (s == docno.GroupBy(g => g.SiteStorage).OrderByDescending(o => o.Key).FirstOrDefault())
                    {
                        car.SiteStorage += s.Key;
                    }
                    else
                    {
                        car.SiteStorage += s.Key + ",";
                    }
                }
                car.StatusBud = docno.FirstOrDefault().StatusBud;
                foreach (var d in docno.GroupBy(g => g.SDpart).Select(s => s.Key).OrderBy(o => o).ToList())
                {
                    var dep = budget_Entities.Departments.Where(w => w.Department_ID == d).FirstOrDefault();
                    if (dep != null)
                    {
                        if (d == docno.GroupBy(g => g.SDpart).Select(s => s.Key).OrderByDescending(o => o).FirstOrDefault())
                        {
                            car.SDpart += dep.ShortBrand;
                        }
                        else
                        {
                            car.SDpart += dep.ShortBrand + ",";
                        }

                    }
                }
                //car.Price = price;
                car.QTY = docno.Count();
                car.Date_Group = docno.OrderBy(o => o.Date_Group).FirstOrDefault().Date_Group;

                car.Docno = Newtonsoft.Json.JsonConvert.SerializeObject(docno.Select(s => s.BFID).Distinct().ToList());

                cars.Add(car);
                priceTOtal += price ?? 0;
                qtyTOtal += docno.Count();
            }
            cars.Add(new Calculate_Car { DeliveryNumber = "", SiteStorage = "Total", Price = priceTOtal, QTY = qtyTOtal, StatusBud = "N" });
            gv_main.DataSource = cars;
            gv_main.DataBind();
            foreach (GridViewRow row in gv_main.Rows)
            {
                Label lbstatusBud = (Label)row.FindControl("lbstatusBud");
                ImageButton imgbtnCheckOrder = (ImageButton)row.FindControl("imgbtnCheckOrder");
                if (lbstatusBud.Text == "")
                {
                    imgbtnCheckOrder.Visible = true;
                }
                else
                {
                    imgbtnCheckOrder.Visible = false;
                }
            }
        }

        protected void lnkDeliveryNumber_Click(object sender, EventArgs e)
        {
            LinkButton lnkDeliveryNumber = (LinkButton)sender;
            dv_main.Style.Add("filter", "blur(50px)");
            dv_main.Style.Add("pointer-events", "none");
            dv_detail.Visible = true;

            var cal = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == lnkDeliveryNumber.Text).OrderBy(o => o.Docno).ToList();
            var sites = cal.GroupBy(g => g.SiteStorage).Select(s => s.Key).ToList();
            lbDetail_DeliveryID.Text = lnkDeliveryNumber.Text;
            //lbDetail_Price.Text = cal.Sum(s => s.Price??0).ToString("#,##0.00");
            List<modelDetail> Mdetail = new List<modelDetail>();
            foreach (var site in sites)
            {
                modelDetail mde = new modelDetail();
                mde.site = site;
                var siteDigit = new string[] { "ZZ", "ZX", "Z6", "Z7", "ZY" }.Contains(site.Substring(0, 2)) ? site.Substring(0, 6) : site.Substring(0, 4);
                var Cuscode = InsideSFG_WF_Entities.Customer_Tax.Where(w => w.CustomerCode == siteDigit).FirstOrDefault();
                if(Cuscode != null)
                {
                    var provineInt = Convert.ToInt32(Cuscode.Province1);
                    var provineName = Whale_Entities.Provinces.Where(w => w.Province_ID == provineInt).FirstOrDefault();
                    mde.address = Cuscode.Address1 + " " + Cuscode.Area1 + " " + Cuscode.Zone1 + " " + Cuscode.Road1 + " " + (provineName != null ? provineName.Province_Name : "") + " " + Cuscode.Postal1;
                }
                var listInSite = cal.Where(w => w.SiteStorage == site).ToList();
                foreach (var lis in listInSite)
                {
                    modelDetail_Sub sub = new modelDetail_Sub();
                    sub.SiteStorage = site;
                    sub.SDpart = lis.SDpart;
                    sub.TypeSendKO = lis.TypeSendKO;
                    sub.QTY = lis.QTY ?? 0;
                    sub.Price = lis.Price ?? 0;
                    mde.sub.Add(sub);

                }
                Mdetail.Add(mde);
            }
            gv_detail.DataSource = Mdetail;
            gv_detail.DataBind();

            foreach (GridViewRow row in gv_detail.Rows)
            {
                GridView gv_Detail_Sub = (GridView)row.FindControl("gv_Detail_Sub");
                Label lbDetail_SiteStorage = (Label)row.FindControl("lbDetail_SiteStorage");
                var sub = Mdetail.Where(w => w.site == lbDetail_SiteStorage.Text).Select(s => s.sub).FirstOrDefault();
                var department = budget_Entities.Departments.ToList();
                gv_Detail_Sub.DataSource = sub.GroupBy(g => new { SDpart = g.SDpart }).Select(s => new
                {
                    SDpart = department.Where(w => w.Department_ID == s.Key.SDpart).FirstOrDefault().Department_Name,
                    QTY = s.Sum(c => c.QTY),
                    Price = s.Sum(c => c.Price) + " บาท"
                });
                gv_Detail_Sub.DataBind();
            }

        }

        protected void btnCloseDv_detail_Click(object sender, EventArgs e)
        {
            dv_main.Style.Remove("filter");
            dv_main.Style.Remove("pointer-events");
            dv_detail.Visible = false;

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dv_gv_main.Visible = true;
            dv_gv_import_Check.Visible = false;
            loadData();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            Set_Price_Delivery();
            var dateST = Convert.ToDateTime(txtDateSt.Text);
            var dateED = Convert.ToDateTime(txtDateED.Text);
            dateST = new DateTime(dateST.Year, dateST.Month, dateST.Day, 0, 0, 1);
            dateED = new DateTime(dateED.Year, dateED.Month, dateED.Day, 23, 59, 59);
            GridView calGrid = new GridView();
            var cal = carrier_Entities.Lalamove_Import.Where(w => w.Date_Complete >= dateST && w.Date_Complete <= dateED).OrderBy(o => o.Date_Complete).ToList();
            //var cal = carrier_Entities.Calculate_Car.Where(w => w.Date_Group >= dateST && w.Date_Group <= dateED).OrderBy(o => o.Date_Group).ToList();
            if (cal.Count == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่พบข้อมูลในช่วงเวลาที่เลือกครับ')", true);
            }
            else
            {


                if (ddlTypeExport.SelectedValue == "REP")
                {
                    var deliveryGroupID = cal.GroupBy(g => g.Delivery_ID).Select(s => s.Key).ToList();
                    var deliveryID = carrier_Entities.Calculate_Car.Where(w => deliveryGroupID.Contains(w.DeliveryNumber)).ToList();
                    var docno = deliveryID.Select(s => s.Docno).ToList();
                    var order = (from b in carrier_Entities.Order_Big_Box
                                 join o in carrier_Entities.Orders on b.Docno equals o.Docno
                                 join i in carrier_Entities.Order_Item on o.Docno equals i.Docno
                                 where docno.Contains(o.Docno)
                                 select new
                                 {
                                     i.Date_Success,
                                     b.BFID,
                                     o.Docno,
                                     o.siteStorage,
                                     o.SDpart,
                                     i.Qty,
                                     o.saleOn

                                 });
                    List<modelExport> res = new List<modelExport>();
                    foreach (var i in order)
                    {
                        var carCal = carrier_Entities.Calculate_Car.Where(w => w.Docno == i.Docno).FirstOrDefault();

                        modelExport item = new modelExport();
                        item.วันที่_Request = i.Date_Success ?? DateTime.Now;
                        item.เลขORDER = i.Docno;
                        item.ร้าน = i.siteStorage;
                        var brand = budget_Entities.Departments.Where(w => w.Department_ID == i.SDpart).FirstOrDefault();
                        item.Brand = brand.Department_Name;
                        item.จำนวนกล่อง_แปลง = i.Qty ?? 0;
                        item.ค่ารถ = carCal.Price ?? 0;
                        item.TaxCode = "VX";
                        item.DeliveryNumber = carCal.DeliveryNumber;
                        Site_Profit site = new Site_Profit();
                        if (i.siteStorage == "CENTER")
                        {
                            var centerSite = carrier_Entities.Site_Center.Where(w => brand.ShortBrand == w.Brand_Center_Short).ToList().FirstOrDefault();
                            if (centerSite == null)
                            {
                                var pro = carrier_Entities.Site_Profit.Where(w => w.Channel == i.saleOn && w.Brand == brand.ShortBrand
                                && (w.Sale_Channel == "Depart" || w.Sale_Channel == "Shop" || w.Sale_Channel == "WebSite")).ToList().FirstOrDefault();
                                if (pro != null)
                                {
                                    item.ProfitCenter = pro.Profit;
                                    item.CostCenter = pro.Costcenter;
                                    item.ServiceCostCenter = pro.Costcenter;
                                }
                            }
                            else
                            {
                                var pro = carrier_Entities.Site_Profit.Where(w => w.Brand == centerSite.Brand_Center_Name_Full).FirstOrDefault();
                                item.ProfitCenter = pro.Profit;
                                item.CostCenter = pro.Costcenter;
                                item.ServiceCostCenter = pro.Costcenter;
                            }
                            item.Shop = i.siteStorage;
                        }
                        else
                        {
                            site = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == i.siteStorage && w.Brand == brand.ShortBrand).FirstOrDefault();

                            if (site != null)
                            {
                                item.ProfitCenter = site.Profit;
                                item.CostCenter = site.Costcenter;
                                item.ServiceCostCenter = site.Costcenter;
                            }
                            else
                            {
                                item.ProfitCenter = "";
                                item.CostCenter = "";
                                item.ServiceCostCenter = "";
                            }

                            item.Shop = i.siteStorage.Length == 6 ? i.siteStorage : i.siteStorage.Substring(4) + i.siteStorage.Substring(6, 2);
                        }
                        item.Assignment = "Lalamove";
                        
                        res.Add(item);
                    }
                    calGrid.DataSource = res;
                    calGrid.DataBind();
                }
                else
                {
                    dv_App.Visible = true;
                    var deliveryGroupID = cal.GroupBy(g => g.Delivery_ID).Select(s => s.Key).ToList();

                    var Lalamove = (from ll in carrier_Entities.Lalamove_Import
                                    join calcar in carrier_Entities.Calculate_Car on ll.Delivery_ID equals calcar.DeliveryNumber
                                    join order in carrier_Entities.Orders on calcar.Docno equals order.Docno
                                    where deliveryGroupID.Contains(ll.Delivery_ID)
                                    select new model_Lalamove_Cal
                                    {

                                        Account = "6050008",
                                        Amount = calcar.Price ?? 0,
                                        Amount_in_LC = "",
                                        Tax_Base_Amount = "",
                                        Tax_Code = "VX",
                                        Bus_Area = "",
                                        Baseline_Date = "",
                                        Payment_Term = "",
                                        Planning_Level = "",
                                        Profit_Center = "",
                                        Cost_Center = "",
                                        Service_Cost_Center = "",
                                        Order = "",
                                        Shop = calcar.SiteStorage,
                                        Assignment = "Lalamove",
                                        Brand = calcar.SDpart,
                                        DeliveryID = ll.Delivery_ID
                                    }).ToList();

                    Lalamove = Lalamove.GroupBy(g => new { shop = g.Shop, brand = g.Brand, Delivery_ID = g.DeliveryID })
                                .Select(s => new model_Lalamove_Cal
                                {

                                    Account = "6050008",
                                    Amount = s.Sum(c => c.Amount),
                                    Amount_in_LC = "",
                                    Tax_Base_Amount = "",
                                    Tax_Code = "VX",
                                    Bus_Area = "",
                                    Baseline_Date = "",
                                    Payment_Term = "",
                                    Planning_Level = "",
                                    Profit_Center = "",
                                    Cost_Center = "",
                                    Service_Cost_Center = "",
                                    Order = "",
                                    Shop = s.Key.shop,
                                    Assignment = "Lalamove",
                                    Brand = s.Key.brand,
                                    DeliveryID = s.Key.Delivery_ID
                                }).ToList();
                    var seekDepart = budget_Entities.Departments.Where(w => w.Department_Name.StartsWith("SEEK")).Select(s => s.Department_ID).ToList();
                    var Seek = Lalamove.Where(w => seekDepart.Contains(w.Brand)).ToList();
                    foreach (var se in Seek)
                    {
                        Lalamove.Remove(se);
                    }
                    if (Seek.Count() != 0)
                    {
                        Lalamove.AddRange(Seek);
                    }
                    double total = 0;
                    foreach (var l in Lalamove)
                    {
                        var shortBrand = budget_Entities.Departments.Where(w => w.Department_ID == l.Brand).FirstOrDefault();
                        l.Brand = "(" + shortBrand.ShortBrand + ")" + shortBrand.Department_Name;
                        l.Posting_Date = (carrier_Entities.Lalamove_Import.Where(w => w.Delivery_ID == l.DeliveryID).FirstOrDefault().Date_Complete ?? DateTime.Now).ToString("ddMMyyyy");
                        total += l.Amount;

                        //ค่ารถจัดส่ง Auto จากระบบ Courier Lalamove รอบ 08/04/2024 - 30/04/2024 เลข DeliveryID:119975549306 SiteStorage:OPPLOPM1
                        var Delivery = "DeliveryID:" + l.DeliveryID;
                        var SiteCar = "SiteStorage:" + l.Shop;

                        var DocnoINBudget = budget_Entities.MainExpense_Sub.Where(w => w.Docno.StartsWith("UP") &&  w.Detail.Contains(Delivery) && w.Detail.Contains(SiteCar)).FirstOrDefault();
                        if(DocnoINBudget != null)
                        {
                            l.เลขที่เอกสารใน_FC = DocnoINBudget.Docno;
                        }

                        
                        var ProfitSite = carrier_Entities.Site_Profit.Where(w => w.Brand == shortBrand.ShortBrand && w.Site_Stroage.Contains(l.Shop)).FirstOrDefault();
                        l.Shop = l.Shop.Length == 8 ? l.Shop.Substring(0, 4) + l.Shop.Substring(6, 2) : l.Shop;
                        l.Profit_Center = ProfitSite?.Profit;
                        l.Cost_Center = ProfitSite?.Costcenter;
                        l.Service_Cost_Center = ProfitSite?.Costcenter;
                        
                        deliveryGroupID.Remove(l.DeliveryID);
                    }
                    foreach (var i in deliveryGroupID)
                    {
                        var FromLala = carrier_Entities.Lalamove_Import.Where(w => w.Delivery_ID == i).FirstOrDefault();
                        var DatePost = (FromLala.Date_Complete ?? DateTime.Now).ToString("ddMMyyyy");
                        Lalamove.Add(new model_Lalamove_Cal
                        {
                            Posting_Date = DatePost,
                            Account = "6050008",
                            Amount = FromLala.Price ?? 0,
                            Amount_in_LC = "",
                            Tax_Base_Amount = "",
                            Tax_Code = "",
                            Bus_Area = "",
                            Baseline_Date = "",
                            Payment_Term = "",
                            Planning_Level = "",
                            Profit_Center = "",
                            Cost_Center = "",
                            Service_Cost_Center = "",
                            Order = "",
                            Shop = "",
                            Assignment = "Lalamove",
                            Brand = "",
                            DeliveryID = i
                        });
                        total += FromLala.Price ?? 0;
                    }
                    Lalamove.Add(new model_Lalamove_Cal
                    {
                        Account = "70477",
                        Amount = total,
                        Amount_in_LC = "",
                        Tax_Base_Amount = "",
                        Tax_Code = "VX",
                        Bus_Area = "",
                        Baseline_Date = "",
                        Payment_Term = "",
                        Planning_Level = "",
                        Profit_Center = "",
                        Cost_Center = "",
                        Service_Cost_Center = "",
                        Order = "",
                        Shop = "",
                        Assignment = "",
                        Brand = "",
                        DeliveryID = ""
                    });
                    List<model_Lalamove_Export> lalamove_Export = new List<model_Lalamove_Export>();
                    foreach(var l in Lalamove)
                    {
                        model_Lalamove_Export exp = new model_Lalamove_Export();
                        exp.Posting_Date = l.Posting_Date;
                        exp.Account = l.Account;
                        exp.Amount = l.Amount.ToString("#,##0.00");
                        exp.Amount_in_LC = l.Amount_in_LC;
                        exp.Tax_Base_Amount = l.Tax_Base_Amount;
                        exp.Tax_Code = l.Tax_Code;
                        exp.Bus_Area = l.Bus_Area;
                        exp.Baseline_Date = l.Baseline_Date;
                        exp.Payment_Term = l.Payment_Term;
                        exp.Planning_Level = l.Planning_Level;
                        exp.Profit_Center = l.Profit_Center;
                        exp.Cost_Center = l.Cost_Center;
                        exp.Service_Cost_Center = l.Service_Cost_Center;
                        exp.Order = l.Order;
                        exp.Shop = l.Shop;
                        exp.Assignment = l.Assignment;
                        exp.Brand = l.Brand;
                        exp.DeliveryID = l.DeliveryID;
                        exp.เลขที่เอกสารใน_FC = l.เลขที่เอกสารใน_FC;
                        lalamove_Export.Add(exp);
                    }
                    calGrid.DataSource = lalamove_Export;
                    calGrid.DataBind();
                }

                //GridViewRow rowHead = (GridViewRow)history.HeaderRow;
                //Label History_NO = (Label)rowHead.FindControl("History_NO");
                //Label Date_Notify = (Label)rowHead.FindControl("Date_Notify");
                //Label Pno = (Label)rowHead.FindControl("Pno");
                //Label Docno = (Label)rowHead.FindControl("Docno");
                //Label Type_Send_KA = (Label)rowHead.FindControl("Type_Send_KA");
                //Export Excel
                Page.Response.ClearContent();
                Page.Response.AddHeader("Content-Disposition", "attachment;filename=" + ddlTypeExport.SelectedValue + "_Lalamove" + dateST.ToString("dd-MM-yyyy") + "_" + dateED.ToString("dd-MM-yyyy") /*DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")*/ + ".xls");
                Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Page.Response.Charset = "utf-8";
                Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-874");
                Page.Response.ContentType = "application/vnd.ms-excel";
                using (StringWriter strwritter = new StringWriter())
                {
                    HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
                    htmltextwrtter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                    htmltextwrtter.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=windows-874'>");
                    calGrid.AllowPaging = false;
                    calGrid.HeaderRow.BackColor = System.Drawing.Color.Yellow;
                    calGrid.RenderControl(htmltextwrtter);
                    Page.Response.Output.Write(strwritter.ToString());

                    Page.Response.End();
                }
            }
        }


        protected void imgbtnCheckOrder_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnCheckOrder = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnCheckOrder.NamingContainer;
            LinkButton lnkDeliveryNumber = (LinkButton)row.FindControl("lnkDeliveryNumber");
            var delivery = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == lnkDeliveryNumber.Text).ToList();
            foreach (var car in delivery)
            {
                car.StatusBud = "A";
            }
            carrier_Entities.SaveChanges();
            txtDelivery.Text = "";
            loadData();
        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            if (fileupload1.HasFiles)
            {
                string FileName = Path.GetFileName(fileupload1.PostedFile.FileName);

                string Extension = Path.GetExtension(fileupload1.PostedFile.FileName);

                string FolderPath = ConfigurationManager.AppSettings["FolderPath"];
                string FilePath = Server.MapPath(FolderPath + "ExelReport/" + FileName);

                fileupload1.SaveAs(FilePath);
                ReadExcel(FilePath);
                Set_Price_Delivery();
            }
            else
            {
                //service_Budget.JSAlert("E", "ไม่สามารถเปิดไฟล์ได้");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่สามารถเปิดไฟล์ได้')", true);

            }
        }

        public void ReadExcel(string filePath)
        {
            dv_gv_main.Visible = false;
            dv_gv_import_Check.Visible = true;
            using (var steam = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var reader = ExcelReaderFactory.CreateReader(steam);
                var result = reader.AsDataSet();
                var tables = result.Tables.Cast<DataTable>();
                try
                {
                    var rowRead = false;
                    List<modelFromLalamove> dataLalamove = new List<modelFromLalamove>();
                    modelFromLalamove total = new modelFromLalamove();
                    foreach (DataTable table in tables)
                    {
                        if (table == tables.FirstOrDefault())
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                var item = row.ItemArray;
                                if (item[0].ToString() == "ORDER ID")
                                {
                                    rowRead = true;
                                }
                                else
                                {
                                    if (rowRead)
                                    {
                                        if (item[0].ToString() == "")
                                        {
                                            rowRead = false;
                                        }
                                        else
                                        {
                                            var delivery = item[0].ToString();
                                            var cal = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == delivery).ToList();
                                            dataLalamove.Add(new modelFromLalamove
                                            {
                                                DeliveryID = item[0].ToString(),
                                                price_Lalamove = -Convert.ToDouble(item[3].ToString()),
                                                dateComplete = Convert.ToDateTime(item[10].ToString()),
                                                Match = cal.Count() == 0,
                                                price = Convert.ToDouble((cal.Sum(s => s.Price) ?? 0).ToString("#,##0"))
                                            });
                                            total.price_Lalamove += -Convert.ToDouble(item[3].ToString());
                                            total.price += Convert.ToDouble((cal.Sum(s => s.Price) ?? 0).ToString("#,##0"));

                                            var car = carrier_Entities.Lalamove_Import.Where(w => w.Delivery_ID == delivery).FirstOrDefault();
                                            if (car != null)
                                            {
                                                car.Status_Match = cal.Count() != 0;
                                                carrier_Entities.SaveChanges();
                                            }
                                            else
                                            {
                                                Lalamove_Import obj = new Lalamove_Import();
                                                obj.Delivery_ID = delivery;
                                                obj.Date_Complete = Convert.ToDateTime(item[10].ToString());
                                                obj.Price = -Convert.ToDouble(item[3].ToString());
                                                obj.Status_Match = cal.Count() != 0;
                                                carrier_Entities.Lalamove_Import.Add(obj);
                                                carrier_Entities.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    dataLalamove = dataLalamove.OrderBy(o => o.dateComplete).ToList();
                    dataLalamove.Add(total);
                    gv_import_Check.DataSource = dataLalamove;
                    gv_import_Check.DataBind();
                    foreach (GridViewRow row in gv_import_Check.Rows)
                    {
                        Label lbMatch = (Label)row.FindControl("lbMatch");
                        if (Convert.ToBoolean(lbMatch.Text))
                        {
                            gv_import_Check.Rows[row.RowIndex].BackColor = System.Drawing.Color.LightPink;
                        }
                        Label lbDeliveryID = (Label)row.FindControl("lbDeliveryID");
                        if (lbDeliveryID.Text == "")
                        {
                            Label lbDateComplete = (Label)row.FindControl("lbDateComplete");
                            lbDateComplete.Text = "Total";
                        }
                        Label lbPrice = (Label)row.FindControl("lbPrice");
                        Label lbPrice_Lalamove = (Label)row.FindControl("lbPrice_Lalamove");
                        lbPrice.Text = Convert.ToDouble(lbPrice.Text).ToString("#,##0.00");
                        lbPrice_Lalamove.Text = Convert.ToDouble(lbPrice_Lalamove.Text).ToString("#,##0.00");
                    }
                }
                catch (Exception ex)
                {

                }
            }

        }



        public void Set_Price_Delivery()
        {
            var carrierNotPrice = carrier_Entities.Calculate_Car.Where(w => w.Price == null).GroupBy(g => g.DeliveryNumber).Select(s => s.Key).ToList();
            foreach (var i in carrierNotPrice)
            {
                var fromlalamove = carrier_Entities.Lalamove_Import.Where(w => w.Delivery_ID == i).FirstOrDefault();
                if (fromlalamove != null)
                {
                    var carcal = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == fromlalamove.Delivery_ID).ToList();
                    var box = carcal.Select(s => s.QTY).Sum(v => v ?? 0);
                    foreach (var c in carcal)
                    {
                        c.Price = (fromlalamove.Price / box) * c.QTY;
                    }
                    carrier_Entities.SaveChanges();
                }

            }
        }

        protected void ddlTypeExport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTypeExport.SelectedValue == "SAP")
            {

                dv_App.Visible = true;
            }
            else
            {
                dv_App.Visible = false;
            }
        }

        

        protected void btnUptoBudget_Click(object sender, EventArgs e)
        {
            txtDateSt.Enabled = false;
            txtDateED.Enabled = false;
            btnUptoBudget.Visible = false;
            btnApprove.Visible = true;
            btnReject.Visible = true;
            ddlTypeExport.Enabled = false;
            dv_DateST.Style.Add("pointer-events", "none");
            dv_DateED.Style.Add("pointer-events", "none");
        }
        protected void btnReject_Click(object sender, EventArgs e)
        {
            txtDateSt.Enabled = true ;
            txtDateED.Enabled = true;
            btnUptoBudget.Visible = true;
            btnApprove.Visible = false;
            btnReject.Visible = false;
            ddlTypeExport.Enabled = true;
            dv_DateST.Style.Remove("pointer-events");
            dv_DateED.Style.Remove("pointer-events");
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            var dateSTOrigin = Convert.ToDateTime(txtDateSt.Text);
            var dateEDOrigin = Convert.ToDateTime(txtDateED.Text).AddDays(1);
            var dateST = new DateTime(dateSTOrigin.Year, dateSTOrigin.Month, dateSTOrigin.Day, 0, 0, 1);
            var dateED = new DateTime(dateEDOrigin.Year, dateEDOrigin.Month, dateEDOrigin.Day, 0, 0, 1);
            var calNotBud = carrier_Entities.Lalamove_Import.Where(w=> w.Date_Complete < dateED && w.Date_Complete >= dateST).GroupBy(g => g.Delivery_ID).Select(s => s.Key).ToList();
            //var calNotBud = carrier_Entities.Calculate_Car.Where(w => w.Date_Group < dateED && w.Date_Group >= dateST).GroupBy(g => g.DeliveryNumber).Select(s => new { DeliveryNumber = s.Key }).ToList();
            
            var calSuccess = carrier_Entities.Calculate_Car.Where(w => calNotBud.Contains(w.DeliveryNumber) && w.StatusBud == "F").ToList();
            if (calSuccess.Count() != 0)
            {
                txtDateSt.Enabled = true;
                txtDateED.Enabled = true;
                //service_Budget.JSAlert("E", "ไม่สามารถบันทึกเข้า Budget ได้ เนื่องจากว่าในช่วงที่ต้องการได้เคยมีการขึ้น Budget ไปแล้วครับ");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่สามารถบันทึกเข้า Budget ได้ เนื่องจากว่าในช่วงที่ต้องการได้เคยมีการขึ้น Budget ไปแล้วครับ')", true);
                return;
            }
            List<string> deliveryFail = new List<string>();

            foreach (var deli in calNotBud)
            {
                var dateDelivery = carrier_Entities.Lalamove_Import.Where(w => w.Delivery_ID == deli).FirstOrDefault();
                var brand = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == deli).GroupBy(g => g.SDpart).Select(s => new { SDpart = s.Key, QTY = s.Sum(x => x.QTY), Price = s.Sum(x => x.Price) }).ToList();
                foreach (var BrandID in brand)
                {
                    var siteStorage = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == deli && w.SDpart == BrandID.SDpart).GroupBy(g => g.SiteStorage).Select(s => s.Key).ToList();
                    var Seek = budget_Entities.Departments.Where(w => w.Department_Name.StartsWith("SEEK") && w.Department_ID == BrandID.SDpart).FirstOrDefault();

                    foreach (var site in siteStorage)
                    {
                        var Docno = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == deli && w.SDpart == BrandID.SDpart && w.SiteStorage == site).ToList();
                        var docnoOne = Docno.FirstOrDefault().Docno;
                        var carOrder = carrier_Entities.Orders.Where(w => w.Docno == docnoOne).FirstOrDefault();
                        cuttemp temp = new cuttemp();
                        temp.date_use = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        temp.depart_id = Seek == null ? Docno.FirstOrDefault().SDpart : "1619";
                        temp.detail_id = "5703";
                        temp.group_id = "5";
                        temp.head_id = "507";
                        temp.money = Convert.ToDouble(Docno.Sum(c=>c.Price));
                        temp.remark = "ค่ารถจัดส่ง Auto จากระบบ Courier Lalamove รอบ " + txtDateSt.Text + " - " + txtDateED.Text + " เลข DeliveryID:" + deli + " SiteStorage:" + site;
                        temp.typeBudget_id = carOrder.saleOn == "ONLINE" ? "1" : "2";
                        temp.userId = "101974";
                        temp.site_storage = site;
                        var ss = service_Budget.Insert_CutBudget(temp);

                        if (ss == "สำเร็จ")
                        {
                            var Delivery = "DeliveryID:" + deli;
                            var SiteCar = "SiteStorage:" + site;
                            var DocInFC = budget_Entities.MainExpense_Sub.Where(w => w.Docno.StartsWith("UP") && w.Detail.Contains(Delivery) && w.Detail.Contains(SiteCar)).FirstOrDefault();
                            if(DocInFC != null)
                            {
                                var budExpense = budget_Entities.MainExpenses.Where(w => w.Docno == DocInFC.Docno).FirstOrDefault();
                                budExpense.Site_Storage = site;
                                DocInFC.SiteStorage = site;
                                DocInFC.Brand_ID = Docno.FirstOrDefault().SDpart;
                                DocInFC.Brand_Percent = 100;

                            }
                            var carpass = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == deli && w.SDpart == BrandID.SDpart && w.SiteStorage == site).ToList();
                            foreach (var c in carpass)
                            {
                                c.StatusBud = "F";
                                c.DocInFC = DocInFC.Docno;
                            }
                            carrier_Entities.SaveChanges();
                        }
                        else
                        {
                            deliveryFail.Add(deli);
                        }

                    }


                }
                carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
                {
                    dateSend = DateTime.Now,
                    path = "RunAuto/CreateBudget",
                    request = "Delivery:" + deli,
                    respon = "Pass",
                    status = "1"
                });
                carrier_Entities.SaveChanges();
            }

            
            if (deliveryFail.Count() == 0)
            {
                //service_Budget.JSAlert("S", "บันทึกเข้า Budget เรียบร้อยครับ");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกเข้า Budget เรียบร้อยครับ')", true);
            }
            else
            {
                var Delivery = "";
                foreach (var fail in deliveryFail)
                {
                    Delivery += fail + ";";
                }
                //service_Budget.JSAlert("E", "บันทึกเข้า Budget ไม่สำเร็จ Delivery:" + Delivery);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกเข้า Budget ไม่สำเร็จ Delivery: " + Delivery+"')", true);

            }

            txtDateSt.Enabled = true;
            txtDateED.Enabled = true;
            btnUptoBudget.Visible = true;
            btnApprove.Visible = false;
            btnReject.Visible = false;
            ddlTypeExport.Enabled = true;
            dv_DateST.Style.Remove("pointer-events");
            dv_DateED.Style.Remove("pointer-events");
        }
    }

    public class mainCar
    {
        public string DeliveryNumber { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string SiteStorage { get; set; }
    }
    public class modelDepartment
    {
        public string departmentID { get; set; }
        public string departmentName { get; set; }
    }

    public class modelDetail_Sub
    {
        public string DeliveryNumber { get; set; }
        public string BFID { get; set; }
        public string Docno { get; set; }
        public int QTY { get; set; }
        public double Price { get; set; }
        public string Date_Group { get; set; }
        public string TypeSendKO { get; set; }
        public string SDpart { get; set; }
        public string SiteStorage { get; set; }
    }

    public class modelDetail
    {
        public string site { get; set; }
        public string address { get; set; }
        public List<modelDetail_Sub> sub { get; set; }
        public modelDetail()
        {
            sub = new List<modelDetail_Sub>();
        }
    }

    public class modelExportCal
    {
        public string DeliveryOrder { get; set; }
        public string เลขที่เอกสาร { get; set; }
        public string เลขที่กล่อง { get; set; }
        public string SiteStorage { get; set; }
        public string แผนก { get; set; }
        public string ที่อยู๋จัดส่ง { get; set; }
        public string จำนวนกล่อง { get; set; }
        public string ราคาต่อกล่อง { get; set; }
    }
    public class modelFromLalamove
    {
        public string DeliveryID { get; set; }
        public double price { get; set; }
        public double price_Lalamove { get; set; }
        public DateTime dateComplete { get; set; }
        public Boolean Match { get; set; }
    }
    public class model_Lalamove_Cal 
    {
        public string Posting_Date { get; set; }
        public string Account { get; set; }
        public double Amount { get; set; }
        public string Amount_in_LC { get; set; }
        public string Tax_Base_Amount { get; set; }
        public string Tax_Code { get; set; }
        public string Bus_Area { get; set; }
        public string Baseline_Date { get; set; }
        public string Payment_Term { get; set; }
        public string Planning_Level { get; set; }
        public string Profit_Center { get; set; }
        public string Cost_Center { get; set; }
        public string Service_Cost_Center { get; set; }
        public string Order { get; set; }
        public string Shop { get; set; }
        public string Assignment { get; set; }
        public string Brand { get; set; }
        public string DeliveryID { get; set; }
        public string เลขที่เอกสารใน_FC { get; set; }
    }

    public class model_Lalamove_Export
    {
        public string Posting_Date { get; set; }
        public string Account { get; set; }
        public string Amount { get; set; }
        public string Amount_in_LC { get; set; }
        public string Tax_Base_Amount { get; set; }
        public string Tax_Code { get; set; }
        public string Bus_Area { get; set; }
        public string Baseline_Date { get; set; }
        public string Payment_Term { get; set; }
        public string Planning_Level { get; set; }
        public string Profit_Center { get; set; }
        public string Cost_Center { get; set; }
        public string Service_Cost_Center { get; set; }
        public string Order { get; set; }
        public string Shop { get; set; }
        public string Assignment { get; set; }
        public string Brand { get; set; }
        public string DeliveryID { get; set; }
        public string เลขที่เอกสารใน_FC { get; set; }
    }
    public class cuttemp
    {
        public string depart_id { get; set; }
        public string depart_name { get; set; }
        public string group_id { get; set; }
        public string group_name { get; set; }
        public string head_id { get; set; }
        public string head_name { get; set; }
        public string detail_id { get; set; }
        public string detail_name { get; set; }
        public double money { get; set; }
        public DateTime date_use { get; set; }
        public string typeBudget { get; set; }
        public string typeBudget_id { get; set; }
        public string remark { get; set; }
        public int half_month { get; set; }
        public string userId { get; set; }
        public string site_storage { get; set; }
    }
}