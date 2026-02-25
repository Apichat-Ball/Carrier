<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Central_Import2.aspx.cs" Inherits="Carrier.Central_Import2" MasterPageFile="~/Site.Master" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    
    <script type="text/javascript">

        function pageLoad() {
            $("#<%= txtDateSt.ClientID %>").datepicker({
                uiLibrary: 'bootstrap4',
                format: 'dd/mm/yyyy'
            }),$("#<%= txtDateED.ClientID %>").datepicker({
                uiLibrary: 'bootstrap4',
                format: 'dd/mm/yyyy'
            }), $("#<%= txtPostingDate.ClientID %>").datepicker({
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
        
        .placeItemCenter{
            place-items:center;
        }
    </style>
    <asp:Label runat="server" ID="lbuserID" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="lbDocno" Visible="false"></asp:Label>
    <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div runat="server" id="dv_main" style="margin-top: 80px;">
                <div class="row">
                    <div class="col-12">
                        <h3>Report Central</h3>
                    </div>
                </div>
                <div class="Search my-2">
                    <div class="row my-2">
                        <div class="col-md-3" runat="server" id="dv_DateST">
                            <asp:Label runat="server" ID="Label1" Text="วันที่เริ่ม"></asp:Label>
                            <asp:TextBox runat="server" ID="txtDateSt"></asp:TextBox>
                        </div>
                        <div class="col-md-3" runat="server" id="dv_DateED">
                            <asp:Label runat="server" ID="Label2" Text="วันที่สิ้นสุด"></asp:Label>
                            <asp:TextBox runat="server" ID="txtDateED"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <br />
                            <asp:Button runat="server" ID="btnSearch" Text="ค้นหา" OnClick="btnSearch_Click" CssClass="btn btn-outline-primary w-100" UseSubmitBehavior="false" />
                        </div>
                    </div>
                </div>
                <div class="Import my-2">
                    <div class="row">
                        <div class="col-md-3">
                            <asp:Label runat="server" ID="Label3" Text="PostingDate"></asp:Label>
                            <asp:TextBox runat="server" ID="txtPostingDate"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <asp:Label runat="server" ID="Label4" Text="Price"></asp:Label>
                            <asp:TextBox runat="server" ID="txtPriceAdj"  CssClass="form-control maxwidthnone"></asp:TextBox>
                        </div>
                        <div class="col-md-3 ">
                            <br />
                            <div style="display: flex;">
                                <asp:FileUpload runat="server" ID="fileupload1" CssClass="form-control maxwidthnone" />
                                <div>
                                    <asp:Button runat="server" ID="btnRun" Text="Import" CssClass="btn btn-outline-primary " OnClick="btnRun_Click" UseSubmitBehavior="false" />
                                </div>
                            </div>
                            

                        </div>
                        <div class="col-md-1" runat="server" id="dv_Export" visible="false">
                            <br />
                            <asp:Button runat="server" ID="btnExport" Text="Export" OnClick="btnExport_Click"  CssClass="btn btn-outline-primary w-100" UseSubmitBehavior="false" />
                        </div>
                    </div>
                </div>
                <div class="UploadTOFC" runat="server" id="dv_uploadTOFC" visible="false">
                    <div class="row">
                        <div class="col-md-4 border-primary">
                            <br />
                            <div style="display: flex;">
                                <asp:Button runat="server" ID="btnUptoBudget" Text="นำไปตัด Budget" OnClick="btnUptoBudget_Click" CssClass="btn btn-outline-primary w-100" UseSubmitBehavior="false" />
                                <asp:Button runat="server" ID="btnApprove" Text="Approve" OnClick="btnApprove_Click" CssClass="btn btn-outline-primary w-100" UseSubmitBehavior="false" Visible="false"/>
                                <asp:Button runat="server" ID="btnReject" Text="Reject" OnClick="btnReject_Click" CssClass="btn btn-outline-danger w-100" UseSubmitBehavior="false" Visible="false"/>

                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 text-center">
                            <span>* โปรดตรวจสอบให้แน่ใจก่อนทำการขึ้น Budget ทุกครั้ง *</span>
                        </div>
                    </div>
                </div>
                
                
                <div class="Adj" runat="server" id="dv_Adj" visible="false">
                    <div class="SaveAdj">
                        <asp:Button runat="server" ID="btnSaveAdj" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveAdj_Click" UseSubmitBehavior="false"/>
                    </div>
                    <div class="row mt-3">
                        <div class="col-12">
                            <asp:GridView runat="server" ID="gv_Temp" CssClass="table table-striped table-bordered table-hover table-sm small"
                                AutoGenerateColumns="false">
                                <Columns>
                                    <asp:BoundField HeaderText="Process Date" DataField="DateProcess" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-Width="200px" />
                                    <asp:BoundField HeaderText="Shop" DataField="Shop" HeaderStyle-Width="200px" />
                                    <asp:BoundField HeaderText="Brand" DataField="Brand"  HeaderStyle-Width="100px" />
                                    <asp:TemplateField HeaderText="Price">
                                        <HeaderStyle Width="300px" />
                                        <ItemStyle CssClass="text-end placeItemCenter"/>
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="txtPriceAdj" CssClass="form-control text-end maxwidthnone" Text='<%# Bind("Price") %>' Visible='<%# Eval("No").ToString() == "1" %>' AutoPostBack="true" OnTextChanged="txtPriceAdj_TextChanged" ></asp:TextBox>
                                            <asp:Label runat="server" ID="lbPriceAdj" Text='<%# Eval("Price") %>' Visible='<%# Eval("No").ToString()!="1" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="VAT(7%)">
                                        <HeaderStyle Width="300px" />
                                        <ItemStyle CssClass="text-end placeItemCenter"/>
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="txtVatAdj" CssClass="form-control text-end maxwidthnone" Text='<%# Bind("Vat") %>' Visible='<%# Eval("No").ToString() == "1" %>' AutoPostBack="true" 
                                                OnTextChanged="txtVatAdj_TextChanged"></asp:TextBox>
                                            <asp:Label runat="server" ID="lbVatAdj" Text='<%# Eval("Vat") %>' Visible='<%# Eval("No").ToString() != "1" %>'></asp:Label>
                                            <asp:Label runat="server" ID="lbDepartmentID" Text='<%# Eval("DepartmentID") %>' Visible="false"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DepartmentID" Visible="false" />
                                </Columns>
                            </asp:GridView>
                        </div>

                    </div>
                    
                </div>
                
                <div class="row mt-3" runat="server" id="dv_gv_main" visible="false">
                    <div class="col-12">
                        <asp:GridView runat="server" ID="gv_main" CssClass="table table-striped table-bordered table-hover table-sm small" AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>Posting Date</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbPosting_Date" Text='<%# Bind("Posting_Date") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" />
                                    
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>Shop</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbSiteStorage" Text='<%# Bind("Shop") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>แผนก/Brand</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbBrand" Text='<%# Bind("Brand") %>'></asp:Label>
                                        <asp:Label runat="server" ID="lbDepartment_id" Text='<%# Bind("departmentID") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>ค่ารถ</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbPrice" Text='<%# Bind("Price") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="120px" CssClass="text-end"/>
                                </asp:TemplateField>
                                <asp:TemplateField >
                                    <HeaderTemplate>
                                        <span>VAT</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbVAT" Text='<%# Bind("VAT") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="120px" CssClass="text-end"/>
                                </asp:TemplateField>
                                 <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>เลขที่เอกสาร</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbDocnoBud" Text='<%# Bind("Docno") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="150px" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
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
