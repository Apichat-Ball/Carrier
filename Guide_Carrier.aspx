<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Guide_Carrier.aspx.cs" Inherits="Carrier.Guide_Carrier" MasterPageFile="~/Site.Master"%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
    <div runat="server" id="div_GuidePrint_Admin" >
        <div class="row mt-4 gj-text-align-center">
            <span class="align-center" style="font-size:x-large">คู่มือการใช้งาน(Admin)</span>
        </div>
        <div class="row">
            <div class="col-sm-6 gj-text-align-center">
                <span>s</span>
            </div>
        </div>
        
        <div class="row">
            <%--<asp:Image runat="server" ID="img1" ImageUrl="" />--%>
        </div>
    </div>
    <div runat="server" id="div" >
        <div class="row mt-4 gj-text-align-center">
            <span class="align-center" style="font-size:x-large">คู่มือการใช้งาน</span>
        </div>
        <div class="row">
            <div class="col-sm-6 gj-text-align-center">
                <span></span>
            </div>
        </div>
        
        <div class="row">
            <%--<asp:Image runat="server" ID="img1" ImageUrl="" />--%>
        </div>
    </div>
</asp:Content>
