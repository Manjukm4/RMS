using RIC.Models;
using RIC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBLibrary.UnitOfWork;
using DBLibrary;
using System.Globalization;
using RIC.Models.Operational;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Table;

//using DBLibrary;
//using DBLibrary.Repository;

namespace RIC.Controllers
{
    [Authorize]
    public class OperationalController : Controller
    {
        string mgrRoleName = System.Configuration.ConfigurationManager.AppSettings["ManagerRole"];
        string tlRoleName = System.Configuration.ConfigurationManager.AppSettings["TLRole"];
        string empRoleName = System.Configuration.ConfigurationManager.AppSettings["EmployeeRole"];
        string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        int adminRoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AdminRoleID"]);
        int directorRoleID = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DirectorID"]);
        int tlRoleID = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TLRoleID"]);
        int mgrRoleID = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ManagerRoleID"]);
        string HrRoleName = System.Configuration.ConfigurationManager.AppSettings["HRRole"];
        string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
        string AdminRoleName = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];

       
        DataTable dtVM = new DataTable();
        List<OperationalViewPartial> opViewData = new List<OperationalViewPartial>();

        DateTime usDate = SystemClock.US_Date;   //TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time");
        //
        // GET: /Operational/
        UnitOfWork unitOfwork = new UnitOfWork();

        [Authorize]
        public ActionResult OperationalDashBoard()
        {
            var t = SystemClock.US_Date;

            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult OperationalReport()
        {

            OperationalReportView view = new OperationalReportView();
            view.FromDate = usDate.Date;
            view.ToDate = usDate.Date;
            view.IncludeTL = true;

            view.ExpSelectList = unitOfwork.SubmissionRule.Get().Select(s => new SelectListItem
            {
                Text = s.RS_Experience,
                Value = s.RS_Id.ToString()

            }).ToList();
            view.ExpSelectList.Add(new SelectListItem
            {
                Text = "All",
                Value = "0"
            });
            view.ExpSelectList = view.ExpSelectList
                                 .OrderBy(o => o.Value).ToList();
            return View(view);
        }
        [Authorize]
        [HttpPost]
        public ActionResult OperationalReport(OperationalReportView operationalView)
        {
            DateTime fromDate = operationalView.FromDate;
            DateTime todate = operationalView.ToDate.AddDays(1);
            var recruterDepartment = new List<int>() { 12, 14, 21 };
            //get the submissions for all users..
            var submissions = unitOfwork.RIC_Job_Report.GetAll()
                                  .Where(s => s.RJ_Submit_Date >= fromDate && s.RJ_Submit_Date <= todate)
                                  .GroupBy(s => new { s.RJ_EmpCd, s.RJ_Submitted_By }).Select(s => new
                                  {
                                      EmpCd = s.Key.RJ_EmpCd,
                                      EmployeeName = s.Key.RJ_Submitted_By,
                                      Submissions = s.Count()
                                  });

            //get the Interviews for all users..
            var Interviews = unitOfwork.RIC_Job_Report.GetAll()
                                  .Where(s => s.RJ_Interview_Date >= fromDate && s.RJ_Interview_Date <= todate)
                                  .GroupBy(s => new { s.RJ_EmpCd, s.RJ_Submitted_By }).Select(s => new
                                  {
                                      EmpCd = s.Key.RJ_EmpCd,
                                      EmployeeName = s.Key.RJ_Submitted_By,
                                      Interviews = s.Count()
                                  });


            //get the Hires for all users..
            var Hires = unitOfwork.RIC_Job_Report.GetAll()
                                  .Where(s => s.RJ_Hire_Date >= fromDate && s.RJ_Hire_Date <= todate)
                                  .GroupBy(s => new { s.RJ_EmpCd, s.RJ_Submitted_By }).Select(s => new
                                  {
                                      EmpCd = s.Key.RJ_EmpCd,
                                      EmployeeName = s.Key.RJ_Submitted_By,
                                      Hires = s.Count()
                                  });
            //get the call statistics for all users.  
            var calls = unitOfwork.CallStatistics
                            .Get(s => s.RC_Date >= fromDate && s.RC_Date <= todate)
                            .GroupBy(g => g.RC_Emp_Cd).Select(s => new
                            {
                                EmpCd = s.Key,
                                CallConnectedIn = s.Where(f => f.RC_CallType == "In").Sum(sum => sum.RC_Call_Connected),
                                VoiceMessageIn = s.Where(f => f.RC_CallType == "In").Sum(sum => sum.RC_Voice_Message),
                                CallConnectedOut = s.Where(f => f.RC_CallType == "Out").Sum(sum => sum.RC_Call_Connected),
                                VoiceMessageOut = s.Where(f => f.RC_CallType == "Out").Sum(sum => sum.RC_Voice_Message)
                            });
            int expID = int.Parse(operationalView.ExpSelected);
            // get the opearational repoart based on submission and call statistics.
            operationalView.FilterData = (from emp in unitOfwork.User.getAllUsers()
                                                  .Where(s => (operationalView.RemoveInactiveMember || s.RE_Resign_Date == null)
                                                        && (s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name != AdminRoleName)
                                                        && (s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name != directorRoleName)
                                                        && (s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name != HrRoleName)
                                                        && (s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name != AccMgrRoleName)
                                                        && (s.RMS_Department==null?false:recruterDepartment.Contains(s.RMS_Department.RD_ID))
                                                        && ((operationalView.IncludeTL) || s.RIC_User_Role.FirstOrDefault().RUR_Role_ID != tlRoleID)
                                                        && ((operationalView.IncludeTL) || s.RIC_User_Role.FirstOrDefault().RUR_Role_ID != mgrRoleID)
                                                        && (s.RE_Sub_Rule_ID == expID || expID == 0)
                                                        )
                                          join sub in submissions
                                          on emp.RE_Emp_Cd equals sub.EmpCd into sj
                                          from sSub in sj.DefaultIfEmpty()
                                          join Interview in Interviews
                                          on emp.RE_Emp_Cd equals Interview.EmpCd into IntList
                                          from sInt in IntList.DefaultIfEmpty()
                                          join Hire in Hires
                                          on emp.RE_Emp_Cd equals Hire.EmpCd into HireList
                                          from sHire in HireList.DefaultIfEmpty()
                                          join call in calls
                                          on emp.RE_Emp_Cd equals call.EmpCd into sCa
                                          from sCall in sCa.DefaultIfEmpty()
                                          select new OperationalList()
                                          {
                                              EmpCd = emp.RE_Emp_Cd,
                                              EmployeeName = emp.RE_Jobdiva_User_Name,
                                              TeamLeadName = emp.ReportingTo,
                                              Submissions = sSub != null ? sSub.Submissions : 0,
                                              Interviews = sInt != null ? sInt.Interviews : 0,
                                              Hires = sHire != null ? sHire.Hires : 0,
                                              CallConnectedIn = sCall != null ? sCall.CallConnectedIn : 0,
                                              CallConnectedOut = sCall != null ? sCall.CallConnectedOut : 0,
                                              VoiceMessageIn = sCall != null ? sCall.VoiceMessageIn : 0,
                                              VoiceMessageOut = sCall != null ? sCall.VoiceMessageOut : 0
                                          }).Where(filter =>
                                          (operationalView.SubSelected == ">" ? filter.Submissions >= operationalView.Submissions : filter.Submissions <= operationalView.Submissions)
                                        && (operationalView.CallSelect == ">" ? filter.CallConnectedOut >= operationalView.Calls : filter.CallConnectedOut <= operationalView.Calls)
                                      ).ToList();
            ViewBag.ShowTable = true;
            operationalView.ExpSelectList = unitOfwork.SubmissionRule.Get().Select(s => new SelectListItem
            {
                Text = s.RS_Experience,
                Value = s.RS_Id.ToString()

            }).ToList();
            operationalView.ExpSelectList.Add(new SelectListItem
            {
                Text = "All",
                Value = "0"
            });
            operationalView.ExpSelectList = operationalView.ExpSelectList
                                            .OrderBy(o => o.Value).ToList();
            return View(operationalView);
        }


        [Authorize]
        [HttpPost]
        public ActionResult OperationalResult(DateTime RequestedDate, string ExportToExcel)
        {
            DateTime endDate = RequestedDate.AddDays(1).AddTicks(-1);

            ViewBag.date = RequestedDate;
            ViewBag.toDate = RequestedDate.AddDays(1);

            string empCd = User.Identity.Name;

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;



            List<OperationalViewPartial> opView = unitOfwork.RIC_Job_Report
                .GetOperationalSubmission(empCd, RequestedDate, endDate, role).Select(s => new OperationalViewPartial
                {

                    EmpCD = s.EmpCD,
                    MgrCD = s.MgrCD,
                    EmployeeName = s.EmployeeName,
                    ManagerName = s.ManagerName,
                    Total_Sub = s.Total_Sub,
                    Total_Vm = s.Total_Vm,
                    Total_Call = s.Total_Call,
                    Total_Prod = s.Total_Prod,
                    Hr_Sub_6_9_AM = s.Hr_Sub_6_9_AM,
                    Hr_Vm_6_9_AM = s.Hr_Vm_6_9_AM,
                    Hr_C_6_9_AM = s.Hr_C_6_9_AM,
                    Hr_P_6_9_AM = s.Hr_P_6_9_AM,
                    Hr_Sub_10_AM = s.Hr_Sub_10_AM,
                    Hr_Vm_10_AM = s.Hr_Vm_10_AM,
                    Hr_C_10_AM = s.Hr_C_10_AM,
                    Hr_P_10_AM = s.Hr_P_10_AM,
                    Hr_Sub_11_AM = s.Hr_Sub_11_AM,
                    Hr_Vm_11_AM = s.Hr_Vm_11_AM,
                    Hr_C_11_AM = s.Hr_C_11_AM,
                    Hr_P_11_AM = s.Hr_P_11_AM,
                    Hr_Sub_12_PM = s.Hr_Sub_12_PM,
                    Hr_Vm_12_PM = s.Hr_Vm_12_PM,
                    Hr_C_12_PM = s.Hr_C_12_PM,
                    Hr_P_12_PM = s.Hr_P_12_PM,
                    Hr_Sub_1_PM = s.Hr_Sub_1_PM,
                    Hr_Vm_1_PM = s.Hr_Vm_1_PM,
                    Hr_C_1_PM = s.Hr_C_1_PM,
                    Hr_P_1_PM = s.Hr_P_1_PM,
                    Hr_Sub_2_PM = s.Hr_Sub_2_PM,
                    Hr_Vm_2_PM = s.Hr_Vm_2_PM,
                    Hr_C_2_PM = s.Hr_C_2_PM,
                    Hr_P_2_PM = s.Hr_P_2_PM,
                    Hr_Sub_3_PM = s.Hr_Sub_3_PM,
                    Hr_Vm_3_PM = s.Hr_Vm_3_PM,
                    Hr_C_3_PM = s.Hr_C_3_PM,
                    Hr_P_3_PM = s.Hr_P_3_PM,
                    Hr_Sub_4_PM = s.Hr_Sub_4_PM,
                    Hr_Vm_4_PM = s.Hr_Vm_4_PM,
                    Hr_C_4_PM = s.Hr_C_4_PM,
                    Hr_P_4_PM = s.Hr_P_4_PM,
                    Hr_Sub_5_PM = s.Hr_Sub_5_PM,
                    Hr_Vm_5_PM = s.Hr_Vm_5_PM,
                    Hr_C_5_PM = s.Hr_C_5_PM,
                    Hr_P_5_PM = s.Hr_P_5_PM,
                    Hr_Sub_6_PM = s.Hr_Sub_6_PM,
                    Hr_Vm_6_PM = s.Hr_Vm_6_PM,
                    Hr_C_6_PM = s.Hr_C_6_PM,
                    Hr_P_6_PM = s.Hr_P_6_PM,
                    Hr_Sub_7_PM = s.Hr_Sub_7_PM,
                    Hr_Vm_7_PM = s.Hr_Vm_7_PM,
                    Hr_C_7_PM = s.Hr_C_7_PM,
                    Hr_P_7_PM = s.Hr_P_7_PM,
                    Hr_Sub_8_PM = s.Hr_Sub_8_PM,
                    Hr_Vm_8_PM = s.Hr_Vm_8_PM,
                    Hr_C_8_PM = s.Hr_C_8_PM,
                    Hr_P_8_PM = s.Hr_P_8_PM,
                    Hr_Sub_9_PM = s.Hr_Sub_9_PM,
                    Hr_Vm_9_PM = s.Hr_Vm_9_PM,
                    Hr_C_9_PM = s.Hr_C_9_PM,
                    Hr_P_9_PM = s.Hr_P_9_PM,
                    Hr_Sub_10_PM_12_AM = s.Hr_Sub_10_PM_12_AM,
                    Hr_Vm_10_PM_12_AM = s.Hr_Vm_10_PM_12_AM,
                    Hr_C_10_PM_12_AM = s.Hr_C_10_PM_12_AM,
                    Hr_P_10_PM_12_AM = s.Hr_P_10_PM_12_AM,
                    EmpLevel = s.EmpLevel
                }).ToList();

          
            return PartialView("OperationalResult", opView);
        }




        public JsonResult GetGraphforWeek(DateTime RequestedDate)
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            DateTime dateToday = RequestedDate;
            DateTime startDateOFweek = dateToday.Date.AddDays(DayOfWeek.Monday - dateToday.Date.DayOfWeek);

            DateTime endDateOFweek = startDateOFweek.AddDays(5);

            RolePrincipal r = (RolePrincipal)User;
            string role = r.GetRoles().FirstOrDefault();

            var jdDataForWeek = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, startDateOFweek, endDateOFweek, role).
                 Where(w => w.RJ_Submit_Date >= startDateOFweek && w.RJ_Submit_Date <= endDateOFweek).
                 Select(s => new
                 {
                     SubDate = s.RJ_Submit_Date,
                     empCd = s.RJ_EmpCd
                 }).GroupBy(s => s.SubDate.Date).Select(sg => new
                 {

                     Date = sg.Key,
                     SubList = sg.GroupBy(sl => sl.SubDate.Hour).Select(slg => new
                     {
                         Hour = slg.Key,
                         SubCount = slg.Count()
                     }).ToList()
                 }).OrderBy(o => o.Date).ToList();
            //                  ;
            List<List<int>> dataForWeek = new List<List<int>>();
            //   var result =jdDataForWeek


            for (int day = 0; day <= 4; day++)
            {
                DateTime date = startDateOFweek.AddDays(day);
                List<int> dataforDay = new List<int>();

                var subForDay = jdDataForWeek.FirstOrDefault(s => s.Date.Date == date);

                if (subForDay != null)
                {
                    for (int j = 5; j <= 24; j++)
                    {
                        var forHour = subForDay.SubList.FirstOrDefault(s => s.Hour == j);
                        dataforDay.Add(forHour != null ? forHour.SubCount : 0);
                    }
                }

                dataForWeek.Add(dataforDay);
            }


            //foreach (var item in jdDataForWeek)
            //{

            //    List<int> dataforDay = new List<int>();
            //    for (int j = 5; j <= 24; j++)
            //    {
            //        var forHour = item.SubList.FirstOrDefault(s => s.Hour == j);
            //        dataforDay.Add(forHour != null ? forHour.SubCount : 0);
            //    }
            //    dataForWeek.Add(dataforDay);

            //}




            //for (int i = 0; i <= 3; i++)
            //{
            //    DateTime date = startDateOFweek.AddDays(i);               
            //    var details = GetDetails(date, date.AddDays(1), empCd);
            //    List<int> dataforDay=new List<int>();
            //    for (int j = 5; j <= 24; j++)
            //    {
            //        var forHour= details.FirstOrDefault(s=>s.forHours==j);
            //        dataforDay.Add(forHour != null ? forHour.submissionCount : 0);
            //    }
            //        dataForWeek.Add(dataforDay);
            //}
            return Json(dataForWeek, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetOutCallForWeek(DateTime RequestedDate)
        {

            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            DateTime dateToday = RequestedDate;
            DateTime startDateOFweek = dateToday.Date.AddDays(DayOfWeek.Monday - dateToday.Date.DayOfWeek);

            DateTime endDateOFweek = startDateOFweek.AddDays(5);

            RolePrincipal r = (RolePrincipal)User;
            string role = r.GetRoles().FirstOrDefault();


            var temp = unitOfwork.CallStatistics
                                .GetCallStataticsForUser(empCd, startDateOFweek, endDateOFweek, role)
                                .Where(w => w.RC_CallType == "Out" && w.RC_Call_Connected == 1);


            var callStats = unitOfwork.CallStatistics
                                .GetCallStataticsForUser(empCd, startDateOFweek, endDateOFweek, role)
                                .Where(w => w.RC_CallType == "Out" && w.RC_Call_Connected == 1)
                                .GroupBy(g => g.RC_Date.Date).Select(s => new
                                {
                                    Date = s.Key,
                                    CallList = s.GroupBy(gc => gc.RC_Time.Hours).Select(ch => new
                                    {
                                        Hour = ch.Key,
                                        CallCount = ch.Count()
                                    })
                                });

            List<List<int>> dataForWeek = new List<List<int>>();
            //   var result =jdDataForWeek


            for (int day = 0; day <= 4; day++)
            {
                DateTime date = startDateOFweek.AddDays(day);
                List<int> dataforDay = new List<int>();

                var subForDay = callStats.FirstOrDefault(s => s.Date.Date == date);

                if (subForDay != null)
                {
                    for (int j = 5; j <= 24; j++)
                    {
                        var forHour = subForDay.CallList.FirstOrDefault(s => s.Hour == j);
                        dataforDay.Add(forHour != null ? forHour.CallCount : 0);
                    }
                }

                dataForWeek.Add(dataforDay);
            }


            return Json(dataForWeek, JsonRequestBehavior.AllowGet);
        }



        [Authorize]
        public ActionResult ViewDetailsPopup(DateTime fromDate, DateTime toDate, string data_Jr, string empCd, string company = null, string showDt = null)
        {

            //if (toDate > usDate)
            //    toDate = usDate.Date;

            // check for director code.
            string directorCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var _duser = unitOfwork.User.GetByEmpID(directorCd);
            string drole = _duser.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            if (empCd == null || empCd == "")
            {
                empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                ViewBag.showSubmittedBy = "Yes";
            }
            else if (drole == directorRoleName)
                ViewBag.showSubmittedBy = "Yes";
            else
                ViewBag.showSubmittedBy = "No";

            string headerText = null;

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            List<RIC_Job_Report> details = new List<RIC_Job_Report>();
            //  toDate = toDate.AddDays(1);

            // DateTime nextDay = date.AddDays(1);
            //if submission clicked.
            if (data_Jr == "Submission")
            {
                headerText = "Submissions";// For " + fromDate.ToString("MM-dd-yyyy");
                details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role)
                             .Where(s => s.RJ_Submit_Date >= fromDate && s.RJ_Submit_Date <= toDate).ToList();
            }
            else if (data_Jr == "interview")//if interview clicked.
            {
                headerText = "Interviews";
                details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role)
                            .Where(s => s.RJ_Interview_Date >= fromDate && s.RJ_Interview_Date <= toDate).ToList();
            }
            else if (data_Jr == "Hire")//if hire click.
            {
                headerText = "Hires";
                details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role)
                          .Where(s => s.RJ_Hire_Date >= fromDate && s.RJ_Hire_Date <= toDate).ToList();
            }

            // if company is not null then filter data by company
            if (company != null)
            {
                details = details.Where(s => s.RJ_Company.ToLower() == company.ToLower()).ToList();
            }
            if (showDt == "Y")
            {
                headerText = headerText + " For " + fromDate.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                toDate = toDate.AddMinutes(1);
                headerText = headerText + " For " + fromDate.ToString("hh tt", CultureInfo.InvariantCulture) +
                     " To " + toDate.ToString("hh tt", CultureInfo.InvariantCulture);
            }
            //if (fromDate.AddDays(1) != toDate)//add the header text
            //    headerText = headerText + " From " + fromDate.ToString("MM-dd-yyyy") + " To " + toDate.AddDays(-1).ToString("MM-dd-yyyy");
            //else
            //    headerText = headerText + " For " + fromDate.ToString("MM-dd-yyyy");

            ViewBag.Header = headerText;
            return PartialView("ViewDetailsPopup", details.OrderBy(s => s.RJ_Submitted_By).ThenBy(s => s.RJ_DateIssued));
        }

        //[Authorize]
        //[NonAction]
        //public List<RecruitDetails> GetDetails(DateTime fromDate, DateTime toDate, string empCd, string company = null)
        //{
        //    List<RecruitDetails> recDetails = new List<RecruitDetails>();


        //    //if (toDate > usDate)
        //    //    toDate = usDate.Date;
        //    // check for director code.
        //    string directorCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
        //    var _duser = unitOfwork.User.GetByEmpID(directorCd);
        //    string drole = _duser.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

        //    if (empCd == null || empCd == "")
        //    {
        //        empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
        //        ViewBag.showSubmittedBy = "Yes";
        //    }
        //    //else if (drole == directorRoleName)
        //    //    ViewBag.showSubmittedBy = "Yes";
        //    //else
        //    //    ViewBag.showSubmittedBy = "No";

        //    string headerText = null;

        //    var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
        //    string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

        //    List<RIC_Job_Report> details = new List<RIC_Job_Report>();

        //    List<JobRepoartWeek> jobReportHour = new List<JobRepoartWeek>();
        //    //List<SubmissionDetails> lstSubDetails = new List<SubmissionDetails>();
        //    //List<InterviewDetails> lstInterDetails = new List<InterviewDetails>();
        //    //List<HireDetails> lstHireDetails = new List<HireDetails>();

        //    //SubmissionDetails subDetails = new SubmissionDetails();
        //    //InterviewDetails interDetails = new InterviewDetails();
        //    //HireDetails hireDetails = new HireDetails();


        //    //      toDate = toDate.AddDays(1);

        //    // DateTime nextDay = date.AddDays(1);
        //    //if submission clicked.
        //    //if (data_Jr == "Submission")
        //    //{
        //    headerText = "Submissions";// For " + fromDate.ToString("MM-dd-yyyy");
        //    details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role)
        //                 .Where(s => s.RJ_Submit_Date.Date >= fromDate.Date && s.RJ_Submit_Date <= toDate).ToList();


        //    var subtmp = from x in details
        //                     orderby x.RJ_Submit_Date.Hour
        //                     group x by x.RJ_Submit_Date.Hour;


        //    foreach (var item in subtmp)
        //    {
        //        // take count and hour
        //        RecruitDetails recDet = new RecruitDetails();
        //        recDet.submissionCount = item.Count();
        //        recDet.forHours = item.Key;

        //        //added by ashish 26-06-2018 for popup.
        //        recDet.FromDate = fromDate.AddHours(item.Key);
        //        recDet.ToDate = fromDate.AddHours(item.Key + 1);
        //        recDet.ToDate = recDet.ToDate.AddMinutes(-1);

        //        recDetails.Add(recDet);
        //    }

        //    //headerText = "Interviews";
        //    //details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role)
        //    //         .Where(s => s.RJ_Interview_Date >= fromDate.Date && s.RJ_Interview_Date <= toDate).ToList();

        //    //var intertmp = from x in details
        //    //               orderby x.RJ_Interview_Date.Value.Hour
        //    //               group x by x.RJ_Interview_Date.Value.Hour;

        //    //foreach (var item in intertmp)
        //    //{
        //    //    // take count and hour
        //    //    RecruitDetails recDet1 = new RecruitDetails();
        //    //    recDet1.interviewCount = item.Count();
        //    //    recDet1.forHours = item.Key;

        //    //    //added by ashish 26-06-2018 for popup.
        //    //    recDet1.FromDate = fromDate.AddHours(item.Key);
        //    //    recDet1.ToDate = fromDate.AddHours(item.Key + 1);

        //    //    recDetails.Add(recDet1);
        //    //}



        //    //headerText = "Hires";
        //    //details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role)
        //    //        .Where(s => s.RJ_Hire_Date >= fromDate.Date && s.RJ_Hire_Date <= toDate).ToList();

        //    //var Hiretmp = from x in details
        //    //              orderby x.RJ_Hire_Date.Value.Hour
        //    //               group x by x.RJ_Hire_Date.Value.Hour;

        //    //foreach (var item in Hiretmp)
        //    //{
        //    //    // take count and hour
        //    //    RecruitDetails recDet2 = new RecruitDetails();
        //    //    recDet2.hireCount = item.Count();
        //    //    recDet2.forHours = item.Key;

        //    //    //added by ashish 26-06-2018 for popup.
        //    //    recDet2.FromDate = fromDate.AddHours(item.Key);
        //    //    recDet2.ToDate = fromDate.AddHours(item.Key + 1);

        //    //    recDetails.Add(recDet2);
        //    //}



        //    ViewBag.Header = headerText;

        //    return recDetails;
        //}



        //public string ToExcelFormat(DataTable OpReportTable)
        //{
        //    var columnHeaders = (from DataColumn x in OpReportTable.Columns select x.ColumnName).ToArray();
        //    StringBuilder builder = new StringBuilder(String.Join(" ", columnHeaders));
        //    builder.Append("\n"); 
        //    foreach (DataRow row in OpReportTable.Rows)
        //    {
        //        for (int i = 0; i < OpReportTable.Columns.Count; i++)
        //        {
        //            builder.Append(row[i].ToString());
        //            builder.Append(i == OpReportTable.Columns.Count - 1 ? "\n" : " ");
        //        }
        //    }
        //    return builder.ToString();
        //}



        public ActionResult ExportToExcel(DateTime RequestedDate)
        {
              ViewBag.Firstname = Request.QueryString["FirstName"];
            //DateTime RequestedDate = DateTime.Parse(HiddenDate);

            DateTime endDate = RequestedDate.AddDays(1).AddTicks(-1);

            ViewBag.date = RequestedDate;
            ViewBag.toDate = RequestedDate.AddDays(1);

            string empCd = User.Identity.Name;

            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

           

            List<OperationalViewPartial> opView = unitOfwork.RIC_Job_Report
                .GetOperationalSubmission(empCd, RequestedDate, endDate, role).Select(s => new OperationalViewPartial
                {

                    EmpCD = s.EmpCD,
                    MgrCD = s.MgrCD,
                    EmployeeName = s.EmployeeName,
                    ManagerName = s.ManagerName,
                    Total_Sub = s.Total_Sub,
                    Total_Vm = s.Total_Vm,
                    Total_Call = s.Total_Call,
                    Total_Prod = s.Total_Prod,
                    Hr_Sub_6_9_AM = s.Hr_Sub_6_9_AM,
                    Hr_Vm_6_9_AM = s.Hr_Vm_6_9_AM,
                    Hr_C_6_9_AM = s.Hr_C_6_9_AM,
                    Hr_P_6_9_AM = s.Hr_P_6_9_AM,
                    Hr_Sub_10_AM = s.Hr_Sub_10_AM,
                    Hr_Vm_10_AM = s.Hr_Vm_10_AM,
                    Hr_C_10_AM = s.Hr_C_10_AM,
                    Hr_P_10_AM = s.Hr_P_10_AM,
                    Hr_Sub_11_AM = s.Hr_Sub_11_AM,
                    Hr_Vm_11_AM = s.Hr_Vm_11_AM,
                    Hr_C_11_AM = s.Hr_C_11_AM,
                    Hr_P_11_AM = s.Hr_P_11_AM,
                    Hr_Sub_12_PM = s.Hr_Sub_12_PM,
                    Hr_Vm_12_PM = s.Hr_Vm_12_PM,
                    Hr_C_12_PM = s.Hr_C_12_PM,
                    Hr_P_12_PM = s.Hr_P_12_PM,
                    Hr_Sub_1_PM = s.Hr_Sub_1_PM,
                    Hr_Vm_1_PM = s.Hr_Vm_1_PM,
                    Hr_C_1_PM = s.Hr_C_1_PM,
                    Hr_P_1_PM = s.Hr_P_1_PM,
                    Hr_Sub_2_PM = s.Hr_Sub_2_PM,
                    Hr_Vm_2_PM = s.Hr_Vm_2_PM,
                    Hr_C_2_PM = s.Hr_C_2_PM,
                    Hr_P_2_PM = s.Hr_P_2_PM,
                    Hr_Sub_3_PM = s.Hr_Sub_3_PM,
                    Hr_Vm_3_PM = s.Hr_Vm_3_PM,
                    Hr_C_3_PM = s.Hr_C_3_PM,
                    Hr_P_3_PM = s.Hr_P_3_PM,
                    Hr_Sub_4_PM = s.Hr_Sub_4_PM,
                    Hr_Vm_4_PM = s.Hr_Vm_4_PM,
                    Hr_C_4_PM = s.Hr_C_4_PM,
                    Hr_P_4_PM = s.Hr_P_4_PM,
                    Hr_Sub_5_PM = s.Hr_Sub_5_PM,
                    Hr_Vm_5_PM = s.Hr_Vm_5_PM,
                    Hr_C_5_PM = s.Hr_C_5_PM,
                    Hr_P_5_PM = s.Hr_P_5_PM,
                    Hr_Sub_6_PM = s.Hr_Sub_6_PM,
                    Hr_Vm_6_PM = s.Hr_Vm_6_PM,
                    Hr_C_6_PM = s.Hr_C_6_PM,
                    Hr_P_6_PM = s.Hr_P_6_PM,
                    Hr_Sub_7_PM = s.Hr_Sub_7_PM,
                    Hr_Vm_7_PM = s.Hr_Vm_7_PM,
                    Hr_C_7_PM = s.Hr_C_7_PM,
                    Hr_P_7_PM = s.Hr_P_7_PM,
                    Hr_Sub_8_PM = s.Hr_Sub_8_PM,
                    Hr_Vm_8_PM = s.Hr_Vm_8_PM,
                    Hr_C_8_PM = s.Hr_C_8_PM,
                    Hr_P_8_PM = s.Hr_P_8_PM,
                    Hr_Sub_9_PM = s.Hr_Sub_9_PM,
                    Hr_Vm_9_PM = s.Hr_Vm_9_PM,
                    Hr_C_9_PM = s.Hr_C_9_PM,
                    Hr_P_9_PM = s.Hr_P_9_PM,
                    Hr_Sub_10_PM_12_AM = s.Hr_Sub_10_PM_12_AM,
                    Hr_Vm_10_PM_12_AM = s.Hr_Vm_10_PM_12_AM,
                    Hr_C_10_PM_12_AM = s.Hr_C_10_PM_12_AM,
                    Hr_P_10_PM_12_AM = s.Hr_P_10_PM_12_AM,
                    EmpLevel = s.EmpLevel
                }).ToList();
           


            dtVM.Columns.Add("EmployeeName", typeof(string));

            dtVM.Columns.Add("Total_Sub", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("Total_Call", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("Total_Vm", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("Total_Prod", typeof(int)).DefaultValue = 0;


            dtVM.Columns.Add("sub([6AM-9AM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([6AM-9AM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([6AM-9AM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([6AM-9AM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([10AM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([10AM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([10AM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([10AM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([11AM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([11AM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([11AM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([11AM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([12PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([12PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([12PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([12PM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([1PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([1PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([1PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([1PM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([2PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([2PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([2PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([2PM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([3PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([3PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([3PM])", typeof(int)).DefaultValue = 0;           
            dtVM.Columns.Add("prod([3PM])", typeof(int)).DefaultValue = 0;


            dtVM.Columns.Add("sub([4PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([4PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([4PM])", typeof(int)).DefaultValue = 0;           
            dtVM.Columns.Add("prod([4PM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([5PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([5PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([5PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([5PM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([6PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([6PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([6PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([6PM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([7PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([7PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([7PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([7PM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([8PM]", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([8PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([8PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([8PM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([9PM]", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([9PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([9PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([9PM])", typeof(int)).DefaultValue = 0;

            dtVM.Columns.Add("sub([10PM-12PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("call([10PM-12PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("vm([10PM-12PM])", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("prod([10PM-12APM])", typeof(int)).DefaultValue = 0;

            opViewData = opView;

            if (opView != null)
            {
                if (opView.Count() == 1)
                {
                    OperationalList(opView, "itemid");
                }
                else
                {
                    OperationalList(opView.Where(s => s.EmpLevel == 0).ToList(), "itemid");
                }
            }
           


            using (ExcelPackage pck = new ExcelPackage())
            {
                DataTable dt = dtVM;
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("OperationalDetailReport");
                ws.Cells["A1"].LoadFromDataTable(dt, true); //You can Use TableStyles property of your desire.    
                //Read the Excel file in a byte array    
                Byte[] fileBytes = pck.GetAsByteArray();

                var response = new FileContentResult(fileBytes, "application/octet-stream");
                response.FileDownloadName = "OperationalDetails.xlsx";// set the file name
                return response;// download file.
            }

           
           
        }

        public ActionResult OperationalList(List<OperationalViewPartial> opView,string emp=null)
        {
          
                 
            DataRow dr;
            

            foreach (var item in opView)
            {
                dr = dtVM.NewRow();
                var dataitem = opViewData.Where(s => s.MgrCD == item.EmpCD).ToList();

                if (dataitem.Count() > 0)
                {
                    dr[0] = item.EmployeeName;
                    dr[1] = item.Total_Sub;
                    dr[2] = item.Total_Call;
                    dr[3] = item.Total_Vm;
                    dr[4] = item.Total_Prod;


                    dr[5] = item.Hr_Sub_6_9_AM;
                    dr[6] = item.Hr_C_6_9_AM;
                    dr[7] = item.Hr_Vm_6_9_AM;
                    dr[8] = item.Hr_P_6_9_AM;

                    dr[9] = item.Hr_Sub_10_AM;
                    dr[10] = item.Hr_C_10_AM;
                    dr[11] = item.Hr_Vm_10_AM;
                    dr[12] = item.Hr_P_10_AM;

                    dr[13] = item.Hr_Sub_11_AM;
                    dr[14] = item.Hr_C_11_AM;
                    dr[15] = item.Hr_Vm_11_AM;
                    dr[16] = item.Hr_P_11_AM;

                    dr[17] = item.Hr_Sub_12_PM;
                    dr[18] = item.Hr_C_12_PM;
                    dr[19] = item.Hr_Vm_12_PM;
                    dr[20] = item.Hr_P_12_PM;

                    dr[21] = item.Hr_Sub_1_PM;
                    dr[22] = item.Hr_C_1_PM;
                    dr[23] = item.Hr_Vm_1_PM;
                    dr[24] = item.Hr_P_1_PM;

                    dr[25] = item.Hr_Sub_2_PM;
                    dr[26] = item.Hr_C_2_PM;
                    dr[27] = item.Hr_Vm_2_PM;
                    dr[28] = item.Hr_P_2_PM;

                    dr[29] = item.Hr_Sub_3_PM;
                    dr[30] = item.Hr_C_3_PM;
                    dr[31] = item.Hr_Vm_3_PM;
                    dr[32] = item.Hr_P_3_PM;

                    dr[33] = item.Hr_Sub_4_PM;
                    dr[34] = item.Hr_C_4_PM;
                    dr[35] = item.Hr_Vm_4_PM;
                    dr[36] = item.Hr_P_4_PM;

                    dr[37] = item.Hr_Sub_5_PM;
                    dr[38] = item.Hr_C_5_PM;
                    dr[39] = item.Hr_Vm_5_PM;
                    dr[40] = item.Hr_P_5_PM;

                    dr[41] = item.Hr_Sub_6_PM;
                    dr[42] = item.Hr_C_6_PM;
                    dr[43] = item.Hr_Vm_6_PM;
                    dr[44] = item.Hr_P_6_PM;

                    dr[45] = item.Hr_Sub_7_PM;
                    dr[46] = item.Hr_C_7_PM;
                    dr[47] = item.Hr_Vm_7_PM;
                    dr[48] = item.Hr_P_7_PM;

                    dr[49] = item.Hr_Sub_8_PM;
                    dr[50] = item.Hr_C_8_PM;
                    dr[51] = item.Hr_Vm_8_PM;
                    dr[52] = item.Hr_P_8_PM;

                    dr[53] = item.Hr_Sub_9_PM;
                    dr[54] = item.Hr_C_9_PM;
                    dr[55] = item.Hr_Vm_9_PM;
                    dr[56] = item.Hr_P_9_PM;

                    dr[57] = item.Hr_Sub_10_PM_12_AM;
                    dr[58] = item.Hr_C_10_PM_12_AM;
                    dr[59] = item.Hr_Vm_10_PM_12_AM;
                    dr[60] = item.Hr_P_10_PM_12_AM;

                    dtVM.Rows.Add(dr);
                    OperationalList(dataitem,item.EmpCD);
                }
                else
                {
                    dr[0] = item.EmployeeName;

                    dr[1] = item.Total_Sub;
                    dr[2] = item.Total_Call;
                    dr[3] = item.Total_Vm;
                    dr[4] = item.Total_Prod;


                    dr[5] = item.Hr_Sub_6_9_AM;
                    dr[6] = item.Hr_C_6_9_AM;
                    dr[7] = item.Hr_Vm_6_9_AM;
                    dr[8] = item.Hr_P_6_9_AM;

                    dr[9] = item.Hr_Sub_10_AM;
                    dr[10] = item.Hr_C_10_AM;
                    dr[11] = item.Hr_Vm_10_AM;
                    dr[12] = item.Hr_P_10_AM;

                    dr[13] = item.Hr_Sub_11_AM;
                    dr[14] = item.Hr_C_11_AM;
                    dr[15] = item.Hr_Vm_11_AM;
                    dr[16] = item.Hr_P_11_AM;

                    dr[17] = item.Hr_Sub_12_PM;
                    dr[18] = item.Hr_C_12_PM;
                    dr[19] = item.Hr_Vm_12_PM;
                    dr[20] = item.Hr_P_12_PM;

                    dr[21] = item.Hr_Sub_1_PM;
                    dr[22] = item.Hr_C_1_PM;
                    dr[23] = item.Hr_Vm_1_PM;
                    dr[24] = item.Hr_P_1_PM;

                    dr[25] = item.Hr_Sub_2_PM;
                    dr[26] = item.Hr_C_2_PM;
                    dr[27] = item.Hr_Vm_2_PM;
                    dr[28] = item.Hr_P_2_PM;

                    dr[29] = item.Hr_Sub_3_PM;
                    dr[30] = item.Hr_C_3_PM;
                    dr[31] = item.Hr_Vm_3_PM;
                    dr[32] = item.Hr_P_3_PM;

                    dr[33] = item.Hr_Sub_4_PM;
                    dr[34] = item.Hr_C_4_PM;
                    dr[35] = item.Hr_Vm_4_PM;
                    dr[36] = item.Hr_P_4_PM;

                    dr[37] = item.Hr_Sub_5_PM;
                    dr[38] = item.Hr_C_5_PM;
                    dr[39] = item.Hr_Vm_5_PM;
                    dr[40] = item.Hr_P_5_PM;

                    dr[41] = item.Hr_Sub_6_PM;
                    dr[42] = item.Hr_C_6_PM;
                    dr[43] = item.Hr_Vm_6_PM;
                    dr[44] = item.Hr_P_6_PM;

                    dr[45] = item.Hr_Sub_7_PM;
                    dr[46] = item.Hr_C_7_PM;
                    dr[47] = item.Hr_Vm_7_PM;
                    dr[48] = item.Hr_P_7_PM;

                    dr[49] = item.Hr_Sub_8_PM;
                    dr[50] = item.Hr_C_8_PM;
                    dr[51] = item.Hr_Vm_8_PM;
                    dr[52] = item.Hr_P_8_PM;

                    dr[53] = item.Hr_Sub_9_PM;
                    dr[54] = item.Hr_C_9_PM;
                    dr[55] = item.Hr_Vm_9_PM;
                    dr[56] = item.Hr_P_9_PM;

                    dr[57] = item.Hr_Sub_10_PM_12_AM;
                    dr[58] = item.Hr_C_10_PM_12_AM;
                    dr[59] = item.Hr_Vm_10_PM_12_AM;                  
                    dr[60] = item.Hr_P_10_PM_12_AM;

                    dtVM.Rows.Add(dr);
                }
            
            }
               

            return null;
        }



    }

	
}
