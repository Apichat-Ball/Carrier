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
using Carrier.Model.Budget;
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
        BudgetEntities budget_Entities;

        Service_Flash service_Flashs;
        public Transport_Form()
        {
            Carrier_Entities = new CarrierEntities();
            InsideSFG_WF_Entities = new InsideSFG_WFEntities();
            Whale_Entities = new WhaleEntities();
            Online_Lazada_Entities = new Online_LazadaEntities();
            budget_Entities = new BudgetEntities();
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
            var Act = Request.QueryString["Act"];
            var User = Session["_UserID"].ToString();
            lbuserID.Text = Session["_UserID"].ToString();
            if (!IsPostBack)
            {
                
                
                if (Docno != null)
                {
                    var checkBox = Carrier_Entities.Order_Big_Box.Where(w => w.BFID == Docno).FirstOrDefault();
                    var checkDocnoInBigBox = Carrier_Entities.Order_Big_Box.Where(w => w.Docno == Docno).FirstOrDefault();
                    List<Order_Big_Box> haveBox = new List<Order_Big_Box>();
                    if (checkBox != null)
                    {
                        haveBox = Carrier_Entities.Order_Big_Box.Where(w => w.BFID == checkBox.BFID).ToList();
                        Docno = haveBox.Select(s => s.Docno).FirstOrDefault();
                        txtDocno.Text = checkBox.BFID;
                    }
                    else if(checkDocnoInBigBox != null)
                    {
                        haveBox = Carrier_Entities.Order_Big_Box.Where(w => w.BFID == checkDocnoInBigBox.BFID).ToList();
                        Docno = haveBox.Select(s => s.Docno).FirstOrDefault();
                        txtDocno.Text = checkDocnoInBigBox.BFID;
                    }
                    else
                    {
                        txtDocno.Text = Request.QueryString["Docno"];
                    }
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
                                     SDpart = order.SDpart,
                                     saleOn = order.saleOn,
                                     SiteStorage = order.siteStorage,
                                     SaleChannel = order.saleChannel,
                                     Transport_Type = order.Transport_Type,
                                     TypeSendKO = orderItem.TypeSendKO,
                                     TypeSend = order.TypeSend,
                                     datesend = order.Date_send
                                 }).ToList().FirstOrDefault();

                    loadPage(query.datesend);
                    ddlTypeSend.SelectedValue = query.TypeSend.ToString();
                    ddlTypeSend.Enabled = false;
                    ddlExpress.SelectedValue = query.Transport_Type.ToString();
                    ddlExpress.Enabled = false;

                    txtsrcName.Text = query.SrcName;
                    txtsrcPhone.Text = query.SrcPhone;
                    var srcpro = Whale_Entities.Provinces.Where(w => w.Province_Name.StartsWith(query.SrcProvinces)).FirstOrDefault();
                    ddlsrcProvinceName.SelectedValue = srcpro.Province_ID.ToString();
                    txtsrcPostalCode.Text = query.SrcPostal;
                    txtsrcDetailAddress.Text = query.SrcDetailAddress;

                    var srcprovince = Convert.ToInt32(ddlsrcProvinceName.SelectedValue);
                    ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == srcprovince).ToList();
                    ddlsrcCityName.DataBind();
                    if (query.SrcCity == "")
                    {
                        ddlsrcCityName.Items.Insert(0, "-");
                    }
                    else
                    {
                        var srccityTemp = Whale_Entities.Cities.Where(w => w.City_Name.Contains(query.SrcCity)).FirstOrDefault().City_ID;
                        ddlsrcCityName.SelectedValue = srccityTemp.ToString();
                    }
                    //ddlsrcCityName.Enabled = true;

                    var srccity = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                    ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == srccity).ToList();
                    ddlsrcDistrictName.DataBind();
                    if (query.SrcDistrict != "")
                    {
                        var srcdistrictTemp = Whale_Entities.Districts.Where(w => w.City_ID == srccity && w.Distinct_Name.Contains(query.SrcDistrict)).FirstOrDefault();
                        if(srcdistrictTemp != null)
                        {

                        ddlsrcDistrictName.SelectedValue = srcdistrictTemp.Distinct_ID.ToString();
                        }
                    }
                    else
                    {
                        ddlsrcDistrictName.Items.Insert(0, "-");
                    }

                    //ddlsrcDistrictName.Enabled = true;

                    txtdstName.Text = query.DstName;
                    txtdstPhone.Text = query.DstPhone;
                    txtdstHomePhone.Text = query.DstHomePhone;
                    var dstpro = Whale_Entities.Provinces.Where(w => w.Province_Name.Contains(query.DstProvinces)).FirstOrDefault();
                    ddldstProvinceName.SelectedValue = dstpro.Province_ID.ToString();

                    var dstprovince = Convert.ToInt32(ddldstProvinceName.SelectedValue);
                    ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == dstprovince).ToList();
                    ddldstCityName.DataBind();
                    if (query.DstCity != "")
                    {
                        var dstcityTemp = Whale_Entities.Cities.Where(w => w.City_Name.Contains(query.DstCity)).FirstOrDefault().City_ID;
                        ddldstCityName.SelectedValue = dstcityTemp.ToString();
                    }
                    else
                    {
                        ddldstCityName.Items.Insert(0, "-");
                    }


                    var dstcity = Convert.ToInt32(ddldstCityName.SelectedValue);
                    ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == dstcity).ToList();
                    ddldstDistrictName.DataBind();
                    if (query.DstDistrict != "")
                    {
                        var dstdistrictTemp = Whale_Entities.Districts.Where(w => w.City_ID == dstcity && w.Distinct_Name.Contains(query.DstDistrict)).FirstOrDefault().Distinct_ID;
                        ddldstDistrictName.SelectedValue = dstdistrictTemp.ToString();
                    }
                    else
                    {
                        ddldstDistrictName.Items.Insert(0, "-");
                    }

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

                    btnSave.Visible = false;

                    ddlSDpart.SelectedValue = query.SDpart;
                    ddlSDpart.Enabled = false;
                    
                    lbDocno.Visible = true;
                    txtDocno.Visible = true;
                    

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


                    //BigBox


                    

                    List<newBox> a = new List<newBox>();
                    foreach(var bb in haveBox)
                    {
                        var boxInBase = Whale_Entities.Boxes.Where(w => w.Flag_Active == true && (w.Box_ID != 1 && w.Box_ID != 6 && w.Box_ID != 8 && w.Box_ID != 12 && w.Box_ID != 13 && w.Box_ID != 14 && w.Box_ID != 16 && w.Box_ID != 17)).ToList();
                        boxInBase.Insert(0, new Box { Box_ID = 0, Box_Name = "เลือกขนาดกล่อง" });
                        var Article = Carrier_Entities.Article_Category.ToList();
                        Article.Insert(0, new Article_Category { ArticleCode = 1111, ArticleName = "กรุณาเลือกประเภทพัสดุ" });
                        a.Add(new newBox { DTID = bb.BFID, TypeBox = boxInBase, Arti = Article , Docno = bb.Docno });
                    }
                    if(haveBox.Count() == 0)
                    {
                        var boxInBase = Whale_Entities.Boxes.Where(w => w.Flag_Active == true && (w.Box_ID != 1 && w.Box_ID != 6 && w.Box_ID != 8 && w.Box_ID != 12 && w.Box_ID != 13 && w.Box_ID != 14 && w.Box_ID != 16 && w.Box_ID != 17)).ToList();
                        boxInBase.Insert(0, new Box { Box_ID = 0, Box_Name = "เลือกขนาดกล่อง" });
                        var Article = Carrier_Entities.Article_Category.ToList();
                        Article.Insert(0, new Article_Category { ArticleCode = 1111, ArticleName = "กรุณาเลือกประเภทพัสดุ" });
                        a.Add(new newBox { DTID = "1", TypeBox = boxInBase, Arti = Article, Docno = Docno });
                    }
                    gv_Big_Box.DataSource = a;
                    gv_Big_Box.DataBind();
                    gv_Big_Box.ShowHeader = true;
                    
                    if (query.Transport_Type == 1)
                    {
                        gv_Big_Box.HeaderRow.Cells[0].Visible = true;
                        gv_Big_Box.HeaderRow.Cells[2].Visible = true;
                        divPrintAll.Visible = true;
                    }
                    else
                    {
                        gv_Big_Box.HeaderRow.Cells[0].Visible = false;
                        gv_Big_Box.HeaderRow.Cells[2].Visible = false;
                        divPrintAll.Visible = false;
                    }
                    List<string> boxSmall = new List<string> { };
                    foreach (GridViewRow row in gv_Big_Box.Rows)
                    {
                        Button btnPDF = (Button)row.FindControl("btnPDF");
                        ImageButton imgADD = (ImageButton)row.FindControl("imgADD");
                        Label lbBBoxID = (Label)row.FindControl("lbBBoxID");
                        Label lbhBBoxTracking = (Label)row.FindControl("lbhBBoxTracking");
                        Label lbBBoxTracking = (Label)row.FindControl("lbBBoxTracking");
                        DropDownList ddlarticleCategory = (DropDownList)row.FindControl("ddlarticleCategory");
                        TextBox txtremark = (TextBox)row.FindControl("txtremark");
                        Label lbhBox = (Label)row.FindControl("lbhBox");
                        DropDownList ddlBox = (DropDownList)row.FindControl("ddlBox");
                        Label lbInputQty = (Label)row.FindControl("lbInputQty");
                        TextBox txtQty = (TextBox)row.FindControl("txtQty");
                        Button btnAdd = (Button)row.FindControl("btnAdd");
                        GridView gv_Small_Box = (GridView)row.FindControl("gv_Small_Box");
                        Label lbhArticleCategory = (Label)row.FindControl("lbhArticleCategory");
                        Label lbhRemark = (Label)row.FindControl("lbhRemark");
                        ImageButton imgDeleteBox = (ImageButton)row.FindControl("imgDeleteBox");
                        Label lbStatuspno = (Label)row.FindControl("lbStatuspno");

                        var orderIt = Carrier_Entities.Order_Item.Where(w => w.Docno == lbBBoxID.Text).FirstOrDefault();
                        if(orderIt.ticketPickupId != null)
                        {
                            var notify = Carrier_Entities.Notifies.Where(w => w.TicketPickupId == orderIt.ticketPickupId).FirstOrDefault();
                            lbStatuspno.Text = notify.TimeoutAtText;
                            var dateNotiDate = new DateTime();
                            if (lbStatuspno.Text.Contains("พรุ่งนี้"))
                            {
                                var dateRaw = DateTime.Parse(notify.DateNotify.ToString()).AddDays(1);
                                dateNotiDate = DateTime.Parse(dateRaw.ToShortDateString());
                                var dateToUpdate = dateNotiDate.AddHours(16).AddMinutes(45);
                                if (DateTime.Now >= dateToUpdate)
                                {
                                    var date = DateTime.Parse(notify.DateNotify.ToString());
                                    var c = service_Flashs.CheckNotify(lbBBoxID.Text);
                                    if (c != "")
                                    {
                                        lbStatuspno.Text = c;
                                    }
                                }
                                else
                                {
                                    if (DateTime.Now.ToShortDateString() == dateToUpdate.ToShortDateString())
                                    {
                                        lbStatuspno.Text = "วันนี้" + lbStatuspno.Text.Substring(8);
                                    }
                                }
                            }
                            else if (lbStatuspno.Text.Contains("วันนี้"))
                            {
                                var dateRaw = DateTime.Parse(notify.DateNotify.ToString());
                                dateNotiDate = DateTime.Parse(dateRaw.ToShortDateString());
                                var dateToUpdate = dateNotiDate.AddHours(17).AddMinutes(30);
                                if (DateTime.Now >= dateToUpdate)
                                {
                                    var date = DateTime.Parse(notify.DateNotify.ToString());
                                    var c = service_Flashs.CheckNotify(lbBBoxID.Text);
                                    if (c != "")
                                    {
                                        lbStatuspno.Text = c;
                                    }
                                    else
                                    {
                                        lbStatuspno.Text = "ยังไม่ได้มารับของ";
                                    }
                                }
                            }
                            gv_Big_Box.HeaderRow.Cells[6].Text = "สถานะกล่อง";
                            lbStatuspno.Visible = true;

                        }
                        else
                        {
                            if (orderIt.Status == "A")
                            {
                                var c = service_Flashs.CheckNotify(lbBBoxID.Text);
                                if (c != "")
                                {
                                    lbStatuspno.Text = c;
                                }
                                
                                gv_Big_Box.HeaderRow.Cells[6].Text = "สถานะกล่อง";
                                lbStatuspno.Visible = true;
                            }
                            else
                            {
                                if(orderIt.Status == "C")
                                {
                                    lbStatuspno.Text = "ยกเลิกแล้ว";
                                    gv_Big_Box.HeaderRow.Cells[6].Text = "สถานะกล่อง";
                                    lbStatuspno.Visible = true;
                                }
                                
                            }
                        }
                        
                        imgDeleteBox.Visible = false;
                        if (lbStatuspno.Text.Contains("ยกเลิกแล้ว"))
                        {
                            lbStatuspno.BackColor = System.Drawing.Color.PaleVioletRed;
                            lbStatuspno.ForeColor = System.Drawing.Color.White;
                            lbStatuspno.CssClass = "status-tracking";
                        }
                        else if (lbStatuspno.Text.Contains("ยังไม่ได้มารับของ"))
                        {
                            lbStatuspno.BackColor = System.Drawing.Color.Orange;
                            lbStatuspno.ForeColor = System.Drawing.Color.White;
                            lbStatuspno.CssClass = "status-tracking";
                        }
                        else
                        {
                            lbStatuspno.BackColor = System.Drawing.Color.LimeGreen;
                            lbStatuspno.ForeColor = System.Drawing.Color.White;
                            lbStatuspno.CssClass = "status-tracking";
                        }

                        
                        

                        if (query.Transport_Type == 1)
                        {
                            row.Cells[0].Visible = true;
                            row.Cells[2].Visible = true;
                        }
                        else
                        {
                            row.Cells[0].Visible = false;
                            row.Cells[2].Visible = false;
                        }


                        lbInputQty.Visible = false;
                        txtQty.Visible = false;
                        btnAdd.Visible = false;

                        lbhRemark.Visible = false;
                        lbhArticleCategory.Visible = false;

                        //Get OrderDetail and Tracking
                        var order = (from or in Carrier_Entities.Orders
                                     join orit in Carrier_Entities.Order_Item on or.Docno equals orit.Docno
                                     where or.Docno == lbBBoxID.Text
                                     select new
                                     {
                                         pno = orit.pno,
                                         remark = or.remark,
                                         status = or.status,
                                         article = or.articleCategory,
                                         transport_Type = or.Transport_Type
                                     }).FirstOrDefault();
                        ddlarticleCategory.Enabled = false;
                        ddlarticleCategory.SelectedValue = order.article.ToString();
                        txtremark.Enabled = false;
                        txtremark.Text = order.remark;
                        lbBBoxTracking.Text = order.pno;
                        gv_Small_Box.Visible = true;
                        ddlBox.Visible = false;
                        lbhBox.Visible = false;
                        btnPDF.Visible = true; 

                        

                        if (order.status == "AP")
                        {
                            btnPDF.CssClass = "btn btn-danger";
                        }
                        else
                        {
                            btnPDF.CssClass = "btn btn-primary";
                        }

                        var listBox = Carrier_Entities.Order_Box.Where(w => w.Docno == lbBBoxID.Text).ToList();
                        if(listBox.Count() != 0)
                        {
                            gv_Small_Box.DataSource = listBox;
                            gv_Small_Box.DataBind();
                            gv_Small_Box.Columns[2].Visible = false;
                            foreach(GridViewRow rows in gv_Small_Box.Rows)
                            {
                                TextBox txtQtytb = (TextBox)rows.FindControl("txtQty");
                                txtQtytb.Enabled = false;
                            }
                            boxSmall.Add("0");
                        }
                        else
                        {
                            boxSmall.Add("1");
                            gv_Small_Box.Visible = false;
                        }
                    }
                    if (boxSmall.Where(w => w == "0").Count() == 0)
                    {
                        gv_Big_Box.HeaderRow.Cells[5].Visible = false ;
                        foreach (GridViewRow row in gv_Big_Box.Rows)
                        {
                            row.Cells[5].Visible = false;
                        }
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
                    ddlFavorites.Items.Insert(3, new ListItem { Value = "ROX", Text = "R.O.X.Flagship store" });
                    ddlFavorites.SelectedValue = query.TypeSendKO;
                    lbFavorites.Visible = true;
                    ddlFavorites.Visible = true;
                    ddlFavorites.Enabled = false;
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
                    lbGuidSiteStorage2.Visible = false;

                    #region Edit
                    //if (Act == "Edit")
                    //{
                    //    txtsrcName.Enabled = true;
                    //    txtsrcPhone.Enabled = true;
                    //    ddlsrcProvinceName.Enabled = true;
                    //    ddlsrcCityName.Enabled = true;
                    //    ddlsrcDistrictName.Enabled = true;
                    //    txtsrcPostalCode.Enabled = true;
                    //    txtsrcDetailAddress.Enabled = true;
                    //    txtsrcDetailAddress.Enabled = true;
                    //    txtremark.Enabled = true;
                    //    radioWorkOn.Enabled = true;
                    //    radioWorkOff.Enabled = true;
                    //    ddlReceiveLocation.Enabled = true;
                    //    ddlSDpart.Enabled = true;
                    //    txtSiteStorage.Enabled = true;
                    //    txtdstName.Enabled = true;
                    //    txtdstPhone.Enabled = true;
                    //    txtdstHomePhone.Enabled = true;
                    //    ddldstProvinceName.Enabled = true;
                    //    ddldstCityName.Enabled = true;
                    //    ddldstDistrictName.Enabled = true;
                    //    txtdstPostalCode.Enabled = true;
                    //    txtdstDetailAddress.Enabled = true;
                    //    ddlBox.Enabled = true;
                    //    txtQty.Enabled = true;
                    //    btnAdd.Enabled = true;
                    //    if (gv_Box != null)
                    //    {
                    //        gv_Box.Columns[2].Visible = true;
                    //    }
                    //    btnSave.Visible = true;
                    //    ddlarticleCategory.Enabled = true;
                    //}
                    #endregion
                }
                else
                {
                    string username = HttpContext.Current.Request.Cookies["sfgweb"]["uname"].Trim();
                    //string username = "9021517";
                    var objuser = (from tEmployee in InsideSFG_WF_Entities.Employees
                                   where (tEmployee.username_ == username || tEmployee.uCode == username)
                                   && tEmployee.StatWork == "Y"
                                   select tEmployee
                                      ).FirstOrDefault();
                    if (objuser != null)
                    {
                        txtsrcName.Text = objuser.name + " " + objuser.surname;
                        if (objuser.Mobile != null && objuser.Mobile.Contains(" "))
                        {
                            txtsrcPhone.Text = objuser.Mobile.Replace(" ", "");
                        }
                        else if (objuser.Mobile != null)
                        {
                            txtsrcPhone.Text = objuser.Mobile.Replace("-", "");
                        }

                    }
                    loadPage(DateTime.Now);

                    radioWorkOn.Checked = true;
                    List<newList> allFavorite = new List<newList>();

                    allFavorite.Insert(0, new newList { val = "select", text = "เลือกผู้ส่งที่ใช้บ่อย" });
                    allFavorite.Insert(1, new newList { val = "SFG", text = "Star Fashion Group" });
                    allFavorite.Insert(2, new newList { val = "SDC1", text = "SDC1" });
                    ddlFavorites.DataSource = allFavorite;
                    ddlFavorites.DataBind();
                    ddlFavorites.Items.Insert(3, new ListItem { Value = "ROX", Text = "R.O.X.Flagship store" });
                    allFavorite.Insert(3, new newList { val = "Depart", text = "หน้าร้าน" });
                    allFavorite.Insert(4, new newList { val = "CENTER", text = "ลูกค้า" });
                    allFavorite.Insert(5, new newList { val = "Event", text = "Event" });
                    allFavorite.Insert(6, new newList { val = "CENTER_Other", text = "อื่นๆ" });
                    allFavorite.ForEach(f => { if (f.val == "select") { f.text = "เลือกปลายทาง"; } });
                    ddlReceiveLocation.DataSource = allFavorite;
                    ddlReceiveLocation.DataBind();
                    lbFavorites.Visible = true;
                    ddlFavorites.Visible = true;
                    if (ddlExpress.SelectedValue == "1")
                    {
                        ddlTypeSend.SelectedValue = "1";
                        ddlTypeSend.Enabled = false;
                    }

                    gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
                    gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
                    gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
                    gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
                    gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
                    gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
                    gv_Big_Box.HeaderRow.Cells.RemoveAt(0);

                }
            }
        }

        public void loadPage(DateTime? date)
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

            var bubget = budget_Entities.MainBudgets.Where(w => w.Year_Budget == date.Value.Year  ).GroupBy(g => g.Department_ID).Select(s => s.Key).ToList();
            var FC = (from d in budget_Entities.Departments
                      where new string[] { "F", "VIP" }.Contains(d.Flag) && bubget.Contains(d.Department_ID) && !d.Department_Name.Contains("SEEK") && !d.Department_Name.Contains("SDC1")
                      select new { departmentID = d.Department_ID, department_ = d.Department_Name }).OrderBy(o=>o.department_).ToList();
            
            var seek = budget_Entities.Departments.Where(w => w.Department_Name.Contains("SEEK") && !new string[] { "1508", "1619" }.Contains(w.Department_ID)).Select(s=>new { departmentID = s.Department_ID, department_ = s.Department_Name }).OrderBy(o=>o.department_).ToList();
            FC.AddRange(seek);
            FC.Insert(0, new { departmentID = "Select", department_ = "กรุณาเลือกแผนกที่ต้องการเบิก" });
            ddlSDpart.DataSource = FC;
            ddlSDpart.DataBind();
            


            var box = Whale_Entities.Boxes.Where(w => w.Flag_Active == true && (w.Box_ID != 1 && w.Box_ID != 6 && w.Box_ID != 8 && w.Box_ID != 12 && w.Box_ID != 13 && w.Box_ID != 14 && w.Box_ID != 16 && w.Box_ID != 17)).ToList();
            box.Insert(0, new Box { Box_ID = 0, Box_Name = "เลือกขนาดกล่อง" });
            List<newBox> a = new List<newBox>();
            var Article = Carrier_Entities.Article_Category.ToList();
            Article.Insert(0, new Article_Category { ArticleCode = 1111, ArticleName = "กรุณาเลือกประเภทพัสดุ" });
            a.Add(new newBox { Docno = "1", TypeBox = box, Arti = Article });
            a.Add(new newBox { Docno = "", TypeBox = box, Arti = Article });
            gv_Big_Box.DataSource = a;
            gv_Big_Box.DataBind();
            setTableBigBox();
            
        }

        public void setTableBigBox()
        {
            foreach (GridViewRow row in gv_Big_Box.Rows)
            {
                Label lbBBoxID = (Label)row.FindControl("lbBBoxID");
                GridView gv_Small_Box = (GridView)row.FindControl("gv_Small_Box");
                TextBox txtQty = (TextBox)row.FindControl("txtQty");
                ImageButton imgADD = (ImageButton)row.FindControl("imgADD");
                if (lbBBoxID.Text == "NEW" || lbBBoxID.Text == "")
                {
                    row.Cells[0].ColumnSpan = 6;
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    row.Cells[0].CssClass = "gj-text-align-center";
                    imgADD.Visible = true;
                    
                }
                else
                {

                    row.Cells[0].Visible = false;
                    row.Cells[1].Visible = false;
                    row.Cells[2].Visible = false;
                    row.Cells[6].Visible = true;
                }

                txtQty.Text = "0";
            }
            //gv_Big_Box.ShowHeader = false;
        }
        public void setTableBigBoxNoHeader()
        {
            foreach (GridViewRow row in gv_Big_Box.Rows)
            {
                Label lbBBoxID = (Label)row.FindControl("lbBBoxID");
                GridView gv_Small_Box = (GridView)row.FindControl("gv_Small_Box");
                TextBox txtQty = (TextBox)row.FindControl("txtQty");
                ImageButton imgADD = (ImageButton)row.FindControl("imgADD");
                if (lbBBoxID.Text == "NEW" || lbBBoxID.Text == "")
                {
                    row.Cells[0].ColumnSpan = 6;
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    row.Cells[0].CssClass = "gj-text-align-center";
                    imgADD.Visible = true;

                }
                else
                {

                    row.Cells[0].Visible = false;
                    row.Cells[1].Visible = false;
                    row.Cells[2].Visible = false;
                    row.Cells[6].Visible = true;
                }
                
                txtQty.Text = "0";
            }
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            //gv_Big_Box.ShowHeader = false;
        }
        public void setTableBigBoxAction(List<BoxSmallLoad> listSmallBox)
        {
            foreach (GridViewRow row in gv_Big_Box.Rows)
            {
                Label lbBBoxID = (Label)row.FindControl("lbBBoxID");
                TextBox txtQty = (TextBox)row.FindControl("txtQty");
                ImageButton imgADD = (ImageButton)row.FindControl("imgADD");
                GridView gv_Small_Box = (GridView)row.FindControl("gv_Small_Box");
                DropDownList ddlarticleCategory = (DropDownList)row.FindControl("ddlarticleCategory");
                if (lbBBoxID.Text == "NEW" || lbBBoxID.Text == "")
                {
                    row.Cells[0].ColumnSpan = 6;
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    gv_Big_Box.Rows[row.RowIndex].Cells.RemoveAt(1);
                    row.Cells[0].CssClass = "gj-text-align-center";
                    imgADD.Visible = true;
                }
                else
                {
                    var have = listSmallBox.Where(w => w.BigBox == lbBBoxID.Text).FirstOrDefault();
                    if (have != null)
                    {
                        gv_Small_Box.DataSource = have.item;
                        gv_Small_Box.DataBind();
                        ddlarticleCategory.SelectedValue = have.Arti_Select;
                    }
                    row.Cells[0].Visible = false;
                    row.Cells[1].Visible = false;
                    row.Cells[2].Visible = false;
                    row.Cells[6].Visible = true;
                }
                txtQty.Text = "0";
            }
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            //gv_Big_Box.ShowHeader = false;
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
            var Provincelist = Whale_Entities.Provinces.OrderBy(o => o.Province_Name).ToList();
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
                ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == province).OrderBy(o=>o.City_Name).ToList();
                ddlsrcCityName.DataBind();
                ddlsrcDistrictName.Enabled = true;

                var city = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).OrderBy(o=>o.Distinct_Name).ToList();
                ddlsrcDistrictName.DataBind();

                var district = Convert.ToInt32(ddlsrcDistrictName.SelectedValue);
                txtsrcPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
            }
            setTableBigBoxNoHeader();
        }

        protected void ddlsrcCityName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlsrcDistrictName.Enabled = true;
            var city = Convert.ToInt32(ddlsrcCityName.SelectedValue);
            ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).OrderBy(o=>o.Distinct_Name).ToList();
            ddlsrcDistrictName.DataBind();

            var district = Convert.ToInt32(ddlsrcDistrictName.SelectedValue);
            txtsrcPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
            setTableBigBoxNoHeader();
        }

        protected void ddlsrcDistrictName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var district = Convert.ToInt32(ddlsrcDistrictName.SelectedValue);
            txtsrcPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
            setTableBigBoxNoHeader();
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
                ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == province).OrderBy(o=>o.City_Name).ToList();
                ddldstCityName.DataBind();
                ddldstDistrictName.Enabled = true;

                var city = Convert.ToInt32(ddldstCityName.SelectedValue);
                ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).OrderBy(o=>o.Distinct_Name).ToList();
                ddldstDistrictName.DataBind();

                var district = Convert.ToInt32(ddldstDistrictName.SelectedValue);
                txtdstPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
            }
            setTableBigBoxNoHeader();
        }

        protected void ddldstCityName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddldstDistrictName.Enabled = true;
            var city = Convert.ToInt32(ddldstCityName.SelectedValue);
            ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).OrderBy(o=>o.Distinct_Name).ToList();
            ddldstDistrictName.DataBind();
            txtdstPostalCode.Text = Whale_Entities.Districts.Where(w => w.City_ID == city).FirstOrDefault().Postcode.ToString();

            var district = Convert.ToInt32(ddldstDistrictName.SelectedValue);
            txtdstPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
            setTableBigBoxNoHeader();
        }

        protected void ddldstDistrictName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var district = Convert.ToInt32(ddldstDistrictName.SelectedValue);
            txtdstPostalCode.Text = Whale_Entities.Districts.Where(w => w.Distinct_ID == district).FirstOrDefault().Postcode.ToString();
            setTableBigBoxNoHeader();
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

                    ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provinceSFG).OrderBy(o=>o.City_Name).ToList();
                    ddlsrcCityName.DataBind();
                    ddlsrcCityName.SelectedValue = "20";
                    ddlsrcCityName.Enabled = true;

                    ddlsrcDistrictName.Enabled = true;
                    var citySFG = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                    ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == citySFG).OrderBy(o=>o.Distinct_Name).ToList();
                    ddlsrcDistrictName.DataBind();
                    ddlsrcDistrictName.SelectedValue = "119";
                    txtsrcPostalCode.Text = "10120";
                    txtsrcDetailAddress.Text = "บริษัท สตาร์แฟชั่น(2551) จำกัด " + "477 พระราม 3 ";

                    break;
                case "SDC1":
                    //ผู้ส่ง
                    //var  user = HttpContext.Current.Session[]
                    //var fromUser = InsideSFG_WF_Entities.Employees.Where(w=>w.userID == )
                    txtsrcName.Text = "ณัฐชยา พงศ์ทองกุล";
                    txtsrcPhone.Text = "0988325926";
                    ddlsrcProvinceName.SelectedValue = "5";
                    var provinceSDC1 = Convert.ToInt32(ddlsrcProvinceName.SelectedValue);

                    ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provinceSDC1).OrderBy(o=>o.City_Name).ToList();
                    ddlsrcCityName.DataBind();
                    ddlsrcCityName.SelectedValue = "755";
                    ddlsrcCityName.Enabled = true;

                    ddlsrcDistrictName.Enabled = true;
                    var citySDC1 = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                    ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == citySDC1).OrderBy(o=>o.Distinct_Name).ToList();
                    ddlsrcDistrictName.DataBind();
                    ddlsrcDistrictName.SelectedValue = "483";
                    txtsrcPostalCode.Text = "13170";
                    txtsrcDetailAddress.Text = "บริษัท เอส.ดี.ซี วัน จำกัด " + "59/1 ม.1 ";

                    break;
                case "ROX":
                    //txtsrcName.Text = "บริษัท สตาร์แฟชั่น(2551) จำกัด";
                    //txtsrcPhone.Text = "0873078300";
                    ddlsrcProvinceName.SelectedValue = "1";
                    var provinceROX = Convert.ToInt32(ddlsrcProvinceName.SelectedValue);

                    ddlsrcCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provinceROX).OrderBy(o=>o.City_Name).ToList();
                    ddlsrcCityName.DataBind();
                    ddlsrcCityName.SelectedValue = "20";
                    ddlsrcCityName.Enabled = true;

                    ddlsrcDistrictName.Enabled = true;
                    var cityROX = Convert.ToInt32(ddlsrcCityName.SelectedValue);
                    ddlsrcDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == cityROX).OrderBy(o=>o.Distinct_Name).ToList();
                    ddlsrcDistrictName.DataBind();
                    ddlsrcDistrictName.SelectedValue = "119";
                    txtsrcPostalCode.Text = "10120";
                    txtsrcDetailAddress.Text = "บริษัท สตาร์แฟชั่น(2551) จำกัด " + "477 พระราม 3 ";

                    break;
            }
            setTableBigBoxNoHeader();
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
                        ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provincedstSCD1).OrderBy(o=>o.City_Name).ToList();
                        ddldstCityName.DataBind();
                        ddldstCityName.SelectedValue = "755";

                        ddldstDistrictName.Enabled = true;
                        var citydstSCD1 = Convert.ToInt32(ddldstCityName.SelectedValue);
                        ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == citydstSCD1).OrderBy(o=>o.Distinct_Name).ToList();
                        ddldstDistrictName.DataBind();
                        ddldstDistrictName.SelectedValue = "483";
                        txtdstPostalCode.Text = "13170";
                        txtdstDetailAddress.Text = "บริษัท เอส.ดี.ซี วัน จำกัด " + "59/1 ม.1 ";
                        break;
                    case "SFG":

                        //ผู้รับ
                        //txtdstName.Text = "บริษัท สตาร์แฟชั่น(2551) จำกัด";
                        //txtdstPhone.Text = "0873078300";
                        //txtdstHomePhone.Text = "-";
                        ddldstProvinceName.SelectedValue = "1";
                        var provincedstSFG = Convert.ToInt32(ddldstProvinceName.SelectedValue);

                        ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provincedstSFG).OrderBy(o=>o.City_Name).ToList();
                        ddldstCityName.DataBind();
                        ddldstCityName.SelectedValue = "20";
                        ddldstCityName.Enabled = true;

                        ddldstDistrictName.Enabled = true;
                        var citydstSFG = Convert.ToInt32(ddldstCityName.SelectedValue);
                        ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == citydstSFG).OrderBy(o => o.Distinct_Name).ToList();
                        ddldstDistrictName.DataBind();
                        ddldstDistrictName.SelectedValue = "119";
                        txtdstPostalCode.Text = "10120";
                        txtdstDetailAddress.Text = "บริษัท สตาร์แฟชั่น(2551) จำกัด " + "477 พระราม 3 ";
                        break;
                }
            }
            setTableBigBoxNoHeader();
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
                    ddldstCityName.DataSource = Whale_Entities.Cities.Where(w => w.Province_ID == provinceSDC1).OrderBy(o=>o.City_Name).ToList();
                    ddldstCityName.DataBind();
                    ddldstCityName.SelectedValue = citylike.City_ID.ToString();
                    ddldstCityName.Enabled = true;

                    var city = Convert.ToInt32(ddldstCityName.SelectedValue);
                    var districtSplit = address.dstDistrict.StartsWith("แขวง")? address.dstDistrict.Substring(4).TrimEnd(): address.dstDistrict;
                    var districtlike = Whale_Entities.Districts.Where(w => w.City_ID == city && w.Distinct_Name.Contains(districtSplit)).OrderBy(o=>o.Distinct_Name).ToList().FirstOrDefault();
                    if (districtlike != null)
                    {
                        ddldstDistrictName.Enabled = true;
                        ddldstDistrictName.DataSource = Whale_Entities.Districts.Where(w => w.City_ID == city).OrderBy(o=>o.Distinct_Name).ToList();
                        ddldstDistrictName.DataBind();
                        ddldstDistrictName.SelectedValue = districtlike.Distinct_ID.ToString();
                    }
                    else
                    {
                        var district = Whale_Entities.Districts.Where(w => w.City_ID == city).OrderBy(o=>o.Distinct_Name).OrderBy(o=>o.Distinct_Name).ToList();
                        district.Insert(0, new District { Distinct_ID = 0, Distinct_Name = "เลือกตำบล" });
                        ddldstDistrictName.Enabled = true;
                        ddldstDistrictName.DataSource = district;
                        ddldstDistrictName.DataBind();
                    }

                }
                else
                {
                    var city = Whale_Entities.Cities.Where(w => w.Province_ID == provinceSDC1).OrderBy(o=>o.City_Name).ToList();
                    city.Insert(0, new City { City_ID = 0, City_Name = "เลือกอำเภอ" });
                    ddldstCityName.Enabled = true;
                    ddldstCityName.DataSource = city;
                    ddldstCityName.DataBind();
                    ddldstDistrictName.Enabled = false;
                    ddldstDistrictName = new DropDownList();
                    ddldstDistrictName.DataBind();
                }

                txtdstPostalCode.Text = address.dstPostal;
                txtdstDetailAddress.Text = address.NameTax + " " + address.dstDetail;
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
            Button btnAdd = (Button)sender;
            GridViewRow rowB = (GridViewRow)btnAdd.NamingContainer;
            DropDownList ddlBox = (DropDownList)rowB.FindControl("ddlBox");
            TextBox txtQty = (TextBox)rowB.FindControl("txtQty");
            GridView gv_Small_Box = (GridView)rowB.FindControl("gv_Small_Box");

            if (ddlBox.SelectedValue != "0" && (txtQty.Text != "" && txtQty.Text != "0"))
            {
                List<BoxItem> listBox = new List<BoxItem>();
                foreach (GridViewRow row in gv_Small_Box.Rows)
                {
                    listBox.Add(new BoxItem { Box_ID = Convert.ToInt32(((Label)row.FindControl("lbBox_ID")).Text), Box_Name = ((Label)row.FindControl("lbBox_Name")).Text, Qty = Convert.ToInt32(((TextBox)row.FindControl("txtQty")).Text) });
                }
                listBox.Add(new BoxItem { Box_ID = Convert.ToInt32(ddlBox.SelectedValue), Box_Name = ddlBox.SelectedItem.Text, Qty = Convert.ToInt32(txtQty.Text) });

                gv_Small_Box.DataSource = listBox;
                gv_Small_Box.DataBind();
            }
            setTableBigBox();
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
        }

        protected void btnDeleteBoxItem_Click(object sender, EventArgs e)
        {
            Button btnDeleteBoxItem = (Button)sender;
            GridViewRow row = (GridViewRow)btnDeleteBoxItem.NamingContainer;
            GridView gv = (GridView)row.NamingContainer;
            List<BoxItem> item = new List<BoxItem>();
            foreach (GridViewRow rows in gv.Rows)
            {
                if (rows != row)
                {

                    item.Add(new BoxItem { Box_ID = Convert.ToInt32(((Label)rows.FindControl("lbBox_ID")).Text), Box_Name = ((Label)rows.FindControl("lbBox_Name")).Text, Qty = Convert.ToInt32(((TextBox)rows.FindControl("txtQty")).Text) });
                }
            }
            gv.DataSource = item;
            gv.DataBind();
            setTableBigBox();
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
            gv_Big_Box.HeaderRow.Cells.RemoveAt(0);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            Button btnPDF = (Button)sender;
            GridViewRow row = (GridViewRow)btnPDF.NamingContainer;
            Label lbBBoxID = (Label)row.FindControl("lbBBoxID");
            //เปิดหน้าแสดง PDF
            Response.Redirect("Transport_bill?Docno=" + lbBBoxID.Text);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var Act = Request.QueryString["Act"];
            if (Act != "Edit")
            {
                #region V1
                //var docno = Carrier_Entities.Orders.Where(w=>w.Docno.StartsWith("FL")).ToList().LastOrDefault();
                //var newId = "";
                //if (docno == null)
                //{
                //    var id = "FL0000000001";
                //    newId = id;
                //}
                //else
                //{
                //    newId = docno.Docno;
                //    var lastId = Convert.ToInt32(newId.Substring(2, 10)) + 1;
                //    newId = newId.Substring(0, 2) + newId.Substring(2, 10 - lastId.ToString().Length) + lastId.ToString();
                //}
                #endregion

                #region Run BigBoxID
                var docnow = "BF" + DateTime.Now.Year.ToString().Substring(2, 2);
                var BigBoxID = Carrier_Entities.Order_Big_Box.Where(w => w.BFID.StartsWith(docnow)).OrderBy(o => o.RunID).ToList() ;
                var BBID = "";
                if (BigBoxID.Count() == 0)
                {

                    BBID = "BF" + DateTime.Now.Year.ToString().Substring(2, 2) + "00000001";
                    var Run = Carrier_Entities.RunDocnoes.Where(w => w.Type == "BF" && w.Year == DateTime.Now.Year).FirstOrDefault();
                    if (Run == null)
                    {
                        Carrier_Entities.RunDocnoes.Add(new RunDocno { Type = "BF", Year = DateTime.Now.Year, RunNo = 1 });
                        Carrier_Entities.SaveChanges();
                    }
                    else
                    {
                        Run.RunNo = Run.RunNo + 1;
                        Carrier_Entities.SaveChanges();
                    }
                }
                else
                {
                    BBID = BigBoxID.LastOrDefault().BFID.Substring(0, 4) + BigBoxID.LastOrDefault().BFID.Substring(4, 8 - (Convert.ToInt32(BigBoxID.LastOrDefault().BFID.Substring(4, 8)) + 1).ToString().Length) + (Convert.ToInt32(BigBoxID.LastOrDefault().BFID.Substring(4, 8)) + 1).ToString();
                    Carrier_Entities.RunDocnoes.Where(w => w.Type == "BF" & w.Year == DateTime.Now.Year).FirstOrDefault().RunNo += 1;
                    Carrier_Entities.SaveChanges();
                }
                #endregion
                
                var ss = gv_Big_Box.Rows.Count -1;
                if(ss != 0)
                {
                    List<string> checkVali = new List<string>();
                    foreach (GridViewRow row in gv_Big_Box.Rows)
                    {
                        if (row.RowIndex != ss)
                        {
                            DropDownList ddlarticleCategory = (DropDownList)row.FindControl("ddlarticleCategory");
                            TextBox txtremark = (TextBox)row.FindControl("txtremark");
                            GridView gv_Small_Box = (GridView)row.FindControl("gv_Small_Box");

                            var item = new Order
                            {
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
                                Ref_Order = "Carrier",
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
                            if (vali != "PASS")
                            {
                                checkVali.Add(vali);
                            }
                        }
                    }

                    if(checkVali.Where(w=>w != "PASS").ToList().Count() != 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + checkVali.Where(w=>w != "PASS").FirstOrDefault() + "')", true);
                    }
                    else
                    {
                        foreach (GridViewRow row in gv_Big_Box.Rows)
                        {
                            if (row.RowIndex != ss)
                            {
                                DropDownList ddlarticleCategory = (DropDownList)row.FindControl("ddlarticleCategory");
                                TextBox txtremark = (TextBox)row.FindControl("txtremark");
                                GridView gv_Small_Box = (GridView)row.FindControl("gv_Small_Box");

                                #region V2

                                var yearnow = "FL" + DateTime.Now.Year.ToString().Substring(2, 2);
                                var docno = Carrier_Entities.Orders.Where(w => w.Docno.StartsWith(yearnow)).ToList().OrderBy(o => o.Date_send).LastOrDefault();
                                var newId = "";
                                if (docno == null)
                                {
                                    var docnoRun = Carrier_Entities.RunDocnoes.Where(w => w.Type == "FL" && w.Year == DateTime.Now.Year).FirstOrDefault();
                                    if (docnoRun == null)
                                    {
                                        Carrier_Entities.RunDocnoes.Add(new RunDocno { Type = "FL", Year = DateTime.Now.Year, RunNo = 1 });
                                        Carrier_Entities.SaveChanges();
                                    }
                                    else
                                    {
                                        docnoRun.RunNo = 1;
                                        Carrier_Entities.SaveChanges();
                                    }
                                    var id = "FL" + DateTime.Now.Year.ToString().Substring(2, 2) + "00000001";
                                    newId = id;
                                }
                                else
                                {
                                    var docnoRun = Carrier_Entities.RunDocnoes.Where(w => w.Type == "FL" && w.Year == DateTime.Now.Year).FirstOrDefault();
                                    if (docnoRun == null)
                                    {
                                        var last = Convert.ToInt32(docno.Docno.Substring(4, 8)) + 1;
                                        Carrier_Entities.RunDocnoes.Add(new RunDocno { Type = "FL", Year = DateTime.Now.Year, RunNo = last });
                                        Carrier_Entities.SaveChanges();
                                        var docnoID = "FL" + DateTime.Now.Year.ToString().Substring(2, 2) + docno.Docno.Substring(4, 8 - last.ToString().Length) + last.ToString();
                                        newId = docnoID;
                                    }
                                    else
                                    {
                                        var last = docnoRun.RunNo + 1;
                                        newId = docno.Docno.Substring(0, 4) + docno.Docno.Substring(4, 8 - last.ToString().Length) + last;
                                        docnoRun.RunNo += 1;
                                        Carrier_Entities.SaveChanges();
                                    }
                                }

                                #endregion

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
                                //var vali = service_Flashs.Validate_Transport(item, ddlReceiveLocation.SelectedValue, ddlFavorites.SelectedValue);
                                //if (vali == "PASS")
                                //{
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

                                            foreach (GridViewRow rows in gv_Small_Box.Rows)
                                            {
                                                Carrier_Entities.Order_Box.Add(new Order_Box { Docno = newId, Box_ID = Convert.ToInt32(((Label)rows.FindControl("lbBox_ID")).Text), Box_Name = ((Label)rows.FindControl("lbBox_Name")).Text, Qty = Convert.ToInt32(((TextBox)rows.FindControl("txtQty")).Text) });
                                            }
                                            Carrier_Entities.SaveChanges();
                                            ClientScript.RegisterStartupScript(this.GetType(), "Success", "<script type='text/javascript'>alert('succes : " + res.success + " Tracking NO : " + res.trackingno + "');window.location='Transport_Form?Docno=" + BBID + "';</script>'");

                                            Carrier_Entities.Order_Big_Box.Add(new Order_Big_Box { BFID = BBID, Docno = newId, Status = "A" });
                                            Carrier_Entities.SaveChanges();
                                        }
                                        else
                                        {
                                            var Run = Carrier_Entities.RunDocnoes.Where(w => w.Type == "BF" && w.Year == DateTime.Now.Year).FirstOrDefault();
                                            Run.RunNo = Run.RunNo - 1;
                                            Carrier_Entities.SaveChanges();
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
                                        foreach (GridViewRow rows in gv_Small_Box.Rows)
                                        {
                                            Carrier_Entities.Order_Box.Add(new Order_Box { Docno = newId, Box_ID = Convert.ToInt32(((Label)rows.FindControl("lbBox_ID")).Text), Box_Name = ((Label)rows.FindControl("lbBox_Name")).Text, Qty = Convert.ToInt32(((TextBox)rows.FindControl("txtQty")).Text) });
                                        }
                                        Carrier_Entities.SaveChanges();
                                        ClientScript.RegisterStartupScript(this.GetType(), "Success", "<script type='text/javascript'>alert('succes : Succes ');window.location='Transport_Form?Docno=" + BBID + "';</script>'");
                                        Carrier_Entities.Order_Big_Box.Add(new Order_Big_Box { BFID = BBID, Docno = newId, Status = "A" });
                                        Carrier_Entities.SaveChanges();
                                    }




                                //}
                                //else
                                //{
                                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + vali + "')", true);
                                //}
                            }

                        }
                    }
                    
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณากรอกข้อมูลกล่องที่ต้องการส่ง จำเป็นต้องมีอย่างน้อย 1 กล่อง')", true);
                }
                
                setTableBigBoxNoHeader();
            }
            else
            {
                var item = new Order
                {
                    Docno = txtDocno.Text,
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
                foreach (GridViewRow row in gv_Big_Box.Rows)
                {
                    Label lbBBoxTracking = (Label)row.FindControl("lbBBoxTracking");
                    var resEdit = service_Flashs.EditOrder(item, lbBBoxTracking.Text, ddlFavorites.SelectedValue);
                }
                setTableBigBox();
            }
        }

        #endregion

        //คือการสร้างอีกหน้าเพื่อเรียกใช้งาน method static
        [WebMethod]
        public static List<string> AutoSearchSiteStorage(string site, string saleChannel, string workon, string BrandId)
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
            List<string> sitepro = new List<string>();
            if (BrandId != "Select")
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
                    && (BG.Contains(w.Brand) || BG.Count == 0)
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
            setTableBigBoxNoHeader();
        }

        protected void btnSearchSite_Click(object sender, EventArgs e)
        {
            getAddressOnSite(txtSiteStorage.Text.ToUpper().Substring(0, 4));
            setTableBigBoxNoHeader();
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
            setTableBigBoxNoHeader();
        }

        protected void lkSendMail_Click(object sender, EventArgs e)
        {
            div_main.Style.Add("filter", "blur(15px)");
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
            else if (RadioWork2.Checked)
            {
                on = RadioWork2.Text;
            }
            var res = service_Flashs.SendMail("apichat.f@sfg-th.com", null, "เพิ่ม SiteStorage จากระบบ Courier",
                "<HTML>" +
                "<body>" +
                "<p>SiteStorage : " + txtSiteAdd.Text + "</p>" +
                "<p>Brand : " + ddlBrand.SelectedItem.Text + "(" + ddlBrand.SelectedValue + ")" + "</p>" +
                "<p>Channel : " + on + "</p>" +
                "<p>Sale_Channel : " + ddlTo.SelectedValue + "</p>" +
                "<p>UserID : " + lbuserID.Text + "</p>"
                + "</body>"
                + "</HTML>"
                );
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('แจ้งความประสงค์ในการเพิ่ม SiteStorage ไปยังเจ้าหน้าที่เรียบร้อย')", true);
            div_main.Style.Remove("filter");
            div_main.Style.Remove("position");
            div_mail.Visible = false;
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            div_main.Style.Remove("filter");
            div_main.Style.Remove("position");
            div_mail.Visible = false;
        }

        protected void imgADD_Click(object sender, ImageClickEventArgs e)
        {
            var head = new List<newBox>();
            var box = Whale_Entities.Boxes.Where(w => w.Flag_Active == true && (w.Box_ID != 1 && w.Box_ID != 6 && w.Box_ID != 8 && w.Box_ID != 12 && w.Box_ID != 13 && w.Box_ID != 14 && w.Box_ID != 16 && w.Box_ID != 17)).ToList();
            box.Insert(0, new Box { Box_ID = 0, Box_Name = "เลือกขนาดกล่อง" });
            var Article = Carrier_Entities.Article_Category.ToList();
            Article.Insert(0, new Article_Category { ArticleCode = 1111, ArticleName = "กรุณาเลือกประเภทพัสดุ" });
            var num = 0;
            List<BoxSmallLoad> listSmallBox = new List<BoxSmallLoad>();

            foreach (GridViewRow row in gv_Big_Box.Rows)
            {
                Label lbBBoxID = (Label)row.FindControl("lbBBoxID");
                DropDownList ddlarticleCategory = (DropDownList)row.FindControl("ddlarticleCategory");
                TextBox txtremark = (TextBox)row.FindControl("txtremark");
                GridView gv_Small_Box = (GridView)row.FindControl("gv_Small_Box");
                List<BoxItem> itemOld = new List<BoxItem>();
                if (lbBBoxID.Text != "")
                {
                    foreach (GridViewRow rows in gv_Small_Box.Rows)
                    {
                        itemOld.Add(new BoxItem { Box_ID = Convert.ToInt32(((Label)rows.FindControl("lbBox_ID")).Text), Box_Name = ((Label)rows.FindControl("lbBox_Name")).Text, Qty = Convert.ToInt32(((TextBox)rows.FindControl("txtQty")).Text) });
                    }
                    listSmallBox.Add(new BoxSmallLoad { BigBox = lbBBoxID.Text, Arti_Select = ddlarticleCategory.SelectedValue, item = itemOld });
                    head.Add(new newBox { Docno = lbBBoxID.Text, Remark = txtremark.Text, Arti = Article, TypeBox = box });
                    num = Convert.ToInt32(lbBBoxID.Text);
                }

            }
            head.Add(new newBox { Docno = (num + 1).ToString(), Arti = Article, TypeBox = box });
            head.Add(new newBox { Docno = "", Arti = Article, TypeBox = box });
            gv_Big_Box.DataSource = head;
            gv_Big_Box.DataBind();

            setTableBigBoxAction(listSmallBox);
        }

        protected void imgDeleteBox_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imgDeleteBox = (ImageButton)sender;
            GridViewRow rowNow = (GridViewRow)imgDeleteBox.NamingContainer;
            GridView gv_Big_Box = (GridView)rowNow.NamingContainer;

            var box = Whale_Entities.Boxes.Where(w => w.Flag_Active == true && (w.Box_ID != 1 && w.Box_ID != 6 && w.Box_ID != 8 && w.Box_ID != 12 && w.Box_ID != 13 && w.Box_ID != 14 && w.Box_ID != 16 && w.Box_ID != 17)).ToList();
            box.Insert(0, new Box { Box_ID = 0, Box_Name = "เลือกขนาดกล่อง" });
            var Article = Carrier_Entities.Article_Category.ToList();
            Article.Insert(0, new Article_Category { ArticleCode = 1111, ArticleName = "กรุณาเลือกประเภทพัสดุ" });
            List<BoxSmallLoad> listSmallBox = new List<BoxSmallLoad>();
            var head = new List<newBox>();
            foreach (GridViewRow row in gv_Big_Box.Rows)
            {
                Label lbBBoxID = (Label)row.FindControl("lbBBoxID");
                DropDownList ddlarticleCategory = (DropDownList)row.FindControl("ddlarticleCategory");
                TextBox txtremark = (TextBox)row.FindControl("txtremark");
                GridView gv_Small_Box = (GridView)row.FindControl("gv_Small_Box");
                List<BoxItem> itemOld = new List<BoxItem>();
                if (lbBBoxID.Text != "" && row != rowNow)
                {
                    foreach (GridViewRow rows in gv_Small_Box.Rows)
                    {
                        itemOld.Add(new BoxItem { Box_ID = Convert.ToInt32(((Label)rows.FindControl("lbBox_ID")).Text), Box_Name = ((Label)rows.FindControl("lbBox_Name")).Text, Qty = Convert.ToInt32(((TextBox)rows.FindControl("txtQty")).Text) });
                    }
                    listSmallBox.Add(new BoxSmallLoad { BigBox = lbBBoxID.Text, Arti_Select = ddlarticleCategory.SelectedValue, item = itemOld });
                    head.Add(new newBox { Docno = lbBBoxID.Text, Remark = txtremark.Text, Arti = Article, TypeBox = box });
                }
                else if (lbBBoxID.Text == "" && row != rowNow)
                {
                    head.Add(new newBox { Docno = "", Remark = txtremark.Text, Arti = Article, TypeBox = box });
                }

            }
            gv_Big_Box.DataSource = head;
            gv_Big_Box.DataBind();

            setTableBigBoxAction(listSmallBox);
        }

        protected void btnPrintAll_Click(object sender, EventArgs e)
        {
            Response.Redirect("Transport_bill?Docno=" + txtDocno.Text);
        }
    }
    public class newBox
    {
        public string DTID { get; set; }
        public string Docno { get; set; }
        public string pno { get; set; }
        public string Remark { get; set; }
        public List<Article_Category> Arti { get; set; }
        public List<Box> TypeBox { get; set; }
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

    public class BoxSmallLoad
    {
        public string BigBox { get; set; }
        public string Arti_Select { get; set; }
        public List<BoxItem> item { get; set; }
    }
    #endregion
}