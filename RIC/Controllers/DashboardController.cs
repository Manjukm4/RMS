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
namespace RIC.Controllers
{
	[Authorize]
	public class DashboardController : Controller
	{
		UnitOfWork unitOfwork = new UnitOfWork();
		string mgrRoleName = System.Configuration.ConfigurationManager.AppSettings["ManagerRole"];
		string tlRoleName = System.Configuration.ConfigurationManager.AppSettings["TLRole"];
		string empRoleName = System.Configuration.ConfigurationManager.AppSettings["EmployeeRole"];
		string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
		int directorRoleID =int.Parse(System.Configuration.ConfigurationManager.AppSettings["DirectorID"]);
		int adminRoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AdminRoleID"]);
		string AdminRoleName = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
		string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
		string HrRoleName = System.Configuration.ConfigurationManager.AppSettings["HRRole"];
        string stafingDirectorRole = System.Configuration.ConfigurationManager.AppSettings["StaffingDirector"];
		DateTime usDate = SystemClock.US_Date;

		 
		[Authorize]
        public ActionResult Index(string empCd = null, string getIndividualRecord = null,string FirstName=null,string FilterName=null,string page=null,string Role=null,string ReportingID=null,string EmpId=null,bool IsActive=false)
		{
			Session.Timeout = 80;
			DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
			// get the employee id.
			if (empCd == null)
				empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
			else
				ViewBag.ShowUserName = "Y";//show the user name.
			DashBoardViewModel dashboardView = new DashBoardViewModel();
			// temp need to change 
			var _user = unitOfwork.User.GetByEmpID(empCd);//.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
			RolePrincipal r = (RolePrincipal)User;
			var rolesArray = r.GetRoles();
			int EmpID= _user.RE_EmpId;
			ViewBag.userName = _user.RE_Jobdiva_User_Name;
            ViewBag.getIndividualRecords = getIndividualRecord;
			//get roles for user..
			string role=_user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

			var ReportingList = from rh in unitOfwork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role,null,EmpID)
															   select rh;
		 

			dashboardView.InterimFeedbackNotification = (from feedback in unitOfwork.RIC_PersonalFeedback.Get()
							   join rhe in ReportingList
							   on feedback.RP_EmployeeID equals rhe.RR_EmpCD
							   join emp in unitOfwork.User.Get() on feedback.RP_EmployeeID equals emp.RE_Emp_Cd into empList
							   from employeeList in empList.DefaultIfEmpty()
							   join dep in unitOfwork.Department.Get() on employeeList.RE_DepartmentID equals dep.RD_ID into depList
							   from departmentList in depList.DefaultIfEmpty()
							   where employeeList.RE_Resign_Date == null && (rhe.RR_ToDate == null || rhe.RR_ToDate >= usDate.Date)  && feedback.RP_Status.ToUpper()== "COMPLETED" 
										&& Convert.ToInt32((feedback.RP_NextReviewDate - usDate.Date).TotalDays) >=0
										&& Convert.ToInt32((feedback.RP_NextReviewDate - usDate.Date).TotalDays) <= 10
							   select new InterimFeedbackNotification
							   {
								 EmpId= feedback.RP_EmployeeID,
								 //ManagerId= rhe.RR_MgrCD,
								 EmployeeName= rhe.Employee_Name,
								 RRStartDate= Convert.ToDateTime(rhe.RR_FromDate),
								 RREndDate= Convert.ToDateTime(rhe.RR_ToDate),
								 ReviewDate=feedback.RP_ReviewDate,
								 NextReviewDate=feedback.RP_NextReviewDate,
								 Department=departmentList.RD_Department
							   }).ToList();

			dashboardView.NewJoineeInterimNotification = ( from rhe in ReportingList
								join emp in unitOfwork.User.Get() on rhe.RR_EmpCD equals emp.RE_Emp_Cd		
								join feedback in unitOfwork.RIC_PersonalFeedback.Get() on rhe.RR_EmpCD equals feedback.RP_EmployeeID into feedbackList
								where emp.RE_Resign_Date == null && (emp.RE_DepartmentID == 1 || emp.RE_DepartmentID == 21)
										&& feedbackList.Count() == 0
										&& (rhe.RR_ToDate == null || rhe.RR_ToDate >= usDate.Date)
										&& Convert.ToInt32((usDate.Date- Convert.ToDateTime(emp.RE_Joining_Date).AddMonths(3).AddDays(-10)).TotalDays) >=0
										&& Convert.ToInt32((usDate.Date - Convert.ToDateTime(emp.RE_Joining_Date).AddMonths(3).AddDays(-10)).TotalDays) <= 10
								select new NewJoineeInterimNotification
								{ 
									EmpId=rhe.RR_EmpCD,
									EmployeeName=emp.RE_Employee_Name,
									JoiningDate=Convert.ToDateTime(emp.RE_Joining_Date),
									ReviewDate= Convert.ToDateTime(emp.RE_Joining_Date).AddMonths(3)
								}).ToList();




			if (Session["View"] == null)
			{
				Session["View"] = "View";
			}
			else
			{
				Session["View"] = "Not Viewed";
			}


			int FeedbackCount = dashboardView.InterimFeedbackNotification.Count();
			int NewJoineeFeedbackCount = dashboardView.NewJoineeInterimNotification.Count();

			ViewBag.PopupBoolean = (FeedbackCount > 0 || NewJoineeFeedbackCount > 0) ? true : false;
			ViewBag.ViewStatus =Session["View"].ToString() == "View" ? true : false;
			ViewBag.EmployeeRoleStatus = (role == mgrRoleName || role == tlRoleName) ? true : false;


			var targetForUser =getTragetForUser(empCd,usDate.Date,role,getIndividualRecord,EmpID);
            dashboardView.RS_SubmissionTr = targetForUser.Item1;
            dashboardView.RS_InterviewsTr = targetForUser.Item2;
            dashboardView.RS_HiresTr = targetForUser.Item3;            
			// add the last 2 month details.            
			for (int i = 1; i <= 2; i++)
			{
				DateTime lastMonthDate = SystemClock.US_Date.AddMonths(-i);
				DateTime lastMonthStartDate = new DateTime(lastMonthDate.Year, lastMonthDate.Month, 1);
				DateTime lastMonthEndDate = new DateTime(lastMonthDate.Year, lastMonthDate.Month,
													 DateTime.DaysInMonth(lastMonthDate.Year, lastMonthDate.Month));
				DateTime? joiningDate = _user.RE_Joining_Date;
				if (joiningDate != null && joiningDate >= lastMonthEndDate)
				{
					// if joining date is greater than the end of month then show null.
					dashboardView.lastTwoMonthData.Add(new SubmissionData()
					{
						date = SystemClock.US_Date.AddMonths(-i),
						hires = null,
						interviews = null,
						submission = null
					});
				}
				else
				{
					//add the extra day in end of month.
					lastMonthEndDate = lastMonthEndDate.AddDays(1);
					// if end of month is greter than date then change the end of month to todays date.
					if (lastMonthEndDate >= SystemClock.US_Date.Date)
						lastMonthEndDate = SystemClock.US_Date.Date;
					//get the submission interview and hire count for user.
                    //if get individual records =="yes" then filter the records by employee id
                    var jr = unitOfwork.RIC_Job_Report
                            .Get_TeamPerformanceResult(lastMonthStartDate, lastMonthEndDate, empCd, role, EmpID)
                            .Where(w=>getIndividualRecord=="Yes"?w.EmpCd==empCd:true).ToList();
                    if (!IsActive)
                    {
						jr = jr.Where(x => x.ResignDate.ToString() == "").ToList();

					}
                    int submissions = jr.Sum(s => s.Submissions);
                    int interviews = jr.Sum(s => s.Interviews);
                    int hires = jr.Sum(s => s.Hires);
                    int submissionTarget = jr.Sum(s => s.SubmissionTarget);
                    int interviewTarget = jr.Sum(s => s.InterviewTarget);
                    int hiresTarget = jr.Sum(s => s.HiresTarget);
                    //var lastMonthJRepoart = unitOfwork.RIC_Job_Report
                    //     .Get_JobRepoartForUser(empCd, lastMonthStartDate, lastMonthEndDate, role)
                    //     .Where(w=>getIndividualRecord=="Yes"?w.RJ_EmpCd==empCd:true);
                    //int submissions = lastMonthJRepoart
                    //                  .Count(subCount => subCount.RJ_Submit_Date != null 
                    //                        && subCount.RJ_Submit_Date >= lastMonthStartDate 
                    //                        && subCount.RJ_Submit_Date <= lastMonthEndDate
                    //                        );
                    //int Interviews = lastMonthJRepoart
                    //                 .Count(intCount => intCount.RJ_Interview_Date != null 
                    //                    && intCount.RJ_Interview_Date >= lastMonthStartDate 
                    //                    && intCount.RJ_Interview_Date <= lastMonthEndDate
                    //                    );
                    //int Hires = lastMonthJRepoart
                    //            .Count(hireCount => hireCount.RJ_Hire_Date != null 
                    //                && hireCount.RJ_Hire_Date >= lastMonthStartDate 
                    //                && hireCount.RJ_Hire_Date <= lastMonthEndDate
                    //                );


					dashboardView.lastTwoMonthData.Add(new SubmissionData()
						{
							date = SystemClock.US_Date.AddMonths(-i),
							hires = hires,
							interviews = interviews,
							submission = submissions,
                            SubmissionTarget=submissionTarget,
                            InterviewTarget=interviewTarget,
                            HiresTarget=hiresTarget
						});
				}
			}
            ViewBag.ProgressReport = true;//role == tlRoleName && getIndividualRecord == null;

            ViewBag.ReportingID = ReportingID;
            ViewBag.Page = page;
            ViewBag.Empcode = EmpId;
            ViewBag.Firstname = FirstName;
            ViewBag.FilterName = FilterName;
            ViewBag.RoleIdFilter = Role;

            //set the default 90 daya for progress report.
            //dashboardView.ProgressReportFromDate = usDate.Date.AddDays(-90);
            //dashboardView.ProgressReportToDate = usDate.Date;

			DateTime now = DateTime.Now;
			var startDate = new DateTime(now.Year, now.Month, 1);
			DateTime startOfMonth = startDate;
			DateTime endOfMonth = LastDayOfMonth(startOfMonth);
			dashboardView.ProgressReportFromDate = startOfMonth;
			dashboardView.ProgressReportToDate = endOfMonth;
			return View(dashboardView);
		}
	 
		public ActionResult TeamDashboardGraph()
		{
			string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
			TeamDashboardViewModel tdView = new TeamDashboardViewModel();

			//List<int> submissionList=new List<int>();
			//List<int> interviewList = new List<int>();
			//List<int> hireList = new List<int>();

			DateTime date = SystemClock.US_Date.Date.AddDays(1);
			DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
			DateTime endOfMonth = date; //new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

			//get the list of employees for team lead.
			var employees = unitOfwork.User.getAllUsers().Where(s => (s.MgrCD == empCd) && (s.RE_Emp_Cd != empCd))
				 .Select(s => new { jobdivaUserName = s.RE_Jobdiva_User_Name, employeeCd = s.RE_Emp_Cd }).ToList();

			foreach (var emp in employees)
			{
				// add the submissions interviews and hires for users.
				var jrRepart = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(emp.employeeCd, startOfMonth, endOfMonth);
				var callStatistics = unitOfwork.CallStatistics.GetCallStataticsForUser(emp.employeeCd, startOfMonth, endOfMonth);

				//get the total submission /interview and hires in current month.
				int sub = jrRepart.Where(s => s.RJ_Submit_Date >= startOfMonth && s.RJ_Submit_Date <= endOfMonth).Count();
				int interview = jrRepart.Where(s => s.RJ_Interview_Date >= startOfMonth && s.RJ_Interview_Date <= endOfMonth).Count();
				int hire = jrRepart.Where(s => s.RJ_Hire_Date >= startOfMonth && s.RJ_Hire_Date <= endOfMonth).Count();

				int outCall = callStatistics.Where(s => s.RC_CallType == "Out").Sum(s => s.RC_Call_Connected);
				int voiceMessage = callStatistics.Where(s => s.RC_CallType == "Out").Sum(s => s.RC_Voice_Message);

				TimeSpan outDuration = callStatistics.Where(s => s.RC_CallType == "Out").Select(item => item.RC_Duration)
								.Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));
				TimeSpan inDuration = callStatistics.Where(s => s.RC_CallType == "In").Select(item => item.RC_Duration)
								.Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));

				tdView.Submissions.Add(sub);
				tdView.Interviews.Add(interview);
				tdView.Hires.Add(hire);

				tdView.OutCalls.Add(outCall);
				tdView.OutVoiceMessage.Add(voiceMessage);

				tdView.OutCallDuration.Add(outDuration);
				tdView.InCallDuration.Add(inDuration);
			}
			// add the employee Names in dashboard.
			tdView.employeeName = employees.Select(s => s.jobdivaUserName).ToList();
			return View(tdView);
		}

		public ActionResult TeamDashboard()
		{
			// DateTime date = SystemClock.US_Date.Date;
			// string  tlCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;          
			// DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();

			//DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
			//DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			//var view = dashboardIndex.getJobRepoartForTeamLead(tlCd, startOfMonth, endOfMonth);

			//string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

			//List<TreeViewModel> treeView = new List<TreeViewModel>();
			//treeView.Add(new TreeViewModel()
			//{
			//    MgrName = unitOfwork.User.GetByEmpID(empCd),
			//    Employees = unitOfwork.User.getAllUsers().Where(s => (s.MgrCD == empCd) && (s.RE_Emp_Cd != empCd)).ToList()
			//});
			// IEnumerable<RIC_Employee> employees = 
			return View();
		}
		public ActionResult AccesslogDashboard()
		{
			// DateTime date = SystemClock.US_Date.Date;
			// string  tlCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;          
			// DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();

			//DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
			//DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			//var view = dashboardIndex.getJobRepoartForTeamLead(tlCd, startOfMonth, endOfMonth);

			//string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

			//List<TreeViewModel> treeView = new List<TreeViewModel>();
			//treeView.Add(new TreeViewModel()
			//{
			//    MgrName = unitOfwork.User.GetByEmpID(empCd),
			//    Employees = unitOfwork.User.getAllUsers().Where(s => (s.MgrCD == empCd) && (s.RE_Emp_Cd != empCd)).ToList()
			//});
			// IEnumerable<RIC_Employee> employees = 
			return View();
		}
		
		public ActionResult DirectorDashboard()
		{
			return View();
		}

		public ActionResult jobReportTeamPartial(int month)
		{
			DateTime date = SystemClock.US_Date.Date.AddMonths(month);
			string empCd = User.Identity.Name;
			var _user = unitOfwork.User.GetByEmpID(empCd);
			int empid = _user.RE_EmpId;
			RolePrincipal rolePrincipal = (RolePrincipal)User;
				var role = rolePrincipal.GetRoles()[0];
			DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
			DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
			DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			//get the submission data by weeks.
			ViewBag.Role = role;
			var view = dashboardIndex.getJobRepoartForTeamLead(empCd, startOfMonth, endOfMonth,role, empid);
			//get holidays from db.
			List<RIC_Holidays> hList = unitOfwork.Holidays.Get().Where(s => s.RH_Date.Month == date.Month).ToList();
			var hList1 = hList.Select(s=>s.RH_Date).ToList();
			ViewData["Holidays"] = hList1;
			return PartialView(view);
		}

		//[HttpPost]
		//public ActionResult GetAccessLog(int  empcd)
		//{
		//	// Do some stuff

		//	return PartialView();
		//}
		[HttpPost]
		public ActionResult GetAccessLog(string fromdate, string empCd)
		{
			//DateTime date1 =DateTime.Parse(fromdate);
			//DateTime date = SystemClock.US_Date.AddMonths(date1);

			CultureInfo culture = new CultureInfo("en-US");
			DateTime date = Convert.ToDateTime(fromdate, culture);

			string empCd1 = User.Identity.Name;
			RolePrincipal rolePrincipal = (RolePrincipal)User;
			var role = rolePrincipal.GetRoles()[0];
			DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
			DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
			DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			//get the submission data by weeks.
			var view = dashboardIndex.getJobRepoartForTeamLead_Accesslog(empCd1, startOfMonth, endOfMonth, role);
			return Json(view);
		}

		public ActionResult DirectorTablePartial(int month)
		{
			DirectorDashboardModel model = new DirectorDashboardModel();
			//get the employee code.
			string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

			var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
			string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

			//get the date of current month.
			DateTime date = SystemClock.US_Date.Date.AddMonths(month);
			DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
			DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

			model.FromDate = startOfMonth;
			model.ToDate = endOfMonth;

			// DateTime endOfMonth2 = endOfMonth;
			endOfMonth = endOfMonth.AddDays(1);// add the days in end of month.

			// if end of month is greter than date then change the end of month to todays date.
			if (endOfMonth >= SystemClock.US_Date.Date)
				endOfMonth = SystemClock.US_Date.Date.AddDays(1);

			endOfMonth = endOfMonth.AddSeconds(-1);

			model.DirectorDashboardTable = new List<DirectorDashboardTable>();

			//get the manager list from db.
			var managerList = unitOfwork.context.RIC_Employee
				 .Where(s => s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == tlRoleName).ToList().Select(mgr => new RIC_ReportingHistory()
											{
												RR_ID = mgr.RE_EmpId,
												RR_EmpCD = mgr.RE_Emp_Cd,
												RR_MgrCD = mgr.RE_Emp_Cd,
												Employee_Name = mgr.RE_Jobdiva_User_Name,
												Mgr_Name = mgr.RE_Jobdiva_User_Name,
												RR_FromDate = startOfMonth,
												RR_ToDate = endOfMonth
											});

			//merge manager list with user list.
			var reportingHistory = ((from emp in unitOfwork.context.RIC_Employee
											 join rh in unitOfwork.context.RIC_ReportingHistory
											 on emp.RE_Emp_Cd equals rh.RR_EmpCD
											 join mgr in unitOfwork.context.RIC_Employee
											 on rh.RR_MgrCD equals mgr.RE_Emp_Cd
											 select new
											 {
												 id = rh.RR_ID,
												 empCd = rh.RR_EmpCD,
												 mgrCd = rh.RR_MgrCD,
												 EmpName = emp.RE_Employee_Name,
												 MgrName = mgr.RE_Jobdiva_User_Name,
												 FromDt = rh.RR_FromDate,
												 ToDt = rh.RR_ToDate
											 }).ToList().Select(emp => new RIC_ReportingHistory
											{
												RR_ID = emp.id,
												RR_EmpCD = emp.empCd,
												RR_MgrCD = emp.mgrCd,
												Employee_Name = emp.EmpName,
												Mgr_Name = emp.MgrName,
												RR_FromDate = emp.FromDt,
												RR_ToDate = emp.ToDt
											}).Union(
												  managerList
											)).ToList();

			// get the submission ,interview and hire based on manager.
			model.DirectorDashboardTable = new List<DirectorDashboardTable>();
			model.DirectorDashboardTable =
								 (
								  from jr in unitOfwork.RIC_Job_Report.GetAll()
								  join mgr in reportingHistory

								  on jr.RJ_EmpCd equals mgr.RR_EmpCD
								  where
								  (// submission data.
											((jr.RJ_Submit_Date >= mgr.RR_FromDate)
										&& (jr.RJ_Submit_Date <= mgr.RR_ToDate)
										&& (jr.RJ_Submit_Date >= startOfMonth)
										&& (jr.RJ_Submit_Date <= endOfMonth))
										||//interview data.
											((jr.RJ_Interview_Date >= mgr.RR_FromDate)
										&& (jr.RJ_Interview_Date <= mgr.RR_ToDate)
										&& (jr.RJ_Interview_Date >= startOfMonth)
										&& (jr.RJ_Interview_Date <= endOfMonth))
										||// hire data
											((jr.RJ_Hire_Date >= mgr.RR_FromDate)
										&& (jr.RJ_Hire_Date <= mgr.RR_ToDate)
										&& (jr.RJ_Hire_Date >= startOfMonth)
										&& (jr.RJ_Hire_Date <= endOfMonth))
								  )
								  select new { jrData = jr, MgrData = mgr }
								  )
								  .GroupBy(s => new { s.MgrData.RR_MgrCD, s.MgrData.Mgr_Name })//, StringComparer.InvariantCultureIgnoreCase)// ignore the case of string.
						  .Select(s => new DirectorDashboardTable()
						  {
							  EmpCd = s.Key.RR_MgrCD,
							  UserName = s.Key.Mgr_Name,
							  Submissions = s.Count(subCount => subCount.jrData.RJ_Submit_Date != null && subCount.jrData.RJ_Submit_Date >= startOfMonth && subCount.jrData.RJ_Submit_Date <= endOfMonth),
							  Interviews = s.Count(intCount => intCount.jrData.RJ_Interview_Date != null && intCount.jrData.RJ_Interview_Date >= startOfMonth && intCount.jrData.RJ_Interview_Date <= endOfMonth),
							  Hires = s.Count(hireCount => hireCount.jrData.RJ_Hire_Date != null && hireCount.jrData.RJ_Hire_Date >= startOfMonth && hireCount.jrData.RJ_Hire_Date <= endOfMonth),
							  // FromDate=startOfMonth,
							  // ToDate=endOfMonth2,
							  //  EmpCd=empCd
						  }).ToList();

			model.DirectorDashboardTable =
								(
								 from jr in unitOfwork.RIC_Job_Report.GetAll()
								 join mgr in reportingHistory

								 on jr.RJ_EmpCd equals mgr.RR_EmpCD
								 where
								 (// submission data.
										  ((jr.RJ_Submit_Date >= mgr.RR_FromDate)
									  && (jr.RJ_Submit_Date <= mgr.RR_ToDate)
									  && (jr.RJ_Submit_Date >= startOfMonth)
									  && (jr.RJ_Submit_Date <= endOfMonth))
									  ||//interview data.
										  ((jr.RJ_Interview_Date >= mgr.RR_FromDate)
									  && (jr.RJ_Interview_Date <= mgr.RR_ToDate)
									  && (jr.RJ_Interview_Date >= startOfMonth)
									  && (jr.RJ_Interview_Date <= endOfMonth))
									  ||// hire data
										  ((jr.RJ_Hire_Date >= mgr.RR_FromDate)
									  && (jr.RJ_Hire_Date <= mgr.RR_ToDate)
									  && (jr.RJ_Hire_Date >= startOfMonth)
									  && (jr.RJ_Hire_Date <= endOfMonth))
								 )
								 select new { jrData = jr, MgrData = mgr }
								 )
								 .GroupBy(s => new { s.MgrData.RR_MgrCD, s.MgrData.Mgr_Name })//, StringComparer.InvariantCultureIgnoreCase)// ignore the case of string.
						 .Select(s => new DirectorDashboardTable()
						 {
							 EmpCd = s.Key.RR_MgrCD,
							 UserName = s.Key.Mgr_Name,
							 Submissions = s.Count(subCount => subCount.jrData.RJ_Submit_Date != null && subCount.jrData.RJ_Submit_Date >= startOfMonth && subCount.jrData.RJ_Submit_Date <= endOfMonth),
							 Interviews = s.Count(intCount => intCount.jrData.RJ_Interview_Date != null && intCount.jrData.RJ_Interview_Date >= startOfMonth && intCount.jrData.RJ_Interview_Date <= endOfMonth),
							 Hires = s.Count(hireCount => hireCount.jrData.RJ_Hire_Date != null && hireCount.jrData.RJ_Hire_Date >= startOfMonth && hireCount.jrData.RJ_Hire_Date <= endOfMonth),
							 // FromDate=startOfMonth,
							 // ToDate=endOfMonth2,
							 //  EmpCd=empCd
						 }).ToList();

			return PartialView(model);
		}

        public ActionResult JobReportPartial(int month, string empCd, string idText, string getIndividualRecord)
		{
			JobReportPartial jrView = new JobReportPartial();
			jrView.idText = idText;
			//get the date of current month.
			DateTime date = SystemClock.US_Date.AddMonths(month);
			ViewBag.date = date;
			if (empCd == null || empCd == "") //get the employee code from authentication.          
				empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
			DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
			//get the start date of month.
			DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
			//get end date of month.
			DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			//get the user details.
			var _user = unitOfwork.User.GetByEmpID(empCd);
			int EmpID = _user.RE_EmpId;
			// get the joining date.
			DateTime? joiningDate = _user.RE_Joining_Date;
			if (joiningDate != null && joiningDate >= endOfMonth.AddMonths(-1))
				ViewBag.ShowPreviousBtn = "N"; // hide the show back button.
			//get the role for user.
			string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;//get the role for user.
			//get the dashboard matrics for user
            var DashboardMatrics = dashboardIndex.getDashboardMatrics(empCd, startOfMonth, endOfMonth, role, getIndividualRecord, EmpID);
			//get the submission matrics 
			jrView.JobReportWeek = DashboardMatrics.Item1;
			// get the call matrics
			jrView.CallStatisticsWeek = DashboardMatrics.Item2;
			ViewBag.showProdRecruter = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name != empRoleName;
			return PartialView("JobReportPartial", jrView);
		}
		public ActionResult AccesslogJobReportPartial(int month, string empCd, string idText, string getIndividualRecord)
		{
			JobReportPartial jrView = new JobReportPartial();
			jrView.idText = idText;
			//get the date of current month.
			DateTime date = SystemClock.US_Date.AddMonths(month);
			ViewBag.date = date;
			if (empCd == null || empCd == "") //get the employee code from authentication.          
				empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
			DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
			//get the start date of month.
			DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
			//get end date of month.
			DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			//get the user details.
			var _user = unitOfwork.User.GetByEmpID(empCd);
			// get the joining date.
			DateTime? joiningDate = _user.RE_Joining_Date;
			if (joiningDate != null && joiningDate >= endOfMonth.AddMonths(-1))
				ViewBag.ShowPreviousBtn = "N"; // hide the show back button.
											   //get the role for user.
			string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;//get the role for user.
																					 //get the dashboard matrics for user
			var DashboardMatrics = dashboardIndex.getDashboardMatrics(empCd, startOfMonth, endOfMonth, role, getIndividualRecord);
			//get the submission matrics 
			jrView.JobReportWeek = DashboardMatrics.Item1;
			// get the call matrics
			jrView.CallStatisticsWeek = DashboardMatrics.Item2;
			ViewBag.showProdRecruter = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name != empRoleName;
			return PartialView("JobReportPartial", jrView);
		}
		public ActionResult GaugeGraphPartial(int month, string empCd, string getIndividualRecord)
		{
			DateTime date = SystemClock.US_Date.AddMonths(month);
			DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
			DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			if (empCd == null || empCd == "")
				empCd = User.Identity.Name;

			GaugeViewModel gaugeView = new GaugeViewModel();

		//	var subRule = unitOfwork.User.GetByEmpID(empCd).RIC_SubmissionRule;
			// temp need to change 
			var _user = unitOfwork.User.GetByEmpID(empCd);//.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
														  //	RolePrincipal r = (RolePrincipal)User;
														  //var rolesArray = r.GetRoles();
			int EmpID = _user.RE_EmpId;
			string userRole= _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
			// if user role is manager.
            //var targetForUser = getTragetForUser(empCd, usDate.Date, userRole, getIndividualRecord);


             
			//string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
			DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
			gaugeView.StartDateOfMonth = startOfMonth;
			gaugeView.EndDateOfMonth = endOfMonth;
			// get the count of remaining days in month.//commented by ashish 27-06-2018
			//gaugeView.RemainingDays = jrWeek.FirstOrDefault().remainingDays;
			gaugeView.RemainingDays = getRemainingDays(startOfMonth, endOfMonth);

			string lableText = "Target for ";

			if (month == 0)
			{
				//lableText += "this month (" + jrWeek.FirstOrDefault().remainingDays + " days to go)";//commented by ashish 27-06-2018
				lableText += "this month (" + getRemainingDays(startOfMonth, endOfMonth) + " days to go)";
			}
			else
			{
				lableText += CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month);
			}
			if (endOfMonth > SystemClock.US_Date)
				//endOfMonth = SystemClock.US_Date.Date.AddDays(-1);
				endOfMonth = SystemClock.US_Date.Date;
			endOfMonth = endOfMonth.AddDays(1);

            var jr = unitOfwork.RIC_Job_Report.Get_TeamPerformanceResult(startOfMonth, endOfMonth, empCd, userRole,EmpID)
                      .Where(w => getIndividualRecord == "Yes" ? w.EmpCd == empCd : true).ToList();

            gaugeView.SubmissionTarget = jr.Sum(s=>s.SubmissionTarget);
            gaugeView.InterviewsTarget =jr.Sum(s=>s.InterviewTarget);
            gaugeView.HiresTarget = jr.Sum(s=>s.HiresTarget);

            gaugeView.TotalSubmissionMonth = jr.Sum(s => s.Submissions);
            gaugeView.TotalInterviewsMonth = jr.Sum(s => s.Interviews);
            gaugeView.TotalHiresMonth = jr.Sum(s => s.Hires);

            //gaugeView.SubmissionTarget = targetForUser.Item1;
            //gaugeView.InterviewsTarget = targetForUser.Item2;
            //gaugeView.HiresTarget = targetForUser.Item3; 


            //get the submission interview and hire count for user.
            //if get individual records =="yes" then filter the records by employee id
            //var JRepoart = unitOfwork.RIC_Job_Report
            //     .Get_JobRepoartForUser(empCd, startOfMonth, endOfMonth, userRole)
            //     .Where(w => getIndividualRecord == "Yes" ? w.RJ_EmpCd == empCd : true);
            //// get the submission /interviews and hire count.
            //gaugeView.TotalSubmissionMonth = JRepoart
            //                    .Count(subCount => subCount.RJ_Submit_Date != null 
            //                            && subCount.RJ_Submit_Date >= startOfMonth 
            //                            && subCount.RJ_Submit_Date <= endOfMonth
            //                           );
            //gaugeView.TotalInterviewsMonth = JRepoart
            //                  .Count(intCount => intCount.RJ_Interview_Date != null 
            //                          && intCount.RJ_Interview_Date >= startOfMonth 
            //                          && intCount.RJ_Interview_Date <= endOfMonth
            //                         );
            //gaugeView.TotalHiresMonth = JRepoart
            //              .Count(hireCount => hireCount.RJ_Hire_Date != null
            //                      && hireCount.RJ_Hire_Date >= startOfMonth
            //                      && hireCount.RJ_Hire_Date <= endOfMonth
            //                    );


			ViewBag.lableText = lableText;
			if (!User.IsInRole(empRoleName))
				ViewBag.showTreeview = true;
			else ViewBag.showTreeview = false;

			return PartialView(gaugeView);
		}

		public static DateTime LastDayOfMonth(DateTime dt)
		{
			DateTime ss = new DateTime(dt.Year, dt.Month, 1);
			return ss.AddMonths(1).AddDays(-1);
		}
		public ActionResult ProgressReportPartial(DateTime fromDate, DateTime toDate, string empCd)
        {
			int Year = usDate.Year;
			DateTime now = DateTime.Now;
			var startDate = new DateTime(now.Year, now.Month, 1);
			string Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month);
			DateTime startOfMonth = startDate;
			var endOfMonth = LastDayOfMonth(startOfMonth);//int monthInt;
														  //DateTime endOfMonth = DateTime.DaysInMonth(now.Year,now.Month);
														  // DateTime date = SystemClock.US_Date.Date.AddMonths(-1);
														  ////DateTime startOfMonth = fromDate;
														  //DateTime startOfMonth = fromDate;

			//DateTime endOfMonth = toDate.AddDays(1);
			startDate = fromDate;
			Month=DateTimeFormatInfo.CurrentInfo.GetMonthName(fromDate.Month);

			if (empCd == null || empCd == "")
             empCd = User.Identity.Name;

            var _user = unitOfwork.User.GetByEmpID(empCd);//.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
														  //  RolePrincipal r = (RolePrincipal)User;
														  // var rolesArray = r.GetRoles();
														  // string userRole = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
														  //DateTime fd =(DateTime) fromDate;
														  //DateTime td = (DateTime) toDate;

			//RolePrincipal r = (RolePrincipal)User;
			int EmpID = _user.RE_EmpId;
			string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
			//get the performance result from db.
			//var jr=unitOfwork.RIC_Job_Report.Get_TeamPerformanceResult(startOfMonth, endOfMonth, empCd, role).ToList();
			var jr = unitOfwork.RIC_Job_Report.Get_TeamPerformanceResult(fromDate, toDate, empCd, role,EmpID).ToList();

			//bind the model
			ProgressReportPartialVM progressReport = new ProgressReportPartialVM()
			{             
				SubmissionTarget = jr.Sum(s => s.SubmissionTarget),
                InterviewTarget = jr.Sum(s => s.InterviewTarget),
                HiresTarget = jr.Sum(s => s.HiresTarget),
                Submissions = jr.Sum(s => s.Submissions),
                Interviews = jr.Sum(s => s.Interviews),
                Hires = jr.Sum(s => s.Hires),
                SubmissionsPer = jr.Sum(s => s.SubmissionTarget) != 0 ? (int)((float)(jr.Sum(s => s.Submissions) / (float)jr.Sum(s => s.SubmissionTarget)) * 100) : 100,
                InterviewsPer = jr.Sum(s => s.InterviewTarget) != 0 ? (int)(((float)jr.Sum(s => s.Interviews) / (float)jr.Sum(s => s.InterviewTarget)) * 100) : 100,
                HiresPer = jr.Sum(s => s.HiresTarget) != 0 ? (int)(((float)jr.Sum(s => s.Hires) / (float)jr.Sum(s => s.HiresTarget)) * 100) : 100,
                FromDate = fromDate,
                ToDate = toDate,
                EmployeeList = jr.Select(s => new ProgressReportEmployeeList
                {
                    EmpCd = s.EmpCd,
                    EmployeeName = s.EmployeeName,
                    FromDate = s.FromDate,
                    ToDate = s.ToDate,
                    SubmissionsTarget = s.SubmissionTarget,
                    InterviewTarget = s.InterviewTarget,
                    HiresTarget = s.HiresTarget,
                    Submissions = s.Submissions,
                    Interviews = s.Interviews,
                    Hires = s.Hires,
                    SubmissionsProgress = s.SubmissionProgress,
                    InterviewsProgress = s.InterviewProgress,
                    HiresProgress = s.HiresProgress,
                    SubmissionsProgressColor = s.SubmissionProgress >= 70 ? "progress-bar-success" :
                                                                     s.SubmissionProgress < 70 && s.SubmissionProgress >= 50 ? "progress-bar-warning"
                                                                     : "progress-bar-danger",
                    InterviewsProgressColor = s.InterviewProgress >= 70 ? "progress-bar-success" :
                                                                     s.InterviewProgress < 70 && s.InterviewProgress >= 50 ? "progress-bar-warning"
                                                                      : "progress-bar-danger",
                    HiresProgressColor = s.HiresProgress >= 70 ? "progress-bar-success" :
                                                                     s.HiresProgress < 70 && s.HiresProgress >= 50 ? "progress-bar-warning"
                                                                      : "progress-bar-danger",
                    AvgProgressClass = (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 >= 70 ? "notice-success" :
                                                                     (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 <= 70 && (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 >= 50 ? "notice-warning"
                                                                      : "notice-danger"
                }).ToList()
            };
            //int Year = usDate.Year;
            //DateTime now = DateTime.Now;
            //var startDate = new DateTime(now.Year, now.Month, 1);
            //string Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month);
            if (role == "Employee")
            {
				progressReport.Role = role;
				var targets = unitOfwork.User.getemployeemonthlytarget(empCd, Month, Year);
				var monthlysubs = unitOfwork.User.GetMonthlyData(empCd, startDate.ToString());
				progressReport.SubmissionTarget = targets.Sum(s => s.SubmissionTarget);
				progressReport.InterviewTarget = targets.Sum(s => s.InterviewTarget);
				progressReport.HiresTarget = targets.Sum(s => s.HireTarget);
				progressReport.Submissions = monthlysubs.Sum(s => s.Submissions);
				progressReport.Interviews = monthlysubs.Sum(s => s.Interviews);
				progressReport.Hires = monthlysubs.Sum(s => s.Hires);
				progressReport.SubmissionsPer = targets.Sum(s => s.SubmissionTarget) != 0 ? (int)((float)(monthlysubs.Sum(s => s.Submissions) / (float)targets.Sum(s => s.SubmissionTarget)) * 100) : 100;
				progressReport.InterviewsPer = targets.Sum(s => s.InterviewTarget) != 0 ? (int)(((float)monthlysubs.Sum(s => s.Interviews) / (float)targets.Sum(s => s.InterviewTarget)) * 100) : 100;
				progressReport.HiresPer = targets.Sum(s => s.HireTarget) != 0 ? (int)(((float)monthlysubs.Sum(s => s.Hires) / (float)targets.Sum(s => s.HireTarget)) * 100) : 100;
				progressReport.EmployeeList = jr.Select(s => new ProgressReportEmployeeList
				{
					EmpCd = s.EmpCd,
					EmployeeName = s.EmployeeName,
					FromDate = s.FromDate,
					ToDate = s.ToDate,
					SubmissionsTarget = targets.Where(x => x.Emp_Cd == s.EmpCd).Select(x => x.SubmissionTarget).SingleOrDefault(),
					InterviewTarget = targets.Where(x => x.Emp_Cd == s.EmpCd).Select(x => x.InterviewTarget).SingleOrDefault(),
					HiresTarget = targets.Where(x => x.Emp_Cd == s.EmpCd).Select(x => x.HireTarget).SingleOrDefault(),
					Submissions = s.Submissions,
					Interviews = s.Interviews,
					Hires = s.Hires,
					SubmissionsProgress = s.SubmissionProgress,
					InterviewsProgress = s.InterviewProgress,
					HiresProgress = s.HiresProgress,
					SubmissionsProgressColor = s.SubmissionProgress >= 70 ? "progress-bar-success" :
													 s.SubmissionProgress < 70 && s.SubmissionProgress >= 50 ? "progress-bar-warning"
													 : "progress-bar-danger",
					InterviewsProgressColor = s.InterviewProgress >= 70 ? "progress-bar-success" :
													 s.InterviewProgress < 70 && s.InterviewProgress >= 50 ? "progress-bar-warning"
													  : "progress-bar-danger",
					HiresProgressColor = s.HiresProgress >= 70 ? "progress-bar-success" :
													 s.HiresProgress < 70 && s.HiresProgress >= 50 ? "progress-bar-warning"
													  : "progress-bar-danger",
					AvgProgressClass = (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 >= 70 ? "notice-success" :
													 (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 <= 70 && (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 >= 50 ? "notice-warning"
													  : "notice-danger"
				}).ToList();

			}
			else if (role == "Team Lead")
            {
				progressReport.Role = role;

				//DateTime now1 = DateTime.Now;
				var monthlysubs = unitOfwork.User.GetMonthlyData(empCd, startDate.ToString());
                var lstEmployees = unitOfwork.User.GetAllMgrEmployees(empCd, Month, Year);
				ViewBag.ProgressbarlstEmployees = lstEmployees;
                progressReport.SubmissionTarget = lstEmployees.Sum(s => s.SubmissionTarget);
                progressReport.InterviewTarget = lstEmployees.Sum(s => s.InterviewTarget);
                progressReport.HiresTarget = lstEmployees.Sum(s => s.HireTarget);
                progressReport.Submissions = monthlysubs.Sum(s => s.Submissions);
                progressReport.Interviews = monthlysubs.Sum(s => s.Interviews);
                progressReport.Hires = monthlysubs.Sum(s => s.Hires);
                progressReport.SubmissionsPer = lstEmployees.Sum(s => s.SubmissionTarget) != 0 ? (int)((float)(monthlysubs.Sum(s => s.Submissions) / (float)lstEmployees.Sum(s => s.SubmissionTarget)) * 100) : 100;
                progressReport.InterviewsPer = lstEmployees.Sum(s => s.InterviewTarget) != 0 ? (int)(((float)monthlysubs.Sum(s => s.Interviews) / (float)lstEmployees.Sum(s => s.InterviewTarget)) * 100) : 100;
                progressReport.HiresPer = lstEmployees.Sum(s => s.HireTarget) != 0 ? (int)(((float)monthlysubs.Sum(s => s.Hires) / (float)lstEmployees.Sum(s => s.HireTarget)) * 100) : 100;
				progressReport.EmployeeList = jr.Select(s => new ProgressReportEmployeeList
				{
					EmpCd = s.EmpCd,
					EmployeeName = s.EmployeeName,
					FromDate = s.FromDate,
					ToDate = s.ToDate,
					SubmissionsTarget = lstEmployees.Where(x=>x.Emp_Cd == s.EmpCd).Select(x=>x.SubmissionTarget).SingleOrDefault(),
					InterviewTarget = lstEmployees.Where(x => x.Emp_Cd == s.EmpCd).Select(x => x.InterviewTarget).SingleOrDefault(),
					HiresTarget = lstEmployees.Where(x => x.Emp_Cd == s.EmpCd).Select(x => x.HireTarget).SingleOrDefault(),
					Submissions = s.Submissions,
					Interviews = s.Interviews,
					Hires = s.Hires,
					SubmissionsProgress = s.SubmissionProgress,
					InterviewsProgress = s.InterviewProgress,
					HiresProgress = s.HiresProgress,
					SubmissionsProgressColor = s.SubmissionProgress >= 70 ? "progress-bar-success" :
																	 s.SubmissionProgress < 70 && s.SubmissionProgress >= 50 ? "progress-bar-warning"
																	 : "progress-bar-danger",
					InterviewsProgressColor = s.InterviewProgress >= 70 ? "progress-bar-success" :
																	 s.InterviewProgress < 70 && s.InterviewProgress >= 50 ? "progress-bar-warning"
																	  : "progress-bar-danger",
					HiresProgressColor = s.HiresProgress >= 70 ? "progress-bar-success" :
																	 s.HiresProgress < 70 && s.HiresProgress >= 50 ? "progress-bar-warning"
																	  : "progress-bar-danger",
					AvgProgressClass = (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 >= 70 ? "notice-success" :
																	 (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 <= 70 && (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 >= 50 ? "notice-warning"
																	  : "notice-danger"
				}).ToList();
				//progressReport.EmployeeList.
			}

			else if (role == "Manager")
			{
				progressReport.Role = role;

				//DateTime now1 = DateTime.Now;
				var monthlysubs = unitOfwork.User.GetMonthlyData(empCd, startDate.ToString());
				var lstEmployees = unitOfwork.User.GetAllMgrEmployees(empCd, Month, Year);
				var lstTotalEmployees = new List<RIC_MonthlyTarget>();
				foreach (var item in lstEmployees)
				{
					//lstTotalEmployees.Remove(lstTotalEmployees.Find(e => e.Emp_Cd == item.Emp_Cd));
					lstTotalEmployees.Add(item);
					var lstReportie = unitOfwork.User.GetAllMgrEmployees(item.Emp_Cd, Month, Year);
					if (lstReportie.Any())
					{
						foreach (RIC_MonthlyTarget Obj in lstReportie)
						{

							if (lstTotalEmployees.Any(e => e.Emp_Cd == Obj.Emp_Cd))
							{
								//if emp is reporting to subordinate currently remove from higher ups, and use subordinate reporting.
								if (unitOfwork.context.RIC_ReportingHistory.Any(e => e.RR_EmpCD == Obj.Emp_Cd && e.RR_MgrCD == item.Emp_Cd && e.RR_ToDate == null))
								{
									lstTotalEmployees.Remove(lstTotalEmployees.Find(e => e.Emp_Cd == Obj.Emp_Cd));

									lstTotalEmployees.Add(Obj);
								}
								else
								{
									//lstTotalEmployees.Add(Obj);
								}
							}
							//If emp is not found in my supervisor
							else
							{
								lstTotalEmployees.Add(Obj);
							}
						}
					}
				}
				//ViewBag.ProgressbarlstEmployees = lstEmployees;
				//lstTotalEmployees = lstTotalEmployees.OrderByDescending(s => s.MT_Id).ToList();
				progressReport.SubmissionTarget = lstTotalEmployees.Sum(s => s.SubmissionTarget);
				progressReport.InterviewTarget = lstTotalEmployees.Sum(s => s.InterviewTarget);
				progressReport.HiresTarget = lstTotalEmployees.Sum(s => s.HireTarget);
				progressReport.Submissions = monthlysubs.Sum(s => s.Submissions);
				progressReport.Interviews = monthlysubs.Sum(s => s.Interviews);
				progressReport.Hires = monthlysubs.Sum(s => s.Hires);
				progressReport.SubmissionsPer = lstTotalEmployees.Sum(s => s.SubmissionTarget) != 0 ? (int)((float)(monthlysubs.Sum(s => s.Submissions) / (float)lstTotalEmployees.Sum(s => s.SubmissionTarget)) * 100) : 100;
				progressReport.InterviewsPer = lstTotalEmployees.Sum(s => s.InterviewTarget) != 0 ? (int)(((float)monthlysubs.Sum(s => s.Interviews) / (float)lstTotalEmployees.Sum(s => s.InterviewTarget)) * 100) : 100;
				progressReport.HiresPer = lstTotalEmployees.Sum(s => s.HireTarget) != 0 ? (int)(((float)monthlysubs.Sum(s => s.Hires) / (float)lstTotalEmployees.Sum(s => s.HireTarget)) * 100) : 100;
				progressReport.EmployeeList = jr.Select(s => new ProgressReportEmployeeList
				{
					EmpCd = s.EmpCd,
					EmployeeName = s.EmployeeName,
					FromDate = s.FromDate,
					ToDate = s.ToDate,
					SubmissionsTarget = lstTotalEmployees.Where(x => x.Emp_Cd == s.EmpCd && x.SubmissionTarget>0).Select(x => x.SubmissionTarget).SingleOrDefault(),
					InterviewTarget = lstTotalEmployees.Where(x => x.Emp_Cd == s.EmpCd && x.InterviewTarget>0).Select(x => x.InterviewTarget).SingleOrDefault(),
					HiresTarget = lstTotalEmployees.Where(x => x.Emp_Cd == s.EmpCd && x.HireTarget>0).Select(x => x.HireTarget).SingleOrDefault(),
					Submissions = s.Submissions,
					Interviews = s.Interviews,
					Hires = s.Hires,
					SubmissionsProgress = s.SubmissionProgress,
					InterviewsProgress = s.InterviewProgress,
					HiresProgress = s.HiresProgress,
					SubmissionsProgressColor = s.SubmissionProgress >= 70 ? "progress-bar-success" :
																	 s.SubmissionProgress < 70 && s.SubmissionProgress >= 50 ? "progress-bar-warning"
																	 : "progress-bar-danger",
					InterviewsProgressColor = s.InterviewProgress >= 70 ? "progress-bar-success" :
																	 s.InterviewProgress < 70 && s.InterviewProgress >= 50 ? "progress-bar-warning"
																	  : "progress-bar-danger",
					HiresProgressColor = s.HiresProgress >= 70 ? "progress-bar-success" :
																	 s.HiresProgress < 70 && s.HiresProgress >= 50 ? "progress-bar-warning"
																	  : "progress-bar-danger",
					AvgProgressClass = (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 >= 70 ? "notice-success" :
																	 (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 <= 70 && (s.SubmissionProgress + s.InterviewProgress + s.HiresProgress) / 3 >= 50 ? "notice-warning"
																	  : "notice-danger"
				}).ToList();
				//progressReport.EmployeeList.
			}

			return PartialView(progressReport);
        }

        public ActionResult ProgressDashboard()
        {

            return View();
        }

        public ActionResult UserDashboard()
        {
            string empCd = User.Identity.Name;
            var dashboardPanels = unitOfwork.User
                            .getDashboardPanelsForUser(s => s.RU_EmpCd == empCd,null, "RIC_Panel")
                            .OrderBy(o=>o.RU_SortOrder);

            UserDashboard userDashboard = new UserDashboard()
            {
                List1 = dashboardPanels.Where(w=>w.RU_ColumnNumber==1).Select(l1=>new  AddPanelsPopup
                                {

                                    Id = l1.RIC_Panel.RP_Id,
                                    ImgUrl = l1.RIC_Panel.RP_ImageName,
                                    PanelName = l1.RIC_Panel.RP_PanelName,
                                    ActionName = l1.RIC_Panel.RP_ActionName,
                                    ControllerName = l1.RIC_Panel.RP_ConTrollerName,
                                    ColumnWidth = l1.RU_ColumnWidth
                                    //PanelSelected = false
                                }).ToList(),
                List2 = dashboardPanels.Where(w => w.RU_ColumnNumber == 2).Select(l2 => new AddPanelsPopup
                                {

                                    Id = l2.RIC_Panel.RP_Id,
                                    ImgUrl = l2.RIC_Panel.RP_ImageName,
                                    PanelName = l2.RIC_Panel.RP_PanelName,
                                    ActionName = l2.RIC_Panel.RP_ActionName,
                                    ControllerName = l2.RIC_Panel.RP_ConTrollerName,
                                    ColumnWidth = l2.RU_ColumnWidth
                                    //PanelSelected = false
                                }).ToList(),
                List3 = dashboardPanels.Where(w => w.RU_ColumnNumber == 3).Select(l3 => new AddPanelsPopup
                                {

                                    Id = l3.RIC_Panel.RP_Id,
                                    ImgUrl = l3.RIC_Panel.RP_ImageName,
                                    PanelName = l3.RIC_Panel.RP_PanelName,
                                    ActionName = l3.RIC_Panel.RP_ActionName,
                                    ControllerName = l3.RIC_Panel.RP_ConTrollerName,
                                    ColumnWidth = l3.RU_ColumnWidth
                                    //PanelSelected = false
                                }).ToList(),
                             };           
            return View(userDashboard);
        }
        [HttpGet]
        [ActionName("AddPanelsPopup")]
        public ActionResult AddPanelsPopup()
        {
            //get the emp id.
            string empCd = User.Identity.Name;
            var panelslistForuser = unitOfwork.User.getDashboardPanelsForUser(s => s.RU_EmpCd == empCd);
            int maxSortOrder =  panelslistForuser.Count()==0?0: panelslistForuser.Max(m => m.RU_SortOrder)+1;
            int rowWidth = panelslistForuser.Where(w => w.RU_ColumnNumber == 1).Count() == 0 ? 0 : panelslistForuser.Where(w => w.RU_ColumnNumber == 1).Max(m => m.RU_ColumnWidth);
            var AddPanelsList= from plist in unitOfwork.User.getPanelsList()
                               join pUser in panelslistForuser
                               on plist.RP_Id equals pUser.RU_PanelId into pu 
                               from pUser in pu.DefaultIfEmpty()
                               select new  AddPanelsPopup
                                    {
                                        Id = plist.RP_Id,
                                        ImgUrl = plist.RP_ImageName,
                                        PanelName = plist.RP_PanelName,
                                        ActionName = plist.RP_ActionName,
                                        ControllerName = plist.RP_ConTrollerName,
                                        SortOrder = pUser == null ? maxSortOrder : pUser.RU_SortOrder,
                                        PanelSelected = pUser==null?false:true,
                                        ColumnNumber = pUser == null ? 1 : pUser.RU_ColumnNumber,
                                        ColumnWidth = pUser == null ? rowWidth : pUser.RU_ColumnWidth
                                    };
           return View(AddPanelsList);
        }
        [HttpPost]
        [ActionName("AddPanelsPopup")]
        public ActionResult AddPanelsPopupPost(List<AddPanelsPopup> popupList)
        {
           //get the emp id.
            string empCd = User.Identity.Name;         
            //bind the popup list.
            var panelsForUser = popupList.Where(w=>w.PanelSelected==true).Select(s => new RIC_UserDashboardPanel
            {
                RU_EmpCd = empCd,
                RU_PanelId = s.Id,
                RU_SortOrder=s.SortOrder,
                RU_ColumnNumber = s.ColumnNumber,
                RU_ColumnWidth = s.ColumnWidth
            }).ToList();
            unitOfwork.User.UpdateDashboardPanel(panelsForUser,empCd);
            unitOfwork.Save();
            return RedirectToAction("UserDashboard");
        }

        #region UserDashboardPanels
       
        [HttpGet]
        public ActionResult SubmissionsInsightPartial()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult InterviewsInsightPartial()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult StartsInsightPartial()
        {
            return PartialView();
        }

        public ActionResult TeamPerformancePartial()
        {
            return PartialView();
        }

        public ActionResult OrganizationPerformanceIndicatorsPartial()
        {
            return PartialView();
        }
        #endregion
        [NonAction]
		public int getRemainingDays(DateTime startOfMonth, DateTime endOfMonth)
		{
			int count = 0;
			//get the holiday list from db.
			var holidayList = unitOfwork.Holidays.Get().ToList();

			for (int i = 1; i <= endOfMonth.Day; i++)
			{
				DateTime currentDate = new DateTime(startOfMonth.Year, startOfMonth.Month, i);
				var holiday = holidayList.FirstOrDefault(s => s.RH_Date == currentDate);
				if (!(currentDate.DayOfWeek == DayOfWeek.Sunday || currentDate.DayOfWeek == DayOfWeek.Saturday || holiday != null))
					if (currentDate >= SystemClock.US_Date.Date)
						count++;
			}

			return count;
		}

		// [HttpGet]
		public ActionResult TreeViewPartial()
		{
			string empCd = User.Identity.Name;
				// get the role for user.
				RolePrincipal r = (RolePrincipal)User;
				var role = r.GetRoles()[0];
			var _user = unitOfwork.User.GetByEmpID(empCd);
			int EmpID = _user.RE_EmpId;
			ViewBag.role = role;
			List<TreeViewPartial> treeView = new List<TreeViewPartial>();
                var reportinghistory = unitOfwork.User.getReportingHistory(empCd, usDate, usDate, role,"REC", EmpID);
                treeView = reportinghistory.Select(s => new
                {
                    EmpCD = s.RR_EmpCD,
                    MgrCD = s.RR_MgrCD,
                    EmployeeName = s.Employee_Name,
                    ManagerName = s.Mgr_Name,
                    EmpLvl = s.EmpLevel,
                    UserCount = reportinghistory.Where(c => c.RR_MgrCD == s.RR_EmpCD).Count() > 0 ? 0 : 1
                }).OrderBy(o => o.UserCount).ThenBy(t => t.EmployeeName).Select(s => new TreeViewPartial {
                    EmpCD = s.EmpCD,
                    MgrCD = s.MgrCD,
                    EmployeeName = s.EmployeeName,
                    ManagerName = s.ManagerName,
                    EmpLvl = s.EmpLvl                
                }).ToList();
			return View(treeView);
		}

		[Authorize]
		public ActionResult ViewDetailsPopup(DateTime fromDate, DateTime toDate, string data_Jr, string empCd, string company = null,string getIndividualRecord=null)
		  {
				if (toDate > SystemClock.US_Date)
					// toDate = SystemClock.US_Date.Date.AddDays(-1);
					 toDate = SystemClock.US_Date.Date;

				// check for director code.
				string empID = User.Identity.Name;
                var _duser = unitOfwork.User.GetByEmpID(empID);
				string drole = _duser.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
          
				if (empCd == null || empCd == "")
				{
					 empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
					 ViewBag.showSubmittedBy = "Yes";
				}
				else if (drole == directorRoleName||drole==stafingDirectorRole)
					 ViewBag.showSubmittedBy = "Yes";
				else
					 ViewBag.showSubmittedBy = "No";

				string headerText = null;
                string empTblHeaderText = null;

				var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
                string role = null;
                if(empCd!="Directors")
                role= _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

				List<RIC_Job_Report> details = new List<RIC_Job_Report>();
				List<Consolidated> Consolidated = new List<Consolidated>();
				List<DetailsByUser>  DetailsByUser=new List<Models.Dashboard.DetailsByUser>();
			List<ClientRequiredDetails> reqs = new List<ClientRequiredDetails>();
			toDate = toDate.AddDays(1);

			//get the submissions for all users..
			var submissions = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role)
								 .Where(s => s.RJ_Submit_Date >= fromDate.Date && s.RJ_Submit_Date <= toDate)
								   .GroupBy(s => new { s.RJ_EmpCd, s.RJ_Submitted_By }).Select(s => new
								   {
									   EmpCd = s.Key.RJ_EmpCd,
									   EmployeeName = s.Key.RJ_Submitted_By,
									   Submissions = s.Count()
								   });

			//get the Interviews for all users..
			var Interviews = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role)
								  .Where(s => s.RJ_Interview_Date >= fromDate && s.RJ_Interview_Date <= toDate)
								  .GroupBy(s => new { s.RJ_EmpCd, s.RJ_Submitted_By }).Select(s => new
								  {
									  EmpCd = s.Key.RJ_EmpCd,
									  EmployeeName = s.Key.RJ_Submitted_By,
									  Interviews = s.Count()
								  });


			//get the Hires for all users..
			var Hires = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role)
								  .Where(s => s.RJ_Hire_Date >= fromDate && s.RJ_Hire_Date <= toDate)
								  .GroupBy(s => new { s.RJ_EmpCd, s.RJ_Submitted_By }).Select(s => new
								  {
									  EmpCd = s.Key.RJ_EmpCd,
									  EmployeeName = s.Key.RJ_Submitted_By,
									  Hires = s.Count()
								  });

			//if submission clicked.
			if (data_Jr == "Submission")
				{
                    empTblHeaderText = "Submissions";
					 headerText = "Submissions";// if submission clicked.
					 details = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, fromDate, toDate, role,headerText)
								  .Where(s => s.RJ_Submit_Date >= fromDate.Date && s.RJ_Submit_Date <= toDate).ToList();
				}
				else if (data_Jr == "interview")//if interview clicked.
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
				reqs = unitOfwork.RIC_Job_Report.Get_Job_required_details(company, fromDate, toDate,"CD" ,role,empCd)
									  .ToList();
			}

			// if company is not null then filter data by company
			if (company != null)
				{
					 details = details.Where(s => s.RJ_Company.ToLower() == company.ToLower()).ToList();
				}
			if(reqs!=null && company != null)
            {
				reqs = reqs.Where(s => s.RJ_Company.ToLower() == company.ToLower()).ToList();
            }
                //filter the records if flag is yes.
                if (getIndividualRecord == "Yes")
                {
                    details = details.Where(s => s.RJ_EmpCd == empCd).ToList();
                }

			  details = details.OrderBy(s => s.RJ_Submitted_By).ThenBy(s => s.RJ_DateIssued).ToList();
			var emplist = details.Select(s =>  s.RJ_EmpCd );
			  Consolidated = details.GroupBy(g => g.RJ_JobDiva_Ref).Select(s => new Consolidated {
                    IssuedDate = s.FirstOrDefault().RJ_DateIssued,
					JobDiva_Ref = s.Key,
                    Title=s.FirstOrDefault().RJ_Title,
					Company = s.FirstOrDefault().RJ_Company,
					Client_Ref=s.FirstOrDefault().RJ_Optional_Ref,
					Count = s.Count()				
				}).ToList();

			//get related all the data about employee.

			DetailsByUser = (from emp  in unitOfwork.RIC_Employee.Get()
										  join sub in submissions
										  on emp.RE_Emp_Cd equals sub.EmpCd into sj
										  from sSub in sj.DefaultIfEmpty()
										  join Interview in Interviews
										  on emp.RE_Emp_Cd equals Interview.EmpCd into IntList
										  from sInt in IntList.DefaultIfEmpty()
										  join Hire in Hires
										  on emp.RE_Emp_Cd equals Hire.EmpCd into HireList
										  from sHire in HireList.DefaultIfEmpty()
										  select new DetailsByUser()
										  {
											  HeaderText = data_Jr,
											  EmpCd = emp.RE_Emp_Cd,
											  EmployeeName = emp.RE_Jobdiva_User_Name,
											  Submissons = sSub != null ? sSub.Submissions : 0,
											  Interviews = sInt != null ? sInt.Interviews : 0,
											  Hires = sHire != null ? sHire.Hires : 0
										  }).Where(s=>(s.Submissons >0 || s.Interviews > 0 || s.Hires > 0)).ToList();

			DetailsByUser = DetailsByUser.Where(s => emplist.Contains(s.EmpCd)).ToList();
			// DetailsByUser = (from emp in DetailsByUser
			//					 // where emplist.Contains(emp.EmpCd)
			//					 join lt in emplist
			//					 on emp.EmpCd equals lt.RJ_EmpCd
			//					 select new DetailsByUser()
			//					  {
			//						  HeaderText = data_Jr,
			//						  EmpCd = emp.EmpCd,
			//						  EmployeeName = emp.EmployeeName,
			//						  Submissons = emp.Submissons,
			//						  Interviews = emp.Interviews,
			//						  Hires = emp.Hires
			//					  }).ToList();
	 

			if (fromDate.AddDays(1) != toDate)//add the header text
					 headerText = headerText + " From " + fromDate.ToString("MM-dd-yyyy") + " To " + toDate.AddDays(-1).ToString("MM-dd-yyyy");
				else
					 headerText = headerText + " For " + fromDate.ToString("MM-dd-yyyy");
                 ViewBag.empTblHeaderText= empTblHeaderText;
			var reqsTupl = new Tuple<string, List<ClientRequiredDetails>>(headerText, reqs);
			if (data_Jr == "Reqs")
			{
				return PartialView("DetailsViewPopupReqsPartila", reqsTupl);
            }
            else
            {
				return PartialView("ViewDetailsPopup", new Tuple<string, List<RIC_Job_Report>, List<Consolidated>, List<DetailsByUser>>(headerText, details, Consolidated, DetailsByUser));
			}
			
			//return PartialView("ViewDetailsPopup", new Tuple<string, List<RIC_Job_Report>, List<Consolidated>, List<DetailsByUser>>(headerText, details, Consolidated, DetailsByUser));
		  }

		[Authorize]
		public ActionResult AccessLogViewDetailsPopup(DateTime CurentDate, string empCd,string category)
		{
			string headerText = null;
			string empTblHeaderText = null;
			string selectCategory = category;
			List<AccessLogResult> accessLogDetails = new List<AccessLogResult>();
			
			CultureInfo culture = new CultureInfo("en-US");
			DateTime StartDate = Convert.ToDateTime(CurentDate, culture);
			DateTime enddate = StartDate.AddHours(23).AddMinutes(59).AddSeconds(59);
			string role = null;
			RolePrincipal rolePrincipal = (RolePrincipal)User;
			//var role = rolePrincipal.GetRoles()[0];
			DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
			//DateTime CurentDateclick = new DateTime(date.Year, date.Month, date);
			

			var viwres =unitOfwork.RIC_Job_Report.Get_AccesslogJobRepoartForUser( StartDate, enddate, empCd,role);
			//return PartialView(view);
			//ViewBag.EmployeeName = viwres.Add()


			if (StartDate.AddDays(1) != CurentDate)//add the header text
				headerText = headerText + " From " + StartDate.ToString("MM-dd-yyyy") + " To " + enddate.AddDays(0).ToString("MM-dd-yyyy");
			else
				headerText = headerText + " For " + enddate.ToString("MM-dd-yyyy");
			ViewBag.empTblHeaderText = empTblHeaderText;
			ViewBag.dropDetails = selectCategory;
			return PartialView("AccessLogViewDetailsPopup", new Tuple<string, List<AccessLogResult>>(headerText, viwres));
		}

		[Authorize]
		public ActionResult Popup()
		{
			string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
			DateTime previousYear = new DateTime(SystemClock.US_Date.Year - 1, 1, 1);
			//previousYear.AddYears(SystemClock.US_Date.Year);
			var list = unitOfwork.SubmissionsTemp.Get(s => s.EmpCd == empCd && s.StrtDate >= previousYear);
			return View(list);
		}

		[HttpGet]
		public ActionResult UploadSubmissions()
		{
			return View();
		}
		[HttpPost]
		public ActionResult UploadSubmissions(HttpPostedFileBase file, DateTime start_Date, DateTime end_Date)
		{
			//DateTime start_Date = DateTime.ParseExact(start_Dates, "MM/dd/yyyy", null);
			//DateTime end_Date = DateTime.ParseExact(end_Dates, "MM/dd/yyyy", null);
			if (file != null)
			{
				// string fileName = file.FileName;
				// string fileContentType = file.ContentType;
				// byte[] fileBytes = new byte[file.ContentLength];
				// var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
				//var usersList = new List & lt;  
				//Users & gt;  
				//();  
				using (var package = new ExcelPackage(file.InputStream))
				{
					var currentSheet = package.Workbook.Worksheets;
					var workSheet = currentSheet.First();
					var noOfCol = workSheet.Dimension.End.Column;
					var noOfRow = workSheet.Dimension.End.Row;
					bool hasHeader = true;
					//foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
					//{
					//    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
					//}
					var startRow = hasHeader ? 2 : 1;
					for (int rowNum = startRow; rowNum <= workSheet.Dimension.End.Row; rowNum++)
					{
						var wsRow = workSheet.Cells[rowNum, 1, rowNum, workSheet.Dimension.End.Column].ToList();

						if (wsRow[0].Text.Trim().ToLower() == "total")
							break;
						// save date to database.
						unitOfwork.Submission.Insert(new RIC_Submission()
					  {
						  RS_RecruterName = wsRow[0].Text.Trim(),
						  RS_Email = wsRow[1].Text.Trim(),
						  RS_Division = wsRow[2].Text.Trim(),
						  RS_Submission = int.Parse(wsRow[3].Text.Trim()),
						  RS_Interviews = int.Parse(wsRow[4].Text.Trim()),
						  RS_Hires = int.Parse(wsRow[5].Text.Trim()),
						  RS_NetbillHires_Hour = float.Parse(wsRow[7].Text.Trim()),
						  RS_CostOfHires_Hour = float.Parse(wsRow[8].Text.Trim()),
						  RS_MarginOfHires_Hour = float.Parse(wsRow[9].Text.Trim()),
						  RS_MarginOfHires_Per = float.Parse(wsRow[10].Text.Trim()),
						  RS_FromDt = start_Date,
						  RS_ToDt = end_Date
					  }
					);
						unitOfwork.Save();
					}
				}
			}
			return Content(("<script language='javascript' type='text/javascript'> alert('File uploaded successfully.');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));
			//return View();
		}

		[HttpGet]
		public ActionResult UploadSubmissionsDaily()
		{
			return View();
		}
		[HttpPost]
		public ActionResult UploadSubmissionsDaily(HttpPostedFileBase file)
		{
			//DateTime start_Date = DateTime.ParseExact(start_Dates, "MM/dd/yyyy", null);
			//DateTime end_Date = DateTime.ParseExact(end_Dates, "MM/dd/yyyy", null);
			if (file != null)
			{
				// string fileName = file.FileName;
				// string fileContentType = file.ContentType;
				// byte[] fileBytes = new byte[file.ContentLength];
				// var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
				//var usersList = new List & lt;  
				//Users & gt;  
				//(); 


				using (var package = new ExcelPackage(file.InputStream))
				{
					var currentSheet = package.Workbook.Worksheets;
					// var workSheet = currentSheet.First();
					foreach (var workSheet in currentSheet.ToList())
					{

						var noOfCol = workSheet.Dimension.End.Column;
						var noOfRow = workSheet.Dimension.End.Row;
						bool hasHeader = true;
						//foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
						//{
						//    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
						//}
						var startRow = hasHeader ? 2 : 1;
						for (int rowNum = startRow; rowNum <= workSheet.Dimension.End.Row; rowNum++)
						{
							//get the rows from excel.
							var wsRow = workSheet.Cells[rowNum, 1, rowNum, workSheet.Dimension.End.Column].ToList();

							if (wsRow.Count >= 6)
							{
								// if (wsRow[2].Text.Trim().ToLower() == "total")
								//   break;
								//get the job diva user name from excel.
								string jobdivaUserName = wsRow[1].Text.Trim();
								//get the employee details from database.
								var empDetails = unitOfwork.User.Get(s => s.RE_Jobdiva_User_Name == jobdivaUserName).
									 FirstOrDefault();
								if (empDetails != null)
								{
									// save submissions date to database.
									unitOfwork.SubmissionDaily.Insert(new RIC_SubmissionDaily()
									{
										RS_Date = DateTime.Parse(wsRow[0].Text.Trim()),
										RS_Emp_Cd = empDetails.RE_Emp_Cd,// set the employye cd.
										RS_Submissions = int.Parse(wsRow[4].Text.Trim()),
										RS_Interviews = int.Parse(wsRow[5].Text.Trim()),
										RS_Hires = int.Parse(wsRow[6].Text.Trim())
									}
								);

								}
							}
						}
						try
						{

							unitOfwork.Save();

						}
						catch (Exception dbUpdateEx)
						{
							if (dbUpdateEx.InnerException != null
									  && dbUpdateEx.InnerException.InnerException != null)
							{
								if (dbUpdateEx.InnerException.InnerException is SqlException)
								{
									switch (((SqlException)dbUpdateEx.InnerException.InnerException).Number)
									{

										case 2601:
										return Content(("<script language='javascript' type='text/javascript'> alert('Duplicate Data Uploaded.');window.location = '" + Request.UrlReferrer.ToString() + "';</script>")); // Duplicated key row error
										// A custom exception of yours for concurrency issues
										// throw new ConcurrencyException();
										default:
										// A custom exception of yours for other DB issues
										throw dbUpdateEx;
									}
								}

							}
						}
					}
				}
			}
			return Content(("<script language='javascript' type='text/javascript'> alert('File uploaded successfully.');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));
			//return View();
		}


		//Added by Madhu 22-03-2018
		[HttpGet]
		public ActionResult UploadCallStatistics()
		{
			return View();
		}
		//Added by Madhu 22-03-2018
		[HttpPost]
		public ActionResult UploadCallStatistics(HttpPostedFileBase file)
		{
			string filePath = string.Empty;
			if (file != null)
			{
				string fileName = file.FileName;
				string fileContentType = file.ContentType;
				byte[] fileBytes = new byte[file.ContentLength];

				var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(file.InputStream.ToString());

				using (ExcelPackage package = new ExcelPackage(file.InputStream))
				{
					// var worksheet = package.Workbook.Worksheets.Add(@"C:\Users\Ramanjari\Desktop\ABC.xls");


					var currentSheet = package.Workbook.Worksheets;
					var workSheet = currentSheet.First();
					var noOfCol = workSheet.Dimension;
					var noOfRow = workSheet.Dimension.End.Row;
					bool hasHeader = true;

					var startRow = hasHeader ? 2 : 1;
					for (int rowNum = startRow; rowNum <= workSheet.Dimension.End.Row; rowNum++)
					{
						var wsRow = workSheet.Cells[rowNum, 1, rowNum, workSheet.Dimension.End.Column].ToList();

						// save date to database.
						if (wsRow.Count >= 9)
						{
							string jobdivaUserName = wsRow[1].Text.ToString();
							var empDetails = unitOfwork.User.Get(s => s.RE_Jobdiva_User_Name == jobdivaUserName).FirstOrDefault();
							if (empDetails != null)
							{

								unitOfwork.CallStatistics.Insert(new RIC_Call_Statistics()
								{
									//RE_Emp_Cd=wsRow[0].Text.Trim(),
									RC_Date = DateTime.Parse(wsRow[2].Text.Trim()),
									RC_Time = TimeSpan.Parse(wsRow[3].Text),
									//RC_CallType =callType,
									RC_CallType = wsRow[4].Text.Trim(),
									RC_Dailed = wsRow[5].Text.Trim(),
									RC_Calling = wsRow[6].Text.Trim(),
									RC_Emp_Cd = empDetails.RE_Emp_Cd,
									RC_Duration = TimeSpan.Parse(wsRow[7].Text.Trim()),
									RC_Call_Connected = int.Parse(wsRow[8].Text.Trim()),
									RC_Voice_Message = int.Parse(wsRow[9].Text.Trim()),
									RC_PRI = wsRow[10].Text.Trim()
								}
							);
							}
						}
						unitOfwork.Save();
					}
				}
			}
			return Content(("<script language='javascript' type='text/javascript'> alert('File uploaded successfully.');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));
			//return View();    
		}

		#region


		public ActionResult UploadUsers()
		{


			return View();
		}
		[HttpPost]
		public ActionResult UploadUsers(HttpPostedFileBase file)
		{

			//DateTime start_Date = DateTime.ParseExact(start_Dates, "MM/dd/yyyy", null);
			//DateTime end_Date = DateTime.ParseExact(end_Dates, "MM/dd/yyyy", null);
			if (file != null)
			{
				string fileName = file.FileName;
				string fileContentType = file.ContentType;
				byte[] fileBytes = new byte[file.ContentLength];
				// var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
				//var usersList = new List & lt;  
				//Users & gt;  
				//();  
				using (var package = new ExcelPackage(file.InputStream))
				{
					var currentSheet = package.Workbook.Worksheets;
					var workSheet = currentSheet.First();
					var noOfCol = workSheet.Dimension.End.Column;
					var noOfRow = workSheet.Dimension.End.Row;
					bool hasHeader = true;
					//foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
					//{
					//    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
					//}
					var startRow = hasHeader ? 2 : 1;
					for (int rowNum = startRow; rowNum <= workSheet.Dimension.End.Row; rowNum++)
					{
						var wsRow = workSheet.Cells[rowNum, 1, rowNum, workSheet.Dimension.End.Column].ToList();

						string[] str = wsRow[0].Text.Trim().Split(' ');

						if (wsRow[0].Text.Trim().ToLower() == "total")
							break;
						// save date to database.
						unitOfwork.User.Insert(new RIC_Employee()
						{
							RE_Emp_Cd = wsRow[1].Text.Trim(),
							RE_Employee_Name = str[0],
							//  RE_Last_Name = str[1],
							RE_AKA_Name = str[0],
							RE_Sub_Rule_ID = 5,
							RE_Start_Date = SystemClock.US_Date,
							RE_End_Date = SystemClock.US_Date,
							RE_Email = wsRow[5].Text.Trim(),
							RE_Password = "123456789",
							ConfirmPassword = "123456789",
							RE_Joining_Date = DateTime.Parse(wsRow[7].Text.Trim()),
							RoleID = int.Parse(wsRow[6].Text.Trim()),
							RE_Jobdiva_User_Name = wsRow[0].Text.Trim()

						}
					);
						unitOfwork.Save();
					}
				}
			}
			return View();
		}

		#endregion

		public ActionResult ViewSubmission(int? page, string Emp_Code, DateTime? Start_Data, DateTime? End_Data)
		{
			int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			IEnumerable<RIC_SubmissionDaily> subList = (unitOfwork.SubmissionDaily.GetSubmission()).ToPagedList(pageIndex, 10);
			//var subList = unitOfwork.SubmissionDaily.Get();
			//var user =unitOfwork.User.Get();

			// set the values in hidden fields.
			ViewBag.Emp_Code = Emp_Code;
			ViewBag.Start_Data = Start_Data;
			ViewBag.End_Data = End_Data;

			return View(subList);
		}

		[HttpGet]
		public ActionResult AddSubmissionRule()
		{
			return View();
		}
		[HttpPost]
		public ActionResult AddSubmissionRule(RIC_SubmissionRule subRule)
		{
            if (ModelState.IsValid)
            {
                var Experience = unitOfwork.SubmissionRule.Get(s => s.RS_Experience == subRule.RS_Experience).Count();
                if (Experience == 0)
                {
                    unitOfwork.SubmissionRule.Insert(subRule);
                    unitOfwork.Save();
                    return Content("<script>alert('New Submission Rule is Added Sucessfully.');window.location = '" + Request.UrlReferrer.ToString() + "';</script>");

                }
                else
                {
                    ModelState.AddModelError("", "User with same Experience already exist.");
                }
                
            }
            return View();
		}

		public ActionResult ViewSubmissionRule(int? page)
		{
            int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			IEnumerable<RIC_SubmissionRule> subRule;
            
            subRule= unitOfwork.SubmissionRule.Get().ToPagedList(pageIndex, 10);
            ViewBag.Pagination = pageIndex;
			return View(subRule);
		}

        public ActionResult ViewSubmissionEdit(int Id)
        {
            List<RIC_SubmissionRule> subRule = unitOfwork.SubmissionRule.Get().ToList();
            var subRuleEdit = subRule.Where(s => s.RS_Id == Id).FirstOrDefault();
            ViewBag.Pagination = Request.QueryString["Page"];
            return View(subRuleEdit);
        }

        [HttpPost]
        public ActionResult ViewSubmissionEdit(int Id, RIC_SubmissionRule subRuleUpdate)
        {
            unitOfwork.SubmissionRule.Update(subRuleUpdate);
            unitOfwork.Save();
            return Content("<script>alert('Submission Rule Updated Sucessfully.');window.location = '" + Request.UrlReferrer.ToString() + "';</script>");
        }

		public ActionResult ViewHolidays()
		{
			//get holidays from db.
			List<RIC_Holidays> hList = unitOfwork.Holidays.Get().OrderBy(s=>s.RH_Date).ToList();
			return View(hList);
		}

		[HttpGet]
		public ActionResult UploadHolidayList()
		{

			return View();
		}
		[HttpPost]
		public ActionResult UploadHolidayList(HttpPostedFileBase file)
		{
			if (file != null)
			{
				string fileName = file.FileName;
				string fileContentType = file.ContentType;
				byte[] fileBytes = new byte[file.ContentLength];
				// var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
				//var usersList = new List & lt;  
				//Users & gt;  
				//();  
				using (var package = new ExcelPackage(file.InputStream))
				{
					var currentSheet = package.Workbook.Worksheets;
					var workSheet = currentSheet.First();
					var noOfCol = workSheet.Dimension.End.Column;
					var noOfRow = workSheet.Dimension.End.Row;
					bool hasHeader = true;
					//foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
					//{
					//    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
					//}
					var startRow = hasHeader ? 2 : 1;
					for (int rowNum = startRow; rowNum <= workSheet.Dimension.End.Row; rowNum++)
					{
						var wsRow = workSheet.Cells[rowNum, 1, rowNum, workSheet.Dimension.End.Column].ToList();

						if (wsRow[0].Text.Trim().ToLower() == "total")
							break;
						// save date to database.
						unitOfwork.Holidays.Insert(new RIC_Holidays()
						{
							RH_Date = DateTime.Parse(wsRow[0].Text.Trim()),
							RH_Festival = wsRow[1].Text.Trim(),

						}
					);
						unitOfwork.Save();
					}
				}
			}
			return Content(("<script language='javascript' type='text/javascript'> alert('File uploaded successfully.');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));
		}

        public Tuple<int, int, int> getTragetForUser(string empCd,DateTime date,string role,string getIndividualRecords,int EmpID=0)
        {
            int submissionTarget = 0;
            int interviewTarget = 0;
            int hiresTarget = 0;

            if (getIndividualRecords != "Yes")
            {
                var EmployeeList = (from rh in unitOfwork.User.getReportingHistory(empCd, date,date, role,null,EmpID)
                                    join user in unitOfwork.User.Get()
                                    on rh.RR_EmpCD equals user.RE_Emp_Cd
                                    select new
                                    {
                                        submissionTarget = user.RIC_SubmissionRule.RS_MonthlySubmissions,
                                        InterviewTarget = user.RIC_SubmissionRule.RS_Monthly_Interviews,
                                        HiresTarget = user.RIC_SubmissionRule.RS_Monthyl_Hire
                                    });
                submissionTarget = EmployeeList.Sum(s => s.submissionTarget);
                interviewTarget = EmployeeList.Sum(s => s.InterviewTarget);
                hiresTarget = EmployeeList.Sum(s => s.HiresTarget);
            }
            submissionTarget += unitOfwork.User.GetByEmpID(empCd).RIC_SubmissionRule.RS_MonthlySubmissions;
            interviewTarget += unitOfwork.User.GetByEmpID(empCd).RIC_SubmissionRule.RS_Monthly_Interviews;
            hiresTarget += unitOfwork.User.GetByEmpID(empCd).RIC_SubmissionRule.RS_Monthyl_Hire;
            return Tuple.Create(submissionTarget, interviewTarget,hiresTarget);
        }

		//public ActionResult AccesslogIndex(string empCd = null, string getIndividualRecord = null, string FirstName = null, string FilterName = null, string page = null, string Role = null, string ReportingID = null, string EmpId = null)
		//{
		//	DashBoardIndexAction dashboardIndex = new DashBoardIndexAction();
		//	// get the employee id.
		//	if (empCd == null)
		//		empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
		//	else
		//		ViewBag.ShowUserName = "Y";//show the user name.
		//	DashBoardViewModel dashboardView = new DashBoardViewModel();
		//	// temp need to change 
		//	var _user = unitOfwork.User.GetByEmpID(empCd);//.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
		//	RolePrincipal r = (RolePrincipal)User;
		//	var rolesArray = r.GetRoles();

		//	ViewBag.userName = _user.RE_Jobdiva_User_Name;
		//	ViewBag.getIndividualRecords = getIndividualRecord;
		//	//get roles for user..
		//	string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

		//	var ReportingList = from rh in unitOfwork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role)
		//						select rh;


		//	dashboardView.InterimFeedbackNotification = (from feedback in unitOfwork.RIC_PersonalFeedback.Get()
		//												 join rhe in ReportingList
		//												 on feedback.RP_EmployeeID equals rhe.RR_EmpCD
		//												 join emp in unitOfwork.User.Get() on feedback.RP_EmployeeID equals emp.RE_Emp_Cd into empList
		//												 from employeeList in empList.DefaultIfEmpty()
		//												 join dep in unitOfwork.Department.Get() on employeeList.RE_DepartmentID equals dep.RD_ID into depList
		//												 from departmentList in depList.DefaultIfEmpty()
		//												 where employeeList.RE_Resign_Date == null && (rhe.RR_ToDate == null || rhe.RR_ToDate >= usDate.Date) && feedback.RP_Status.ToUpper() == "COMPLETED"
		//														  && Convert.ToInt32((feedback.RP_NextReviewDate - usDate.Date).TotalDays) >= 0
		//														  && Convert.ToInt32((feedback.RP_NextReviewDate - usDate.Date).TotalDays) <= 10
		//												 select new InterimFeedbackNotification
		//												 {
		//													 EmpId = feedback.RP_EmployeeID,
		//													 //ManagerId= rhe.RR_MgrCD,
		//													 EmployeeName = rhe.Employee_Name,
		//													 RRStartDate = Convert.ToDateTime(rhe.RR_FromDate),
		//													 RREndDate = Convert.ToDateTime(rhe.RR_ToDate),
		//													 ReviewDate = feedback.RP_ReviewDate,
		//													 NextReviewDate = feedback.RP_NextReviewDate,
		//													 Department = departmentList.RD_Department
		//												 }).ToList();

		//	dashboardView.NewJoineeInterimNotification = (from rhe in ReportingList
		//												  join emp in unitOfwork.User.Get() on rhe.RR_EmpCD equals emp.RE_Emp_Cd
		//												  join feedback in unitOfwork.RIC_PersonalFeedback.Get() on rhe.RR_EmpCD equals feedback.RP_EmployeeID into feedbackList
		//												  where emp.RE_Resign_Date == null && (emp.RE_DepartmentID == 1 || emp.RE_DepartmentID == 21)
		//														  && feedbackList.Count() == 0
		//														  && (rhe.RR_ToDate == null || rhe.RR_ToDate >= usDate.Date)
		//														  && Convert.ToInt32((usDate.Date - Convert.ToDateTime(emp.RE_Joining_Date).AddMonths(3).AddDays(-10)).TotalDays) >= 0
		//														  && Convert.ToInt32((usDate.Date - Convert.ToDateTime(emp.RE_Joining_Date).AddMonths(3).AddDays(-10)).TotalDays) <= 10
		//												  select new NewJoineeInterimNotification
		//												  {
		//													  EmpId = rhe.RR_EmpCD,
		//													  EmployeeName = emp.RE_Employee_Name,
		//													  JoiningDate = Convert.ToDateTime(emp.RE_Joining_Date),
		//													  ReviewDate = Convert.ToDateTime(emp.RE_Joining_Date).AddMonths(3)
		//												  }).ToList();




		//	if (Session["View"] == null)
		//	{
		//		Session["View"] = "View";
		//	}
		//	else
		//	{
		//		Session["View"] = "Not Viewed";
		//	}


		//	int FeedbackCount = dashboardView.InterimFeedbackNotification.Count();
		//	int NewJoineeFeedbackCount = dashboardView.NewJoineeInterimNotification.Count();

		//	ViewBag.PopupBoolean = (FeedbackCount > 0 || NewJoineeFeedbackCount > 0) ? true : false;
		//	ViewBag.ViewStatus = Session["View"].ToString() == "View" ? true : false;
		//	ViewBag.EmployeeRoleStatus = (role == mgrRoleName || role == tlRoleName) ? true : false;


		//	var targetForUser = getTragetForUser(empCd, usDate.Date, role, getIndividualRecord);
		//	dashboardView.RS_SubmissionTr = targetForUser.Item1;
		//	dashboardView.RS_InterviewsTr = targetForUser.Item2;
		//	dashboardView.RS_HiresTr = targetForUser.Item3;
		//	// add the last 2 month details.            
		//	for (int i = 1; i <= 2; i++)
		//	{
		//		DateTime lastMonthDate = SystemClock.US_Date.AddMonths(-i);
		//		DateTime lastMonthStartDate = new DateTime(lastMonthDate.Year, lastMonthDate.Month, 1);
		//		DateTime lastMonthEndDate = new DateTime(lastMonthDate.Year, lastMonthDate.Month,
		//											 DateTime.DaysInMonth(lastMonthDate.Year, lastMonthDate.Month));
		//		DateTime? joiningDate = _user.RE_Joining_Date;
		//		if (joiningDate != null && joiningDate >= lastMonthEndDate)
		//		{
		//			// if joining date is greater than the end of month then show null.
		//			dashboardView.lastTwoMonthData.Add(new SubmissionData()
		//			{
		//				date = SystemClock.US_Date.AddMonths(-i),
		//				hires = null,
		//				interviews = null,
		//				submission = null
		//			});
		//		}
		//		else
		//		{
		//			//add the extra day in end of month.
		//			lastMonthEndDate = lastMonthEndDate.AddDays(1);
		//			// if end of month is greter than date then change the end of month to todays date.
		//			if (lastMonthEndDate >= SystemClock.US_Date.Date)
		//				lastMonthEndDate = SystemClock.US_Date.Date;
		//			//get the submission interview and hire count for user.
		//			//if get individual records =="yes" then filter the records by employee id
		//			var jr = unitOfwork.RIC_Job_Report
		//					.Get_TeamPerformanceResult(lastMonthStartDate, lastMonthEndDate, empCd, role)
		//					.Where(w => getIndividualRecord == "Yes" ? w.EmpCd == empCd : true).ToList();

		//			int submissions = jr.Sum(s => s.Submissions);
		//			int interviews = jr.Sum(s => s.Interviews);
		//			int hires = jr.Sum(s => s.Hires);
		//			int submissionTarget = jr.Sum(s => s.SubmissionTarget);
		//			int interviewTarget = jr.Sum(s => s.InterviewTarget);
		//			int hiresTarget = jr.Sum(s => s.HiresTarget);
		//			//var lastMonthJRepoart = unitOfwork.RIC_Job_Report
		//			//     .Get_JobRepoartForUser(empCd, lastMonthStartDate, lastMonthEndDate, role)
		//			//     .Where(w=>getIndividualRecord=="Yes"?w.RJ_EmpCd==empCd:true);
		//			//int submissions = lastMonthJRepoart
		//			//                  .Count(subCount => subCount.RJ_Submit_Date != null 
		//			//                        && subCount.RJ_Submit_Date >= lastMonthStartDate 
		//			//                        && subCount.RJ_Submit_Date <= lastMonthEndDate
		//			//                        );
		//			//int Interviews = lastMonthJRepoart
		//			//                 .Count(intCount => intCount.RJ_Interview_Date != null 
		//			//                    && intCount.RJ_Interview_Date >= lastMonthStartDate 
		//			//                    && intCount.RJ_Interview_Date <= lastMonthEndDate
		//			//                    );
		//			//int Hires = lastMonthJRepoart
		//			//            .Count(hireCount => hireCount.RJ_Hire_Date != null 
		//			//                && hireCount.RJ_Hire_Date >= lastMonthStartDate 
		//			//                && hireCount.RJ_Hire_Date <= lastMonthEndDate
		//			//                );


		//			dashboardView.lastTwoMonthData.Add(new SubmissionData()
		//			{
		//				date = SystemClock.US_Date.AddMonths(-i),
		//				hires = hires,
		//				interviews = interviews,
		//				submission = submissions,
		//				SubmissionTarget = submissionTarget,
		//				InterviewTarget = interviewTarget,
		//				HiresTarget = hiresTarget
		//			});
		//		}
		//	}
		//	ViewBag.ProgressReport = true;//role == tlRoleName && getIndividualRecord == null;

		//	ViewBag.ReportingID = ReportingID;
		//	ViewBag.Page = page;
		//	ViewBag.Empcode = EmpId;
		//	ViewBag.Firstname = FirstName;
		//	ViewBag.FilterName = FilterName;
		//	ViewBag.RoleIdFilter = Role;

		//	//set the default 90 daya for progress report.
		//	dashboardView.ProgressReportFromDate = usDate.Date.AddDays(-90);
		//	dashboardView.ProgressReportToDate = usDate.Date;
		//	return View(dashboardView);
		//}

		public ActionResult AuditData()
        {
			string empCD=FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
			var _user = unitOfwork.User.GetByEmpID(empCD);

			//get roles for user..
			string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
			ViewBag.role = role;

			var ds = unitOfwork.User.getAuditDetails();

			if (ds.Tables[0].Rows.Count > 0)
			{
				ViewBag.JD = ds.Tables[0].Rows[0][0].ToString();

			}
			if (ds.Tables[1].Rows.Count > 0)
			{
				ViewBag.ST = ds.Tables[1].Rows[0][0].ToString();

			}
			if (ds.Tables[2].Rows.Count > 0)
			{
				ViewBag.EC = ds.Tables[2].Rows[0][0].ToString();

			}
			if (ds.Tables[3].Rows.Count > 0)
			{
				ViewBag.AL = ds.Tables[3].Rows[0][0].ToString();

			}

			return View();
        }
	}

}
