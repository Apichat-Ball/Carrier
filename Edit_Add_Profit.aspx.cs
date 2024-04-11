using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Service;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Budget;

namespace Carrier
{
    public partial class Edit_Add_Profit : System.Web.UI.Page
    {
        Service_Flash service_Flashs = new Service_Flash();
        CarrierEntities carrier_Entities = new CarrierEntities();
        InsideSFG_WFEntities insideSFG_WF_Entities = new InsideSFG_WFEntities();
        BudgetEntities budget_Entities = new BudgetEntities();
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

            //var FC = insideSFG_WF_Entities.BG_ForeCast.Where(w => w.ActiveStatus == 1).GroupBy(g => g.DepartmentID).Select(s => new Forecasts { DepartmentID = s.Key });
            //var FC = (from d in budget_Entities.Departments
            //          join mb in budget_Entities.MainBudgets on d.Department_ID equals mb.Department_ID
            //          where new string[] { "F", "VIP" }.Contains(d.Flag)
            //          select d.Department_ID).ToList();
            //var depart = (from BG_HA in insideSFG_WF_Entities.BG_HApprove
            //              join BG_HAPF in insideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
            //              where  FC.Contains(BG_HA.departmentID) && 
            //              (BG_HA.Sta == "B" || BG_HA.Sta == "S" || BG_HA.Sta == "N") && !BG_HA.department_.Contains("SEEK")
            //              select new BrandShowInfo
            //              {
            //                  name = BG_HA.department_,
            //                  nameShot = BG_HAPF.Depart_Short
            //              }).OrderBy(r => r.name).ToList();
            //var seek = (from BG_HA in insideSFG_WF_Entities.BG_HApprove
            //            join BG_HAPF in insideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
            //            where (BG_HA.Sta == "B" || BG_HA.Sta == "S" || BG_HA.Sta == "N") && BG_HA.department_.Contains("SEEK")
            //            select new BrandShowInfo
            //            {
            //                name = BG_HA.department_,
            //                nameShot = BG_HAPF.Depart_Short
            //            }).OrderBy(r => r.name).ToList();
            //depart.AddRange(seek);

            var budget = budget_Entities.MainBudgets.Where(w => w.Year_Budget == DateTime.Now.Year).GroupBy(g => g.Department_ID).Select(s => s.Key).ToList();
            var depart = budget_Entities.Departments.Where(w => budget.Contains(w.Department_ID) && new string[] { "F", "VIP" }.Contains(w.Flag) && !w.Department_Name.Contains("SEEK") && !w.Department_Name.Contains("SDC1")).Select(s => new BrandShowInfo { name = s.Department_Name, nameShot = s.ShortBrand }).OrderBy(o=>o.name).ToList();
            var seek = budget_Entities.Departments.Where(w => w.Department_Name.Contains("SEEK") && w.Flag == "F" && !new string[] { "1508", "1619" }.Contains(w.Department_ID)).Select(s => new BrandShowInfo { name = s.Department_Name, nameShot = s.ShortBrand }).OrderBy(o => o.name).ToList();
            depart.AddRange(seek);
            foreach (var dep in depart)
            {
                var depCen = carrier_Entities.Site_Center.Where(w => w.Brand_Center_Short == dep.nameShot).FirstOrDefault();
                if(depCen != null)
                {
                    dep.name = "(" + depCen.Brand_Center_Short + ")" + dep.name;
                    dep.nameShot = depCen.Brand_Center_Short;
                }
                else
                {
                    dep.name = "("+dep.nameShot+")"+dep.name;
                }
            }
            depart.Insert(0,new BrandShowInfo { name = "--- Selected ---", nameShot = "--- Selected ---" });
            ddlBrandSearch.DataSource = depart.ToList();
            ddlBrandSearch.DataBind();
            ddlBrandADD.DataSource = depart.ToList();
            ddlBrandADD.DataBind();
        }
        public void loadProfit(string siteStorage)
        {
            //var BG = budget_Entities.Departments.Where(w => w.ShortBrand == ddlBrandSearch.SelectedValue).ToList();
            
            var departShot = ddlBrandSearch.SelectedValue;
            var center = carrier_Entities.Site_Center.Where(w => w.Brand_Center_Short == departShot).FirstOrDefault();
            var profit = carrier_Entities.Site_Profit.ToList();
            if (center != null)
            {
                profit = profit.Where(w => w.Brand == center.Brand_Center_Short).ToList();
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
                    Site_Stroage = txtSiteStorage.Text.ToUpper(),
                    Channel = ddlChannel.SelectedValue,
                    Sale_Channel = ddlSaleChannel.SelectedValue,
                    COMCODE = txtComcode.Text,
                    Costcenter = txtCostcenter.Text,
                    Profit = txtProfit.Text,
                    Date_Create = DateTime.Now
                    
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
                profit.Date_Update = DateTime.Now;
            }
           
            try
            {
                carrier_Entities.SaveChanges();
                var a = new
                {
                    Brand = lbBrandTemp.Text,
                    Site_Stroage = txtSiteStorage.Text.ToUpper(),
                    Channel = ddlChannel.SelectedValue,
                    Sale_Channel = ddlSaleChannel.SelectedValue,
                    COMCODE = txtComcode.Text,
                    Costcenter = txtCostcenter.Text,
                    Profit = txtProfit.Text
                };
                carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
                {
                    dateSend = DateTime.Now,
                    path = "Carrier/Edit_Add_Profit",
                    status = "1",
                    request = Newtonsoft.Json.JsonConvert.SerializeObject(a),
                    respon = "Success"
                });
                carrier_Entities.SaveChanges();

            }
            catch(Exception ex)
            {
                var a = new {
                    Brand = lbBrandTemp.Text,
                    Site_Stroage = txtSiteStorage.Text.ToUpper(),
                    Channel = ddlChannel.SelectedValue,
                    Sale_Channel = ddlSaleChannel.SelectedValue,
                    COMCODE = txtComcode.Text,
                    Costcenter = txtCostcenter.Text,
                    Profit = txtProfit.Text
                };
                carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
                {
                    dateSend = DateTime.Now,
                    path = "Carrier/Edit_Add_Profit",
                    status = "2",
                    request = Newtonsoft.Json.JsonConvert.SerializeObject(a),
                    respon = ex.Message
                });
                carrier_Entities.SaveChanges();


                service_Flashs.SendMail("apichat.f@sfg-th.com", new string[] { }, "Site_Profit_Edit ERROR",
                    "<HTML>" +
                "<body><p>OLD</p>" +
                "<p>SiteStorage : " + profit.Site_Stroage + "</p>" +
                "<p>Brand : " + lbBrandTemp.Text + "</p>" +
                "<p>Channel : " + profit.Channel + "</p>" +
                "<p>Sale_Channel : " + profit.Sale_Channel + "</p>" +
                "<p>COMCODE : " + profit.COMCODE + "</p>" +
                "<p>Profit : " + profit.Profit + "</p>" +
                "<p>Costcenter : " + profit.Costcenter + "</p>" +
                "<p>New</p>"+
                "<p>SiteStorage : " + txtSiteStorage.Text.ToUpper() + "</p>" +
                "<p>Brand : " + lbBrandTemp.Text + "</p>" +
                "<p>Channel : " + ddlChannel.SelectedValue + "</p>" +
                "<p>Sale_Channel : " + ddlSaleChannel.SelectedValue + "</p>" +
                "<p>COMCODE : " + txtComcode.Text + "</p>" +
                "<p>Profit : " + txtProfit.Text + "</p>" +
                "<p>Costcenter : " + txtCostcenter.Text + "</p>" +
                "<p>ERROR : " + ex.Message + "</p><p>UserID : " + Session["_UserID"].ToString() +
                "</p></body>" +
                "</HTML>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Process : ERROR! Update Entity กรุณาติดต่อ [MIS]Ball ')", true);

            }
            imgbtnCancel.Visible = false;
            imgbtnEdit.Visible = true;
            imgbtnSave.Visible = false;
            imgbtnDel.Visible = true;

            lbSiteStorage.Visible = true;
            lbSiteStorage.Text = txtSiteStorage.Text.ToUpper();
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
            div_main.Style.Add("pointer-events", "none");

            div_ADD.Visible = true;
            ddlBrandADD.SelectedValue = ddlBrandSearch.SelectedValue;
            loadProfit("");
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            div_main.Style.Remove("filter");
            div_main.Style.Remove("position");
            div_main.Style.Remove("pointer-events");
            div_ADD.Visible = false;
        }

        protected void btnSaveAdd_Click(object sender, EventArgs e)
        {
            if(gv_TempBrand.Rows.Count != 0)
            {
                    List<Site_Profit> listSite = new List<Site_Profit>();
                foreach(GridViewRow row in gv_TempBrand.Rows)
                {
                    Label lbBrand_Short = (Label)row.FindControl("lbBrand_Short");
                    Site_Profit site = new Site_Profit();
                    try
                    {
                        site.Brand = lbBrand_Short.Text.ToUpper();
                        site.Site_Stroage = txtSiteStorageADD.Text.ToUpper();
                        site.Sale_Channel = ddlSaleChannelADD.SelectedValue;
                        site.Channel = ddlChannelADD.SelectedValue;
                        site.COMCODE = txtComcodeADD.Text;
                        site.Profit = txtProfitADD.Text;
                        site.Costcenter = txtCostcenterADD.Text;
                        site.Date_Create = DateTime.Now;
                        
                        var have = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == txtSiteStorageADD.Text && w.Brand == ddlBrandADD.SelectedValue && w.Sale_Channel == ddlSaleChannelADD.SelectedValue).ToList();
                        if (have.Count() == 0)
                        {
                            listSite.Add(site);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ได้มีการบันทึกไว้แล้วครับ')", true);
                            return;
                        }


                        carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
                        {
                            dateSend = DateTime.Now,
                            path = "Carrier/Edit_Add_Profit",
                            request = Newtonsoft.Json.JsonConvert.SerializeObject(site),
                            status = "1",
                            respon = "Success"
                        });
                        carrier_Entities.SaveChanges();


                    }
                    catch (Exception ex)
                    {
                        carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
                        {
                            dateSend = DateTime.Now,
                            path = "Carrier/Edit_Add_Profit",
                            request = Newtonsoft.Json.JsonConvert.SerializeObject(new Site_Center { Brand_Center_Short = site.Brand, Brand_Center_Name_Full = site.Brand }),
                            respon = ex.Message,
                            status = "2"
                        });
                        carrier_Entities.SaveChanges();

                        service_Flashs.SendMail("apichat.f@sfg-th.com", new string[] { }, "Site_Profit_Add_New ERROR",
                            "<HTML>" +
                        "<body><p>NEW</p>" +
                        "<p>SiteStorage : " + site.Site_Stroage + "</p>" +
                        "<p>Brand : " + ddlBrandADD.SelectedItem.Text + "(" + site.Brand + ")" + "</p>" +
                        "<p>Channel : " + site.Channel + "</p>" +
                        "<p>Sale_Channel : " + site.Sale_Channel + "</p>" +
                        "<p>COMCODE : " + site.COMCODE + "</p>" +
                        "<p>Profit : " + site.Profit + "</p>" +
                        "<p>Costcenter : " + site.Costcenter + "</p>" +
                        "<p>ERROR : " + ex.Message + "</p><p>UserID : " + Session["_UserID"].ToString() +
                        "</p></body>" +
                        "</HTML>");
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Process : ERROR! ได้ทำการส่งไปทาง Dev เรียบร้อยโปรดรอการปรับปรุง หรือติดต่อ [MIS]Ball')", true);
                        return;
                    }
                }
                try
                {
                    carrier_Entities.Site_Profit.AddRange(listSite);
                    carrier_Entities.SaveChanges();
                }
                catch (Exception ex)
                {
                    carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
                    {
                        dateSend = DateTime.Now,
                        path = "Carrier/Edit_Add_Profit",
                        request = Newtonsoft.Json.JsonConvert.SerializeObject(listSite),
                        respon = ex.Message,
                        status = "2"
                    });
                    carrier_Entities.SaveChanges();

                    service_Flashs.SendMail("apichat.f@sfg-th.com", new string[] { }, "Site_Profit_Add_New ERROR",
                        "<HTML>" +
                    "<body><p>ADD to base</p>" +
                    "<p>"+ Newtonsoft.Json.JsonConvert.SerializeObject(listSite) + "</p>"+
                    "<p>ERROR : " + ex.Message + "</p><p>UserID : " + Session["_UserID"].ToString() +
                    "</p></body>" +
                    "</HTML>");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Process : ERROR! ที่การบันทึกข้อมูล ระบบได้ทำการส่งไปทาง Dev เรียบร้อยโปรดรอการปรับปรุง หรือติดต่อมาที่ [MIS]Ball')", true);
                    return;
                }
                btnClose_Click(this, EventArgs.Empty);
                loadProfit("");
                ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript",
                        "setTimeout(function(){alert('บันทึกสำเร็จ') ;window.location.href ='Edit_Add_Profit.aspx'}, 3000);", true);

            }
            else
            {
                Site_Profit site = new Site_Profit();
                try
                {
                    site.Brand = ddlBrandADD.SelectedValue;
                    site.Site_Stroage = txtSiteStorageADD.Text.ToUpper();
                    site.Sale_Channel = ddlSaleChannelADD.SelectedValue;
                    site.Channel = ddlChannelADD.SelectedValue;
                    site.COMCODE = txtComcodeADD.Text;
                    site.Profit = txtProfitADD.Text;
                    site.Costcenter = txtCostcenterADD.Text;
                    site.Date_Create = DateTime.Now;
                    //var BG = budget_Entities.Departments.Where(w => w.ShortBrand == site.Brand).FirstOrDefault();
                    //var BG = (from ha in insideSFG_WF_Entities.BG_HApprove
                    //          join haP in insideSFG_WF_Entities.BG_HApprove_Profitcenter on ha.departmentID equals haP.DepartmentID
                    //          where haP.Depart_Short == site.Brand && ha.Sta == "B" && ha.departmentID != "1508"
                    //          select ha).FirstOrDefault();
                    //if (BG != null)
                    //{
                    //    var cariCenter = carrier_Entities.Site_Center.Where(w => w.Brand_Center_Name_Full == BG.Department_Name || w.Brand_Center_Name_Full == site.Brand).FirstOrDefault();
                    //    if (cariCenter == null)
                    //    {
                    //        carrier_Entities.Site_Center.Add(new Site_Center { Brand_Center_Short = site.Brand, Brand_Center_Name_Full = site.Brand });
                    //    }
                    //}
                    var have = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == txtSiteStorageADD.Text && w.Brand == ddlBrandADD.SelectedValue && w.Sale_Channel == ddlSaleChannelADD.SelectedValue).ToList();
                    if (have.Count() == 0)
                    {
                        carrier_Entities.Site_Profit.Add(site);
                        carrier_Entities.SaveChanges();
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ได้มีการบันทึกไว้แล้วครับ')", true);
                        return;
                    }


                    carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
                    {
                        dateSend = DateTime.Now,
                        path = "Carrier/Edit_Add_Profit",
                        request = Newtonsoft.Json.JsonConvert.SerializeObject(site),
                        status = "1",
                        respon = "Success"
                    });
                    carrier_Entities.SaveChanges();

                    btnClose_Click(this, EventArgs.Empty);
                    loadProfit("");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript",
                            "setTimeout(function(){alert('บันทึกสำเร็จ') ;window.location.href ='Edit_Add_Profit.aspx'}, 3000);", true);
                }
                catch (Exception ex)
                {
                    carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
                    {
                        dateSend = DateTime.Now,
                        path = "Carrier/Edit_Add_Profit",
                        request = Newtonsoft.Json.JsonConvert.SerializeObject(new Site_Center { Brand_Center_Short = site.Brand, Brand_Center_Name_Full = site.Brand }),
                        respon = ex.Message,
                        status = "2"
                    });
                    carrier_Entities.SaveChanges();

                    service_Flashs.SendMail("apichat.f@sfg-th.com", new string[] { }, "Site_Profit_Add_New ERROR",
                        "<HTML>" +
                    "<body><p>NEW</p>" +
                    "<p>SiteStorage : " + site.Site_Stroage + "</p>" +
                    "<p>Brand : " + ddlBrandADD.SelectedItem.Text + "(" + site.Brand + ")" + "</p>" +
                    "<p>Channel : " + site.Channel + "</p>" +
                    "<p>Sale_Channel : " + site.Sale_Channel + "</p>" +
                    "<p>COMCODE : " + site.COMCODE + "</p>" +
                    "<p>Profit : " + site.Profit + "</p>" +
                    "<p>Costcenter : " + site.Costcenter + "</p>" +
                    "<p>ERROR : " + ex.Message + "</p><p>UserID : " + Session["_UserID"].ToString() +
                    "</p></body>" +
                    "</HTML>");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Process : ERROR! ได้ทำการส่งไปทาง Dev เรียบร้อยโปรดรอการปรับปรุง หรือติดต่อ [MIS]Ball')", true);
                }
            }
            
        }

        protected void imgbtnDel_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgbtnDel = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgbtnDel.NamingContainer;
            Label lbSiteStorage = (Label)row.FindControl("lbSiteStorage");
            Label lbBrandTemp = (Label)row.FindControl("lbBrandTemp");
            var del = carrier_Entities.Site_Profit.Where(w => w.Site_Stroage == lbSiteStorage.Text && w.Brand == lbBrandTemp.Text).FirstOrDefault();
            try
            {
                carrier_Entities.Site_Profit.Remove(del);
                carrier_Entities.SaveChanges();
                carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Edit_Add_Profit/Del", request = Newtonsoft.Json.JsonConvert.SerializeObject(del), status = "1" , respon = "Success"});
                carrier_Entities.SaveChanges();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ทำการลบเรียบร้อย')", true);
            }
            catch (Exception ex)
            {
                carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log { dateSend = DateTime.Now, path = "Carrier/Edit_Add_Profit/Del", request = Newtonsoft.Json.JsonConvert.SerializeObject(del), status = "2", respon = "Success" });
                carrier_Entities.SaveChanges();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ลบไม่สำเร็จ')", true);
            }
            
            loadProfit("");
        }

        protected void btnADD_Brand_Click(object sender, EventArgs e)
        {
            List<TempBrand> temp = new List<TempBrand>();
            foreach(GridViewRow row in gv_TempBrand.Rows)
            {
                Label lbBrand_Short = (Label)row.FindControl("lbBrand_Short");
                Label lbBrand = (Label)row.FindControl("lbBrand");
                temp.Add(new TempBrand { Brand_Short = lbBrand_Short.Text, Brand = lbBrand.Text });
            }
            if(temp.Where(w=>w.Brand_Short == ddlBrandADD.SelectedValue).Count() == 0)
            {
                temp.Add(new TempBrand { Brand_Short = ddlBrandADD.SelectedValue, Brand = ddlBrandADD.SelectedItem.Text });
            }
            
            gv_TempBrand.DataSource = temp;
            gv_TempBrand.DataBind();
        }

        protected void imgDelete_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgDelete = (ImageButton)sender;
            GridViewRow row = (GridViewRow)imgDelete.NamingContainer;
            List<TempBrand> temp = new List<TempBrand>();
            foreach (GridViewRow rowo in gv_TempBrand.Rows)
            {
                Label lbBrand_Short = (Label)rowo.FindControl("lbBrand_Short");
                Label lbBrand = (Label)rowo.FindControl("lbBrand");
                if (row != rowo)
                {
                    temp.Add(new TempBrand { Brand_Short = lbBrand_Short.Text, Brand = lbBrand.Text });
                }


            }
            gv_TempBrand.DataSource = temp;
            gv_TempBrand.DataBind();
        }
    }
    public class BrandShowInfo
    {
        public string name { get; set; }
        public string nameShot { get; set; }
    }
    public class TempBrand
    {
        public string Brand_Short { get; set; }
        public string Brand { get; set; }
    }
}