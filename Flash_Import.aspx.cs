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
using Carrier.Model.Budget_2025;
using Carrier.Model.Whale;
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Online_NonAPI;
using Carrier.Model.Online_Lazada;
using Carrier.Model.SFG;
using Carrier.Model.Ecommerce;
using Carrier.Model.BC_TB;
using Carrier.Service;
using static Carrier.Service.Service_Whale;
using ClosedXML.Excel;

namespace Carrier
{
    public partial class Flash_Import : System.Web.UI.Page
    {
        CarrierEntities carrier_Entities = new CarrierEntities();
        BudgetEntities budget_Entities = new BudgetEntities();
        Budget_2025Entities budget_2025_Entities = new Budget_2025Entities();
        WhaleEntities whale_Entities = new WhaleEntities();
        Online_NonAPIEntities entities_Online_NonAPI = new Online_NonAPIEntities();
        Online_LazadaEntities online_Lazada_Entities = new Online_LazadaEntities();
        InsideSFG_WFEntities insideSFG_WF_Entities = new InsideSFG_WFEntities();
        SFGEntities sFG_Entities = new SFGEntities();
        ECommerceEntities eCommerce_Entities = new ECommerceEntities();
        BC_TBEntities bC_TB_Entities = new BC_TBEntities();
        

        Service_Flash service_Flash = new Service_Flash();
        Service_Whale service_Whale = new Service_Whale();
        Service_Budget service_Budget = new Service_Budget();
        Service_BC service_BC = new Service_BC();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDateSt.Text = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                txtDateED.Text = DateTime.Now.ToString("dd/MM/yyyy");
                
            }
            
        }
        public void FixedExpense()
        {
            var exxpense = budget_Entities.MainExpenses.Where(w => w.Docno.StartsWith("UP") && w.Remark.StartsWith("ค่ารถจัดส่ง Auto จากระบบ Courier Flash รอบ 01/09/2024 - 30/09/2024")).ToList();
            foreach(var ex in exxpense)
            {
                //var carHave = carrier_Entities.Flash_EX_Import.Where(w => w.Docno_Budget == ex.Docno).ToList();
                //foreach(var ca in carHave)
                //{
                //    var budDouble = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(ca.Docno)).ToList();
                //    if(budDouble.Count() == 2)
                //    {
                //        var up = carrier_Entities.Flash_EX_Import.Where(w => w.Docno == ca.Docno).FirstOrDefault();
                //        up.Docno_Budget = budDouble.OrderBy(o => o.Date_Send).FirstOrDefault().Docno;
                //        carrier_Entities.SaveChanges();
                //    }
                //    else if(budDouble.Count() == 1)
                //    {
                //        var up = carrier_Entities.Flash_EX_Import.Where(w => w.Docno == ca.Docno).FirstOrDefault();
                //        up.Docno_Budget = null;
                //        up.Status_Budget = false;
                //        carrier_Entities.SaveChanges();
                //    }
                //}


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
                                                var docno = item[1].ToString().Trim();
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
                                                    flash.Docno = docno;
                                                    flash.pno = pno;
                                                    flash.Price = Convert.ToDouble(item[22].ToString());
                                                    flash.Status_Budget = false;

                                                    var dataTOCheck = new model_GV_Check();



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

                                                        #region New
                                                        switch (flash.Docno.StartsWith("SO") ? "SO" : flash.Docno.StartsWith("OPC") ? "OPC" : int.TryParse(flash.Docno.Substring(0,1), out _) ? "Shopify" : "")
                                                        {
                                                            case "SO":
                                                                var whaleOrder = Get_Order_Whale(docno).FirstOrDefault();

                                                                if (whaleOrder != null)
                                                                {
                                                                    var departTrue = "";
                                                                    var depart = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.BRANDABB == whaleOrder.Brand_Short).FirstOrDefault();
                                                                    if (whaleOrder.Customer_Code != null)
                                                                    {
                                                                        if (whaleOrder.Customer_Code.Length == 6)
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
                                                                    if (new string[] { "ZX", "Z6" }.Contains(flash.Shop.Substring(0, 2)))
                                                                    {
                                                                        flash.saleOn = "ONLINE";
                                                                    }
                                                                    else
                                                                    {
                                                                        flash.saleOn = "OFFLINE";
                                                                    }
                                                                }

                                                                break;
                                                            case "OPC":
                                                                var sap = (from vbrk in sFG_Entities.SAP_VBRK_NEWSAP
                                                                           join vbrp in sFG_Entities.SAP_VBRP_NEWSAP on vbrk.VBELN equals vbrp.VBELN
                                                                           where vbrk.Refdoc == flash.Docno
                                                                           select new
                                                                           {
                                                                               vbrk.Refdoc,
                                                                               vbrk.KUNRG,
                                                                               vbrp.BRAND
                                                                           }).FirstOrDefault();
                                                                if (sap != null)
                                                                {
                                                                    var budget = budget_Entities.Departments.Where(w => w.ShortBrand == sap.BRAND).FirstOrDefault();
                                                                    if (budget != null)
                                                                    {
                                                                        dataTOCheck.sitestorage = sap.KUNRG.Substring(0, 4) + sap.BRAND + sap.KUNRG.Substring(4, 2);
                                                                        dataTOCheck.From = "Ecommerce";
                                                                        dataTOCheck.Department_ID = budget.Department_ID;
                                                                        flash.Shop = sap.KUNRG.Substring(0, 4) + sap.BRAND + sap.KUNRG.Substring(4, 2);
                                                                        flash.department_id = Convert.ToInt32(budget.Department_ID);
                                                                        flash.saleOn = "ONLINE";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    var ecom = (from i in eCommerce_Entities.Form_Orderitem
                                                                                join c in eCommerce_Entities.Channels on i.Channel_ID equals c.Channel_ID
                                                                                where i.Docno == flash.Docno
                                                                                select new
                                                                                {
                                                                                    c.PrefixChannel,
                                                                                    brand = i.SKU.Substring(0, 2)
                                                                                }).FirstOrDefault();
                                                                    var budget = budget_Entities.Departments.Where(w => w.ShortBrand == ecom.brand).FirstOrDefault();
                                                                    if (budget != null)
                                                                    {
                                                                        dataTOCheck.sitestorage = ecom.PrefixChannel;
                                                                        dataTOCheck.From = "Ecommerce";
                                                                        dataTOCheck.Department_ID = budget.Department_ID;
                                                                        flash.Shop = ecom.PrefixChannel;
                                                                        flash.department_id = Convert.ToInt32(budget.Department_ID);
                                                                        flash.saleOn = "ONLINE";
                                                                    }
                                                                }

                                                                break;
                                                            case "Shopify":
                                                                var shopify = online_Lazada_Entities.API_Shopify_GetOrders.Where(w => w.ShopifyOrderID == flash.Docno).FirstOrDefault();
                                                                var whaleOrders = Get_Order_Whale(shopify.Docno).FirstOrDefault();

                                                                if (whaleOrders != null)
                                                                {
                                                                    var departTrue = "";
                                                                    var depart = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.BRANDABB == whaleOrders.Brand_Short).FirstOrDefault();
                                                                    if (whaleOrders.Customer_Code != null)
                                                                    {
                                                                        if (whaleOrders.Customer_Code.Length == 6)
                                                                        {
                                                                            if (whaleOrders.Customer_Code.StartsWith("ZY") || whaleOrders.Customer_Code == "CENTER")
                                                                            {
                                                                                flash.Shop = whaleOrders.Customer_Code;
                                                                            }
                                                                            else
                                                                            {
                                                                                flash.Shop = whaleOrders.Customer_Code.Substring(0, 4) + whaleOrders.Brand_Short + whaleOrders.Customer_Code.Substring(4, 2);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            flash.Shop = whaleOrders.Customer_Code;
                                                                        }
                                                                    }



                                                                    if (depart == null)
                                                                    {
                                                                        departTrue = budget_Entities.Departments.Where(w => w.ShortBrand == whaleOrders.Brand_Short).FirstOrDefault().Department_ID;
                                                                    }
                                                                    else
                                                                    {
                                                                        departTrue = depart.departmentID;
                                                                    }
                                                                    flash.department_id = Convert.ToInt32(departTrue);
                                                                    shop = whaleOrders.Customer_Code;
                                                                    dateprocess = whaleOrders.Date_Send ?? DateTime.Now;
                                                                    dataTOCheck.sitestorage = flash.Shop;
                                                                    dataTOCheck.From = "Shopify";
                                                                    dataTOCheck.Department_ID = departTrue;
                                                                    flash.saleOn = "ONLINE";

                                                                }
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                        #endregion

                                                        #region Old
                                                        //if (whaleOrder != null)
                                                        //{
                                                        //    var departTrue = "";
                                                        //    var depart = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.BRANDABB == whaleOrder.Brand_Short).FirstOrDefault();
                                                        //    if (whaleOrder.Customer_Code != null)
                                                        //    {
                                                        //        if (whaleOrder.Customer_Code.Length == 6)
                                                        //        {
                                                        //            if (whaleOrder.Customer_Code.StartsWith("ZY") || whaleOrder.Customer_Code == "CENTER")
                                                        //            {
                                                        //                flash.Shop = whaleOrder.Customer_Code;
                                                        //            }
                                                        //            else
                                                        //            {
                                                        //                flash.Shop = whaleOrder.Customer_Code.Substring(0, 4) + whaleOrder.Brand_Short + whaleOrder.Customer_Code.Substring(4, 2);
                                                        //            }
                                                        //        }
                                                        //        else
                                                        //        {
                                                        //            flash.Shop = whaleOrder.Customer_Code;
                                                        //        }
                                                        //    }



                                                        //    if (depart == null)
                                                        //    {
                                                        //        departTrue = budget_Entities.Departments.Where(w => w.ShortBrand == whaleOrder.Brand_Short).FirstOrDefault().Department_ID;
                                                        //    }
                                                        //    else
                                                        //    {
                                                        //        departTrue = depart.departmentID;
                                                        //    }
                                                        //    flash.department_id = Convert.ToInt32(departTrue);
                                                        //    shop = whaleOrder.Customer_Code;
                                                        //    dateprocess = whaleOrder.Date_Send ?? DateTime.Now;
                                                        //    dataTOCheck.sitestorage = flash.Shop;
                                                        //    dataTOCheck.From = "Whale";
                                                        //    dataTOCheck.Department_ID = departTrue;
                                                        //    flash.saleOn = "ONLINE";
                                                        //}
                                                        //else
                                                        //{
                                                        //    var sap = (from vbrk in sFG_Entities.SAP_VBRK_NEWSAP
                                                        //               join vbrp in sFG_Entities.SAP_VBRP_NEWSAP on vbrk.VBELN equals vbrp.VBELN
                                                        //               where vbrk.Refdoc == flash.Docno
                                                        //               select new
                                                        //               {
                                                        //                   vbrk.Refdoc,
                                                        //                   vbrk.KUNRG,
                                                        //                   vbrp.BRAND
                                                        //               }).FirstOrDefault();
                                                        //    if (sap != null)
                                                        //    {
                                                        //        var budget = budget_Entities.Departments.Where(w => w.ShortBrand == sap.BRAND).FirstOrDefault();
                                                        //        if (budget != null)
                                                        //        {
                                                        //            dataTOCheck.sitestorage = sap.KUNRG.Substring(0, 4) + sap.BRAND + sap.KUNRG.Substring(4, 2);
                                                        //            dataTOCheck.From = "Ecommerce";
                                                        //            dataTOCheck.Department_ID = budget.Department_ID;
                                                        //            flash.Shop = sap.KUNRG.Substring(0, 4) + sap.BRAND + sap.KUNRG.Substring(4, 2);
                                                        //            flash.department_id = Convert.ToInt32(budget.Department_ID);
                                                        //        }
                                                        //    }
                                                        //    else
                                                        //    {
                                                        //        flash.Shop = "";
                                                        //    }
                                                        //    if (flash.Shop != "" && flash.Shop != null)
                                                        //    {
                                                        //        if (new string[] { "ZX", "Z6" }.Contains(flash.Shop.Substring(0, 2)))
                                                        //        {
                                                        //            flash.saleOn = "ONLINE";
                                                        //        }
                                                        //        else
                                                        //        {
                                                        //            flash.saleOn = "OFFLINE";
                                                        //        }
                                                        //    }
                                                        //    else
                                                        //    {
                                                        //        flash.saleOn = "OFFLINE";
                                                        //    }

                                                        //}

                                                        #endregion

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
            var flash = carrier_Entities.Flash_EX_Import.Where(w => w.Date_Process >= datest && w.Date_Process <= dateed && (w.pno == txtPno.Text || txtPno.Text == ""))
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
                                                             //&& (tOrder.b_IDCard == "" || tOrder.b_IDCard == null ? !tCustomer.SAP_Channel.EndsWith("_ETAX") : tCustomer.SAP_Channel.EndsWith("_ETAX"))
                                                             && !tCustomer.SAP_Channel.EndsWith("_ETAX")
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


                var filename = "BC_Flash_" + dateST.ToString("dd-MM-yyyy") + "_" + dateED.ToString("dd-MM-yyyy") /*DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")*/ + ".xls";

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
                    Amount = s.Price??0,
                    Shop = s.Shop.ToUpper(),
                    department_ID = s.department_id,
                    Docno = s.Docno,
                    เลขที่เอกสารใน_FC = s.Docno_Budget,
                    saleon = s.saleOn,
                    pno = s.pno

                }).OrderBy(o=>o.Posting_Date);

            if(Flash == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่พบข้อมูลในช่วงเวลาที่เลือกครับ')", true);
                return null;
            }
            //V1
            List<model_Flash_Export_BC> flash_item = new List<model_Flash_Export_BC>();
            List<model_Flash_Export_BC> flash_item_Seek = new List<model_Flash_Export_BC>();
            double total = 0;
            var brandBC = service_BC.getDimensionValue("BRAND_PROFIT CENTER");
            var costcenterBC = service_BC.getDimensionValue("COST CENTER");
            foreach (var i in Flash)
            {
                var departmentID_STR = i.department_ID.ToString();
                var seekDepart = budget_Entities.Departments.Where(w => w.Department_Name.StartsWith("SEEK")).Select(s => s.Department_ID).ToList();

                var car = carrier_Entities.Orders.Where(w => w.Docno == i.Docno).FirstOrDefault();
                model_Flash_Export_BC FItem = new model_Flash_Export_BC();

                var brandMatch = brandBC.Where(w => w.DepartmentID == departmentID_STR).FirstOrDefault();
                FItem.Brand_Profit_center_Code = brandMatch == null ? "CENTER" : brandMatch.Dimension_Value_Code;
                //Option
                if (brandMatch == null)
                {

                    var departmentCEnter = costcenterBC.Where(w => w.DepartmentID == departmentID_STR).FirstOrDefault();
                    FItem.Cost_center_Code = departmentCEnter == null ? "SALES (XXX110)" : departmentCEnter.Dimension_Value_Code;
                    FItem.Brand_Profit_center_Code = departmentCEnter == null ? "CENTER" : "SUPPORT 5020";
                }
                else
                {
                    if (brandMatch.Dimension_Value_Code == "BRATPACK SHOP 5030")
                    {
                        var site2DIgit = i.Shop.Substring(2, 2);
                        var departmentCEnter = costcenterBC.Where(w => w.Site == site2DIgit).FirstOrDefault();
                        if (departmentCEnter != null)
                        {
                            FItem.Cost_center_Code = departmentCEnter.Dimension_Value_Code;
                            FItem.Brand_Profit_center_Code = brandMatch.Dimension_Value_Code;
                        }
                        else
                        {
                            FItem.Cost_center_Code = "SALES (XXX110)";
                            FItem.Brand_Profit_center_Code = brandMatch.Dimension_Value_Code;
                        }

                    }
                    else
                    {
                        FItem.Cost_center_Code = "SALES (XXX110)";
                        FItem.Brand_Profit_center_Code = brandMatch.Dimension_Value_Code;
                    }
                }


                FItem.เลขที่เอกสารใน_FC = i.เลขที่เอกสารใน_FC;
                FItem.Direct_Unit_Cost_Excl_VAT = i.Amount.ToString("#,##0.00");
                FItem.Line_Amount_Excl_VAT = i.Amount.ToString("#,##0.00");

                var shop = i.Shop.Length == 8 ? i.Shop.Substring(0, 4) + i.Shop.Substring(6, 2) : i.Shop;
                var convertSite = bC_TB_Entities.SiteSAP_BC.Where(w => w.SiteSAP == shop).FirstOrDefault();
                if (convertSite != null)
                {
                    FItem.Site_shop_Code = convertSite.SiteBC;
                }
                else
                {
                    FItem.Site_shop_Code = shop;
                }
                
                FItem.Description_Comment = i.Shop + "_" + i.pno + "_" + "ค่าขนส่ง_ค่าพาหนะเฉพาะจัดส่ง_M." + (i.Posting_Date?? DateTime.Now).ToString("MM/yy");

                FItem.Chanel_Code = i.saleon;


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


            DataSet DS = new DataSet();
            DataTable BC_V1 = new DataTable("BC_V1");

            BC_V1.Columns.Add(new DataColumn("type", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("No", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("ItemReference_No", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Description_Comment", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Description2", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Attached_to_Subscription_Contract_line", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Location_Code", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Gen_Bus_Posting_Group", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Gen_Prod_Posting_Group", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("VAT_Bus_Posting_Group", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("VAT_Prod_Posting_Group", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("WHT_Business_Posting_Group", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("WHT_Product_Posting_Group", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Sustainability_Account_No", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Energy_Source_Code", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Quantity", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Unit_of_Measure_Code", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Direct_Unit_Cost_Excl_VAT", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Line_Discount_Percent", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Line_Amount_Excl_VAT", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Qty_to_Assign", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Qty_Assigned", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Renewable_Energy", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Emission_CO2", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Emission_CH4", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Emission_N2O", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Source_of_Emission_data", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Emission_Verified", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("CBAM", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Brand_Profit_center_Code", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Cost_center_Code", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Site_shop_Code", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Chanel_Code", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Io_Code", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Business_area_Code", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Tax_Invoice_Date", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Tax_Invoice_No", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Tax_Vendor_No", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Tax_Invoice_Name", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Tax_Invoice_Base", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Tax_Head_Office", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("VAT_Branch_Code", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("Vat_Registration_No", typeof(string)));
            BC_V1.Columns.Add(new DataColumn("เลขที่เอกสารใน_FC", typeof(string)));


            foreach(var i in flash_item)
            {
                DataRow rowV1 = BC_V1.NewRow();
                rowV1[0] = "G/L Account";
                rowV1[1] = "6050008";
                rowV1[2] = "";
                rowV1[3] = i.Description_Comment;
                rowV1[4] = "";
                rowV1[5] = "No";
                rowV1[6] = "";
                rowV1[7] = "EXPENSE";
                rowV1[8] = "GL";
                rowV1[9] = "VATHO";
                rowV1[10] = "NOVAT";
                rowV1[11] = "WHT53";
                rowV1[12] = "TRANSPORT";
                rowV1[13] = "";
                rowV1[14] = "";
                rowV1[15] = "1";
                rowV1[16] = "";
                rowV1[17] = i.Direct_Unit_Cost_Excl_VAT;
                rowV1[18] = "";
                rowV1[19] = i.Line_Amount_Excl_VAT;
                rowV1[20] = "0";
                rowV1[21] = "";
                rowV1[22] = "NO";
                rowV1[23] = "0";
                rowV1[24] = "0";
                rowV1[25] = "0";
                rowV1[26] = "";
                rowV1[27] = "NO";
                rowV1[28] = "NO";
                rowV1[29] = i.Brand_Profit_center_Code;
                rowV1[30] = i.Cost_center_Code;
                rowV1[31] = i.Site_shop_Code;
                rowV1[32] = i.Chanel_Code;
                rowV1[33] = "NONE";
                rowV1[34] = "BA1000";
                rowV1[35] = "";
                rowV1[36] = "";
                rowV1[37] = "";
                rowV1[38] = "";
                rowV1[39] = "0";
                rowV1[40] = "NO";
                rowV1[41] = "";
                rowV1[42] = "";
                rowV1[43] = i.เลขที่เอกสารใน_FC;

                BC_V1.Rows.Add(rowV1);
            }
            DS.Tables.Add(BC_V1);

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
                    var brandid = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.departmentID == b.ToString()).FirstOrDefault();
                    //Sitestorage
                    foreach(var si in site)
                    {
                        var shop = "";
                        var convertSite = bC_TB_Entities.SiteSAP_BC.Where(w => w.SiteSAP == si.shop).FirstOrDefault();
                        if (convertSite != null)
                        {
                            shop = convertSite.SiteBC;
                        }
                        else
                        {
                            shop = si.shop;
                        }


                        var seek = budget_Entities.Departments.Where(w => w.Department_ID == si.department_id.ToString() && w.Department_Name.StartsWith("SEEK")).FirstOrDefault();
                        if ((si.shop == "CENTER" || si.shop.StartsWith("ZY") || si.shop == "" || si.shop.StartsWith("Z6"))&& (si.shop.Length == 6 || si.shop.Length == 0))
                        {
                            var siteOff = carrier_Entities.Flash_EX_Import.Where(w => w.department_id == b && w.Shop == (si.shop == "" ? null : si.shop) && w.saleOn == si.saleon && w.Date_Process >= dateSTOrigin && w.Date_Process <= dateEDOrigin && w.Status_Budget == false)
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
                                        if(temp.depart_id != "")
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

                                //    siteOff = carrier_Entities.Flash_EX_Import.Where(w => w.department_id == b && w.Shop == (si.shop == "" ? "" : si.shop.Substring(0, 4) + (brand_name == null ? "" : brand_name.ShortBrand) + si.shop.Substring(4, 2)) && w.saleOn == si.saleon && w.Date_Process >= dateSTOrigin && w.Date_Process <= dateEDOrigin && w.Status_Budget == false)
                                //.GroupBy(g => new
                                //{
                                //    g.department_id,
                                //    g.saleOn
                                //})
                                //.Select(c => new
                                //{
                                //    c.Key.department_id,
                                //    c.Key.saleOn,
                                //    price = c.Sum(v => v.Price),
                                //    docno = c.Select(v => v.Docno).ToList()
                                //}).ToList();
                                var Shop_For_query = si.shop == "" ? "" : si.shop.Substring(0, 4) + (brand_name == null ? "" : brand_name.ShortBrand) + si.shop.Substring(4, 2);
                                siteOff = carrier_Entities.Flash_EX_Import.Where(w => w.department_id == b && w.Shop == Shop_For_query && w.saleOn == si.saleon && w.Date_Process >= dateSTOrigin && w.Date_Process <= dateEDOrigin && w.Status_Budget == false)
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
                                        if (temp.depart_id != "")
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
                            var siteOrder = carrier_Entities.Flash_EX_Import.Where(w => w.department_id == b && w.Shop.StartsWith(siteST) && w.Shop.EndsWith(siteED) && w.saleOn == si.saleon && w.Date_Process >= dateSTOrigin && w.Date_Process <= dateEDOrigin && w.Status_Budget == false)
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
                                temp.remark = "ค่ารถจัดส่ง Auto จากระบบ Courier Flash รอบ " + txtDateSt.Text + " - " + txtDateED.Text + " เลขที่เอกสาร :" + Newtonsoft.Json.JsonConvert.SerializeObject(saleonInSite.docno) + " Site:" + shop;
                                temp.typeBudget_id = saleonInSite.saleOn == "OFFLINE" ? "2" : "1";
                                temp.userId = "101974";
                                temp.site_storage = siteST + brand_name.ShortBrand + siteED;  

                                var budHave = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark)).FirstOrDefault();
                                if(budHave == null)
                                {

                                    var io = budget_2025_Entities.Department_IO_2025.Where(w => w.SiteStorage.Contains(shop) && w.Action_Start <= DateTime.Now && w.Action_End >= DateTime.Now && w.Status_IO == "Y").FirstOrDefault();
                                    if(io != null)
                                    {
                                        var ioBrand = budget_2025_Entities.Department_IO_2025_Brand.Where(w => w.Department_IO_ID == io.Department_IO_ID && w.dateST <= DateTime.Now && w.dateED >= DateTime.Now);
                                        temp.depart_id = io.Department_IO_ID;
                                        temp.group_id = "13";
                                        temp.head_id = "1324";
                                        temp.detail_id = "132404";
                                        foreach (var bio in ioBrand)
                                        {
                                            if(bio.Brand_ID == brandid.ID_Brand)
                                            {
                                                
                                                temp.brand.Add(new cutCudget_brand_Filter
                                                {
                                                    brand_id = bio.Brand_ID,
                                                    brand_percent = 100,
                                                    SiteStorage = bio.Site_Storage_B
                                                });
                                            }
                                            else
                                            {
                                                temp.brand.Add(new cutCudget_brand_Filter
                                                {
                                                    brand_id = bio.Brand_ID,
                                                    brand_percent = 0,
                                                    SiteStorage = bio.Site_Storage_B
                                                });
                                            }
                                        }
                                        if(temp.brand.Where(w=>w.brand_percent == 100).FirstOrDefault() == null)
                                        {
                                            temp.brand = new List<cutCudget_brand_Filter>();
                                            temp.detail_id = "5703";
                                            temp.group_id = "5";
                                            temp.head_id = "507";
                                            temp.depart_id = seek == null ? b.ToString() : "1619";
                                        }
                                    }
                                    var ss = service_Budget.Insert_CutBudget(temp);

                                    if (ss == "สำเร็จ")
                                    {
                                        foreach (var docno in saleonInSite.docno)
                                        {
                                            var depInt = seek == null ? b.ToString() : "1619";
                                            var typeBud = saleonInSite.saleOn == "OFFLINE" ? 2 : 1;
                                            var budget = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark) && w.Department_ID == temp.depart_id && w.TypeBudget_ID == typeBud).FirstOrDefault();

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
                                        var budget = budget_Entities.MainExpenses.Where(w => w.Remark.Contains(temp.remark) && w.Department_ID == temp.depart_id && w.TypeBudget_ID == typeBud).FirstOrDefault();

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
                            Site += fu.site_storage ?? "";
                        }
                        else
                        {
                            Site += (fu.site_storage??"") + ",";
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

    public class model_Flash_Export_BC
    {
        public string type { get; set; }
        public string No { get; set; }
        public string ItemReference_No { get; set; }
        public string Description_Comment { get; set; }
        public string Description2 { get; set; }
        public string Attached_to_Subscription_Contract_line { get; set; }
        public string Location_Code { get; set; }
        public string Gen_Bus_Posting_Group { get; set; }
        public string Gen_Prod_Posting_Group { get; set; }
        public string VAT_Bus_Posting_Group { get; set; }
        public string VAT_Prod_Posting_Group { get; set; }
        public string WHT_Business_Posting_Group { get; set; }
        public string WHT_Product_Posting_Group { get; set; }
        public string Sustainability_Account_No { get; set; }
        public string Quantity { get; set; }
        public string Unit_of_Measure_Code { get; set; }
        public string Direct_Unit_Cost_Excl_VAT { get; set; }
        public string Line_Discount_Percent { get; set; }
        public string Line_Amount_Excl_VAT { get; set; }
        public string Qty_to_Assign { get; set; }
        public string Qty_Assigned { get; set; }
        public string Emission_CO2 { get; set; }
        public string Emission_CH4 { get; set; }
        public string Emission_N2O { get; set; }
        public string Brand_Profit_center_Code { get; set; }
        public string Cost_center_Code { get; set; }
        public string Site_shop_Code { get; set; }
        public string Chanel_Code { get; set; }
        public string Io_Code { get; set; }
        public string Business_area_Code { get; set; }
        public string Tax_Invoice_Date { get; set; }
        public string Tax_Invoice_No { get; set; }
        public string Tax_Vendor_No { get; set; }
        public string Tax_Invoice_Name { get; set; }
        public string Tax_Invoice_Base { get; set; }
        public string Tax_Head_Office { get; set; }
        public string VAT_Branch_Code { get; set; }
        public string Vat_Registration_No { get; set; }
        public string เลขที่เอกสารใน_FC { get; set; }


    }

    public class model_FailUpload
    {
        public string department_id { get; set; }
        public string site { get; set; }

    }

}