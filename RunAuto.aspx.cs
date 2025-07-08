using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Carrier.Model.Carrier;
using Carrier.Model.InsideSFG_WF;
using Carrier.Model.Budget;
using Carrier.Service;

namespace Carrier
{
    public partial class RunAuto : System.Web.UI.Page
    {
        CarrierEntities carrier_Entities = new CarrierEntities();
        InsideSFG_WFEntities InsideSFG_WF_Entities = new InsideSFG_WFEntities();
        BudgetEntities budget_Entities = new BudgetEntities();
        Service_Flash service_Flashs = new Service_Flash();
        Service_Budget service_Budget = new Service_Budget();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if(Request.QueryString["Action"] != null)
                {
                    switch (Request.QueryString["Action"].ToString())
                    {
                        case "LoadUpdateNotify":
                            LoadUpdateNotify();
                            break;
                        case "CallAuto":
                            LoadUpdateNotify();
                            System.Threading.Thread.Sleep(1000);
                            CallAuto();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public void LoadUpdateNotify()
        {
            var orderList = (from orderItem in carrier_Entities.Order_Item
                             join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                             where orderItem.Status == null  && order.Transport_Type == 1
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
                    #region V2
                    var checkNO = lastNolist.OrderByDescending(o => o.History_ID).FirstOrDefault().History_NO;
                    if (checkNO.Length == 8)
                    {
                        lastNo = "HIS" + DateTime.Now.Year.ToString().Substring(2, 2) + "00000";
                    }
                    else if (checkNO.Substring(3, 2) != DateTime.Now.Year.ToString().Substring(2, 2))
                    {
                        lastNo = "HIS" + DateTime.Now.Year.ToString().Substring(2, 2) + "00000";
                    }
                    else
                    {
                        lastNo = checkNO;
                    }
                    var lenght = (Convert.ToInt32(lastNo.Substring(5, 5)) + 1).ToString().Length;
                    var newNo = lastNo.Substring(0, 10 - lenght) + (Convert.ToInt32(lastNo.Substring(5, 5)) + 1).ToString();
                    #endregion
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

        public void CallAuto()
        {
            var dateOld = DateTime.Now.AddDays(-7);
            var dateOldFixed = new DateTime(dateOld.Year, dateOld.Month, dateOld.Day, 0, 0, 1);
            var ordNotPrint = (from orderItem in carrier_Entities.Order_Item
                             join order in carrier_Entities.Orders on orderItem.Docno equals order.Docno
                             where (orderItem.Status == null || orderItem.Status == "") && order.Transport_Type == 1 && order.Date_send >= dateOldFixed && order.Date_send <= DateTime.Now
                             select new { Docno = order.Docno, pno = orderItem.pno }).ToList();
            //var ordNotPrint = (from b in carrier_Entities.Order_Big_Box
            //                   join o in carrier_Entities.Orders on b.Docno equals o.Docno
            //                   join i in carrier_Entities.Order_Item on o.Docno equals i.Docno
            //                   where o.Transport_Type == 1  && (i.Status == null || i.Status == "") && o.Date_send >= dateOldFixed && o.Date_send <= DateTime.Now && b.Status == "A"
                               
            //                   select new { Docno = o.Docno , pno = i.pno }).ToList();
            if (ordNotPrint.Count() != 0)
            {
                var Orderpno = ordNotPrint.Select(s => s.pno);
                var responseNotifyList = service_Flashs.Notify(Orderpno.ToList());


                //List<messageNotify> messageNoti = new List<messageNotify>();
                DateTime dateSuccess = DateTime.Now;
                List<History_Notify_Order> his = new List<History_Notify_Order>();
                #region V2
                var lastNo = "";
                var checkNO = carrier_Entities.History_Notify_Order.OrderByDescending(o => o.History_ID).FirstOrDefault().History_NO;
                if (checkNO.Length == 8)
                {
                    lastNo = "HIS" + DateTime.Now.Year.ToString().Substring(2, 2) + "00000";
                }
                else if (checkNO.Substring(3, 2) != DateTime.Now.Year.ToString().Substring(2, 2))
                {
                    lastNo = "HIS" + DateTime.Now.Year.ToString().Substring(2, 2) + "00000";
                }
                else
                {
                    lastNo = checkNO;
                }
                var lenght = (Convert.ToInt32(lastNo.Substring(5, 5)) + 1).ToString().Length;
                var newNo = lastNo.Substring(0, 10 - lenght) + (Convert.ToInt32(lastNo.Substring(5, 5)) + 1).ToString();
                #endregion
                foreach (var responseNotify in responseNotifyList)
                {
                    var mess = "เลขที่พัสดุที่ : ";
                    if (responseNotify.code == 1)
                    {
                        var lastpno = responseNotify.pno.LastOrDefault();
                        foreach (var pno in responseNotify.pno)
                        {
                            var orderItem = carrier_Entities.Order_Item.Where(w => w.pno == pno).FirstOrDefault();
                            orderItem.Status = "A";
                            orderItem.CodeResponse = responseNotify.code;
                            orderItem.ticketPickupId = responseNotify.ticketPickupId;
                            orderItem.Date_Success = dateSuccess;
                            if (lastpno == pno)
                            {
                                mess += pno + " เรียกรถเรียบร้อยแล้ว";
                            }
                            else
                            {
                                mess += pno + ",";
                            }
                            his.Add(new History_Notify_Order
                            {
                                History_NO = newNo,
                                Docno = orderItem.Docno,
                                Date_Notify = dateSuccess,
                                pno = pno,
                                Type_Send_KA = orderItem.TypeSendKO,
                                SaveFrom = "Auto"
                            });
                        }
                        carrier_Entities.Notifies.Add(new Notify
                        {
                            TicketPickupId = responseNotify.ticketPickupId,
                            StaffInfoId = responseNotify.staffInfoId,
                            StaffInfoName = responseNotify.staffInfoName,
                            StaffInfoPhone = responseNotify.staffInfoPhone,
                            UpCountryNote = responseNotify.upCountryNote,
                            TimeoutAtText = responseNotify.timeoutAtText,
                            TicketMessage = responseNotify.ticketMessage,
                            DateNotify = responseNotify.dateSuccess,
                            warehouseNo = responseNotify.warehouseNo
                        });
                        carrier_Entities.SaveChanges();
                    }
                    else
                    {
                        if (responseNotify.code == 1010)
                        {
                            var notiOld = carrier_Entities.Notifies.Where(w => w.warehouseNo == responseNotify.warehouseNo).OrderByDescending(r => r.DateNotify).ToList();
                            foreach (var pno in responseNotify.pno)
                            {
                                var lastpno = responseNotify.pno.LastOrDefault();
                                var orderItem = carrier_Entities.Order_Item.Where(w => w.pno == pno).FirstOrDefault();
                                if (notiOld == null)
                                {
                                    orderItem.Status = "A";
                                    orderItem.CodeResponse = 1;
                                    orderItem.Date_Success = dateSuccess;
                                    //orderItem.ticketPickupId = ;
                                    carrier_Entities.SaveChanges();
                                }
                                else
                                {
                                    orderItem.Status = "A";
                                    orderItem.CodeResponse = 1;
                                    orderItem.ticketPickupId = notiOld.Select(s => s.TicketPickupId).FirstOrDefault();
                                    orderItem.Date_Success = dateSuccess;
                                    carrier_Entities.SaveChanges();
                                }
                                if (lastpno == pno)
                                {
                                    mess += pno + " เรียกรถเรียบร้อยแล้ว";
                                }
                                else
                                {
                                    mess += pno + ",";
                                }
                                var hisNoti = carrier_Entities.History_Notify_Order.Where(w => w.pno == pno).FirstOrDefault();
                                if (hisNoti == null)
                                {
                                    his.Add(new History_Notify_Order
                                    {
                                        History_NO = newNo,
                                        Docno = orderItem.Docno,
                                        Date_Notify = dateSuccess,
                                        pno = pno,
                                        Type_Send_KA = orderItem.TypeSendKO,
                                        SaveFrom = "Auto"
                                    });
                                }
                            }
                        }
                        else
                        {

                            var lastpno = responseNotify.pno.LastOrDefault();
                            foreach (var pno in responseNotify.pno)
                            {
                                var orderItem = carrier_Entities.Order_Item.Where(w => w.pno == pno).FirstOrDefault();
                                orderItem.CodeResponse = responseNotify.code;
                                carrier_Entities.SaveChanges();
                                if (lastpno == pno)
                                {
                                    mess += pno + " ไม่สามารถเรียกรถได้";
                                }
                                else
                                {
                                    mess += pno + ",";
                                }
                            }
                            //messageNoti.Add(new messageNotify { code = 1, message = mess });
                        }
                    }
                    var history = carrier_Entities.History_Notify_Order.Where(w => Orderpno.ToList().Contains(w.pno)).ToList();
                    carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
                    {
                        dateSend = DateTime.Now,
                        path = "Carrier/RunAuto_ADD_History_Notify",
                        request = Newtonsoft.Json.JsonConvert.SerializeObject(history),
                        status = responseNotify.code.ToString(),
                        respon = mess
                    });
                    carrier_Entities.SaveChanges();
                }
                if (his.Count != 0)
                {
                    carrier_Entities.History_Notify_Order.AddRange(his);
                    carrier_Entities.SaveChanges();
                }
            }

        }


        

    }
}