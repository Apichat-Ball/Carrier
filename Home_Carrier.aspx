<%@ Page Title="Courier" Language="C#" AutoEventWireup="true" CodeBehind="Home_Carrier.aspx.cs" Inherits="Carrier.Home_Carrier" MasterPageFile="~/Site.Master" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <style>
        .s-15px {
            font-size: 15px;
        }

        .status-tracking {
            border-radius: 7px;
            padding: 3px;
        }
        .rainbow {
            text-align: center;
            font-size: 22px;
            font-family:LilyUPC;
/*            font-family: monospace;*/
/*            letter-spacing: 1px;*/
            animation: colorRotate 2s linear 0s infinite;
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
    <div style="margin-top:80px;">
        <asp:Label runat="server" ID="lbForm" Text="Transportation Outsource" CssClass="h1"></asp:Label>
        <asp:Label runat="server" ID="lbuserid" Visible="false"></asp:Label>
        <div class="float-end mt-3">
            <asp:Button runat="server" ID="btnCreateOrder" Text="Create Order" CssClass=" btn btn-primary s-15px" Width="100%" OnClick="btnCreateOrder_Click" />
        </div>
    </div>
    <div class="row col-12" runat="server" id="dv_Comment" >
        <asp:Label runat="server" ID="lbUpdateStatusComment" Text="ประกาศจากระบบ" CssClass="h3"></asp:Label><br />
        <div class="row col-12 overflow-auto" style="height:80px">
            <asp:GridView runat="server" ID="gv_UpdateComment" ShowHeader="false" AutoGenerateColumns="false" CssClass="border-0 table-sm">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbStatusComment" CssClass="status-tracking" Text='<%# Bind("CM_Status") %>'></asp:Label>
                        <asp:Label runat="server" ID="lbDateCreate" Text='<%# Bind("CM_DateCreate") %>'></asp:Label>
                        <asp:Label runat="server" ID="lbComment" Text='<%# Bind("CM_Message") %>'></asp:Label>
                        <asp:Label runat="server"  ID="lbUrlFile" Text='<%# Bind("CM_Url") %>' Visible="false"></asp:Label>
                        <asp:ImageButton runat="server" ID="imgGuie" ImageUrl="~/Icon/pdf.png" Width="20px"  Visible="false" />
                        <asp:label runat="server" id="lbNew" Text="NEW!"  CssClass="rainbow" visible="false"></asp:label>
                    </ItemTemplate>
                    <ItemStyle CssClass="mb-2 small" Height="20px" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView> 
        </div>
    </div>
    <div class="mt-1 ">
        <div class="row">
            <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                <div class=" input-group">
                    <asp:Label runat="server" ID="lbDocnoSearch" Text="เลขที่เอกสาร" CssClass="input-group-text"></asp:Label>
                    <asp:TextBox runat="server" ID="txtDocnoSearch" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                <div class="input-group">
                    <asp:Label runat="server" ID="lbPnoSearch" Text="รหัสพัสดุ" CssClass="input-group-text"></asp:Label>
                    <asp:TextBox runat="server" ID="txtPnoSearch" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                <div class="input-group">
                    <asp:Label runat="server" ID="lbDstNameSearch" Text="ชื่อผู้รับ" CssClass="input-group-text"></asp:Label>
                    <asp:TextBox runat="server" ID="txtDstNameSearch" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                <div class=" align-self-center">
                    <div class="input-group">
                        <asp:Label runat="server" ID="lbArticleSearch" Text="ประเภทพัสดุ" CssClass="input-group-text"></asp:Label>
                        <asp:TextBox runat="server" ID="txtArticleSearch" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                <div class=" input-group">
                    <asp:Label runat="server" ID="lbDateStart" Text="เริ่มวันที่ " CssClass="input-group-text ">
                        <asp:TextBox runat="server" ID="txtDateStart" CssClass="form-control" BackColor="White"></asp:TextBox></asp:Label>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidatorDateStart" runat="server" ControlToValidate="txtDateStart" ErrorMessage="กรุณาเลือกวันที่เริ่มในการค้นหา" ValidationExpression="^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                </div>
            </div>
            <div class="col-sm-12 col-md-6 col-lg-4 col-xl-3 gy-3">
                <div class=" input-group">
                    <asp:Label runat="server" ID="lbDateEnd" Text="สิ้นสุดวันที่" CssClass="input-group-text">
                        <asp:TextBox runat="server" ID="txtDateEnd" CssClass="form-control" BackColor="White"></asp:TextBox></asp:Label>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidatorDateEnd" runat="server" ControlToValidate="txtDateEnd" ErrorMessage="กรุณาเลือกวันที่สุดท้ายในการค้นหา" ValidationExpression="^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>

                </div>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-2 col-xl-1 gy-3">
                    <asp:Button runat="server" ID="btnSearch" Text="SEARCH" CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
                    <asp:Label runat="server" ID="lbstatusSearch" Text="First" Visible="false"></asp:Label>
            </div>
            <div class="col-sm-12 col-md-12 col-lg-2 col-xl-1 gy-3">
                    <asp:Button runat="server" ID="btnClear" Text="CLEAR" CssClass="btn btn-primary w-100" OnClick="btnClear_Click" Visible="false" />
            </div>
        </div>

    </div>
    <div class="row mt-3 overflow-auto">
        <asp:GridView runat="server" ID="gv_OrderAll" Width="1300px" EmptyDataText="ไม่มีการสร้างรายการ" AutoGenerateColumns="false" CssClass="table table-striped table-bordered table-hover small"
            HeaderStyle-HorizontalAlign="Center">
            <Columns>
                <asp:TemplateField ControlStyle-CssClass="ml-1">
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbhDocno" Text="เลขที่เอกสาร"></asp:Label><br />
                        <asp:Label runat="server" ID="lbhpno" Text="รหัสพัสดุ"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lkDocno" Text='<%# Bind("Docno") %>' OnClick="lkDocno_Click"></asp:LinkButton><br />
                        <asp:Label runat="server" ID="lbpno" Text='<%# Bind("pno") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label runat="server" ID="lbhSRC" Text="จุดรับ"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbSRC" Text='<%# Bind("TypesendKO") %>'></asp:Label>
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
                        <asp:Label runat="server" ID="lbhTrackingPickup" Text="Tracking Picup ID"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbStatus" Text='<%# Bind("TrackingPickup") %>'></asp:Label>
                        <asp:Label runat="server" ID="lbStatusItem" Text='<%# Bind("status") %>' Visible="false"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle CssClass="gj-text-align-center" />
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
                        <%--<asp:ImageButton runat="server" ID="imgbtnEditOrder" Width="30px" ImageUrl="~/Icon/edit.png" OnClick="imgbtnEditOrder_Click" />--%>
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

</asp:Content>
