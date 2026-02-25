<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderFromOutEmployee.aspx.cs" Inherits="Carrier.OrderFromOutEmployee" MasterPageFile="~/Site.Master" %>


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
                })

        }
    </script>
    <style>
        .fixedHeader {
            position: sticky;
            top: 0;
            background-color: #6c757d;
            z-index: 10;
            color: #fff;
        }
    </style>
    <div style="margin-top: 80px;">

        <asp:Label runat="server" ID="lbForm" Text="Transportation From Out Employee" CssClass="h1"></asp:Label>
        <asp:Label runat="server" ID="lbuserid" Visible="false"></asp:Label>

    </div>
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
        <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
            <div class=" input-group">
                <asp:Label runat="server" ID="lbDocnoSearch" Text="เลขที่เอกสาร" CssClass="input-group-text small"></asp:Label>
                <asp:TextBox runat="server" ID="txtDocnoSearch" CssClass="form-control small"></asp:TextBox>
            </div>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
            <div class=" input-group">
                <asp:Label runat="server" ID="lbUserS" Text="พนักงาน" CssClass="input-group-text small"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlUserS" CssClass="form-control small" DataValueField="userid" DataTextField="username" AutoPostBack="true" OnSelectedIndexChanged="ddlUserS_SelectedIndexChanged"></asp:DropDownList>
            </div>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
            <asp:Button runat="server" ID="btnSearch" Text="ค้นหา" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
        </div>
    </div>

    <div class="my-2">
        <asp:UpdatePanel runat="server" ID="updatePanel6">
            <ContentTemplate>
                <asp:GridView ID="gv_data" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-hover table-bordered text-center align-middle mb-0 small table-sm"
                    EmptyDataText="ไม่พบข้อมูล"
                    OnRowCommand="gv_data_RowCommand"
                    Style="table-layout: fixed;">

                    <%--<HeaderStyle BackColor="#343a40" ForeColor="White" CssClass="fixedHeader" />
                    <RowStyle BackColor="#2a2d38" ForeColor="#e5e5e5" />--%>
                    <HeaderStyle CssClass="fixedHeader" />
                    <%--<AlternatingRowStyle BackColor="#3a3d47" />--%>

                    <Columns>
                        <asp:TemplateField HeaderText="#" HeaderStyle-Width="100px">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lkbRecTransaction" Text='<%# Eval("BFID") %>' CommandArgument='<%# Eval("BFID") %>' CommandName="Open"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="ผู้สร้าง" DataField="UserCreate" HeaderStyle-Width="170px" />
                        <asp:BoundField HeaderText="Brand" DataField="Brand" HeaderStyle-Width="130px" />
                        <asp:BoundField HeaderText="ชื่อผู้ส่ง" DataField="NameSend" HeaderStyle-Width="200px" />
                        <asp:BoundField HeaderText="ที่อยู่ผู้รับ" DataField="AddressSend" HeaderStyle-Width="300px" />
                        <asp:BoundField HeaderText="ชื่อผู้รับ" DataField="NameRecieve" HeaderStyle-Width="200px" />
                        <asp:BoundField HeaderText="ที่อยู่ผู้รับ" DataField="AddressRecieve" HeaderStyle-Width="300px" />
                        <asp:BoundField HeaderText="วันที่สร้าง" DataField="DateCreate" HeaderStyle-Width="100px" />
                        <asp:BoundField HeaderText="รูปแบบจัดส่ง" DataField="TypeSend" HeaderStyle-Width="100px" />
                        <asp:BoundField HeaderText="สถานะจัดส่ง" DataField="Status" HeaderStyle-Width="200px" />
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>


</asp:Content>
