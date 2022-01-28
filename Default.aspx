<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Carrier._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
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
                });
        }
    </script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
    <div class="mt-3">

        <asp:Label runat="server" ID="lbForm" Text="Transportation Outsource(Admin)" CssClass="h1"></asp:Label>
        <asp:Label runat="server" ID="lbuserid" Visible="false"></asp:Label>
        <div class="float-end mt-3">
            <asp:Button runat="server" ID="btnCreateOrder" Text="Create Order" CssClass="btn btn-primary s-15px" Width="100%" OnClick="btnCreateOrder_Click" UseSubmitBehavior="false" />
        </div>
    </div>
    <div class="mt-5 ">
        <div class="row">
            <div class="col-sm-3 input-group w-25">
                <asp:Label runat="server" ID="lbDocnoSearch" Text="เลขที่เอกสาร" CssClass="input-group-text"></asp:Label>
                <asp:TextBox runat="server" ID="txtDocnoSearch" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-sm-3 input-group w-25">
                <asp:Label runat="server" ID="lbPnoSearch" Text="รหัสพัสดุ" CssClass="input-group-text ml-2"></asp:Label>
                <asp:TextBox runat="server" ID="txtPnoSearch" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-sm-3 input-group w-25">
                <asp:Label runat="server" ID="lbDstNameSearch" Text="ชื่อผู้รับ" CssClass="input-group-text ml-2"></asp:Label>
                <asp:TextBox runat="server" ID="txtDstNameSearch" CssClass="form-control"></asp:TextBox>
            </div>
             <div class="col-sm-3 input-group w-25">
                <asp:Label runat="server" ID="Label1" Text="สถานะเอกสาร" CssClass="input-group-text ml-2"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlStatusOrder"  CssClass="btn dropdown-item-text">
                    <asp:ListItem Text="ยังไม่ได้เรียกรถ" Value="1" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="เรียกรถแล้ว" Value="2" ></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col-sm-3 align-self-center">
                <div class="input-group w-100">
                    <asp:Label runat="server" ID="lbArticleSearch" Text="ประเภทพัสดุ" CssClass="input-group-text"></asp:Label>
                    <asp:TextBox runat="server" ID="txtArticleSearch" CssClass="form-control"></asp:TextBox>
                </div>
            </div>

            <div class="col-sm-3 input-group w-25">
                <asp:Label runat="server" ID="lbDateStart" Text="เริ่มวันที่ " CssClass="input-group-text ml-2">
                    <asp:TextBox runat="server" ID="txtDateStart" CssClass="form-control" BackColor="White" AutoCompleteType="Disabled"></asp:TextBox>
                </asp:Label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorDateStart" runat="server" ControlToValidate="txtDateStart" ErrorMessage="กรุณาเลือกวันที่เริ่มในการค้นหา" ValidationExpression="^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-3 input-group w-25">
                <asp:Label runat="server" ID="lbDateEnd" Text="สิ้นสุดวันที่" CssClass="input-group-text ml-2">
                    <asp:TextBox runat="server" ID="txtDateEnd" CssClass="form-control" BackColor="White" AutoCompleteType="Disabled"></asp:TextBox>
                </asp:Label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorDateEnd" runat="server" ControlToValidate="txtDateEnd" ErrorMessage="กรุณาเลือกวันที่สุดท้ายในการค้นหา" ValidationExpression="^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-1">
                <asp:Button runat="server" ID="btnSearch" Text="SEARCH" CssClass="btn btn-primary" OnClick="btnSearch_Click" UseSubmitBehavior="true" />
            </div>
            <div class="col-sm-1">
                <asp:Button runat="server" ID="btnClear" Text="CLEAR" CssClass="btn btn-primary" OnClick="btnClear_Click" Visible="false" UseSubmitBehavior="false" />
                <asp:Label runat="server" ID="lbStatusSearch" Text="First" Visible="false"></asp:Label>
            </div>
        </div>

    </div>
    <div class="row mt-3">
        <asp:GridView runat="server" ID="gv_OrderAll" Width="100%" EmptyDataText="ไม่มีการสร้างรายการ" AutoGenerateColumns="false" CssClass="table table-striped table-bordered table-hover table-sm small"
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
                        <asp:Label runat="server" ID="lbhDocno" Text="เลขที่เอกสาร"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lkbDocno" Text='<%# Bind("Docno") %>' OnClick="lkbDocno_Click"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbhpno" Text="รหัสพัสดุ"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbpno" Text='<%# Bind("pno") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbhBrand" Text="Brand"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbBrand" Text='<%# Bind("Brand") %>'></asp:Label>
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
                <asp:TemplateField>
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
                    <ItemStyle CssClass="gj-text-align-center" />
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
                        <asp:Label runat="server" ID="lbhStaffinfoId" Text="ชื่อพนักงานคูเรีย"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbStaffinfoId" Text='<%# Bind("StaffInfoName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbhTimeTracking" Text="เวลารถมารับ"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbTimeTrackingText" Text='<%# Bind("TimeTrackingText") %>'></asp:Label>
                        <asp:Label runat="server" ID="lbTimeTracking" Text='<%# Bind("TimeTracking") %>' Visible="false"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="gj-text-align-center" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbProcess" Text="การดำเนินการ"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbTypeSend" Text='<%# Bind("TypeSend") %>' Visible="false"></asp:Label>
                        <asp:ImageButton runat="server" ID="imgbtnCancelOrder" Width="30px" ImageUrl="~/Icon/x-button.png" OnClick="imgbtnCancelOrder_Click" />
                    </ItemTemplate>
                    <ItemStyle CssClass="gj-text-align-center" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div aria-label="nav Page navigation example">
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
    <div style="margin-bottom: 90px">
        <div class="row float-right " style="flex-wrap: nowrap; margin-right: 100px">
            <div class="col-5" runat="server" id="div_SendFree" >
                <asp:Button runat="server" ID="btnSendFree" OnClick="btnSendFree_Click" CssClass="btn btn-primary mt-4" Text="ส่งไปรษณีย์/ส่งเอง" UseSubmitBehavior="false" Visible="false" />
            </div>
            <div class="col-6">
                <asp:ImageButton runat="server" ID="btnLalamove" OnClick="btnLalamove_Click" Width="110px" CssClass="mt-2 mg-Lala" ImageUrl="~/Icon/Lalamove.png" />
            </div>
            <div class="col-6">
                <asp:ImageButton runat="server" ID="btnNotify" OnClick="btnNotifications_Click" Width="180px" CssClass="img-mt--12px" ImageUrl="~/Icon/TrackingFlashExpress.png" />
            </div>
        </div>
    </div>

</asp:Content>
