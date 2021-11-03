<%--<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Transport_bill.aspx.cs" Inherits="Carrier.Transport_bill" %>--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Transport_bill.aspx.cs" Inherits="Carrier.Transport_bill" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>
                
        </title>
        <script type="text/javascript">
            function print_iFrame() {
                document.getElementById('myFrame').focus(); document.getElementById('myFrame').contentWindow.print();
            }
        </script>
        <link href="Content/bootstrap.min.css" rel="stylesheet" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div class="row justify-content-center mt-3 mb-2">
        <asp:Button runat="server" ID="btnBack" Text="กลับหน้า Transport" OnClick="btnBack_Click" CssClass="btn btn-primary"/>
        <asp:Label runat="server" ID="lbDocno" Visible="false"></asp:Label>
    </div>
    <div >
        <iframe name="myFrame" id="myFrame" runat="server" width="100%" height="590px" visible="false" ></iframe>
    </div>  
        </form>
    </body>
</html>

    
    
    