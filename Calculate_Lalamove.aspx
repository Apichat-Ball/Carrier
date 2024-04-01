<%@ Page Title="Courier" Language="C#" AutoEventWireup="true" CodeBehind="Calculate_Lalamove.aspx.cs" MasterPageFile="~/Site.Master" Inherits="Carrier.Calculate_Lalamove" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        function pageLoad() {
            $("#<%= txtDateStart.ClientID %>").datepicker({
                uiLibrary: 'bootstrap4',
                format: 'dd/mm/yyyy'
            }),

                $("#<%= txtDateEnd.ClientID %>").datepicker({
                    uiLibrary: 'bootstrap4',
                    format: 'dd/mm/yyyy'
                });

        }
    </script>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-MrcW6ZMFYlzcLA8Nl+NtUVF0sA7MsXsP1UyJoMp4YLEuNSfAP+JcXn/tWtIaxVXM" crossorigin="anonymous"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous" />
    <script src="https://code.jquery.com/ui/1.13.0/jquery-ui.js"></script>
    <style>
        .fixedHeader {
            position: sticky;
            top: 0;
            background-color: #f9f9f9;
            z-index: 10;
        }

        .maxTable {
            min-width: 1370px;
        }
    </style>

    <script>
        toastr.options = {
            "closeButton": true,
            "debug": true,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-center",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "15000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        }

    </script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
    <div style="margin-top:80px;">

        <asp:Label runat="server" ID="lbForm" Text="บันทึกค่ารถ Lalamove" CssClass="h1"></asp:Label>
        <asp:Label runat="server" ID="lbuserid" Visible="false"></asp:Label>
        <div class="mt-2">
            <div class="row">
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
                <div class="col-sm-12 col-md-12 col-lg-2 col-xl-2 gy-3" style="display:flex;justify-content: space-between;">
                    <%--<div class="col-sm-1">--%>
                        <asp:Button runat="server" ID="btnSearch" Text="Search" CssClass="btn btn-outline-primary w-100" OnClick="btnSearch_Click" UseSubmitBehavior="true" />
                        <asp:Button runat="server" ID="btnExport" Text="Export" CssClass="btn btn-outline-primary w-100" OnClick="btnExport_Click" UseSubmitBehavior="true" />
                    <%--</div>--%>
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-12" style="overflow: auto; max-height: 700px; height: 100%;width: 100%">
                    <asp:GridView runat="server" ID="gv_Car" AutoGenerateColumns="false" CssClass="table table-hover table-border table-sm align-top maxTable"
                        HeaderStyle-CssClass="fixedHeader">
                        <Columns>
                            
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>เลขที่เอกสาร</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbOrderBig" Text='<%# Bind("BFID") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>เลขที่กล่อง</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbOrder" Text='<%# Bind("Docno") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>From</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbTypeSendKO" Text='<%# Bind("TypeSendKO") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>วันที่เรียกรถ</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbDateSucces" Text='<%# Bind("Date_Success") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>Brand</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbBrandname" Text=""></asp:Label>
                                    <asp:Label runat="server" ID="lbBrandID" Text='<%# Bind("SDpart") %>' Visible="false" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>SiteStorage</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbSiteStorage" Text='<%# Bind("siteStorage") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>จำนวนกล่อง(แปลง)</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbBox" Text='<%# Bind("QTY") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>Group Delivery</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" ID="chGroup"></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-3">
                    <span>DeliveryNumber</span>
                    <asp:TextBox runat="server" ID="txtDeliveryNumber" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <span>ค่ารถต่อรอบ</span>
                    <asp:TextBox runat="server" ID="txtPriceCar" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2 col-sm-3">
                    <br />
                    <asp:Button runat="server" ID="btnGroup" Text="คำนวณ" CssClass="btn btn-outline-primary" OnClick="btnGroup_Click" UseSubmitBehavior="false"/>
                </div>
                <div class="col-md-2 col-sm-3">
                    <br />
                    <asp:Button runat="server" ID="btnSave" Text="บันทึก" Enabled="false" CssClass="btn btn-outline-primary" OnClick="btnSave_Click" UseSubmitBehavior="false"/>
                </div>
            </div>
            <div class="row mt-2" runat="server" id="dv_Group" visible="false">
                <span style="font-size:medium">ค่ารถ</span>
                    <div class="col-12" style="overflow: auto; max-height: 700px; height: 100%;width: 100%">
                    <asp:GridView runat="server" ID="gv_Group" AutoGenerateColumns="false" CssClass="text-end table table-hover table-border table-sm align-top maxTable"
                        HeaderStyle-CssClass="fixedHeader">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    เลยที่รถ
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lbDelivery" runat="server" Text='<%# Bind("DeliveryNumber") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>เลขที่เอกสาร</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbOrderBig" Text='<%# Bind("BFID") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>เลขที่กล่อง</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbOrder" Text='<%# Bind("Docno") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>From</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbTypeSendKO" Text='<%# Bind("TypeSendKO") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>Brand</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbBrandID" Text='<%# Bind("SDpart") %>' Visible="false"></asp:Label>
                                    <asp:Label runat="server" ID="lbBrandname" Text=""></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>SiteStorage</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbSiteStorage" Text='<%# Bind("SiteStorage") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>จำนวนกล่อง(แปลง)</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbBox" Text='<%# Bind("Qty") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>ค่าใช้จ่าย</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbPrice" Text='<%# Bind("Price") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>วันที่บันทึก</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbDate_Success" Text='<%# Bind("Date_Success") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span>สถานะ</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbNew" Text='<%# Bind("New") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
    
</asp:Content>