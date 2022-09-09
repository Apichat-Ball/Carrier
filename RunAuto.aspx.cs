using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.Carrier;
using Carrier.Service;

namespace Carrier
{
    public partial class RunAuto : System.Web.UI.Page
    {
        CarrierEntities carrier_Entities = new CarrierEntities();
        Service_Flash service_Flashs = new Service_Flash();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUpdateNotify();
            }
        }
        public void LoadUpdateNotify()
        {
            var orderList = (from orderItem in carrier_Entities.Order_Item
                             join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                             where orderItem.Status == null
                             select order.Docno).ToList();
            foreach (var i in orderList)
            {
                var hi = carrier_Entities.History_Notify_Order.Where(w => w.Docno == i).ToList();
                var res = service_Flashs.CheckNotify(i);
                if (res != "" && hi.Count == 0)
                {
                    var order = carrier_Entities.Order_Item.Where(w => w.Docno == i).ToList();
                    order.FirstOrDefault().Status = "A";
                    order.FirstOrDefault().CodeResponse = 1;
                    var his = carrier_Entities.History_Notify_Order.Where(w => w.Docno == i).ToList();
                    var lastNolist = carrier_Entities.History_Notify_Order.ToList();
                    var lastNo = "";
                    if (lastNolist.Count == 0)
                    {
                        lastNo = "HIS00001";
                    }
                    else
                    {
                        lastNo = carrier_Entities.History_Notify_Order.OrderByDescending(o => o.History_ID).FirstOrDefault().History_NO;
                    }
                    var lenght = (Convert.ToInt32(lastNo.Substring(3, 5)) + 1).ToString().Length;
                    var newNo = lastNo.Substring(0, 8 - lenght) + (Convert.ToInt32(lastNo.Substring(3, 5)) + 1).ToString();
                    try
                    {
                        if (his.Count == 0)
                        {
                            carrier_Entities.History_Notify_Order.Add(new History_Notify_Order { Date_Notify = DateTime.Now, Docno = order.FirstOrDefault().Docno, pno = order.FirstOrDefault().pno, Type_Send_KA = order.FirstOrDefault().TypeSendKO, History_NO = newNo, SaveFrom = "Update" });
                            carrier_Entities.SaveChanges();
                        }
                    }
                    catch(Exception ex)
                    {
                        carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
                        {
                            dateSend = DateTime.Now,
                            path = "Carrier/RunAuto_ADD_History_Notify",
                            request = Newtonsoft.Json.JsonConvert.SerializeObject(new History_Notify_Order { Date_Notify = DateTime.Now, Docno = order.FirstOrDefault().Docno, pno = order.FirstOrDefault().pno, Type_Send_KA = order.FirstOrDefault().TypeSendKO, History_NO = newNo, SaveFrom = "Update" }),
                            status = "2",
                            respon = ex.Message
                        });
                        carrier_Entities.SaveChanges();
                    }
                    

                }
            }
        }
    }
}