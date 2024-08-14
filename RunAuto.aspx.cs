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
                LoadUpdateNotify();
                //CreateBudget();
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

        //public void CreateBudget()
        //{
        //    var dateRate = DateTime.Now.AddDays(-7);
        //    var dateCurrent = new DateTime(dateRate.Year, dateRate.Month, dateRate.Day, 0, 0, 1);
        //    var calNotBud = carrier_Entities.Calculate_Car.Where(w => w.StatusBud == "A" && w.Date_Group < dateCurrent).GroupBy(g=>g.DeliveryNumber).Select(s=>new { DeliveryNumber = s.Key }).ToList();
        //    foreach(var deli in calNotBud)
        //    {
        //        var brand = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == deli.DeliveryNumber).GroupBy(g=> g.SDpart).Select(s=>new { SDpart = s.Key , QTY = s.Sum(x=>x.QTY) , Price = s.Sum(x=>x.Price) }).ToList();
        //        foreach(var BrandID in brand)
        //        {
        //            var siteStorage = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == deli.DeliveryNumber && w.SDpart == BrandID.SDpart).GroupBy(g=>g.SiteStorage).Select(s=>s.Key).ToList();
        //            var siteString = "";
        //            foreach(var site in siteStorage)
        //            {
        //                if(site == siteStorage.LastOrDefault())
        //                {
        //                    siteString += site;
        //                }
        //                else
        //                {
        //                    siteString += site + ",";
        //                }
                        
        //            }
        //            modelToCreate req = new modelToCreate();
        //            req.Department_ID = BrandID.SDpart;
        //            req.TypeBudget_ID = 2;
        //            req.Detail_ID = "5703";
        //            req.Detail = "ค่ารถจัดส่ง Auto จากระบบ Courier Lalamove เลข DeliveryID:"+deli.DeliveryNumber +" SiteStorage:" + siteString;
        //            req.OD_Docno = "";
        //            req.Date_Use = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        //            var usIdHeadstr = budget_Entities.Departments.Where(w=>w.Department_ID == BrandID.SDpart).FirstOrDefault().UserID;
        //            var usIdHead = Convert.ToInt32(usIdHeadstr.Contains(",") ? usIdHeadstr.Split(',')[0] : usIdHeadstr);
        //            var usAppID = InsideSFG_WF_Entities.Employees.Where(w => w.userID == usIdHead).FirstOrDefault();
        //            var departIO = budget_Entities.Department_IO.Where(w => w.Department_ID == BrandID.SDpart).ToList();
        //            if (departIO.Count() == 0)
        //            {
        //                var departmentDetail = InsideSFG_WF_Entities.BG_HApprove.Where(w => w.departmentID == BrandID.SDpart && new string[] { "S", "B" }.Contains(w.Sta)).FirstOrDefault();
        //                req.UserId_Approve = usIdHead.ToString();
        //            }
        //            else
        //            {
        //                req.UserId_Approve = usIdHead.ToString();
        //            }
        //            req.UserID = 101974;
        //            req.Estimate = Convert.ToDouble(BrandID.Price);
        //            req.Actual = Convert.ToDouble(BrandID.Price);

        //            var res = service_Budget.Create_MainExpense(req);
        //            if (res.code == "S")
        //            {
        //                var carpass = carrier_Entities.Calculate_Car.Where(w => w.DeliveryNumber == deli.DeliveryNumber && w.SDpart == BrandID.SDpart).ToList();
        //                foreach(var c in carpass)
        //                {
        //                    c.StatusBud = "F";
        //                }
        //                carrier_Entities.SaveChanges();
        //            }
        //        }

        //        carrier_Entities.API_Carrier_Log.Add(new API_Carrier_Log
        //        {
        //            dateSend = DateTime.Now,
        //            path = "RunAuto/CreateBudget",
        //            request = "Delivery:"+ deli.DeliveryNumber,
        //            respon = "Pass",
        //            status = "1"
        //        });
        //        carrier_Entities.SaveChanges();
        //    }
        //}
        

    }
}