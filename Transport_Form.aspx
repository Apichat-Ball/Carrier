<%@ Page Title="Carrier" Language="C#" AutoEventWireup="true" CodeBehind="Transport_Form.aspx.cs" MasterPageFile="~/Site.Master" Inherits="Carrier.Transport_Form" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <style>
        .s-15px {
            font-size: 15px;
        }

        .drop-grey {
            background-color: darkgrey;
        }
    </style>
    <%--<script type="text/javascript">
        $(function(){
            $("#<%= txtSiteStorage.ClientID %>").keypress(
        });
    </script>--%>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
    <asp:UpdatePanel ID="updatePanel1" runat="server">
        <ContentTemplate>
            <div class="mt-5">
                <div style="position: absolute; left: 50%;" class="float-end">
                    <asp:Button runat="server" ID="btnPrint" Text="พิมพ์ใบปะหน้ากล่อง" Visible="false" CssClass="btn btn-primary" OnClick="btnPrint_Click" />
                </div>
                <div style="position: absolute; left: 80%;" class="float-end">
                    <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-primary " Text="กลับหน้าหลัก" OnClick="btnCancel_Click" />
                </div>
                <asp:Label runat="server" ID="lbForm" Text="Transportation" CssClass="h1"></asp:Label>
            </div>
            <div class="row mt-5 mb-3">
                <div class="col-sm-6">
                    <div class="row " style="margin-bottom: 10px; display: none;">
                        <div class="col-sm-2 w-100 input-group mb-2 ">
                            <asp:Label runat="server" ID="lbWareHouseNo" Text="รหัสคลังสินค้า" CssClass=" input-group-text s-15px shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtWareHouse" Width="100%" CssClass="form-control s-15px shadow"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2 w-100 input-group mb-2">
                            <asp:Label runat="server" ID="lbDocno" Text="เลขที่เอกสาร" CssClass=" input-group-text s-15px shadow" Visible="false"></asp:Label>
                            <asp:TextBox runat="server" ID="txtDocno" Width="100%" CssClass="form-control s-15px shadow" Enabled="false" Visible="false"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row ">
                        <div class="col-sm-2 w-100 input-group mb-2" runat="server">
                            <asp:Label runat="server" ID="lbTrackingID" Text="เลขพัสดุ" CssClass=" input-group-text s-15px shadow" Visible="false"></asp:Label>
                            <asp:TextBox runat="server" ID="txtTrackingID" Width="100%" CssClass="form-control s-15px shadow" Enabled="false" Visible="false"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2 w-100 input-group mb-2 ">
                            <asp:Label runat="server" ID="lbFavorites" Text="Favorites" CssClass="input-group-text s-15px shadow" Visible="false"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlFavorites" CssClass="btn dropdown-toggle s-15px shadow" Visible="false" DataValueField="val" DataTextField="text" OnSelectedIndexChanged="ddlFavorites_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                        </div>
                    </div>


                    <asp:Label runat="server" ID="lbgroupSend" Text="ผู้ส่ง" CssClass=" s-15px "></asp:Label>
                    <div class="border border-1 " style="border-radius: 15px; padding: 20px; width: 80%">
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbsrcName" Text="ชื่อ" CssClass=" input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtsrcName" Width="100%" CssClass="form-control s-15px shadow" AutoCompleteType="Disabled"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbsrcPhone" Text="เบอร์โทรศัพท์" CssClass=" input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtsrcPhone" Width="100%" CssClass="form-control s-15px shadow " MaxLength="10" TextMode="Phone" AutoCompleteType="Disabled"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtsrcPhone" ErrorMessage="กรุณากรอกเบอร์โทรศัพท์ให้ถูกต้อง" ValidationExpression="[0]{1}(([6,8,9]{1}[0-9]{8})|([2]{1}[0-9]{7}))" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbsrcProvinceName" Text="จังหวัด" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlsrcProvinceName" Width="100%" CssClass="btn   dropdown-toggle s-15px shadow" DataValueField="Province_ID" DataTextField="Province_Name" OnSelectedIndexChanged="ddlsrcProvinceName_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbsrcCityName" Text="อำเภอ" CssClass=" input-group-text s-15px shadow"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlsrcCityName" Width="100%" CssClass="btn   dropdown-toggle s-15px shadow" DataValueField="City_ID" DataTextField="City_Name" OnSelectedIndexChanged="ddlsrcCityName_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbsrcDistrictName" Text="ตำบล" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlsrcDistrictName" Width="100%" CssClass="btn   dropdown-toggle s-15px shadow" DataValueField="Distinct_ID" DataTextField="Distinct_Name" OnSelectedIndexChanged="ddlsrcDistrictName_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbsrcPostalCode" Text="รหัสไปรษณีย์" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtsrcPostalCode" Width="100%" CssClass="form-control s-15px shadow" MaxLength="5" AutoCompleteType="Disabled"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtsrcPostalCode" ErrorMessage="กรุณากรอกรหัสไปรษณีย์ให้ถูกต้อง" ValidationExpression="[0-9]{5}" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtsrcPostalCode" Display="Static" ErrorMessage="*" runat="server" ForeColor="Red" CssClass="ml-1" />--%>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbsrcDetailAddress" Text="รายละเอียดที่อยู่" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtsrcDetailAddress" Width="100%" CssClass="form-control s-15px shadow" AutoCompleteType="Disabled" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="mt-2 input-group mb-2 ">
                        <asp:Label runat="server" ID="lbarticleCategory" Text="ประเภทสินค้า" CssClass="input-group-text s-15px shadow"></asp:Label>
                        <asp:DropDownList runat="server" ID="ddlarticleCategory" CssClass="btn dropdown-toggle shadow" DataTextField="ArticleName" DataValueField="ArticleCode"></asp:DropDownList>
                    </div>
                    <div class="input-group mb-2 w-100">
                        <asp:Label runat="server" ID="lbremark" Text="หมายเหตุ" CssClass="input-group-text s-15px shadow"></asp:Label>
                        <asp:TextBox runat="server" ID="txtremark" CssClass="form-control s-15px shadow" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                </div>
                <div class="col-sm-6">
                    <div class="row " style="margin-bottom: 10px;">
                        <div class="col-sm-2 w-50 input-group mb-2 ">
                            <asp:Label runat="server" ID="lbExpress" Text="รูปแบบการจัดส่ง" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlExpress" CssClass="btn   dropdown-toggle s-15px shadow" DataTextField="text" DataValueField="val" Enabled="false" Visible="false"></asp:DropDownList>
                            <asp:TextBox runat="server" Text="Flash Express" Enabled="false" CssClass="form-control s-15px shadow text-center"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row " style="margin-bottom: 10px;">
                        <div class="col-sm-2 w-100 input-group mb-2 ">
                            <asp:Label runat="server" ID="Label1" Text="รูปแบบการขายสินค้า" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:RadioButton runat="server" ID="radioWorkOn" Text="ONLINE" GroupName="SaleOn" CssClass="ml-4 mt-2 custom-radio" />
                            <asp:RadioButton runat="server" ID="radioWorkOff" Text="OFFLINE" GroupName="SaleOn" CssClass="ml-4 mt-2 custom-radio " />
                        </div>
                    </div>
                    <div class="row " style="margin-bottom: 10px;">
                        <div class="col-sm-2 w-100 input-group mb-2 ">
                            <asp:Label runat="server" ID="Label4" Text="เบิกเพื่อใช้ในแผนก" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:DropDownList ID="ddlSDpart" runat="server" DataTextField="department_" DataValueField="departmentID" CssClass="btn text-start s-15px shadow "></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2 w-100 input-group mb-2 " runat="server" id="div_Receive" visible="false">
                            <asp:Label runat="server" ID="lbReceiveLocation" Text="ปลายทาง" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlReceiveLocation" CssClass="btn dropdown-toggle s-15px shadow" DataValueField="val" DataTextField="text" OnSelectedIndexChanged="ddlReceiveLocation_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divSite" visible="false" style="margin-bottom: 10px;">
                        <div class="col-sm-2 w-100 input-group mb-2 ">
                            <asp:Label runat="server" ID="lbSite" Text="Site Storage" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtSiteStorage" CssClass="form-control s-15px shadow" OnTextChanged="txtSiteStorage_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                    </div>
                    <asp:Label runat="server" ID="Label2" Text="ผู้รับ" CssClass="s-15px"></asp:Label>
                    <div class="border border-1 " style="border-radius: 15px; padding: 20px; width: 80%">
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbdstName" Text="ชื่อ" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtdstName" Width="100%" CssClass="form-control s-15px shadow" AutoCompleteType="Disabled"></asp:TextBox>
                                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtdstName" Display="Static" ErrorMessage="*" runat="server" ForeColor="Red" CssClass="ml-1" />--%>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbdstPhone" Text="เบอร์โทรศัพท์มือถือ" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtdstPhone" Width="100%" CssClass="form-control s-15px shadow" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtdstPhone" ErrorMessage="กรุณากรอกเบอร์โทรศัพท์ให้ถูกต้อง" ValidationExpression="([0]{1}[6,8,9]{1}[0-9]{8})|([-]{1})" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                                <%--                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtdstPhone" Display="Static" ErrorMessage="*" runat="server" ForeColor="Red" CssClass="ml-1" />--%>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbdstHomePhone" Text="เบอร์โทรศัพท์บ้าน" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtdstHomePhone" Width="100%" CssClass="form-control s-15px shadow" MaxLength="9" AutoCompleteType="Disabled"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtdstHomePhone" ErrorMessage="กรุณากรอกเบอร์โทรศัพท์บ้านให้ถูกต้อง" ValidationExpression="([0]{1}[2]{1}[0-9]{7})|([-]{1})" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbdstProvinceName" Text="จังหวัด " CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddldstProvinceName" Width="100%" CssClass="btn   dropdown-toggle s-15px shadow" DataValueField="Province_ID" DataTextField="Province_Name" OnSelectedIndexChanged="ddldstProvinceName_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbdstCityName" Text="อำเภอ" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddldstCityName" Width="100%" CssClass="btn   dropdown-toggle s-15px shadow" DataValueField="City_ID" DataTextField="City_Name" OnSelectedIndexChanged="ddldstCityName_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbdstDistrictName" Text="ตำบล" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:DropDownList runat="server" ID="ddldstDistrictName" Width="100%" CssClass="btn   dropdown-toggle s-15px shadow" DataValueField="Distinct_ID" DataTextField="Distinct_Name" OnSelectedIndexChanged="ddldstDistrictName_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbdstPostalCode" Text="รหัสไปรษณีย์" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtdstPostalCode" Width="100%" CssClass="form-control s-15px shadow" AutoCompleteType="Disabled"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="txtdstPostalCode" ErrorMessage="กรุณากรอกเบอร์โทรศัพท์ให้ถูกต้อง" ValidationExpression="[0-9]{5}" Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator6" ControlToValidate="txtdstPostalCode" Display="Static" ErrorMessage="*" runat="server" ForeColor="Red" CssClass="ml-1"  />--%>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2 w-100 input-group mb-2">
                                <asp:Label runat="server" ID="lbdstDetailAddress" Text="รายละเอียดที่อยู่" CssClass="input-group-text s-15px shadow"></asp:Label>
                                <asp:TextBox runat="server" ID="txtdstDetailAddress" Width="100%" CssClass="form-control s-15px shadow" AutoCompleteType="Disabled" TextMode="MultiLine" Rows="3"></asp:TextBox>

                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div style="display: flex;" runat="server" id="div_Box" class="mt-3">
                    <div class="col-sm-3 w-50  ">
                        <div class="row w-100 input-group mb-2">
                            <asp:Label runat="server" ID="Label3" Text="ประเภทกล่อง" Width="40%" CssClass="input-group-text s-15px shadow"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlBox" Width="60%" CssClass="btn dropdown-toggle s-15px shadow" DataTextField="Box_Name" DataValueField="Box_ID"></asp:DropDownList>
                        </div>
                        <div class="row w-100 input-group mb-2">
                            <asp:Label runat="server" ID="Label5" Text="จำนวน" Width="40%" CssClass=" input-group-text s-15px shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtQty" Width="60%" CssClass="form-control shadow text-end" TextMode="Number" ValidateRequestMode="Enabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-sm-1  w-50 input-group mb-2">
                        <asp:Button runat="server" ID="btnAdd" Text="ADD" Width="100%" Height="85px" CssClass="text-center btn btn-success s-15px" OnClick="btnAdd_Click" />
                    </div>
                    <div style="display: flex; justify-content: center;" runat="server" id="div_tb_Box" class="col-sm-3 w-100">
                        <asp:GridView runat="server" ID="gv_Box" CssClass="table table-striped table-bordered table-hover small bg-gradient " AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label runat="server" ID="lbhBox_Name" Text="ขนาดกล่อง"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbBox_ID" Text='<%# Bind("Box_ID") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lbBox_Name" Text='<%# Bind("Box_Name") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label runat="server" ID="lbhQty" Text="จำนวน"></asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbQty" Text='<%# Bind("Qty") %>' Visible="false"></asp:Label>
                                        <asp:TextBox runat="server" ID="txtQty" Text='<%# Bind("Qty") %>' CssClass="text-end" Width="50px"></asp:TextBox>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="text-end" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button runat="server" ID="btnDeleteBoxItem" OnClick="btnDeleteBoxItem_Click" CssClass="btn-close" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                    </div>
                </div>
            </div>
                

            <div class="row">

                <div style="display: flex; justify-content: center;" class="mt-3">
                    <asp:Button runat="server" ID="btnSave" Text="SAVE" CssClass="text-center btn btn-success s-15px" Width="100%" OnClick="btnSave_Click" />
                </div>
                <div style="display: none;">
                    <div class="col-sm-2 input-group mb-2 w-25">
                        <asp:Label runat="server" ID="lbweight" Text="น้ำหนัก (กรัม)" CssClass="input-group-text s-15px shadow"></asp:Label>
                        <asp:TextBox runat="server" ID="txtweight" CssClass="form-control s-15px shadow" Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col-sm-2 input-group mb-2 w-25">
                        <asp:Label runat="server" ID="lbwidth" Text="ความกว้าง (เซนติเมตร)" CssClass="input-group-text s-15px shadow"></asp:Label>
                        <asp:TextBox runat="server" ID="txtwidth" CssClass="form-control s-15px shadow" Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col-sm-2 input-group mb-2 w-25">
                        <asp:Label runat="server" ID="lblength" Text="ความยาว (เซนติเมตร)" CssClass="input-group-text s-15px shadow"></asp:Label>
                        <asp:TextBox runat="server" ID="txtlength" CssClass="form-control s-15px shadow" Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col-sm-2 input-group mb-2 w-25">
                        <asp:Label runat="server" ID="lbheight" Text="ความสูง (เซนติเมตร)" CssClass="input-group-text s-15px shadow"></asp:Label>
                        <asp:TextBox runat="server" ID="txtheight" CssClass="form-control s-15px shadow" Enabled="false"></asp:TextBox>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSave" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
