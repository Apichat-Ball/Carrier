<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Edit_Add_Profit.aspx.cs" Inherits="Carrier.Edit_Add_Profit" MasterPageFile="~/Site.Master" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
    
    <asp:UpdatePanel runat="server" ID="update">
        <ContentTemplate>
            <div runat="server" id="div_main">
                <div style="margin-top: 80px;">
                    <asp:Label runat="server" ID="lbForm" Text="Edit&Add SiteStorage Profit" CssClass="h1"></asp:Label>
                    <asp:Label runat="server" ID="lbuserid" Visible="false"></asp:Label>
                </div>
                <div class="row justify-content-around mt-2 my-2">
                    <div class="col-sm-12 col-md-4 hide">
                        <asp:Label runat="server" ID="lbBrand" Text="Brand"></asp:Label>
                        <asp:DropDownList runat="server" ID="ddlBrandSearch" DataTextField="name" DataValueField="nameShot" AutoPostBack="true" OnSelectedIndexChanged="ddlBrand_SelectedIndexChanged" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="col-sm-12 col-md-3">
                        <asp:Label runat="server" ID="lbSite" Text="SiteStorage"></asp:Label>
                        <asp:TextBox runat="server" ID="txtSite" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-sm-12 col-md-5">
                        <br />
                        <asp:Button runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" CssClass="btn btn-primary" />
                    </div>
                </div>
                <asp:UpdateProgress runat="server" ID="UpProgress" >
                <ProgressTemplate>
                    <div class="gj-text-align-center">
                        <div class="spinner-border text-dark" role="status" aria-hidden="true">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
                <div class="row col-12 ">
                    <div class="mb-2">
                        <asp:ImageButton runat="server" ID="imgbtnAdd" ImageUrl="~/Icon/add.png" Width="30px" CssClass="float-end" OnClick="imgbtnAdd_Click" />
                    </div>
                    <div>
                        <asp:GridView runat="server" ID="gv_main" AutoGenerateColumns="false" CssClass="table table-bordered table-hover small table-sm" HeaderStyle-BackColor="DarkGray"
                            EmptyDataText="No Data">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>SiteStorage
                                        </span>
                                    </HeaderTemplate>

                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbSiteStorage" Text='<%# Bind("Site_Stroage") %>'></asp:Label>
                                        <asp:TextBox runat="server" ID="txtSiteStorage" Visible="false" CssClass="form-control" Width="110px"></asp:TextBox>
                                        
                                    </ItemTemplate>
                                    <ItemStyle Width="110px" CssClass="justify-content-center" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>Brand
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbBrandTemp" Text='<%# Bind("Brand") %>'></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlBrand" DataTextField="name" DataValueField="nameShot"  CssClass="form-control" Visible="false"></asp:DropDownList>
                                    </ItemTemplate>
                                    <ItemStyle Width="110px" CssClass="justify-content-center" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>Channel
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbChannel" Text='<%# Bind("Channel") %>'></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlChannel" Visible="false" CssClass="form-control">
                                            <asp:ListItem Text="--SELECT--" Value="--SELECT--"></asp:ListItem>
                                            <asp:ListItem Text="ONLINE" Value="ONLINE"></asp:ListItem>
                                            <asp:ListItem Text="OFFLINE" Value="OFFLINE"></asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <ItemStyle Width="110px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>Sale Channel
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbSaleChannel" Text='<%# Bind("Sale_Channel") %>'></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlSaleChannel" DataTextField="Sale_Channel" CssClass="form-control" DataValueField="Sale_Channel" Visible="false">
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <ItemStyle Width="130px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>COMCODE
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbComcode" Text='<%# Bind("COMCODE") %>'></asp:Label>
                                        <asp:TextBox runat="server" ID="txtComcode" Visible="false" CssClass="form-control"></asp:TextBox>
                                    </ItemTemplate>
                                    <ItemStyle Width="150px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>Profit
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbProfit" Text='<%# Bind("Profit") %>'></asp:Label>
                                        <asp:TextBox runat="server" ID="txtProfit" Visible="false" CssClass="form-control"></asp:TextBox>
                                    </ItemTemplate>
                                    <ItemStyle Width="150px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>Costcenter
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbCostcenter" Text='<%# Bind("Costcenter") %>'></asp:Label>
                                        <asp:TextBox runat="server" ID="txtCostcenter" Visible="false" CssClass="form-control"></asp:TextBox>
                                    </ItemTemplate>
                                    <ItemStyle Width="150px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <span>Process
                                        </span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" ID="imgbtnEdit" ImageUrl="~/Icon/edit.png" Width="30px" OnClick="imgbtnEdit_Click" />
                                        <asp:ImageButton runat="server" ID="imgbtnSave" ImageUrl="~/Icon/correct.png" Width="30px" Visible="false" OnClick="imgbtnSave_Click" />
                                        <asp:ImageButton runat="server" ID="imgbtnCancel" ImageUrl="~/Icon/cancellation.png" Width="30px" Visible="false" OnClick="imgbtnCancel_Click" />
                                        <asp:ImageButton runat="server" ID="imgbtnDel" ImageUrl="~/Icon/x-button.png" Width="30px" OnClick="imgbtnDel_Click" />
                                    </ItemTemplate>
                                    <ItemStyle Width="75px" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
            <div class="container" runat="server" id="div_ADD" visible ="false" style="margin-top: 80px; position: absolute; backdrop-filter: contrast(0.5);padding-bottom:10px;border-radius:8px; ">
                <div class="position-absolute" style=" width: 100%;text-align: right; z-index:2" >
                    <asp:Button runat="server" ID="btnClose" CssClass="btn-close mt-2 mr-4" OnClick="btnClose_Click"/>
                </div>
                <div class="row" style="z-index:-3">
                    <div style="margin-top: 20px;">
                        <asp:Label runat="server" ID="Label1" Text="Add SiteStorage Profit" CssClass="h1"></asp:Label>
                    </div>
                </div>
                <div class="row">
                    <div class=" col-sm-12 my-2">
                        <div class="input-group">
                            <asp:Label runat="server" ID="Label8" Text="Brand" CssClass="input-group-text shadow"></asp:Label>
                            <asp:DropDownList ID="ddlBrandADD" runat="server" DataTextField="name" DataValueField="nameShot" CssClass="btn text-start shadow " BackColor="White"></asp:DropDownList>
                        </div>
                    </div>
                    <div class=" col-sm-12 col-md-3  my-2">
                        <div class="input-group">
                            <asp:Label runat="server" ID="Label7" Text="SiteStorage" CssClass="input-group-text shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtSiteStorageADD" CssClass="form-control text-uppercase"></asp:TextBox>
                        </div>
                    </div>
                    <div class=" col-sm-12 col-md-4 my-2" style="margin-bottom: 10px; ">
                        <div class="  input-group" >
                            <asp:Label runat="server" ID="Label9" Text="Sale Channel" CssClass="input-group-text shadow"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlSaleChannelADD" DataTextField="Sale_Channel"  CssClass="btn" BackColor="White" DataValueField="Sale_Channel">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class=" col-sm-12 col-md-5 my-2"  style="margin-bottom: 10px; flex-wrap: nowrap;">
                        <div class="  input-group mb-2 " style="width:100%">
                            <asp:Label runat="server" ID="Label2" Text="Channel" CssClass="input-group-text shadow"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlChannelADD"  CssClass="form-control" Width="100%">
                                            <asp:ListItem Text="--SELECT--" Value="--SELECT--"></asp:ListItem>
                                            <asp:ListItem Text="ONLINE" Value="ONLINE"></asp:ListItem>
                                            <asp:ListItem Text="OFFLINE" Value="OFFLINE"></asp:ListItem>
                                        </asp:DropDownList>
                        </div>
                    </div>
                    <div class=" col-sm-12 col-md-3 my-2 "  style="margin-bottom: 10px; flex-wrap: nowrap;">
                        <div class="  input-group mb-2 ">
                            <asp:Label runat="server" ID="Label3" Text="COMCODE" CssClass="input-group-text shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtComcodeADD" CssClass="form-control" ></asp:TextBox>
                        </div>
                    </div>
                    <div class=" col-sm-12 col-md-3 my-2"  style="margin-bottom: 10px; flex-wrap: nowrap;">
                        <div class="  input-group mb-2 ">
                            <asp:Label runat="server" ID="Label4" Text="Profit" CssClass="input-group-text shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtProfitADD" CssClass="form-control" ></asp:TextBox>
                        </div>
                    </div>
                    <div class=" col-sm-12 col-md-3 my-2"  style="margin-bottom: 10px; flex-wrap: nowrap;">
                        <div class="  input-group mb-2 " >
                            <asp:Label runat="server" ID="Label5" Text="Cost Center" CssClass="input-group-text shadow"></asp:Label>
                            <asp:TextBox runat="server" ID="txtCostcenterADD" CssClass="form-control" ></asp:TextBox>
                        </div>
                    </div>

                </div>
                <div class="row float-end">
                    <div class="col-sm-12 col-md-3 ">
                        <asp:Button runat="server" ID="btnSaveAdd" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveAdd_Click"></asp:Button>
                    </div>
                </div>
            </div>
            
            
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
