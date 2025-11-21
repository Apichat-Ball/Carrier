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
using Carrier.Service;
using System.Data.Entity.SqlServer;

namespace Carrier
{
    public partial class Central_Import : System.Web.UI.Page
    {
        CarrierEntities carrier_Entities = new CarrierEntities();
        InsideSFG_WFEntities insideSFG_WF_Entities = new InsideSFG_WFEntities();
        BudgetEntities budget_Entities = new BudgetEntities();

        Service_Budget service_Budget = new Service_Budget();

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
            var datest = new DateTime(datestOld.Year, datestOld.Month, datestOld.Day ,0, 0,0);
            var dateed = new DateTime(dateedOld.Year, dateedOld.Month, dateedOld.Day , 23,59,59);

            var import = carrier_Entities.Central_Import.AsEnumerable()
                .Where(w => DateTime.Parse(w.Date_Posting) >= datest && DateTime.Parse( w.Date_Posting) <= dateed).ToList();

            List<modelFromCentral> total = new List<modelFromCentral>();

            foreach(var item in import)
            {
                modelFromCentral log = new modelFromCentral();

                log.Posting_Date = item.Date_Posting;
                log.Shop = item.Shop;
                log.Brand = item.Brand;
                log.departmentID = item.Department_ID;
                log.Docno = item.Docno_Budget ?? "";
                log.Center = item.Center;
                log.Price = (item.Price??0).ToString("#,##0.00");
                log.Profit = item.Profit;
                log.VAT = (item.Vat??0).ToString("#,##0.00");
                total.Add(log);
            }
            gv_main.DataSource = total;
            gv_main.DataBind();
            if (gv_main.Rows.Count != 0)
            {
                btnExport.Visible = true;
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var datest = Convert.ToDateTime(txtDateSt.Text);
            var dateed = Convert.ToDateTime(txtDateED.Text);

            var centralimport = carrier_Entities.Central_Import.Where(w => w.Date_Create >= datest && w.Date_Create <= dateed).ToList();
            GridView gv_export = new GridView();

            List<object> total = new List<object>();
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

                    total.Add(new
                    {
                        Posting_Date = lbPosting_Date.Text,
                        Shop = lbSiteStorage.Text,
                        Brand = lbBrand.Text,
                        Price = lbPrice.Text,
                        Docno = lbDocnoBud.Text

                    });

                }

                gv_export.DataSource = total.ToList();
                gv_export.DataBind();

                Page.Response.ClearContent();
                Page.Response.AddHeader("Content-Disposition", "attachment;filename=" + "Cutbudget_Central" + datest.ToString("dd-MM-yyyy") + "_" + dateed.ToString("dd-MM-yyyy") /*DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")*/ + ".xls");
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
            using (var steam = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var reader = ExcelReaderFactory.CreateReader(steam);
                var result = reader.AsDataSet();
                var tables = result.Tables.Cast<DataTable>();
                try
                {
                    var rowRead = false;
                    List<modelFromCentral> dataCentral = new List<modelFromCentral>();
                    modelFromCentral total = new modelFromCentral();
                    foreach (DataTable table in tables)
                    {
                        if (table.TableName.StartsWith("SAP Upload"))
                        {
                            foreach (DataRow row in table.Rows)
                            {

                                var item = row.ItemArray;
                                if (item[21].ToString() != "" && rowRead == false)
                                {
                                    rowRead = true;
                                }
                                else
                                {
                                    if (rowRead)
                                    {
                                        if (item[16].ToString() == "VX" && item[21].ToString() != "")
                                        {
                                            total = new modelFromCentral();
                                            var Shop = item[25].ToString();
                                            var Profit = item[21].ToString();
                                            var Center = item[22].ToString();
                                            var VAT = Convert.ToDecimal(Convert.ToDouble(item[13].ToString()) * 0.07);

                                            var BRAND = carrier_Entities.Site_Profit.Where(w => w.Profit == Profit && w.Costcenter == Center).FirstOrDefault();
                                            var vbrand = insideSFG_WF_Entities.vBrandAndHeadFCs.Where(w => w.BRANDABB == BRAND.Brand).FirstOrDefault();
                                            total.Posting_Date = item[2].ToString().Substring(0,2) +"/"+ item[2].ToString().Substring(2, 2) +"/"+ item[2].ToString().Substring(4, 4);
                                            total.Shop = Shop;
                                            total.Brand = vbrand.BRANDABB;
                                            total.departmentID = vbrand.departmentID;
                                            total.Profit = Profit;
                                            total.Center = Center;
                                            total.Price = item[13].ToString();
                                            total.VAT = VAT.ToString("#,##0.00");

                                            var cenImp = carrier_Entities.Central_Import.Where(w => w.Shop == total.Shop && w.Brand == total.Brand && w.Date_Posting == total.Posting_Date).FirstOrDefault();
                                            if (cenImp == null)
                                            {
                                                var priceDeci = Convert.ToDecimal(total.Price);
                                                carrier_Entities.Central_Import.Add(new Model.Carrier.Central_Import
                                                {
                                                    Date_Create = DateTime.Now,
                                                    Shop = total.Shop,
                                                    Department_ID = vbrand.departmentID,
                                                    Brand = total.Brand,
                                                    Date_Posting = total.Posting_Date,
                                                    Profit = total.Profit,
                                                    Center = total.Center,
                                                    Price = priceDeci,
                                                    Vat = VAT
                                                }) ;
                                                carrier_Entities.SaveChanges();
                                            }
                                            else
                                            {
                                                total.Docno = cenImp.Docno_Budget;
                                            }
                                            dataCentral.Add(total);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    gv_main.DataSource = dataCentral.ToList();
                    gv_main.DataBind();

                    

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
            txtDateSt.Enabled = true ;
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
            if(gv_main.Rows.Count != 0)
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
                            temp.money = Convert.ToDouble(lbPrice.Text) ;
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
            }

            btnExport_Click(this, EventArgs.Empty);
            btnReject_Click(this, EventArgs.Empty);
        }


        public void ReloadLoadData()
        {
            foreach (GridViewRow row in gv_main.Rows)
            {
                Label lblbPosting_Date = (Label)row.FindControl("lbPosting_Date");
                Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
                Label lbBrand = (Label)row.FindControl("lbBrand");
                Label lbDepartment_id = (Label)row.FindControl("lbDepartment_id");
                Label lbPrice = (Label)row.FindControl("lbPrice");
                Label lbDocnoBud = (Label)row.FindControl("lbDocnoBud");


                

            }
        }



        //Model
        public class modelFromCentral
        {
            public string Posting_Date { get; set; }
            public string Shop { get; set; }
            public string Price { get; set; }
            public string VAT { get; set; }
            public string Profit { get; set; }
            public string Center { get; set; }
            public string Brand { get; set; }
            public string departmentID { get; set; }
            public bool StatusBud { get; set; }
            public string Docno { get; set; }
        }
    }
}