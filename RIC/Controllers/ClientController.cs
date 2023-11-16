using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBLibrary;
using DBLibrary.UnitOfWork;
using Newtonsoft.Json;
using RIC.Models.Client;

namespace RIC.Controllers
{
     [Authorize]
    public class ClientController : Controller
    {
        string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        private UnitOfWork unitOfWork = new UnitOfWork();

        [HttpGet]
        public ActionResult ClientDashboard()
        {
            
            return View();
        }

        public ActionResult ClientTablePartial(int month)
        {
            ClientDashboardModel model = new ClientDashboardModel();
            //get the employee code.
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            var _user = unitOfWork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            //get the date of current month.
            DateTime date = SystemClock.US_Date.Date.AddMonths(month);
            DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            model.FromDate = startOfMonth;
            model.ToDate = endOfMonth;
            model.EmpCd = empCd;
           // DateTime endOfMonth2 = endOfMonth;
            endOfMonth=  endOfMonth.AddDays(1);// add the days in end of month.

             // if end of month is greter than date then change the end of month to todays date.
             if (endOfMonth >= SystemClock.US_Date.Date)
                 endOfMonth = SystemClock.US_Date.Date.AddDays(1);

            //get the submission ,interview and hire based on company.
             model.ClientTable = new List<ClintModelTable>();
             model.ClientTable = unitOfWork.RIC_Job_Report.Get_JobRepoartForUser(empCd, startOfMonth, endOfMonth,role).
                        GroupBy(s => s.RJ_Company, StringComparer.InvariantCultureIgnoreCase)// ignore the case of string.
                        .Select(s => new ClintModelTable()
                        {
                            Company = s.Key,
                            Submissions = s.Count(subCount => subCount.RJ_Submit_Date != null && subCount.RJ_Submit_Date >= startOfMonth && subCount.RJ_Submit_Date <= endOfMonth),
                            Interviews = s.Count(intCount => intCount.RJ_Interview_Date != null && intCount.RJ_Interview_Date >= startOfMonth && intCount.RJ_Interview_Date <= endOfMonth),
                            Hires = s.Count(hireCount => hireCount.RJ_Hire_Date != null && hireCount.RJ_Hire_Date >= startOfMonth && hireCount.RJ_Hire_Date <= endOfMonth),
                            // FromDate=startOfMonth,
                            // ToDate=endOfMonth2,
                            //  EmpCd=empCd
                        }).ToList();
            //get the total submission ,Interviews,Hires..
             model.TotalSubmission = model.ClientTable.Sum(s => s.Submissions);
             model.TotalInterviews = model.ClientTable.Sum(s => s.Interviews);
             model.TotalHires = model.ClientTable.Sum(s => s.Hires);
            
            return PartialView(model); 
        }
    
        [HttpGet]
        public ActionResult ClientSetting()
        {
            ClientSettingModel settingModel = new ClientSettingModel();
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            //get the date of current month.
            DateTime date = SystemClock.US_Date.Date;
            DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            endOfMonth.AddDays(1);
           
            // get the old list
             var selectdClient=  unitOfWork.ClientSetting.Get(s=>s.RC_EmpCd==empCd).FirstOrDefault();
            //get the client list for the current month.
             var clientListForUser=  unitOfWork.RIC_Job_Report.Get_ClientList(empCd, startOfMonth, endOfMonth)                                  
                                     .ToList();

            //initilize the selected client.
             settingModel.SelectedClient = new List<string>();
             if (selectdClient != null)
             {
                 List<string> oldlist=JsonConvert.DeserializeObject<List<string>>(selectdClient.RC_ClientList);                
                 // add the list to selected combo box and filter the strings not in clients list.
                 settingModel.SelectedClient = oldlist.Where(s=>clientListForUser.Any(a=>a==s));
             }
            // filter the selected client.
             //get the list of clients for user where not in selected.
             settingModel.ClientList = clientListForUser
                                       .Where(s=> !settingModel.SelectedClient.Any(a=>a==s))
                                       .ToList();           
            return View(settingModel);
        }

        [HttpPost]
        public JsonResult SaveClient(string clients)
        {
            //deserilize the json object.
            //var model = JsonConvert.DeserializeObject<List<string>>(clients);
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var clientSetting=   unitOfWork.ClientSetting.Get(s => s.RC_EmpCd == empCd).FirstOrDefault();
            if (clientSetting == null)
            {
                // add the new client list for the user.
                unitOfWork.ClientSetting.Insert(new RIC_ClientSetting
                {
                    RC_EmpCd = empCd,
                    RC_ClientList = clients
                });
                unitOfWork.Save();
            }
            else
            {
                // update the clients for user.
                clientSetting.RC_ClientList = clients;
                unitOfWork.ClientSetting.Update(clientSetting);
                unitOfWork.Save();
            }            
            return Json(clients, JsonRequestBehavior.AllowGet);  
        }        

        //public ActionResult ClientListView()
        //{
        //    ClientListView clist = new ClientListView();

        //    clist.clientList = unitOfWork.context.RIC_Client.ToList();
        //    return View(clist);
        //}

        
    }
}
