using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Service;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;

namespace Carrier
{
    public partial class Edit_Add_Profit : System.Web.UI.Page
    {
        Service_Flash service_Flashs = new Service_Flash();
        CarrierEntities carrier_Entities = new CarrierEntities();
        InsideSFG_WFEntities insideSFG_WF_Entities = new InsideSFG_WFEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_UserID"] == null)
            {
                service_Flashs.Check_UserID();
            }
            if (Session["_UserID"] == null)
            {
                Response.Redirect("https://www.sfg-th.com/Login/");
            }
            if (!IsPostBack)
            {
                loadBrand();
                loadProfit("");
            }

        }
        public void loadBrand()
        {

            var FC = insideSFG_WF_Entities.BG_ForeCast.Where(w => w.ActiveStatus == 1).GroupBy(g => g.DepartmentID).Select(s => new Forecasts { DepartmentID = s.Key });
            var depart = (from BG_HA in insideSFG_WF_Entities.BG_HApprove
                          join BG_HAPF in insideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
                          where FC.Select(s => s.DepartmentID).Contains(BG_HA.departmentID) && (BG_HA.Sta == "B" || BG_HA.Sta == "S" || BG_HA.Sta == "N")
                          select new BrandShowInfo
                          {
                              name = BG_HA.department_,
                              nameShot = BG_HAPF.Depart_Short
                          }).OrderBy(r => r.name).ToList();
            foreach(var dep in depart)
            {
                var depCen = carrier_Entities.Site_Center.Where(w => w.Brand_Center_Short == dep.nameShot).FirstOrDefault();
                if(depCen != null)
                {
                    dep.name = "(" + depCen.Brand_Center_Name_Full + ")" + dep.name;
                    dep.nameShot = depCen.Brand_Center_Name_Full;
                }
                else
                {
                    dep.name = "("+dep.nameShot+")"+dep.name;
                }
            }
            depart.Insert(0,new BrandShowInfo { name = "--- Selected ---", nameShot = "--- Selected ---" });
            ddlBrandSearch.DataSource = depart.OrderBy(r => r.name).ToList();
            ddlBrandSearch.DataBind();
            ddlBrandADD.DataSource = depart.OrderBy(r => r.name).ToList();
            ddlBrandADD.DataBind();
        }
        public void loadProfit(string siteStorage)
        {
            var BG = (from ha in insideSFG_WF_Entities.BG_HApprove
                      join haP in insideSFG_WF_Entities.BG_HApprove_Profitcenter on ha.departmentID equals haP.DepartmentID
                      where haP.Depart_Short == ddlBrandSearch.SelectedValue
                      select haP).ToList();
            var departShot = ddlBrandSearch.SelectedValue;
            var center = carrier_Entities.Site_Center.Where(w => w.Brand_Center_Short == departShot).FirstOrDefault();
            var profit = carrier_Entities.Site_Profit.ToList();
            if (center != null)
            {
                profit = profit.Where(w => w.Brand == center.Brand_Center_Name_Full).ToList();
            }
            else
            {
                profit = profit.Where(w => w.Brand == departShot).ToList();
            }
            
            if (siteStorage != "")
            {
                profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage.StartsWith(siteStorage)).ToList();
            }
            gv_main.DataSource = profit.OrderBy(o=>o.Site_Stroage);
            gv_main.DataBind();
            
            var saleChannel = carrier_Entities.Site_Profit.Select(s => new { s.Sale_Channel }).Distinct().ToList();
            ddlSaleChannelADD.DataSource = saleChannel;
            ddlSaleChannelADD.DataBind();
            foreach (GridViewRow row in gv_main.Rows)
            {
                DropDownList ddlSaleChannel = (DropDownList)row.FindControl("ddlSaleChannel");
                ddlSaleChannel.DataSource = saleChannel;
                ddlSaleChannel.DataBind();
                Label lbSaleChannel = (Label)row.FindControl("lbSaleChannel");
                ddlSaleChannel.SelectedValue = lbSaleChannel.Text;

                DropDownList ddlChannel = (DropDownList)row.FindControl("ddlChannel");
                Label lbChannel = (Label)row.FindControl("lbChannel");
                ddlChannel.SelectedValue = lbChannel.Text;

            }
            txtSiteStorageADD.Text = "";
            txtComcodeADD.Text = "";
            txtProfitADD.Text = "";
            txtCostcenterADD.Text = "";
            System.Threading.Thread.Sleep(1000);
        }
        protected void ddlBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSite.Text = "";
            loadProfit("");
        }

        protected void imgbtnEdit_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnEdit = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnEdit.NamingContainer;
            ImageButton imgbtnSave = (ImageButton)row.FindControl("imgbtnSave");
            ImageButton imgbtnCancel = (ImageButton)row.FindControl("imgbtnCancel");
            ImageButton imgbtnDel = (ImageButton)row.FindControl("imgbtnDel");
            imgbtnSave.Visible = true;
            imgbtnEdit.Visible = false;
            imgbtnCancel.Visible = true;
            imgbtnDel.Visible = false;

            Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
            lbSiteStorage.Visible = false;
            TextBox txtSiteStorage = (TextBox)row.FindControl("txtSiteStorage");
            txtSiteStorage.Text = lbSiteStorage.Text;
            txtSiteStorage.Visible = true;

            Label lbChannel = (Label)row.FindControl("lbChannel");
            lbChannel.Visible = false;
            DropDownList ddlChannel = (DropDownList)row.FindControl("ddlChannel");
            ddlChannel.Visible = true;

            Label lbSaleChannel = (Label)row.FindControl("lbSaleChannel");
            lbSaleChannel.Visible = false;
            DropDownList ddlSaleChannel = (DropDownList)row.FindControl("ddlSaleChannel");
            ddlSaleChannel.Visible = true;

            Label lbComcode = (Label)row.FindControl("lbComcode");
            lbComcode.Visible = false;
            TextBox txtComcode = (TextBox)row.FindControl("txtComcode");
            txtComcode.Text = lbComcode.Text;
            txtComcode.Visible = true;

            Label lbProfit = (Label)row.FindControl("lbProfit");
            lbProfit.Visible = false;
            TextBox txtProfit = (TextBox)row.FindControl("txtProfit");
            txtProfit.Text = lbProfit.Text;
            txtProfit.Visible = true;

            Label lbCostcenter = (Label)row.FindControl("lbCostcenter");
            lbCostcenter.Visible = false;
            TextBox txtCostcenter = (TextBox)row.FindControl("txtCostcenter");
            txtCostcenter.Text = lbCostcenter.Text;
            txtCostcenter.Visible = true;

        }

        protected void imgbtnSave_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnSave = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnSave.NamingContainer;
            ImageButton imgbtnEdit = (ImageButton)row.FindControl("imgbtnEdit");
            ImageButton imgbtnCancel = (ImageButton)row.FindControl("imgbtnCancel");
            ImageButton imgbtnDel = (ImageButton)row.FindControl("imgbtnDel");
            Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
            TextBox txtSiteStorage = (TextBox)row.FindControl("txtSiteStorage");
            Label lbBrandTemp = (Label)row.FindControl("lbBrandTemp");
            Label lbChannel = (Label)row.FindControl("lbChannel");
            DropDownList ddlChannel = (DropDownList)row.FindControl("ddlChannel");
            Label lbSaleChannel = (Label)row.FindControl("lbSaleChannel");
            DropDownList ddlSaleChannel = (DropDownList)row.FindControl("ddlSaleChannel");
            Label lbComcode = (Label)row.FindControl("lbComcode");
            TextBox txtComcode = (TextBox)row.FindControl("txtComcode");
            Label lbProfit = (Label)row.FindControl("lbProfit");
            TextBox txtProfit = (TextBox)row.FindControl("txtProfit");
            Label lbCostcenter = (Label)row.FindControl("lbCostcenter");
            TextBox txtCostcenter = (TextBox)row.FindControl("txtCostcenter");

                var profit = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == lbSiteStorage.Text && w.Brand == lbBrandTemp.Text).FirstOrDefault();
            if (lbSiteStorage.Text != txtSiteStorage.Text)
            {
                Site_Profit profitNew = new Site_Profit()
                {
                    Brand = lbBrandTemp.Text,
                    Site_Stroage = txtSiteStorage.Text,
                    Channel = ddlChannel.SelectedValue,
                    Sale_Channel = ddlSaleChannel.SelectedValue,
                    COMCODE = txtComcode.Text,
                    Costcenter = txtCostcenter.Text,
                    Profit = txtProfit.Text
                };
                var old = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == lbSiteStorage.Text && w.Brand == lbBrandTemp.Text).FirstOrDefault();
                carrier_Entities.Site_Profit.Remove(old);
                carrier_Entities.Site_Profit.Add(profitNew);
            }
            else
            {
                if (ddlChannel.SelectedValue != "--SELECT--")
                {
                    profit.Channel = ddlChannel.SelectedValue;
                }
                profit.Sale_Channel = ddlSaleChannel.SelectedValue;
                profit.COMCODE = txtComcode.Text;
                profit.Profit = txtProfit.Text;
                profit.Costcenter = txtCostcenter.Text;
            }
           
            try
            {
                carrier_Entities.SaveChanges();

            }catch(Exception ex)
            {
                service_Flashs.SendMail("apichat.f@sfg-th.com", new string[] { }, "Site_Profit_Add_New ERROR",
                    "<HTML>" +
                "<body>" +
                "<p>SiteStorage : " + profit.Site_Stroage + "</p>" +
                "<p>Brand : " + lbBrandTemp.Text + "</p>" +
                "<p>Channel : " + profit.Channel + "</p>" +
                "<p>Sale_Channel : " + profit.Sale_Channel + "</p>" +
                "<p>COMCODE : " + profit.COMCODE + "</p>" +
                "<p>Profit : " + profit.Profit + "</p>" +
                "<p>Costcenter : " + profit.Costcenter + "</p>" +
                "<p>ERROR : " + ex.Message + "</p>" +
                "</body>" +
                "</HTML>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Process : ERROR! Update Entity กรุณาติดต่อ dev ')", true);

            }
            imgbtnCancel.Visible = false;
            imgbtnEdit.Visible = true;
            imgbtnSave.Visible = false;
            imgbtnDel.Visible = true;

            lbSiteStorage.Visible = true;
            lbSiteStorage.Text = txtSiteStorage.Text;
            txtSiteStorage.Visible = false;

            lbChannel.Visible = true;
            ddlChannel.Visible = false;
            lbChannel.Text = ddlChannel.SelectedValue;

            lbSaleChannel.Visible = true;
            ddlSaleChannel.Visible = false;
            lbSaleChannel.Text = ddlSaleChannel.SelectedValue;

            lbComcode.Visible = true;
            lbComcode.Text = txtComcode.Text;
            txtComcode.Visible = false;

            lbProfit.Visible = true;
            lbProfit.Text = txtProfit.Text;
            txtProfit.Visible = false;

            lbCostcenter.Visible = true;
            lbCostcenter.Text = txtCostcenter.Text;
            txtCostcenter.Visible = false;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            loadProfit(txtSite.Text);
        }

        protected void imgbtnCancel_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnCancel = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnCancel.NamingContainer;
            ImageButton imgbtnDel = (ImageButton)row.FindControl("imgbtnDel");
            ImageButton imgbtnEdit = (ImageButton)row.FindControl("imgbtnEdit");
            ImageButton imgbtnSave = (ImageButton)row.FindControl("imgbtnSave");
            Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
            TextBox txtSiteStorage = (TextBox)row.FindControl("txtSiteStorage");
            Label lbBrandTemp = (Label)row.FindControl("lbBrandTemp");
            Label lbChannel = (Label)row.FindControl("lbChannel");
            DropDownList ddlChannel = (DropDownList)row.FindControl("ddlChannel");
            Label lbSaleChannel = (Label)row.FindControl("lbSaleChannel");
            DropDownList ddlSaleChannel = (DropDownList)row.FindControl("ddlSaleChannel");
            Label lbComcode = (Label)row.FindControl("lbComcode");
            TextBox txtComcode = (TextBox)row.FindControl("txtComcode");
            Label lbProfit = (Label)row.FindControl("lbProfit");
            TextBox txtProfit = (TextBox)row.FindControl("txtProfit");
            Label lbCostcenter = (Label)row.FindControl("lbCostcenter");
            TextBox txtCostcenter = (TextBox)row.FindControl("txtCostcenter");

            imgbtnCancel.Visible = false;
            imgbtnEdit.Visible = true;
            imgbtnSave.Visible = false;
            imgbtnDel.Visible = true;

            lbSiteStorage.Visible = true;
            txtSiteStorage.Visible = false;

            lbChannel.Visible = true;
            ddlChannel.Visible = false;


            lbSaleChannel.Visible = true;
            ddlSaleChannel.Visible = false;

            lbComcode.Visible = true;
            txtComcode.Visible = false;

            lbProfit.Visible = true;
            txtProfit.Visible = false;

            lbCostcenter.Visible = true;
            txtCostcenter.Visible = false;
        }

        protected void imgbtnAdd_Click(object sender, ImageClickEventArgs e)
        {
            
            div_main.Style.Add("filter", "blur(15px)");
            div_main.Style.Add("position", "absolute");

            div_ADD.Visible = true;
            ddlBrandADD.SelectedValue = ddlBrandSearch.SelectedValue;
            loadProfit("");
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            div_main.Style.Remove("filter");
            div_main.Style.Remove("position");
            div_ADD.Visible = false;
        }

        protected void btnSaveAdd_Click(object sender, EventArgs e)
        {
                Site_Profit site = new Site_Profit();
            try
            {
                site.Brand = ddlBrandADD.SelectedValue;
                site.Site_Stroage = txtSiteStorageADD.Text;
                site.Sale_Channel = ddlSaleChannelADD.SelectedValue;
                site.Channel = ddlChannelADD.SelectedValue;
                site.COMCODE = txtComcodeADD.Text;
                site.Profit = txtProfitADD.Text;
                site.Costcenter = txtCostcenterADD.Text;
                carrier_Entities.Site_Profit.Add(site);
                carrier_Entities.SaveChanges();
            }catch(Exception ex)
            {
                service_Flashs.SendMail("apichat.f@sfg-th.com",new string[] { },"Site_Profit_Add_New ERROR",
                    "<HTML>" +
                "<body>" +
                "<p>SiteStorage : " + site.Site_Stroage + "</p>" +
                "<p>Brand : " + ddlBrandADD.SelectedItem.Text + "(" + site.Brand + ")" + "</p>" +
                "<p>Channel : " + site.Channel + "</p>" +
                "<p>Sale_Channel : " + site.Sale_Channel + "</p>"+
                "<p>COMCODE : " + site.COMCODE + "</p>"+
                "<p>Profit : " + site.Profit + "</p>"+
                "<p>Costcenter : " + site.Costcenter + "</p>"+
                "<p>ERROR : " + ex.Message + "</p>"+
                "</body>"+
                "</HTML>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Process : ERROR! ได้ทำการส่งไปทาง Dev เรียบร้อยโปรดรอการปรับปรุง ')", true);
            }
            btnClose_Click(this, EventArgs.Empty);
            loadProfit("");
        }

        protected void imgbtnDel_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnDel = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnDel.NamingContainer;
            Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
            Label lbBrandTemp = (Label)row.FindControl("lbBrandTemp");
            var del = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == lbSiteStorage.Text && w.Brand == lbBrandTemp.Text).FirstOrDefault();
            carrier_Entities.Site_Profit.Remove(del);
            carrier_Entities.SaveChanges();
        }
    }
    public class BrandShowInfo
    {
        public string name { get; set; }
        public string nameShot { get; set; }
    }
}