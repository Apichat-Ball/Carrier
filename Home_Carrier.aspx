<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home_Carrier.aspx.cs" Inherits="Carrier.Home_Carrier" MasterPageFile="~/Site.Master" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <script type="text/javascript">
<%--        $(document).ready(function () {
            $('#<%= txtDateStart.ClientID %>').datepicker({
                uiLibrary: 'bootstrap4',
                format: 'dd/mm/yyyy'
            });
                $('#<%= txtDateEnd.ClientID%>').datepicker({
                    uiLibrary: 'bootstrap4',
                    format: 'dd/mm/yyyy'
                });
        }--%>

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
        <asp:Label runat="server" ID="lbForm" Text="Transportation" CssClass="h1"></asp:Label>
        <asp:Label runat="server" ID="lbuserid" Visible="false"></asp:Label>
         <div  class="float-end mt-3">
            <asp:Button runat="server" ID="btnCreateOrder" Text="Create Order" CssClass=" btn btn-primary " Width="100%" Height="30px" OnClick="btnCreateOrder_Click"/>
        </div>
    </div>
    <div class="mt-3 " >
        <div class="row">
            <div class="col-md-3">
                <asp:TextBox runat="server" ID="txtDateStart" CssClass="form-control" Width="100%"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:TextBox runat="server" ID="txtDateEnd" CssClass="form-control " Width="100%"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary " Text="Search" Width="100%" />
            </div>
        </div>

    </div>
    <div class="row mt-3">
        <asp:GridView runat="server" ID="gv_OrderAll" Width="100%" EmptyDataText="ไม่มีการสร้างรายการ" AutoGenerateColumns="false" CssClass="table-hover">
            <Columns>
                <asp:TemplateField ControlStyle-CssClass="ml-1">
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbhpno" Text="รหัสพัสดุ"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbDocno" Text='<%# Bind("Docno") %>' Visible="false" ></asp:Label>
                       <asp:LinkButton runat="server" ID="lkbpno" Text='<%# Bind("pno") %>' OnClick="lkbpno_Click"></asp:LinkButton>
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
                        <asp:label runat="server" id="lbhCategory" Text="ประเภทพัสดุ"></asp:label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbCategory" Text='<%# Bind("ArticleCategory") %>' ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbhDateCreate" Text="Date Create"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbDateCreate" Text='<%# Bind("dateCreate") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbhTrackingPickup" Text="Tracking Picup ID"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbStatus" Text='<%# Bind("TrackingPickup") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbhTimeTracking" Text="เวลารถมารับ"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbTimeTracking" Text='<%# Bind("TimeTracking") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbProcess" Text="การดำเนินการ"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:ImageButton runat="server" ID="imgbtnCancelOrder" Width="30px" ImageUrl="~/Icon/cancellation.png" OnClick="imgbtnCancelOrder_Click"/>
                    </ItemTemplate>
                    <ItemStyle CssClass="align-content-center" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>