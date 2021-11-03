using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Whale;
using Carrier.Model.Online_Lazada;
using Carrier.Service;
using System.IO;
using System.Net;

namespace Carrier
{
    public partial class Transport_Form : System.Web.UI.Page
    {
        CarrierEntities Carrier_Entities;
        InsideSFG_WFEntities InsideSFG_WF_Entities;
        WhaleEntities Whale_Entities;
        Online_LazadaEntities Online_Lazada_Entities;

        Service_Flash service_Flashs;
        public Transport_Form()
        {
            Carrier_Entities = new CarrierEntities();
            InsideSFG_WF_Entities = new InsideSFG_WFEntities();
            Whale_Entities = new WhaleEntities();
            Online_Lazada_Entities = new Online_LazadaEntities();

            service_Flashs = new Service_Flash();
        }
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
            var Docno = Request.QueryString["Docno"];
            var User = Session["_UserID"].ToString();
            if (!IsPostBack)
            {
                loadPage();
                if (Docno != null)
                {
                    var query = (from order in Carrier_Entities.Orders
                                 join orderItem in Carrier_Entities.Order_Item on order.Docno equals orderItem.Docno
                                 where order.Docno == Docno
                                 select new
                                 {
                                     Docno = order.Docno,
                                     UserId = order.UserID,
                                     Express = order.ExpressCategory,
                                     SrcName = order.srcName,
                                     SrcPhone = order.srcPhone,
                                     SrcProvinces = order.srcProvinceName,
                                     SrcCity = order.srcCityName,
                                     SrcDistrict = order.srcDistrictName,
                                     SrcPostal = order.srcPostalCode,
                                     SrcDetailAddress = order.srcDetailAddress,
                                     DstName = order.dstName,
                                     DstPhone = order.dstPhone,
                                     DstHomePhone = order.dstHomePhone,
                                     DstProvinces = order.dstProvinceName,
                                     DstCity = order.dstProvinceName,
                                     DstDistrict = order.dstDistrictName,
                                     DstPostal = order.dstPostalCode,
                                     DstDetailAddress = order.dstDetailAddress,
                                     ArticleCategory = order.articleCategory,
                                     Remark = order.remark,
                                     status = order.status,
                                     TrackingNo = orderItem.pno,
                                     SDpart = order.SDpart
                                     
                                 }).ToList().FirstOrDefault();

                    
                    txtTrackingID.Text = query.TrackingNo;
                    lbTrackingID.Visible = true;
                    txtTrackingID.Visible = true;
                    txtsrcName.Text = query.SrcName;
                    txtsrcPhone.Text = query.SrcPhone;
                    var srcpro = Whale_Entities.Provinces.Where(w => w.Province_Name == query.SrcProvinces).FirstOrDefault();
                    ddlsrcProvinceName.SelectedValue = srcpro.Province_ID.ToString();
                    txtsrcPostalCode.Text = query.SrcPostal;
                    txtsrcDetailAddress.Text = query.SrcDetailAddress;

                    var srcprovince = Convert.ToInt32(ddlsrcProvinceName.SelectedValue);
                    ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == srcprovince).ToList();
                    ddlsrcCityName.DataBind();
                    ddlsrcCityName.SelectedValue = query.SrcCity;
                    //ddlsrcCityName.Enabled = true;

                    var srccity = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                    ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == srccity).ToList();
                    ddlsrcDistrictName.DataBind();
                    ddlsrcDistrictName.SelectedItem.Text = query.SrcDistrict;
                    //ddlsrcDistrictName.Enabled = true;

                    txtdstName.Text = query.DstName;
                    txtdstPhone.Text = query.DstPhone;
                    txtdstHomePhone.Text = query.DstHomePhone;
                    var dstpro = Whale_Entities.Provinces.Where(w => w.Province_Name == query.DstProvinces).FirstOrDefault();
                    ddldstProvinceName.SelectedValue = dstpro.Province_ID.ToString();

                    var dstprovince = Convert.ToInt32(ddldstProvinceName.SelectedValue);
                    ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == dstprovince).ToList();
                    ddldstCityName.DataBind();
                    ddldstCityName.SelectedValue = query.DstCity;
                    //ddldstCityName.Enabled = true;

                    var dstcity = Convert.ToInt32(ddldstCityName.SelectedValue);
                    ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == dstcity).ToList();
                    ddldstDistrictName.DataBind();
                    ddldstDistrictName.SelectedValue = query.DstDistrict;
                    //ddldstDistrictName.Enabled = true;

                    txtdstPostalCode.Text = query.DstPostal;
                    txtdstDetailAddress.Text = query.DstDetailAddress;
                    btnPrint.Visible = true;

                    //Enable false
                    ddldstProvinceName.Enabled = false;
                    ddlsrcProvinceName.Enabled = false;
                    txtsrcName.Enabled = false;
                    txtsrcPhone.Enabled = false;
                    txtsrcPostalCode.Enabled = false;
                    txtsrcDetailAddress.Enabled = false;
                    txtdstName.Enabled = false;
                    txtdstPhone.Enabled = false;
                    txtdstHomePhone.Enabled =false;
                    txtdstPostalCode.Enabled = false;
                    txtdstDetailAddress.Enabled = false;

                    if(query.status == "AP")
                    {
                        btnPrint.CssClass = "btn btn-danger";
                    }
                    else
                    {
                        btnPrint.CssClass = "btn btn-primary";
                    }
                    ddlarticleCategory.SelectedValue = query.ArticleCategory.ToString();
                    ddlarticleCategory.Enabled = false;
                    btnSave.Visible = false;

                    ddlSDpart.SelectedValue = query.SDpart;
                    ddlSDpart.Enabled = false;
                    txtremark.Enabled = false;
                    lbDocno.Visible = true;
                    txtDocno.Visible = true;
                    txtDocno.Text = query.Docno;
                }
                else
                {
                    var user = Convert.ToInt32(Session["_UserID"].ToString());
                    var Favorites = Carrier_Entities.Orders.Where(w => w.UserID == user).GroupBy(g => new { userId = g.UserID, srcName = g.srcName ,province = g.srcProvinceName , city = g.srcCityName , distric = g.dstDistrictName}).Select(s => new { userid = s.Key.userId ?? 0, srcName = s.Key.srcName, province = s.Key.province , City = s.Key.city , distric = s.Key.distric ,item = s.Count() }).ToList();
                    
                    
                    List<newList> allFavorite = new List<newList>();
                    foreach (var item in Favorites)
                    {
                        if (item.item >= 3)
                        {
                            allFavorite.Add(new newList { val = item.srcName, text = item.srcName });
                        }
                    }
                    
                    if (allFavorite.Count() == 0)
                    {
                        //allFavorite.Add(new newList { text = "ไม่มีข้อมูลผู้ส่งที่ใช้บ่อย" ,val = "No Data"});
                        //ddlFavorites.DataSource = allFavorite;
                        //ddlFavorites.DataBind();
                        //ddlFavorites.Enabled = false;
                        allFavorite.Insert(0, new newList { val = "select", text = "เลือกผู้ส่งที่ใช้บ่อย" });
                        allFavorite.Insert(1, new newList { val = "StarFashion", text = "Star Fashion Group" });
                        ddlFavorites.DataSource = allFavorite;
                        ddlFavorites.DataBind();
                        lbFavorites.Visible = true;
                        ddlFavorites.Visible = true;
                    }
                    else
                    {
                        allFavorite.Insert(0, new newList { val = "select", text = "เลือกผู้ส่งที่ใช้บ่อย" });
                        allFavorite.Insert(1, new newList { val = "StarFashion", text = "Star Fashion Group" });
                        ddlFavorites.DataSource = allFavorite;
                        ddlFavorites.DataBind();
                        lbFavorites.Visible = true;
                        ddlFavorites.Visible = true;
                    }
                    
                }
            }
        }
        public void loadPage()
        {
            List<newList> express = new List<newList>();
            express.Add(new newList { val = "1", text = "ธรรมดา" });
            express.Add(new newList { val = "2", text = "บริการ Speed" });
            ddlExpress.DataSource = express;
            ddlExpress.DataBind();
            ddlExpress.SelectedValue = "1";
            loadArticle();
            loadProvince();
            txtweight.Text = "1";
            txtwidth.Text = "1";
            txtlength.Text = "1";
            txtheight.Text = "1";
            var FC = InsideSFG_WF_Entities.BG_ForeCast.Where(w => w.ActiveStatus == 1).GroupBy(g => g.DepartmentID).Select(s => new Forecasts { DepartmentID = s.Key });
            var depart = (from BG_HA in InsideSFG_WF_Entities.BG_HApprove
                          where FC.Select(s=>s.DepartmentID).Contains(BG_HA.departmentID) &&(BG_HA.Sta == "B" || BG_HA.Sta == "S" || BG_HA.Sta == "N")
                          select new { departmentID = BG_HA.departmentID, department_ = BG_HA.department_ }
                          ).OrderBy(r => r.department_).ToList();
            depart.Insert(0, new { departmentID = "Select", department_ = "กรุณาเลือกแผนกที่ต้องการเบิก" });
            ddlSDpart.DataSource = depart;
            ddlSDpart.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            
            var docno = Carrier_Entities.Orders.ToList().LastOrDefault();
            var newId = "";
            if (docno == null)
            {
                var id = "FL0000000001";
                newId = id;
            }
            else
            {
                newId = docno.Docno;
                var lastId = Convert.ToInt32(newId.Substring(2, 10)) + 1;
                newId = newId.Substring(0, 2) + newId.Substring(2, 10 - lastId.ToString().Length) + lastId.ToString();
            }
            var item = new Order
            {
                Docno = newId,
                Date_send = DateTime.Now,
                UserID = Convert.ToInt32(Session["_UserID"].ToString()),
                articleCategory = Convert.ToInt32(ddlarticleCategory.SelectedValue),
                ExpressCategory = Convert.ToInt32(ddlExpress.SelectedValue),
                srcName = txtsrcName.Text,
                srcPhone = txtsrcPhone.Text,
                srcProvinceName = ddlsrcProvinceName.SelectedItem.Text,
                srcCityName = "",
                srcDistrictName = "",
                srcPostalCode = txtsrcPostalCode.Text,
                srcDetailAddress = txtsrcDetailAddress.Text,
                Ref_Order = "Test",
                dstName = txtdstName.Text,
                dstPhone = txtdstPhone.Text,
                dstHomePhone = txtdstHomePhone.Text,
                dstProvinceName = ddldstProvinceName.SelectedItem.Text,
                dstCityName = "",
                dstDistrictName = "",
                dstPostalCode = txtdstPostalCode.Text,
                dstDetailAddress = txtdstDetailAddress.Text,
                insured = 0,
                Transport_Type = 1,
                weight = Convert.ToInt32(txtweight.Text),
                width = Convert.ToInt32(txtwidth.Text),
                length = Convert.ToInt32(txtlength.Text),
                height = Convert.ToInt32(txtheight.Text),
                remark = txtremark.Text,
                SDpart = ddlSDpart.SelectedValue
            };
            var vali =  service_Flashs.Validate_Transport(item);
            if(vali == "PASS")
            {
                item.srcCityName = ddlsrcCityName.SelectedItem.Text;
                item.srcDistrictName = ddlsrcDistrictName.SelectedItem.Text;
                item.dstCityName = ddldstCityName.SelectedItem.Text;
                item.dstDistrictName = ddldstDistrictName.SelectedItem.Text;
                Carrier_Entities.Orders.Add(item);
                Carrier_Entities.SaveChanges();
                var res = service_Flashs.CreateOrderFLASH(newId);

                if (res.success == true)
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('succes : " + res.success + " Tracking NO : " + res.trackingno + "')", true);
                    var returnText = service_Flashs.Get_Docment(newId);
                    btnSave.Visible = false;
                    //Response.Redirect("Transport_Form?Docno=" + newId);
                    ClientScript.RegisterStartupScript(this.GetType(), "Success", "<script type='text/javascript'>alert('succes : " + res.success + " Tracking NO : " + res.trackingno + "');window.location='Transport_Form?Docno=" + newId+"';</script>'");
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('succes : " + res.success + " Message : " + res.trackingno + "')", true);
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('"+vali+"')", true);
            }
            
        }

        public void loadArticle()
        {
            var Article = Carrier_Entities.Article_Category.ToList();
            Article.Insert(0, new Article_Category { ArticleCode = 0, ArticleName = "กรุณาเลือกประเภทพัสดุ" });
            ddlarticleCategory.DataSource = Article;
            ddlarticleCategory.DataBind();
        }
        public void loadProvince()
        {
            //src
            var Provincelist = Whale_Entities.Provinces.ToList();
            Provincelist.Insert(0, new Province { Province_ID = 0, Province_Name = "เลือกจังหวัด" });
            ddlsrcProvinceName.DataSource = Provincelist;
            ddlsrcProvinceName.DataBind();
            ddlsrcCityName.Enabled = false;
            ddlsrcDistrictName.Enabled = false;
            //dst

            ddldstProvinceName.DataSource = Provincelist;
            ddldstProvinceName.DataBind();
            ddldstCityName.Enabled = false;
            ddldstDistrictName.Enabled = false;
        }

        protected void ddlsrcProvinceName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlsrcCityName.Enabled = true;
            var province = Convert.ToInt32(ddlsrcProvinceName.SelectedValue);
            if(province == 0)
            {
                List<City> defaultCity = new List<City>();
                defaultCity.Add(new City { City_ID = 0, City_Name = ""});
                ddlsrcCityName.DataSource = defaultCity;
                ddlsrcCityName.DataBind();
                ddlsrcCityName.Enabled = false;
                List<District> defaultDistrict = new List<District>();
                defaultDistrict.Add(new District { Distinct_ID = 0, Distinct_Name = "" });
                ddlsrcDistrictName.DataSource = defaultDistrict;
                ddlsrcDistrictName.DataBind();
                ddlsrcDistrictName.Enabled = false;
                txtsrcPostalCode.Text = "";
            }
            else
            {
                ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == province).ToList();
                ddlsrcCityName.DataBind();
                ddlsrcDistrictName.Enabled = true;

                var city = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).ToList();
                ddlsrcDistrictName.DataBind();

                var district = Convert.ToInt32(ddlsrcDistrictName.SelectedValue);
                txtsrcPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
            }
            
        }

        protected void ddlsrcCityName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlsrcDistrictName.Enabled = true;
            var city = Convert.ToInt32(ddlsrcCityName.SelectedValue);
            ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).ToList();
            ddlsrcDistrictName.DataBind();

            var district = Convert.ToInt32(ddlsrcDistrictName.SelectedValue);
            txtsrcPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
        }

        protected void ddlsrcDistrictName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var district = Convert.ToInt32(ddlsrcDistrictName.SelectedValue);
            txtsrcPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
        }

        protected void ddldstProvinceName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddldstCityName.Enabled = true;
            var province = Convert.ToInt32(ddldstProvinceName.SelectedValue);
            if(province == 0)
            {
                List<City> defaultCity = new List<City>();
                defaultCity.Add(new City { City_ID = 0, City_Name = "" });
                ddldstCityName.DataSource = defaultCity;
                ddldstCityName.DataBind();
                ddldstCityName.Enabled = false;
                List<District> defaultDistrict = new List<District>();
                defaultDistrict.Add(new District { Distinct_ID = 0, Distinct_Name = "" });
                ddldstDistrictName.DataSource = defaultDistrict;
                ddldstDistrictName.DataBind();
                ddldstDistrictName.Enabled = false;
                txtdstPostalCode.Text = "";

            }
            else
            {
                ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == province).ToList();
                ddldstCityName.DataBind();
                ddldstDistrictName.Enabled = true;

                var city = Convert.ToInt32(ddldstCityName.SelectedValue);
                ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).ToList();
                ddldstDistrictName.DataBind();

                var district = Convert.ToInt32(ddldstDistrictName.SelectedValue);
                txtdstPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
            }
            
        }

        protected void ddldstCityName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddldstDistrictName.Enabled = true;
            var city = Convert.ToInt32(ddldstCityName.SelectedValue);
            ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).ToList();
            ddldstDistrictName.DataBind();
            txtdstPostalCode.Text = Whale_Entities.Districts.Where(w => w.City_ID == city).FirstOrDefault().Postcode.ToString();

            var district = Convert.ToInt32(ddldstDistrictName.SelectedValue);
            txtdstPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
        }

        protected void ddldstDistrictName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var district = Convert.ToInt32(ddldstDistrictName.SelectedValue);
            txtdstPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {

            Response.Redirect("Transport_bill?Docno=" + lbDocno.Text);
        }

        protected void txtsrcPhone_TextChanged(object sender, EventArgs e)
        {
            TextBox s = (TextBox)sender;

            var f = s.Text;
        }

        protected void ddlFavorites_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectFavorite = ddlFavorites.SelectedValue;
            var user = Convert.ToInt32(Session["_UserID"].ToString());
            if (selectFavorite != "select")
            {
                if(selectFavorite == "StarFashion")
                {
                    txtsrcName.Text = "บริษัท สตาร์แฟชั่น(2551) จำกัด";
                    txtsrcPhone.Text = "0873078300";
                    ddlsrcProvinceName.SelectedValue = "1";
                    var province = Convert.ToInt32(ddlsrcProvinceName.SelectedValue);

                    ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == province).ToList();
                    ddlsrcCityName.DataBind();
                    ddlsrcDistrictName.Enabled = true;
                    ddlsrcCityName.SelectedValue = "20";

                    var city = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                    ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).ToList();
                    ddlsrcDistrictName.DataBind();
                    ddlsrcDistrictName.SelectedValue = "119";
                    txtsrcPostalCode.Text = "10120";
                    txtsrcDetailAddress.Text = "477 พระราม 3 ";
                }
                else{
                    var Favorites = Carrier_Entities.Orders.Where(w => w.UserID == user && w.srcName == selectFavorite).GroupBy(g => new { userId = g.UserID, srcName = g.srcName,phone = g.srcPhone , province = g.srcProvinceName, city = g.srcCityName, distric = g.srcDistrictName }).Select(s => new { userid = s.Key.userId ?? 0, srcName = s.Key.srcName, phone = s.Key.phone, province = s.Key.province, City = s.Key.city, distric = s.Key.distric, item = s.Count() }).OrderBy(r=> r.item).ToList().LastOrDefault();

                    var query = Carrier_Entities.Orders.Where(w => w.UserID == Favorites.userid && w.srcName == Favorites.srcName && w.srcPhone == Favorites.phone && w.srcProvinceName == Favorites.province && w.srcCityName == Favorites.City && w.srcDistrictName == Favorites.distric).ToList().LastOrDefault();
                    lbDocno.Text = query.Docno ?? "";
                    txtsrcName.Text = query.srcName ?? "";
                    txtsrcPhone.Text = query.srcPhone ?? "";
                    var pro = Whale_Entities.Provinces.Where(w => w.Province_Name == query.srcProvinceName).ToList().FirstOrDefault().Province_ID;
                    ddlsrcProvinceName.SelectedValue = pro.ToString();
                    ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == pro).ToList();
                    ddlsrcCityName.DataBind();
                    var city = Whale_Entities.Cities.Where(w => w.City_Name == query.srcCityName).ToList().FirstOrDefault().City_ID;
                    ddlsrcCityName.SelectedValue = city.ToString();
                    ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).ToList();
                    ddlsrcDistrictName.DataBind();
                    var district = Whale_Entities.Districts.Where(w => w.Distinct_Name == query.srcDistrictName).ToList().FirstOrDefault().Distinct_ID;
                    ddlsrcDistrictName.SelectedValue = district.ToString();
                    txtsrcPostalCode.Text = query.srcPostalCode ?? "";
                    txtsrcDetailAddress.Text = query.srcDetailAddress ?? "";
                }
                

            }else
            {
                txtsrcName.Text = "";
                txtsrcPhone.Text = "";
                ddlsrcProvinceName.SelectedValue = "0";
                List<City> defaultCity = new List<City>();
                defaultCity.Add(new City { City_ID = 0, City_Name = "" });
                ddlsrcCityName.DataSource = defaultCity;
                ddlsrcCityName.DataBind();
                ddlsrcCityName.Enabled = false;
                List<District> defaultDistrict = new List<District>();
                defaultDistrict.Add(new District { Distinct_ID = 0, Distinct_Name = "" });
                ddlsrcDistrictName.DataSource = defaultDistrict;
                ddlsrcDistrictName.DataBind();
                ddlsrcDistrictName.Enabled = false;
                txtsrcPostalCode.Text = "";
                txtsrcDetailAddress.Text = "";
            }
        }
    }
    public class newList
    {
        public string val { get; set; }
        public string text { get; set; }
    }
    public class listFavorite
    {
        public int userid { get; set; }
        public string srcName { get; set; }
        public int item { get; set; }
    }
    public class Forecasts
    {
        public string DepartmentID { get; set; }
    }
}