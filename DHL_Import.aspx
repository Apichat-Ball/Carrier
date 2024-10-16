<%@ Page Title="DHL" Language="C#" AutoEventWireup="true" CodeBehind="DHL_Import.aspx.cs" Inherits="Carrier.DHL_Import" MasterPageFile="~/Site.Master" %>


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

        
    </script>

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

        .fixedHeader {
            position: sticky;
            top: 65px;
            background-color: #f9f9f9;
            z-index: 10;
        }

        .maxTable {
            min-width: 1370px;
        }
    </style>
    <asp:Label runat="server" ID="lbuserID" Visible="false"></asp:Label>
    <asp:UpdatePanel ID="updatePanel1" runat="server">
        <ContentTemplate>
            <div runat="server" id="dv_main" style="margin-top: 80px;">
                <div class="row">
                    <div class="col-12">
                        <h>Report DHL</h>
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
                        <asp:Label runat="server" ID="lbpno" Text="เลขที่พัสดุ" CssClass="form-label"></asp:Label>
                        <asp:TextBox runat="server" ID="txtPno" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-1">
                        <br />
                        <asp:Button runat="server" ID="btnSearch" Text="ค้นหา" CssClass="btn btn-outline-primary w-100" UseSubmitBehavior="false" OnClick="btnSearch_Click" />
                    </div>
                </div>
                <div  class="row">
                    <div class="col-md-4">
                        <span>Import From Flash</span>
                        <asp:FileUpload runat="server" ID="fileupload1" />

                        <asp:Button runat="server" ID="btnRun" Text="Import" CssClass="btn btn-outline-primary" OnClick="btnRun_Click" UseSubmitBehavior="false"/>
                    </div>
                    <div class="col-md-2">
                        <asp:Button runat="server" ID="btnExport" Text="Export SAP" CssClass="btn btn-outline-primary" OnClick="btnExport_Click" UseSubmitBehavior="false"/>
                    </div>
                    <div class="col-md-3">
                        <asp:Button runat="server" ID="btnUploadToBudget" Text="Upload to Budget" OnClick="btnUploadToBudget_Click" CssClass="btn btn-outline-primary" UseSubmitBehavior="false"/>
                    </div>
                    <div class="col-md-3">
                        <asp:Button runat="server" ID="btnRejectUploadBud" Visible="false" Text="Reject" OnClick="btnRejectUploadBud_Click" CssClass="btn btn-outline-primary"  UseSubmitBehavior="false"/>
                    </div>
                </div>
            </div>
            <div runat="server" id="dv_gv_import_Check" visible="false">
                <div class="row">
                    <div class="col">
                        <asp:GridView runat="server" ID="gv_Import" EmptyDataText="ไม่พบข้อมูลในช่วงเวลานี้ครับ" AutoGenerateColumns="false"  CssClass="table table-borderless table-hover"
                            HeaderStyle-CssClass="fixedHeader">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>วันที่จัดส่ง</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbDateProcess" Text='<%# Bind("DateProcess") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>TrackingNo</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbTrackingNo" Text='<%# Bind("TrackingNo") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>เลขที่เอกสารใน Whale</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbDocno" Text='<%# Bind("Docno") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>ราคา</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbPrice" Text='<%# Bind("Price") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>Shop</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbShop" Text='<%# Bind("sitestorage") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>Brand</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbDepartment" Text='<%# Bind("Department") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>เลขที่เอกสาร Budget</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbDocBud" Text='<%# Bind("Docno_Bud") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
            </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnRun" />
            <asp:PostBackTrigger ControlID="btnExport" />
        </Triggers>
        </asp:UpdatePanel>
</asp:Content>
