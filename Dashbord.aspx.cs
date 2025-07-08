using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Service;

namespace Carrier
{
    public partial class Dashbord : System.Web.UI.Page
    {
        Service_Flash service_Flashs = new Service_Flash();
        protected void Page_Load(object sender, EventArgs e)
        {
            //สำหรับ Admin
            //Session.Clear();
            Session["_UserID"] = null;
            HttpContext.Current.Session["_UserID"] = "102063";
            if (Session["_UserID"] == null)
            {
                service_Flashs.Check_UserID();
            }
            if (Session["_UserID"] == null)
            {
                Response.Redirect("https://www.sfg-th.com/Login/Default.aspx?Page=Carrier/");
            }
            lbuserid.Text = Session["_UserID"].ToString();
        }
    }
}