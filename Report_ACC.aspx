<%@ Page Title="Carrier" Language="C#" AutoEventWireup="true" CodeBehind="Report_ACC.aspx.cs" MasterPageFile="~/Site.Master" Inherits="Carrier.Report_ACC" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
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
        <asp:Label runat="server" ID="lbForm" Text="Transportation Report(ACC)" CssClass="h1"></asp:Label>
        <asp:Label runat="server" ID="lbuserid" Visible="false"></asp:Label>
    </div>
    <div class="mt-2">
        <asp:Label runat="server" ID="lbFirstLoad" Text="first" Visible="false"></asp:Label>
        <div class="row">
            <div class="col-sm-3">
                <div class="row " style="margin-bottom: 10px;">
                    <div class="col-sm-2 w-100 input-group mb-2 ">
                        <asp:Label runat="server" ID="lbDateStart" Text="เริ่มวันที่ " CssClass="input-group-text ml-2">
                            <asp:TextBox runat="server" ID="txtDateStart" CssClass="ml-2 form-control" BackColor="White" AutoCompleteType="Disabled"></asp:TextBox>
                        </asp:Label>

                    </div>
                </div>
            </div>
            <div class="col-sm-3">
                <div class="row " style="margin-bottom: 10px;">
                    <div class="col-sm-2 w-100 input-group mb-2 ">
                        <asp:Label runat="server" ID="lbDateEnd" Text="สิ้นสุดวันที่" CssClass="input-group-text ml-2">
                            <asp:TextBox runat="server" ID="txtDateEnd" CssClass="ml-2 form-control" BackColor="White" AutoCompleteType="Disabled"></asp:TextBox>
                        </asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-sm-2">
                <div class="row " style="margin-bottom: 10px;">
                    <div class="col-sm-2 w-100 input-group" style="margin-top: 9px;">
                        <asp:Button runat="server" ID="btnSearch" Text="SEARCH" OnClick="btnSearch_Click" CssClass="btn w-100 btn-primary " />
                    </div>
                </div>
            </div>
            <div class="col-sm-2">
                <div class="row " style="margin-bottom: 10px;">
                    <div class="col-sm-2 w-100 input-group" style="margin-top: 9px;">
                        <asp:Button runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" CssClass="btn w-100 btn-primary " Visible="false"/>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <asp:GridView runat="server" ID="gv_Report" AutoGenerateColumns="false" EmptyDataText="ไม่มีรายการ" CssClass="table table-striped table-bordered table-hover" HeaderStyle-HorizontalAlign="Center">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label runat="server" ID="lbhDocno" Text="เลขที่เอกสาร"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lkbDocno" Text='<%# Bind("Docno") %>'></asp:Label>
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
                    <asp:TemplateField ControlStyle-Width="200px">
                        <HeaderTemplate>
                            <asp:Label runat="server" ID="lbhBrand" Text="Brand"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lbBrand" Text='<%# Bind("Brand") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="gj-text-align-center" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label runat="server" ID="lbhBrandShort" Text="BrandShort"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lbBrandShort"></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="gj-text-align-center" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label runat="server" ID="lbhSaleOn" Text="Online/Offline"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lbSaleOn" Text='<%# Bind("SaleOn") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ControlStyle-Width="220px">
                        <HeaderTemplate>
                            <asp:Label runat="server" ID="lbhsrcName" Text="ชื่อผู้ส่ง"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lbsrcName" Text='<%# Bind("srcName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ControlStyle-Width="220px">
                        <HeaderTemplate>
                            <asp:Label runat="server" ID="lbhdstName" Text="ชื่อผู้รับ"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lbdstName" Text='<%# Bind("dstName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ControlStyle-Width="120px">
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
                            <asp:Label runat="server" ID="lbhDateCreate" Text="Date Create"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lbDateCreate" Text='<%# Bind("dateCreate") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="gj-text-align-center" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label runat="server" ID="lbhTrackingPickup" Text="Tracking Picup ID"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lbStatus" Text='<%# Bind("TrackingPickup") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ControlStyle-Width="100px">
                        <HeaderTemplate>
                            <asp:Label runat="server" ID="lbhComcode" Text="ComCode"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lbComcode"></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="gj-text-align-center" />
                    </asp:TemplateField>
                    <asp:TemplateField ControlStyle-Width="100px">
                        <HeaderTemplate>
                            <asp:Label runat="server" ID="lbhProfit" Text="Profit"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lbProfit"></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="gj-text-align-center" />
                    </asp:TemplateField>
                    <asp:TemplateField ControlStyle-Width="100px">
                        <HeaderTemplate>
                            <asp:Label runat="server" ID="lbhCostCenter" Text="CostCenter"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lbCostCenter"></asp:Label>
                        </ItemTemplate>
                        <ItemStyle CssClass="gj-text-align-center" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <div aria-label="nav Page navigation example">
        <div class="btn-group" role="group" aria-label="Basic outlined example">
            <asp:LinkButton runat="server" ID="lkPrevious"  CssClass="btn btn-outline-primary " OnCommand="selectPage" Visible="false" >PREVIOUS</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lkFirst"  CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false"></asp:LinkButton>
            <asp:LinkButton runat="server" ID="lk1"  CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false"></asp:LinkButton>
            <asp:LinkButton runat="server" ID="lk2"  CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false"></asp:LinkButton>
            <asp:LinkButton runat="server" ID="lk3"  CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false"></asp:LinkButton>
            <asp:LinkButton runat="server" ID="lkLast"  CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false"></asp:LinkButton>
            <asp:LinkButton runat="server" ID="lkNext"  CssClass="btn btn-outline-primary" OnCommand="selectPage" Visible="false" >NEXT</asp:LinkButton>
        </div>
    </div>
    </div>
</asp:Content>
