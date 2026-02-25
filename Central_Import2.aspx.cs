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
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Budget;
using Carrier.Model.BC_TB;
using Carrier.Service;

namespace Carrier
{
    public partial class Central_Import2 : System.Web.UI.Page
    {
        CarrierEntities carrier_Entities = new CarrierEntities();
        InsideSFG_WFEntities insideSFG_WF_Entities = new InsideSFG_WFEntities();
        BudgetEntities budget_Entities = new BudgetEntities();
        BC_TBEntities bC_TB_Entities = new BC_TBEntities();

        Service_Budget service_Budget = new Service_Budget();
        Service_BC service_BC = new Service_BC();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDateSt.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDateED.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var datestOld = Convert.ToDateTime(txtDateSt.Text);
            var dateedOld = Convert.ToDateTime(txtDateED.Text);
            var datest = new DateTime(datestOld.Year, datestOld.Month, datestOld.Day, 0, 0, 0);
            var dateed = new DateTime(dateedOld.Year, dateedOld.Month, dateedOld.Day, 23, 59, 59);

            var import = carrier_Entities.Central_Import.AsEnumerable()
                .Where(w => DateTime.Parse(w.Date_Posting) >= datest && DateTime.Parse(w.Date_Posting) <= dateed).ToList();

            List<modelFromCentral> total = new List<modelFromCentral>();
            decimal totalPrice = 0;
            decimal totalVat = 0;
            foreach (var item in import)
            {
                modelFromCentral log = new modelFromCentral();

                log.Posting_Date = item.Date_Posting;
                log.Shop = item.Shop;
                log.Brand = item.Brand;
                log.departmentID = item.Department_ID;
                log.Docno = item.Docno_Budget ?? "";
                log.Price = (item.Price ?? 0).ToString("#,##0.00");
                log.VAT = (item.Vat ?? 0).ToString("#,##0.00");
                total.Add(log);
                totalPrice += (item.Price ?? 0);
                totalVat += (item.Vat ?? 0);
            }
            gv_main.DataSource = total;
            gv_main.DataBind();
            if (gv_main.Rows.Count != 0)
            {
                gv_main.HeaderRow.Cells[3].Text = "ค่ารถ(" + totalPrice + ")";
                gv_main.HeaderRow.Cells[4].Text = "VAT(" + totalVat + ")";
                dv_gv_main.Visible = true;
                dv_uploadTOFC.Visible = true;
                btnExport.Visible = true;
            }

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            
            
            //LoadData
            var datest = Convert.ToDateTime(txtDateSt.Text);
            var dateed = Convert.ToDateTime(txtDateED.Text);

            //var centralimport = carrier_Entities.Central_Import.Where(w => w.Date_Posting >= datest && w.Date_Posting <= dateed).ToList();
            GridView gv_export = new GridView();

            if (gv_main.Rows.Count != 0)
            {
                List<model_Central_Export_BC> Central_Export_BC = new List<model_Central_Export_BC>();
                foreach (GridViewRow row in gv_main.Rows)
                {
                    Label lbPosting_Date = (Label)row.FindControl("lbPosting_Date");
                    Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
                    Label lbDepartment_id = (Label)row.FindControl("lbDepartment_id");
                    Label lbBrand = (Label)row.FindControl("lbBrand");
                    Label lbPrice = (Label)row.FindControl("lbPrice");
                    Label lbVAT = (Label)row.FindControl("lbVAT");
                    Label lbDocnoBud = (Label)row.FindControl("lbDocnoBud");

                    var convertSite = bC_TB_Entities.SiteSAP_BC.Where(w => w.SiteSAP == lbSiteStorage.Text).FirstOrDefault();
                    if (convertSite != null)
                    {
                        lbSiteStorage.Text = convertSite.SiteBC;
                    }
                    var brandBC = service_BC.getDimensionValue("BRAND_PROFIT CENTER");
                    var costcenterBC = service_BC.getDimensionValue("COST CENTER");
                    var brandMatch = brandBC.Where(w => w.DepartmentID == lbDepartment_id.Text).FirstOrDefault();
                    //Option
                    var brand = "";
                    var Cost_Center_Code = "";

                    if (brandMatch == null)
                    {

                        var departmentCEnter = costcenterBC.Where(w => w.DepartmentID == lbDepartment_id.Text).FirstOrDefault();
                        Cost_Center_Code = departmentCEnter == null ? "SALES (XXX110)" : departmentCEnter.Dimension_Value_Code;
                        brand = departmentCEnter == null ? "CENTER" : "SUPPORT 5020";
                    }
                    else
                    {
                        if (brandMatch.Dimension_Value_Code == "BRATPACK SHOP 5030")
                        {
                            var site2DIgit = lbSiteStorage.Text.Substring(2, 2);
                            var departmentCEnter = costcenterBC.Where(w => w.Site == site2DIgit).FirstOrDefault();
                            if (departmentCEnter != null)
                            {
                                Cost_Center_Code = departmentCEnter.Dimension_Value_Code;
                                brand = brandMatch.Dimension_Value_Code;
                            }
                            else
                            {
                                Cost_Center_Code = "SALES (XXX110)";
                                brand = brandMatch.Dimension_Value_Code;
                            }

                        }
                        else
                        {
                            Cost_Center_Code = "SALES (XXX110)";
                            brand = brandMatch.Dimension_Value_Code;
                        }
                    }
                    model_Central_Export_BC exp = new model_Central_Export_BC();
                    exp.type = "G/L Account";
                    exp.No = "6050008";
                    exp.ItemReference_No = "";
                    exp.Description_Comment = lbSiteStorage.Text + "_" + "Central_ค่าขนส่ง_ค่าพาหนะเฉพาะจัดส่ง_M" + Convert.ToDateTime(lbPosting_Date.Text).ToString("MM/yy");
                    exp.Description2 = "";
                    exp.Attached_to_Subscription_Contract_line = "No";
                    exp.Location_Code = "";
                    exp.Gen_Bus_Posting_Group = "EXPENSE";
                    exp.Gen_Prod_Posting_Group = "GL";
                    exp.VAT_Bus_Posting_Group = "VATHO";
                    exp.VAT_Prod_Posting_Group = "VATS7";
                    exp.WHT_Business_Posting_Group = "NOWHT";//Flash WHT53
                    exp.WHT_Product_Posting_Group = "TRANSPORT";
                    exp.Sustainability_Account_No = "";
                    exp.Quantity = "1";
                    exp.Unit_of_Measure_Code = "";
                    exp.Direct_Unit_Cost_Excl_VAT = lbPrice.Text;
                    exp.Line_Discount_Percent = "";
                    exp.Line_Amount_Excl_VAT = lbPrice.Text;
                    exp.Qty_to_Assign = "0";
                    exp.Qty_Assigned = "";
                    exp.Renewable_Energy = "NO";
                    exp.Emission_CO2 = "0";
                    exp.Emission_CH4 = "0";
                    exp.Emission_N2O = "0";
                    exp.Emission_Verified = "NO";
                    exp.CBAM = "NO";
                    exp.Brand_Profit_center_Code = brand;
                    exp.Cost_center_Code = Cost_Center_Code;
                    exp.Site_shop_Code = lbSiteStorage.Text;
                    exp.Chanel_Code = "OFFLINE";
                    exp.Io_Code = "NONE";
                    exp.Business_area_Code = "BA1000";
                    exp.Tax_Invoice_Date = "";
                    exp.Tax_Invoice_No = "";
                    exp.Tax_Vendor_No = "";
                    exp.Tax_Invoice_Name = "";
                    exp.Tax_Invoice_Base = "0";
                    exp.Tax_Head_Office = "NO";
                    exp.VAT_Branch_Code = "";
                    exp.Vat_Registration_No = "";
                    exp.เลขที่เอกสารใน_FC = lbDocnoBud.Text;
                    Central_Export_BC.Add(exp);
                }

                gv_export.DataSource = Central_Export_BC.ToList();
                gv_export.DataBind();



                Page.Response.ClearContent();
                Page.Response.AddHeader("Content-Disposition", "attachment;filename=" + "BC_Central_" + datest.ToString("dd-MM-yyyy") + "_" + dateed.ToString("dd-MM-yyyy") /*DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")*/ + ".xls");
                Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Page.Response.Charset = "utf-8";
                Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-874");
                Page.Response.ContentType = "application/vnd.ms-excel";
                using (StringWriter strwritter = new StringWriter())
                {
                    HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
                    htmltextwrtter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                    htmltextwrtter.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=windows-874'>");
                    gv_export.AllowPaging = false;
                    gv_export.HeaderRow.BackColor = System.Drawing.Color.Yellow;
                    gv_export.RenderControl(htmltextwrtter);
                    Page.Response.Output.Write(strwritter.ToString());

                    Page.Response.End();
                }
            }

        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            if(!decimal.TryParse(txtPriceAdj.Text, out _))
            {
                service_Budget.JSAlert("E", "กรุณากรอกจำนวนเงินให้ถูกต้อง");
                return;
            }

            if(txtPostingDate.Text == "")
            {
                service_Budget.JSAlert("E", "กรุณากรอกวันที่ Posting");
                return;
            }

            if (fileupload1.HasFiles)
            {
                string FileName = Path.GetFileName(fileupload1.PostedFile.FileName);

                string Extension = Path.GetExtension(fileupload1.PostedFile.FileName);

                string FolderPath = ConfigurationManager.AppSettings["FolderPath"];
                string FilePath = Server.MapPath(FolderPath + "ExelReport/" + FileName);

                fileupload1.SaveAs(FilePath);
                ReadExcel2(FilePath);
                dv_gv_main.Visible = false;
                dv_uploadTOFC.Visible = false;
                dv_Export.Visible = false;
            }
            else
            {
                //service_Budget.JSAlert("E", "ไม่สามารถเปิดไฟล์ได้");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่สามารถเปิดไฟล์ได้')", true);

            }
        }

        

        public void ReadExcel2(string filePath)
        {
            var priceTrue = Convert.ToDouble(txtPriceAdj.Text);


            using (var steam = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var reader = ExcelReaderFactory.CreateReader(steam);
                var result = reader.AsDataSet();
                var tables = result.Tables.Cast<DataTable>();
                try
                {
                    var rowRead = false;
                    List<model_Report_Due_Delivery> dataCentral = new List<model_Report_Due_Delivery>();
                    int totalBox = 0;
                    
                    foreach (DataTable table in tables)
                    {
                        if (table.TableName.StartsWith("Report_Due_Delivery"))
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                model_Report_Due_Delivery dataCentralRow = new model_Report_Due_Delivery();
                                var item = row.ItemArray;
                                if (item[0].ToString() != "" && rowRead == false)
                                {
                                    rowRead = true;
                                }
                                else
                                {
                                    if (rowRead)
                                    {
                                        int box2 = Convert.ToInt32(item[12].ToString());
                                        int box4 = Convert.ToInt32(item[13].ToString())*2;
                                        dataCentralRow.Box = box2 + box4;
                                        totalBox += box2 + box4;
                                        var Shop = item[5].ToString().Length == 8 ? item[5].ToString().Substring(0, 4) + item[5].ToString().Substring(6, 2) : item[5].ToString();
                                        dataCentralRow.Shop = Shop;
                                        dataCentralRow.Brand = item[8].ToString();
                                        var depart = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.BRANDABB == dataCentralRow.Brand).FirstOrDefault();
                                        dataCentralRow.DepartmentID = depart.departmentID;
                                        dataCentralRow.DateProcess = Convert.ToDateTime(item[0].ToString()).ToString("dd/MM/yyyy");
                                        dataCentral.Add(dataCentralRow);
                                    }
                                }
                            }
                        }
                    }


                    var pricePerbox = priceTrue / totalBox;
                    double totalPrice = 0;
                    double totalVat = 0;
                    foreach (var i in dataCentral)
                    {
                        i.Price = Math.Round(pricePerbox * i.Box,2).ToString("#,##0.00") ;
                        i.Vat = Math.Round((Convert.ToDouble(i.Price) * 0.07),2).ToString("#,##0.00");
                        totalPrice += Math.Round(pricePerbox * i.Box, 2);
                        totalVat += Math.Round((Convert.ToDouble(i.Price) * 0.07), 2);
                    }

                    //dataCentral = dataCentral.GroupBy(g => new { g.Shop, g.Brand , g.DateProcess }).Select(s => new model_Report_Due_Delivery
                    //{
                    //    Shop = s.Key.Shop,
                    //    Brand = s.Key.Brand,
                    //    DepartmentID = s.FirstOrDefault().DepartmentID,
                    //    DateProcess = s.Key.DateProcess,
                    //    Box = s.Sum(c=>c.Box),
                    //    Vat = s.Sum(c=> Convert.ToDouble(c.Vat)).ToString("#,##0.00"),
                    //    Price = s.Sum(c=>Convert.ToDouble(c.Price)).ToString("#,##0.00")
                    //}).ToList();
                    var no = 1;
                    foreach(var i in dataCentral)
                    {
                        i.No = no;
                        no++;
                    }
                    gv_Temp.DataSource = dataCentral;
                    gv_Temp.DataBind();

                    gv_Temp.HeaderRow.Cells[3].Text = "Price(" + totalPrice.ToString("#,##0.00") + ")";
                    gv_Temp.HeaderRow.Cells[4].Text = "VAT(7%)(" + totalVat.ToString("#,##0.00") + ")";

                    dv_Adj.Visible = true;
                }
                catch (Exception ex)
                {
                    service_Budget.JSAlert("E", "ERROR : " + ex.Message);
                }
            }

        }

        protected void btnUptoBudget_Click(object sender, EventArgs e)
        {
            txtDateSt.Enabled = false;
            txtDateED.Enabled = false;
            btnUptoBudget.Visible = false;
            btnApprove.Visible = true;
            btnReject.Visible = true;
            dv_DateST.Style.Add("pointer-events", "none");
            dv_DateED.Style.Add("pointer-events", "none");
        }
        protected void btnReject_Click(object sender, EventArgs e)
        {
            txtDateSt.Enabled = true;
            txtDateED.Enabled = true;
            btnUptoBudget.Visible = true;
            btnApprove.Visible = false;
            btnReject.Visible = false;
            dv_DateST.Style.Remove("pointer-events");
            dv_DateED.Style.Remove("pointer-events");
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            var strError = "";
            List<modelFromCentral> total = new List<modelFromCentral>();
            GridView gv_export = new GridView();
            if (gv_main.Rows.Count != 0)
            {
                foreach (GridViewRow row in gv_main.Rows)
                {
                    Label lbPosting_Date = (Label)row.FindControl("lbPosting_Date");
                    Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
                    Label lbDepartment_id = (Label)row.FindControl("lbDepartment_id");
                    Label lbBrand = (Label)row.FindControl("lbBrand");
                    Label lbPrice = (Label)row.FindControl("lbPrice");
                    Label lbVAT = (Label)row.FindControl("lbVAT");
                    Label lbDocnoBud = (Label)row.FindControl("lbDocnoBud");

                    try
                    {
                        if (lbDocnoBud.Text == "")
                        {
                            var brand_id = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.departmentID == lbDepartment_id.Text).FirstOrDefault();
                            var Seek = budget_Entities.Departments.Where(w => w.Department_Name.StartsWith("SEEK") && w.Department_ID == lbDepartment_id.Text).FirstOrDefault();

                            cuttemp temp = new cuttemp();
                            temp.date_use = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                            temp.depart_id = Seek == null ? lbDepartment_id.Text : "1619";
                            temp.detail_id = "5703";
                            temp.group_id = "5";
                            temp.head_id = "507";
                            temp.money = Convert.ToDouble(lbPrice.Text);
                            temp.remark = "ค่ารถจัดส่ง Auto จากระบบ Courier Central รอบ :" + lbPosting_Date.Text + " SiteStorage:" + lbSiteStorage.Text + " Brand : " + lbBrand.Text;
                            temp.typeBudget_id = "2";
                            temp.userId = "101974";
                            temp.site_storage = lbSiteStorage.Text;
                            var ss = service_Budget.Insert_CutBudget(temp);

                            if (ss == "สำเร็จ")
                            {
                                var budDocno = budget_Entities.MainExpenses.Where(w => w.UserID == "101974" && w.Docno.StartsWith("UP") && w.Remark.Contains(temp.remark)).FirstOrDefault();
                                if (budDocno != null)
                                {
                                    lbDocnoBud.Text = budDocno.Docno;
                                    var central = carrier_Entities.Central_Import.Where(w => w.Date_Posting == lbPosting_Date.Text && w.Shop == lbSiteStorage.Text && w.Brand == lbBrand.Text).FirstOrDefault();
                                    central.Docno_Budget = budDocno.Docno;
                                    carrier_Entities.SaveChanges();
                                }
                            }

                        }
                        total.Add(new modelFromCentral
                        {
                            Posting_Date = lbPosting_Date.Text,
                            Shop = lbSiteStorage.Text,
                            Brand = lbBrand.Text,
                            departmentID = lbDepartment_id.Text,
                            Price = lbPrice.Text,
                            Docno = lbDocnoBud.Text

                        });

                    }
                    catch (Exception ex)
                    {
                        strError += lbSiteStorage.Text + " - Brand : " + lbBrand.Text + " Error = " + ex.Message + ";" + @"</br>";
                    }
                }

                if (strError != "")
                {
                    service_Budget.JSAlert("E", strError);
                }
                else
                {
                    service_Budget.JSAlert("S", "บันทึกค่าใช้จ่ายลง Budget สำเร็จ");



                }
                gv_main.DataSource = total.OrderBy(o => o.Docno).ToList();
                gv_main.DataBind();

                updatePanel1.Update();
            }

            btnExport_Click(this, EventArgs.Empty);
            btnReject_Click(this, EventArgs.Empty);
        }


        



        //Model
        public class modelFromCentral
        {
            public string Posting_Date { get; set; }
            public string Shop { get; set; }
            public string Price { get; set; }
            public string VAT { get; set; }
            public string Brand { get; set; }
            public string departmentID { get; set; }
            public bool StatusBud { get; set; }
            public string Docno { get; set; }
            
        }

        public class model_Report_Due_Delivery
        {
            public int No { get; set; }
            public string Shop { get; set; }
            public string Brand { get; set; }
            public string Price { get; set; }
            public string Vat { get; set; }
            public int Box { get; set; }
            public string DateProcess { get; set; }
            public string DepartmentID { get; set; }
        }

        public class model_Central_Export_BC
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
            public string Energy_Source_Code { get; set; }
            public string Quantity { get; set; }
            public string Unit_of_Measure_Code { get; set; }
            public string Direct_Unit_Cost_Excl_VAT { get; set; }
            public string Line_Discount_Percent { get; set; }
            public string Line_Amount_Excl_VAT { get; set; }
            public string Qty_to_Assign { get; set; }
            public string Qty_Assigned { get; set; }
            public string Renewable_Energy { get; set; }
            public string Emission_CO2 { get; set; }
            public string Emission_CH4 { get; set; }
            public string Emission_N2O { get; set; }
            public string Source_of_Emission_data { get; set; }
            public string Emission_Verified { get; set; }
            public string CBAM { get; set; }
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

        protected void btnSaveAdj_Click(object sender, EventArgs e)
        {
            if(!DateTime.TryParse(txtPostingDate.Text,out _))
            {
                service_Budget.JSAlert("E", "วันที่ Posting Date ไม่ถูกต้อง");
                return;
            }

            //SaveData and Set gv_main

            modelFromCentral total = new modelFromCentral();
            List<modelFromCentral> dataCentral = new List<modelFromCentral>();
            decimal totalPrice = 0;
            decimal totalVat = 0;
            foreach (GridViewRow row in gv_Temp.Rows)
            {
                if (row.RowType != DataControlRowType.DataRow)
                    continue;

                // Posting Date
                DateTime processDate = Convert.ToDateTime(
                    row.Cells[0].Text);
                string processDateStr = processDate.ToString("dd/MM/yyyy");

                // Shop
                string shop = row.Cells[1].Text;

                // Brand
                string brand = row.Cells[2].Text;

                // DepartmentID (Hidden BoundField)
                Label lbDepartmentID = (Label)row.FindControl("lbDepartmentID");
                string departmentID = lbDepartmentID.Text;

                // Controls
                TextBox txtPrice = (TextBox)row.FindControl("txtPriceAdj");
                Label lbPrice = (Label)row.FindControl("lbPriceAdj");

                TextBox txtVat = (TextBox)row.FindControl("txtVatAdj");
                Label lbVat = (Label)row.FindControl("lbVatAdj");

                decimal price = 0;
                decimal vat = 0;

                // ถ้าเป็น editable row
                if (txtPrice != null && txtPrice.Visible)
                    decimal.TryParse(txtPrice.Text, out price);
                else
                    decimal.TryParse(lbPrice.Text, out price);

                if (txtVat != null && txtVat.Visible)
                    decimal.TryParse(txtVat.Text, out vat);
                else
                    decimal.TryParse(lbVat.Text, out vat);


                total = new modelFromCentral();
                total.Posting_Date = txtPostingDate.Text;
                total.Shop = shop;
                total.Brand = brand;
                total.departmentID = departmentID;
                total.Price = price.ToString("#,##0.00");
                total.VAT = (vat).ToString("#,##0.00");
                dataCentral.Add(total);
                totalPrice += price;
                totalVat += vat;
            }

            dataCentral = dataCentral.GroupBy(g => new { g.Brand, shop = g.Shop }).OrderBy(o=>o.Key.Brand).Select(s => new modelFromCentral
            {
                Brand = s.Key.Brand,
                Shop = s.Key.shop,
                departmentID = s.FirstOrDefault().departmentID,
                Posting_Date = s.FirstOrDefault().Posting_Date,
                Price = s.Sum(c => Convert.ToDecimal(c.Price)).ToString("#,##0.00"),
                VAT = s.Sum(c => Convert.ToDecimal(c.VAT)).ToString("#,##0.00")

            }).ToList();

            foreach(var dtct in dataCentral)
            {
                // ใช้งานข้อมูล

                var cenImp = carrier_Entities.Central_Import.Where(w => w.Shop == dtct.Shop && w.Brand == dtct.Brand && w.Date_Posting == dtct.Posting_Date ).FirstOrDefault();
                if (cenImp == null)
                {
                    decimal price = Convert.ToDecimal(dtct.Price);
                    decimal VAT = Convert.ToDecimal(dtct.VAT);
                    carrier_Entities.Central_Import.Add(new Model.Carrier.Central_Import
                    {
                        Date_Create = DateTime.Now,
                        Shop = dtct.Shop,
                        Department_ID = dtct.departmentID,
                        Brand = dtct.Brand,
                        Date_Posting = dtct.Posting_Date,
                        Price = price,
                        Vat = VAT
                    });
                    carrier_Entities.SaveChanges();

                }
            }

            gv_main.DataSource = dataCentral;
            gv_main.DataBind();
            gv_main.HeaderRow.Cells[3].Text = "ค่ารถ(" + totalPrice + ")";
            gv_main.HeaderRow.Cells[4].Text = "VAT(" + totalVat + ")";

            dv_Adj.Visible = false;
            dv_gv_main.Visible = true;
            dv_uploadTOFC.Visible = true;
            dv_Export.Visible = true;
        }

        protected void txtPriceAdj_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow row = (GridViewRow)txt.NamingContainer;

            string departmentID = ((Label)row.FindControl("lbDepartmentID")).Text;

            decimal price = 0;
            decimal.TryParse(txt.Text, out price);

            decimal vat = price * 0.07m;
            Label lbPriceAdj = (Label)row.FindControl("lbPriceAdj");

            var headerPrice = Convert.ToDecimal(gv_Temp.HeaderRow.Cells[3].Text.Split('(')[1].TrimEnd(')'));
            var headerVat = Convert.ToDecimal(gv_Temp.HeaderRow.Cells[4].Text.Replace("(7%)","").Split('(')[1].TrimEnd(')'));
            headerPrice -= Convert.ToDecimal(lbPriceAdj.Text);
            headerPrice += Convert.ToDecimal(txt.Text);


            Label lbVat = (Label)row.FindControl("lbVatAdj");
            TextBox txtVatAdj = (TextBox)row.FindControl("txtVatAdj");

            if (lbVat != null)
            {
                headerVat -= Convert.ToDecimal(lbVat.Text);
                headerVat += vat;

                lbVat.Text = vat.ToString("N2");
                txtVatAdj.Text = vat.ToString("N2");


            }
            gv_Temp.HeaderRow.Cells[3].Text = "Price(" + headerPrice.ToString("#,##0.00") + ")";
            gv_Temp.HeaderRow.Cells[4].Text = "VAT(7%)(" + headerVat.ToString("#,##0.00") + ")";
            // refresh updatepanel

            lbPriceAdj.Text = txt.Text;
            updatePanel1.Update();
        }

        protected void txtVatAdj_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow row = (GridViewRow)txt.NamingContainer;
            Label lbVat = (Label)row.FindControl("lbVatAdj");
            var headerVat = Convert.ToDecimal(gv_Temp.HeaderRow.Cells[4].Text.Replace("(7%)", "").Split('(')[1].TrimEnd(')'));
            headerVat -= Convert.ToDecimal(lbVat.Text);
            headerVat += Convert.ToDecimal(txt.Text);

            gv_Temp.HeaderRow.Cells[4].Text = "VAT(7%)(" + headerVat.ToString("#,##0.00") + ")";

            updatePanel1.Update();
        }
    }
}