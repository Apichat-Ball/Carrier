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
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Whale;
using Carrier.Service;
using ClosedXML.Excel;

namespace Carrier
{
    public partial class DHL_Import : System.Web.UI.Page
    {

        CarrierEntities carrier_Entities = new CarrierEntities();
        BudgetEntities budget_Entities = new BudgetEntities();
        InsideSFG_WFEntities insideSFG_WF_Entities = new InsideSFG_WFEntities();
        WhaleEntities whale_Entities = new WhaleEntities();

        Service_Whale service_Whale = new Service_Whale();
        Service_Budget service_Budget = new Service_Budget();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var now = DateTime.Now;
                txtDateSt.Text = new DateTime(now.Year, now.Month, 1).ToShortDateString();
                txtDateED.Text = new DateTime(now.Year, now.Month, now.Day).ToShortDateString();
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
                            List<model_GV_Check_DHL> dataCheck = new List<model_GV_Check_DHL>();
                            foreach (DataRow row in table.Rows)
                            {
                                var item = row.ItemArray;


                                if (item[6].ToString() == "Pick up date")
                                {
                                    rowRead = true;
                                }
                                else
                                {
                                    if (rowRead)
                                    {

                                        if (item[6].ToString() == "")
                                        {
                                            rowRead = false;
                                        }
                                        else
                                        {
                                            if (DateTime.TryParse(item[6].ToString(), out _))
                                            {
                                                var trackingNo = item[9].ToString().Replace("'","");

                                                var docno = whale_Entities.SalesOrder_Item.Where(w => w.Trackingno == trackingNo).FirstOrDefault()?.Docno;

                                                var order = Get_Order_Whale(docno).ToList() ;
                                                var totalQTY = order.Sum(s => s.TotalQTY);
                                                var total = Convert.ToDouble(item[56].ToString());
                                                
                                                try
                                                {
                                                    foreach(var brand  in order)
                                                    {
                                                        model_GV_Check_DHL dataC = new model_GV_Check_DHL();
                                                        DHL_eCom_Import one = new DHL_eCom_Import();
                                                        one.Docno = brand.Docno;
                                                        one.Date_Process = Convert.ToDateTime(item[6].ToString());
                                                        one.Tracking_No = trackingNo;
                                                        one.Date_Import = DateTime.Now;
                                                        one.Price = (total / totalQTY) * brand.TotalQTY;
                                                        one.Per_Price = (total / totalQTY);
                                                        
                                                        var depart = budget_Entities.Departments.Where(w => w.ShortBrand == brand.Brand_Short ).FirstOrDefault();
                                                        one.department_id = Convert.ToInt32(depart.Department_ID);
                                                        one.QTY = brand.TotalQTY;

                                                        dataC.sitestorage = brand.Customer_Code;
                                                        
                                                        if (brand.Customer_Code == "" || brand.Customer_Code == null)
                                                        {
                                                            var idCard = whale_Entities.SalesOrders.Where(w => w.Docno == brand.Docno).FirstOrDefault().b_IDCard;
                                                            if (idCard == "" || idCard == null)
                                                            {
                                                                if (depart.Department_Name.Contains("SFG"))
                                                                {
                                                                    dataC.sitestorage = "ZXSFOL";
                                                                }
                                                                else
                                                                {
                                                                    dataC.sitestorage = "Z6SFOL";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (depart.Department_Name.Contains("SFG"))
                                                                {
                                                                    dataC.sitestorage = "ZXETOL";
                                                                }
                                                                else
                                                                {
                                                                    dataC.sitestorage = "Z6ETOL";
                                                                }
                                                            }
                                                        }
                                                        one.Shop = dataC.sitestorage;

                                                        dataC.Department = "("+depart.ShortBrand+")"+ depart.Department_Name;
                                                        dataC.DateProcess = one.Date_Process?? DateTime.Now;
                                                        dataC.Docno = brand.Docno;
                                                        dataC.TrackingNo = trackingNo;
                                                        dataC.Price = ((total / totalQTY) * brand.TotalQTY)??0;
                                                        var importHave = carrier_Entities.DHL_eCom_Import.Where(w => w.Docno == brand.Docno && w.department_id == one.department_id).FirstOrDefault();

                                                        if(importHave != null)
                                                        {
                                                            
                                                            importHave.Per_Price = (total / totalQTY);
                                                            importHave.Shop = dataC.sitestorage;
                                                            carrier_Entities.SaveChanges();
                                                            dataC.Docno_Bud = importHave.Docno_Budget;
                                                            dataC.Date_Budget = importHave.Date_Budget;
                                                            
                                                        }
                                                        else
                                                        {
                                                            carrier_Entities.DHL_eCom_Import.Add(one);
                                                            carrier_Entities.SaveChanges();
                                                        }
                                                        dataCheck.Add(dataC);
                                                    }
                                                }catch(Exception ex)
                                                {  

                                                    return;
                                                }
                                            }

                                        }
                                    }

                                }
                            }

                            if (dataCheck.Count() == 0)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่พบข้อมูลครับ')", true);
                            }
                            else
                            {
                                dv_gv_import_Check.Visible = true;
                            }

                            gv_Import.DataSource = dataCheck.OrderBy(o => o.DateProcess).ToList();
                            gv_Import.DataBind();


                            //foreach (GridViewRow row in gv_Import.Rows)
                            //{
                            //    Label lbDocno_Match = (Label)row.FindControl("lbDocno_Match");
                            //    Label lbDateProcess = (Label)row.FindControl("lbDateProcess");
                            //    Label lbPno = (Label)row.FindControl("lbPno");
                            //    Label lbDocno = (Label)row.FindControl("lbDocno");
                            //    Label lbPrice = (Label)row.FindControl("lbPrice");
                            //    Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
                            //    Label lbDepartment_ID = (Label)row.FindControl("lbDepartment_ID");
                            //    Label lbDocnoInBud = (Label)row.FindControl("lbDocnoInBud");
                            //    Image imgCheck = (Image)row.FindControl("imgCheck");

                            //    var match = Convert.ToBoolean(lbDocno_Match.Text);
                            //    if (match)
                            //    {
                            //        imgCheck.ImageUrl = "~\\Icon\\correct.png";
                            //    }
                            //    else
                            //    {
                            //        imgCheck.ImageUrl = "~\\Icon\\x-button.png";
                            //        row.BackColor = System.Drawing.Color.LightPink;
                            //    }

                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่สามารถอ่านไฟล์ได้')", true);
                }
            }
        }

        public IQueryable<modelTOExport_DHL> LoadExport()
        {
            var dateSt = Convert.ToDateTime(txtDateSt.Text);
            var dateEd = Convert.ToDateTime(txtDateED.Text);
            try
            {
                //var carrier = carrier_Entities.DHL_eCom_Import.Where(w => w.Date_Process >= dateSt && w.Date_Process <= dateEd).ToList();
                var carrier = carrier_Entities.DHL_eCom_Import.Where(w => w.Date_Process >= dateSt && w.Date_Process <= dateEd)
                .GroupBy(g => new
                {
                    g.Tracking_No,
                    g.department_id,
                    g.Shop,
                    g.Docno_Budget
                })
                .Select(s => new modelTOExport_DHL
                {
                    Date_Process = s.FirstOrDefault().Date_Process,
                    Tracking_No = s.Key.Tracking_No,
                    department_id = s.Key.department_id,
                    Price = s.Sum(c => c.Price)??0,
                    Docno = s.FirstOrDefault().Docno,
                    Docno_Budget = s.Key.Docno_Budget,
                    Shop = s.Key.Shop
                });
                return carrier;
            }
            catch(Exception ex)
            {
                return null;
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
                    QTY = s.Sum(c=>c.QTY)
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
                                    TotalQTY = tOrder.QTY
                                });

            list_Model_gvData.AddRange(objOrder_SAP.ToList());

            var objOrder_POS = (from tOrder in objOrder
                                where tOrder.Type_Transaction == "POS"
                                select new Model_gvData
                                {
                                    Date_Send = tOrder.Date_Send,
                                    Docno = tOrder.Docno,
                                    Customer_Code = tOrder.Channel_refCode,
                                    Brand_Short = tOrder.Brand_ID,
                                    TotalQTY = tOrder.QTY
                                });

            list_Model_gvData.AddRange(objOrder_POS.ToList());


            
            return list_Model_gvData;
        }


        protected void btnExport_Click(object sender, EventArgs e)
        {

            var dateST = Convert.ToDateTime(txtDateSt.Text);
            var dateED = Convert.ToDateTime(txtDateED.Text);
            var getDataSAP = loaddataExport();

            if(getDataSAP != null)
            {
                var filename = "SAP_DHL" + dateST.ToString("dd-MM-yyyy") + "_" + dateED.ToString("dd-MM-yyyy") /*DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")*/ + ".xls";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(getDataSAP);
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + filename);
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

        public DataSet loaddataExport()
        {
            DataSet DS = new DataSet();
            DataTable SAP = new DataTable("SAP_DHL");

            SAP.Columns.Add(new DataColumn("Posting_Date", typeof(string)));
            SAP.Columns.Add(new DataColumn("Account", typeof(string)));
            SAP.Columns.Add(new DataColumn("Amount", typeof(string)));
            SAP.Columns.Add(new DataColumn("Amount_in_LC", typeof(string)));
            SAP.Columns.Add(new DataColumn("Tax_Base_Amount", typeof(string)));
            SAP.Columns.Add(new DataColumn("Tax_Code", typeof(string)));
            SAP.Columns.Add(new DataColumn("Bus_Area", typeof(string)));
            SAP.Columns.Add(new DataColumn("Baseline_Date", typeof(string)));
            SAP.Columns.Add(new DataColumn("Payment_Term", typeof(string)));
            SAP.Columns.Add(new DataColumn("Planning_Level", typeof(string)));
            SAP.Columns.Add(new DataColumn("Profit_Center", typeof(string)));
            SAP.Columns.Add(new DataColumn("Cost_Center", typeof(string)));
            SAP.Columns.Add(new DataColumn("Service_Cost_Center", typeof(string)));
            SAP.Columns.Add(new DataColumn("Order", typeof(string)));
            SAP.Columns.Add(new DataColumn("Shop", typeof(string)));
            SAP.Columns.Add(new DataColumn("Assignment", typeof(string)));
            SAP.Columns.Add(new DataColumn("Brand", typeof(string)));
            SAP.Columns.Add(new DataColumn("Docno", typeof(string)));
            SAP.Columns.Add(new DataColumn("เลขที่เอกสารใน_FC", typeof(string)));

            var carrierdata = LoadExport();
            double total = 0;
            foreach (var res in carrierdata)
            {
                total += res.Price;
                DataRow row = SAP.NewRow();
                row[0] = (res.Date_Process ?? DateTime.Now).ToString("ddMMyyyy");
                row[1] = "6050008";
                row[2] = res.Price.ToString("#,##0.00");
                row[3] = "";
                row[4] = "";
                row[5] = "VX";
                row[6] = "";
                row[7] = "";
                row[8] = "";
                row[9] = "";

                var brand = budget_Entities.Departments.Where(w=>w.Department_ID == res.department_id.ToString()).FirstOrDefault();
                var sitestorage = (res.Shop.Substring(0, 2) == "Z6" ? res.Shop.Substring(0, 2) + "SF" + brand.ShortBrand + "OL" : res.Shop.Substring(0, 4) + brand.ShortBrand + res.Shop.Substring(4, 2)) ;
                var carrYSite = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == sitestorage && w.Brand == brand.ShortBrand && w.Channel == "ONLINE").FirstOrDefault();

                row[10] = carrYSite == null?"": carrYSite.Profit;
                row[11] = carrYSite == null ? "" : carrYSite.Costcenter;
                row[12] = "";
                row[13] = "";
                row[14] = res.Shop;
                row[15] = "DHL";
                row[16] = "("+brand.ShortBrand +")" + brand.Department_Name;
                row[17] = res.Docno;
                row[18] = res.Docno_Budget;

                SAP.Rows.Add(row);
            }
            DataRow rowLast = SAP.NewRow();
            rowLast[1] = "70392";
            rowLast[2] = total;
            rowLast[5] = "VX";

            SAP.Rows.Add(rowLast);

            DS.Tables.Add(SAP);
            return DS;
        }
        

        protected void btnUploadToBudget_Click(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(txtDateSt.Text, out _))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('รูปแบบวันที่เริ่มไม่ถูกต้อง')", true);

                return;
            }
            if (!DateTime.TryParse(txtDateED.Text, out _))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('รูปแบบวันที่สิ้นสุดไม่ถูกต้อง')", true);

                return;
            }
            if(Convert.ToDateTime(txtDateSt.Text) > Convert.ToDateTime(txtDateED.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ระยะเวลาไม่ถูกต้อง')", true);

                return;
            }

            if (btnUploadToBudget.Text == "Upload to Budget")
            {
                
                dv_DateST.Style.Add("pointer-events", "none");
                dv_DateED.Style.Add("pointer-events", "none");
                btnUploadToBudget.Text = "Approve";
                btnRejectUploadBud.Visible = true;
            }
            else
            {
                var datest = Convert.ToDateTime(txtDateSt.Text);
                var dateed = Convert.ToDateTime(txtDateED.Text);
                var allDHL = carrier_Entities.DHL_eCom_Import.Where(w => w.Date_Process >= datest && w.Date_Process <= dateed && (w.Docno_Budget == "" || w.Docno_Budget == null))
                    .GroupBy(g=>new
                    {
                        g.Tracking_No,
                        g.department_id,
                        g.Shop
                    })
                    .Select(s=>new
                    {
                        Tracking_No = s.Key.Tracking_No,
                        Department_id = s.Key.department_id,
                        Shop = s.Key.Shop,
                        Price = s.Sum(c=>c.Price)
                    }).ToList();
                if(allDHL.Count() != 0)
                {
                    List<string> trackFail = new List<string>();
                    
                    try
                    {
                        foreach (var Track in allDHL)
                        {
                            var seek = budget_Entities.Departments.Where(w => w.Department_ID == Track.Department_id.ToString() && w.Department_Name.StartsWith("SEEK")).FirstOrDefault();
                            var brand = budget_Entities.Departments.Where(w => w.Department_ID == Track.Department_id.ToString()).FirstOrDefault();
                            cuttemp temp = new cuttemp();
                            temp.date_use = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                            temp.depart_id = seek == null ? Track.Department_id.ToString() : "1619";
                            temp.detail_id = "5703";
                            temp.group_id = "5";
                            temp.head_id = "507";
                            temp.money = Track.Price ?? 0;
                            temp.remark = "ค่ารถจัดส่ง Auto จากระบบ Courier DHL รอบ " + txtDateSt.Text + " - " + txtDateED.Text + " Tracking:" + Track.Tracking_No + " Site:" + Track.Shop.Substring(0,4) + brand.ShortBrand + Track.Shop.Substring(4,2);
                            temp.typeBudget_id = "1";
                            temp.userId = "101974";
                            temp.site_storage = Track.Shop.Substring(0, 4) + brand.ShortBrand + Track.Shop.Substring(4, 2);

                            var res = service_Budget.Insert_CutBudget(temp);
                            if (res != "สำเร็จ")
                            {
                                trackFail.Add(Track.Tracking_No);
                            }
                            else
                            {
                                var dep = seek == null ? Track.Department_id.ToString() : "1619";
                                var bud = budget_Entities.MainExpenses.Where(w => w.Department_ID == dep && w.Remark.Contains("Tracking:" + Track.Tracking_No + " Site:" + Track.Shop.Substring(0, 4) + brand.ShortBrand + Track.Shop.Substring(4, 2))).FirstOrDefault();
                                if(bud != null)
                                {
                                    var car = carrier_Entities.DHL_eCom_Import.Where(w => w.Tracking_No == Track.Tracking_No && w.department_id == Track.Department_id && w.Shop == Track.Shop).FirstOrDefault();
                                    car.Docno_Budget = bud.Docno;
                                    car.Date_Budget = DateTime.Now;
                                    carrier_Entities.SaveChanges();
                                }
                            }
                        }

                        if (trackFail.Count() != 0)
                        {
                            trackFail = trackFail.Distinct().ToList();
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกสำเร็จ ยังมีบางรายการที่ไม่ผ่าน" + Newtonsoft.Json.JsonConvert.SerializeObject(trackFail) + "')", true);

                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกสำเร็จ')", true);

                        }
                    }
                    catch(Exception ex)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกไม่สำเร็จ')", true);
                        return;
                    }
                    
                }




                dv_DateST.Style.Remove("pointer-events");
                dv_DateED.Style.Remove("pointer-events");
                btnUploadToBudget.Text = "Upload to Budget";
                btnRejectUploadBud.Visible = false;
            }
            




        }

        protected void btnRejectUploadBud_Click(object sender, EventArgs e)
        {
            btnUploadToBudget.Text = "Upload to Budget";
            btnRejectUploadBud.Visible = false;
        }




        #region Model
        public class model_GV_Check_DHL
        {
            public DateTime DateProcess { get; set; }
            public string Docno { get; set; }
            public double Price { get; set; }
            public string sitestorage { get; set; }
            public string Docno_Bud { get; set; }
            public string Department { get; set; }
            public string TrackingNo { get; set; }
            public DateTime? Date_Budget { get; set; }
        }


        public class modelTOExport_DHL
        {
            public string Docno { get; set; }
            public string Tracking_No { get; set; }
            public DateTime? Date_Process { get; set; }
            public double Price { get; set; }
            public int department_id { get; set; }
            public string Docno_Budget { get; set; }
            public string Shop { get; set; }
        }
        #endregion

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var datest = Convert.ToDateTime(txtDateSt.Text);
            var dateed = Convert.ToDateTime(txtDateED.Text).AddDays(1);
            List<model_GV_Check_DHL> dhl = new List<model_GV_Check_DHL>();

            var car = carrier_Entities.DHL_eCom_Import.Where(w => w.Date_Process >= datest && w.Date_Process <= dateed).ToList();
            foreach(var item in car)
            {
                model_GV_Check_DHL dhl_one = new model_GV_Check_DHL();
                dhl_one.DateProcess = (item.Date_Process??DateTime.Now);
                dhl_one.Docno = item.Docno;
                var depart = budget_Entities.Departments.Where(w => w.Department_ID == item.department_id.ToString()).FirstOrDefault();
                dhl_one.Department = "(" + depart.ShortBrand + ")" + depart.Department_Name;
                dhl_one.TrackingNo = item.Tracking_No;
                dhl_one.sitestorage = item.Shop;
                dhl_one.Date_Budget = item.Date_Budget;
                dhl_one.Docno_Bud = item.Docno_Budget;
                dhl_one.Price = item.Price??0;
                dhl.Add(dhl_one);
            }

            gv_Import.DataSource = dhl.OrderBy(o => o.DateProcess).ToList();
            gv_Import.DataBind();
            dv_gv_import_Check.Visible = true;
        }
    }

}