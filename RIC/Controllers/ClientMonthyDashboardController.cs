using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBLibrary;
using DBLibrary.UnitOfWork;
using OfficeOpenXml;
using RIC.Models;
using RIC.Utility;
using PagedList;
using System.Data.SqlClient;
using System.Diagnostics;
using RIC.Models.Dashboard;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using RIC.Models.Client;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;


namespace RIC.Controllers
{
    public class ClientMonthyDashboardController : Controller
    {
        //
        // GET: /ClientMonthyDashboard/
        UnitOfWork unitOfwork = new UnitOfWork();

        string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        string stafingDirectorRole = System.Configuration.ConfigurationManager.AppSettings["StaffingDirector"];
        string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];


        public ActionResult ClientMonthyDashboardDisplayView()
        {
            string empCd = null;//int i = 0;

            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var _user = unitOfwork.User.GetByEmpID(empCd);
            var selectedAM = Request["AMList"];
            ViewBag.Empcd = empCd;
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
            ClientMonthlyReportview client = new ClientMonthlyReportview();
            if (role == directorRoleName || role == stafingDirectorRole)
            {
                if (!string.IsNullOrEmpty(selectedAM))
                {
                    empCd = selectedAM;
                }


            }


            List<RIC_Client> company = (from s in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                                        select new RIC_Client()
                                        {
                                            RC_Id = s.RIC_Client.RC_Id,
                                            RC_ClientName = s.RIC_Client.RC_ClientName


                                        }).OrderBy(s => s.RC_ClientName).ToList();
            if (role == directorRoleName || role == stafingDirectorRole)
            {
                if (string.IsNullOrEmpty(selectedAM))
                {
                    company = getList();
                }
            }

            List<SelectListItem> _lstQuater = new List<SelectListItem>();
            int quater = GetQuarter(DateTime.Now.Date);
            for (int j = 1; j <= 4; j++)
            {
                SelectListItem lt = new SelectListItem();
                lt.Text = "Q" + j;
                lt.Value = j.ToString();
                lt.Selected = j == quater ? true : false;
                _lstQuater.Add(lt);
            }
            //_lstQuater.Add(new SelectListItem() { Text = "All", Value = "0" });

            client.AMSelectList = getAMList(empCd);
            client.QuaterSelectList = _lstQuater;
            client.ClientList = getClientlist(company);
            client.MonthList = getMonthList(quater);
            return View(client);

        }
        DateTime usDate = SystemClock.US_Date;

        public ActionResult ClientMonthlyPartial(int month, string empCd, string AM, string Client,int Quater)
        {

            //get the date of current month.
            DateTime date = SystemClock.US_Date.AddMonths(month);
             ViewBag.date = date;
            DateTime TodayDate = SystemClock.US_Date.Date;

            RolePrincipal rolePrincipal = (RolePrincipal)User;
            var role = rolePrincipal.GetRoles()[0];

            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            if (role == directorRoleName || role == stafingDirectorRole)
            {
                if (AM != "All")
                {
                    empCd = AM;
                    role = AccMgrRoleName;
                }
            }

            //get the start date of month.
            // DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime startOfMonth = new DateTime(TodayDate.Year, month, 1);

            //get end date of month.
            DateTime endOfMonth = new DateTime(TodayDate.Year, month, DateTime.DaysInMonth(TodayDate.Year, month));
            if (month == 0)
            {
                startOfMonth = new DateTime(date.Year, (Quater - 1) * 3 + 1, 1);
                endOfMonth = startOfMonth.AddMonths(3).AddDays(-1);
                
            }
            DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
            var DashboardMatrics = dashboardIndex.getClientDashboard(empCd, startOfMonth, endOfMonth, TodayDate, role);
            if (Client != "All")
            {
                DashboardMatrics = DashboardMatrics.Where(s => Client.Contains(s.RJ_Company)).ToList();
            }
            return PartialView(DashboardMatrics);
        }

        [HttpGet]
        public ActionResult ClientReport(string empCd)
        {

            ClientReportView view = new ClientReportView();
            view.FromDate = usDate.Date;
            view.ToDate = usDate.Date;
            ViewBag.Empcd = empCd;
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var _user = unitOfwork.User.GetByEmpID(empCd);
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
            // var company1 = unitOfwork.User.getClientList().ToList();

            List<SelectListItem> _lstClient = new List<SelectListItem>();

            List<RIC_Client> company = (from s in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                                        select new RIC_Client()
                                        {
                                            RC_Id = s.RIC_Client.RC_Id,
                                            RC_ClientName = s.RIC_Client.RC_ClientName


                                        }).ToList();
            if (role == directorRoleName || role == stafingDirectorRole)
            {
                company = getList();

            }

            _lstClient.Add(new SelectListItem() { Text = "All", Value = "All" });


            foreach (var rule in company)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = rule.RC_ClientName;
                selectlistitem.Value = rule.RC_ClientName.ToString();
                _lstClient.Add(selectlistitem);

            }

            List<SelectListItem> _lstYear = new List<SelectListItem>();

            _lstYear.Add(new SelectListItem() { Text = "All", Value = "0" });

            var yearList = Enumerable.Range(2016, usDate.Year - 2015).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Value).ToList();

            foreach (var year in yearList)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = year.Text;
                selectlistitem.Value = year.Value;
                _lstYear.Add(selectlistitem);

            }

            view.YearSelectList = _lstYear;

            view.ClientList = _lstClient;
            return View(view);


        }

        [HttpPost]
        public ActionResult ClientReport(ClientReportView ClientView, string empCd, string GetDates)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;


            RolePrincipal rolePrincipal = (RolePrincipal)User;
            var role = rolePrincipal.GetRoles()[0];

            DateTime fromDate = ClientView.FromDate;
            DateTime todate = ClientView.ToDate.AddDays(1);
            DateTime startdate, enddate;
            //get the submissions for all users..

            bool getDateBool = (GetDates == "") ? true : false;

            var crosslist = (from Company in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                             from emp in unitOfwork.RIC_Employee.Get()
                             where emp.RE_Resign_Date == null
                             select new
                             {
                                 Client = Company.RIC_Client.RC_ClientName,
                                 Empcd = emp.RE_Emp_Cd,
                                 JobDivaUserName = emp.RE_Jobdiva_User_Name
                             }).ToList();

            if (role == directorRoleName || role == stafingDirectorRole)
            {
                crosslist = (from Company in unitOfwork.RIC_ClientMapping.Get()
                             from emp in unitOfwork.RIC_Employee.Get()
                             where emp.RE_Resign_Date == null
                             select new
                             {
                                 Client = Company.RIC_Client.RC_ClientName,
                                 Empcd = emp.RE_Emp_Cd,
                                 JobDivaUserName = emp.RE_Jobdiva_User_Name
                             }).ToList();
            }

            crosslist = crosslist.GroupBy(a => new { a.Client, a.Empcd, a.JobDivaUserName }).Select(grp => grp.First()).ToList();

            if (getDateBool)
            {
                enddate = DateTime.Today.AddDays(0 - DateTime.Today.Day);
                startdate = enddate.AddDays(1 - enddate.Day);

            }
            else
            {
                startdate = fromDate;
                enddate = todate;
            }
            var jobdivaratio = unitOfwork.RIC_Job_Report.getClientDiva(startdate, enddate)
                 .Select(s => new
                 {
                     Client = s.Company,
                     Empcode = s.re_emp_cd,
                     EmployeeName = s.submitted_by,
                     Percentage = s.percentage
                 }).ToList();

            var submissionsYear = unitOfwork.RIC_Job_Report.GetAll()
                  .Where(s => (ClientView.GetYear != 0 ? (getDateBool == false ? (s.RJ_Submit_Date >= fromDate && s.RJ_Submit_Date <= todate) : (s.RJ_Submit_Date.Year == ClientView.GetYear)) : (s.RJ_Submit_Date != null)))
                                   .GroupBy(s => new { s.RJ_Company, s.RJ_EmpCd, s.RJ_Submitted_By })
                                   .Select(s => new
                                   {
                                       Client = s.Key.RJ_Company,
                                       Empcode = s.Key.RJ_EmpCd,
                                       EmployeeName = s.Key.RJ_Submitted_By,
                                       Submissions = s.Count()
                                   }).ToList();

            var InterviewYear = unitOfwork.RIC_Job_Report.GetAll()
                      .Where(s => (ClientView.GetYear != 0 ? (getDateBool == false ? (s.RJ_Interview_Date >= fromDate && s.RJ_Interview_Date <= todate) : (s.RJ_Interview_Date != null && s.RJ_Interview_Date.Value.Year == ClientView.GetYear)) : (s.RJ_Interview_Date != null)))
                      .GroupBy(s => new { s.RJ_Company, s.RJ_EmpCd, s.RJ_Submitted_By })
                                  .Select(s => new
                                  {
                                      Client = s.Key.RJ_Company,
                                      Empcode = s.Key.RJ_EmpCd,
                                      EmployeeName = s.Key.RJ_Submitted_By,
                                      Interviews = s.Count()
                                  }).ToList();

            var HiresYear = unitOfwork.RIC_Job_Report.GetAll()
                                      .Where(s => (ClientView.GetYear != 0 ? (getDateBool == false ? (s.RJ_Hire_Date >= fromDate && s.RJ_Hire_Date <= todate) : (s.RJ_Hire_Date != null && s.RJ_Hire_Date.Value.Year == ClientView.GetYear)) : (s.RJ_Hire_Date != null)))

                               .GroupBy(s => new { s.RJ_Company, s.RJ_EmpCd, s.RJ_Submitted_By }).Select(s => new
                               {
                                   Client = s.Key.RJ_Company,
                                   Empcode = s.Key.RJ_EmpCd,
                                   EmployeeName = s.Key.RJ_Submitted_By,
                                   Hires = s.Count()
                               }).ToList();


            ClientView.FilterData = (from getData in
                                         (from data in crosslist
                                          join Sub in submissionsYear on new { a = data.Client, b = data.Empcd } equals new { a = Sub.Client, b = Sub.Empcode } into sj
                                          from sSub in sj.DefaultIfEmpty()
                                          join Int in InterviewYear on new { a = data.Client, b = data.Empcd } equals new { a = Int.Client, b = Int.Empcode } into si
                                          from sInt in si.DefaultIfEmpty()
                                          join Hire in HiresYear on new { a = data.Client, b = data.Empcd } equals new { a = Hire.Client, b = Hire.Empcode } into sh
                                          from sHir in sh.DefaultIfEmpty()
                                          join Div in jobdivaratio on new { a = data.Client, b = data.Empcd, c = data.JobDivaUserName } equals new { a = Div.Client, b = Div.Empcode, c = Div.EmployeeName } into sd
                                          from sPer in sd.DefaultIfEmpty()
                                          where sSub != null || sInt != null || sHir != null || sPer != null

                                          select new ClientOperationalList()
                                          {
                                              Client = data.Client,
                                              EmpCd = data.Empcd,
                                              EmployeeName = data.JobDivaUserName,
                                              Submissions = sSub != null ? sSub.Submissions : 0,
                                              Interviews = sInt != null ? sInt.Interviews : 0,
                                              Hires = sHir != null ? sHir.Hires : 0,
                                              JdPercentage = sPer != null ? sPer.Percentage : 0,
                                              SubByInterview = Math.Round((Convert.ToDouble(sInt != null ? sInt.Interviews : 0) / Convert.ToDouble(sSub != null ? sSub.Submissions : 1) * 100), 2),
                                              SubByHire = Math.Round((Convert.ToDouble(sHir != null ? sHir.Hires : 0) / Convert.ToDouble(sSub != null ? sSub.Submissions : 1) * 100), 2),
                                              InterviewByHire = Math.Round((Convert.ToDouble(sHir != null ? sHir.Hires : 0) / Convert.ToDouble(sInt != null ? sInt.Interviews : 1) * 100), 2)
                                          })
                                     where ((ClientView.ClientSelected != "All" ? ClientView.ClientSelected == getData.Client : null != getData.Client) &&
                                          (ClientView.SubSelected == ">" ? getData.Submissions >= ClientView.Submissions : getData.Submissions <= ClientView.Submissions) &&
                                          (ClientView.InterviewSelected == ">" ? getData.Interviews >= ClientView.Interviews : getData.Interviews <= ClientView.Interviews) &&
                                          (ClientView.HireSelected == ">" ? getData.Hires >= ClientView.Hires : getData.Hires <= ClientView.Hires))
                                     select getData)
                               .ToList();



            ViewBag.ShowTable = true;


            List<RIC_Client> company = (from s in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                                        select new RIC_Client()
                                        {
                                            RC_Id = s.RIC_Client.RC_Id,
                                            RC_ClientName = s.RIC_Client.RC_ClientName


                                        }).ToList();
            if (role == directorRoleName || role == stafingDirectorRole)
            {
                company = getList();
            }
            List<SelectListItem> _lstClient = new List<SelectListItem>();
            List<SelectListItem> _lstYear = new List<SelectListItem>();


            _lstClient.Add(new SelectListItem() { Text = "All", Value = "All" });


            foreach (var rule in company)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = rule.RC_ClientName;
                selectlistitem.Value = rule.RC_ClientName.ToString();
                _lstClient.Add(selectlistitem);

            }

            _lstYear.Add(new SelectListItem() { Text = "All", Value = "0" });

            var yearList = Enumerable.Range(2016, usDate.Year - 2015).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Value).ToList();

            foreach (var year in yearList)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = year.Text;
                selectlistitem.Value = year.Value;
                _lstYear.Add(selectlistitem);

            }

            ClientView.YearSelectList = _lstYear;

            ClientView.ClientList = _lstClient;

            return View(ClientView);

        }

        [HttpPost]
        public JsonResult ClientList(string Prefix, string empCd)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            List<RIC_Client> company = (from s in unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping
                                        select new RIC_Client()
                                        {
                                            RC_Id = s.RIC_Client.RC_Id,
                                            RC_ClientName = s.RIC_Client.RC_ClientName


                                        }).ToList();

            var ClientList = (from N in company
                              where N.RC_ClientName.ToUpper().StartsWith(Prefix)
                              select new { N.RC_ClientName }).ToList();

            return Json(ClientList, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ClientDetailsByQuaterly(string empCd)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            ViewBag.Empcd = empCd;
            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            ClientReportView ClientView = new ClientReportView();

            ClientView.AMSelectList = getAMList(empCd);

            List<RIC_Client> company = getClientlist(empCd);


            if (role == directorRoleName || role == stafingDirectorRole)
            {
                company = getList();
            }

            ClientView.YearSelectList = getYear();

            ClientView.ClientList = getClientlist(company);
            return View(ClientView);
        }

        [HttpPost]
        public ActionResult ClientDetailsByQuaterly(ClientReportView ClientView, string empCd, string AMSelectList)
        {

            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            if (AMSelectList != "All" && AMSelectList != null)
            {
                empCd = AMSelectList;
            }
            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            ClientView.AMSelectList = getAMList(empCd);

            List<RIC_Client> company = getClientlist(empCd);

            if (role == directorRoleName || role == stafingDirectorRole)
            {
                company = getList();
            }

            ClientView.YearSelectList = getYear();
            ClientView.ClientList =   getClientlist(company);
            return View(ClientView);

        }

        public ActionResult ClientDetailsByQuaterlyPartial(string empCd, int Year, string Client, string Account)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
            if (role == stafingDirectorRole || role == directorRoleName)
            {
                if (Account == "All")
                {
                    empCd = "SBS0229";
                }
                else
                {
                    empCd = Account;
                    role = AccMgrRoleName;
                }


            }
            ClientDashboardQuaterly view = new ClientDashboardQuaterly();

            DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
            var ClientQuaterly = dashboardIndex.ClientDashboardQuaterWise(empCd, Year, role);
            var ClientMonthly = dashboardIndex.ClientDashboardMonthly(empCd, Year, role);

            if (Client != "All")
            {
                ClientQuaterly = ClientQuaterly.Where(s => s.RJ_Company.ToLower().Trim() == Client.ToLower().Trim()).ToList();
                ClientMonthly = ClientMonthly.Where(s => s.RJ_Company.ToLower().Trim() == Client.ToLower().Trim()).ToList();
            }

            var tupleData = new Tuple<List<ClientDashboardQuaterly>, List<ClientDashboardMonthly>>(ClientQuaterly, ClientMonthly);
            return PartialView("ClientDetailsByQuaterlyPartial", tupleData);

        }

        [HttpGet]
        public ActionResult ClientDetailsByQuaterlyNew(string empCd)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            ViewBag.Empcd = empCd;
            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
            ClientReportView ClientView = new ClientReportView();


            ClientView.AMSelectList = getAMList(empCd);

            List<RIC_Client> company = getClientlist(empCd);
            if (role == directorRoleName || role == stafingDirectorRole)
            {
                company = getList();
            }

            ClientView.YearSelectList = getYear();
      
            ClientView.ClientList = getClientlist(company).Where(s=>s.Text != "All").ToList();
           // ViewBag.clist= getClientlist(company);
            return View(ClientView);
        }

        [HttpPost]
        public ActionResult ClientDetailsByQuaterlyNew(ClientReportView ClientView, string empCd, string AMSelectList)
        {

            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            if (AMSelectList != "All" && AMSelectList != null)
            {
                empCd = AMSelectList;
            }
            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
            // ClientReportView ClientView = new ClientReportView();

            ClientView.AMSelectList = getAMList(empCd);

            List<RIC_Client> company = getClientlist(empCd);
            if (role == directorRoleName || role == stafingDirectorRole)
            {
                company = getList();
            }

            ClientView.YearSelectList = getYear();

            ClientView.ClientList = getClientlist(company).Where(s => s.Text != "All").ToList();
            return View(ClientView);

        }

        public ActionResult ClientDetailsByQuaterlyPartialNew(string empCd, int Year, string Client, string Account)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
            if (role == stafingDirectorRole || role == directorRoleName)
            {
                if (Account == "All")
                {
                    empCd = "SBS0229";
                }
                else
                {
                    empCd = Account;
                    role = AccMgrRoleName;
                }


            }
            Client = Client == "null" ? "All" : Client;
            string[] clist = Client.Split(',');
            //if (clist.Length > 0 && clist[0] == "All")
            //{
            //    Client = "All";
            //}

            ClientDashboardQuaterly totals = new ClientDashboardQuaterly();

            DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
            var ClientQuaterly = dashboardIndex.ClientDashboardQuaterWiseNew(empCd, Year, role, Client);
            var ClientMonthly = dashboardIndex.ClientDashboardMonthlyNew(empCd, Year, role, Client);
            var ClientMonthlyCompany = dashboardIndex.ClientDashboardMonthly(empCd, Year, role);
            if (Client != "All")
            {
                
                ClientMonthlyCompany = ClientMonthlyCompany
                                  .Where(s => clist.Contains(s.RJ_Company))
                                  .ToList();

            }


            var tupleData = new Tuple<List<ClientDashboardQuaterly>, List<ClientDashboardMonthly>, List<ClientDashboardMonthly>>(ClientQuaterly, ClientMonthly, ClientMonthlyCompany);
            return PartialView("ClientDetailsByQuaterlyPartialNew", tupleData);

        }

        [Authorize]
        public ActionResult DetailsViewPopup(string empCd, DateTime fromDate, DateTime toDate, string data_Jr, string company, string Account)
        {


            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            string headerText = null;
            string empTblHeaderText = null;


            toDate = toDate.AddDays(1);
            DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
            var DashboardMatrics = dashboardIndex.ClientDashboardQuaterWiseDetails(empCd, data_Jr, company, fromDate, toDate, role);


            if (empCd != "Directors")
                role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            if (role == directorRoleName || role == stafingDirectorRole)
            {
                empCd = "SBS0229";
            }

            if (Account != "All" && Account != null)
            {
                empCd = Account;
            }

            List<RIC_Job_Report> details = new List<RIC_Job_Report>();
            // List<RIC_Job_Report> Portfoliodetails = new List<RIC_Job_Report>();
            List<Consolidated> Consolidated = new List<Consolidated>();
            List<ClientRequiredDetails> reqs = new List<ClientRequiredDetails>();
            List<TotalRecruiterdetails> totreq = new List<TotalRecruiterdetails>();

            //if submission clicked.
            if (data_Jr == "Submission")
            {
                empTblHeaderText = "Submissions";
                headerText = "Submissions";// if submission clicked.
                details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role, headerText)
                                    .Where(s => s.RJ_Submit_Date >= fromDate.Date && s.RJ_Submit_Date <= toDate).ToList();
            }
            else if (data_Jr == "Interview")//if interview clicked.
            {
                empTblHeaderText = "Interviews";
                headerText = "Interviews";
                details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role, headerText)
                                    .Where(s => s.RJ_Interview_Date >= fromDate.Date && s.RJ_Interview_Date <= toDate).ToList();
            }
            else if (data_Jr == "Hire")//if hire click.
            {
                empTblHeaderText = "Hires";
                headerText = "Hires";
                details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role, headerText)
                                     .Where(s => s.RJ_Hire_Date >= fromDate.Date && s.RJ_Hire_Date <= toDate).ToList();
            }
            else if (data_Jr == "Reqs")//if hire click.
            {
                empTblHeaderText = "Requirements";
                headerText = "Requirements";
                //DateTime toDate1 = toDate.AddDays(-1);
                reqs = unitOfwork.RIC_Job_Report.Get_Job_required_details(company, fromDate, toDate, "", role, empCd)
                                      .ToList();
            }
            else if (data_Jr == "TotalRecriter")//if TotalRecriter click.
            {
                empTblHeaderText = "Total Recruiter";
                headerText = "Total Recruiter";
                DateTime toDate1 = toDate.AddDays(-1);

                totreq = unitOfwork.RIC_Job_Report.Get_TotalRecruiter_details(company, fromDate, toDate1, "total", role, empCd)
                                      .ToList();
                ViewBag.MenuText = "Total Recruiters";
            }
            else if (data_Jr == "ActiveRecriter")//if ActiveRecriter click.
            {
                //string company1 = "";
                   
                empTblHeaderText = "Active Recruiter";
                headerText = "Active Recruiter";
                if (company == null || company == "")
                {
                    company = "1";
                }
                 
                DateTime toDate1 = toDate.AddDays(-1);
                totreq = unitOfwork.RIC_Job_Report.Get_TotalRecruiter_details(company, fromDate, toDate1, "act", role, empCd)
                                      .ToList();
                ViewBag.MenuText = "Active Recruiters";
            }
            var companylist = unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping.Select(s => s.RIC_Client.RC_ClientName).Distinct().ToList();


            if (role == directorRoleName || role == stafingDirectorRole)
            {

                companylist = dashboardIndex.ClientDashboardMonthly(empCd, fromDate.Year, role)
                    .Where(s => s.RequirementsCount > 0)
                    .Where(s => s.StartDate >= fromDate.Date && s.EndDate < toDate)
                   .Select(s => s.RJ_Company.Trim()).Distinct()
                    .ToList();

                if (Account != "All" && Account != null)
                {
                    companylist = unitOfwork.User.GetByEmpID(Account).RIC_ClientMapping.Select(s => s.RIC_Client.RC_ClientName).Distinct().ToList();
                }

            }


            //var clst=companylist.
            if (string.IsNullOrEmpty(company))
            {
                details = details.Where(s => companylist.Contains(s.RJ_Company)).ToList();
                DashboardMatrics = DashboardMatrics.Where(s => companylist.Contains(s.Client)).ToList();
                if (reqs.Count > 0)
                {

                    reqs = reqs.Where(s => companylist.Contains(s.RJ_Company.Trim())).ToList();
                }

            }

            // if company is not null then filter data by company
            string[] clist = company.Split(',');
            if (!(string.IsNullOrEmpty(company)))
            {
               
                details = details.Where(s => company.Contains(s.RJ_Company)).ToList();
                if (reqs.Count > 0)
                {
                    reqs = reqs.Where(s => company.Contains(s.RJ_Company)).ToList();
                }
                else if (totreq.Count > 0)
                {

                    var ClientMonthlyCompany = dashboardIndex.ClientDashboardMonthly(empCd, fromDate.Year, role)
                        .Where(s => company.Contains(s.RJ_Company))
                        .Where(s => s.StartDate >= fromDate.Date && s.EndDate <= toDate);
                }
            }
            details = details.OrderBy(s => s.RJ_Submitted_By).ThenBy(s => s.RJ_DateIssued).ToList();

            if (!(string.IsNullOrEmpty(company)))
            {
                Consolidated = details.GroupBy(g => g.RJ_JobDiva_Ref).Select(s => new Consolidated
                {
                    IssuedDate = s.FirstOrDefault().RJ_DateIssued,
                    JobDiva_Ref = s.Key,
                    Title = s.FirstOrDefault().RJ_Title,
                    Company = s.FirstOrDefault().RJ_Company,
                    Client_Ref = s.FirstOrDefault().RJ_Optional_Ref,
                    Count = s.Count()
                }).Where(s =>clist.Contains(s.Company)).ToList();
            }
            else
            {
                Consolidated = details.GroupBy(g => g.RJ_JobDiva_Ref).Select(s => new Consolidated
                {
                    IssuedDate = s.FirstOrDefault().RJ_DateIssued,
                    JobDiva_Ref = s.Key,
                    Title = s.FirstOrDefault().RJ_Title,
                    Company = s.FirstOrDefault().RJ_Company,
                    Client_Ref = s.FirstOrDefault().RJ_Optional_Ref,
                    Count = s.Count()
                }).OrderBy(s => s.Company).ToList();
            }


            if (fromDate.AddDays(1) != toDate)//add the header text
                headerText = headerText + " From " + fromDate.ToString("MM-dd-yyyy") + " To " + toDate.AddDays(-1).ToString("MM-dd-yyyy");
            else
                headerText = headerText + " For " + fromDate.ToString("MM-dd-yyyy");
            ViewBag.empTblHeaderText = empTblHeaderText;

            var tupleData = new Tuple<string, List<ClientOperationalList>, List<RIC_Job_Report>, List<Consolidated>>(headerText, DashboardMatrics, details, Consolidated);
            var reqsTupl = new Tuple<string, List<ClientRequiredDetails>>(headerText, reqs);
            var TotalrecruitersTupl = new Tuple<string, List<TotalRecruiterdetails>>(headerText, totreq);
            if (data_Jr == "Reqs")
            {
                return PartialView("DetailsViewPopupReqsPartila", reqsTupl);
            }
            else if (data_Jr == "TotalRecriter" || data_Jr == "ActiveRecriter")
            {
                return PartialView("DetailsViewPopupTotalRecruiter", TotalrecruitersTupl);
            }
            else
            {
                return PartialView("DetailsViewPopup", tupleData);
            }
            // return PartialView("DetailsViewPopup", tupleData);
        }
        [HttpGet]
        public ActionResult ClientPositionDetails(string empCd)
        {

            ViewBag.Empcd = null;
            //string empCd=null;
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            ClientReportView ClientView = new ClientReportView();

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.

            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            ClientView.AMSelectList = getAMList(empCd);

            ClientView.YearSelectList = getYear();

            List<RIC_Client> company = getClientlist(empCd);
            if (role == directorRoleName || role == stafingDirectorRole)
            {
                company = getList();
            }
            ClientView.ClientList = getClientlist(company).Where(s => s.Text != "All").ToList();
            return View(ClientView);
        }
        [HttpPost]
        public ActionResult ClientPositionDetails(ClientReportView ClientView, string empCd, string AMSelectList)
        {

            ViewBag.Empcd = null;
            // string empCd = null;
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            if (AMSelectList != "All" && AMSelectList != null)
            {
                empCd = AMSelectList;
            }
            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            ClientView.AMSelectList = getAMList(empCd);


            ClientView.YearSelectList = getYear();

            List<RIC_Client> company = getClientlist(empCd);
            if (role == directorRoleName || role == stafingDirectorRole)
            {
                company = getList();
            }

            ClientView.ClientList = getClientlist(company).Where(s => s.Text != "All").ToList();
            return View(ClientView);
        }
        public ActionResult ClientPositionDetailsPartial(string EmpCd, string Year, string Client, string Account)
        {
            if (EmpCd == null || EmpCd == "") //get the employee code from authentication.          
                if (Request.Cookies[FormsAuthentication.FormsCookieName].Value != null)
                {
                    EmpCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                }
                else
                {
                    ViewBag.Logout = "Session expired please login again.";
                    return RedirectToAction("Login", "Account");
                }

            bool getallrecords = false;
            var _user = unitOfwork.User.GetByEmpID(EmpCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            if (role == stafingDirectorRole || role == directorRoleName)
            {
                if (Account == "All")
                {
                    EmpCd = "SBS0229";
                }
                else
                {
                    EmpCd = Account;
                    role = AccMgrRoleName;
                }


            }
            //Client = Client == "" ? "All" : Client;
            Client = Client == "null" ? "All" : Client;
            string[] clist = Client.Split(',');
            if (clist.Length > 0 && clist[0] == "All")
            {
                Client = "All";
            }

            DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
            var ClientQuaterly = dashboardIndex.PositionDashboardQuaterWise(EmpCd, Convert.ToInt32(Year), role, Client);
            var ClientMonthly = dashboardIndex.PositionDashboardMonthly(EmpCd, Convert.ToInt32(Year), role, Client);
            // positiondetails = positiondetails.Where(s => !s.Company.Contains("Total"));
            ClientQuaterly = ClientQuaterly.Where(s => s.Categories != "").ToList();
            ClientMonthly = ClientMonthly.Where(s => s.Categories != "").ToList();

            //int yearTotal = ClientQuaterly.Sum(s => s.TotalPositions);
            double[] total = new double[7];
            total[0] = ClientQuaterly.Sum(s => s.TotalPositions);
            var totSub = ClientQuaterly.Sum(s => s.TotalSubmission);
            var totint = ClientQuaterly.Sum(s => s.TotalInterview);
            var tothire = ClientQuaterly.Sum(s => s.TotalHire);
            total[1] = totSub; total[2] = totint; total[3] = tothire;
            total[4] = Math.Round((Convert.ToDouble(totint) / Convert.ToDouble(totSub != 0 ? totSub : 1) * 100), 2);
            total[5] = Math.Round((Convert.ToDouble(tothire) / Convert.ToDouble(totSub != 0 ? totSub : 1) * 100), 2);
            total[6] = Math.Round((Convert.ToDouble(tothire) / Convert.ToDouble(totint != 0 ? totint : 1) * 100), 2);



            ViewBag.YearSelectList = Enumerable.Range(2016, usDate.Year - 2015).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString(),
                Selected = s.ToString() == usDate.Year.ToString() ? true : false
            }).OrderByDescending(s => s.Value).ToList();
            var tupel = new Tuple<List<PositionDashboardQuaterly>, List<PositionDashboardMonthly>, double[]>(ClientQuaterly, ClientMonthly, total);
            return View("_ClientPositionDetailsPartial", tupel);

        }

        [Authorize]
        public ActionResult DetailsViewPopupPosition(string empCd, DateTime fromDate, DateTime toDate, string data_Jr, string company, string Type, string Account)
        {


            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            string headerText = null;
            string empTblHeaderText = null;


            // toDate = toDate.AddDays(1);

            if (empCd != "Directors")
                role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            DateTime toDate1 = toDate.AddDays(1);

            company = company == "" ? "All" : company;
            string[] clist = company.Split(',');
            if (clist.Length > 0 && clist[0] == "All")
            {
                company = "All";
            }
            DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
            var DashboardMatrics = dashboardIndex.ClientDashboardQuaterWiseDetails1(empCd, Type, company, fromDate, toDate1, data_Jr, role);

            List<RIC_Job_Report> details = new List<RIC_Job_Report>();
            // List<RIC_Job_Report> Portfoliodetails = new List<RIC_Job_Report>();
            List<Consolidated> Consolidated = new List<Consolidated>();
            List<Sp_PositionDetails> reqs = new List<Sp_PositionDetails>();
            if (Type == "Position")
            {
                empTblHeaderText = "Position";
                headerText = "Position";
                reqs = unitOfwork.RIC_Job_Report.Get_Job_Position_Details(empCd, fromDate, toDate, role, data_Jr).ToList();
                if (company != "All")
                {
                    //reqs = reqs.Where(s => s.Company.ToLower().Trim() == company.ToLower().Trim()).ToList();
                    reqs = reqs.Where(s => clist.Contains(s.Company)).ToList();
                }
                if (Account != "All" && Account != null)
                {
                    var amlist = unitOfwork.User.GetByEmpID(Account).RIC_ClientMapping.Select(s => s.RIC_Client.RC_ClientName).Distinct().ToList();

                    reqs = reqs.Where(s => amlist.Contains(s.Company)).ToList();
                }
            }



            //if submission clicked.
            if (Type == "Submission")
            {
                empTblHeaderText = "Submissions";
                headerText = "Submissions";// if submission clicked.
                if (data_Jr == "TotalSub")
                {
                    details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role, headerText)
                                   .Where(s => s.RJ_Submit_Date >= fromDate.Date && s.RJ_Submit_Date <= toDate && s.RJ_Position_Type != null)
                                   .Where(s => s.RJ_DateIssued.Year >= fromDate.Year && s.RJ_DateIssued.Year <= toDate.Year)
                                   // .Where((s => s.RJ_Position_Type.ToLower().Trim() == data_Jr.ToLower().Trim()))
                                   .ToList();
                }
                else
                {
                    details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role, headerText)

                                   .Where(s => s.RJ_DateIssued.Year >= fromDate.Year && s.RJ_Position_Type != null)
                                   .Where(s => s.RJ_Submit_Date >= fromDate.Date && s.RJ_Submit_Date <= toDate.Date)

                                   .Where((s => s.RJ_Position_Type.ToLower().Trim() == data_Jr.ToLower().Trim()))
                                   .ToList();
                }
                //details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role, headerText)
                //                    .Where(s => s.RJ_Submit_Date >= fromDate.Date && s.RJ_Submit_Date <= toDate  && s.RJ_Position_Type != null)
                //                    .Where(s=> s.RJ_DateIssued.Year >= fromDate.Year && s.RJ_DateIssued.Year <= toDate.Year)
                //                    .Where((s => s.RJ_Position_Type.ToLower().Trim() == data_Jr.ToLower().Trim()))
                //                    .ToList();
            }
            else if (Type == "Interview")//if interview clicked.
            {
                empTblHeaderText = "Interviews";
                headerText = "Interviews";
                if (data_Jr == "TotalInt")
                {
                    details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role, headerText)
                                    .Where(s => s.RJ_Interview_Date >= fromDate.Date && s.RJ_Interview_Date <= toDate && s.RJ_Position_Type != null)
                                   .Where(s => s.RJ_DateIssued.Year >= fromDate.Year && s.RJ_DateIssued.Year <= toDate.Year)
                                    //.Where((s => s.RJ_Position_Type.ToLower().Trim() == data_Jr.ToLower().Trim()))
                                    .ToList();
                }
                else
                {
                    details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role, headerText)
                                    .Where(s => s.RJ_Interview_Date >= fromDate.Date && s.RJ_Interview_Date <= toDate && s.RJ_Position_Type != null)
                                   .Where(s => s.RJ_DateIssued.Year >= fromDate.Year && s.RJ_DateIssued.Year <= toDate.Year)
                                   .Where((s => s.RJ_Position_Type.ToLower().Trim() == data_Jr.ToLower().Trim()))
                                    .ToList();
                }

            }
            else if (Type == "Hire")//if hire click.
            {
                empTblHeaderText = "Hires";
                headerText = "Hires";
                if (data_Jr == "TotalHire")
                {
                    details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role, headerText)
                                 .Where(s => s.RJ_Hire_Date >= fromDate.Date && s.RJ_Hire_Date <= toDate && s.RJ_Position_Type != null)
                                .Where(s => s.RJ_DateIssued.Year >= fromDate.Year && s.RJ_DateIssued.Year <= toDate.Year)
                                //.Where((s => s.RJ_Position_Type.ToLower().Trim() == data_Jr.ToLower().Trim()))
                                .ToList();
                }
                else
                {
                    details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role, headerText)
                                 .Where(s => s.RJ_Hire_Date >= fromDate.Date && s.RJ_Hire_Date <= toDate && s.RJ_Position_Type != null)
                                .Where(s => s.RJ_DateIssued.Year >= fromDate.Year && s.RJ_DateIssued.Year <= toDate.Year)
                               .Where((s => s.RJ_Position_Type.ToLower().Trim() == data_Jr.ToLower().Trim()))
                                .ToList();
                }

            }
            var companylist = unitOfwork.User.GetByEmpID(empCd).RIC_ClientMapping.Select(s => s.RIC_Client.RC_ClientName).Distinct().ToList();


            if (role == directorRoleName || role == stafingDirectorRole)
            {

                companylist = dashboardIndex.ClientDashboardMonthly(empCd, fromDate.Year, role)
                    .Where(s => s.RequirementsCount > 0)
                    .Where(s => s.StartDate >= fromDate.Date && s.EndDate <= toDate.Date)
                   .Select(s => s.RJ_Company.Trim()).Distinct()
                    .ToList();
                //var cli = details.Select(s => s.RJ_Company).OrderBy(s => s)
                //    .Distinct().ToList();

                ////cli = companylist.Intersect(cli).ToList();
                //var cli2 = cli.Where(t2 => companylist.Count(t1 => t2.Contains(t1)) == 0);
                if (Account != "All" && Account != null)
                {
                    companylist = unitOfwork.User.GetByEmpID(Account).RIC_ClientMapping.Select(s => s.RIC_Client.RC_ClientName).Distinct().ToList();
                }

            }

            if (companylist.Count > 0)
            {
                details = details.Where(s => companylist.Contains(s.RJ_Company)).ToList();
                var emplist = details.Select(s => s.RJ_EmpCd).Distinct();
                // DashboardMatrics = DashboardMatrics.Where(s => companylist.Contains(s.Client)).ToList();
                DashboardMatrics = DashboardMatrics.Where(s => emplist.Contains(s.EmpCd)).ToList();

            }

            if (company != "All")
            {
                //details = details.Where(s => s.RJ_Company.ToLower() == company.ToLower()).ToList();
                details = details.Where(s => clist.Contains(s.RJ_Company)).ToList();

            }
            details = details.OrderBy(s => s.RJ_Submitted_By).ThenBy(s => s.RJ_DateIssued).ToList();

            if (company != "All")
            {
                Consolidated = details.GroupBy(g => g.RJ_JobDiva_Ref).Select(s => new Consolidated
                {
                    IssuedDate = s.FirstOrDefault().RJ_DateIssued,
                    JobDiva_Ref = s.Key,
                    Title = s.FirstOrDefault().RJ_Title,
                    Company = s.FirstOrDefault().RJ_Company,
                    Client_Ref = s.FirstOrDefault().RJ_Optional_Ref,
                    Count = s.Count()
                })
                .Where(s => clist.Contains(s.Company)).ToList();
                //.Where(s => s.Company == company).ToList();
            }
            else
            {
                Consolidated = details.GroupBy(g => g.RJ_JobDiva_Ref).Select(s => new Consolidated
                {
                    IssuedDate = s.FirstOrDefault().RJ_DateIssued,
                    JobDiva_Ref = s.Key,
                    Title = s.FirstOrDefault().RJ_Title,
                    Company = s.FirstOrDefault().RJ_Company,
                    Client_Ref = s.FirstOrDefault().RJ_Optional_Ref,
                    Count = s.Count()
                }).OrderBy(s => s.Company).ToList();
            }

            if (fromDate.AddDays(1) != toDate)//add the header text
                headerText = headerText + " From " + fromDate.ToString("MM-dd-yyyy") + " To " + toDate.AddDays(-0).ToString("MM-dd-yyyy");
            else
                headerText = headerText + " For " + fromDate.ToString("MM-dd-yyyy");
            ViewBag.empTblHeaderText = empTblHeaderText;

            //var tupleData = new Tuple<string, List<ClientOperationalList>, List<RIC_Job_Report>, List<Consolidated>>(headerText, DashboardMatrics, details, Consolidated);
            var reqsTupl = new Tuple<string, List<Sp_PositionDetails>>(headerText, reqs);
            var tupleData = new Tuple<string, List<ClientOperationalList>, List<RIC_Job_Report>, List<Consolidated>>(headerText, DashboardMatrics, details, Consolidated);
            if (Type == "Position")
            {
                return PartialView("DetailsViewPopupPosition", reqsTupl);
            }
            else
            {
                return PartialView("DetailsViewPopup", tupleData);
            }


        }

        public ActionResult ClientSubmissionDetails(string empCd)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            ViewBag.Empcd = empCd;
            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            ClientSubmissionView ClientView = new ClientSubmissionView();

            ClientView.AMSelectList = getAMList(empCd);

            List<RIC_Client> company = getClientlist(empCd);

            DateTime currentdate = DateTime.Now;
            DateTime mondayOfLastWeek = currentdate.AddDays(-(int)currentdate.DayOfWeek - 6);
            DateTime FridayOfLastWeek = currentdate.AddDays(-(int)currentdate.DayOfWeek - 2);
            ViewBag.mondayOfLastWeek = mondayOfLastWeek.Date.ToShortDateString();
            ViewBag.FridayOfLastWeek = FridayOfLastWeek.Date.ToShortDateString();
            if (role == directorRoleName || role == stafingDirectorRole)
            {
                company = getList();
            }

            ClientView.YearSelectList = getYear();

            ClientView.ClientList = getClientlist(company);
            return View(ClientView);
        }
        [HttpPost]
        public ActionResult ClientSubmissionDetails(ClientSubmissionView ClientView, string empCd, string AMSelectList, DateTime FromDate, DateTime ToDate)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            if (AMSelectList != "All" && AMSelectList != null)
            {
                empCd = AMSelectList;
            }
            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            ClientView.AMSelectList = getAMList(empCd);

            List<RIC_Client> company = getClientlist(empCd);
            ViewBag.mondayOfLastWeek = FromDate.Date.ToShortDateString();
            ViewBag.FridayOfLastWeek = ToDate.Date.ToShortDateString();
            if (role == directorRoleName || role == stafingDirectorRole)
            {
                company = getList();
            }
           
            ClientView.YearSelectList = getYear();
            ClientView.ClientList = getClientlist(company);
            return View(ClientView);
        }

        public ActionResult ClientSubmissionDeatilsPartial(string empCd, int Year, string Client, string Account,DateTime FromDate,DateTime ToDate)
        {
            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
            if (role == stafingDirectorRole || role == directorRoleName)
            {
                if (Account == "All")
                {
                    empCd = "SBS0229";
                }
                else
                {
                    empCd = Account;
                    role = AccMgrRoleName;
                }


            }
           

            DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();

            var ClientQuaterly = dashboardIndex.ClientSubmissionDeatils(empCd, Year, role, new DateTime(DateTime.Now.Year, 01,01), DateTime.Today);
           // var ClientMonthly = dashboardIndex.ClientDashboardMonthly(empCd, Year, role);

            if (Client != "All")
            {
                ClientQuaterly = ClientQuaterly.Where(s => Client.Contains(s.Company)).ToList();
            }
            if(FromDate!=null && ToDate != null)
            {
                ClientQuaterly = ClientQuaterly.Where(s => s.DateIssued >= FromDate && s.DateIssued <= ToDate).ToList();
            }

            var tupleData = new Tuple<List<ClientSubmissionDetails> >(ClientQuaterly);
            return PartialView("ClientSubmissionDeatilsPartial", tupleData);
        }

        public ActionResult SubmissionViewPopup(string empCd, DateTime fromDate, DateTime toDate, string data_Jr)
        {

            if (empCd == null || empCd == "") //get the employee code from authentication.          
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            
            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            string headerText = null;
            string empTblHeaderText = null;


             toDate = toDate.AddDays(1);

            if (empCd != "Directors")
                role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            // toDate1 = toDate.AddDays(1);
            List<Sp_PositionDetails> reqs = new List<Sp_PositionDetails>();
            empTblHeaderText = "Submission Details";
            headerText = "Submission Details";

            reqs = unitOfwork.RIC_Job_Report.Get_SubmissionDetails(empCd, fromDate, toDate, role, data_Jr).ToList();


            if (fromDate.AddDays(1) != toDate)//add the header text
                headerText = headerText + " From " + fromDate.ToString("MM-dd-yyyy") + " To " + toDate.AddDays(-1).ToString("MM-dd-yyyy");
            else
                headerText = headerText + " For " + fromDate.ToString("MM-dd-yyyy");
            ViewBag.empTblHeaderText = empTblHeaderText;


            var reqsTupl = new Tuple<string, List<Sp_PositionDetails>>(headerText, reqs);
            return PartialView("SubmissionViewPopup", reqsTupl);
        }
        private List<SelectListItem> getYear()
        {
            int styear = usDate.Year - 4;
            var year = Enumerable.Range(styear, 5).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString(),
                Selected = s.ToString() == usDate.Year.ToString() ? true : false
            }).OrderByDescending(s => s.Value).ToList();
            return year;
        }
        public List<RIC_Client> getList()
        {
            List<RIC_Client> lst = (from Company in unitOfwork.RIC_ClientMapping.Get()
                                    select new RIC_Client()
                                    {
                                        RC_Id = Company.RIC_Client.RC_Id,
                                        RC_ClientName = Company.RIC_Client.RC_ClientName


                                    }).GroupBy(s => s.RC_Id).Select
                           (s => new RIC_Client
                           {
                               RC_Id = s.Key,
                               RC_ClientName = s.FirstOrDefault().RC_ClientName
                           }).OrderBy(s => s.RC_ClientName).Distinct().ToList();

            return lst;
        }

        public List<SelectListItem> getAMList(string amcode)
        {
            var amList = (from emplist in unitOfwork.RIC_Employee.Get()
                          join UR in unitOfwork.RIC_User_Role.Get()
                  on emplist.RE_EmpId equals UR.RUR_Emp_ID
                          where UR.RUR_Role_ID == 11

                          select new
                          {
                              emplist.RE_Employee_Name,
                              emplist.RE_Emp_Cd,

                          }
            ).OrderBy(s => s.RE_Employee_Name).ToList();
            List<SelectListItem> _lstAM = new List<SelectListItem>();

            _lstAM.Add(new SelectListItem() { Text = "All", Value = "All" });

            foreach (var rule in amList)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = rule.RE_Employee_Name;
                selectlistitem.Value = rule.RE_Emp_Cd;
                selectlistitem.Selected = rule.RE_Emp_Cd == amcode ? true : false;
                _lstAM.Add(selectlistitem);

            }
            return _lstAM;
        }

        private List<RIC_Client> getClientlist(string empcd)
        {
            var list = (from s in unitOfwork.User.GetByEmpID(empcd).RIC_ClientMapping
                        select new RIC_Client()
                        {
                            RC_Id = s.RIC_Client.RC_Id,
                            RC_ClientName = s.RIC_Client.RC_ClientName

                        }).OrderBy(s => s.RC_ClientName).ToList();

            return list;
        }

        private List<SelectListItem> getClientlist(List<RIC_Client> company)
        {
            List<SelectListItem> _lstClient = new List<SelectListItem>();
            var distinctcompany = company.Select(s => s.RC_ClientName).Distinct();
            _lstClient.Add(new SelectListItem() { Text = "All", Value = "All" });
            foreach (var rule in distinctcompany)// add the items in select list.
            {

                SelectListItem selectlistitem = new SelectListItem();
                selectlistitem.Text = rule;
                selectlistitem.Value = rule;
                _lstClient.Add(selectlistitem);

            }
            return _lstClient;
        }

        public int GetQuarter(DateTime date)
        {
            return (date.Month + 2) / 3;
        }

        public List<SelectListItem> getMonthList(int quater)
        {
            List<SelectListItem> _lstmonth = new List<SelectListItem>();
            SelectListItem lstall = new SelectListItem();
            //lstall.Text = "All";
            //lstall.Value = 0.ToString();
            //_lstmonth.Add(lstall);
            var months = Enumerable.Range(1, 12).Select(i => new { I = i, M = DateTimeFormatInfo.CurrentInfo.GetMonthName(i) });
            if (quater == 1)
            {
                months = months.Where(s => s.I >= 1 && s.I <= 3);
                foreach (var kvp in months)
                {
                    SelectListItem lst = new SelectListItem();
                    lst.Text = kvp.M;
                    lst.Value = kvp.I.ToString();
                    lst.Selected = kvp.I == DateTime.Now.Month ? true : false;
                    _lstmonth.Add(lst);
                }

            }
            else if (quater == 2)
            {
                months = months.Where(s => s.I >= 4 && s.I <= 6);
                foreach (var kvp in months)
                {
                    SelectListItem lst = new SelectListItem();
                    lst.Text = kvp.M;
                    lst.Value = kvp.I.ToString();
                    lst.Selected = kvp.I == DateTime.Now.Month ? true : false;
                    _lstmonth.Add(lst);
                }

            }
            else if (quater == 3)
            {
                months = months.Where(s => s.I >= 7 && s.I <= 9);
                foreach (var kvp in months)
                {
                    SelectListItem lst = new SelectListItem();
                    lst.Text = kvp.M;
                    lst.Value = kvp.I.ToString();
                    lst.Selected = kvp.I == DateTime.Now.Month ? true : false;
                    _lstmonth.Add(lst);
                }

            }
            else if (quater == 4)
            {
                months = months.Where(s => s.I >= 10 && s.I <= 12);
                foreach (var kvp in months)
                {
                    SelectListItem lst = new SelectListItem();
                    lst.Text = kvp.M;
                    lst.Value = kvp.I.ToString();
                    lst.Selected = kvp.I == DateTime.Now.Month ? true : false;
                    _lstmonth.Add(lst);
                }

            }

            return _lstmonth;
        }

    }
}
