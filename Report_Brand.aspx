<%@ Page Title="Courier" Language="C#" AutoEventWireup="true" CodeBehind="Report_Brand.aspx.cs" Inherits="Carrier.Report_Brand" MasterPageFile="~/Site.Master" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
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
    <div class="mt-5">

        <asp:Label runat="server" ID="lbForm" Text="Report Brand" CssClass="h1"></asp:Label>
        <asp:Label runat="server" ID="lbuserid" Visible="false"></asp:Label>
    </div>
    <div class="mt-5 ">
        <div class="row">
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
           
            <div class="col-sm-3 input-group w-25">
                <asp:ImageButton runat="server" ID="imgbtnAdd" OnClick="imgbtnAdd_Click" ImageUrl="~/Icon/add.png" Width="40px" Height="40px" />
            </div>
        </div>
    </div>
    <asp:GridView runat="server" ID="gv_Head" AutoGenerateColumns="false" Width="100%" ShowHeader="false" CssClass="mt-3 border-0">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lbhSaleChannel" Text='<%# Bind("saleChannel") %>'  CssClass="h4 bold"></asp:Label>
                    <asp:GridView runat="server" ID="gv_Table" AutoGenerateColumns="false" Width="100%" CssClass="table table-striped table-bordered table-hover table-sm small " HeaderStyle-HorizontalAlign="Center"
                        DataSource=<%# Bind("saleBrand") %>> 
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label runat="server" ID="lbhDateSend" Text="วันที่"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbDateSend" Text='<%# Bind("Date_send") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label runat="server" ID="lbhSite" Text="SiteStorage"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbSite" Text='<%# Bind("siteStorage") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label runat="server" ID="lbhBrand" Text="Brand"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbBrand" Text='<%# Bind("brand") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label runat="server" ID="lbhQty" Text="จำนวน"></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbQty" Text='<%# Bind("qty") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
