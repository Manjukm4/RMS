using DBLibrary;
using DBLibrary.UnitOfWork;
using RIC.Models;
using RIC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RIC.Controllers
{
    public class AccesslogController : Controller
    {
		//
		// GET: /Accesslog/

		UnitOfWork unitOfwork = new UnitOfWork();
		string mgrRoleName = System.Configuration.ConfigurationManager.AppSettings["ManagerRole"];
		string tlRoleName = System.Configuration.ConfigurationManager.AppSettings["TLRole"];
		string empRoleName = System.Configuration.ConfigurationManager.AppSettings["EmployeeRole"];
		string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
		int directorRoleID = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DirectorID"]);
		int adminRoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AdminRoleID"]);
		string AdminRoleName = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
		string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
		string HrRoleName = System.Configuration.ConfigurationManager.AppSettings["HRRole"];
		string stafingDirectorRole = System.Configuration.ConfigurationManager.AppSettings["StaffingDirector"];
		DateTime usDate = SystemClock.US_Date;
		[Authorize]
		public ActionResult Index(string empCd = null, string getIndividualRecord = null, string FirstName = null, string FilterName = null, string page = null, string Role = null, string ReportingID = null, string EmpId = null)
		{
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

			ViewBag.userName = _user.RE_Jobdiva_User_Name;
			ViewBag.getIndividualRecords = getIndividualRecord;
			//get roles for user..
			string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

			var ReportingList = from rh in unitOfwork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role)
								select rh;


			dashboardView.InterimFeedbackNotification = (from feedback in unitOfwork.RIC_PersonalFeedback.Get()
														 join rhe in ReportingList
														 on feedback.RP_EmployeeID equals rhe.RR_EmpCD
														 join emp in unitOfwork.User.Get() on feedback.RP_EmployeeID equals emp.RE_Emp_Cd into empList
														 from employeeList in empList.DefaultIfEmpty()
														 join dep in unitOfwork.Department.Get() on employeeList.RE_DepartmentID equals dep.RD_ID into depList
														 from departmentList in depList.DefaultIfEmpty()
														 where employeeList.RE_Resign_Date == null && (rhe.RR_ToDate == null || rhe.RR_ToDate >= usDate.Date) && feedback.RP_Status.ToUpper() == "COMPLETED"
																  && Convert.ToInt32((feedback.RP_NextReviewDate - usDate.Date).TotalDays) >= 0
																  && Convert.ToInt32((feedback.RP_NextReviewDate - usDate.Date).TotalDays) <= 10
														 select new InterimFeedbackNotification
														 {
															 EmpId = feedback.RP_EmployeeID,
															 //ManagerId= rhe.RR_MgrCD,
															 EmployeeName = rhe.Employee_Name,
															 RRStartDate = Convert.ToDateTime(rhe.RR_FromDate),
															 RREndDate = Convert.ToDateTime(rhe.RR_ToDate),
															 ReviewDate = feedback.RP_ReviewDate,
															 NextReviewDate = feedback.RP_NextReviewDate,
															 Department = departmentList.RD_Department
														 }).ToList();

			dashboardView.NewJoineeInterimNotification = (from rhe in ReportingList
														  join emp in unitOfwork.User.Get() on rhe.RR_EmpCD equals emp.RE_Emp_Cd
														  join feedback in unitOfwork.RIC_PersonalFeedback.Get() on rhe.RR_EmpCD equals feedback.RP_EmployeeID into feedbackList
														  where emp.RE_Resign_Date == null && (emp.RE_DepartmentID == 1 || emp.RE_DepartmentID == 21)
																  && feedbackList.Count() == 0
																  && (rhe.RR_ToDate == null || rhe.RR_ToDate >= usDate.Date)
																  && Convert.ToInt32((usDate.Date - Convert.ToDateTime(emp.RE_Joining_Date).AddMonths(3).AddDays(-10)).TotalDays) >= 0
																  && Convert.ToInt32((usDate.Date - Convert.ToDateTime(emp.RE_Joining_Date).AddMonths(3).AddDays(-10)).TotalDays) <= 10
														  select new NewJoineeInterimNotification
														  {
															  EmpId = rhe.RR_EmpCD,
															  EmployeeName = emp.RE_Employee_Name,
															  JoiningDate = Convert.ToDateTime(emp.RE_Joining_Date),
															  ReviewDate = Convert.ToDateTime(emp.RE_Joining_Date).AddMonths(3)
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
			ViewBag.ViewStatus = Session["View"].ToString() == "View" ? true : false;
			ViewBag.EmployeeRoleStatus = (role == mgrRoleName || role == tlRoleName) ? true : false;


			var targetForUser = getTragetForUser(empCd, usDate.Date, role, getIndividualRecord);
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
							.Get_TeamPerformanceResult(lastMonthStartDate, lastMonthEndDate, empCd, role)
							.Where(w => getIndividualRecord == "Yes" ? w.EmpCd == empCd : true).ToList();

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
						SubmissionTarget = submissionTarget,
						InterviewTarget = interviewTarget,
						HiresTarget = hiresTarget
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
			dashboardView.ProgressReportFromDate = usDate.Date.AddDays(-90);
			dashboardView.ProgressReportToDate = usDate.Date;
			return View(dashboardView);
		}
		public Tuple<int, int, int> getTragetForUser(string empCd, DateTime date, string role, string getIndividualRecords)
		{
			int submissionTarget = 0;
			int interviewTarget = 0;
			int hiresTarget = 0;

			if (getIndividualRecords != "Yes")
			{
				var EmployeeList = (from rh in unitOfwork.User.getReportingHistory(empCd, date, date, role)
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
			return Tuple.Create(submissionTarget, interviewTarget, hiresTarget);
		}


	}
}
