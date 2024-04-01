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

namespace Carrier
{
    public partial class Transport_Form_Lalamove : System.Web.UI.Page
    {
        Service_Flash service_Flash = new Service_Flash();
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
            }

        }

        public void loadData()
        {
            var datest = Convert.ToDateTime(txtDateSt.Text);
            var dateed = Convert.ToDateTime(txtDateED.Text);
            var Delivery = carrier_Entities.Calculate_Car.Where(w => w.Date_Group >= datest && w.Date_Group <= dateed).GroupBy(g=>g.DeliveryNumber).Select(s=>s.Key).ToList();
            List<Calculate_Car> cars = new List<Calculate_Car>();
            foreach(var del in Delivery)
            {
                Calculate_Car car = new Calculate_Car();
                var docno = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == del).ToList();
                var price = docno.Sum(s => s.Price);
                car.DeliveryNumber = del;
                foreach(var s in docno.GroupBy(g => g.SiteStorage).OrderBy(o => o.Key).ToList())
                {
                    if(s == docno.GroupBy(g => g.SiteStorage).OrderByDescending(o => o.Key).FirstOrDefault())
                    {
                        car.SiteStorage += s.Key;
                    }
                    else
                    {
                        car.SiteStorage += s.Key + ",";
                    }
                }

                foreach(var d in docno.GroupBy(g => g.SDpart).Select(s => s.Key).OrderBy(o=>o).ToList())
                {
                    var dep = budget_Entities.Departments.Where(w => w.Department_ID == d).FirstOrDefault();
                    if(dep!= null)
                    {
                        if(d == docno.GroupBy(g => g.SDpart).Select(s => s.Key).OrderByDescending(o => o).FirstOrDefault())
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
                cars.Add(car);
            }
            gv_main.DataSource = cars;
            gv_main.DataBind();
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
            foreach(var site in sites)
            {
                modelDetail mde = new modelDetail();
                mde.site = site;
                var siteDigit = new string[] { "ZZ", "ZX", "Z6", "Z7" }.Contains(site.Substring(0, 2)) ? site.Substring(0, 6) : site.Substring(0, 4);
                var Cuscode = InsideSFG_WF_Entities.Customer_Tax.Where(w => w.CustomerCode == siteDigit).FirstOrDefault();
                var provineInt = Convert.ToInt32(Cuscode.Province1);
                mde.address = Cuscode.Address1 + " " + Cuscode.Area1 + " " + Cuscode.Zone1 + " " + Cuscode.Road1 + " " + Whale_Entities.Provinces.Where(w => w.Province_ID == provineInt).FirstOrDefault().Province_Name + " " + Cuscode.Postal1;
                var listInSite = cal.Where(w => w.SiteStorage == site).ToList();
                foreach (var lis in listInSite)
                {
                    modelDetail_Sub sub = new modelDetail_Sub();
                    sub.SiteStorage = site;
                    sub.SDpart = lis.SDpart;
                    sub.TypeSendKO = lis.TypeSendKO;
                    sub.QTY = lis.QTY??0;
                    mde.sub.Add(sub);
                    
                }
                Mdetail.Add(mde);
            }
            gv_detail.DataSource = Mdetail;
            gv_detail.DataBind();

            foreach(GridViewRow row in gv_detail.Rows)
            {
                GridView gv_Detail_Sub = (GridView)row.FindControl("gv_Detail_Sub");
                Label lbDetail_SiteStorage = (Label)row.FindControl("lbDetail_SiteStorage");
                var sub = Mdetail.Where(w => w.site == lbDetail_SiteStorage.Text).Select(s => s.sub).FirstOrDefault();
                var department = budget_Entities.Departments.ToList();
                gv_Detail_Sub.DataSource = sub.GroupBy(g => new { SDpart =  g.SDpart }).Select(s => new
                {
                    SDpart = department.Where(w => w.Department_ID == s.Key.SDpart).FirstOrDefault().Department_Name,
                    QTY = s.Sum(c=> c.QTY)
                }) ;
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
            loadData();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            GridView gv = new GridView();
            gv.DataSource = loadExport();
            gv.DataBind();
            Page.Response.ClearContent();
            Page.Response.AddHeader("Content-Disposition", "attachment;filename=" + "Car_Lalamove_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".xls");
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Page.Response.Charset = "utf-8";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-874");
            Page.Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter strwritter = new StringWriter())
            {
                HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
                htmltextwrtter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                htmltextwrtter.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=windows-874'>");
                gv.AllowPaging = false;
                gv.HeaderRow.BackColor = System.Drawing.Color.Yellow;
                gv.RenderControl(htmltextwrtter);
                Page.Response.Output.Write(strwritter.ToString());
                Page.Response.End();
            }
        }
        public List<modelExportCal> loadExport()
        {
            var datest = Convert.ToDateTime(txtDateSt.Text);
            var dateed = Convert.ToDateTime(txtDateED.Text);
            var Delivery = carrier_Entities.Calculate_Car.Where(w => w.Date_Group >= datest && w.Date_Group <= dateed).GroupBy(g => g.DeliveryNumber).Select(s => s.Key).ToList();
            List<modelExportCal> cars = new List<modelExportCal>();
            foreach (var del in Delivery)
            {
                
                var docno = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == del).ToList();
                var price = docno.Sum(s => s.Price);
                foreach (var s in docno)
                {
                    modelExportCal car = new modelExportCal();
                    var dep = budget_Entities.Departments.Where(w => w.Department_ID == s.SDpart).FirstOrDefault();
                    car.DeliveryOrder = s.DeliveryNumber;
                    car.แผนก = dep.Department_Name;
                    car.เลขที่เอกสาร = s.BFID;
                    car.เลขที่กล่อง = s.Docno;
                    car.SiteStorage = s.SiteStorage;
                    car.จำนวนกล่อง = (s.QTY??0).ToString("#,##0.00");
                    car.ราคาต่อกล่อง = (s.Price??0).ToString("#,##0.00");

                    var siteDigit = new string[] { "ZZ", "ZX", "Z6", "Z7" }.Contains(s.SiteStorage.Substring(0, 2)) ? s.SiteStorage.Substring(0, 6) : s.SiteStorage.Substring(0, 4);
                    var Cuscode = InsideSFG_WF_Entities.Customer_Tax.Where(w => w.CustomerCode == siteDigit).FirstOrDefault();
                    var provineInt = Convert.ToInt32(Cuscode.Province1);
                    car.ที่อยู๋จัดส่ง = Cuscode.Address1 + " " + Cuscode.Area1 + " " + Cuscode.Zone1 + " " + Cuscode.Road1 + " " + Whale_Entities.Provinces.Where(w => w.Province_ID == provineInt).FirstOrDefault().Province_Name + " " + Cuscode.Postal1;
                    cars.Add(car);
                }

               

            }
            return cars;
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
        public string Price { get; set; }
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
}