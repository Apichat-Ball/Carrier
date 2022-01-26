using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.InsideSFG_WF;

namespace Carrier
{
    public partial class SiteMaster : MasterPage
    {
        InsideSFG_WFEntities entities_InsideSFG_WF;
        public SiteMaster()
        {
            entities_InsideSFG_WF = new InsideSFG_WFEntities();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //string menuid = "";
            //if (Request.QueryString["menuid"] != null)
            //{
            //    menuid = Request.QueryString["menuid"];
            //}

            if (!this.IsPostBack)
            {
                Check_UserID();
                lblUserID.Text = Session["_UserID"].ToString();

            }
        }
        protected void Check_UserID()
        {
            if (string.IsNullOrEmpty(Session["_UserID"] as string))
            {
                if ((Request.Cookies["sfgweb"] != null))
                {
                    if ((Request.Cookies["sfgweb"]["uname"] != null))
                    {
                        string username = Request.Cookies["sfgweb"]["uname"].Trim();
                        var objuser = (from tEmployee in entities_InsideSFG_WF.Employees
                                       where (tEmployee.username_ == username || tEmployee.uCode==username)
                                       && tEmployee.StatWork == "Y"
                                       select tEmployee
                                          ).FirstOrDefault();
                        if (objuser != null)
                        {
                            Session["_UserID"] = objuser.userID.ToString();
                            lblName.Text = "<span style='line-height:1;'>" + objuser.name + " " + objuser.surname + "</span>";
                            
                        }
                        else { Response.Redirect("Home_Carrier.aspx"); }
                    }
                    else { Response.Redirect("Home_Carrier.aspx"); }
                }
                else { Response.Redirect("Home_Carrier.aspx"); }
            }
            else
            {
                int userid = Convert.ToInt32(Session["_UserID"]);
                var objuser = (from tEmployee in entities_InsideSFG_WF.Employees
                               where tEmployee.userID == userid
                                    && tEmployee.StatWork == "Y"
                               select tEmployee
                                            ).FirstOrDefault();
                if (objuser != null)
                {
                    lblName.Text = "<span style='line-height:1;'>" + objuser.name + " " + objuser.surname + "</span>";
                }
                else { Response.Redirect("Home_Carrier.aspx"); }
            }

        }
    }
}