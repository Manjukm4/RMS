using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBLibrary;
using DBLibrary.UnitOfWork;
using System.Security.Cryptography;
using RIC.Utility;
using PagedList;
using System.Web.UI.WebControls;
using RIC.Models;
using System.Data;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Newtonsoft.Json;
using RIC.Models.Account;
using System.Globalization;
using RIC.Models.Client;
using DBLibrary.Migrations;

namespace RIC.Controllers
{
    public class AccountController : Controller
    {
        int adminRoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AdminRoleID"]);
        string AdminRoleName = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
        string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        string HrRoleName = System.Configuration.ConfigurationManager.AppSettings["HRRole"];
        string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
        string EmployeeRoleName = System.Configuration.ConfigurationManager.AppSettings["EmployeeRole"];
        string DevLeadRole = System.Configuration.ConfigurationManager.AppSettings["DEV_LeadRole"];
        string mgrRoleName = System.Configuration.ConfigurationManager.AppSettings["ManagerRole"];
        string tlRoleName = System.Configuration.ConfigurationManager.AppSettings["TLRole"];
        string stafingDirectorRole = System.Configuration.ConfigurationManager.AppSettings["StaffingDirector"];


        private UnitOfWork unitOfWork = new UnitOfWork();
        //
        // GET: /Account/
        [HttpGet]
        [ActionName("Login")]
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                //get the employee code.
                string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

                //  Session["NotificationCount"]= unitOfWork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen==false).Count();


                //  Session["DisplayName"] = _user.RE_Jobdiva_User_Name;//add the name for display.


                //get roles for user..
                //    

                var _user = unitOfWork.User.GetByEmpID(empCd);//get the role for user.
                // return RedirectToAction("UserProfile");
                string adminRole = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
                // redirect to specific view based on role.
                if (_user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == adminRole || _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == HrRoleName)
                    return RedirectToAction("ViewUsers");
                else if (_user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == DevLeadRole)
                    return RedirectToAction("AnnualFeedbackDashboard", "Review");
                else if (_user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == AccMgrRoleName)
                    return RedirectToAction("ClientMonthyDashboardDisplayView", "ClientMonthyDashboard");
                else
                    return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        [ActionName("Login")]
        public ActionResult Login(RIC_Employee user)
        {

            if (ModelState.IsValidField("RU_User_Name") && ModelState.IsValidField("RU_Password"))
            {
                //var users = unitOfWork.User.Get(s => s.RE_Emp_Cd == user.RE_Emp_Cd &&
                //            s.RE_Password == user.RE_Password).ToList();
                var users = unitOfWork.User.Get()
                                .Where(s => s.RE_Emp_Cd.ToLower().Trim() == user.RE_Emp_Cd.ToLower().Trim() && s.RE_Resign_Date==null &&
                                         s.RE_Password == user.RE_Password);

           

                if (users.Count() != 0)
                {
                    string empCd = users.First().RE_Emp_Cd;
                    FormsAuthentication.SetAuthCookie(empCd, false);

                    //get the notification count.
                    Session["NotificationCount"] = unitOfWork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen == false).Count();

                    Session["DisplayName"] = users.FirstOrDefault().RE_Jobdiva_User_Name;//add the name for display.

                    // return RedirectToAction("UserProfile");
                    string AdminRole = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
                    if (users.FirstOrDefault().RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == AdminRole || users.FirstOrDefault().RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == HrRoleName)
                        return RedirectToAction("ViewUsers");
                    else if (users.FirstOrDefault().RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == DevLeadRole)
                        return RedirectToAction("AnnualFeedbackDashboard", "Review");
                    else if (users.FirstOrDefault().RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == AccMgrRoleName)
                        return RedirectToAction("ClientMonthyDashboardDisplayView", "ClientMonthyDashboard");
                    else
                        return RedirectToAction("Index", "Dashboard");

                }
                else
                {
                    ViewBag.errorMessage = "Invalid Username OR Password";
                }
            }
            return View(user);
        }

        public string AccessDenied()
        {
            Response.StatusCode = 403;
            return "Access Denied.";
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string RE_Emp_Cd)
        {
            if (ModelState.IsValid)
            {
                var User = unitOfWork.User.GetByEmpID(RE_Emp_Cd);
                if (User != null)
                {
                    // generate the token.
                    string token = CryptEng.Encrypt(User.RE_Emp_Cd, "sblw-3hn8-sqoy19");

                    // send mail..
                    string error;

                    string ResetLink = @"<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='viewport' content='width=device-width, initial-scale=1'><meta name='ProgId' content='Word.Document'></head><body style='font-family: Arial; font-size: 12px;'><div style='font-size: 14px; width: 50%; margin-left: 25%;box-sizing: unset;border-radius: 7px;border: 1px solid #c5c5c5!important;box-shadow: 3px 3px 10px #bfbfbf;margin-top: 5%;padding: 1%;'><p>Hi,</p><p>You have requested a password reset, please follow the link below to reset your password.</p><p>Please ignore this email if you did not request a password change.</p><p><a href='" + Url.Action("ResetPassword", "Account", new { rt = token }, "http") + "'>Follow this link to reset your password.</a></p></br></br></br></br><p style='font-style: italic;'>Please do not reply to this mail.</p></div></body></html>";

                    Email.SendMail(new string[] { User.RE_Email }, "RMS Password rest link", ResetLink, out error);

                    if (string.IsNullOrEmpty(error))
                        return Content(("<script language='javascript' type='text/javascript'> alert('Password reset link sent to your registered email ');window.location = '" + Url.Action("Login") + "';</script>"));
                    else
                        return Content(("<script language='javascript' type='text/javascript'> alert('" + error + "');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));
                    //    return Content(("<script language='javascript' type='text/javascript'> alert('Password reset link sent to your registered email ');window.location = '/Account/Login';</script>"));

                }
                else
                {
                    return Content(("<script language='javascript' type='text/javascript'> alert('Invalid Employee ID');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));

                }
            }
            return View();
        }

        public ActionResult ChangePassword()
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            string oldPassword = unitOfWork.User.GetByEmpID(empCd).RE_Password;
            ChangePasswordViewModel view = new ChangePasswordViewModel();
            view.OldPasswordCompare = oldPassword;
            return View(view);
        }
        [HttpPost]
        [ActionName("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordViewModel view,string EmpCD, string pass)
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            if (!string.IsNullOrEmpty(EmpCD))
            {
                empCd = EmpCD;
            }
            string oldPassword = unitOfWork.User.GetByEmpID(empCd).RE_Password;
            if (role == AdminRoleName)
            {
                unitOfWork.User.resetPassword(empCd, pass);
                unitOfWork.Save();
                Logout();
                return Content(("<script language='javascript' type='text/javascript'> alert('Password Changed Successfully ');window.location = '" + Url.Action("Login") + "';</script>"));
            }
            else
            {
                if (view.OldPassword != oldPassword)
                    ModelState.AddModelError("OldPassword", "Invalid Password");
            }


            if (ModelState.IsValid)
            {
                unitOfWork.User.resetPassword(empCd, view.NewPassword);
                unitOfWork.Save();
                Logout();
                return Content(("<script language='javascript' type='text/javascript'> alert('Password Changed Successfully ');window.location = '" + Url.Action("Login") + "';</script>"));
            }
            return View();
        }

        [HttpGet]// reset password action.
        public ActionResult ResetPassword(string rt)
        {
            string empID = CryptEng.Decrypt(rt, "sblw-3hn8-sqoy19");
            var foundUserName = unitOfWork.User.GetByEmpID(empID);
            return View(foundUserName);
        }
        [HttpPost]
        public ActionResult ResetPassword(RIC_Employee user)
        {
            if (ModelState.IsValidField("RE_Password") && ModelState.IsValidField("ConfirmPassword"))
            {
                unitOfWork.User.resetPassword(user.RE_Emp_Cd, user.RE_Password);
                unitOfWork.Save();
                return Content(("<script language='javascript' type='text/javascript'> alert('Password Changed Successfully ');window.location = '" + Url.Action("Login") + "';</script>"));
                // return Content(("<script language='javascript' type='text/javascript'> alert('Password Changed Successfully ');window.location = ''" + Url.Action("Login") + "';</script>"));
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateUser()
        {
            RIC_Employee user = new RIC_Employee();
            //Bind Designation
            user.DesignationList = unitOfWork.Designation
                                    .Get().Select(e => new SelectListItem()
                                    {
                                        Value = e.RD_ID.ToString()
                                          ,
                                        Text = e.RD_Designation
                                    });

            user.DepartmentList = unitOfWork.Department
                .Get().Select(e => new SelectListItem()
                {
                    Text = e.RD_Department,
                    Value = e.RD_ID.ToString()
                });
            //user.DepartmentList = new List<SelectListItem>();
            // user.DepartmentList = unitOfWork.User.getDepartmentList();                                
            // bind the role.
            user.RoleList = unitOfWork.Role.getRoleList();
            // bind the manager list.            
            user.ManagerList = unitOfWork.User.getMgrList();
            // bind the submission rule list.
            List<SelectListItem> _lstSubmissionRule = new List<SelectListItem>();
            var submissionRuleList = unitOfWork.SubmissionRule.Get();    //entities.usp_GetRoles();
            foreach (var rule in submissionRuleList)// add the items in select list.
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = rule.RS_Experience;
                selectListItem.Value = rule.RS_Id.ToString();
                _lstSubmissionRule.Add(selectListItem);
            }
            user.SubmissionRule = _lstSubmissionRule;
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateUser(RIC_Employee user, string RE_Mgr_ID, List<string> RE_Reviewer_List)
        {
            if (ModelState.IsValid)
            {
                var emp = unitOfWork.User.GetByEmpID(user.RE_Emp_Cd);
                if (emp == null)
                {
                    //user.RE_ReviewerList = Json(RE_Reviewer_List).ToString();

                    user.RE_Emp_Cd = user.RE_Emp_Cd.ToUpper().Trim();
                    string JobdivName = user.RE_Jobdiva_User_Name;
                    JobdivName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(JobdivName.ToLower());
                    user.RE_Jobdiva_User_Name = JobdivName.Trim();

                    string EmployeeName = user.RE_Employee_Name;
                    EmployeeName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(EmployeeName.ToLower());
                    user.RE_Employee_Name = EmployeeName.Trim();
                    //For all recurtiers the default form id is one.
                    user.RE_AnnualFeedBackFormID = 1;
                    //	user.RE_ReviewerList = JsonConvert.SerializeObject(RE_Reviewer_List);
                    unitOfWork.User.Insert(user);
                    unitOfWork.Save();
                    return Content(("<script language='javascript' type='text/javascript'> alert('User Created Successfully. ');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));
                    // ViewBag.Message = "User Created Successfully.";
                }
                else
                {

                    //  return Content(("<script language='javascript' type='text/javascript'> alert('User with same employee Id already exist.');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));
                    ModelState.AddModelError("", "User with same employee Id already exist.");// add the error message.

                }
            }

            // bind the role.
            user.RoleList = unitOfWork.Role.getRoleList();
            // bind the manager list.            
            user.ManagerList = unitOfWork.User.getMgrList();
            //Bind Designation
            user.DesignationList = unitOfWork.Designation.Get().Select(e => new SelectListItem() { Value = e.RD_ID.ToString(), Text = e.RD_Designation });
            user.DepartmentList = unitOfWork.Department.Get().Select(e => new SelectListItem() { Value = e.RD_ID.ToString(), Text = e.RD_Department });

            // bind the submission rule list.
            List<SelectListItem> _lstSubmissionRule = new List<SelectListItem>();
            var submissionRuleList = unitOfWork.SubmissionRule.Get();    //entities.usp_GetRoles();
            foreach (var rule in submissionRuleList)// add the items in select list.
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = rule.RS_Experience;
                selectListItem.Value = rule.RS_Id.ToString();
                _lstSubmissionRule.Add(selectListItem);
            }
            user.SubmissionRule = _lstSubmissionRule;
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult UpdateUser(int id, string Empid, string FirstName, int? RoleIdFilter, string FilterName, int? page, string StatusName)
        {
            var user = unitOfWork.User.GetByID(id); // unitOfWork.User.Get(s => s.RU_User_Name.ToLower() == userId.ToLower()).First();
            // bind the role.
            bool UserStatus = user.RE_User_Status;
            user.RoleList = unitOfWork.Role.getRoleList();

            user.DesignationList = unitOfWork.Designation.Get().Select(e => new SelectListItem() { Value = e.RD_ID.ToString(), Text = e.RD_Designation });

            // bind the submission rule list.
            List<SelectListItem> _lstSubmissionRule = new List<SelectListItem>();



            var SelectList = new List<SelectListItem>
					 {
							 new SelectListItem { Text = "Active", Value = "False" },
							 new SelectListItem { Text = "Inactive", Value = "True" }
					 };

            if (UserStatus != false)
            {
                SelectList.FirstOrDefault(s => s.Text == "Inactive").Selected = true;
                StatusName = "Inactive";
            }

            ViewBag.SelectList = SelectList;
            var submissionRuleList = unitOfWork.SubmissionRule.Get();    //entities.usp_GetRoles();
            foreach (var rule in submissionRuleList)// add the items in select list.
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = rule.RS_Experience;
                selectListItem.Value = rule.RS_Id.ToString();
                _lstSubmissionRule.Add(selectListItem);
            }
            user.SubmissionRule = _lstSubmissionRule;

            if (user.RE_ReviewerList != null)
                ViewBag.reviewerList = JsonConvert.DeserializeObject<List<string>>(user.RE_ReviewerList);


            ReviewUtitity reviewUtility = new ReviewUtitity();
            ViewBag.reviewerSelectList = reviewUtility.GetReviewerList();

            // bind the manager list.            
            user.ManagerList = unitOfWork.User.getMgrList().Where(manager => manager.Value != user.RE_EmpId.ToString());
            user.RoleID = user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_RoleId;
            user.DesignationList = unitOfWork.Designation.Get().Select(e => new SelectListItem() { Value = e.RD_ID.ToString(), Text = e.RD_Designation });
            //get the department list.
            user.DepartmentList = unitOfWork.User.getDepartmentList();
            user.ConfirmPassword = user.RE_Password;



            ViewUserBack();


            // get the previous url and store it with view model
            ViewBag.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer;


            return View(user);
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateUser(RIC_Employee user, string PreviousUrl, List<string> RE_Reviewer_List1, bool StatusName)
        {
            //who has modified the user.
            user.RE_Upd_User= FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            //get the Designation list.
            user.DesignationList = unitOfWork.Designation.Get().Select(e => new SelectListItem() { Value = e.RD_ID.ToString(), Text = e.RD_Designation });
            //get the department list.
            user.DepartmentList = unitOfWork.User.getDepartmentList();

            user.RE_User_Status = StatusName;

            var SelectList = new List<SelectListItem>
                     {
                             new SelectListItem { Text = "Active", Value = "False" },
                             new SelectListItem { Text = "Inactive", Value = "True" }
                     };

            ViewBag.SelectList = SelectList;

            // bind the role.
            user.RoleList = unitOfWork.Role.getRoleList();
            // bind the manager list.            
            user.ManagerList = unitOfWork.User.getMgrList().Where(manager => manager.Value != user.RE_EmpId.ToString());

            string JobdivName = user.RE_Jobdiva_User_Name;
            JobdivName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(JobdivName.ToLower());
            user.RE_Jobdiva_User_Name = JobdivName.Trim();

            string EmployeeName = user.RE_Employee_Name;
            EmployeeName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(EmployeeName.ToLower());
            user.RE_Employee_Name = EmployeeName.Trim();

            // bind the submission rule list.
            List<SelectListItem> _lstSubmissionRule = new List<SelectListItem>();
            var submissionRuleList = unitOfWork.SubmissionRule.Get();    //entities.usp_GetRoles();
            foreach (var rule in submissionRuleList)// add the items in select list.
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = rule.RS_Experience;
                selectListItem.Value = rule.RS_Id.ToString();
                _lstSubmissionRule.Add(selectListItem);
            }
            user.SubmissionRule = _lstSubmissionRule;
            if (ModelState.IsValid)
            {
                //	user.RE_ReviewerList = JsonConvert.SerializeObject(RE_Reviewer_List);
                // update the user details.
                unitOfWork.User.Update(user);
                //Get Reporting history for employee with latest record.
                var ReportingHistory = unitOfWork.ReportingHistory.Get().Where(e => e.RR_EmpCD == user.RE_Emp_Cd && (e.RR_ToDate == null ? DateTime.Now.Date : e.RR_ToDate) >= DateTime.Now.Date).FirstOrDefault();
                if (ReportingHistory != null)
                {
                    ReportingHistory.RR_ToDate = user.RE_Resign_Date;
                    // Update Reporting history
                    unitOfWork.ReportingHistory.Update(ReportingHistory);
                }

                unitOfWork.Save();
                // return Content(("<script language='javascript' type='text/javascript'> alert('User Modified Successfully.');window.location = '/Account/ViewUsers';</script>"));
                // return Content(("<script language='javascript' type='text/javascript'> alert('User Modified Successfully.');window.location = '"+Url.Action("ViewUsers")+"';</script>"));
                return Content(("<script language='javascript' type='text/javascript'> alert('User Modified Successfully.');window.location = '" + PreviousUrl + "';</script>"));
                // ViewBag.Message = "User modified successfully.";
            }

            // get the previous url and store it with view model
            ViewBag.PreviousUrl = PreviousUrl;

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Reassign(int id)
        {

            ReassignManagerVM reportingHistory = new ReassignManagerVM();
            //RIC_ReportingHistory reportingHistory = new RIC_ReportingHistory();
            RIC_Employee user = unitOfWork.User.GetByID(id);


            reportingHistory.FromDate = unitOfWork.ReportingHistory//get the max date form reproting history
                 .Get(s => s.RR_EmpCD == user.RE_Emp_Cd).Max(s => s.RR_ToDate);

            if (reportingHistory.FromDate.HasValue)
            {
                reportingHistory.FromDate = reportingHistory.FromDate.Value.AddDays(1);
            }
            reportingHistory.JobdivaUserName = user.RE_Jobdiva_User_Name;
            reportingHistory.EmployeeCd = user.RE_Emp_Cd;
            reportingHistory.ManagerList = unitOfWork.User
                .getMgrList();

            ViewUserBack();

            //IEnumerable<SelectListItem> temp = unitOfWork.User.getMgrList(id);
            return View(reportingHistory);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Reassign(ReassignManagerVM ReassignManagerVM)
        {

            if (ModelState.IsValid)
            {

                // get the current manager for user.
                var currentMgr = unitOfWork.ReportingHistory
                    .Get(s => s.RR_EmpCD == ReassignManagerVM.EmployeeCd
                          && (s.RR_ToDate >= ReassignManagerVM.FromDate || s.RR_ToDate == null))
                            .FirstOrDefault();


                //update the previous end date of current reporting manager.
                if (currentMgr != null)
                {
                    DateTime endDate = (DateTime)ReassignManagerVM.FromDate;
                    endDate = endDate.AddDays(-1);
                    currentMgr.RR_ToDate = endDate;
                    unitOfWork.ReportingHistory.Update(currentMgr);
                }

                RIC_ReportingHistory reportingHistory = new RIC_ReportingHistory();
                var target = unitOfWork.User.GetMonthlyTarget(ReassignManagerVM.EmployeeCd,Convert.ToDateTime( ReassignManagerVM.FromDate).AddDays(-1).ToString("MMMM"),Convert.ToDateTime(ReassignManagerVM.FromDate).Year);
                if (target != null) 
                {

                    target.Mgr_Cd= ReassignManagerVM.ManagerId;
                    target.Updated_By= User.Identity.Name;
                    target.Updated_Date= SystemClock.US_Date;
                    
                }

                //Assign the reporting manager.
                //	RIC_Employee user = unitOfWork.User.GetByID(reportingHistory.MgrId);
                reportingHistory.RR_CreatedDate = SystemClock.US_Date;
                reportingHistory.RR_EmpCD = ReassignManagerVM.EmployeeCd;
                reportingHistory.RR_MgrCD = ReassignManagerVM.ManagerId;
                reportingHistory.RR_UpdatedBy = User.Identity.Name;
                reportingHistory.RR_FromDate = ReassignManagerVM.FromDate;
                reportingHistory.RR_ToDate = ReassignManagerVM.ToDate;
                unitOfWork.ReportingHistory.Insert(reportingHistory);
                unitOfWork.Save();
                // return Content(("<script language='javascript' type='text/javascript'> alert('User Modified Successfully.');window.location = '/Account/ViewUsers';</script>"));
                return Content(("<script language='javascript' type='text/javascript'> alert('User Modified Successfully.');window.location = '" + Url.Action("ViewUsers") + "';</script>"));
            }

            var EmployeeDetails = unitOfWork.User.GetByEmpID(ReassignManagerVM.EmployeeCd);
            ReassignManagerVM.EmployeeCd = EmployeeDetails.RE_Emp_Cd;
            ReassignManagerVM.ManagerList = unitOfWork.User
                                                    .getMgrList();
            //var mgrList = unitOfWork.User.getMgrList(roleID).ToList();
            return View(ReassignManagerVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ReportingHistory(int id)
        {
            RIC_Employee user = unitOfWork.User.GetByID(id);
            //get the reporting history for the user.
            IEnumerable<ReportingHistoryVM> reportingHistory = unitOfWork.User.getDirectReportingHistory(user.RE_Emp_Cd).
                  OrderByDescending(s => s.RR_FromDate).Select(s => new ReportingHistoryVM
                  {
                      Id = s.RR_ID,
                      ManagerName = s.Mgr_Name,
                      FromDate = s.RR_FromDate,
                      ToDate = s.RR_ToDate
                  }).ToList();
            TempData["Id"] = id;
            ViewUserBack();


            return View(reportingHistory);
        }


        [HttpGet]
        public ActionResult EditReportingHistory(int id)
        {
            // Get the manager from db.
            RIC_ReportingHistory reportingHistory = unitOfWork.ReportingHistory.GetByID(id);

            EditReportingHistoryVM editReportingHistoryVM = new EditReportingHistoryVM();

            //get the user from db.
            RIC_Employee user = unitOfWork.User.GetByEmpID(reportingHistory.RR_EmpCD);

            editReportingHistoryVM.Id = reportingHistory.RR_ID;
            editReportingHistoryVM.EmployeeCd = reportingHistory.RR_EmpCD;
            editReportingHistoryVM.JobdivaUserName = user.RE_Jobdiva_User_Name;
            //get the manager list for the user.
            editReportingHistoryVM.ManagerList = unitOfWork.User.getMgrList();
            editReportingHistoryVM.ManagerId = reportingHistory.RR_MgrCD;
            editReportingHistoryVM.FromDate = reportingHistory.RR_FromDate;
            editReportingHistoryVM.ToDate = reportingHistory.RR_ToDate;
            ViewBag.Id = TempData["Id"];

            ViewUserBack();
            return View(editReportingHistoryVM);

        }
        [HttpPost]
        public ActionResult EditReportingHistory(EditReportingHistoryVM editReportingHistoryVM)
        {
            if (ModelState.IsValid)
            {
                // get the current manager for user.
                var currentMgr = unitOfWork.ReportingHistory.Get(s =>
                            s.RR_EmpCD == editReportingHistoryVM.EmployeeCd && s.RR_ToDate >= editReportingHistoryVM.FromDate).FirstOrDefault();


                //update the previous end date of current reporting manager.
                if (currentMgr != null)
                {
                    DateTime endDate = (DateTime)editReportingHistoryVM.FromDate;
                    endDate = endDate.AddDays(-1);
                    currentMgr.RR_ToDate = endDate;
                    unitOfWork.ReportingHistory.Update(currentMgr);
                }

                // update the manager details.
                RIC_ReportingHistory rHistory = unitOfWork.ReportingHistory.GetByID(editReportingHistoryVM.Id);
                //	RIC_Employee mgr = unitOfWork.User.GetByID(reportingHistory.MgrId);
                rHistory.RR_CreatedDate = SystemClock.US_Date;
                rHistory.RR_EmpCD = editReportingHistoryVM.EmployeeCd;
                rHistory.RR_MgrCD = editReportingHistoryVM.ManagerId;
                rHistory.RR_FromDate = editReportingHistoryVM.FromDate;
                rHistory.RR_ToDate = editReportingHistoryVM.ToDate;
                rHistory.RR_UpdatedBy = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                unitOfWork.ReportingHistory.Update(rHistory);
                unitOfWork.Save();

                return Content(("<script language='javascript' type='text/javascript'> alert('User Modified Successfully.');window.location = '" + Url.Action("ViewUsers") + "';</script>"));
            }
            //get the user from db.
            RIC_Employee user = unitOfWork.User.GetByID(editReportingHistoryVM.EmployeeCd);
            //get the manager list for the user.
            editReportingHistoryVM.ManagerList = unitOfWork.User.getMgrList();
            editReportingHistoryVM.ManagerId = unitOfWork.User.GetByEmpID(editReportingHistoryVM.ManagerId).RE_Emp_Cd;

            return View(editReportingHistoryVM);
        }

        [NonAction]
        public void ViewUserBack()
        {
            ViewBag.Firstname = Request.QueryString["FirstName"];
            ViewBag.Empcode = Request.QueryString["Empid"];
            ViewBag.RoleIdFilter = Request.QueryString["Role"];
            ViewBag.FilterName = Request.QueryString["FilterName"];
            ViewBag.Page = Request.QueryString["page"];
            ViewBag.ReportingID=Request.QueryString["ReportingID"];

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            RIC_ReportingHistory rHistory = unitOfWork.ReportingHistory.GetByID(id);

            unitOfWork.ReportingHistory.Delete(rHistory);
            unitOfWork.Save();
            ViewUserBack();

            return RedirectToAction("ReportingHistory", new { id = TempData["Id"], First_Name = ViewBag.Firstname, Empid = ViewBag.Empcode, Role = ViewBag.RoleIdFilter, FilterName = ViewBag.FilterName, Page = ViewBag.Page });
        }

        #region Create role action

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateRole()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateRole(RIC_Role role)
        {
            // create the role.
            if (ModelState.IsValid)
            {
                role.RR_Create_Dt = SystemClock.US_Date;
                unitOfWork.Role.Insert(role);
                unitOfWork.Save();
            }
            return View(role);
        }
        [HttpGet]
        public ActionResult CreateMenu()
        {

            return View();
        }
        [HttpPost]

        public ActionResult CreateMenu(RIC_Menu_Module menu)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Menu.Insert(menu);
                unitOfWork.Save();
            }
            return View(menu);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult ViewMenu()
        {
            List<RIC_Menu_Module> menuList = unitOfWork.Menu.Get().ToList();

            return View(menuList);
        }
        #endregion

        [Authorize]
        [HttpGet]
        public ActionResult UserProfile()
        {
            //get the user details.
            string empId = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var user = unitOfWork.User.Get(s => s.RE_Emp_Cd.ToLower() == empId.ToLower()).First();
            user.RoleID = user.RIC_User_Role.First().RUR_Role_ID;


            user.ConfirmPassword = user.RE_Password;
            return View(user);
        }
        [Authorize]
        [HttpPost]
        public ActionResult UserProfile(RIC_Employee user)
        {

            //ModelState.Remove("ConfirmPassword");
            // ModelState.Remove("RE_Password");
            user.ConfirmPassword = user.RE_Password;
            if (ModelState.IsValid)
            {
                // update the user details.
                unitOfWork.User.Update(user);
                unitOfWork.Save();
                return Content(("<script language='javascript' type='text/javascript'> alert('Profile modified successfully.');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));
                // ViewBag.Message = "Profile modified successfully.";
            }
            return View(user);
        }

        [Authorize(Roles = "Admin,Manager,Team Lead,HR Manager")]
        [HttpGet]
        public ActionResult UserDetails(int id)
        {
            // get the user.
            var user = unitOfWork.User.GetByID(id);



            return View(user);
        }

        [Authorize(Roles = "Admin,Manager,Team Lead,Director,HR Manager,Director-Staffing")]
        //[HttpGet]
        public ActionResult ViewUsers(int? page, int? RoleID,string filterDdl, string Emp_Code, string First_Name, string SearchBtn,string ReportingID)
        {
            string empCd = User.Identity.Name;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            DateTime usDate = SystemClock.US_Date;
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName };
            //get the reporting history for user.
            var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role, "REC")
                                   .Select(s => s.RR_EmpCD).ToList();
            if (!(First_Name == null) && !(First_Name=="")) 
            { First_Name = First_Name.Trim(); }

            if (!(Emp_Code == null) && !(Emp_Code==""))
            { Emp_Code = Emp_Code.Trim(); }
            if (RoleID == null)
            {
                if (Request.QueryString["Role"] != null)
                    RoleID = Convert.ToInt32(Request.QueryString["Role"]);
            }

            var SelectList = new List<SelectListItem>
					 {
							 new SelectListItem { Text = "All", Value = "All" },
							 new SelectListItem { Text = "Active", Value = "Active" },
							 new SelectListItem { Text = "Inactive", Value = "Inactive" }
					 };
            if (filterDdl != null)
                SelectList.FirstOrDefault(s => s.Value == filterDdl).Selected = true;
            else
            {
                SelectList.FirstOrDefault(s => s.Value == "Active").Selected = true;
                filterDdl = "Active";
            }

            var ReportingToList = unitOfWork.User.getAllUsers()
                     .Where(s => roleList.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name)
                            && reportingHistory.Contains(s.RE_Emp_Cd))
                     .Select(s => new SelectListItem
                     {
                         Text = s.RE_Jobdiva_User_Name,
                         Value = s.RE_Jobdiva_User_Name
                     }).OrderBy(o => o.Text).ToList();

            if (ReportingID != null)
            {
                if (ReportingID != "")
                {
                    ReportingToList.FirstOrDefault(s=>s.Value == ReportingID).Selected=true;
                }
                // unitOfWork.User.getMgrList1().FirstOrDefault(s => s.Value == ReportingID).Selected = true;
            }

            page = SearchBtn == "SearchBtn" ? 1 : page;


            ViewBag.SelectList = SelectList;
            int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            string AdminRoleName = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
            // bind the role.
            IEnumerable<RIC_Employee> listOfUsers;

            ViewBag.role = role;

            if (role == AdminRoleName || role == directorRoleName || role == HrRoleName || role == AccMgrRoleName)
            {
                //get the users from db. unitOfWork.User.Get
                listOfUsers = unitOfWork.User.getAllUsersListWithLatestReporting().Where(s =>
                     s.RIC_User_Role.FirstOrDefault().RUR_Role_ID != adminRoleId &&
                     ((Emp_Code == null || Emp_Code == "") || s.RE_Emp_Cd.ToLower().StartsWith(Emp_Code.ToLower())) &&
                     ((First_Name == null || First_Name == "") || s.RE_Jobdiva_User_Name.ToLower().Contains(First_Name.ToLower())) &&
                     ((RoleID == null) || s.RIC_User_Role.Where(sr => sr.RUR_Role_ID == RoleID).Count() != 0) &&
                     ((ReportingID == null || ReportingID == "") || s.ReportingTo == ReportingID) &&
                     ((filterDdl == "Active") ? (s.RE_Resign_Date == null || s.RE_Resign_Date > SystemClock.US_Date) : (filterDdl == "Inactive") ? s.RE_Resign_Date <= SystemClock.US_Date : true)
                     )
                     .ToPagedList(pageIndex, 10);
                ViewBag.showExportBtn = "Yes";
            }
            else// if role is not admin. unitOfWork.User.Get
            {
                listOfUsers = unitOfWork.User.getAllUsers().Where(s => s.MgrCD == empCd &&
                     (s.RE_Emp_Cd != empCd) &&
                     ((Emp_Code == null || Emp_Code == "") || s.RE_Emp_Cd.ToLower().StartsWith(Emp_Code.ToLower())) &&
                    ((First_Name == null || First_Name == "") || s.RE_Jobdiva_User_Name.ToLower().Contains(First_Name.ToLower())) &&
                    ((RoleID == null) || s.RIC_User_Role.Where(sr => sr.RUR_Role_ID == RoleID).Count() != 0) &&
                    ((ReportingID == null || ReportingID == "") || s.ReportingTo == ReportingID) &&
                     ((filterDdl == "Active") ? (s.RE_Resign_Date == null || s.RE_Resign_Date > SystemClock.US_Date) : (filterDdl == "Inactive") ? s.RE_Resign_Date <= SystemClock.US_Date : true)
                    )
                    .ToPagedList(pageIndex, 10);
                ViewBag.showExportBtn = "No";
            }
            //listOfUsers = unitOfWork.User.getAllUsers().Where(s => s.RIC_EmployeeRef.RE_Emp_Cd == empId &&
            //     ((Emp_Code == null || Emp_Code == "") || s.RE_Emp_Cd.ToLower().StartsWith(Emp_Code.ToLower())) &&
            //    ((First_Name == null || First_Name == "") || s.RE_Employee_Name.ToLower().StartsWith(First_Name.ToLower())) &&
            //    ((RoleID == null) || s.RIC_User_Role.Where(sr => sr.RUR_Role_ID == RoleID).Count() != 0)
            //    )
            //    .ToPagedList(pageIndex, 10);

            // add the role list 
            ViewBag.RoleList = unitOfWork.Role.getRoleList();
            ViewBag.MgrList= ReportingToList;
            ViewBag.ReportingTo = ReportingID;


            //string url = string.Format("Emp_Code={0}&First_Name={1}&filterDdl={2}&RoleID={3}", Emp_Code, First_Name, filterDdl, RoleID);
            //ViewBag.ReturnUrl = url;


            ViewBag.Page = page;
            ViewBag.Empcode = Emp_Code;
            ViewBag.First = First_Name;
            ViewBag.Filter = filterDdl;
            ViewBag.RoleIdFilter = RoleID;


            return View(listOfUsers);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Team Lead,Director,Director-Staffing")]
        public ActionResult AddTargets()
        {
            string empCd = User.Identity.Name;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            DateTime usDate = SystemClock.US_Date;
            var model = new AddTargetsVM();
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName };
            //get the reporting history for user.
            var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role,"REC")
                                   .Select(s => s.RR_EmpCD).ToList();
            reportingHistory.Add(empCd);
            //get the user list for tl.
            model.ReportingList = unitOfWork.User.getAllUsers()
                     .Where(s => roleList.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name)
                            && reportingHistory.Contains(s.RE_Emp_Cd))
                     .Select(s => new SelectListItem
                     {
                         Text = s.RE_Jobdiva_User_Name
                         ,Value = s.RE_Emp_Cd
                     }).OrderBy(o=>o.Text).ToList();
            //get the year list.
            model.YearList = Enumerable.Range(2001, usDate.Year - 2000).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s=>s.Text).ToList();
            ////get the month list.
            //model.MonthList = Enumerable.Range(1, 12)
            //                  .Select(i => new SelectListItem
            //                          {
            //                              Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i),
            //                              Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
            //                          }).ToList();
            // get the quarter List for select list. 
            model.QuarterList = new List<SelectListItem>()
            {
                new SelectListItem{ Text="Quarter 1",Value="Quarter 1"},
                new SelectListItem{ Text="Quarter 2",Value="Quarter 2"},
                new SelectListItem{ Text="Quarter 3",Value="Quarter 3"},
                new SelectListItem{ Text="Quarter 4",Value="Quarter 4"},
            };

            model.Year = usDate.Year;
            // model.Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month);
            model.Quarter = "Quarter " + (usDate.Month <= 3?1: usDate.Month>=4 && usDate.Month <= 6?2: usDate.Month >= 7 && usDate.Month <= 9 ? 3:4);
            //month has been changed to quarter, so need to get lastmonth name.
            //model.PreviousMonth = DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month-1);

            List<string> editForRoleList = new List<string>(){
                 directorRoleName,stafingDirectorRole,AdminRoleName
             };
            bool editForRole = editForRoleList.Contains(role);
            ViewBag.Header = editForRole ? "Add Targets" : "View Targets";
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Team Lead,Director,Director-Staffing")]
        public ActionResult AddTargets(AddTargetsVM model, string submit)
        {
            ModelState.Clear();
            string empCd = User.Identity.Name;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            //get the date.
            DateTime usDate = SystemClock.US_Date;
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName };

            bool allowEdit = true; //usDate.Year == model.Year&& DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month) == model.Month;
            List<string> editForRoleList = new List<string>(){
                 directorRoleName,stafingDirectorRole,AdminRoleName
             };
            bool editForRole = editForRoleList.Contains(role);
            ViewBag.Header = editForRole ? "Add Targets" : "View Targets";
            //get the year list.
            model.YearList = Enumerable.Range(2001, usDate.Year - 2000).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Text).ToList();
            ////get the month list.
            //model.MonthList = Enumerable.Range(1, 12)
            //                  .Select(i => new SelectListItem
            //                  {
            //                      Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i),
            //                      Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
            //                  }).ToList();

            // get the quarter List for select list. 
            model.QuarterList = new List<SelectListItem>()
            {
                new SelectListItem{ Text="Quarter 1",Value="Quarter 1"},
                new SelectListItem{ Text="Quarter 2",Value="Quarter 2"},
                new SelectListItem{ Text="Quarter 3",Value="Quarter 3"},
                new SelectListItem{ Text="Quarter 4",Value="Quarter 4"},
            };


            //get the reporting history for user.
            var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role,"REC")
                                   .Select(s => s.RR_EmpCD).ToList();

            reportingHistory.Add(empCd);
            //get the user list for tl.
            model.ReportingList = unitOfWork.User.getAllUsers()
                     .Where(s => roleList.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name)
                            && reportingHistory.Contains(s.RE_Emp_Cd))
                     .Select(s => new SelectListItem
                     {
                         Text = s.RE_Jobdiva_User_Name
                         ,Value = s.RE_Emp_Cd
                     }).OrderBy(o=>o.Text).ToList();

            
            if (submit == "SearchBtn")
            {
                //get the target for the team.
                // var targets = unitOfWork.User.GetTargetForTeam(model.EmpCd, model.Month, model.Year);
                var targets = unitOfWork.User.GetTargetForTeam(model.EmpCd, model.Quarter, model.Year);
                //bind the employee list.

                if (allowEdit)
                {
                    //get the current reporting history with target.
                    model.EmployeeList = (from rh in unitOfWork.User.getReportingHistory(empCd, usDate, usDate, "Admin")
                                            .Where(w => w.RR_MgrCD == model.EmpCd||w.RR_EmpCD==model.EmpCd)
                                          join emp in unitOfWork.User.Get() on rh.RR_EmpCD equals emp.RE_Emp_Cd
                                          join target in targets on rh.RR_EmpCD equals target.RT_Emp_Cd into lt
                                          from target in lt.DefaultIfEmpty()
                                          select new AddTargetEmployeeList
                                                {
                                                    TargetId = target == null ? 0 : target.RT_ID
                                                    ,ReportingID=rh.RR_ID
                                                    ,EmpCd = rh.RR_EmpCD
                                                    ,MgrCd = rh.RR_MgrCD
                                                    ,EmployeeName = rh.Employee_Name
                                                    ,ReportingTo = rh.Mgr_Name
                                                    ,TotalExp = emp.RE_Experience
                                                    ,Year = target == null ? model.Year : target.RT_Year
                                                   // ,Month = target == null ? model.Month : target.RT_Month
                                                   ,Quarter = target == null ? model.Quarter : target.RT_Month
                                                    ,
                                              ExpInSunrise = emp.RE_Joining_Date.HasValue ? GetDateDif(emp.RE_Joining_Date.Value, usDate.Date) : null
                                                    ,SubmissionTarget = target == null ? (float?)null : target.RT_SubmissionsTarget
                                                    ,InterviewTarget = target == null ? (float?)null : target.RT_InterviewsTarget
                                                    ,HiresTarget = target == null ? (float?)null : target.RT_HiresTarget
                                                    ,Comments = target == null ? null : target.RT_Comments
                                                    ,EditCheckBox = target == null ? true : false
                                                }).OrderBy(o=>o.EmployeeName).ToList();
                }else                
                {
                    //get the data from tragt table 
                    model.EmployeeList = (from target in targets
                                          join emp in unitOfWork.User.Get() on target.RT_Emp_Cd equals emp.RE_Emp_Cd
                                          join mgr in unitOfWork.User.Get() on target.RT_MgrCd equals mgr.RE_Emp_Cd
                                          select new AddTargetEmployeeList
                                          {
                                              TargetId = target == null ? 0 : target.RT_ID
                                              ,EmpCd = target.RT_Emp_Cd
                                              ,MgrCd = target == null ? model.EmpCd : target.RT_MgrCd
                                              ,EmployeeName =emp.RE_Jobdiva_User_Name
                                              ,ReportingTo = mgr.RE_Jobdiva_User_Name
                                              ,TotalExp = emp.RE_Experience
                                              ,Year = target == null ? model.Year : target.RT_Year
                                              //,Month = target == null ? model.Month : target.RT_Month
                                              ,Quarter = target == null ? model.Quarter : target.RT_Month
                                              ,
                                              ExpInSunrise = emp.RE_Joining_Date.HasValue ? GetDateDif(emp.RE_Joining_Date.Value, usDate.Date) : null
                                              ,SubmissionTarget = target == null ? (float?)null : target.RT_SubmissionsTarget
                                              ,InterviewTarget = target == null ? (float?)null : target.RT_InterviewsTarget
                                              ,HiresTarget = target == null ? (float?)null : target.RT_HiresTarget
                                              ,Comments = target == null ? null : target.RT_Comments
                                              //,Edit = target == null ? true : false
                                          }).OrderBy(o => o.EmployeeName).ToList();
                   
                }
                model.SelectAllCheck = targets.Count() == 0;
                ViewBag.allowEdit = allowEdit && editForRole;
                ViewBag.ButtonName = targets.Count() == 0 ? "SaveBtn" : "UpdateBtn";
                ViewBag.ButtonText = targets.Count() == 0 ? "Save" : "Update";
            }
            else if (submit == "SearchResetBtn")
            {
                return RedirectToAction("AddTargets");
            }
            else if (submit == "SaveBtn" )
            {
                List<RIC_Targets> targetlist = model.EmployeeList.Where(w=>w.EditCheckBox)
                                                .Select(s => new RIC_Targets
                                                {
                                                     RT_Emp_Cd = s.EmpCd
                                                    ,RT_ReportingId=s.ReportingID
                                                    ,RT_MgrCd = s.MgrCd
                                                    ,RT_SubmissionsTarget = s.SubmissionTarget.Value
                                                    ,RT_InterviewsTarget = s.InterviewTarget.Value
                                                    ,RT_HiresTarget = s.HiresTarget.Value
                                                    ,RT_Comments = s.Comments
                                                    //,RT_Month = s.Month
                                                    ,RT_Month = s.Quarter
                                                    ,
                                                    RT_Year =s.Year
                                                    ,RT_CreatedBy = empCd
                                                    ,RT_CreatedDate = usDate
                                                }).ToList();
                unitOfWork.User.InsertTarget(targetlist);
                //update target history.
                List<RIC_TargetsHistory> targetHistory = model.EmployeeList.Where(w => w.EditCheckBox)
                                                .Select(s => new RIC_TargetsHistory
                                                {
                                                    RT_Emp_Cd = s.EmpCd,
                                                    RT_ReportingId = s.ReportingID,
                                                    RT_MgrCd = s.MgrCd,
                                                    RT_SubmissionsTarget = s.SubmissionTarget.Value,
                                                    RT_InterviewsTarget = s.InterviewTarget.Value,
                                                    RT_HiresTarget = s.HiresTarget.Value,
                                                    RT_Comments = s.Comments,
                                                    //RT_Month = s.Month,
                                                    RT_Month = s.Quarter,
                                                    RT_Year = s.Year,
                                                    RT_CreatedBy = empCd,
                                                    RT_CreatedDate = usDate
                                                }).ToList();
                unitOfWork.User.InsertTargetHistory(targetHistory);
                unitOfWork.Save();
                //get the target for the team.
                var targets = unitOfWork.User.GetTargetForTeam(model.EmpCd, model.Quarter, model.Year);
                //get the current reporting history with target.
                //get the current reporting history with target.
                model.EmployeeList = (from rh in unitOfWork.User.getReportingHistory(empCd, usDate, usDate, "Admin")
                                        .Where(w => w.RR_MgrCD == model.EmpCd || w.RR_EmpCD == model.EmpCd)
                                      join emp in unitOfWork.User.Get() on rh.RR_EmpCD equals emp.RE_Emp_Cd
                                      join target in targets on rh.RR_EmpCD equals target.RT_Emp_Cd into lt
                                      from target in lt.DefaultIfEmpty()
                                      select new AddTargetEmployeeList
                                      {
                                          TargetId = target == null ? 0 : target.RT_ID,
                                          ReportingID = rh.RR_ID,
                                          EmpCd = rh.RR_EmpCD,
                                          MgrCd = rh.RR_MgrCD,
                                          EmployeeName = rh.Employee_Name,
                                          ReportingTo = rh.Mgr_Name,
                                          TotalExp = emp.RE_Experience,
                                          Year = target == null ? model.Year : target.RT_Year,
                                          //Month = target == null ? model.Month : target.RT_Month,
                                          Quarter = target == null ? model.Quarter : target.RT_Month,
                                          ExpInSunrise = emp.RE_Joining_Date.HasValue ? GetDateDif(emp.RE_Joining_Date.Value, usDate.Date) : null,
                                          SubmissionTarget = target == null ? (float?)null : target.RT_SubmissionsTarget,
                                          InterviewTarget = target == null ? (float?)null : target.RT_InterviewsTarget,
                                          HiresTarget = target == null ? (float?)null : target.RT_HiresTarget,
                                          Comments = target == null ? null : target.RT_Comments,
                                          EditCheckBox = target == null ? true : false
                                      }).OrderBy(o => o.EmployeeName).ToList();
                model.SelectAllCheck = targets.Count() == 0;
                ViewBag.ButtonText =  "Update";
                ViewBag.ButtonName = "UpdateBtn";
                ViewBag.allowEdit = allowEdit && editForRole;
                ViewBag.alertText = "Targets updated successfully.";
            }
            else if (submit == "UpdateBtn")
            {
                foreach (var item in model.EmployeeList.Where(w => w.EditCheckBox))
                {
                    var target = unitOfWork.User.getAllTargets(s => s.RT_ID == item.TargetId);
                    if (target.Count() != 0)
                    {
                        //update the records.
                        target.First().RT_SubmissionsTarget = item.SubmissionTarget.Value;
                        target.First().RT_InterviewsTarget = item.InterviewTarget.Value;
                        target.First().RT_HiresTarget = item.HiresTarget.Value;
                        target.First().RT_Comments = item.Comments;
                        target.First().RT_UpdatedBy = empCd;
                        target.First().RT_UpdatedDate = usDate;
                    }
                    else
                    {
                        //Insert the records.
                        unitOfWork.User.InsertTarget(new RIC_Targets
                                                {
                                                    RT_Emp_Cd = item.EmpCd,
                                                    RT_ReportingId=item.ReportingID,
                                                    RT_MgrCd = item.MgrCd,
                                                    RT_SubmissionsTarget = item.SubmissionTarget.Value,
                                                    RT_InterviewsTarget = item.InterviewTarget.Value,
                                                    RT_HiresTarget = item.HiresTarget.Value,
                                                    RT_Comments = item.Comments,
                                                    //RT_Month = item.Month,
                                                    RT_Month = item.Quarter,
                                                    RT_Year = item.Year,
                                                    RT_CreatedBy = empCd,
                                                    RT_CreatedDate = usDate
                                                });
                    }
                }
                unitOfWork.Save();
                //update target history.
                List<RIC_TargetsHistory> targetlist = model.EmployeeList.Where(w => w.EditCheckBox)
                                                .Select(s => new RIC_TargetsHistory
                                                {
                                                    RT_Emp_Cd = s.EmpCd,
                                                    RT_ReportingId = s.ReportingID,
                                                    RT_MgrCd = s.MgrCd,
                                                    RT_SubmissionsTarget = s.SubmissionTarget.Value,
                                                    RT_InterviewsTarget = s.InterviewTarget.Value,
                                                    RT_HiresTarget = s.HiresTarget.Value,
                                                    RT_Comments = s.Comments,
                                                   // RT_Month = s.Month,
                                                    RT_Month = s.Quarter,
                                                    RT_Year = s.Year,
                                                    RT_CreatedBy = empCd,
                                                    RT_CreatedDate = usDate
                                                }).ToList();
                unitOfWork.User.InsertTargetHistory(targetlist);
                unitOfWork.Save();
                //get the target for the team.
                var targets = unitOfWork.User.GetTargetForTeam(model.EmpCd, model.Quarter, model.Year);
                //get the current reporting history with target.
                model.EmployeeList = (from rh in unitOfWork.User.getReportingHistory(empCd, usDate, usDate, "Admin")
                                        .Where(w => w.RR_MgrCD == model.EmpCd || w.RR_EmpCD == model.EmpCd)
                                      join emp in unitOfWork.User.Get() on rh.RR_EmpCD equals emp.RE_Emp_Cd
                                      join target in targets on rh.RR_EmpCD equals target.RT_Emp_Cd into lt
                                      from target in lt.DefaultIfEmpty()
                                      select new AddTargetEmployeeList
                                      {
                                          TargetId = target == null ? 0 : target.RT_ID,
                                          ReportingID = rh.RR_ID,
                                          EmpCd = rh.RR_EmpCD,
                                          MgrCd = rh.RR_MgrCD,
                                          EmployeeName = rh.Employee_Name,
                                          ReportingTo = rh.Mgr_Name,
                                          TotalExp = emp.RE_Experience,
                                          Year = target == null ? model.Year : target.RT_Year,
                                          //   Month = target == null ? model.Month : target.RT_Month,
                                          Quarter = target == null ? model.Quarter : target.RT_Month,
                                          ExpInSunrise = emp.RE_Joining_Date.HasValue ? GetDateDif(emp.RE_Joining_Date.Value, usDate.Date) : null,
                                          SubmissionTarget = target == null ? (float?)null : target.RT_SubmissionsTarget,
                                          InterviewTarget = target == null ? (float?)null : target.RT_InterviewsTarget,
                                          HiresTarget = target == null ? (float?)null : target.RT_HiresTarget,
                                          Comments = target == null ? null : target.RT_Comments,
                                          EditCheckBox = target == null ? true : false
                                      }).OrderBy(o => o.EmployeeName).ToList();
                model.SelectAllCheck = targets.Count() == 0;
                ViewBag.ButtonText = "Update";
                ViewBag.ButtonName = "UpdateBtn";
                ViewBag.allowEdit = allowEdit && editForRole;
                ViewBag.alertText = "Targets updated successfully.";
            }           
            return View(model);
        }

        public JsonResult GetTargetHistoryForUser(string empCd)
        {
            var targetList = (from target in unitOfWork.User.getAllTargetsHistory(s => s.RT_Emp_Cd == empCd)
                             join emp in unitOfWork.User.Get()
                             on target.RT_Emp_Cd equals emp.RE_Emp_Cd
                             join mgr in unitOfWork.User.Get()
                             on target.RT_MgrCd equals mgr.RE_Emp_Cd
                             orderby target.RT_CreatedDate
                             select new
                             {
                                 EmpCd=target.RT_Emp_Cd,
                                 EmployeeName=emp.RE_Jobdiva_User_Name,
                                 ManagerName = mgr.RE_Jobdiva_User_Name,
                                 Month=target.RT_Month,
                                 Year=target.RT_Year,
                                 SubmissionTarget=target.RT_SubmissionsTarget,
                                 InterviewTarget=target.RT_InterviewsTarget,
                                 HiresTarget=target.RT_HiresTarget,
                                 UpdatedBy=unitOfWork.User.GetByEmpID(target.RT_CreatedBy).RE_Jobdiva_User_Name,
                                 UpdateDate=target.RT_CreatedDate.ToString(),
                                 Comments = target.RT_Comments==null?"":target.RT_Comments
                             }).ToList();
            return Json(targetList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//export to xls.
        public ActionResult ExportToExcel(int? RoleID, string filterDdl, string Emp_Code, string First_Name)
        {
            string AdminRoleName = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            string role = Roles.GetRolesForUser()[0];
            ViewBag.role = role;
            IEnumerable<RIC_Employee> listOfUsers;
            if (role == AdminRoleName || role == directorRoleName || role == HrRoleName || role == AccMgrRoleName)
                //get the users from db. unitOfWork.User.Get
                listOfUsers = unitOfWork.User.getAllUsers().Where(s =>
                     s.RIC_User_Role.FirstOrDefault().RUR_Role_ID != adminRoleId &&
                     ((Emp_Code == null || Emp_Code == "") || s.RE_Emp_Cd.ToLower().StartsWith(Emp_Code.ToLower())) &&
                     ((First_Name == null || First_Name == "") || s.RE_Jobdiva_User_Name.ToLower().Contains(First_Name.ToLower())) &&
                     ((RoleID == null) || s.RIC_User_Role.Where(sr => sr.RUR_Role_ID == RoleID).Count() != 0) &&
                     ((filterDdl == "Active") ? s.RE_Resign_Date == null : (filterDdl == "Inactive") ? s.RE_Resign_Date <= SystemClock.US_Date : true)

                     );
            else// if role is not admin. unitOfWork.User.Get
                listOfUsers = unitOfWork.User.getAllUsers().Where(s => s.MgrCD == empCd &&
                     (s.RE_Emp_Cd != empCd) &&
                     ((Emp_Code == null || Emp_Code == "") || s.RE_Emp_Cd.ToLower().StartsWith(Emp_Code.ToLower())) &&
                    ((First_Name == null || First_Name == "") || s.RE_Jobdiva_User_Name.ToLower().Contains(First_Name.ToLower())) &&
                    ((RoleID == null) || s.RIC_User_Role.Where(sr => sr.RUR_Role_ID == RoleID).Count() != 0) &&
                     ((filterDdl == "Active") ? s.RE_Resign_Date == null : (filterDdl == "Inactive") ? s.RE_Resign_Date <= SystemClock.US_Date : true)
                    );
            if (listOfUsers.Count() != 0)
                using (ExcelPackage pck = new ExcelPackage())
                {
                    var tableToExport = listOfUsers.Select(s => new
                    {
                        Employee_ID = s.RE_Emp_Cd,
                        Jobdiva_User_Name = s.RE_Jobdiva_User_Name,
                        Reporting_To = s.ReportingTo,
                        From_Date = s.RE_Start_Date,
                        To_Date = s.RE_End_Date,
                        Role = s.RIC_User_Role.First().RIC_Role.RR_Role_Name
                    }).ToList();

                    DataTable dt = ToDataTable(tableToExport);
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("SearchReport");
                    ws.Cells["A1"].LoadFromDataTable(dt, true, TableStyles.Medium15); //You can Use TableStyles property of your desire.    
                    //Read the Excel file in a byte array    
                    Byte[] fileBytes = pck.GetAsByteArray();

                    var response = new FileContentResult(fileBytes, "application/octet-stream");
                    response.FileDownloadName = "UserDetails.xlsx";// set the file name
                    return response;// download file.
                }
            else
            {
                return Content(("<script language='javascript' type='text/javascript'> alert('No data found');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));

            }
        }

        [NonAction]
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable();
            //Get all the properties    
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names    
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows    
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);

            }
            //put a breakpoint here and check datatable    
            return dataTable;
        }

        public ActionResult ChangeStatus(int id)
        {
            RIC_Employee employee = unitOfWork.User.GetByID(id);
            employee.RE_User_Status = !employee.RE_User_Status;
            employee.ConfirmPassword = employee.RE_Password;
            employee.RoleID = employee.RIC_User_Role.FirstOrDefault().RIC_Role.RR_RoleId;
            unitOfWork.User.Update(employee);
            unitOfWork.Save();
            return RedirectToAction("ViewUsers");
        }


        public JsonResult BindMgrList(string roleID)
        {
            if (roleID != "")
            {
                var mgrList = unitOfWork.User.getMgrList(int.Parse(roleID)).ToList();
                return Json(mgrList, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReviewerList()
        {
            ReviewUtitity reviewUtility = new ReviewUtitity();
            var reviewerList = reviewUtility.GetReviewerList();

            return Json(reviewerList, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Logout()
        {
            // logout the user.
            FormsAuthentication.SignOut();

            Session["DisplayName"] = null;
            Session.Abandon();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult AddClientList(int id, string TeamLeadId = null, int ClientMappingId = 0)
        {

            string empCd = User.Identity.Name;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            DateTime usDate = SystemClock.US_Date;
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName };
            //get the reporting history for user.
            var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role, "REC")
                                   .Select(s => s.RR_EmpCD).ToList();

            //get the employee jobdiva name
            string JobDivaUserName = unitOfWork.User.Get().Where(s => s.RE_EmpId == id).FirstOrDefault().RE_Jobdiva_User_Name;
            string getEmployeeId = unitOfWork.User.Get().Where(s => s.RE_EmpId == id).FirstOrDefault().RE_Emp_Cd;
            //get the team lead list
            var ReportingToList = unitOfWork.User.getAllUsers()
                     .Where(s => roleList.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name)
                            && reportingHistory.Contains(s.RE_Emp_Cd))
                     .Select(s => new SelectListItem
                     {
                         Text = s.RE_Jobdiva_User_Name,
                         Value = s.RE_Emp_Cd
                     }).OrderBy(o => o.Text).ToList();
            ReportingToList.Add(new SelectListItem() { Text = JobDivaUserName, Value = getEmployeeId });

            ViewBag.MgrList = ReportingToList.OrderBy(o=>o.Value).ToList();
            ViewBag.JobUserName = JobDivaUserName;
            //if the ClientMappingId is greater than zero then it will delete client mapping id betwwen team lead and account manager
            if (ClientMappingId != 0)
            {
                var clientResult = unitOfWork.RIC_ClientMapping.GetByID(ClientMappingId);
                unitOfWork.RIC_ClientMapping.Delete(clientResult);
                unitOfWork.Save();

            }

            //get all the client mappind id's based on account manager and team lead
            List<RIC_ClientMapping> ListClientId = (from s in unitOfWork.RIC_ClientMapping.Get()
                                                    where s.RCM_EmpId == id && (s.RCM_TeamLeadId.ToString() == TeamLeadId || TeamLeadId == null)
                                                    select new RIC_ClientMapping()
                                                    {
                                                        RCM_ClientId = s.RCM_ClientId

                                                    }).ToList();


            int[] ClientId = new int[200];
            //collect all client mapping id's based on account manager and team lead
            foreach (var Element in ListClientId)
            {
                ClientId = ClientId.Concat(new int[] { Element.RCM_ClientId }).ToArray();


            }
            //it will collect all companies which ever not assigned to team lead and account manager
            List<SelectListItem> _lstClient = new List<SelectListItem>();
            //it will collect all companies which ever assigned to team lead and account manager
            List<SelectListItem> _lstClientSelected = new List<SelectListItem>();

            if (TeamLeadId != null)
            {
                //here you will get client companies 
                var clientList = unitOfWork.User.getClientList();
                foreach (var rule in clientList)// add the items in select list.
                {
                    int count = 0;
                    if (ClientId.Length != 0)
                    {

                        for (int i = 0; i < ClientId.Length; i++)
                        {
                            if (ClientId[i] == Convert.ToInt32(rule.Value))
                            {
                                count++;
                                SelectListItem selectlistitem = new SelectListItem();
                                selectlistitem.Text = rule.Text;
                                selectlistitem.Value = rule.Value.ToString();
                                _lstClientSelected.Add(selectlistitem);
                            }
                        }
                        if (count == 0)
                        {
                            SelectListItem selectlistitem = new SelectListItem();
                            selectlistitem.Text = rule.Text;
                            selectlistitem.Value = rule.Value.ToString();
                            _lstClient.Add(selectlistitem);
                        }


                    }
                    else
                    {
                        SelectListItem selectListItem = new SelectListItem();
                        selectListItem.Text = rule.Text;
                        selectListItem.Value = rule.Value.ToString();
                        _lstClient.Add(selectListItem);

                    }
                }
            }
            else
            {
                _lstClient= unitOfWork.User.getClientList().ToList();
            }

            //it will collect all the information about company's has assigned to team lead and account manager
            var result = (from s in unitOfWork.RIC_ClientMapping.Get()
                          join client in unitOfWork.RIC_Client.Get() on s.RCM_ClientId equals client.RC_Id
                          join emp in unitOfWork.User.Get() on s.RCM_EmpId equals emp.RE_EmpId into employee
                          from AccountManagerName in employee.DefaultIfEmpty()
                          join mgr in unitOfWork.User.Get() on s.RCM_TeamLeadId.ToString() equals mgr.RE_Emp_Cd.ToString() into manager
                          from ManagerName in manager.DefaultIfEmpty()
                          where s.RCM_EmpId == id && (s.RCM_TeamLeadId.ToString() == TeamLeadId || TeamLeadId == null || TeamLeadId == "")
                                                    select new ClientMapping()
                                                    {
                                                        ClinetName=client.RC_ClientName,
                                                        AccountManagerName=AccountManagerName.RE_Employee_Name,
                                                        TeamLeadName=ManagerName.RE_Employee_Name,
                                                        EmpId=id,
                                                        RCM_ClientId = s.RCM_Id,
                                                        RCM_StartDate = s.RCM_StartDate,
                                                        RCM_EndDate = s.RCM_EndDate,
                                                    }).ToList();
            ViewBag.ClientList = _lstClient.OrderBy(c => c.Text).ToList();
            ViewBag.ClientList2 = _lstClientSelected.OrderBy(c => c.Text).ToList();

            if (ClientMappingId > 0)
            {
                ViewBag.Message = string.Format("Account Deleted Succcessfully");
            }

            ViewUserBack();

            return View(result);
        }
        [HttpPost]
        public ActionResult AddClientList(ClientMapping client, int id, string AccountId=null,string TeamLeadId=null)
        {
            string empCd = User.Identity.Name;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            DateTime usDate = SystemClock.US_Date;
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName };
            //get the reporting history for user.
            var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role, "REC")
                                   .Select(s => s.RR_EmpCD).ToList();

            //get the employee jobdiva name
            string JobDivaUserName = unitOfWork.User.Get().Where(s => s.RE_EmpId == id).FirstOrDefault().RE_Jobdiva_User_Name;
            string getEmployeeId = unitOfWork.User.Get().Where(s => s.RE_EmpId == id).FirstOrDefault().RE_Emp_Cd;

            //get the team lead list
            var ReportingToList = unitOfWork.User.getAllUsers()
                     .Where(s => roleList.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name)
                            && reportingHistory.Contains(s.RE_Emp_Cd))
                     .Select(s => new SelectListItem
                     {
                         Text = s.RE_Jobdiva_User_Name,
                         Value = s.RE_Emp_Cd
                     }).OrderBy(o => o.Text).ToList();
            ReportingToList.Add(new SelectListItem() { Text = JobDivaUserName, Value = getEmployeeId });

            ViewBag.MgrList = ReportingToList.OrderBy(o=>o.Text).ToList();
            //get the employee jobdiva name
            ViewBag.JobUserName = JobDivaUserName;
                //get all the client mappind id's between the startdate and enddate.
            int[] allGetClientId = unitOfWork.RIC_ClientMapping.Get().
                Where(c => c.RCM_EmpId == id && c.RCM_TeamLeadId == TeamLeadId && (c.RCM_EndDate == null?client.RCM_StartDate.Date:c.RCM_EndDate.Value.Date)>= client.RCM_StartDate.Date).Select(c=>c.RCM_ClientId).ToArray();
            
            //get only client mapping id's which ever not existed in database.
            int[] getClientId = (AccountId.Split(',').Select(n => Convert.ToInt32(n)).ToArray()).Except(allGetClientId).ToArray();


            List<RIC_ClientMapping> ClientData = new List<RIC_ClientMapping>();


            //insert all new client mappings with teamlead and account manager.
            for (int i = 0; i < getClientId.Length; i++)
            {

                var list = new RIC_ClientMapping
                {
                    RCM_ClientId = getClientId[i],
                    RCM_EmpId = id,
                    RCM_CreatedBy = "Admin",
                    RCM_CreatedDate = DateTime.Now,
                    RCM_TeamLeadId=TeamLeadId,
                    RCM_StartDate=client.RCM_StartDate,
                    RCM_EndDate = client.RCM_EndDate,
                    RCM_UpdatedBy = null,
                    RCM_UpdatedDate = null
                };
                unitOfWork.RIC_ClientMapping.Insert(list);
                unitOfWork.Save();
            }



            //get all the client mappind id's based on account manager and team lead
            List<RIC_ClientMapping> ListClientId = (from s in unitOfWork.RIC_ClientMapping.Get()
                                                    where s.RCM_EmpId == id && (s.RCM_TeamLeadId == TeamLeadId || TeamLeadId == null)
                                                    select new RIC_ClientMapping()
                                                    {
                                                        RCM_ClientId = s.RCM_ClientId

                                                    }).ToList();
            int[] ClientId = new int[150];
            //collect all client mapping id's based on account manager and team lead
            foreach (var Element in ListClientId)
            {
                ClientId = ClientId.Concat(new int[] { Element.RCM_ClientId }).ToArray();

                        
            }

            //it will collect all companies which ever not assigned to team lead and account manager
            List<SelectListItem> _lstClient = new List<SelectListItem>();

            //it will collect all companies which ever assigned to team lead and account manager
            List<SelectListItem> _lstClientSelected = new List<SelectListItem>();

            //here you will get client companies 
            var clientList = unitOfWork.User.getClientList();

            foreach (var rule in clientList)// add the items in select list.
            {
                int count = 0;
                if (ClientId.Length != 0)
                {

                    for (int i = 0; i < ClientId.Length; i++)
                    {
                        if (ClientId[i] == Convert.ToInt32(rule.Value))
                        {
                            count++;
                            SelectListItem selectlistitem = new SelectListItem();
                            selectlistitem.Text = rule.Text;
                            selectlistitem.Value = rule.Value.ToString();
                            _lstClientSelected.Add(selectlistitem);
                        }
                    }
                    if (count == 0)
                    {
                        SelectListItem selectlistitem = new SelectListItem();
                        selectlistitem.Text = rule.Text;
                        selectlistitem.Value = rule.Value.ToString();
                        _lstClient.Add(selectlistitem);
                    }


                }
                else
                {
                    SelectListItem selectListItem = new SelectListItem();
                    selectListItem.Text = rule.Text;
                    selectListItem.Value = rule.Value.ToString();
                    _lstClient.Add(selectListItem);

                }
            }

            //it will collect all the information about company's has assigned to team lead and account manager
            var result = (from s in unitOfWork.RIC_ClientMapping.Get()
                          join clientData in unitOfWork.RIC_Client.Get() on s.RCM_ClientId equals clientData.RC_Id
                          join emp in unitOfWork.User.Get() on s.RCM_EmpId equals emp.RE_EmpId into employee
                          from AccountManagerName in employee.DefaultIfEmpty()
                          join mgr in unitOfWork.User.Get() on s.RCM_TeamLeadId.ToString() equals mgr.RE_Emp_Cd.ToString() into manager
                          from ManagerName in manager.DefaultIfEmpty()
                          where s.RCM_EmpId == id && (s.RCM_TeamLeadId.ToString() == TeamLeadId || TeamLeadId == null)
                          select new ClientMapping()
                          {
                              ClinetName = clientData.RC_ClientName,
                              AccountManagerName = AccountManagerName.RE_Employee_Name,
                              TeamLeadName = ManagerName.RE_Employee_Name,
                              EmpId = id,
                              RCM_ClientId = s.RCM_Id,
                              RCM_StartDate = s.RCM_StartDate,
                              RCM_EndDate = s.RCM_EndDate,

                          }).ToList();
            ViewUserBack();
            ViewBag.ClientList = _lstClient.OrderBy(c => c.Text).ToList();
            ViewBag.ClientList2 = _lstClientSelected.OrderBy(c => c.Text).ToList();

            
            if (getClientId.Count() > 0)
            {
                //if new client is mapping to team lead and account manager
                ViewBag.Message = string.Format("Client Details Saved Succcessfully");
            }
            else
            {
                ViewBag.Message = string.Format("AM TL Mapping already exists between start date and end date");
            }

            return View(result);
        }


        //[HttpGet]
        [Authorize(Roles = "Admin,Manager,Team Lead,Director,HR Manager,Director-Staffing")]
        public ActionResult GetTargetList()
        {
            string empCd = User.Identity.Name;
            var _user = unitOfWork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;

            List<TargetSubIntHire> TargetList = unitOfWork.User.Get_TargetSubIntHire(empCd, role).Select(s => new TargetSubIntHire
            {
                RE_Jobdiva_User_Name = s.RE_Jobdiva_User_Name,
                RE_Emp_Cd = s.RE_Emp_Cd,
                Designation = s.Designation,
                RE_Resign_Date = s.RE_Resign_Date,
                RE_Joining_Date = s.RE_Joining_Date,
                RS_MonthlySubmissions = s.RS_MonthlySubmissions,
                RS_Monthly_Interviews = s.RS_Monthly_Interviews,
                RS_Monthyl_Hire = s.RS_Monthyl_Hire,
                EffectiveFromDate = s.EffectiveFromDate,
                EffectiveToDate = s.EffectiveToDate
            }).ToList();

            return View(TargetList);

        }

        [NonAction]
        public string GetDateDif(DateTime fromDate, DateTime toDate)
        {
            // DateTime Birth = new DateTime(1954, 7, 30);
            //DateTime Today = DateTime.Now;

            TimeSpan Span = toDate - fromDate;

            DateTime duration = DateTime.MinValue + Span;

            // note: MinValue is 1/1/1 so we have to subtract...
            return (duration.Year - 1).ToString() + " Y " + (duration.Month - 1).ToString() + " M ";
        }

        public ActionResult PopulateTargets(string ReportEmpCd, string Month,int Year)
        {
            AddTargetsVM model = new AddTargetsVM();
            string empCd = User.Identity.Name;
            //get the date.
            DateTime usDate = SystemClock.US_Date;
          

            bool allowEdit = true; //usDate.Year == model.Year&& DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month) == model.Month;
            

                //get the target for the team.
            var targets = unitOfWork.User.GetTargetForTeam(ReportEmpCd, Month, Year);
            var ClientList = new List<AddTargetEmployeeList>();
                //bind the employee list.

                if (allowEdit)
                {
                    //get the current reporting history with target.
                    ClientList = (from rh in unitOfWork.User.getReportingHistory(empCd, usDate, usDate, "Admin")
                                            .Where(w => w.RR_MgrCD == ReportEmpCd || w.RR_EmpCD == ReportEmpCd)
                                          join emp in unitOfWork.User.Get() on rh.RR_EmpCD equals emp.RE_Emp_Cd
                                          join target in targets on rh.RR_EmpCD equals target.RT_Emp_Cd into lt
                                          from target in lt.DefaultIfEmpty()
                                          select new AddTargetEmployeeList
                                                {
                                                    EmpCd = rh.RR_EmpCD ,                                                  
                                                    SubmissionTarget = target == null ? (float?)null : target.RT_SubmissionsTarget,
                                                    InterviewTarget = target == null ? (float?)null : target.RT_InterviewsTarget,
                                                    HiresTarget = target == null ? (float?)null : target.RT_HiresTarget,
                                                    Comments = target == null ? null : target.RT_Comments                                                  
                                                }).OrderBy(o => o.EmployeeName).ToList();
                }
                else
                {
                    //get the data from tragt table 
                    ClientList = (from target in targets
                                          join emp in unitOfWork.User.Get() on target.RT_Emp_Cd equals emp.RE_Emp_Cd
                                          join mgr in unitOfWork.User.Get() on target.RT_MgrCd equals mgr.RE_Emp_Cd
                                          select new AddTargetEmployeeList
                                          {
                                             
                                              EmpCd = target.RT_Emp_Cd,                                            
                                              SubmissionTarget = target == null ? (float?)null : target.RT_SubmissionsTarget,
                                              InterviewTarget = target == null ? (float?)null : target.RT_InterviewsTarget,
                                              HiresTarget = target == null ? (float?)null : target.RT_HiresTarget,
                                              Comments = target == null ? null : target.RT_Comments
                                          }).OrderBy(o => o.EmployeeName).ToList();

                }
                return Json(ClientList, JsonRequestBehavior.AllowGet);

            
        }

        //Added by Dinesh for adding Clients

        [HttpGet]
        public ActionResult ManageClientList(string RC_Id)
        {
            ViewBag.RC_Id = RC_Id;
            UnitOfWork unitOfWork = new UnitOfWork();
            List<RIC_Client> getClient = new List<RIC_Client>();
            getClient = unitOfWork.Client.getClientList();

            return PartialView("ManageClientList", getClient);
        }


        public ActionResult ManageClientListPost(string Clientname)
        {
            List<RIC_Client> getClient = new List<RIC_Client>();
            getClient = unitOfWork.Client.getClientList();
            getClient = getClient.Where(x => x.RC_ClientName.Trim().ToLower() == Clientname.Trim().ToLower()).ToList();
            if (string.IsNullOrEmpty(Clientname) || string.IsNullOrWhiteSpace(Clientname))
            {
                ViewBag.message = "Client name is required";
                ModelState.AddModelError("Clientname", "Clientname is required");
            }
            else if (getClient.Count() > 0)
            {
                ViewBag.message = "Client name already exist";
                ModelState.AddModelError("Clientname", "Clientname already exist");
            }
            if (ModelState.IsValid)

            {
                UnitOfWork unitOfWork = new UnitOfWork();

                var rIC_Client = new RIC_Client();
                rIC_Client.RC_ClientName = Clientname;
                rIC_Client.RC_CreatedBy = "Admin";
                rIC_Client.RC_CreatedDate = DateTime.Now.Date;

                unitOfWork.Client.Insert(rIC_Client);

                unitOfWork.Save();
                ViewBag.message = "Client added successfully!";
            }
            getClient = unitOfWork.Client.getClientList();

            var getClientlist = getClient.OrderByDescending(s => s.RC_Id);
            // ViewBag.clientlist = getClientlist;
            //return RedirectToAction("ManageClientList", "Account", new { RC_Id = "Admin" });
            return PartialView("ManageClientList", getClientlist);
            //return Json(getClientlist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageClientDeleteRecord(int RC_ID)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            List<RIC_Client> getClient = new List<RIC_Client>();
            unitOfWork.Client.DeleteRecord(RC_ID);
            getClient = unitOfWork.Client.getClientList();
            return PartialView("ManageClientList", getClient);
        }
        public ActionResult ManageClientUpdateRecord(string clientname, int RC_Id)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            List<RIC_Client> getClient = new List<RIC_Client>();
            unitOfWork.Client.UpdateRecord(clientname, RC_Id);
            getClient = unitOfWork.Client.getClientList();
            return PartialView("ManageClientList", getClient);
        }
         

    }
}