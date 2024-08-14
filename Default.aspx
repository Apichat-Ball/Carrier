<%@ Page Title="Courier" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Carrier._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        @media (min-width: 576px) {
            .col-sm-12 {
                flex: 0 0 auto;
                width: 100%;
                margin-top: 10px;
            }
        }

        @media screen and (device-width: 320px) and (device-width: 768px) {
            .col-6 {
                max-width: 90%;
            }
        }

        @media screen and (device-width: 1440px) {
            .col-6 {
                max-width: 45%;
            }
        }

        .s-15px {
            font-size: 15px;
        }

        .img-mt--12px {
            margin-top: -26px;
            margin-left: -52px;
            margin-right: 64px;
        }

        .mg-Lala {
            margin: 0px 0px 0px 26px;
        }

        .status-tracking {
            border-radius: 7px;
            padding: 3px;
        }

        .w-7 {
            width: 7%;
        }

        .w-10 {
            width: 10%;
        }

        .rainbow {
            text-align: center;
            font-size: 22px;
            font-family: LilyUPC;
            /*            font-family: monospace;*/
            /*            letter-spacing: 1px;*/
            animation: colorRotate 2s linear 0s infinite;
        }
        .maxwidthnone {
            max-width: none;
        }

        .ddlwidthFull {
            width: -webkit-fill-available;
        }
        @keyframes colorRotate {
            from {
                color: #6666ff;
            }

            10% {
                color: #0099ff;
            }

            50% {
                color: #00ff00;
            }

            75% {
                color: #ff3399;
            }

            100% {
                color: #6666ff;
            }
        }
    </style>

    <style>
        .reset {
            all: revert;
            border-radius: 10px;
            border-color: lightgray;
        }

        .imgX {
            background-image: url("ImgFurniture/x-button.png");
        }

        .border-Dropdrawn {
            border-color: darkgray;
        }

        .vertical-align-middle {
            vertical-align: middle;
            text-align: center;
        }

        .modal-dialog-scrollable .modal-content {
            max-height: 90%;
            overflow: hidden
        }

        .font-HeadDoc-Create {
            font-size: 30px;
        }

        @media (min-width:0px) {
            .w-img {
                width: 60%;
            }

            .maxW50 {
                max-width: 90%;
            }

            .maxW60 {
                max-width: 90%;
            }
        }

        @media (min-width:576px) {
            .w-img {
                width: 50%;
            }

            .maxW50 {
                max-width: 95%;
            }

            .maxW60 {
                max-width: 95%;
            }
        }

        @media (min-width:768px) {
            .w-img {
                width: 40%;
            }

            .maxW50 {
                max-width: 50%;
                transform: translate(50%);
            }

            .maxW60 {
                max-width: 70%;
                transform: translate(15%);
            }
            /*            .div_modal{
                transform: translate(15%);
            }*/
        }

        @media (min-width:992px) {
            .w-img {
                width: 30%;
            }

            .maxW50 {
                max-width: 50%;
                transform: translate(50%);
            }

            .maxW60 {
                max-width: 70%;
                transform: translate(15%);
            }
            /*            .div_modal{
                transform: translate(15%);
            }*/
        }

        @media (min-width:1200px) {
            .w-img {
                width: 30%;
            }

            .maxW50 {
                max-width: 50%;
                transform: translate(50%);
            }

            .maxW60 {
                max-width: 70%;
                transform: translate(15%);
            }
            /*            .div_modal {
                transform: translate(15%);
            }*/
        }

        .Status-Success {
            padding: 0px 5px 0px 5px;
            background-color: lawngreen;
            border-radius: 8px;
        }

        .Status-Rent {
            padding: 0px 5px 0px 5px;
            background-color: orange;
            border-radius: 8px;
        }

        .div_modal {
            --bs-modal-bg: #fff;
            --bs-modal-border-width: 1px;
            --bs-modal-border-radius: 0.5rem;
            --bs-modal-box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
            --bs-modal-width: 500px;
            --bs-modal-padding: 1rem;
            --bs-modal-margin: 0.5rem;
            --bs-border-color-translucent: rgba(0, 0, 0, 0.175);
            --bs-modal-border-color: var(--bs-border-color-translucent);
            --bs-modal-border-width: 1px;
            --bs-modal-border-radius: 0.5rem;
            --bs-modal-box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
            --bs-modal-inner-border-radius: calc(0.5rem - 1px);
            --bs-modal-header-padding-x: 1rem;
            --bs-modal-header-padding-y: 1rem;
            --bs-modal-header-padding: 1rem 1rem;
            --bs-border-color: #dee2e6;
            --bs-modal-header-border-color: var(--bs-border-color);
            --bs-modal-header-border-width: 1px;
            --bs-modal-title-line-height: 1.5;
            --bs-modal-footer-gap: 0.5rem;
            position: fixed;
            top: 0;
            width: 100%;
            height: 90%;
            overflow-x: hidden;
            overflow-y: auto;
            outline: 0
        }


        .btn-White {
            --bs-btn-color: #000000;
            --bs-btn-bg: #ffffff;
            --bs-btn-border-color: #000000;
            --bs-btn-hover-color: #000000;
            --bs-btn-hover-bg: #ffffff;
            --bs-btn-hover-border-color: #0a58ca;
            --bs-btn-focus-shadow-rgb: 49,132,253;
            --bs-btn-active-color: #fff;
            --bs-btn-active-bg: #ffffff;
            --bs-btn-active-border-color: #0a53be;
            --bs-btn-active-shadow: inset 0 3px 5px rgba(0, 0, 0, 0.125);
            --bs-btn-disabled-color: #000000;
            --bs-btn-disabled-bg: #ffffff;
            --bs-btn-disabled-border-color: #0d6efd;
        }

        .siteWrapAnywhere {
            overflow-wrap: anywhere;
        }
    </style>
    <script type="text/javascript">

        function pageLoad() {
            $("#<%= txtDateStart.ClientID %>").datepicker({
                uiLibrary: 'bootstrap4',
                format: 'dd/mm/yyyy'
            }),

                $("#<%= txtDateEnd.ClientID %>").datepicker({
                    uiLibrary: 'bootstrap4',
                    format: 'dd/mm/yyyy'
                })

        }
    </script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
    <div runat="server" id="dv_Main">
        <div style="margin-top: 80px;">

            <asp:Label runat="server" ID="lbForm" Text="Transportation Outsource(Admin)" CssClass="h1"></asp:Label>
            <asp:Label runat="server" ID="lbuserid" Visible="false"></asp:Label>
            <div class="row justify-content-end ">
                <div class=" col-lg-2 col-md-2 col-sm-6 mt-3 ">
                    <asp:Button runat="server" ID="btnCreateOrder" Text="Create Order" CssClass="btn btn-primary s-15px" Width="100%" OnClick="btnCreateOrder_Click" UseSubmitBehavior="false" />
                </div>
                <div class=" col-lg-2 col-md-2 col-sm-6 mt-3">
                    <asp:Button runat="server" ID="btnUpdatePno" Text="Update พัสดุ" CssClass="btn btn-primary s-15px" Width="100%" OnClick="btnUpdatePno_Click" UseSubmitBehavior="false" />
                </div>
            </div>

        </div>
        <div class="row col-12" runat="server" id="dv_Comment">
            <asp:Label runat="server" ID="lbUpdateStatusComment" Text="ประกาศจากระบบ" CssClass="h3"></asp:Label><br />
            <div class="row col-12 overflow-auto" style="height: 80px">
                <asp:GridView runat="server" ID="gv_UpdateComment" ShowHeader="false" AutoGenerateColumns="false" CssClass="border-0 table-sm">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbCommerntID" Visible="false" Text='<%# Bind("CM_ID") %>'></asp:Label>
                                <asp:Label runat="server" ID="lbStatusComment" CssClass="status-tracking" Text='<%# Bind("CM_Status") %>'></asp:Label>&nbsp;
                        (<asp:Label runat="server" ID="lbDateCreate" Text='<%# Bind("CM_DateCreate") %>'></asp:Label>)
                        <asp:Label runat="server" ID="lbComment" Text='<%# Bind("CM_Message") %>'></asp:Label>
                                <asp:Label runat="server" ID="lbUrlFile" Text='<%# Bind("CM_Url") %>' Visible="false"></asp:Label>
                                <asp:ImageButton runat="server" ID="imgGuie" ImageUrl="~/Icon/pdf.png" Width="20px" Visible="false" />
                                <asp:Label runat="server" ID="lbNew" Text="NEW!" CssClass="rainbow" Visible="false">
                                </asp:Label>
                            </ItemTemplate>
                            <ItemStyle CssClass="mb-2 small" Height="20px" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="mt-2">
            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                    <div class=" input-group">
                        <asp:Label runat="server" ID="lbDocnoSearch" Text="เลขที่เอกสาร" CssClass="input-group-text small"></asp:Label>
                        <asp:TextBox runat="server" ID="txtDocnoSearch" CssClass="form-control small"></asp:TextBox>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                    <div class=" input-group ">
                        <asp:Label runat="server" ID="lbPnoSearch" Text="รหัสพัสดุ" CssClass="input-group-text small"></asp:Label>
                        <asp:TextBox runat="server" ID="txtPnoSearch" CssClass="form-control small"></asp:TextBox>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                    <div class=" input-group">
                        <asp:Label runat="server" ID="lbDstNameSearch" Text="ชื่อผู้รับ" CssClass="input-group-text small"></asp:Label>
                        <asp:TextBox runat="server" ID="txtDstNameSearch" CssClass="form-control small"></asp:TextBox>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                    <div class=" input-group">
                        <asp:Label runat="server" ID="Label1" Text="สถานะเอกสาร" CssClass="input-group-text small"></asp:Label>
                        <asp:DropDownList runat="server" ID="ddlStatusOrder" CssClass="btn dropdown-item-text s-15px shadow small" OnSelectedIndexChanged="ddlStatusOrder_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Text="ยังไม่ได้เรียกรถ" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="เรียกรถแล้ว" Value="2"></asp:ListItem>
                            <asp:ListItem Text="ยกเลิก" Value="3"></asp:ListItem>
                            <asp:ListItem Text="ทั้งหมด" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <%--</div>
        <div class="row mt-2">--%>
                <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3" style="align-self: center;">
                    <%--<div class=" align-self-center">--%>
                    <div class="input-group ">
                        <asp:Label runat="server" ID="lbArticleSearch" Text="ประเภทพัสดุ" CssClass="input-group-text"></asp:Label>
                        <asp:TextBox runat="server" ID="txtArticleSearch" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <%--</div>--%>
                <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                    <div class=" input-group ">
                        <asp:Label runat="server" ID="lbDateStart" Text="เริ่มวันที่ " CssClass="input-group-text ">
                            <asp:TextBox runat="server" ID="txtDateStart" CssClass="form-control" BackColor="White" AutoCompleteType="Disabled"></asp:TextBox>
                        </asp:Label>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidatorDateStart" runat="server" ControlToValidate="txtDateStart" ErrorMessage="กรุณาเลือกวันที่เริ่มในการค้นหา" ValidationExpression="^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                    <div class=" input-group">
                        <asp:Label runat="server" ID="lbDateEnd" Text="สิ้นสุดวันที่" CssClass="input-group-text">
                            <asp:TextBox runat="server" ID="txtDateEnd" CssClass="form-control" BackColor="White" AutoCompleteType="Disabled"></asp:TextBox>
                        </asp:Label>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidatorDateEnd" runat="server" ControlToValidate="txtDateEnd" ErrorMessage="กรุณาเลือกวันที่สุดท้ายในการค้นหา" ValidationExpression="^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3" style="align-self: center;" runat="server" id="dv_DO_Search" visible="false">
                    <div class=" input-group">
                        <asp:Label runat="server" ID="Label2" Text="DO" CssClass="input-group-text"></asp:Label>
                        <asp:TextBox runat="server" ID="txtDOSearch" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="col-sm-12 col-md-12 col-lg-2 col-xl-1 gy-3">
                    <%--<div class="col-sm-1">--%>
                    <asp:Button runat="server" ID="btnSearch" Text="SEARCH" CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" UseSubmitBehavior="true" />
                    <%--</div>--%>
                </div>
                <div class="col-sm-12 col-md-12 col-lg-2 col-xl-1 gy-3">
                    <%--<div class="col-sm-1">--%>
                    <asp:Button runat="server" ID="btnClear" Text="CLEAR" CssClass="btn btn-primary w-100" OnClick="btnClear_Click" Visible="false" UseSubmitBehavior="false" />
                    <asp:Label runat="server" ID="lbStatusSearch" Text="First" Visible="false"></asp:Label>
                    <%--</div>--%>
                </div>
                <div class="col-sm-12 col-md-12 col-lg-2 col-xl-1 gy-3">
                    <%--<div class="col-sm-1">--%>
                    <asp:Button runat="server" ID="btnExport" Text="Export/Day" CssClass="btn btn-primary w-100" OnClick="btnExport_Click" UseSubmitBehavior="false" ToolTip="" />
                    <%--</div>--%>
                </div>
                <div class="col-sm-12 col-md-12 col-lg-2 col-xl-1 gy-3">
                    <div class=" input-group" style="flex-wrap: nowrap;">
                        <asp:Label runat="server" ID="Label3" Text="บริษัทขนส่ง" CssClass="input-group-text small "></asp:Label>
                        <asp:DropDownList runat="server" ID="ddlTypsend" CssClass="btn dropdown-item-text s-15px shadow small maxwidthnone" OnSelectedIndexChanged="ddlStatusOrder_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Text="FlashExpress" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Lalamove" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                
            </div>
            <div class="row mt-2 align-center">
                <asp:Label runat="server" ID="lbAlert" Text="* ปุ่ม Export/Day เป็นรายงานของการเรียกรถ Flash ให้มารับที่จุดรับของ ซึ่งจะรวมรายการตั้งแต่ 16.30 น. ของเมื่อวานถึง 16.30น. ของวันนี้ *" ForeColor="Red"></asp:Label>
            </div>

        </div>
        <div class="row my-3 overflow-auto">
                <asp:GridView runat="server" ID="gv_OrderAll" Width="1500px" EmptyDataText="ไม่มีการสร้างรายการ" AutoGenerateColumns="false" CssClass="table table-striped table-bordered table-hover table-sm small"
                    HeaderStyle-HorizontalAlign="Center">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox runat="server" ID="cbAll" ToolTip="เลือกทั้งหมด" CssClass="m-2" OnCheckedChanged="cbAll_CheckedChanged" AutoPostBack="true" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="cbItem" CssClass="m-2" />
                            </ItemTemplate>
                            <ItemStyle CssClass="gj-text-align-center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbhDocno" Text="เลขที่เอกสาร/เลขที่พัสดุ"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lkbDocno" Text='<%# Bind("Docno") %>' OnClick="lkbDocno_Click"></asp:LinkButton><br />
                                <asp:Label runat="server" ID="lbpno" Text='<%# Bind("pno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbhUserCreate" Text="ผู้สร้าง"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbUserCreate" Text='<%# Bind("nameCreate") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbhBrand" Text="Brand"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbBrand" Text='<%# Bind("Brand") %>'></asp:Label>
                                <asp:Label runat="server" ID="lbBrandID" Text='<%# Bind("Brand") %>' Visible="false"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <span>ที่อยู่ผู้รับ</span>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbSiteStorage" Text='<%# Bind("dstDetailAddress") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbhsrcName" Text="ชื่อผู้ส่ง"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbsrcName" Text='<%# Bind("srcName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbhdstName" Text="ชื่อผู้รับ"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbdstName" Text='<%# Bind("dstName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="false">
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbhCategory" Text="ประเภทพัสดุ"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbCategory" Text='<%# Bind("ArticleCategory") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle CssClass="gj-text-align-center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbhRemark" Text="หมายเหตุ"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbRemark" Text='<%# Bind("Remark") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle CssClass="gj-text-align-center w-10" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbhDateCreate" Text="Date Create"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbDateCreate" Text='<%# Bind("dateCreate") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle CssClass="gj-text-align-center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbhTrackingPickup" Text="รูปแบบการจัดส่ง"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbTransport_Type" Text='<%# Bind("Transport_Type") %>'></asp:Label>
                                <asp:Label runat="server" ID="lbStatus" Text='<%# Bind("TrackingPickup") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lbStatusItem" Text='<%# Bind("status") %>' Visible="false"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <span>ชื่อพนักงานคูเรีย/<br />Delivery Order</span>
                                
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbStaffinfoId" Text='<%# Bind("StaffInfoName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbhTimeTracking" Text="เวลารถมารับ"></asp:Label>
                            </HeaderTemplate>
                            <ItemStyle CssClass="w-10" />
                            <ItemTemplate>
                                <div class="align-center" style="padding-top: 11%;">
                                    <asp:Label runat="server" ID="lbTimeTrackingText" Text='<%# Bind("TimeTrackingText") %>'></asp:Label>
                                    <asp:Label runat="server" ID="lbTimeTracking" Text='<%# Bind("TimeTracking") %>' Visible="false"></asp:Label>
                                </div>
                            </ItemTemplate>
                            <ItemStyle CssClass="gj-text-align-center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="lbProcess" Text="การดำเนินการ"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbTypeSend" Text='<%# Bind("TypeSend") %>' Visible="false"></asp:Label>
                                <asp:ImageButton runat="server" ID="imgbtnEdit" Width="30px" ImageUrl="~/Icon/edit.png" OnClick="imgbtnEdit_Click" Visible="false" CssClass="mr-2" />
                                <asp:ImageButton runat="server" ID="imgbtnCancelOrder" Width="30px" ImageUrl="~/Icon/x-button.png" OnClick="imgbtnCancelOrder_Click" />
                                <asp:ImageButton runat="server" ID="imgbtnGet" Width="30px" ImageUrl="~/Icon/correct.png" OnClick="imgbtnGet_Click" Visible="false" />
                            </ItemTemplate>
                            <ItemStyle CssClass="gj-text-align-center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            
        </div>
        <div class="row">
            <div class="col-md-4">
                <div aria-label="nav Page navigation example" runat="server" id="div_Page_Bar">
                    <div class="btn-group" role="group" aria-label="Basic outlined example">
                        <asp:LinkButton runat="server" ID="lkPrevious" CssClass="btn btn-outline-primary " OnCommand="selectPage" Visible="false">PREVIOUS</asp:LinkButton>
                        <asp:LinkButton runat="server" ID="lkFirst" CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false"></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="lk1" CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false"></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="lk2" CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false"></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="lk3" CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false"></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="lkLast" CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false"></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="lkNext" CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false">NEXT</asp:LinkButton>
                    </div>
                </div>
            </div>
           
            <div class="col-sm-12 col-md-3 border-2 my-2" >
                
                <div class=" input-group" runat="server" id="dv_Deliver" visible="false">

                    <span class="input-group-text">Delivery Order</span>
                    <asp:TextBox runat="server" ID="txtDeliveryOrder" CssClass="form-control"></asp:TextBox>
                </div>

            </div>
            <div class="col-sm-12 col-md-3 border-2 my-2" >
                <%--<div class=" input-group" runat="server" id="dv_price" visible="false">
                    <span class="input-group-text">ราคา</span><br />
                    <asp:TextBox runat="server" ID="txtPriceDelivery" CssClass="form-control"></asp:TextBox>
                </div>--%>

            </div>
            <div class="col-md-1">
                <div style="margin-bottom: 90px">
                    <div class="row float-right " style="flex-wrap: nowrap; ">
                        <div class="col-5" runat="server" id="div_SendFree">
                            <asp:Button runat="server" ID="btnSendFree" OnClick="btnSendFree_Click" CssClass="btn btn-primary mt-4" Text="ส่งไปรษณีย์/ส่งเอง" UseSubmitBehavior="false" Visible="false" />
                        </div>
                        <div class="col-6">
                            <asp:ImageButton runat="server" ID="btnLalamove" OnClick="btnLalamove_Click" Width="110px" CssClass="mt-2 mg-Lala" ImageUrl="~/Icon/Lalamove.png" Visible="false" />
                        </div>
                        <div class="col-6">
                            <asp:ImageButton runat="server" ID="btnNotify" OnClick="btnNotifications_Click" Width="100px" CssClass="img-mt--12px" ImageUrl="~/Icon/fast-delivery.png" />
                        </div>
                    </div>
                </div>
            </div>
            
        </div>
        
        <!-- Timer Tick for click update -->
        <%-- <div>
        <asp:Timer runat="server" ID="Time1" OnTick="Time1_Tick" Interval="1000">
        </asp:Timer>
        <asp:UpdatePanel runat="server" ID="upPanel" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Time1" />
            </Triggers>
            <ContentTemplate>
                <asp:Label runat="server" ID="lbTime" Visible="false"></asp:Label>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>--%>
    </div>


    <%--<div runat="server" id="dv_Lalamove" visible="false">
        <div class="div_modal modal-dialog maxW50 modal-md modal-dialog-scrollable modal-dialog-centered" style="position: absolute;">
            <div class="modal-content dv">
                <div class="modal-header with-border">
                    <h4 class="modal-title">Lalamove Delivery</h4>
                    <asp:Button runat="server" ID="btnCloseDv_Lalamove" class="btn-close" OnClick="btnCloseDv_Lalamove_Click" UseSubmitBehavior="false"></asp:Button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-8">
                            <span>Delivery Order</span><br />
                            <asp:TextBox runat="server" ID="txtDeliveryOrder" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-4" runat="server" id="dv_price" visible="false">
                            <span>ราคา</span><br />
                            <asp:TextBox runat="server" ID="txtPriceDelivery" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-4">
                            <br />
                            <asp:Button runat="server" ID="btnConfirmDelivery" Text="ยืนยัน" CssClass="btn btn-primary small" OnClick="btnConfirmDelivery_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>--%>
    
</asp:Content>
