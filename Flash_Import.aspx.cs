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
using Carrier.Model.Online_Lazada;
using Carrier.Model.SFG;
using Carrier.Service;
using static Carrier.Service.Service_Whale;
using ClosedXML.Excel;

namespace Carrier
{
    public partial class Flash_Import : System.Web.UI.Page
    {
        CarrierEntities carrier_Entities = new CarrierEntities();
        BudgetEntities budget_Entities = new BudgetEntities();
        WhaleEntities whale_Entities = new WhaleEntities();
        Online_NonAPIEntities entities_Online_NonAPI = new Online_NonAPIEntities();
        Online_LazadaEntities online_Lazada_Entities = new Online_LazadaEntities();
        InsideSFG_WFEntities insideSFG_WF_Entities = new InsideSFG_WFEntities();
        SFGEntities sFG_Entities = new SFGEntities();
        

        Service_Flash service_Flash = new Service_Flash();
        Service_Whale service_Whale = new Service_Whale();
        Service_Budget service_Budget = new Service_Budget();

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
                        if (table == tables.FirstOrDefault())
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
                                                
                                                


                                                try
                                                {
                                                    var carHave = (from o in carrier_Entities.Orders
                                                                   join i in carrier_Entities.Order_Item on o.Docno equals i.Docno
                                                                   where o.Docno == docno && i.Status == "A"
                                                                   select new
                                                                   {
                                                                       docno = o.Docno,
                                                                       pno = i.pno,
                                                                       sitestorage = o.siteStorage,
                                                                       departmentId = o.SDpart,
                                                                       saleon = o.saleOn
                                                                   }).FirstOrDefault();

                                                    var shop = "";
                                                    var dateprocess = DateTime.Now;



                                                    var flash = new Flash_EX_Import();
                                                    
                                                    flash.Date_Import = DateTime.Now;
                                                    flash.Date_Process = Convert.ToDateTime(item[0].ToString());
                                                    flash.Docno = item[1].ToString();
                                                    flash.pno = item[2].ToString();
                                                    flash.Price = Convert.ToDouble(item[22].ToString());
                                                    flash.Status_Budget = false;

                                                    var dataTOCheck = new model_GV_Check();
                                                    

                                                    var whaleOrder = Get_Order_Whale(docno).FirstOrDefault();

                                                    if (carHave != null)
                                                    {
                                                        flash.Status_Match = true;
                                                        flash.Shop = carHave.sitestorage;
                                                        flash.department_id = Convert.ToInt32(carHave.departmentId);
                                                        flash.saleOn = carHave.saleon;
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
                                                            var departTrue = "";
                                                            var depart = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.BRANDABB == whaleOrder.Brand_Short).FirstOrDefault();
                                                            if(whaleOrder.Customer_Code != null)
                                                            {
                                                                if (whaleOrder.Customer_Code.Length == 6 )
                                                                {
                                                                    if (whaleOrder.Customer_Code.StartsWith("ZY") || whaleOrder.Customer_Code == "CENTER")
                                                                    {
                                                                        flash.Shop = whaleOrder.Customer_Code;
                                                                    }
                                                                    else
                                                                    {
                                                                        flash.Shop = whaleOrder.Customer_Code.Substring(0, 4) + whaleOrder.Brand_Short + whaleOrder.Customer_Code.Substring(4, 2);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    flash.Shop = whaleOrder.Customer_Code;
                                                                }
                                                            }
                                                            


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
                                                            dataTOCheck.sitestorage = flash.Shop;
                                                            dataTOCheck.From = "Whale";
                                                            dataTOCheck.Department_ID = departTrue;
                                                            flash.saleOn = "ONLINE";
                                                        }
                                                        else
                                                        {
                                                            var sap = (from vbrk in sFG_Entities.SAP_VBRK_NEWSAP
                                                                       join vbrp in sFG_Entities.SAP_VBRP_NEWSAP on vbrk.VBELN equals vbrp.VBELN
                                                                       where vbrk.Refdoc == flash.Docno
                                                                       select new
                                                                       {
                                                                           vbrk.Refdoc,
                                                                           vbrk.KUNRG,
                                                                           vbrp.BRAND
                                                                       }).FirstOrDefault() ;
                                                            if(sap != null)
                                                            {
                                                                var budget = budget_Entities.Departments.Where(w => w.ShortBrand == sap.BRAND).FirstOrDefault();
                                                                if (budget != null)
                                                                {
                                                                    dataTOCheck.sitestorage = sap.KUNRG.Substring(0, 4) + sap.BRAND + sap.KUNRG.Substring(4, 2);
                                                                    dataTOCheck.From = "Ecommerce";
                                                                    dataTOCheck.Department_ID = budget.Department_ID;
                                                                    flash.Shop = sap.KUNRG.Substring(0, 4) + sap.BRAND + sap.KUNRG.Substring(4, 2);
                                                                    flash.department_id = Convert.ToInt32(budget.Department_ID);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                flash.Shop = "";
                                                            }
                                                            if(flash.Shop != "")
                                                            {
                                                                if (new string[] { "ZX", "Z6" }.Contains(flash.Shop.Substring(0, 2)))
                                                                {
                                                                    flash.saleOn = "ONLINE";
                                                                }
                                                                else
                                                                {
                                                                    flash.saleOn = "OFFLINE";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                flash.saleOn = "OFFLINE";
                                                            }
                                                            
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

                                                        before.department_id = flash.department_id;
                                                        before.Shop = flash.Shop;
                                                        before.saleOn = flash.saleOn;
                                                        before.Price = Convert.ToDouble(item[22].ToString());
                                                        dataTOCheck.Docno_Bud = before.Docno_Budget;

                                                    }
                                                    else
                                                    {
                                                        carrier_Entities.Flash_EX_Import.Add(flash);
                                                    }

                                                    dataCheck.Add(dataTOCheck);
                                                    carrier_Entities.SaveChanges();
                                                }
                                                catch(Exception ex)
                                                {
                                                    var ss = "";
                                                    return;
                                                }
                                                
                                            }
                                            
                                        }
                                    }
                                    
                                }
                            }

                            if(dataCheck.Count() == 0)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่พบข้อมูลครับ')", true);
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
            var dateED = Convert.ToDateTime(txtDateED.Text).AddDays(1);

            var data = LoadDataFlash(dateST , dateED);
            if(data == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่พบข้อมูลในช่วงเวลาที่เลือกครับ')", true);
                return;
            }
            else
            {


                var filename = "SAP_Flash" + dateST.ToString("dd-MM-yyyy") + "_" + dateED.ToString("dd-MM-yyyy") /*DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")*/ + ".xls";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(data);
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename= " + filename);
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            
        }
        public DataSet  LoadDataFlash(DateTime datest , DateTime dateed)
        {
            var Flash = carrier_Entities.Flash_EX_Import.Where(w => w.Date_Process >= datest && w.Date_Process <= dateed )
                
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
                    เลขที่เอกสารใน_FC = s.Docno_Budget,
                    saleon = s.saleOn

                }).OrderBy(o=>o.Posting_Date);

            if(Flash == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่พบข้อมูลในช่วงเวลาที่เลือกครับ')", true);
                return null;
            }
            //V1
            List<modelExport_Flash> flash_item = new List<modelExport_Flash>();
            List<modelExport_Flash> flash_item_Seek = new List<modelExport_Flash>();
            double total = 0;
            foreach(var i in Flash)
            {
                var departmentID_STR = i.department_ID.ToString();
                var seekDepart = budget_Entities.Departments.Where(w => w.Department_Name.StartsWith("SEEK")).Select(s => s.Department_ID).ToList();

                var car = carrier_Entities.Orders.Where(w => w.Docno == i.Docno).FirstOrDefault();
                modelExport_Flash FItem = new modelExport_Flash();
                FItem.Posting_Date = i.Posting_Date.Value.ToString("ddMMyyyy");
                FItem.Account = i.Account;
                FItem.Amount = i.Amount;
                total += i.Amount;
                FItem.Tax_Code = i.Tax_Code;
                FItem.Shop = i.Shop == null ? "" : i.Shop.Length == 6 ? i.Shop : i.Shop.Substring(0, 4) + i.Shop.Substring(6, 2);
                FItem.Assignment = i.Assignment;
                if (car != null)
                {
                    var brand = budget_Entities.Departments.Where(w => w.Department_ID == departmentID_STR).FirstOrDefault();
                    var Profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == i.Shop && w.Brand == brand.ShortBrand && w.Channel == i.saleon).FirstOrDefault();
                    FItem.Brand = "(" + brand.ShortBrand + ")" + brand.Department_Name;
                    if (Profit != null)
                    {
                        FItem.Profit_Center = Profit.Profit;
                        FItem.Cost_Center = Profit.Costcenter;
                    }
                    else if (i.Shop == "CENTER")
                    {
                        var center = carrier_Entities.Site_Center.Where(w => w.Brand_Center_Short == brand.ShortBrand).FirstOrDefault();
                        Profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == i.Shop && w.Brand == center.Brand_Center_Name_Full && w.Channel == i.saleon).FirstOrDefault();
                        FItem.Profit_Center = Profit.Profit;
                        FItem.Cost_Center = Profit.Costcenter;
                    }
                }
                else
                {
                    if (departmentID_STR != "")
                    {
                        var brand = budget_Entities.Departments.Where(w => w.Department_ID == departmentID_STR).FirstOrDefault();

                        FItem.Brand = "(" + brand.ShortBrand + ")" + brand.Department_Name;



                        var sitestorage = "";
                        if (i.Shop != null)
                        {
                            if (i.Shop == "CENTER" || i.Shop.Length == 8)
                            {

                                sitestorage = i.Shop;
                            }
                            else if (i.Shop.Length == 6)
                            {
                                sitestorage = i.Shop.Substring(0, 4) + brand.ShortBrand + i.Shop.Substring(4, 2);
                            }

                            var Profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == sitestorage && w.Brand == brand.ShortBrand && w.Channel == i.saleon).FirstOrDefault();
                            if (Profit != null)
                            {
                                FItem.Profit_Center = Profit.Profit;
                                FItem.Cost_Center = Profit.Costcenter;
                            }
                            else if (i.Shop == "CENTER")
                            {
                                var center = carrier_Entities.Site_Center.Where(w => w.Brand_Center_Short == brand.ShortBrand).FirstOrDefault();
                                Profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == sitestorage && w.Brand == center.Brand_Center_Name_Full && w.Channel == i.saleon).FirstOrDefault();
                                FItem.Profit_Center = Profit.Profit;
                                FItem.Cost_Center = Profit.Costcenter;
                            }
                        }


                    }
                }
                
                FItem.Docno = i.Docno;
                FItem.เลขที่เอกสารใน_FC = i.เลขที่เอกสารใน_FC;


                if (!seekDepart.Contains(departmentID_STR))
                {
                    flash_item.Add(FItem);
                }
                else
                {
                    flash_item_Seek.Add(FItem);
                }


            }
            flash_item.AddRange(flash_item_Seek);

            flash_item.Add(new modelExport_Flash
            {
                Account = "70480",
                Amount = total,
                Tax_Code = "VX"
            }) ;

            DataSet DS = new DataSet();
            DataTable SAP_V1 = new DataTable("SAP_V1");

            SAP_V1.Columns.Add(new DataColumn("Posting_Date", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Account", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Amount", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Amount_in_LC", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Tax_Base_Amount", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Tax_Code", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Bus_Area", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Baseline_Date", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Payment_Term", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Planning_Level", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Profit_Center", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Cost_Center", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Service_Cost_Center", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Order", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Shop", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Assignment", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Brand", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("Docno", typeof(string)));
            SAP_V1.Columns.Add(new DataColumn("เลขที่เอกสารใน_FC", typeof(string)));


            foreach(var i in flash_item)
            {
                DataRow rowV1 = SAP_V1.NewRow();
                rowV1[0] = i.Posting_Date;
                rowV1[1] = i.Account;
                rowV1[2] = i.Amount;
                rowV1[3] = i.Amount_in_LC;
                rowV1[4] = i.Tax_Base_Amount;
                rowV1[5] = i.Tax_Code;
                rowV1[6] = i.Bus_Area;
                rowV1[7] = i.Baseline_Date;
                rowV1[8] = i.Payment_Term;
                rowV1[9] = i.Planning_Level;
                rowV1[10] = i.Profit_Center;
                rowV1[11] = i.Cost_Center;
                rowV1[12] = i.Service_Cost_Center;
                rowV1[13] = i.Order;
                rowV1[14] = i.Shop;
                rowV1[15] = i.Assignment;
                rowV1[16] = i.Brand;
                rowV1[17] = i.Docno;
                rowV1[18] = i.เลขที่เอกสารใน_FC;

                SAP_V1.Rows.Add(rowV1);
            }
            DS.Tables.Add(SAP_V1);

            //V2


            var FlashV2 = carrier_Entities.Flash_EX_Import.Where(w => w.Date_Process >= datest && w.Date_Process <= dateed )
                .Select(s => new
                {
                    Account = "6050008",
                    Amount = s.Price ?? 0,
                    Tax_Code = "VX",
                    Shop = s.Shop.Length == 6 ? s.Shop.ToUpper() : s.Shop.ToUpper().Substring(0,4) + s.Shop.ToUpper().Substring(6,2),
                    Assignment = "Flash",
                    department_ID = s.department_id,
                    เลขที่เอกสารใน_FC = s.Docno_Budget,
                    saleon = s.saleOn,
                    docno = s.Docno
                })
                .GroupBy(g=> new
                {
                    Shop = g.Shop,
                    department_ID = g.department_ID,
                    saleon = g.saleon,
                    เลขที่เอกสารใน_FC = g.เลขที่เอกสารใน_FC,
                })
                .Select(sc => new
                {
                    Account = "6050008",
                    Amount = sc.Sum(c=>c.Amount),
                    Tax_Code = "VX",
                    Shop = sc.Key.Shop,
                    Assignment = "Flash",
                    department_ID = sc.Key.department_ID,
                    เลขที่เอกสารใน_FC = sc.Key.เลขที่เอกสารใน_FC,
                    saleon = sc.Key.saleon,
                    docno = sc.Select(c=>c.docno).ToList()
                });

            DataTable SAP_V2 = new DataTable("SAP_V2");
            SAP_V2.Columns.Add(new DataColumn("Account", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Amount", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Amount_in_LC", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Tax_Base_Amount", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Tax_Code", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Bus_Area", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Baseline_Date", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Payment_Term", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Planning_Level", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Profit_Center", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Cost_Center", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Service_Cost_Center", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Order", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Shop", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Assignment", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Brand", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("Docno", typeof(string)));
            SAP_V2.Columns.Add(new DataColumn("เลขที่เอกสารใน_FC", typeof(string)));

            flash_item = new List<modelExport_Flash>();
            flash_item_Seek = new List<modelExport_Flash>();
            total = 0;
            var brands = FlashV2.Select(s => s.department_ID).Distinct();
            foreach(var br in brands)
            {
                var budSeek = budget_Entities.Departments.Where(w => w.Department_ID == br.ToString() && w.Department_Name.StartsWith("SEEK")).FirstOrDefault();
                if(budSeek == null)
                {
                    var siteStorage = FlashV2.Where(w => w.department_ID == br).Select(s => s.Shop).Distinct();
                    foreach (var site in siteStorage)
                    {
                        var same = FlashV2.Where(w => w.department_ID == br && w.Shop == site)
                            .FirstOrDefault();

                        DataRow rowV2 = SAP_V2.NewRow();
                        rowV2[0] = same.Account;
                        rowV2[1] = same.Amount;
                        rowV2[2] = "";
                        rowV2[3] = "";
                        rowV2[4] = "VX";
                        rowV2[5] = "";
                        rowV2[6] = "";
                        rowV2[7] = "";
                        rowV2[8] = "";
                        total += same.Amount;

                        var brand = budget_Entities.Departments.Where(w => w.Department_ID == br.ToString()).FirstOrDefault();


                        var sitestorage = "";

                        if (site != null)
                        {

                            if (site == "CENTER" || site.StartsWith("ZY") )
                            {
                                sitestorage = site;
                            }
                            else if (site.Length == 6)
                            {
                                if(site.StartsWith("RX") || site.StartsWith("OP"))
                                {
                                    sitestorage = site.Substring(0, 4) + site.Substring(0,2) + site.Substring(4, 2);
                                }
                                else
                                {
                                    sitestorage = site.Substring(0, 4) + brand.ShortBrand + site.Substring(4, 2);
                                }
                                
                            }

                            var Profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == sitestorage && w.Brand == brand.ShortBrand && w.Channel == same.saleon).FirstOrDefault();
                            if (Profit != null)
                            {
                                rowV2[9] = Profit.Profit;
                                rowV2[10] = Profit.Costcenter;
                            }
                            else if (site == "CENTER")
                            {
                                var center = carrier_Entities.Site_Center.Where(w => w.Brand_Center_Short == brand.ShortBrand).FirstOrDefault();
                                Profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == sitestorage && w.Brand == center.Brand_Center_Name_Full && w.Channel == same.saleon).FirstOrDefault();

                                rowV2[9] = Profit.Profit;
                                rowV2[10] = Profit.Costcenter;
                            }
                        }
                        rowV2[11] = "";
                        rowV2[12] = "";
                        rowV2[13] = same.Shop;
                        rowV2[14] = "Flash";
                        rowV2[15] = "(" + brand.ShortBrand + ")" + brand.Department_Name;
                        rowV2[16] = Newtonsoft.Json.JsonConvert.SerializeObject(same.docno);
                        rowV2[17] = same.เลขที่เอกสารใน_FC;

                        SAP_V2.Rows.Add(rowV2);
                    }
                }
                else
                {
                    var FLashSeek = FlashV2.Where(w => w.department_ID == br).ToList();
                    foreach(var i in FLashSeek)
                    {

                        var same = FlashV2.Where(w => w.department_ID == br && w.Shop == i.Shop)
                            .FirstOrDefault();

                        modelExport_Flash FItem = new modelExport_Flash();
                        FItem.Account = i.Account;
                        FItem.Amount = i.Amount;
                        total += i.Amount;
                        FItem.Tax_Code = i.Tax_Code;
                        FItem.Shop = i.Shop == "" ? "" : i.Shop.Length == 6 ? i.Shop : i.Shop.Substring(0, 4) + i.Shop.Substring(6, 2);
                        FItem.Assignment = i.Assignment;
                        if (i.department_ID.ToString() != "")
                        {
                            var brand = budget_Entities.Departments.Where(w => w.Department_ID == i.department_ID.ToString()).FirstOrDefault();

                            FItem.Brand = "(" + brand.ShortBrand + ")" + brand.Department_Name;


                            var sitestorage = "";
                            if (i.Shop != null && i.Shop != "")
                            {
                                if (i.Shop == "CENTER" || i.Shop.StartsWith("ZY"))
                                {
                                    sitestorage = i.Shop;
                                }
                                else if (i.Shop.Length == 6)
                                {
                                    if (i.Shop.StartsWith("RX") || i.Shop.StartsWith("OP"))
                                    {
                                        sitestorage = i.Shop.Substring(0, 4) + i.Shop.Substring(0, 2) + i.Shop.Substring(4, 2);
                                    }
                                    else
                                    {
                                        sitestorage = i.Shop.Substring(0, 4) + brand.ShortBrand + i.Shop.Substring(4, 2);
                                    }
                                }

                                var Profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == sitestorage && w.Brand == brand.ShortBrand && w.Channel == i.saleon).FirstOrDefault();
                                if (Profit != null)
                                {
                                    FItem.Profit_Center = Profit.Profit;
                                    FItem.Cost_Center = Profit.Costcenter;
                                }
                                else if (i.Shop == "CENTER")
                                {
                                    var center = carrier_Entities.Site_Center.Where(w => w.Brand_Center_Short == brand.ShortBrand).FirstOrDefault();
                                    Profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == sitestorage && w.Brand == center.Brand_Center_Name_Full && w.Channel == i.saleon).FirstOrDefault();
                                    FItem.Profit_Center = Profit.Profit;
                                    FItem.Cost_Center = Profit.Costcenter;
                                }
                            }


                        }
                        FItem.Docno = Newtonsoft.Json.JsonConvert.SerializeObject(i.docno) ;
                        FItem.เลขที่เอกสารใน_FC = i.เลขที่เอกสารใน_FC;
                        flash_item_Seek.Add(FItem);
                    }
                    
                }
                
            }

            //seekLast
            foreach(var i in flash_item_Seek)
            {
                DataRow rowV2 = SAP_V2.NewRow();
                rowV2[0] = i.Account;
                rowV2[1] = i.Amount;
                rowV2[2] = i.Amount_in_LC;
                rowV2[3] = i.Tax_Base_Amount;
                rowV2[4] = i.Tax_Code;
                rowV2[5] = i.Bus_Area;
                rowV2[6] = i.Baseline_Date;
                rowV2[7] = i.Payment_Term;
                rowV2[8] = i.Planning_Level;
                rowV2[9] = i.Profit_Center;
                rowV2[10] = i.Cost_Center;
                rowV2[11] = i.Service_Cost_Center;
                rowV2[12] = i.Order;
                rowV2[13] = i.Shop;
                rowV2[14] = i.Assignment;
                rowV2[15] = i.Brand;
                rowV2[16] = i.Docno;
                rowV2[17] = i.เลขที่เอกสารใน_FC;
                SAP_V2.Rows.Add(rowV2);
            }
            DataRow rowV2_TOtal = SAP_V2.NewRow();
            rowV2_TOtal[0] = "70480";
            rowV2_TOtal[1] = total;
            rowV2_TOtal[4] = "VX";
            SAP_V2.Rows.Add(rowV2_TOtal);
            DS.Tables.Add(SAP_V2);

            return DS;


        }

        protected void btnUploadToBudget_Click(object sender, EventArgs e)
        {
            if(btnUploadToBudget.Text == "Upload to Budget")
            {
                btnUploadToBudget.Text = "Approve";
                btnRejectUploadBud.Visible = true;
                dv_DateST.Style.Add("pointer-events", "none");
                dv_DateED.Style.Add("pointer-events", "none");
                txtDateSt.Enabled = false;
                txtDateED.Enabled = false;
            }
            else if(btnUploadToBudget.Text == "Approve")
            {
                btnUploadToBudget.Text = "Upload to Budget";
                btnRejectUploadBud.Visible = false;


                //Upload Budget
                var dateSTOrigin = Convert.ToDateTime(txtDateSt.Text);
                var dateEDOrigin = Convert.ToDateTime(txtDateED.Text).AddDays(1);

                var flashIM = carrier_Entities.Flash_EX_Import.Where(w => w.Date_Process >= dateSTOrigin && w.Date_Process <= dateEDOrigin && w.Status_Budget == false)
                    .GroupBy(g=> new
                    {
                        shop = g.Shop == ""? "" :g.Shop.Length == 6 ? g.Shop : g.Shop.Substring(0,4) + g.Shop.Substring(6,2),
                        department_id = g.department_id,
                        saleon = g.saleOn
                    })
                    .Select(s=>new
                    {
                        s.Key.shop,
                        s.Key.department_id,
                        s.Key.saleon,
                    }).ToList();

                List<cuttemp> FailUpload = new List<cuttemp>();

                var brand = flashIM.Select(s => s.department_id).Distinct();


                foreach(var b in brand)
                {
                    var brand_name = budget_Entities.Departments.Where(w => w.Department_ID == b.ToString()).FirstOrDefault();
                    var site = flashIM.Where(w => w.department_id == b).ToList();
                    //Sitestorage
                    foreach(var si in site)
                    {
                        var seek = budget_Entities.Departments.Where(w => w.Department_ID == si.department_id.ToString() && w.Department_Name.StartsWith("SEEK")).FirstOrDefault();
                        if ((si.shop == "CENTER" || si.shop.StartsWith("ZY") || si.shop == "" || si.shop.StartsWith("Z6"))&& (si.shop.Length == 6 || si.shop.Length == 0))
                        {
                            var siteOff = carrier_Entities.Flash_EX_Import.Where(w => w.department_id == b && w.Shop == (si.shop == "" ? null : si.shop) && w.saleOn == si.saleon)
                                .GroupBy(g => new
                                {
                                    g.department_id,
                                    g.saleOn
                                })
                                .Select(c => new
                                {
                                    c.Key.department_id,
                                    c.Key.saleOn,
                                    price = c.Sum(v => v.Price),
                                    docno = c.Select(v => v.Docno).ToList()
                                }).ToList();
                            if(siteOff.Count() != 0)
                            {
                                foreach (var siteCenter in siteOff)
                                {
                                    cuttemp temp = new cuttemp();
                                    temp.date_use = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                                    temp.depart_id = seek == null ? b.ToString() : "1619";
                                    temp.detail_id = "5703";
                                    temp.group_id = "5";
                                    temp.head_id = "507";
                                    temp.money = Convert.ToDouble(siteCenter.price);
                                    temp.remark = "ค่ารถจัดส่ง Auto จากระบบ Courier Flash รอบ " + txtDateSt.Text + " - " + txtDateED.Text + " เลขที่เอกสาร :" + Newtonsoft.Json.JsonConvert.SerializeObject(siteCenter.docno) + " Site:" + si.shop;
                                    temp.typeBudget_id = siteCenter.saleOn == "OFFLINE" ? "2" : "1";
                                    temp.userId = "101974";
                                    temp.site_storage = si.shop;

                                    var budHave = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark)).FirstOrDefault();
                                    if (budHave == null)
                                    {
                                        var ss = service_Budget.Insert_CutBudget(temp);

                                        if (ss == "สำเร็จ")
                                        {
                                            foreach (var docno in siteCenter.docno)
                                            {
                                                var depInt = seek == null ? b.ToString() : "1619";
                                                var typeBud = siteCenter.saleOn == "OFFLINE" ? 2 : 1;
                                                var budget = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark) && w.Department_ID == depInt && w.TypeBudget_ID == typeBud).FirstOrDefault();

                                                var carFlashImport = carrier_Entities.Flash_EX_Import.Where(w => w.Docno == docno).FirstOrDefault();
                                                carFlashImport.Docno_Budget = budget.Docno;
                                                carFlashImport.Status_Budget = true;
                                                carrier_Entities.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            FailUpload.Add(temp);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if(si.shop != "")
                                {
                                    var ShopAndBrand = si.shop.Substring(0, 4) + brand_name.ShortBrand + si.shop.Substring(4, 2);
                                }

                                siteOff = carrier_Entities.Flash_EX_Import.Where(w => w.department_id == b && w.Shop == (si.shop == ""? null : si.shop.Substring(0, 4) + brand_name.ShortBrand + si.shop.Substring(4, 2))  && w.saleOn == si.saleon)
                                .GroupBy(g => new
                                {
                                    g.department_id,
                                    g.saleOn
                                })
                                .Select(c => new
                                {
                                    c.Key.department_id,
                                    c.Key.saleOn,
                                    price = c.Sum(v => v.Price),
                                    docno = c.Select(v => v.Docno).ToList()
                                }).ToList();
                                foreach (var siteCenter in siteOff)
                                {
                                    cuttemp temp = new cuttemp();
                                    temp.date_use = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                                    temp.depart_id = seek == null ? b.ToString() : "1619";
                                    temp.detail_id = "5703";
                                    temp.group_id = "5";
                                    temp.head_id = "507";
                                    temp.money = Convert.ToDouble(siteCenter.price);
                                    temp.remark = "ค่ารถจัดส่ง Auto จากระบบ Courier Flash รอบ " + txtDateSt.Text + " - " + txtDateED.Text + " เลขที่เอกสาร :" + Newtonsoft.Json.JsonConvert.SerializeObject(siteCenter.docno) + " Site:" + si.shop;
                                    temp.typeBudget_id = siteCenter.saleOn == "OFFLINE" ? "2" : "1";
                                    temp.userId = "101974";
                                    temp.site_storage = si.shop;

                                    var budHave = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark)).FirstOrDefault();
                                    if (budHave == null)
                                    {
                                        var ss = service_Budget.Insert_CutBudget(temp);

                                        if (ss == "สำเร็จ")
                                        {
                                            foreach (var docno in siteCenter.docno)
                                            {
                                                var depInt = seek == null ? b.ToString() : "1619";
                                                var typeBud = siteCenter.saleOn == "OFFLINE" ? 2 : 1;
                                                var budget = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark) && w.Department_ID == depInt && w.TypeBudget_ID == typeBud).FirstOrDefault();

                                                var carFlashImport = carrier_Entities.Flash_EX_Import.Where(w => w.Docno == docno).FirstOrDefault();
                                                carFlashImport.Docno_Budget = budget.Docno;
                                                carFlashImport.Status_Budget = true;
                                                carrier_Entities.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            FailUpload.Add(temp);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var docno in siteCenter.docno)
                                        {
                                            var depInt = seek == null ? b.ToString() : "1619";
                                            var typeBud = siteCenter.saleOn == "OFFLINE" ? 2 : 1;
                                            var budget = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark) && w.Department_ID == depInt && w.TypeBudget_ID == typeBud).FirstOrDefault();

                                            var carFlashImport = carrier_Entities.Flash_EX_Import.Where(w => w.Docno == docno).FirstOrDefault();
                                            carFlashImport.Docno_Budget = budget.Docno;
                                            carFlashImport.Status_Budget = true;
                                            carrier_Entities.SaveChanges();
                                        }
                                    }
                                }
                            }
                            
                        }
                        else
                        {
                            var siteST = si.shop.Substring(0, 4);
                            var siteED = si.shop.Substring(4, 2);
                            var siteOrder = carrier_Entities.Flash_EX_Import.Where(w => w.department_id == b && w.Shop.StartsWith(siteST) && w.Shop.EndsWith(siteED) && w.saleOn == si.saleon)
                                .GroupBy(g => new
                                {
                                    g.department_id,
                                    g.saleOn
                                })
                                .Select(c=>new
                                {
                                    c.Key.department_id,
                                    c.Key.saleOn,
                                    price = c.Sum(v=>v.Price),
                                    docno = c.Select(v=>v.Docno).ToList()
                                }).ToList();

                            foreach(var saleonInSite in siteOrder)
                            {
                                cuttemp temp = new cuttemp();
                                temp.date_use = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                                temp.depart_id = seek == null ? b.ToString() : "1619";
                                temp.detail_id = "5703";
                                temp.group_id = "5";
                                temp.head_id = "507";
                                temp.money = Convert.ToDouble(saleonInSite.price);
                                temp.remark = "ค่ารถจัดส่ง Auto จากระบบ Courier Flash รอบ " + txtDateSt.Text + " - " + txtDateED.Text + " เลขที่เอกสาร :" + Newtonsoft.Json.JsonConvert.SerializeObject(saleonInSite.docno) + " Site:" + si.shop;
                                temp.typeBudget_id = saleonInSite.saleOn == "OFFLINE" ? "2" : "1";
                                temp.userId = "101974";
                                temp.site_storage = siteST + brand_name.ShortBrand + siteED;  

                                var budHave = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark)).FirstOrDefault();
                                if(budHave == null)
                                {
                                    var ss = service_Budget.Insert_CutBudget(temp);

                                    if (ss == "สำเร็จ")
                                    {
                                        foreach (var docno in saleonInSite.docno)
                                        {
                                            var depInt = seek == null ? b.ToString() : "1619";
                                            var typeBud = saleonInSite.saleOn == "OFFLINE" ? 2 : 1;
                                            var budget = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark) && w.Department_ID == depInt && w.TypeBudget_ID == typeBud).FirstOrDefault();

                                            var carFlashImport = carrier_Entities.Flash_EX_Import.Where(w => w.Docno == docno).FirstOrDefault();
                                            carFlashImport.Docno_Budget = budget.Docno;
                                            carFlashImport.Status_Budget = true;
                                            carrier_Entities.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        FailUpload.Add(temp);
                                    }
                                }
                                else
                                {
                                    foreach (var docno in saleonInSite.docno)
                                    {
                                        var depInt = seek == null ? b.ToString() : "1619";
                                        var typeBud = saleonInSite.saleOn == "OFFLINE" ? 2 : 1;
                                        var budget = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark) && w.Department_ID == depInt && w.TypeBudget_ID == typeBud).FirstOrDefault();

                                        var carFlashImport = carrier_Entities.Flash_EX_Import.Where(w => w.Docno == docno).FirstOrDefault();
                                        carFlashImport.Docno_Budget = budget.Docno;
                                        carFlashImport.Status_Budget = true;
                                        carrier_Entities.SaveChanges();
                                    }
                                }
                                
                            }

                        }

                    }


                   
                }

                if(FailUpload.Count() != 0)
                {
                    var Site = "";
                    foreach(var fu in FailUpload)
                    {
                        if(fu == FailUpload.Last())
                        {
                            Site += fu.site_storage;
                        }
                        else
                        {
                            Site += fu.site_storage + ",";
                        }
                    }

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกสำเร็จ แต่ยังมีบางส่วนที่ไม่สามารถบันทึกได้ "+Site+"')", true);

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกสำเร็จ')", true);

                }
                dv_DateST.Style.Remove("pointer-events");
                dv_DateED.Style.Remove("pointer-events");
                txtDateSt.Enabled = true;
                txtDateED.Enabled = true;
            }
        }

        protected void btnRejectUploadBud_Click(object sender, EventArgs e)
        {
            btnUploadToBudget.Text = "Upload to Budget";
            btnRejectUploadBud.Visible = false;
            dv_DateST.Style.Remove("pointer-events");
            dv_DateED.Style.Remove("pointer-events");
            txtDateSt.Enabled = true;
            txtDateED.Enabled = true;
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

    public class model_FailUpload
    {
        public string department_id { get; set; }
        public string site { get; set; }

    }

}