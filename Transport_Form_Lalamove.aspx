<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Transport_Form_Lalamove.aspx.cs" MasterPageFile="~/Site.Master" Inherits="Carrier.Transport_Form_Lalamove" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    
    <script type="text/javascript">

        function pageLoad() {
            $("#<%= txtDateSt.ClientID %>").datepicker({
                uiLibrary: 'bootstrap4',
                format: 'dd/mm/yyyy'
            }),$("#<%= txtDateED.ClientID %>").datepicker({
                uiLibrary: 'bootstrap4',
                format: 'dd/mm/yyyy'
            });

        }


        $(document).ready(
            function () {

            }
        )
        
    </script>
    <script type="text/javascript" src='https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3.min.js'></script>
    <script src="https://code.jquery.com/jquery-3.6.0.js"></script>
    <script src="https://code.jquery.com/ui/1.13.0/jquery-ui.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
    
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

            .marginDetail{
                margin-top: 33%;
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
            .marginDetail{
                margin-top: 33%;
            }
        }

        @media (min-width:768px) {
            .w-img {
                width: 40%;
            }
            .maxW50 {
                max-width: 80%;
            }

            .maxW60 {
                max-width: 70%;
                transform: translate(15%);
            }

            .marginDetail{
                margin-top: 44%;
            }
        }

        @media (min-width:992px) {
            .w-img {
                width: 30%;
            }
            .maxW50 {
                max-width: 80%;
                transform: translate(4%);
            }

            .maxW60 {
                max-width: 70%;
                transform: translate(15%);
            }
            .marginDetail{
                margin-top: 33%;
            }
        }

        @media (min-width:1200px) {
            .w-img {
                width: 30%;
            }
            .maxW50 {
                max-width: 80%;
                transform: translate(6%);
            }

            .maxW60 {
                max-width: 70%;
                transform: translate(15%);
            }
            .marginDetail{
                margin-top: 33%;
            }
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
        .maxwidthnone {
            max-width: none;
        }

        .ddlwidthFull {
            width: -webkit-fill-available;
        }

        .s-15px {
            font-size: 15px;
        }

        .s-22px {
            font-size: 22px;
        }
    </style>
    <asp:Label runat="server" ID="lbuserID" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="lbDocno" Visible="false"></asp:Label>
    <asp:UpdatePanel ID="updatePanel1" runat="server">
        <ContentTemplate>
            <div runat="server" id="dv_main" style="margin-top: 80px;">
                <div class="row">
                    <div class="col-12">
                        <h>Report Lalamove</h>
                    </div>
                </div>
                <div class="row my-2" >
                    <div class="col-md-3" runat="server" id="dv_DateST">
                        <asp:Label runat="server" ID="lbDateST" Text="วันที่เริ่ม"></asp:Label>
                        <asp:TextBox runat="server" ID="txtDateSt"></asp:TextBox>
                    </div>
                    <div class="col-md-3" runat="server" id="dv_DateED">
                        <asp:Label runat="server" ID="lbDateED" Text="วันที่สิ้นสุด"></asp:Label>
                        <asp:TextBox runat="server" ID="txtDateED"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <asp:Label runat="server" ID="lbDeliveryId" Text="DeliveryID" CssClass="form-label"></asp:Label>
                        <asp:TextBox runat="server" ID="txtDelivery" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-1">
                        <br />
                        <asp:Button runat="server" ID="btnSearch" Text="ค้นหา" OnClick="btnSearch_Click" CssClass="btn btn-outline-primary w-100" />
                    </div>

                    <div class="row">
                        <br />
                        <div class="col-md-4">
                            <span>Import From Lalamove For Check</span>
                            <asp:FileUpload runat="server" ID="fileupload1" />

                            <asp:Button runat="server" ID="btnRun" Text="CHECK" CssClass="btn btn-outline-primary" OnClick="btnRun_Click" />
                        </div>
                        <div class="col-md-1">
                            <asp:Label runat="server" ID="lbTypeExcel" Text="Type Export"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlTypeExport" CssClass="btn" OnSelectedIndexChanged="ddlTypeExport_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Value="REP" Text="Report" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="SAP" Text="SAP"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-1">
                            <br />
                            <asp:Button runat="server" ID="btnExport" Text="Export" OnClick="btnExport_Click"  CssClass="btn btn-outline-primary w-100" UseSubmitBehavior="false"  />
                        </div>
                        <div class="col-md-4 border-primary" runat="server" id="dv_App">
                            <span>สร้าง Budget ค่าคนส่งอัตโนมัติหรือใหม่?</span>
                            <div style="display: flex;">
                                <asp:Button runat="server" ID="btnUptoBudget" Text="นำไปตัด Budget" OnClick="btnUptoBudget_Click" CssClass="btn btn-outline-primary w-100" UseSubmitBehavior="false" />
                                <asp:Button runat="server" ID="btnApprove" Text="Approve" OnClick="btnApprove_Click" CssClass="btn btn-outline-primary w-100" UseSubmitBehavior="false" Visible="false"/>
                                <asp:Button runat="server" ID="btnReject" Text="Reject" OnClick="btnReject_Click" CssClass="btn btn-outline-danger w-100" UseSubmitBehavior="false" Visible="false"/>

                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-12 text-center">
                        <span>* การขึ้น Budget เป็นการสร้างเอกสารลงค่าใช้จ่ายของค่าขนส่ง ในเวลา 01.15 น. ของทุกวัน หลังจากกดยืนยันให้ขึ้น Budget โดย Order Delivery ตรงกันกับทางระบบ Lalamove *</span><br />
                        <span>* โปรดตรวจสอบให้แน่ใจก่อนทำการขึ้น Budget ทุกครั้ง *</span>
                    </div>
                </div>
                <div class="row mt-3" runat="server" id="dv_gv_main">
                    <div class="col-12">
                        <asp:GridView runat="server" ID="gv_main" CssClass="table table-striped table-bordered table-hover table-sm small" AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField Visible="false">
                                    <HeaderTemplate>
                                        <span>ขึ้น Budget</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbstatusBud" Visible="false" Text='<%# Bind("StatusBud") %>'></asp:Label>
                                        <asp:ImageButton runat="server" ID="imgbtnCheckOrder" ImageUrl="~/Icon/correct.png" Visible="false" Width="30px" OnClick="imgbtnCheckOrder_Click"/>
                                    </ItemTemplate>
                                    <ItemStyle Width="50px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>เลขที่ Order Delivery</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="lnkDeliveryNumber" Text='<%# Bind("DeliveryNumber") %>' OnClick="lnkDeliveryNumber_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>เวลา</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbDateTime" Text='<%# Bind("Date_Group") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>SiteStorage</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbSiteStorage" Text='<%# Bind("SiteStorage") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="400px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>แผนก/Brand</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbSDpart" Text='<%# Bind("SDpart") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>จำนวนกล่อง</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbQTY" Text='<%# Bind("QTY") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="50px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>ค่ารถ</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbPrice" Text='<%# Bind("Price") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="50px" CssClass="text-end"/>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div class="row mt-3" id="dv_gv_import_Check" runat="server" visible="false">
                    <asp:GridView runat="server" id="gv_import_Check" AutoGenerateColumns="false" CssClass="table table-hover table-bordered">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>Delivery</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbMatch" Text='<%# Bind("Match") %>' Visible="false"></asp:Label>
                                    <asp:Label runat="server" ID="lbDeliveryID" Text='<%# Bind("DeliveryID") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>Date Complete</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbDateComplete" Text='<%# Bind("dateComplete") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>Price E-Form</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbPrice" Text='<%# Bind("price") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>Price Real</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbPrice_Lalamove" Text='<%# Bind("price_Lalamove") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            <div runat="server" id="dv_detail" visible="false" class="marginDetail">
                <div class="div_modal maxW50 modal-dialog  modal-md modal-dialog-scrollable modal-dialog-centered " style="position: absolute;    min-width: min-content;
    max-block-size: min-content;">
                    <div class="modal-content dv">
                        <div class="modal-header with-border">
                            <h4 class="modal-title">Lalamove Delivery Detail</h4>
                            <asp:Button runat="server" ID="btnCloseDv_detail" class="btn-close" OnClick="btnCloseDv_detail_Click" UseSubmitBehavior="false"></asp:Button>
                        </div>
                        <div class="modal-body">
                            <div class="row my-2">
                                <div class="col-4">
                                    <div class="input-group">
                                        <span class="input-group-text">Delivery ID:</span>
                                        <asp:Label runat="server" ID="lbDetail_DeliveryID" CssClass="form-control" ></asp:Label>
                                    </div>
                                </div>
                                <div class="col-4" runat="server" id="dv_detail_price" visible="false">
                                    <div class="input-group">
                                        <span class="input-group-text">ราคา:</span>
                                        <asp:Label runat="server" ID="lbDetail_Price" CssClass="form-control"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-12 overflow-auto">
                                    <asp:GridView runat="server" ID="gv_detail" CssClass="table table-striped table-bordered table-hover table-sm small" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <span>Site Storage</span>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lbDetail_SiteStorage" Text='<%# Bind("site") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <span>จุดส่ง</span>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lbDetail_Address" Text='<%# Bind("address") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <span>แผนก | จำนวนกล่อง</span>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:GridView runat="server" ID="gv_Detail_Sub" ShowHeader="false" CssClass="table table-striped table-bordered table-hover table-sm small" AutoGenerateColumns="false">
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lbSDpart" Text='<%# Bind("SDpart") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lbQTY" Text='<%# Bind("QTY") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lbPrice" Text='<%# Bind("Price") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID ="btnExport" />
            <asp:PostBackTrigger ControlID ="btnRun" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
