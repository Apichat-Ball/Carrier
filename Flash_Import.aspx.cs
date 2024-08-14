using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.Carrier;
using Carrier.Model.Budget;
using Carrier.Model.Whale;
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Online_NonAPI;
using Carrier.Service;
using static Carrier.Service.Service_Whale;

namespace Carrier
{
    public partial class Flash_Import : System.Web.UI.Page
    {
        CarrierEntities carrier_Entities = new CarrierEntities();
        BudgetEntities budget_Entities = new BudgetEntities();
        WhaleEntities whale_Entities = new WhaleEntities();
        Online_NonAPIEntities entities_Online_NonAPI = new Online_NonAPIEntities();
        InsideSFG_WFEntities insideSFG_WF_Entities = new InsideSFG_WFEntities();

        Service_Flash service_Flash = new Service_Flash();
        Service_Whale service_Whale = new Service_Whale();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDateSt.Text = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                txtDateED.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
            
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
            }
            else
            {
                //service_Budget.JSAlert("E", "ไม่สามารถเปิดไฟล์ได้");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่สามารถเปิดไฟล์ได้')", true);

            }
        }

        public void ReadExcel(string filePath)
        {
            dv_gv_import_Check.Visible = true;
            using (var steam = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var reader = ExcelReaderFactory.CreateReader(steam);
                var result = reader.AsDataSet();
                var tables = result.Tables.Cast<DataTable>();
                try
                {
                    var rowRead = false;
                    foreach (DataTable table in tables)
                    {
                        if (table.TableName == "งานรับ")
                        {
                            List<model_GV_Check> dataCheck = new List<model_GV_Check>();
                            foreach (DataRow row in table.Rows)
                            {
                                var item = row.ItemArray;


                                if(item[0].ToString() == "เวลาทำรายการ")
                                {
                                    rowRead = true;
                                }
                                else
                                {
                                    if (rowRead)
                                    {
                                        
                                        if(item[0].ToString() == "")
                                        {
                                            rowRead = false;
                                        }
                                        else
                                        {
                                            if (DateTime.TryParse(item[0].ToString(), out _))
                                            {
                                                var docno = item[1].ToString();
                                                var pno = item[2].ToString();
                                                var carHave = (from o in carrier_Entities.Orders
                                                               join i in carrier_Entities.Order_Item on o.Docno equals i.Docno
                                                               where o.Docno == docno && i.Status == "A"
                                                               select new
                                                               {
                                                                   docno = o.Docno,
                                                                   pno = i.pno,
                                                                   sitestorage = o.siteStorage,
                                                                   departmentId = o.SDpart
                                                               }).FirstOrDefault();

                                                var shop = "";
                                                var dateprocess = DateTime.Now;
                                                
                                                

                                                var flash = new Flash_EX_Import();

                                                flash.Date_Import = DateTime.Now;
                                                flash.Date_Process = Convert.ToDateTime(item[0].ToString());
                                                flash.Docno = item[1].ToString();
                                                flash.pno = item[2].ToString();
                                                flash.Price = Convert.ToDouble(item[14].ToString());
                                                flash.Status_Budget = false;

                                                var dataTOCheck = new model_GV_Check();
                                                var whaleOrder = Get_Order_Whale(docno).FirstOrDefault();

                                                if (carHave != null)
                                                {
                                                    flash.Status_Match = true;
                                                    flash.Shop = carHave.sitestorage;
                                                    flash.department_id = Convert.ToInt32(carHave.departmentId);
                                                    dataTOCheck.DateProcess = flash.Date_Process ?? DateTime.Now;
                                                    dataTOCheck.Docno = flash.Docno;
                                                    dataTOCheck.Pno = flash.pno;
                                                    dataTOCheck.Price = flash.Price ?? 0;
                                                    dataTOCheck.Docno_Match = true;
                                                    dataTOCheck.sitestorage = carHave.sitestorage;
                                                    dataTOCheck.From = "Carrier";
                                                    dataTOCheck.Department_ID = carHave.departmentId;
                                                }
                                                else
                                                {

                                                    //var Ecommerce = Get_Order_Ecommerce(docno);
                                                    if (whaleOrder != null)
                                                    {
                                                        flash.Shop = whaleOrder.Customer_Code;
                                                        var departTrue = "";
                                                        var depart = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.BRANDABB == whaleOrder.Brand_Short).FirstOrDefault();
                                                        if (depart == null)
                                                        {
                                                            departTrue = budget_Entities.Departments.Where(w => w.ShortBrand == whaleOrder.Brand_Short).FirstOrDefault().Department_ID;
                                                        }
                                                        else
                                                        {
                                                            departTrue = depart.departmentID;
                                                        }
                                                        flash.department_id = Convert.ToInt32(departTrue);
                                                        shop = whaleOrder.Customer_Code;
                                                        dateprocess = whaleOrder.Date_Send ?? DateTime.Now;
                                                        dataTOCheck.sitestorage = whaleOrder.Customer_Code;
                                                        dataTOCheck.From = "Whale";
                                                        dataTOCheck.Department_ID = departTrue;
                                                    }



                                                    flash.Status_Match = false;
                                                    dataTOCheck.DateProcess = flash.Date_Process ?? DateTime.Now;
                                                    dataTOCheck.Docno = flash.Docno;
                                                    dataTOCheck.Pno = flash.pno;
                                                    dataTOCheck.Price = flash.Price ?? 0;
                                                    dataTOCheck.Docno_Match = false;
                                                }



                                                var before = carrier_Entities.Flash_EX_Import.Where(w => w.Docno == docno).FirstOrDefault();
                                                if (before != null)
                                                {

                                                    if (carHave != null)
                                                    {
                                                        before.Price = Convert.ToDouble(item[14].ToString());
                                                        before.Status_Match = true;
                                                        before.Shop = carHave.sitestorage;
                                                        before.department_id = Convert.ToInt32(carHave.departmentId);
                                                    }
                                                    else
                                                    {
                                                        if (whaleOrder != null)
                                                        {
                                                            var departTrue = "";
                                                            var depart = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.BRANDABB == whaleOrder.Brand_Short).FirstOrDefault();
                                                            if(depart == null)
                                                            {
                                                                departTrue = budget_Entities.Departments.Where(w => w.ShortBrand == whaleOrder.Brand_Short).FirstOrDefault().Department_ID;
                                                            }
                                                            else
                                                            {
                                                                departTrue = depart.departmentID;
                                                            }
                                                            before.department_id = Convert.ToInt32(departTrue);
                                                            before.Shop = whaleOrder.Customer_Code;
                                                        }
                                                    }
                                                    dataTOCheck.Docno_Bud = before.Docno_Budget;

                                                }
                                                else
                                                {
                                                    carrier_Entities.Flash_EX_Import.Add(flash);
                                                }

                                                dataCheck.Add(dataTOCheck);
                                                carrier_Entities.SaveChanges();
                                            }
                                            
                                        }
                                    }
                                    
                                }
                            }

                            gv_Import.DataSource = dataCheck.OrderBy(o => o.DateProcess).ToList();
                            gv_Import.DataBind();


                            foreach (GridViewRow row in gv_Import.Rows)
                            {
                                Label lbDocno_Match = (Label)row.FindControl("lbDocno_Match");
                                Label lbDateProcess = (Label)row.FindControl("lbDateProcess");
                                Label lbPno = (Label)row.FindControl("lbPno");
                                Label lbDocno = (Label)row.FindControl("lbDocno");
                                Label lbPrice = (Label)row.FindControl("lbPrice");
                                Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
                                Label lbDepartment_ID = (Label)row.FindControl("lbDepartment_ID");
                                Label lbDocnoInBud = (Label)row.FindControl("lbDocnoInBud");
                                Image imgCheck = (Image)row.FindControl("imgCheck");

                                var match = Convert.ToBoolean(lbDocno_Match.Text);
                                if (match)
                                {
                                    imgCheck.ImageUrl = "~\\Icon\\correct.png";
                                }
                                else
                                {
                                    imgCheck.ImageUrl = "~\\Icon\\x-button.png";
                                    row.BackColor = System.Drawing.Color.LightPink;
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่สามารถอ่านไฟล์ได้')", true);
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            loadData();
            dv_gv_import_Check.Visible = true;
        }

        public void loadData()
        {
            var datest = Convert.ToDateTime(txtDateSt.Text);
            var dateed = Convert.ToDateTime(txtDateED.Text);
            var flash = carrier_Entities.Flash_EX_Import.Where(w => w.Date_Process >= datest && w.Date_Process <= dateed)
                .Select(s=> new
                {
                    Docno_Match = s.Status_Match,
                    DateProcess = s.Date_Process,
                    Pno = s.pno,
                    Docno = s.Docno,
                    Price = s.Price,
                    sitestorage = s.Shop,
                    Department_ID = s.department_id
                })
                .ToList();
            gv_Import.DataSource = flash;
            gv_Import.DataBind();
            

            foreach(GridViewRow row in gv_Import.Rows)
            {
                Label lbDocno_Match = (Label)row.FindControl("lbDocno_Match");
                Label lbDateProcess = (Label)row.FindControl("lbDateProcess");
                Label lbPno = (Label)row.FindControl("lbPno");
                Label lbDocno = (Label)row.FindControl("lbDocno");
                Label lbPrice = (Label)row.FindControl("lbPrice");
                Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
                Label lbDepartment_ID = (Label)row.FindControl("lbDepartment_ID");
                Label lbDocnoInBud = (Label)row.FindControl("lbDocnoInBud");
                Image imgCheck = (Image)row.FindControl("imgCheck");

                var match = Convert.ToBoolean(lbDocno_Match.Text);
                if (match)
                {
                    imgCheck.ImageUrl = "~\\Icon\\correct.png";
                }
                else
                {
                    imgCheck.ImageUrl = "~\\Icon\\x-button.png";
                    row.BackColor = System.Drawing.Color.LightPink;
                }
                
            }
        }

        public List<Model_gvData> Get_Order_Whale(string docno)
        {
            List<Model_gvData> list_Model_gvData = new List<Model_gvData>();
            

            var objOrder = service_Whale.Get_Order(docno)
                .Where(x => !new string[] { "CC", "D" }.Contains(x.Status))
                .GroupBy(g => new
                {
                    Date_Send = g.Date_Send,
                    Docno = g.Docno,
                    Type_Transaction = g.Type_Transaction,
                    Channel_ID = g.Channel_ID,
                    Brand_ID = (g.SKU.Length < 2 ? "XX" : g.SKU.Substring(0, 2)),
                    b_IDCard = g.b_IDCard,
                    Channel_refCode = g.Channel_refCode,
                    SKU = g.SKU
                })
                .Select(s => new
                {
                    Date_Send = s.Key.Date_Send,
                    Docno = s.Key.Docno,
                    Type_Transaction = s.Key.Type_Transaction,
                    Channel_ID = s.Key.Channel_ID,
                    Brand_ID = s.Key.Brand_ID,
                    b_IDCard = s.Key.b_IDCard,
                    Channel_refCode = s.Key.Channel_refCode,
                    SKU = s.Key.SKU
                }).ToList();




            var objCustomer = (from tCusomter in whale_Entities.Customers
                               join tCustomer_Brand in whale_Entities.Customer_Brand on tCusomter.Customer_ID equals tCustomer_Brand.Customer_ID
                               select new
                               {
                                   Customer_ID = tCusomter.Customer_ID,
                                   Channel_ID = tCusomter.Channel_ID,
                                   Brand_ID = tCustomer_Brand.Brand_ID,
                                   Customer_Code = tCusomter.refCode,
                                   SAP_Channel = tCusomter.SAP_Channel
                               }).ToList();


            var objOrder_SAP = (from tOrder in objOrder
                                where tOrder.Type_Transaction == "SAP"
                                select new Model_gvData
                                {
                                    Date_Send = tOrder.Date_Send,
                                    Docno = tOrder.Docno,
                                    Customer_Code = (from tCustomer in objCustomer
                                                     where tCustomer.Channel_ID.ToString() == tOrder.Channel_ID && tCustomer.Brand_ID == tOrder.Brand_ID
                                                             && (tOrder.b_IDCard == "" || tOrder.b_IDCard == null ? !tCustomer.SAP_Channel.EndsWith("_ETAX") : tCustomer.SAP_Channel.EndsWith("_ETAX"))
                                                     select tCustomer).FirstOrDefault()?.Customer_Code,
                                    Brand_Short = tOrder.Brand_ID,
                                    SKU = tOrder.SKU,
                                });

            list_Model_gvData.AddRange(objOrder_SAP.ToList());

            var objOrder_POS = (from tOrder in objOrder
                                where tOrder.Type_Transaction == "POS"
                                select new Model_gvData
                                {
                                    Date_Send = tOrder.Date_Send,
                                    Docno = tOrder.Docno,
                                    Customer_Code = tOrder.Channel_refCode,
                                    SKU = tOrder.SKU,
                                    Brand_Short = tOrder.Brand_ID
                                });

            list_Model_gvData.AddRange(objOrder_POS.ToList());


            ////WMK
            //var objOrder2 = service_Whale.Get_Order2(docno)
            //                .ToList()
            //                //.GroupBy(g => new
            //                //{
            //                //    Date_Send = g.Date_Send,
            //                //    Docno = g.Docno,
            //                //    SKU = g.SKU,
            //                //})
            //                //.Select(s => new Model_gvData
            //                //{
            //                //    Date_Send = s.Key.Date_Send,
            //                //    Docno = s.Key.Docno,
            //                //    SKU = s.Key.SKU
            //                //});
            //                ;
            //if(objOrder2.Count() != 0)
            //{
            //    var ss = "";
            //}
            //list_Model_gvData.AddRange(objOrder2.ToList());
            return list_Model_gvData;
        }

        public void Get_Order_Ecommerce(string docno)
        {

        }


        //Export
        protected void btnExport_Click(object sender, EventArgs e)
        {
            var dateST = Convert.ToDateTime(txtDateSt.Text);
            var dateED = Convert.ToDateTime(txtDateED.Text);

            var data = LoadDataFlash(dateST , dateED);
            if(data.Count() == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่พบข้อมูลในช่วงเวลาที่เลือกครับ')", true);
                return;
            }
            else
            {
                GridView grid = new GridView();
                grid.DataSource = data;
                grid.DataBind();

                Page.Response.ClearContent();
                Page.Response.AddHeader("Content-Disposition", "attachment;filename=SAP_Flash" + dateST.ToString("dd-MM-yyyy") + "_" + dateED.ToString("dd-MM-yyyy") /*DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")*/ + ".xls");
                Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Page.Response.Charset = "utf-8";
                Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-874");
                Page.Response.ContentType = "application/vnd.ms-excel";
                using (StringWriter strwritter = new StringWriter())
                {
                    HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
                    htmltextwrtter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                    htmltextwrtter.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=windows-874'>");
                    grid.AllowPaging = false;
                    grid.HeaderRow.BackColor = System.Drawing.Color.Yellow;
                    grid.RenderControl(htmltextwrtter);
                    Page.Response.Output.Write(strwritter.ToString());

                    Page.Response.End();
                }
            }
            
        }
        public List<modelExport_Flash>  LoadDataFlash(DateTime datest , DateTime dateed)
        {
            var Flash = carrier_Entities.Flash_EX_Import.Where(w => w.Date_Process >= datest && w.Date_Process <= dateed)
                .Select(s => new 
                {
                    Posting_Date = s.Date_Process,
                    Account = "6050008",
                    Amount = s.Price??0,
                    Tax_Code = "VX",
                    Shop = s.Shop.ToUpper(),
                    Assignment = "Flash",
                    department_ID = s.department_id,
                    Docno = s.Docno,
                    เลขที่เอกสารใน_FC = s.Docno_Budget

                }).OrderBy(o=>o.Posting_Date).ToList();
            List<modelExport_Flash> flash_item = new List<modelExport_Flash>();
            foreach(var i in Flash)
            {
                var departmentID_STR = i.department_ID.ToString();
                var seekDepart = budget_Entities.Departments.Where(w => w.Department_Name.StartsWith("SEEK")).Select(s => s.Department_ID).ToList();
                if (!seekDepart.Contains(departmentID_STR))
                {
                    var car = carrier_Entities.Orders.Where(w=>w.Docno == i.Docno).FirstOrDefault();
                    modelExport_Flash FItem = new modelExport_Flash();
                    FItem.Posting_Date = i.Posting_Date.Value.ToString("ddMMyyyy");
                    FItem.Account = i.Account;
                    FItem.Amount = i.Amount;
                    FItem.Tax_Code = i.Tax_Code;
                    FItem.Shop = i.Shop == null ? "" : i.Shop.Length == 6 ? i.Shop: i.Shop.Substring(0,4) + i.Shop.Substring(6,2);
                    FItem.Assignment = i.Assignment;
                    if(departmentID_STR != "")
                    {
                        var brand = budget_Entities.Departments.Where(w => w.Department_ID == departmentID_STR).FirstOrDefault();

                        FItem.Brand = "(" + brand.ShortBrand + ")" + brand.Department_Name;
                        var Channel = "";
                        if (car == null)
                        {
                            Channel = "ONLINE";
                        }
                        else
                        {
                            Channel = car.saleOn;
                        }
                        var siteStorage = i.Shop.Contains("CENTER") ? i.Shop : i.Shop.Length == 6 ? i.Shop.Substring(0, 4) + brand.ShortBrand + i.Shop.Substring(4, 2) : i.Shop;
                        var Profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == siteStorage && w.Brand == brand.ShortBrand && w.Channel == Channel).FirstOrDefault();
                        if (Profit != null)
                        {
                            FItem.Profit_Center = Profit.Profit;
                            FItem.Cost_Center = Profit.Costcenter;
                        }

                    }
                    FItem.Docno = i.Docno;
                    FItem.เลขที่เอกสารใน_FC = i.เลขที่เอกสารใน_FC;

                    

                    flash_item.Add(FItem);
                }


            }

            return flash_item;


        }
    }
    public class model_GV_Check
    {
        public DateTime DateProcess { get; set; }
        public string Docno { get; set; }
        public string Pno { get; set; }
        public double Price { get; set; }
        public string sitestorage { get; set; }
        public string Docno_Bud { get; set; }
        public bool Docno_Match { get; set; }
        public string From { get; set; }
        public string Department_ID { get; set; }
    }

    //Model
    public class Model_gvData
    {
        public DateTime? Date_Send { get; set; }
        public string Docno { get; set; }
        public string Status { get; set; }
        public string Status_Name { get; set; }
        public string Ref_Order { get; set; }
        public int Transaction_Type { get; set; }
        public DateTime? Transaction_Date { get; set; }
        public string Channel_ID { get; set; }
        public string Channel_Name { get; set; }
        public string Customer_Code { get; set; }
        public string s_Name { get; set; }
        public string SKU { get; set; }
        public int? TotalQTY { get; set; }
        public double? TotalNetPrice { get; set; }
        public string Trackingno { get; set; }
        public double? Shippingfee { get; set; }
        public double? Paymentfee { get; set; }
        public double? Commissionfee { get; set; }
        public string Brand_Short { get; set; }

    }

    public class modelExport_Flash
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
        public string Docno { get; set; }
        public string เลขที่เอกสารใน_FC { get; set; }
    }

}