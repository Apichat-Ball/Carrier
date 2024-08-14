<%@ Page Language="C#" Title="Flash Import" AutoEventWireup="true" CodeBehind="Flash_Import.aspx.cs" Inherits="Carrier.Flash_Import" MasterPageFile="~/Site.Master" %>


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
                        <asp:Label runat="server" ID="lbpno" Text="เลขที่พัสดุ" CssClass="form-label"></asp:Label>
                        <asp:TextBox runat="server" ID="txtPno" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-1">
                        <br />
                        <asp:Button runat="server" ID="btnSearch" Text="ค้นหา" OnClick="btnSearch_Click" CssClass="btn btn-outline-primary w-100" />
                    </div>
                </div>
                <div  class="row">
                    <div class="col-md-4">
                        <span>Import From Flash</span>
                        <asp:FileUpload runat="server" ID="fileupload1" />

                        <asp:Button runat="server" ID="btnRun" Text="Import" CssClass="btn btn-outline-primary" OnClick="btnRun_Click" />
                    </div>
                    <div class="col-md-3">
                        <asp:Button runat="server" ID="btnExport" Text="Export SAP" CssClass="btn btn-outline-primary" OnClick="btnExport_Click" />
                    </div>
                </div>
            </div>
            <div runat="server" id="dv_gv_import_Check" visible="false">
                <div class="row">
                    <div class="col">
                        <asp:GridView runat="server" ID="gv_Import" EmptyDataText="ไม่พบข้อมูลในช่วงเวลานี้ครับ" AutoGenerateColumns="false" CssClass="table table-borderless table-hover"
                            HeaderStyle-CssClass="fixedHeader">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate> 
                                        <span>
                                            วันที่ทำรายการ
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbDocno_Match" Text='<%# Bind("Docno_Match") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lbDateProcess" Text='<%# Bind("DateProcess") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>
                                            รหัสพัสดุ
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbPno" Text='<%# Bind("Pno") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>
                                            เลขที่เอกสาร
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbDocno" Text='<%# Bind("Docno") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>
                                            ค่ารถ
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbPrice" Text='<%# Bind("Price") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>
                                            Shop
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" id="lbSiteStorage" Text='<%# Bind("sitestorage") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>
                                            แผนก
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbDepartment_ID" Text='<%# Bind("Department_ID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <HeaderTemplate>
                                        <span>
                                            เลขที่เอกสารเบิกค่าใช้จ่าย
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbDocnoInBud" ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>
                                            
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Image runat="server" ID="imgCheck" Width="30" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--<asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>จากระบบ</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbFrom" Text='<%# Bind("From") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
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
