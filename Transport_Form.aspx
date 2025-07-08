<%@ Page Title="Courier" Language="C#" AutoEventWireup="true" CodeBehind="Transport_Form.aspx.cs" MasterPageFile="~/Site.Master" Inherits="Carrier.Transport_Form" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <style>
        .s-15px {
            font-size: 15px;
        }

        .s-22px {
            font-size: 22px;
        }

        .drop-grey {
            background-color: darkgrey;
        }
        /* Fade-In Effect */
        .visible {
            visibility: visible;
            opacity: 1;
            transition: opacity 2s linear;
        }
        /* Fade-Out Effect */
        .hidden {
            visibility: hidden;
            opacity: 0;
            transition: visibility 0s 2s, opacity 2s linear;
        }
        .radius{
            border-radius: 15px;
        }
        .status-tracking {
            border-radius: 7px;
            padding: 3px;
        }
        .fitTextArea{
            max-width: initial;
        }
        .text-Driver{
            max-width: 70%;
        }
        .fitTextBox{
                max-width: -webkit-fill-available;
        }
        .maxwide-none{
            max-width:none;
        }
    </style>
    <script type="text/javascript" src='https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3.min.js'></script>
    <script src="https://code.jquery.com/jquery-3.6.0.js"></script>
    <script src="https://code.jquery.com/ui/1.13.0/jquery-ui.js"></script>
    <script type="text/javascript">
        function pageLoad() {

            $(function () {
                $('[id*=txtSiteStorage]').on("input", function () {

                    var workOnline = $('input[id*=radioWorkOn]').is(":checked");
                    var work = "";
                    if (workOnline == true) {
                        work = $('[id*=radioWorkOn]').siblings().text();
                    } else {
                        work = $('[id*=radioWorkOff]').siblings().text();
                    }
                    var da = { site: $('[id*=txtSiteStorage]').val(), saleChannel: $('[id*=ddlReceiveLocation]').val(), workon: work, BrandId: $('[id*=ddlSDpart]').val() }
                    console.log("da :" + JSON.stringify(da));
                    $.ajax({
                        url: "Transport_Form.aspx/AutoSearchSiteStorage",
                        data: JSON.stringify(da),
                        dataType: "json",
                        method: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataFilter: function (data) { return data; },
                        success: function (data) {
                            console.log(JSON.stringify(data));

                            $("#<% = txtSiteStorage.ClientID%>").autocomplete({
                                source: function (request, response) {
                                    response($.map(data.d, function (item) {
                                        return {
                                            label: item,
                                            val: item,
                                        }
                                    }))
                                }
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            var err = eval("(" + XMLHttpRequest.responseText + ")");
                            console.log("Ajax Error! :" + err.Message);
                        }
                    });

                });
            });
        };

    </script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
    <asp:Label runat="server" ID="lbuserID" Visible="false"></asp:Label>
    <asp:UpdatePanel ID="updatePanel1" runat="server">
        <ContentTemplate>
            <div runat="server" id="div_main">
                <div style="margin-top: 80px;">
                    <%--<div style="position: absolute; left: 50%;" class="float-end">
                        <asp:Button runat="server" ID="btnPrint" Text="พิมพ์ใบปะหน้ากล่อง" Visible="false" CssClass="btn btn-primary" OnClick="btnPrint_Click" UseSubmitBehavior="false" />
                    </div>--%>
                    <div style="position: absolute; width: 80%; text-align: -webkit-right;" class="float-end">
                        <div class="row" style="width: 50%;">
                            <div class="col-md-7 col-sm-6" style="top: 14px;" runat="server" id="dv_deliveryID" visible="false">
                               
                                <div class="input-group">
                                    
                                    <asp:Label runat="server" ID="lbDeliveryOrder" Text="Delivery Order:" CssClass="input-group-text"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtDeliveryOrder" CssClass="form-control"></asp:TextBox>
                                </div>
                                
                            </div>
                            <div class="col-md-2 col-sm-6">
                                <asp:ImageButton runat="server" ID="btnNotiLalamove" OnClick="btnNotiLalamove_Click1" Width="60px"  CssClass="img-Noti" ImageUrl="~/Icon/fast-delivery.png" Visible="false"/>
                            </div>
                            <div class="col-md-3 col-sm-6 align-self-center" >
                                <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-primary " Text="กลับหน้าหลัก" OnClick="btnCancel_Click" UseSubmitBehavior="false" />
                            </div>
                        </div>
                        
                        
                    </div>
                    <asp:Label runat="server" ID="lbForm" Text="Transportation" CssClass="h1"></asp:Label>
                </div>
                <div class="row col mt-5"  runat="server" id="dv_Ifram" visible="false">
                    <div style="pointer-events:none;">
                        <iframe runat="server" id="iframeTracking" width="100%" height="590px" ></iframe>
                    </div>
                    
                    <br />
                    <div class="input-group">
                        <span class="input-group-text">Shared Link</span>
                        <asp:TextBox runat="server" ID="txtSharedLink" CssClass="form-control maxwide-none"   Enabled="false" ></asp:TextBox>
                    </div>
                </div>
                <div class="row mt-5 mb-3">
                    <%-- Columns ซ้าย--%>
                    <div class="col-sm-12 col-md-6">
                        <div class="row " style="margin-bottom: 10px; display: none;">
                            <div class="col-sm-2 w-100 input-group mb-2 ">
                                <asp:Label runat="server" ID="lbWareHouseNo" Text="รหัสคลังสินค้า" CssClass=" input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtWareHouse" Width="100%" CssClass="form-control s-15px shadow"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbDocno" Text="เลขที่เอกสาร" CssClass=" input-group-text s-15px shadow" Visible="false"></asp:Label>
                                <asp:TextBox runat="server" ID="txtDocno" Width="100%" CssClass="form-control s-15px shadow" Enabled="false" Visible="false"></asp:TextBox>
                            </div>
                        </div>
                        <%--<div class="row ">
                            <div class="col-sm-2 w-100 input-group mb-2" runat="server">
                                <asp:Label runat="server" ID="lbTrackingID" Text="เลขพัสดุ" CssClass=" input-group-text s-15px shadow" Visible="false"></asp:Label>
                                <asp:TextBox runat="server" ID="txtTrackingID" Width="100%" CssClass="form-control s-15px shadow" Enabled="false" Visible="false"></asp:TextBox>
                            </div>
                        </div>--%>
                        <div class="row" runat="server" id="dv_Approve_name" visible="false">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbApprove" Text="ผู้อนุมัติการเรียกรถ" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtApprove"  Width="100%" CssClass="form-control s-15px shadow" Enabled="false"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row col-sm-12 col-md-6" runat="server" id="div_TypeSend" visible="false">
                            <div class=" input-group mb-2 ">
                                <asp:Label runat="server" ID="lbTypeSend" Text="ประเภทการส่ง" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlTypeSend" CssClass="btn dropdown-toggle s-15px shadow">
                                    <asp:ListItem Text="ส่ง" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="รับ" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row col-sm-12 col-md-6" runat="server" id="divLalaCar">
                            <div class=" mb-2 ">
                                <asp:Label runat="server" ID="Label3" Text="ประเภทรถ" CssClass=" s-15px "></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlCar" CssClass="btn border-1 dropdown-toggle s-15px shadow" OnSelectedIndexChanged="ddlCar_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="เลือกรถเพื่อมารับของ" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="รถมอเตอร์ไซต์" Value="MOTORCYCLE"></asp:ListItem>
                                    <asp:ListItem Text="รถกระบะตู้ทึบ" Value="TRUCK330"></asp:ListItem>
                                    <asp:ListItem Text="รถเก๋ง 4 ประตู" Value="CAR"></asp:ListItem>
                                    <asp:ListItem Text="รถ 5 ประตู" Value="MPV"></asp:ListItem>
                                    <asp:ListItem Text="รถกระบะ" Value="PICK_UP_TRUCK"></asp:ListItem>

                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row col-sm-12 col-md-6" runat="server" id="divSpecial" visible="false">
                            <div class=" mb-2 ">
                                <div class="row mt-2">
                                    <asp:Label runat="server" ID="Label6" Text="บริการพิเศษช่วยขนของ" CssClass=" s-15px "></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlSpecial" CssClass="btn border-1 dropdown-toggle s-15px shadow text-left" DataTextField="SpecialRequest_Name_TH" DataValueField="SpecialRequests_Name">
                                </asp:DropDownList>

                                </div>
                                <br />
                                <div class="row" runat="server" id="dv_rount">
                                    <label for="ckRount">บริการพิเศษอื่นๆ</label>
                                    <asp:CheckBox runat="server" ID="ckRount" Text="ไป - กลับ" CssClass="" />
                                </div>


                            </div>
                        </div>
                        <div class="col-10 m-2">
                            <div class="row border" runat="server" id="dv_DetailCar" visible="false">
                                <div class="col-md-5 col-sm-5">
                                    <asp:Image runat="server" ID="imgCar" Width="100%" />
                                </div>
                                <div class="col-md-7 col-sm-7">
                                    <asp:Label runat="server" ID="lbCarSize" CssClass="form-text"></asp:Label>
                                    <br />
                                    <asp:Label runat="server" ID="lbCarDetail" CssClass="form-text"></asp:Label>
                                    <br />
                                    <asp:Label runat="server" ID="lbCarLoad" CssClass="form-text"></asp:Label>
                                </div>

                            </div>

                        </div>
                        <asp:Label runat="server" ID="lbgroupSend" Text="ผู้ส่ง" CssClass="s-22px "></asp:Label>
                        <div class="border border-1 " style="border-radius: 15px; padding: 20px; width: 90%">
                            <div class="row col-sm-12 my-2">
                                <div class=" input-group">
                                    <asp:Label runat="server" ID="lbFavorites" Text="Favorites" CssClass="input-group-text s-15px shadow" Visible="false"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlFavorites" CssClass="btn text-start  dropdown-toggle s-15px shadow" Visible="false" DataValueField="val" DataTextField="text" OnSelectedIndexChanged="ddlFavorites_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row col-sm-12 my-2">
                                <div class=" input-group ">
                                    <asp:Label runat="server" ID="lbsrcName" Text="ชื่อ" CssClass=" input-group-text s-15px shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtsrcName" CssClass="form-control s-15px shadow fitTextBox" AutoCompleteType="Disabled"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row col-sm-12 my-2">
                                <div class="input-group ">
                                    <asp:Label runat="server" ID="lbsrcPhone" Text="เบอร์โทรศัพท์" CssClass=" input-group-text s-15px shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtsrcPhone" CssClass="form-control s-15px shadow " MaxLength="10" TextMode="Phone" AutoCompleteType="Disabled"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtsrcPhone" ErrorMessage="กรุณากรอกเบอร์โทรศัพท์ให้ถูกต้อง" ValidationExpression="[0]{1}(([6,8,9]{1}[0-9]{8})|([2]{1}[0-9]{7}))" Display="Dynamic" ForeColor="Red" SetFocusOnError="true"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="row col-sm-12 my-2">
                                <div class=" input-group">
                                    <asp:Label runat="server" ID="lbsrcProvinceName" Text="จังหวัด" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlsrcProvinceName" CssClass="btn text-start  dropdown-toggle s-15px shadow" DataValueField="Province_ID" DataTextField="Province_Name" OnSelectedIndexChanged="ddlsrcProvinceName_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row col-sm-12 my-2">
                                <div class="input-group ">
                                    <asp:Label runat="server" ID="lbsrcCityName" Text="อำเภอ" CssClass=" input-group-text s-15px shadow"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlsrcCityName" CssClass="btn  text-start dropdown-toggle s-15px shadow" DataValueField="City_ID" DataTextField="City_Name" OnSelectedIndexChanged="ddlsrcCityName_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row col-sm-12 my-2">
                                <div class=" input-group w-100">
                                    <asp:Label runat="server" ID="lbsrcDistrictName" Text="ตำบล" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlsrcDistrictName" CssClass="btn text-start  dropdown-toggle s-15px shadow" DataValueField="Distinct_ID" DataTextField="Distinct_Name" OnSelectedIndexChanged="ddlsrcDistrictName_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row col-sm-12 my-2">
                                <div class=" input-group w-100">
                                    <asp:Label runat="server" ID="lbsrcPostalCode" Text="รหัสไปรษณีย์" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtsrcPostalCode" CssClass="form-control s-15px shadow" MaxLength="5" AutoCompleteType="Disabled"></asp:TextBox>
                                </div>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtsrcPostalCode" ErrorMessage="กรุณากรอกรหัสไปรษณีย์ให้ถูกต้อง" ValidationExpression="[0-9]{5}" Display="Dynamic" ForeColor="Red" SetFocusOnError="true"></asp:RegularExpressionValidator>
                                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtsrcPostalCode" Display="Static" ErrorMessage="*" runat="server" ForeColor="Red" CssClass="ml-1" />--%>
                            </div>
                            <div class="row col-sm-12 my-2">
                                <div class=" input-group w-100">
                                    <asp:Label runat="server" ID="lbsrcDetailAddress" Text="รายละเอียดที่อยู่" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtsrcDetailAddress" CssClass="form-control s-15px shadow" AutoCompleteType="Disabled" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <!--<div class="col-sm-12 my-2">
                            <div class="input-group">
                                <asp:Label runat="server" ID="lbarticleCategory" Text="ประเภทสินค้า" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlarticleCategory" CssClass="btn dropdown-toggle shadow" DataTextField="ArticleName" DataValueField="ArticleCode"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-sm-12 my-2">
                            <div class="input-group w-100">
                                <asp:Label runat="server" ID="lbremark" Text="หมายเหตุ" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtremark" CssClass="form-control s-15px shadow" TextMode="MultiLine" Rows="3"></asp:TextBox>                                
                            </div>
                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtremark" ErrorMessage="ห้ามใช้เครื่องหมาย + " ValidationExpression="" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>--%>
                        </div>-->


                    </div>
                    <%--Columns ขวา--%>
                    <div class="col-sm-12 col-md-6">

                        <div runat="server" id="dv_Driver_Lalamove" visible="false">
                            <span class="s-22px">พนักงานขับรถ</span>
                            <div class="row border border-1 " style="border-radius: 15px; padding: 20px; width: 100%" runat="server">
                                <div class="col-3">
                                    <asp:Image runat="server" ID="imgDriver" Width="100%" />
                                </div>
                                <div class="col-9" style="display:inline-grid;">
                                    <span style="font-weight: bold;">ID:</span><asp:Label runat="server" ID="lbDriverID"></asp:Label>
                                    <span style="font-weight: bold;">ชื่อ:</span><asp:Label runat="server" ID="lbDriverName"></asp:Label>
                                    <span style="font-weight: bold;">เบอร์โทร:</span><asp:Label runat="server" ID="lbDriverPhone"></asp:Label>
                                    <span style="font-weight: bold;">ป้ายทะเบียน:</span><asp:Label runat="server" ID="lbDriverPlateNumber"></asp:Label>

                                </div>
                            </div>
                        </div>

                        <div class="row col-sm-12 my-2" style="margin-bottom: 10px;">
                            <div class="input-group ">
                                <asp:Label runat="server" ID="lbExpress" Text="รูปแบบการจัดส่ง" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlExpress" CssClass="btn  text-start dropdown-toggle s-15px shadow" DataTextField="text" DataValueField="val" OnSelectedIndexChanged="ddlExpress_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                <%--<asp:TextBox runat="server" Text="Flash Express" Enabled="false" CssClass="form-control s-15px shadow text-center"></asp:TextBox>--%>
                            </div>
                        </div>
                        <div class="row col-sm-12 my-2" style="margin-bottom: 10px;">
                            <div class=" input-group">
                                <asp:Label runat="server" ID="Label1" Text="รูปแบบการขายสินค้า" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:RadioButton runat="server" ID="radioWorkOn" Text="ONLINE" GroupName="SaleOn" CssClass="ml-4 mt-2 custom-radio" />
                                <asp:RadioButton runat="server" ID="radioWorkOff" Text="OFFLINE" GroupName="SaleOn" CssClass="ml-4 mt-2 custom-radio " />
                            </div>
                        </div>
                        
                        
                        <asp:Label runat="server" ID="Label5" Text="ค้นหาที่อยู่ผู้รับจาก Sitestorage" CssClass="s-22px"></asp:Label>
                        <div class="border border-1 " runat="server" id="divSite" style="border-radius: 15px; padding: 20px; width: 100%; margin-bottom: 10px; flex-wrap: nowrap;">
                            <div class="row col-sm-12 my-2" runat="server" id="dv_BrandOld" >
                                <div class="input-group">
                                    <asp:Label runat="server" ID="Label4" Text="แผนก" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:DropDownList ID="ddlSDpart" runat="server" DataTextField="department_" DataValueField="departmentID" CssClass="btn text-start s-15px shadow mw-100" ></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row col-sm-12 my-2">
                                <div class="col-8  input-group mb-2 " style="width: 60%">
                                    <asp:Label runat="server" ID="lbSite" Text="Site Storage" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtSiteStorage" CssClass="form-control s-15px shadow text-uppercase " MaxLength="14" OnTextChanged="txtSiteStorage_TextChanged" AutoCompleteType="Disabled" ToolTip="กรุณากรอก SiteStorage ถ้าไม่มีให้ใส่ CENTER"></asp:TextBox>

                                </div>
                                <div class="col-4 input-group h-50" style="width: 40%">
                                    <asp:Button runat="server" ID="btnSearchSite" CssClass="btn btn-primary input-group-text shadow" Text="Search" UseSubmitBehavior="false" OnClick="btnSearchSite_Click" />
                                    <asp:Button runat="server" ID="btnClearSite" CssClass="btn btn-danger input-group-text shadow" Text="Clear" UseSubmitBehavior="false" OnClick="btnClearSite_Click" />
                                </div>
                            </div>

                        </div>


                        <asp:Label runat="server" ID="Label2" Text="ผู้รับ" CssClass="s-22px"></asp:Label>
                        <div class="border border-1 " style="border-radius: 15px; padding: 20px; width: 100%">
                            <div class="row col-sm-12 my-2">
                            <div class="input-group" runat="server" id="div_Receive">
                                <asp:Label runat="server" ID="lbReceiveLocation" Text="ปลายทาง" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlReceiveLocation" CssClass="btn text-start dropdown-toggle s-15px shadow" DataValueField="val" DataTextField="text" OnSelectedIndexChanged="ddlReceiveLocation_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            </div>
                        </div>
                            <div class="row col-sm-12 my-2">
                                <div class="input-group">
                                    <asp:Label runat="server" ID="lbdstName" Text="ชื่อ" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtdstName" CssClass="form-control s-15px shadow fitTextBox" AutoCompleteType="Disabled"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtdstName" Display="Static" ErrorMessage="*" runat="server" ForeColor="Red" CssClass="ml-1" />--%>
                                </div>
                            </div>
                            <div class="row  col-sm-12 my-2">
                                <div class="input-group">
                                    <asp:Label runat="server" ID="lbdstPhone" Text="เบอร์โทรศัพท์มือถือ" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtdstPhone" CssClass="form-control s-15px shadow" MaxLength="12" AutoCompleteType="Disabled"></asp:TextBox>
                                </div>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtdstPhone" ErrorMessage="กรุณากรอกเบอร์โทรศัพท์ให้ถูกต้อง" ValidationExpression="([0]{1}[4,6,8,9,5]{1}[0-9]\w{7,9})|([-]{1})" Display="Dynamic" ForeColor="Red" SetFocusOnError="true"></asp:RegularExpressionValidator>
                            </div>
                            <div class="row  col-sm-12 my-2">
                                <div class="input-group">
                                    <asp:Label runat="server" ID="lbdstHomePhone" Text="เบอร์โทรศัพท์บ้าน" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtdstHomePhone" CssClass="form-control s-15px shadow" MaxLength="9" AutoCompleteType="Disabled"></asp:TextBox>
                                </div>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtdstHomePhone" ErrorMessage="กรุณากรอกเบอร์โทรศัพท์บ้านให้ถูกต้อง" ValidationExpression="([0]{1}[2-7]{1}[0-9]{7})|([-,]{1})" Display="Dynamic" ForeColor="Red" SetFocusOnError="true"></asp:RegularExpressionValidator>
                            </div>
                            <div class="row  col-sm-12 my-2">
                                <div class="input-group ">
                                    <asp:Label runat="server" ID="lbdstProvinceName" Text="จังหวัด " CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddldstProvinceName" CssClass="btn text-start  dropdown-toggle s-15px shadow" DataValueField="Province_ID" DataTextField="Province_Name" OnSelectedIndexChanged="ddldstProvinceName_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row  col-sm-12 my-2">
                                <div class=" input-group">
                                    <asp:Label runat="server" ID="lbdstCityName" Text="อำเภอ" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddldstCityName" CssClass="btn  text-start dropdown-toggle s-15px shadow" DataValueField="City_ID" DataTextField="City_Name" OnSelectedIndexChanged="ddldstCityName_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row  col-sm-12 my-2">
                                <div class="input-group">
                                    <asp:Label runat="server" ID="lbdstDistrictName" Text="ตำบล" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddldstDistrictName" CssClass="btn text-start  dropdown-toggle s-15px shadow" DataValueField="Distinct_ID" DataTextField="Distinct_Name" OnSelectedIndexChanged="ddldstDistrictName_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row  col-sm-12 my-2">
                                <div class="input-group">
                                    <asp:Label runat="server" ID="lbdstPostalCode" Text="รหัสไปรษณีย์" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtdstPostalCode" CssClass="form-control s-15px shadow" AutoCompleteType="Disabled"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="txtdstPostalCode" ErrorMessage="กรุณากรอกรหัสไปรษณีย์ให้ถูกต้อง" ValidationExpression="[0-9]{5}" Display="Dynamic" ForeColor="Red" SetFocusOnError="true"></asp:RegularExpressionValidator>
                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator6" ControlToValidate="txtdstPostalCode" Display="Static" ErrorMessage="*" runat="server" ForeColor="Red" CssClass="ml-1"  />--%>
                                </div>
                            </div>
                            <div class="row col-sm-12 my-2">
                                <div class=" input-group">
                                    <asp:Label runat="server" ID="lbdstDetailAddress" Text="รายละเอียดที่อยู่" CssClass="input-group-text s-15px shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtdstDetailAddress" CssClass="form-control s-15px shadow" AutoCompleteType="Disabled" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtdstDetailAddress" ErrorMessage="ที่อยู่ไม่ควรมีเครื่องหมาย + *" ValidationExpression="[\w\s-/(),]*" Display="Dynamic" ForeColor="Red" SetFocusOnError="true"></asp:RegularExpressionValidator>--%>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <%--<iframe
                    width="600"
                    height="450"
                    style="border: 0"
                    loading="lazy"
                    allowfullscreen
                    referrerpolicy="no-referrer-when-downgrade"
                    src="https://www.google.com/maps/embed/v1/place?key=AIzaSyC3oAoBojE55Dsle4J-vN4HrFNBPcFHSow&q=14.1833853,100.6259676"
                    runat="server" id="iframe1"></iframe>--%>
                <div class="row my-2" runat="server" id="dv_Address_Google" visible="false">
                    <span class="text-center">โปรดเลือกที่อยู่ที่ถูกต้อง เพื่อความแม่นยำต่อการจัดส่ง</span>
                    <div class="row">
                        <div class="col m-2">
                            <span>ที่อยู่ผู้ส่ง : </span>
                            <asp:Label runat="server" ID="lbSRCAddress_Detail_Google" Visible="false"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlSRC" CssClass="btn shadow" DataTextField="AddressDetail" DataValueField="No" OnSelectedIndexChanged="ddlSRC_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            <asp:Label runat="server" ID="lbSRCCount" CssClass="mb-2"></asp:Label>
                            <br />
                            <iframe
                                width="600"
                                height="450"
                                style="border: 0"
                                loading="lazy"
                                allowfullscreen
                                referrerpolicy="no-referrer-when-downgrade"
                                
                                runat="server" id="iframe_SRC"></iframe>
                        </div>
                        <div class="col m-2">
                            <span>ที่อยู่ผู้รับ : </span>
                            <asp:Label runat="server" ID="lbDSTAddress_Detail_Google" Visible="false"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlDST" CssClass="btn shadow" DataTextField="AddressDetail" DataValueField="No" OnSelectedIndexChanged="ddlDST_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            <asp:Label runat="server" ID="lbDSTCount" CssClass="mb-2"></asp:Label>
                            <br />
                            <iframe
                                width="600"
                                height="450"
                                style="border: 0"
                                loading="lazy"
                                allowfullscreen
                                referrerpolicy="no-referrer-when-downgrade"
                                
                                runat="server" id="iframe_DST"></iframe>
                        </div>

                    </div>
                    
                    <div style="text-align-last: center;" class="my-2">
                        <asp:Button runat="server" ID="btnClearLocation_Google" CssClass="btn btn-warning" Text="แก้ไขที่อยู่" OnClick="btnClearLocation_Google_Click" UseSubmitBehavior="false" />
                    </div>
                </div>
                <div runat="server" id="dv_RemarkDriver" visible="false" class="mb-4">
                    <div class="row text-center" >
                        <h3>
                            รายละเอียดให้พนักงานขับรถ
                        </h3>
                    </div>
                    <div class="row text-center"  >
                        <div class="input-group" style="place-content: center;">
                            <asp:TextBox runat="server" ID="txtRemarkDriver" CssClass="form-control  s-15px shadow fitTextArea text-Driver"  Rows="2"  AutoCompleteType="Disabled" TextMode="MultiLine" Width="100%" ToolTip="กรอกรายละเอียดให้พนักงานขับรถ"></asp:TextBox>
                        </div>
                    </div>
                </div>
                
                <div class="row text-center">
                    <h3>
                        กล่องที่ต้องการส่ง
                    </h3>
                    <br />
                    <span style="color:red">*1 รายการ : 1 กล่อง*</span>
                    <div class="row  justify-content-center my-2"  runat="server" id="divPrintAll" visible="false">
                        <asp:Button runat="server" ID="btnPrintAll" Text="พิมพ์ใบปะหน้ากล่องทั้งหมด" OnClick="btnPrintAll_Click" CssClass="btn btn-primary w-50"/>
                    </div>
                </div>
                <div class="row mb-2 ml-2">
                            <asp:Label runat="server" ID="lbGuidSiteStorage" Text=" - กรุณากรอก SiteStorage ถ้าไม่มีให้ใส่ CENTER" Font-Names="Comic Sans MS" Font-Size="Small" ForeColor="Red" Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lbGuidSiteStorage2" Text=" - SiteStorage CENTER สามารถใส่ได้เฉพาะของกองหลังเท่านั้น" Font-Names="Comic Sans MS" Font-Size="Small" ForeColor="Red"></asp:Label>
                        </div>
                <div class="row">
                    <asp:GridView runat="server" ID="gv_Big_Box" EmptyDataText="No Reccord." CssClass="table table-hover  table-bordered small" Width="100%" AutoGenerateColumns="false" BorderStyle="None" RowStyle-BorderColor="Gray" RowStyle-CssClass="" >
                        <Columns>
                            <asp:TemplateField  ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnPDF" Text="พิมพ์ใบปะหน้ากล่อง" Visible="false" OnClick="btnPrint_Click"  />
                                    <asp:ImageButton runat="server" ID="imgADD" ImageUrl="~/Icon/add.png" Visible="false"  Width="30px" ToolTip="เพิ่มกล่องใหญ่" OnClick="imgADD_Click"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField  ItemStyle-VerticalAlign="Top">
                                <HeaderTemplate>
                                    <span>เลขที่กล่อง</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="input-group mt-2">
                                    <%--<asp:Label runat="server" ID="lbhBBoxID" CssClass="input-group-text" Text ="เลขที่กล่อง : "></asp:Label>--%>
                                    <asp:Label runat="server" ID="lbBBoxID" CssClass="input-group-text" Text='<%# Bind("Docno") %>'></asp:Label>
                                        </div>
                                </ItemTemplate>
                                </asp:TemplateField>
                            <asp:TemplateField  ItemStyle-VerticalAlign="Top">
                                <HeaderTemplate>
                                    <span>
                                        เลขพัสดุ
                                    </span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="input-group mt-2">
                                    <%--<asp:Label runat="server" ID="lbhBBoxTracking" CssClass="input-group-text" Text="เลขพัสดุ : "></asp:Label>--%>
                                    <asp:Label runat="server" ID="lbBBoxTracking" CssClass="input-group-text"></asp:Label>
                                        </div>
                                </ItemTemplate>
                                </asp:TemplateField>
                            <asp:TemplateField  ItemStyle-VerticalAlign="Top">
                                <HeaderTemplate>
                                    <span>
                                        Brand & SiteStorage
                                    </span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class=" mt-2">
                                        <div class="row my-2">
                                            <div class="input-group">
                                                <span class="input-group-text">เบิกเพื่อใช้ในแผนก</span>
                                                <asp:DropDownList runat="server" ID="ddlDepartment" CssClass="btn dropdown-toggle shadow text-left" DataTextField="department_" DataValueField="departmentID"></asp:DropDownList>
                                                <asp:Label runat="server" ID="lbDepartment_id" Visible="false" Text='<%# Bind("department") %>' ></asp:Label>
                                            </div>
                                        </div>
                                        <div class="row my-2">
                                            <div class="input-group">
                                                <span class="input-group-text">SiteStorage</span>
                                                <asp:TextBox runat="server" ID="txtSitestorageBox" Text='<%# Bind("Sitestorage") %>' CssClass="form-control s-15px shadow text-uppercase" MaxLength="14" AutoCompleteType="Disabled" ToolTip="กรุณากรอก SiteStorage ถ้าไม่มีให้ใส่ CENTER"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                </asp:TemplateField>
                            <asp:TemplateField ItemStyle-VerticalAlign="Top" >
                                <HeaderTemplate>
                                    <span>
                                        ประเภทสินค้า
                                    </span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="input-group mt-2">
                                    <asp:Label runat="server" ID="lbhArticleCategory" Text="ประเภทสินค้า : " CssClass="input-group-text shadow"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlarticleCategory" DataSource='<%# Bind("Arti") %>' CssClass="btn dropdown-toggle shadow" BackColor="White" DataTextField="ArticleName" DataValueField="ArticleCode" Width="100%" ></asp:DropDownList>
                                    
                                        </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-VerticalAlign="Top" >
                                <HeaderTemplate>
                                    หมายเหตุ
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="input-group mt-2">
                                    <asp:Label runat="server" ID="lbhRemark" Text="หมายเหตุ" CssClass="input-group-text shadow"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtremark" TextMode="MultiLine" Rows="3" CssClass="form-control shadow" Text='<%# Bind("Remark") %>'></asp:TextBox>
                                        </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-VerticalAlign="Top">
                                <HeaderTemplate>
                                        ภายในกล่อง
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <div class="row col-sm-12 input-group my-2">
                                            <asp:Label runat="server" ID="lbhBox" Text="ประเภทกล่อง" Width="40%" CssClass="input-group-text s-15px shadow"></asp:Label>
                                            <asp:DropDownList runat="server" ID="ddlBox" DataSource='<%# Bind("TypeBox") %>' Width="60%" CssClass="btn dropdown-toggle s-15px shadow" BackColor="White" DataTextField="Box_Name" DataValueField="Box_ID"></asp:DropDownList>
                                        </div>
                                        <div class="row col-sm-12 input-group ">
                                            <asp:Label runat="server" ID="lbInputQty" Text="จำนวน" Width="40%" CssClass=" input-group-text s-15px shadow"></asp:Label>
                                            <asp:TextBox runat="server" ID="txtQty" Width="60%" CssClass="form-control shadow text-end" TextMode="Number" ValidateRequestMode="Enabled"></asp:TextBox>
                                        </div>

                                        <div class=" gj-text-align-center mt-1">
                                            <asp:Button runat="server" ID="btnAdd" Text="ADD" Width="100%" Height="46px" CssClass="text-center btn btn-success s-15px" OnClick="btnAdd_Click" />
                                        </div>
                                        
                                    <br />
                                    <asp:GridView runat="server" ID="gv_Small_Box" CssClass="table table-striped table-bordered table-hover small bg-gradient " AutoGenerateColumns="false" >
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label runat="server" ID="lbhBox_Name" Text="ขนาดกล่อง"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lbBox_ID" Text='<%# Bind("Box_ID") %>' Visible="false"></asp:Label>
                                                    <asp:Label runat="server" ID="lbBox_Name" Text='<%# Bind("Box_Name") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle BackColor="White" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label runat="server" ID="lbhQty" Text="จำนวน"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lbQty" Text='<%# Bind("Qty") %>' Visible="false"></asp:Label>
                                                    <asp:TextBox runat="server" ID="txtQty" Text='<%# Bind("Qty") %>' CssClass="form-control text-end" Width="50px"></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle CssClass="text-end" BackColor="White" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Button runat="server" ID="btnDeleteBoxItem" OnClick="btnDeleteBoxItem_Click" CssClass="btn-close" />
                                                </ItemTemplate>
                                                <ItemStyle BackColor="White" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemStyle CssClass="justify-content-center"/>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbStatuspno" Visible="false" ></asp:Label>
                                    <asp:ImageButton runat="server" ID="imgDeleteBox" ImageUrl="~/Icon/x-button.png" Width="30px" OnClick="imgDeleteBox_Click"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                           
                        </Columns>
                    </asp:GridView>
                </div>

                <%--<div runat="server" id="div_Box" class="row mt-3">
                    <div class="col-md-4 col-sm-12">
                        <div class="row col-sm-12 input-group my-2">
                            <asp:Label runat="server" ID="Label3" Text="ประเภทกล่อง" Width="40%" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlBox" Width="60%" CssClass="btn dropdown-toggle s-15px shadow" DataTextField="Box_Name" DataValueField="Box_ID"></asp:DropDownList>
                        </div>
                        <div class="row col-sm-12 input-group mb-2">
                            <asp:Label runat="server" ID="Label5" Text="จำนวน" Width="40%" CssClass=" input-group-text s-15px shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtQty" Width="60%" CssClass="form-control shadow text-end" TextMode="Number" ValidateRequestMode="Enabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-2 col-sm-12 gj-text-align-center my-3">
                        <asp:Button runat="server" ID="btnAdd" Text="ADD" Width="100%" Height="85px" CssClass="text-center btn btn-success s-15px" OnClick="btnAdd_Click" />
                    </div>
                    <div class="col-md-4 col-sm-12">
                        <div style="display: flex; justify-content: center;" runat="server" id="div_tb_Box">
                            <asp:GridView runat="server" ID="gv_Box" CssClass="table table-striped table-bordered table-hover small bg-gradient " AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:Label runat="server" ID="lbhBox_Name" Text="ขนาดกล่อง"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lbBox_ID" Text='<%# Bind("Box_ID") %>' Visible="false"></asp:Label>
                                            <asp:Label runat="server" ID="lbBox_Name" Text='<%# Bind("Box_Name") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:Label runat="server" ID="lbhQty" Text="จำนวน"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lbQty" Text='<%# Bind("Qty") %>' Visible="false"></asp:Label>
                                            <asp:TextBox runat="server" ID="txtQty" Text='<%# Bind("Qty") %>' CssClass="text-end" Width="50px"></asp:TextBox>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-end" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button runat="server" ID="btnDeleteBoxItem" OnClick="btnDeleteBoxItem_Click" CssClass="btn-close" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>--%>
                
                

                <div class="row">

                    <div style="display: flex; justify-content: center;" class="mt-3">
                        <asp:Button runat="server" ID="btnCheck" Text="Check" CssClass="text-center btn btn-warning s-15px" Width="100%" OnClick="btnCheck_Click"  Visible="false"/>
                        <asp:Button runat="server" ID="btnSave" Text="SAVE" CssClass="text-center btn btn-success s-15px" Width="100%" OnClick="btnSave_Click" Visible="false" />
                    </div>
                    <div style="display: none;">
                        <div class="col-sm-2 input-group mb-2 w-25">
                            <asp:Label runat="server" ID="lbweight" Text="น้ำหนัก (กรัม)" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtweight" CssClass="form-control s-15px shadow" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="col-sm-2 input-group mb-2 w-25">
                            <asp:Label runat="server" ID="lbwidth" Text="ความกว้าง (เซนติเมตร)" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtwidth" CssClass="form-control s-15px shadow" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="col-sm-2 input-group mb-2 w-25">
                            <asp:Label runat="server" ID="lblength" Text="ความยาว (เซนติเมตร)" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtlength" CssClass="form-control s-15px shadow" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="col-sm-2 input-group mb-2 w-25">
                            <asp:Label runat="server" ID="lbheight" Text="ความสูง (เซนติเมตร)" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtheight" CssClass="form-control s-15px shadow" Enabled="false"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSave" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
