<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Carrier._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .s-15px{
            font-size: 15px;
        }
    </style>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
    <div class="mt-3">
        
        <asp:Label runat="server" ID="lbForm" Text="Transportation(Admin)" CssClass="h1"></asp:Label>
        <asp:Label runat="server" ID="lbuserid" Visible="false"></asp:Label>
        <%--<div style="position: absolute; left: 80%;" class="float-end mt-3">--%>
        <div class="float-end mt-3">
            <asp:Button runat="server" ID="btnCreateOrder" Text="Create Order" CssClass="btn btn-primary s-15px" Width="100%" Height="30px" OnClick="btnCreateOrder_Click" />
        </div>
    </div>
    <div class="row mt-2">
        <asp:GridView runat="server" ID="gv_OrderAll" Width="100%" AutoGenerateColumns="false" CssClass="table-hover">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:CheckBox runat="server" ID="cbAll" ToolTip="เลือกทั้งหมด" CssClass="m-2" OnCheckedChanged="cbAll_CheckedChanged" AutoPostBack="true" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox runat="server" ID="cbItem" CssClass="m-2" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
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
            </Columns>
        </asp:GridView>
    </div>
    <div class ="row">
        <div >
            <asp:ImageButton runat="server" id="btnNotify" OnClick="btnNotifications_Click" Width="50px" CssClass="float-end" ImageUrl="~/Icon/tracking.png"/>
        </div>
    </div>
</asp:Content>
