using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.Carrier;
using Carrier.Model.Budget;
using Carrier.Service;

namespace Carrier
{
    public partial class Calculate_Lalamove : System.Web.UI.Page
    {
        CarrierEntities carrier_Entities = new CarrierEntities();
        BudgetEntities budget_Entities = new BudgetEntities();
        Service_Flash service_Flash = new Service_Flash();
        protected void Page_Load(object sender, EventArgs e)
        {
            //HttpContext.Current.Session["_UserID"] = "101635";
            if (Session["_UserID"] == null)
            {
                service_Flash.Check_UserID();
            }
            if (Session["_UserID"] == null)
            {
                Response.Redirect("http://www.sfg-th.com/Login/");
            }
            lbuserid.Text = Session["_UserID"].ToString();
            if (!IsPostBack)
            {
                txtDateStart.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDateEnd.Text = DateTime.Now.ToString("dd/MM/yyyy");
                LoadData();
            }
        }

        public void LoadData()
        {
            var dateST = Convert.ToDateTime(txtDateStart.Text);
            var dateED = Convert.ToDateTime(txtDateEnd.Text);
            dateST = new DateTime(dateST.Year, dateST.Month, dateST.Day, 0, 0, 1);
            dateED = new DateTime(dateED.Year, dateED.Month, dateED.Day, 23, 59, 59);
            var lalamove = (from b in carrier_Entities.Order_Big_Box
                            join o in carrier_Entities.Orders on b.Docno equals o.Docno
                            join i in carrier_Entities.Order_Item on o.Docno equals i.Docno
                            where o.Transport_Type == 2 && i.Status == "SL" && (i.Date_Success >= dateST && i.Date_Success <= dateED)  && ( b.StatusCal == null || b.StatusCal == "")
                            select new
                            {
                                b.BFID,
                                o.Docno,
                                i.TypeSendKO,
                                i.Date_Success,
                                i.Qty,
                                o.SDpart,
                                o.siteStorage

                            }).ToList();

            gv_Car.DataSource = lalamove.GroupBy(g=> new
            {
                g.BFID,
                g.Docno,
                g.TypeSendKO,
                g.SDpart,
                g.siteStorage,
                g.Date_Success
            }).Select(s=>new
            {
                s.Key.BFID,
                s.Key.Docno,
                s.Key.TypeSendKO,
                s.Key.SDpart,
                s.Key.siteStorage,
                s.Key.Date_Success,
                QTY = s.Sum(x=>x.Qty)
            }).OrderBy(o=>o.Date_Success);
            gv_Car.DataBind();
            var CalculateCar = carrier_Entities.Calculate_Car.Where(w => (w.DeliveryNumber == txtDeliveryNumber.Text || txtDeliveryNumber.Text == "")&& w.Date_Group >= dateST && w.Date_Group <= dateED)
                .Select(s => new groupOrderCar
                {
                    BFID = s.BFID,
                    Docno = s.Docno,
                    DeliveryNumber = s.DeliveryNumber,
                    Date_Success = s.Date_Group ?? DateTime.Now,
                    TypeSendKO = s.TypeSendKO,
                    SDpart = s.SDpart,
                    SiteStorage = s.SiteStorage,
                    Qty = s.QTY ?? 0,
                    Price = ""+(s.Price??0),
                    New = ""
                }).OrderBy(o=>o.Date_Success).ToList();
            gv_Group.DataSource = CalculateCar;
            gv_Group.DataBind();
            if(CalculateCar.Count() != 0)
            {
                dv_Group.Visible = true;
            }
            else
            {
                dv_Group.Visible = false;
            }

            txtDeliveryNumber.Text = "";
            txtPriceCar.Text = "";

            foreach(GridViewRow row in gv_Car.Rows)
            {
                Label lbBrandID = (Label)row.FindControl("lbBrandID");
                Label lbBrandname = (Label)row.FindControl("lbBrandname");

                var brand = budget_Entities.Departments.Where(w => w.Department_ID == lbBrandID.Text).FirstOrDefault();
                lbBrandname.Text = brand.Department_Name;
            }

            foreach(GridViewRow row in gv_Group.Rows)
            {
                Label lbBrandID = (Label)row.FindControl("lbBrandID");
                Label lbBrandname = (Label)row.FindControl("lbBrandname");

                var brand = budget_Entities.Departments.Where(w => w.Department_ID == lbBrandID.Text).FirstOrDefault();
                lbBrandname.Text = brand.Department_Name;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void btnGroup_Click(object sender, EventArgs e)
        {
            
            if(txtDeliveryNumber.Text == "")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('โปรดกรอก DeliveryNumber')", true);
                txtDeliveryNumber.Focus();
                return;
            }

            //if(!Double.TryParse(txtPriceCar.Text, out _))
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('โปรดกรอกราคา')", true);
            //    txtPriceCar.Focus();
            //    return;
            //}




            var dateST = Convert.ToDateTime(txtDateStart.Text);
            var dateED = Convert.ToDateTime(txtDateEnd.Text);
            dateST = new DateTime(dateST.Year, dateST.Month, dateST.Day, 0, 0, 1);
            dateED = new DateTime(dateED.Year, dateED.Month, dateED.Day, 23, 59, 59);
            List<groupOrderCar> orderall = new List<groupOrderCar>();
            foreach(GridViewRow row in gv_Car.Rows)
            {
                Label lbOrder = (Label)row.FindControl("lbOrder");
                CheckBox chGroup = (CheckBox)row.FindControl("chGroup");
                if (chGroup.Checked)
                {
                    var order = (from b in carrier_Entities.Order_Big_Box
                                 join o in carrier_Entities.Orders on b.Docno equals o.Docno
                                 join i in carrier_Entities.Order_Item on o.Docno equals i.Docno
                                 where o.Docno == lbOrder.Text
                                 select new groupOrderCar
                                 {
                                     BFID = b.BFID,
                                     Docno = b.Docno,
                                     Qty = i.Qty ?? 0,
                                     TypeSendKO = i.TypeSendKO,
                                     SDpart = o.SDpart,
                                     SiteStorage = o.siteStorage
                                 });

                    orderall.AddRange(order);
                }
                



            }
            //var box = orderall.Sum(s => s.Qty);
            //var pricePerBox = txtPriceCar.Text == "" ? 0 :Convert.ToDouble(txtPriceCar.Text) / box;
            foreach (var i in orderall)
            {
                i.DeliveryNumber = txtDeliveryNumber.Text;
                i.Price = 0.ToString("#,##0.00");
                i.New = "NEW";
                i.Date_Success = DateTime.Now;
            }
            var oldgroup = carrier_Entities.Calculate_Car.Where(w => w.Date_Group >= dateST && w.Date_Group <= dateED && w.DeliveryNumber == txtDeliveryNumber.Text)
                .Select(s => new groupOrderCar
                {
                    DeliveryNumber = s.DeliveryNumber,
                    BFID = s.BFID,
                    Docno = s.Docno,
                    TypeSendKO = s.TypeSendKO,
                    SDpart = s.SDpart,
                    SiteStorage = s.SiteStorage,
                    Qty = s.QTY ?? 0,
                    Price = s.Price.ToString(),
                    Date_Success = s.Date_Group ?? DateTime.Now
                }).ToList();
            oldgroup.AddRange(orderall);

            if(txtPriceCar.Text != "")
            {
                var box = oldgroup.Sum(s => s.Qty);
                var pricePerBox = txtPriceCar.Text == "" ? 0 : Convert.ToDouble(txtPriceCar.Text) / box;
                foreach (var i in oldgroup)
                {
                    i.Price = (pricePerBox * i.Qty).ToString("#,##0.00");
                }
            }
            
            gv_Group.DataSource = oldgroup.OrderByDescending(o=>o.Date_Success);
            gv_Group.DataBind();
            dv_Group.Visible = true;
            btnSave.Enabled = true;

            foreach(GridViewRow row in gv_Group.Rows)
            {
                Label lbBrandname = (Label)row.FindControl("lbBrandname");
                Label lbBrandID = (Label)row.FindControl("lbBrandID");
                var brand = budget_Entities.Departments.Where(w => w.Department_ID == lbBrandID.Text).FirstOrDefault();
                lbBrandname.Text = brand.Department_Name;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow row in gv_Group.Rows)
                {
                    Label lbDelivery = (Label)row.FindControl("lbDelivery");
                    Label lbOrderBig = (Label)row.FindControl("lbOrderBig");
                    Label lbOrder = (Label)row.FindControl("lbOrder");
                    Label lbTypeSendKO = (Label)row.FindControl("lbTypeSendKO");
                    Label lbBox = (Label)row.FindControl("lbBox");
                    Label lbPrice = (Label)row.FindControl("lbPrice");
                    Label lbNew = (Label)row.FindControl("lbNew");
                    Label lbBrandID = (Label)row.FindControl("lbBrandID");
                    Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");

                    if (lbNew.Text != "")
                    {
                        Calculate_Car cal = new Calculate_Car();
                        cal.BFID = lbOrderBig.Text;
                        cal.Docno = lbOrder.Text;
                        cal.DeliveryNumber = lbDelivery.Text;
                        cal.Date_Group = DateTime.Now;
                        cal.TypeSendKO = lbTypeSendKO.Text;
                        cal.QTY = Convert.ToInt32(lbBox.Text);
                        cal.Price = Convert.ToDouble(lbPrice.Text);
                        cal.SDpart = lbBrandID.Text;
                        cal.SiteStorage = lbSiteStorage.Text;
                        carrier_Entities.Calculate_Car.Add(cal);
                        carrier_Entities.SaveChanges();

                        var bigbox = carrier_Entities.Order_Big_Box.Where(w => w.Docno == lbOrder.Text && w.BFID == lbOrderBig.Text).FirstOrDefault();
                        bigbox.StatusCal = "F";
                        carrier_Entities.SaveChanges();
                    }
                    else
                    {
                        var car = carrier_Entities.Calculate_Car.Where(w => w.BFID == lbOrderBig.Text && w.DeliveryNumber == lbDelivery.Text && w.Docno == lbOrder.Text).FirstOrDefault();
                        car.Price = Convert.ToDouble(lbPrice.Text);
                        carrier_Entities.SaveChanges();
                    }
                }
                btnSave.Enabled = false;
                txtDeliveryNumber.Text = "";
                txtPriceCar.Text = "";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกสำเร็จ')", true);
                LoadData();
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกไม่สำเร็จ Message:"+ ex.Message + "')", true);
            }



        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var dateST = Convert.ToDateTime(txtDateStart.Text);
            var dateED = Convert.ToDateTime(txtDateEnd.Text);
            dateST = new DateTime(dateST.Year, dateST.Month, dateST.Day, 0, 0, 1);
            dateED = new DateTime(dateED.Year, dateED.Month, dateED.Day, 23, 59, 59);
            GridView calGrid = new GridView();
            var cal = carrier_Entities.Calculate_Car.Where(w => w.Date_Group >= dateST && w.Date_Group <= dateED).OrderBy(o=>o.Date_Group).ToList();
            if (cal.Count == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่พบข้อมูลในช่วงเวลาที่เลือกครับ')", true);
            }
            else
            {
                var docno = cal.Select(s => s.Docno).ToList();
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
                foreach(var i in order)
                {
                    modelExport item = new modelExport();
                    item.วันที่_Request = i.Date_Success?? DateTime.Now;
                    item.เลขORDER = i.Docno;
                    item.ร้าน = i.siteStorage;
                    var brand = budget_Entities.Departments.Where(w => w.Department_ID == i.SDpart).FirstOrDefault();
                    item.Brand = brand.Department_Name;
                    item.จำนวนกล่อง_แปลง = i.Qty??0;
                    item.ค่ารถ = carrier_Entities.Calculate_Car.Where(w => w.Docno == i.Docno).FirstOrDefault().Price ?? 0;
                    item.TaxCode = "VX";
                    item.DeliveryNumber = carrier_Entities.Calculate_Car.Where(w => w.Docno == i.Docno).FirstOrDefault().DeliveryNumber;
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
                        
                        if(site != null)
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
                //GridViewRow rowHead = (GridViewRow)history.HeaderRow;
                //Label History_NO = (Label)rowHead.FindControl("History_NO");
                //Label Date_Notify = (Label)rowHead.FindControl("Date_Notify");
                //Label Pno = (Label)rowHead.FindControl("Pno");
                //Label Docno = (Label)rowHead.FindControl("Docno");
                //Label Type_Send_KA = (Label)rowHead.FindControl("Type_Send_KA");
                //Export Excel
                Page.Response.ClearContent();
                Page.Response.AddHeader("Content-Disposition", "attachment;filename=" + "Calculate_Car_Lalamove" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".xls");
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
    }


    public class groupOrderCar
    {
        public string DeliveryNumber { get; set; }
        public string BFID { get; set; }
        public string Docno { get; set; }
        public string TypeSendKO { get; set; }
        public string SDpart { get; set; }
        public string SiteStorage { get; set; }
        public DateTime Date_Success { get; set; }
        public int Qty { get; set; }
        public string Price { get; set; }
        public string New { get; set; }

    }

    public class modelExport
    {
        public string เลขORDER { get; set; }
        public string วันที่truck        { get; set; }
        public string วันที่Ship        { get; set; }
        public DateTime วันที่_Request { get; set; }
        //public string เลขที่เอกสาร { get; set; }
        public string ร้าน { get; set; }
        public string Brand { get; set; }
        public string Route        { get; set; }
        public string DeliveryNumber        { get; set; }
        public string ประเภทขนส่ง        { get; set; }
        public string ทะเบียนรถ        { get; set; }
        public string กล่องที่        { get; set; }
        public string จำนวนชิ้น        { get; set; }
        public string ขนาดกล่อง_จริง        { get; set; }
        public int จำนวนกล่อง_แปลง { get; set; }
        public string เลขที่รอบรถ        { get; set; }
        public double ค่ารถ        { get; set; }
        public string จำนวนกล่องทั้งหมดในรอบรถ        { get; set; }
        public string ค่ารถเฉลี่ย        { get; set; }
        public string TaxCode { get; set; }
        public string ProfitCenter { get; set; }
        public string CostCenter { get; set; }
        public string ServiceCostCenter        { get; set; }
        public string Shop { get; set; }
        public string Assignment { get; set; }
    }
}