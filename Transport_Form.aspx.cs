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
using System.Web.Services;

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
                                     DstCity = order.dstCityName,
                                     DstDistrict = order.dstDistrictName,
                                     DstPostal = order.dstPostalCode,
                                     DstDetailAddress = order.dstDetailAddress,
                                     ArticleCategory = order.articleCategory,
                                     Remark = order.remark,
                                     status = order.status,
                                     TrackingNo = orderItem.pno,
                                     SDpart = order.SDpart,
                                     saleOn = order.saleOn,
                                     SiteStorage = order.siteStorage,
                                     SaleChannel = order.saleChannel,
                                     Transport_Type = order.Transport_Type,
                                     TypeSendKO = orderItem.TypeSendKO,
                                     TypeSend = order.TypeSend
                                 }).ToList().FirstOrDefault();

                    if (query.Transport_Type == 1)
                    {
                        txtTrackingID.Text = query.TrackingNo;
                        lbTrackingID.Visible = true;
                        txtTrackingID.Visible = true;
                        btnPrint.Visible = true;
                    }
                    else
                    {
                        btnPrint.Visible = false;
                    }
                    ddlTypeSend.SelectedValue = query.TypeSend.ToString();
                    ddlTypeSend.Enabled = false;
                    ddlExpress.SelectedValue = query.Transport_Type.ToString();
                    ddlExpress.Enabled = false;

                    txtsrcName.Text = query.SrcName;
                    txtsrcPhone.Text = query.SrcPhone;
                    var srcpro = Whale_Entities.Provinces.Where(w => w.Province_Name == query.SrcProvinces).FirstOrDefault();
                    ddlsrcProvinceName.SelectedValue = srcpro.Province_ID.ToString();
                    txtsrcPostalCode.Text = query.SrcPostal;
                    txtsrcDetailAddress.Text = query.SrcDetailAddress;

                    var srcprovince = Convert.ToInt32(ddlsrcProvinceName.SelectedValue);
                    ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == srcprovince).ToList();
                    ddlsrcCityName.DataBind();
                    var srccityTemp = Whale_Entities.Cities.Where(w => w.City_Name == query.SrcCity).FirstOrDefault().City_ID;
                    ddlsrcCityName.SelectedValue = srccityTemp.ToString();
                    //ddlsrcCityName.Enabled = true;

                    var srccity = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                    ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == srccity).ToList();
                    ddlsrcDistrictName.DataBind();
                    var srcdistrictTemp = Whale_Entities.Districts.Where(w =>w.City_ID == srccity && w.Distinct_Name == query.SrcDistrict).FirstOrDefault().Distinct_ID;
                    ddlsrcDistrictName.SelectedValue = srcdistrictTemp.ToString();
                    //ddlsrcDistrictName.Enabled = true;

                    txtdstName.Text = query.DstName;
                    txtdstPhone.Text = query.DstPhone;
                    txtdstHomePhone.Text = query.DstHomePhone;
                    var dstpro = Whale_Entities.Provinces.Where(w => w.Province_Name == query.DstProvinces).FirstOrDefault();
                    ddldstProvinceName.SelectedValue = dstpro.Province_ID.ToString();

                    var dstprovince = Convert.ToInt32(ddldstProvinceName.SelectedValue);
                    ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == dstprovince).ToList();
                    ddldstCityName.DataBind();
                    var dstcityTemp = Whale_Entities.Cities.Where(w => w.City_Name == query.DstCity).FirstOrDefault().City_ID;
                    ddldstCityName.SelectedValue = dstcityTemp.ToString();
                    //ddldstCityName.Enabled = true;

                    var dstcity = Convert.ToInt32(ddldstCityName.SelectedValue);
                    ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == dstcity).ToList();
                    ddldstDistrictName.DataBind();
                    var dstdistrictTemp = Whale_Entities.Districts.Where(w =>w.City_ID == dstcity && w.Distinct_Name == query.DstDistrict).FirstOrDefault().Distinct_ID;
                    ddldstDistrictName.SelectedValue = dstdistrictTemp.ToString();
                    //ddldstDistrictName.Enabled = true;

                    txtdstPostalCode.Text = query.DstPostal;
                    txtdstDetailAddress.Text = query.DstDetailAddress;
                    

                    //Enable false
                    ddldstProvinceName.Enabled = false;
                    ddlsrcProvinceName.Enabled = false;
                    txtsrcName.Enabled = false;
                    txtsrcPhone.Enabled = false;
                    txtsrcPostalCode.Enabled = false;
                    txtsrcDetailAddress.Enabled = false;
                    txtdstName.Enabled = false;
                    txtdstPhone.Enabled = false;
                    txtdstHomePhone.Enabled = false;
                    txtdstPostalCode.Enabled = false;
                    txtdstDetailAddress.Enabled = false;

                    if (query.status == "AP")
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
                    txtremark.Text = query.Remark;
                    lbDocno.Visible = true;
                    txtDocno.Visible = true;
                    txtDocno.Text = query.Docno;

                    radioWorkOn.Enabled = false;
                    radioWorkOff.Enabled = false;
                    switch (query.saleOn)
                    {
                        case "ONLINE":
                            radioWorkOn.Checked = true;
                            break;
                        case "OFFLINE":
                            radioWorkOff.Checked = true;
                            break;
                    }
                    var listBox = Carrier_Entities.Order_Box.Where(w => w.Docno == Docno).ToList();

                    div_Box.Visible = true;

                    var box = Whale_Entities.Boxes.Where(w => w.Flag_Active == true).ToList();
                    box.Insert(0, new Box { Box_ID = 0, Box_Name = "เลือกขนาดกล่อง" });
                    ddlBox.DataSource = box;
                    ddlBox.DataBind();
                    ddlBox.Enabled = false;

                    txtQty.Text = "0";
                    txtQty.Enabled = false;

                    btnAdd.Enabled = false;

                    gv_Box.DataSource = listBox;
                    gv_Box.DataBind();
                    gv_Box.Columns[2].Visible = false;
                    foreach (GridViewRow row in gv_Box.Rows)
                    {
                        TextBox txtQty = (TextBox)row.FindControl("txtQty");
                        Label lbQty = (Label)row.FindControl("lbQty");
                        txtQty.Visible = false;
                        lbQty.Visible = true;
                    }
                    if (query.SiteStorage == null)
                    {
                        divSite.Visible = false;
                    }
                    else
                    {
                        divSite.Visible = true;
                        txtSiteStorage.Enabled = false;
                        txtSiteStorage.Text = query.SiteStorage;
                    }
                    List<newList> allFavorite = new List<newList>();
                    allFavorite.Add(new newList { val = "select", text = "เลือกผู้ส่งที่ใช้บ่อย" });
                    allFavorite.Add(new newList { val = "SFG", text = "Star Fashion Group" });
                    allFavorite.Add(new newList { val = "SDC1", text = "SDC1" });
                    ddlFavorites.DataSource = allFavorite;
                    ddlFavorites.DataBind();
                    ddlFavorites.SelectedValue = query.TypeSendKO;
                    allFavorite.Add(new newList { val = "Depart", text = "หน้าร้าน" });
                    allFavorite.Add(new newList { val = "CENTER", text = "ลูกค้า" });
                    allFavorite.Add(new newList { val = "Event", text = "Event" });
                    allFavorite.Add(new newList { val = "CENTER_Other", text = "อื่นๆ" });
                    allFavorite.ForEach(f => { if (f.val == "select") { f.text = "เลือกปลายทาง"; } });
                    ddlReceiveLocation.DataSource = allFavorite;
                    ddlReceiveLocation.DataBind();
                    ddlReceiveLocation.SelectedValue = query.SaleChannel;
                    ddlReceiveLocation.Enabled = false;
                    lbGuidSiteStorage.Visible = false;
                    
                }
                else
                {
                    //string username = HttpContext.Current.Request.Cookies["sfgweb"]["uname"].Trim();
                    string username = "9012400";
                    var objuser = (from tEmployee in InsideSFG_WF_Entities.Employees
                                   where (tEmployee.username_ == username || tEmployee.uCode == username)
                                   && tEmployee.StatWork == "Y"
                                   select tEmployee
                                      ).FirstOrDefault();
                    if (objuser != null)
                    {
                        txtsrcName.Text = objuser.name + " " + objuser.surname ;
                        if(objuser.Mobile != null && objuser.Mobile.Contains(" "))
                        {
                            txtsrcPhone.Text = objuser.Mobile.Replace(" ", "");
                        }
                        else if(objuser.Mobile != null)
                        {
                            txtsrcPhone.Text = objuser.Mobile.Replace("-", "");
                        }
                        
                    }
                    radioWorkOn.Checked = true;
                    List<newList> allFavorite = new List<newList>();

                    allFavorite.Insert(0, new newList { val = "select", text = "เลือกผู้ส่งที่ใช้บ่อย" });
                    allFavorite.Insert(1, new newList { val = "SFG", text = "Star Fashion Group" });
                    allFavorite.Insert(2, new newList { val = "SDC1", text = "SDC1" });
                    ddlFavorites.DataSource = allFavorite;
                    ddlFavorites.DataBind();
                    allFavorite.Insert(3, new newList { val = "Depart", text = "หน้าร้าน" });
                    allFavorite.Insert(4, new newList { val = "CENTER", text = "ลูกค้า" });
                    allFavorite.Insert(5, new newList { val = "Event", text = "Event" });
                    allFavorite.Insert(6, new newList { val = "CENTER_Other", text = "อื่นๆ" });
                    allFavorite.ForEach(f => { if (f.val == "select") { f.text = "เลือกปลายทาง"; } });
                    ddlReceiveLocation.DataSource = allFavorite;
                    ddlReceiveLocation.DataBind();
                    lbFavorites.Visible = true;
                    ddlFavorites.Visible = true;
                    if(ddlExpress.SelectedValue == "1")
                    {
                        ddlTypeSend.SelectedValue = "1";
                        ddlTypeSend.Enabled = false;
                    }
                }
            }
        }

        public void loadPage()
        {
            List<newList> express = new List<newList>();
            express.Add(new newList { val = "1", text = "Flash Express" });
            express.Add(new newList { val = "2", text = "Lalamove" });
            ddlExpress.DataSource = express;
            ddlExpress.DataBind();
            ddlExpress.SelectedValue = "FlashEX";
            loadArticle();
            loadProvince();
            txtweight.Text = "1";
            txtwidth.Text = "1";
            txtlength.Text = "1";
            txtheight.Text = "1";
            var FC = InsideSFG_WF_Entities.BG_ForeCast.Where(w => w.ActiveStatus == 1).GroupBy(g => g.DepartmentID).Select(s => new Forecasts { DepartmentID = s.Key });
            var depart = (from BG_HA in InsideSFG_WF_Entities.BG_HApprove
                          join BG_HAPF in InsideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
                          where FC.Select(s => s.DepartmentID).Contains(BG_HA.departmentID) && (BG_HA.Sta == "B" || BG_HA.Sta == "S" || BG_HA.Sta == "N")
                          select new { departmentID = BG_HA.departmentID, department_ = BG_HA.department_ }
                          ).OrderBy(r => r.department_).ToList();
            depart.Insert(0, new { departmentID = "Select", department_ = "กรุณาเลือกแผนกที่ต้องการเบิก" });
            ddlSDpart.DataSource = depart;
            ddlSDpart.DataBind();
            
            var box = Whale_Entities.Boxes.Where(w=>w.Flag_Active == true &&( w.Box_ID != 1 && w.Box_ID != 6 && w.Box_ID != 8 && w.Box_ID != 12 && w.Box_ID != 13 && w.Box_ID != 14 && w.Box_ID != 16 && w.Box_ID != 17)).ToList();
            box.Insert(0,new Box { Box_ID = 0, Box_Name = "เลือกขนาดกล่อง" });
            ddlBox.DataSource = box;
            ddlBox.DataBind();
            txtQty.Text = "0";

        }

        public void loadArticle()
        {
            var Article = Carrier_Entities.Article_Category.ToList();
            Article.Insert(0, new Article_Category { ArticleCode = 1111, ArticleName = "กรุณาเลือกประเภทพัสดุ" });
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

        #region DROPROWNLIST select changed
        protected void ddlsrcProvinceName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlsrcCityName.Enabled = true;
            var province = Convert.ToInt32(ddlsrcProvinceName.SelectedValue);
            if (province == 0)
            {
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
            if (province == 0)
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

        protected void ddlFavorites_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectFavorite = ddlFavorites.SelectedValue;
            var user = Convert.ToInt32(Session["_UserID"].ToString());
            
                switch (selectFavorite)
                {
                    case "SFG":
                        //txtsrcName.Text = "บริษัท สตาร์แฟชั่น(2551) จำกัด";
                        //txtsrcPhone.Text = "0873078300";
                        ddlsrcProvinceName.SelectedValue = "1";
                        var provinceSFG = Convert.ToInt32(ddlsrcProvinceName.SelectedValue);

                        ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provinceSFG).ToList();
                        ddlsrcCityName.DataBind();
                        ddlsrcCityName.SelectedValue = "20";
                        ddlsrcCityName.Enabled = true;

                        ddlsrcDistrictName.Enabled = true;
                        var citySFG = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                        ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == citySFG).ToList();
                        ddlsrcDistrictName.DataBind();
                        ddlsrcDistrictName.SelectedValue = "119";
                        txtsrcPostalCode.Text = "10120";
                        txtsrcDetailAddress.Text = "บริษัท สตาร์แฟชั่น(2551) จำกัด "+"477 พระราม 3 ";

                        break;
                    case "SDC1":
                    //ผู้ส่ง
                    //var  user = HttpContext.Current.Session[]
                    //var fromUser = InsideSFG_WF_Entities.Employees.Where(w=>w.userID == )
                    txtsrcName.Text = "ณัฐชยา พงศ์ทองกุล";
                    txtsrcPhone.Text = "0988325926";
                        ddlsrcProvinceName.SelectedValue = "5";
                        var provinceSDC1 = Convert.ToInt32(ddlsrcProvinceName.SelectedValue);

                        ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provinceSDC1).ToList();
                        ddlsrcCityName.DataBind();
                        ddlsrcCityName.SelectedValue = "755";
                        ddlsrcCityName.Enabled = true;

                        ddlsrcDistrictName.Enabled = true;
                        var citySDC1 = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                        ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == citySDC1).ToList();
                        ddlsrcDistrictName.DataBind();
                        ddlsrcDistrictName.SelectedValue = "483";
                        txtsrcPostalCode.Text = "13170";
                        txtsrcDetailAddress.Text = "บริษัท เอส.ดี.ซี วัน จำกัด "+"59/1 ม.1 ";
                        
                        break;
                    
                }
            
        }

        protected void ddlReceiveLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectReceive = ddlReceiveLocation.SelectedValue;
            
                //divSite.Visible = false;
                txtdstName.Enabled = true;
                if (selectReceive != "select")
                {
                    switch (selectReceive)
                    {
                        case "SDC1":
                            

                            //ผู้รับ
                            //txtdstName.Text = "บริษัท เอส.ดี.ซี วัน จำกัด";
                            //txtdstPhone.Text = "0944764565";
                            //txtdstHomePhone.Text = "-";
                            ddldstProvinceName.SelectedValue = "5";
                            var provincedstSCD1 = Convert.ToInt32(ddldstProvinceName.SelectedValue);
                            ddldstCityName.Enabled = true;
                            ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provincedstSCD1).ToList();
                            ddldstCityName.DataBind();
                            ddldstCityName.SelectedValue = "755";

                            ddldstDistrictName.Enabled = true;
                            var citydstSCD1 = Convert.ToInt32(ddldstCityName.SelectedValue);
                            ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == citydstSCD1).ToList();
                            ddldstDistrictName.DataBind();
                            ddldstDistrictName.SelectedValue = "483";
                            txtdstPostalCode.Text = "13170";
                            txtdstDetailAddress.Text = "บริษัท เอส.ดี.ซี วัน จำกัด "+"59/1 ม.1 ";
                            break;
                        case "SFG":
                            
                            //ผู้รับ
                            //txtdstName.Text = "บริษัท สตาร์แฟชั่น(2551) จำกัด";
                            //txtdstPhone.Text = "0873078300";
                            //txtdstHomePhone.Text = "-";
                            ddldstProvinceName.SelectedValue = "1";
                            var provincedstSFG = Convert.ToInt32(ddldstProvinceName.SelectedValue);

                            ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provincedstSFG).ToList();
                            ddldstCityName.DataBind();
                            ddldstCityName.SelectedValue = "20";
                            ddldstCityName.Enabled = true;

                            ddldstDistrictName.Enabled = true;
                            var citydstSFG = Convert.ToInt32(ddldstCityName.SelectedValue);
                            ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == citydstSFG).ToList();
                            ddldstDistrictName.DataBind();
                            ddldstDistrictName.SelectedValue = "119";
                            txtdstPostalCode.Text = "10120";
                            txtdstDetailAddress.Text = "บริษัท สตาร์แฟชั่น(2551) จำกัด "+ "477 พระราม 3 ";
                            break;
                    }
                }
            
        }

        #endregion


        protected void txtSiteStorage_TextChanged(object sender, EventArgs e)
        {
            
            

        }

        public void getAddressOnSite(string siteId)
        {
            var brand = ddlSDpart.SelectedValue;
            var online = Online_Lazada_Entities.PROVINCEs.Select(s => s.PROVINCE_ID).ToList();
            var address = (from tax in InsideSFG_WF_Entities.Customer_Tax
                           where online.Contains(tax.Province1) && tax.CustomerCode == siteId
                           select new
                           {
                               NameTax = tax.NameTax,
                               dstDetail = tax.Address1 + " " + tax.lane1 + " " + tax.Road1,
                               dstProvince = tax.Province1,
                               dstCity = tax.Area1,
                               dstDistrict = tax.Zone1,
                               dstPostal = tax.Postal1,
                               dstPhone = tax.phone1
                           }).ToList().FirstOrDefault();
            if (address != null)
            {
                
                //if(address.dstPhone.Length >= 8)
                //{
                //    if (address.dstPhone.Substring(0, 2) == "08" || address.dstPhone.Substring(0, 2) == "09")
                //    {
                //        txtdstPhone.Text = address.dstPhone;
                //        txtdstHomePhone.Text = "-";
                //    }
                //    else if ( address.dstPhone.Substring(0) == "8" || address.dstPhone.Substring(0) == "9")
                //    {
                //        txtdstPhone.Text = "0" + address.dstPhone;
                //        txtdstHomePhone.Text = "-";
                //    }
                //    else if (address.dstPhone.Substring(0, 2) == "02")
                //    {
                //        txtdstPhone.Text = "-";
                //        txtdstHomePhone.Text = address.dstPhone;
                //    }
                //    else if(address.dstPhone.Substring(0) == "2" )
                //    {
                //        txtdstPhone.Text = "-"; 
                //        txtdstHomePhone.Text = "0" + address.dstPhone;
                //    }
                //    else
                //    {
                //        txtdstPhone.Text = "-";
                //        txtdstHomePhone.Text = "-";
                //    }
                    
                //}
                //else
                //{
                //    txtdstPhone.Text = "-";
                //}
                
                ddldstProvinceName.SelectedValue = address.dstProvince;
                var provinceSDC1 = Convert.ToInt32(ddldstProvinceName.SelectedValue);


                var citylike = Whale_Entities.Cities.Where(w => w.Province_ID == provinceSDC1 && w.City_Name.Contains(address.dstCity)).ToList().FirstOrDefault();
                if (citylike != null)
                {
                    ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provinceSDC1).ToList();
                    ddldstCityName.DataBind();
                    ddldstCityName.SelectedValue = citylike.City_ID.ToString();
                    ddldstCityName.Enabled = true;

                    var city = Convert.ToInt32(ddldstCityName.SelectedValue);
                    var districtlike = Whale_Entities.Districts.Where(w => w.City_ID == city && w.Distinct_Name.Contains(address.dstDistrict)).ToList().FirstOrDefault();
                    if (districtlike != null)
                    {
                        ddldstDistrictName.Enabled = true;
                        ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).ToList();
                        ddldstDistrictName.DataBind();
                        ddldstDistrictName.SelectedValue = districtlike.Distinct_ID.ToString();
                    }
                    else
                    {
                        var district = Whale_Entities.Districts.Where(w => w.City_ID == city).ToList();
                        district.Insert(0, new District { Distinct_ID = 0, Distinct_Name = "เลือกตำบล" });
                        ddldstDistrictName.Enabled = true;
                        ddldstDistrictName.DataSource = district;
                        ddldstDistrictName.DataBind();
                    }

                }
                else
                {
                    var city = Whale_Entities.Cities.Where(w => w.Province_ID == provinceSDC1).ToList();
                    city.Insert(0, new City { City_ID = 0, City_Name = "เลือกอำเภอ" });
                    ddldstCityName.Enabled = true;
                    ddldstCityName.DataSource = city;
                    ddldstCityName.DataBind();
                    ddldstDistrictName.Enabled = false;
                    ddldstDistrictName = new DropDownList();
                    ddldstDistrictName.DataBind();
                }

                txtdstPostalCode.Text = address.dstPostal;
                txtdstDetailAddress.Text = address.NameTax +" "+ address.dstDetail;
            }
            //else
            //{
            //    txtdstName.Text = "";
            //    txtdstName.Enabled = true;
            //    txtdstPhone.Text = "";
            //    txtdstHomePhone.Text = "";
            //    txtdstPostalCode.Text = "";
            //    ddldstProvinceName.SelectedValue = "0";
            //    ddldstCityName.DataSource = new List<City>();
            //    ddldstCityName.DataBind();
            //    //ddldstCityName.ClearSelection();
            //    ddldstCityName.Enabled = false;
            //    ddldstDistrictName.DataSource = new List<District>();
            //    //ddldstDistrictName.ClearSelection();
            //    ddldstDistrictName.DataBind();
            //    ddldstDistrictName.Enabled = false;
            //    txtdstDetailAddress.Text = "";
            //}


        }

        #region Button
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if(ddlBox.SelectedValue != "0" && (txtQty.Text != "" && txtQty.Text !="0"))
            {
                List<BoxItem> listBox = new List<BoxItem>();
                foreach(GridViewRow row in gv_Box.Rows)
                {
                    listBox.Add(new BoxItem { Box_ID = Convert.ToInt32(((Label)row.FindControl("lbBox_ID")).Text), Box_Name = ((Label)row.FindControl("lbBox_Name")).Text, Qty = Convert.ToInt32(((TextBox)row.FindControl("txtQty")).Text) });
                }
                    listBox.Add(new BoxItem { Box_ID = Convert.ToInt32(ddlBox.SelectedValue), Box_Name = ddlBox.SelectedItem.Text, Qty = Convert.ToInt32(txtQty.Text) });
                
                gv_Box.DataSource = listBox;
                gv_Box.DataBind();
            }
        }

        protected void btnDeleteBoxItem_Click(object sender, EventArgs e)
        {
            Button btnDeleteBoxItem = (Button)sender;
            GridViewRow row = (GridViewRow)btnDeleteBoxItem.NamingContainer;
            GridView gv = (GridView)row.NamingContainer;
            List<BoxItem> item = new List<BoxItem>();
            foreach(GridViewRow rows in gv.Rows)
            {
                if(rows != row)
                {

                    item.Add(new BoxItem {Box_ID = Convert.ToInt32(((Label)rows.FindControl("lbBox_ID")).Text), Box_Name = ((Label)rows.FindControl("lbBox_Name")).Text, Qty = Convert.ToInt32(((TextBox)rows.FindControl("txtQty")).Text) });
                }
            }
            gv_Box.DataSource = item;
            gv_Box.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            //เปิดหน้าแสดง PDF
            Response.Redirect("Transport_bill?Docno=" + txtDocno.Text);
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

                var siteCode = "";
                if (txtSiteStorage.Text.Length >= 4)
                {
                    siteCode = txtSiteStorage.Text.Substring(0, 4).ToUpper();
                }
                var online = Online_Lazada_Entities.PROVINCEs.Select(s => s.PROVINCE_ID).ToList();
                var siteAddress = (from tax in InsideSFG_WF_Entities.Customer_Tax
                                   where online.Contains(tax.Province1) && tax.CustomerCode == siteCode
                                   select new
                                   {
                                       NameTax = tax.NameTax,
                                       srcDetail = tax.Address1 + " " + tax.lane1 + " " + tax.Road1,
                                       srcProvince = tax.Province1,
                                       srcCity = tax.Area1,
                                       srcDistrict = tax.Zone1,
                                       srcPostal = tax.Postal1

                                   }).ToList().FirstOrDefault();

                //เก็บข้อมูลทั้งหมดที่กรอกเพื่อไป check และไปใช้ส่ง api และเก็บเข้า base
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
                    srcCityName = ddlsrcCityName.SelectedItem == null ? "" : ddlsrcCityName.SelectedItem.Text,
                    srcDistrictName = ddlsrcDistrictName.SelectedItem == null ? "" : ddlsrcDistrictName.SelectedItem.Text,
                    srcPostalCode = txtsrcPostalCode.Text,
                    srcDetailAddress = txtsrcDetailAddress.Text,
                    Ref_Order = "API",
                    dstName = txtdstName.Text,
                    dstPhone = (txtdstPhone.Text.Contains(" ") ? "-" : txtdstPhone.Text),
                    dstHomePhone = txtdstHomePhone.Text,
                    dstProvinceName = ddldstProvinceName.SelectedItem.Text,
                    dstCityName = ddldstCityName.SelectedItem == null ? "" : ddldstCityName.SelectedItem.Text,
                    dstDistrictName = ddldstDistrictName.SelectedItem == null ? "" : ddldstDistrictName.SelectedItem.Text,
                    dstPostalCode = txtdstPostalCode.Text,
                    dstDetailAddress = txtdstDetailAddress.Text,
                    insured = 0,
                    Transport_Type = Convert.ToInt32(ddlExpress.SelectedValue),
                    weight = Convert.ToInt32(txtweight.Text),
                    width = Convert.ToInt32(txtwidth.Text),
                    length = Convert.ToInt32(txtlength.Text),
                    height = Convert.ToInt32(txtheight.Text),
                    remark = txtremark.Text,
                    SDpart = ddlSDpart.SelectedValue,
                    siteStorage = txtSiteStorage.Text.ToUpper(),
                    saleChannel = ddlReceiveLocation.SelectedValue == "select" ? "" : ddlReceiveLocation.SelectedValue,
                    TypeSend = Convert.ToInt32(ddlTypeSend.SelectedValue),

                };
                if (radioWorkOn.Checked)
                {
                    item.saleOn = radioWorkOn.Text;
                }
                else
                {
                    item.saleOn = radioWorkOff.Text;
                }
                var vali = service_Flashs.Validate_Transport(item, ddlReceiveLocation.SelectedValue, ddlFavorites.SelectedValue);
                if (vali == "PASS")
                {
                    Carrier_Entities.Orders.Add(item);
                    Carrier_Entities.SaveChanges();
                    if (item.Transport_Type == 1)
                    {
                        var res = service_Flashs.CreateOrderFLASH(newId, ddlFavorites.SelectedValue);
                        //Check การสร้าง order 
                        if (res.success == true)
                        {
                            String originalPath = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
                            string filePath = originalPath.Substring(0, originalPath.LastIndexOf("/Transport_Form")) + "/PDFFile/" + newId + ".pdf";
                            //Check path ของไฟล์ที่จะเปิดว่าเปิดได้หรือเปล่าถ้าได้ให้ลบ
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            //ทำการสร้างไฟล์ขึ้นมาใหม่
                            var returnText = service_Flashs.Get_Docment(newId, "/Transport_Form");
                            btnSave.Visible = false;
                            foreach (GridViewRow row in gv_Box.Rows)
                            {
                                Carrier_Entities.Order_Box.Add(new Order_Box { Docno = newId, Box_ID = Convert.ToInt32(((Label)row.FindControl("lbBox_ID")).Text), Box_Name = ((Label)row.FindControl("lbBox_Name")).Text, Qty = Convert.ToInt32(((TextBox)row.FindControl("txtQty")).Text) });
                            }
                            Carrier_Entities.SaveChanges();
                            ClientScript.RegisterStartupScript(this.GetType(), "Success", "<script type='text/javascript'>alert('succes : " + res.success + " Tracking NO : " + res.trackingno + "');window.location='Transport_Form?Docno=" + newId + "';</script>'");
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Process : " + res.success + " Message : " + res.trackingno + "')", true);
                        }
                    }
                    else
                    {
                        Order_Item order = new Order_Item
                        {
                            Docno = item.Docno,
                            Date_Success = DateTime.Now,
                            sign = null,
                            pno = null,
                            mchId = null,
                            sortCode = null,
                            dstStoreName = null,
                            sortingLineCode = null,
                            Qty = 1,
                            earlyFlightEnabled = null,
                            packEnabled = null,
                            upcountryCharge = null,
                            TypeSendKO = ddlFavorites.SelectedValue == "select" ? "SFG" : ddlFavorites.SelectedValue,
                            Status = "SL"
                        };
                        Carrier_Entities.Order_Item.Add(order);
                        Carrier_Entities.SaveChanges();
                        foreach (GridViewRow row in gv_Box.Rows)
                        {
                            Carrier_Entities.Order_Box.Add(new Order_Box { Docno = newId, Box_ID = Convert.ToInt32(((Label)row.FindControl("lbBox_ID")).Text), Box_Name = ((Label)row.FindControl("lbBox_Name")).Text, Qty = Convert.ToInt32(((TextBox)row.FindControl("txtQty")).Text) });
                        }
                        Carrier_Entities.SaveChanges();
                        ClientScript.RegisterStartupScript(this.GetType(), "Success", "<script type='text/javascript'>alert('succes : Succes ');window.location='Transport_Form?Docno=" + newId + "';</script>'");
                    }


                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + vali + "')", true);
                }
           
        }

        #endregion

        //คือการสร้างอีกหน้าเพื่อเรียกใช้งาน method static
        [WebMethod]
        public static List<string> AutoSearchSiteStorage(string site, string saleChannel, string workon,string BrandId)
        {
            InsideSFG_WFEntities InsideSFG_WF_Entities = new InsideSFG_WFEntities();
            Online_LazadaEntities Online_Lazada_Entities = new Online_LazadaEntities();
            CarrierEntities carrier_Entities = new CarrierEntities();
                var online = Online_Lazada_Entities.PROVINCEs.Select(s => s.PROVINCE_ID).ToList();
                var siteId = site.ToUpper();

            var BG = (from ha in InsideSFG_WF_Entities.BG_HApprove
                      join haP in InsideSFG_WF_Entities.BG_HApprove_Profitcenter on ha.departmentID equals haP.DepartmentID
                      where (ha.departmentID == BrandId || BrandId == "Select")
                      select haP.Depart_Short).ToList();
            List<string> sitepro  = new List<string>();
            if(BrandId != "Select")
            {
                if (saleChannel == "Event")
                {
                    sitepro = carrier_Entities.Event_Shop.Where(w => w.Shop_Code.Substring(0, site.Length).Contains(siteId)).Select(s => s.Shop_Code).ToList();
                }
                else
                {
                    sitepro = carrier_Entities.Site_Profit.Where(w => w.Channel == workon
                        && w.Site_Stroage.StartsWith(siteId)
                    //&& BG.Contains(w.Brand)
                    ).Select(s => s.Site_Stroage).Distinct().ToList();
                }
            }
            else
            {
                if (saleChannel == "Event")
                {
                    sitepro = carrier_Entities.Event_Shop.Where(w => w.Shop_Code.Substring(0, site.Length).Contains(siteId)).Select(s => s.Shop_Code).ToList();
                }
                else
                {
                    sitepro = carrier_Entities.Site_Profit.Where(w => w.Channel == workon
                        && w.Site_Stroage.StartsWith(siteId)
                    && ( BG.Contains(w.Brand) || BG.Count == 0)
                    ).Select(s => s.Site_Stroage).Distinct().ToList();
                }
            }
            return sitepro.Take(10).ToList();
            
        }

        protected void ddlExpress_SelectedIndexChanged(object sender, EventArgs e)
        {
            var express = ddlExpress.SelectedValue;
            if (express == "2")
            {
                ddlTypeSend.SelectedValue = "1";
                ddlTypeSend.Enabled = true;
            }
            else
            {
                ddlTypeSend.SelectedValue = "1";
                ddlTypeSend.Enabled = false;
            }
        }

        protected void btnSearchSite_Click(object sender, EventArgs e)
        {
            getAddressOnSite(txtSiteStorage.Text.ToUpper().Substring(0, 4));
        }

        protected void btnClearSite_Click(object sender, EventArgs e)
        {
            txtdstPostalCode.Text = "";
            ddldstProvinceName.SelectedValue = "0";
            ddldstCityName.DataSource = new List<City>();
            ddldstCityName.DataBind();
            //ddldstCityName.ClearSelection();
            ddldstCityName.Enabled = false;
            ddldstDistrictName.DataSource = new List<District>();
            //ddldstDistrictName.ClearSelection();
            ddldstDistrictName.DataBind();
            ddldstDistrictName.Enabled = false;
            txtdstDetailAddress.Text = "";
        }

        protected void lkSendMail_Click(object sender, EventArgs e)
        {
            div_main.Style.Add("filter","blur(15px)");
            div_main.Style.Add("position", "absolute");
            div_mail.Visible = true;
            
            List<newList> allFavorite = new List<newList>();
            allFavorite.Add(new newList { val = "select", text = "--" });
            allFavorite.Add(new newList { val = "Depart", text = "หน้าร้าน" });
            allFavorite.Add(new newList { val = "CENTER", text = "ลูกค้า" });
            allFavorite.Add(new newList { val = "Event", text = "Event" });
            allFavorite.Add(new newList { val = "CENTER_Other", text = "อื่นๆ" });
            allFavorite.ForEach(f => { if (f.val == "select") { f.text = "เลือกปลายทาง"; } });
            ddlTo.DataSource = allFavorite;
            ddlTo.DataBind();
            var FC = InsideSFG_WF_Entities.BG_ForeCast.Where(w => w.ActiveStatus == 1).GroupBy(g => g.DepartmentID).Select(s => new Forecasts { DepartmentID = s.Key });
            var depart = (from BG_HA in InsideSFG_WF_Entities.BG_HApprove
                          join BG_HAPF in InsideSFG_WF_Entities.BG_HApprove_Profitcenter on BG_HA.departmentID equals BG_HAPF.DepartmentID
                          where FC.Select(s => s.DepartmentID).Contains(BG_HA.departmentID) && (BG_HA.Sta == "B" || BG_HA.Sta == "S" || BG_HA.Sta == "N")
                          select new { departmentShot = BG_HAPF.Depart_Short, department_ = BG_HA.department_ }
                          ).OrderBy(r => r.department_).ToList();
            depart.Insert(0, new { departmentShot = "Select", department_ = "กรุณาเลือกแผนกที่ต้องการเบิก" });
            ddlBrand.DataSource = depart;
            ddlBrand.DataBind();
        }

        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            var on = "";
            if (RadioWork1.Checked)
            {
                on = RadioWork1.Text;
            }
            else if(RadioWork2.Checked)
            {
                on = RadioWork2.Text;
            }
            var res = service_Flashs.SendMail("apichat.f@sfg-th.com",new string[] {"apichat_075@hotmail.com"},"เพิ่ม SiteStorage", 
                "<HTML>"+
                "<body>"+
                "<p>SiteStorage : "+ txtSiteAdd.Text+ "</p>"+
                "<p>Brand : "+ ddlBrand.SelectedItem.Text+ "("+ ddlBrand.SelectedValue+")"+ "</p>"+
                "<p>Channel : "+on+"</p>"+
                "<p>Sale_Channel : "+ ddlTo.SelectedValue+"</p>"
                + "</body>"
                +"</HTML>"
                );
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + res + "')", true);
            div_main.Style.Remove("filter");
            div_main.Style.Remove("position");
            div_mail.Visible = false ;
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            div_main.Style.Remove("filter");
            div_main.Style.Remove("position");
            div_mail.Visible = false;
        }
    }

    #region Model
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
    public class BoxItem
    {
        public int Box_ID { get; set; }
        public string Box_Name { get; set; }
        public int Qty { get; set; }
    }
    #endregion
}