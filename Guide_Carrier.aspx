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
                <ul class="list-unstyled ps-0">
                    <li class="mb-1">
                        <asp:Button runat="server" ID="btnMain" class="btn btn-toggle align-items-center rounded " Text="ClickMain"/>
                        <%--<button class="btn btn-toggle align-items-center rounded collapsed" data 
                        <div class="collapsing" id="collapseID">
                            Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.
                        </div>
                    --%></li>
                </ul>
            </div>
        </div>
        
        <div class="row">
            <%--<asp:Image runat="server" ID="img1" ImageUrl="" />--%>
        </div>
    </div>
</asp:Content>
