using Carrier.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.Carrier;

namespace Carrier
{
    public partial class Transport_bill : System.Web.UI.Page
    {
        Service_Flash Service_Flash;
        CarrierEntities Carrier_Entities;
        public Transport_bill()
        {
            Service_Flash = new Service_Flash();
            Carrier_Entities = new CarrierEntities();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var docno = Request.QueryString["Docno"];
            lbDocno.Text = Request.QueryString["Docno"];
            String originalPath = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
            string filePath = "";
            string dataDir = "";
            var checkBig = Carrier_Entities.Order_Big_Box.Where(w => w.BFID == docno).ToList();

            List<log> log = new List<log>();
            if(checkBig.Count() != 0)
            {
                try
                {
                    foreach (var i in checkBig)
                    {
                        var pathDocno = HttpContext.Current.Server.MapPath("PDFFile/") + i.Docno + ".pdf";
                        if (File.Exists(pathDocno))
                        {
                            File.Delete(pathDocno);
                            var create = Service_Flash.Get_Docment(i.Docno, "/Transport_bill");
                        }
                        else
                        {
                            var create = Service_Flash.Get_Docment(i.Docno, "/Transport_bill");
                        }
                    }


                    filePath = originalPath.Substring(0, originalPath.LastIndexOf("/Transport_bill")) + "/MergePDF/" + lbDocno.Text + ".pdf";
                    dataDir = HttpContext.Current.Server.MapPath("MergePDF/") + lbDocno.Text + ".pdf";
                    if (File.Exists(dataDir))
                    {
                        File.Delete(dataDir);
                        var create = Service_Flash.GetDocumentAll(docno, "/Transport_bill");
                    }
                    else
                    {
                        var create = Service_Flash.GetDocumentAll(docno, "/Transport_bill");
                    }
                    foreach (var i in checkBig)
                    {
                        Carrier_Entities.Orders.Where(w => w.Docno == i.Docno).FirstOrDefault().status = "AP";
                        Carrier_Entities.SaveChanges();
                    }
                }
                catch(Exception ex)
                {
                    
                }
                
            }
            else
            {
                try
                {
                    filePath = originalPath.Substring(0, originalPath.LastIndexOf("/Transport_bill")) + "/PDFFile/" + lbDocno.Text + ".pdf";
                    dataDir = HttpContext.Current.Server.MapPath("PDFFile/") + lbDocno.Text + ".pdf";
                    if (File.Exists(dataDir))
                    {
                        File.Delete(dataDir);
                        var create = Service_Flash.Get_Docment(docno, "/Transport_bill");
                    }
                    else
                    {
                        var create = Service_Flash.Get_Docment(docno, "/Transport_bill");
                    }
                    Carrier_Entities.Orders.Where(w => w.Docno == lbDocno.Text).FirstOrDefault().status = "AP";
                    Carrier_Entities.SaveChanges();
                }
                catch(Exception ex)
                {

                }
                
            }
            #region //เปิดไฟล์แบบเต็มหน้า
            //string fileExtention = Path.GetExtension(filePath);
            //WebClient client = new WebClient();
            //Byte[] buffer = client.DownloadData(filePath);
            //Response.ContentType = Service_Flash.ReturnExtension(fileExtention);
            //Response.AddHeader("content-length", buffer.Length.ToString());
            //Response.BinaryWrite(buffer);
            #endregion

            #region//เปิดไฟล์ให้อยู่แค่ใน Frame
            Page myPage = (Page)HttpContext.Current.Handler;
            myFrame.Src = filePath;
            myFrame.Visible = true;

            //div_Image.Visible = true;
            //ImageLabel.ImageUrl = filePath;
            //ClientScript.RegisterStartupScript(GetType(), "print", "window.print();", true);
            ScriptManager.RegisterStartupScript(myPage, myPage.GetType(), "CallMyFunction", "print_iFrame();", true);
            #endregion

            

        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Transport_Form?Docno=" + lbDocno.Text);
        }

    }
        public class log
        {
            public string row { get; set; }
            public string data { get; set; }
        }
}