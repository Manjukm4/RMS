using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBLibrary;
using DBLibrary.UnitOfWork;
using RIC.Models.Review;
using PagedList;
using RIC.Utility;
using System.Net.Mail;
using System.Net;
using RIC.Models.Client;
using OfficeOpenXml;
using System.Data;
using RIC.CustomHelper;
using System.Data.SqlClient;

namespace RIC.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        UnitOfWork unitOfwork = new UnitOfWork();
        string mgrRoleName = System.Configuration.ConfigurationManager.AppSettings["ManagerRole"];
        string tlRoleName = System.Configuration.ConfigurationManager.AppSettings["TLRole"];
        string empRoleName = System.Configuration.ConfigurationManager.AppSettings["EmployeeRole"];
        string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        int directorRoleID = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DirectorID"]);
        int adminRoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AdminRoleID"]);
        string AdminRoleName = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
        string HrRoleName = System.Configuration.ConfigurationManager.AppSettings["HRRole"];
        string devRoleName = System.Configuration.ConfigurationManager.AppSettings["DEVRole"];
        string devLeadRole = System.Configuration.ConfigurationManager.AppSettings["DEV_LeadRole"];
        string stafingDirectorRole = System.Configuration.ConfigurationManager.AppSettings["StaffingDirector"];
        string Reviewdate = System.Configuration.ConfigurationManager.AppSettings["ReviewDate"];
        static string EmailTo = System.Configuration.ConfigurationManager.AppSettings["EmailTo"];
        DateTime usDate = SystemClock.US_Date;

        [HttpGet]
        public ActionResult AnnualFeedbackDashboard()
        {
            //get the user id from authentication
            string empCd = User.Identity.Name;

            //get the review requested notification.
            Session["AnnualReviewRequest"] = unitOfwork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen == false
                                            && s.RN_NotificationType == "AnnualReviewRequest").Count();
            var reviewUpdate = unitOfwork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen == false
                                            && s.RN_NotificationType == "ReviewUpdate").ToList();
            Session["ReviewUpdate"] = reviewUpdate.Count();
            //set the notification to seen
            reviewUpdate.ForEach(m => m.RN_IsSeen = true);
            unitOfwork.Save();
            //set the session values
            Session["ReviewUpdate"] = 0;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var UserRole = r.GetRoles()[0];
            var dirRoles = new List<string> { directorRoleName, HrRoleName, AdminRoleName };
            var userRoles = new List<string> { empRoleName, devRoleName, tlRoleName, mgrRoleName, devLeadRole };
            //get the user list based on reporting history.//// updated by ashish 08-02-2019
            var userList = (from user in unitOfwork.User.Get()
                            join rh in unitOfwork.User.getReportingHistory(empCd, usDate.Date, usDate, UserRole)
                       on user.RE_Emp_Cd equals rh.RR_EmpCD
                            select new
                            {
                                EmpCd = user.RE_Emp_Cd,
                                EmployeeName = user.RE_Jobdiva_User_Name,
                                FormID = user.RE_AnnualFeedBackFormID,
                                MgrCd = rh.RR_MgrCD,
                                ReportingTo = rh.Mgr_Name,
                                RoleName = user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name
                            }).ToList();
            // get the user data from entity.
            var model = (from user in userList
                             // .Where(w => w.RE_Resign_Date == null)                     
                         join annualFeedback in unitOfwork.RIC_AnnualFeedback.Get(s => s.RA_Date.Year == usDate.Year)
                         on user.EmpCd equals annualFeedback.RA_EmployeeId into u
                         from Feedback in u.DefaultIfEmpty()
                             // where userRoles.Contains(user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name) &&(dirRoles.Contains(UserRole) || user.MgrCD == empCd)
                         select new AnnualFeedbackDashboardVM
                         {
                             EmployeeID = user.EmpCd,
                             EmployeeName = user.EmployeeName,
                             FormID = user.FormID.HasValue ? user.FormID.Value : 0,
                             ReportingTo = user.ReportingTo,
                             LeadReviewerId = user.MgrCd,
                             LastReviewDate = Feedback == null ? (DateTime?)null : Feedback.RA_Date,
                             UserRole = user.RoleName,
                             ShowCheckBox = Feedback != null ? (Feedback.RA_Date.Year != usDate.Year) : true,
                             Status = Feedback != null ? Feedback.RA_Status : "Not Released",
                             HrName = Feedback != null ? unitOfwork.User.GetByEmpID(Feedback.RA_HrId).RE_Jobdiva_User_Name : null,
                             HrReviewStatus = Feedback != null ? Feedback.RA_Hr_ReviewStatus == "Completed" : false,
                             //get the pending reviewers.
                             reviewers = Feedback != null ? (from reviewTbl in Feedback.RIC_AnnualFeedbacReviewer
                                                             join uTbl in unitOfwork.User.Get()
                                                             on reviewTbl.RR_ReviewerID equals uTbl.RE_Emp_Cd
                                                             where reviewTbl.RR_Status == "InProgress"
                                                             select new { uTbl.RE_Jobdiva_User_Name })
                                                             .Select(s => s.RE_Jobdiva_User_Name)
                                                             .ToList() : null,
                             LastUpdatedDate = Feedback != null ? Feedback.RA_UpdatedDate : null,
                             NotificationCount = Feedback != null ? reviewUpdate.Count(rc => rc.Rn_ReviewID == Feedback.RA_ReviewId) : 0
                         }).OrderByDescending(o => o.LastUpdatedDate).ThenByDescending(t => t.LastReviewDate)
                        .GroupBy(g => g.EmployeeID).Select(s => new AnnualFeedbackDashboardVM
                        {
                            EmployeeID = s.Key,
                            EmployeeName = s.FirstOrDefault().EmployeeName,
                            FormID = s.FirstOrDefault().FormID,
                            LeadReviewerId = s.FirstOrDefault().LeadReviewerId,
                            ReportingTo = s.FirstOrDefault().ReportingTo,
                            LastReviewDate = s.FirstOrDefault().LastReviewDate,
                            UserRole = s.FirstOrDefault().UserRole,
                            ShowCheckBox = s.FirstOrDefault().ShowCheckBox,
                            Status = s.FirstOrDefault().Status,
                            reviewers = s.FirstOrDefault().reviewers,
                            NotificationCount = s.FirstOrDefault().NotificationCount,
                            HrName = s.FirstOrDefault().HrName,
                            HrReviewStatus = s.FirstOrDefault().HrReviewStatus
                        });
            // show the request for review button..
            ViewBag.ShowRequestBtn = UserRole == HrRoleName ? true : false;
            return View(model);
        }

        [HttpPost]
        public ActionResult AnnualFeedbackDashboard(List<AnnualFeedbackDashboardPost> ReviewList)
        {


            string UserID = User.Identity.Name;
            // add the review request in context
            foreach (var userReview in ReviewList)
            {
                var review = new RIC_AnnualFeedback
                {
                    RA_FormId = userReview.FormID,
                    RA_EmployeeId = userReview.EmpID,
                    RA_LeadReviewerId = userReview.LeadReviewerID,
                    RA_HrId = UserID,
                    RA_Status = "Released",
                    RA_Hr_ReviewStatus = "Released",
                    RA_Date = usDate,
                    // add the reviewer list..
                    RIC_AnnualFeedbacReviewer = userReview.ReviewerList
                    .Where(ar => ar != userReview.LeadReviewerID).Distinct().Select(s => new RIC_AnnualFeedbackReviewers
                    {
                        RR_ReviewerID = s,
                        RR_Status = "InProgress"
                    }).ToList()
                };
                //add the team lead if if reviewer id is not null.
                if (userReview.LeadReviewerID != null)
                    review.RIC_AnnualFeedbacReviewer.Add(new RIC_AnnualFeedbackReviewers()
                    {
                        RR_ReviewerID = userReview.LeadReviewerID,
                        RR_Status = "InProgress"
                    });
                review.RA_UpdatedBy = unitOfwork.User.GetByEmpID(UserID).RE_AltJobdiva_User_Name;
                review.RA_UpdatedDate = usDate;
                unitOfwork.RIC_AnnualFeedback.Insert(review);
                unitOfwork.Save();
                //add the notification for TL..
                unitOfwork.Notification.Insert(new RIC_Notification
                {
                    RN_EmpCd = review.RA_LeadReviewerId,
                    Rn_ReviewID = review.RA_ReviewId,
                    RN_NotificationText = "Form Released For User" + review.RA_EmployeeId,
                    RN_IsSeen = false,
                    Date = usDate,
                    RN_NotificationType = "AnnualReviewRequest"
                });
                //add notification for reviewers.
                foreach (var reviewer in userReview.ReviewerList.Distinct())
                {
                    unitOfwork.Notification.Insert(new RIC_Notification
                    {
                        RN_EmpCd = reviewer,
                        Rn_ReviewID = review.RA_ReviewId,
                        RN_NotificationText = "Form Released For User" + review.RA_EmployeeId,
                        RN_IsSeen = false,
                        Date = usDate,
                        RN_NotificationType = "AnnualReviewRequest"
                    });

                }


                //add the notification for HR..
                unitOfwork.Notification.Insert(new RIC_Notification
                {
                    RN_EmpCd = UserID,
                    Rn_ReviewID = review.RA_ReviewId,
                    RN_NotificationText = "Form Released For User " + review.RA_EmployeeId,
                    RN_IsSeen = false,
                    Date = usDate,
                    RN_NotificationType = "AnnualReviewRequest"
                });

                var GetEmployeeName = (from e in unitOfwork.RIC_Employee.Get()
                                       where e.RE_Emp_Cd == review.RA_EmployeeId
                                       select new
                                       {

                                           EmployeeName = e.RE_Employee_Name
                                       }).ToList();

                var GetHrName = (from e in unitOfwork.RIC_Employee.Get()
                                 where e.RE_Emp_Cd == review.RA_HrId
                                 select new
                                 {

                                     HrName = e.RE_Employee_Name
                                 }).ToList();

                var GetLeadName = (from e in unitOfwork.RIC_Employee.Get()
                                   where e.RE_Emp_Cd == review.RA_LeadReviewerId
                                   select new
                                   {

                                       LeadName = e.RE_Employee_Name
                                   }).ToList();

                var GetReviewer = (from e in unitOfwork.RIC_Employee.Get()
                                   where e.RE_Emp_Cd == review.RIC_AnnualFeedbacReviewer.FirstOrDefault().RR_ReviewerID
                                   select new
                                   {

                                       ReviewerName = e.RE_Employee_Name
                                   }).ToList();

                string error;
                string ReviewDetails = @"<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='viewport' content='width=device-width, initial-scale=1'><meta name='ProgId' content='Word.Document'></head><body style='font-family: Arial; font-size: 12px;'>
<div style='font-size: 14px; width: 50%; margin-left: 25%;box-sizing: unset;border-radius: 7px;border: 1px solid #c5c5c5!important;box-shadow: 3px 3px 10px #bfbfbf;margin-top: 5%;padding: 1%;'>
<h3>Annual Review Details</h3>
<span style='font-weight:bold'>Employee Name: </span><span>" + GetEmployeeName.FirstOrDefault().EmployeeName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Reporting To : </span><span>" + GetLeadName.FirstOrDefault().LeadName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Review Hr : </span><span>" + GetHrName.FirstOrDefault().HrName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Reviewer Name: </span><span>" + GetReviewer.FirstOrDefault().ReviewerName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Review Date: </span><span style='text-align:left'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</span><br/><br/><p><span style='font-weight:bold'>Note:</span>This is an automated email message. Please do not reply to this email.<p></div></body></html>";

                Email.SendMail1(GetEmail(), "Annual Review Details", ReviewDetails, out error);


                unitOfwork.Save();
            }
            return Json(new { success = true, responseText = "Review Request successfuly sent!" }, JsonRequestBehavior.AllowGet);
        }
        #region Annual Feedback
        [HttpGet]
        public ActionResult SubmittedAnnualFeedback(int? page, string EmployeeName)
        {
            int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            //set the notification text.
            Session["ReviewUpdate"] = unitOfwork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen == false
                                                       && s.RN_NotificationType == "ReviewUpdate").Count();
            //get the review requested notification.
            Session["AnnualReviewRequest"] = unitOfwork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen == false
                                            && s.RN_NotificationType == "AnnualReviewRequest").Count();
            // get the list of submitted feedback.
            IEnumerable<SubmittedAnnualFeedbackVM> submetted =
                            (from review in unitOfwork.RIC_AnnualFeedbackReviewers.Get(s => s.RR_ReviewerID == empCd && s.RR_Status == "Completed")
                             join employee in unitOfwork.User.Get()
                             on review.RIC_AnnualFeedback.RA_EmployeeId equals employee.RE_Emp_Cd
                             join hr in unitOfwork.User.Get()
                             on review.RIC_AnnualFeedback.RA_HrId equals hr.RE_Emp_Cd
                             select new SubmittedAnnualFeedbackVM
                             {
                                 ReviewID = review.RR_ReviewID,
                                 EmployeeName = employee.RE_Jobdiva_User_Name,
                                 IssuedBy = hr.RE_Jobdiva_User_Name,
                                 ReviewDate = review.RIC_AnnualFeedback.RA_Date
                             }).Where(s => (EmployeeName == null || EmployeeName == "" || s.EmployeeName.ToLower().Contains(EmployeeName.ToLower())))
                             .OrderByDescending(o => o.ReviewDate).ToPagedList(pageIndex, 10);
            //  ViewBag.ReturnUrl=
            return View(submetted);
        }

        [HttpGet]
        public ActionResult AnnualFeedbackDetails(int reviewID, string returnUrl)
        {
            //bind the annual feedback model.
            AnnualFeedbackDetailsVM model = new AnnualFeedbackDetailsVM();
            var review = unitOfwork.RIC_AnnualFeedback.GetByID(reviewID);
            var employee = unitOfwork.User.GetByEmpID(review.RA_EmployeeId);
            // get employee id from from authentication. 
               string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            // get the role for user.
            RolePrincipal rolePrincipal = (RolePrincipal)User;
            var role = rolePrincipal.GetRoles()[0];
            // set the flag to show all users.
            bool showAllUser = (role == HrRoleName || role == directorRoleName || role == stafingDirectorRole) && review.RA_Status == "Completed";
            // bool showAllUser = (role == HrRoleName || role == directorRoleName);
            // bind the user details to model.
            model.ReviewID = reviewID;
            model.EmployeeID = employee.RE_Emp_Cd;
            model.EmployeeName = employee.RE_Jobdiva_User_Name;
            model.JoiningDate = employee.RE_Joining_Date;
            model.Designation = employee.RMS_Designation != null ? employee.RMS_Designation.RD_Designation : null;
            model.ReviewDate = review.RA_Date;
            model.Hr_LeaveTakenOnLop = review.RA_Hr_LeavesTakenOnLop;
            model.Hr_LeaveTaken = review.RA_Hr_LeaveHistory;
            model.Hr_Warnings = review.RA_Hr_Warnings;

            model.Hr_CompanyValue = review.RA_Hr_LoyaltyToCompanyValues;
            model.Hr_CultValue = review.RA_Hr_BehavingToCulturalValue;
            model.Hr_CultPeople = review.RA_Hr_RespectsDiffInCulturalValue;
            model.AverageRating = review.RA_AvgRating;
            model.AdditionalComments = new List<AdditionalComments>();
            model.ImprovementsRecommended = new List<ImprovementsRecommended>();
            string UserName = unitOfwork.User.GetByEmpID(review.RA_HrId).RE_Jobdiva_User_Name;

            if (showAllUser)
            {
                // Add HR comments.
                model.AdditionalComments.Add(new AdditionalComments { ReviewerName = UserName, Comments = review.RA_Hr_AdditionalComments });
                model.ImprovementsRecommended.Add(new ImprovementsRecommended { ReviewerName = UserName, Comments = review.RA_Hr_ImprovementsRecommended });

                //add the additional comments for reviewer
                foreach (var reviewer in review.RIC_AnnualFeedbacReviewer)
                {
                    //get the user Name
                    UserName = unitOfwork.User.GetByEmpID(reviewer.RR_ReviewerID).RE_Jobdiva_User_Name;
                    //add the comments in list
                    model.AdditionalComments.Add(new AdditionalComments { ReviewerName = UserName, Comments = reviewer.RR_AdditionalComments });
                    model.ImprovementsRecommended.Add(new ImprovementsRecommended { ReviewerName = UserName, Comments = reviewer.RR_ImprovementsRecommended });
                }
            }
            else if (role == HrRoleName)
            {
                // Add HR comments.
                model.AdditionalComments.Add(new AdditionalComments { ReviewerName = UserName, Comments = review.RA_Hr_AdditionalComments });
                model.ImprovementsRecommended.Add(new ImprovementsRecommended { ReviewerName = UserName, Comments = review.RA_Hr_ImprovementsRecommended });
            }
            else if (role == stafingDirectorRole)
            {
                //UserName = review.;
                //var reviewer = review.RIC_AnnualFeedbacReviewer.FirstOrDefault(s => s.RR_Status == "Completed");
                //UserName = reviewer.RIC_AnnualFeedback.RA_UpdatedBy.ToString();
                ////add the comments in list

                foreach (var reviewer1 in review.RIC_AnnualFeedbacReviewer)
                {
                    //get the user Name
                    UserName = unitOfwork.User.GetByEmpID(reviewer1.RR_ReviewerID).RE_Jobdiva_User_Name;
                    //add the comments in list
                    model.AdditionalComments.Add(new AdditionalComments { ReviewerName = UserName, Comments = reviewer1.RR_AdditionalComments });
                    model.ImprovementsRecommended.Add(new ImprovementsRecommended { ReviewerName = UserName, Comments = reviewer1.RR_ImprovementsRecommended });
                }
            }

            else
            {
                UserName = unitOfwork.User.GetByEmpID(empCd).RE_Jobdiva_User_Name;
                var reviewer = review.RIC_AnnualFeedbacReviewer.FirstOrDefault(s => s.RR_ReviewerID == empCd);

                //add the comments in list
                model.AdditionalComments.Add(new AdditionalComments { ReviewerName = UserName, Comments = reviewer.RR_AdditionalComments });
                model.ImprovementsRecommended.Add(new ImprovementsRecommended { ReviewerName = UserName, Comments = reviewer.RR_ImprovementsRecommended });

            }
            // get the attribute list 
            model.AttributHeaders = (from reviewer in review.RIC_AnnualFeedbacReviewer
                                     join details in unitOfwork.RIC_AnnualFeedbackDtl.Get()
                                     on reviewer.RR_ID equals details.RA_ReviewID
                                     join reviewerName in unitOfwork.User.Get()
                                     on reviewer.RR_ReviewerID equals reviewerName.RE_Emp_Cd
                                     join fields in unitOfwork.RIC_AnnualFeedbackFields.Get()
                                     on details.RA_FieldID equals fields.AF_FieldId
                                     join header in unitOfwork.RIC_AnnualFeedbackFields.Get()
                                     on fields.AF_Pid equals header.AF_FieldId
                                     where showAllUser || role == stafingDirectorRole || reviewer.RR_ReviewerID == empCd
                                     select new
                                     {
                                         ReviewerID = reviewer.RR_ReviewerID,
                                         ReviewerName = reviewerName.RE_Jobdiva_User_Name,
                                         AdditionalComments = reviewer.RR_AdditionalComments,
                                         ImprovementsRecommended = reviewer.RR_ImprovementsRecommended,
                                         Status = reviewer.RR_Status,
                                         FieldID = details.RA_FieldID,
                                         HeaderName = header.AF_FieldName,
                                         FieldName = fields.AF_FieldName,
                                         ReviewRating = details.RA_ReviewRating,
                                         ReviewComments = details.RA_ReviewComments,
                                         // ImprovementsRecommended=reviewer.RR_ImprovementsRecommended
                                     }).GroupBy(g => new { g.HeaderName })
                                .Select(header => new AnnualFeedbackDetailsAttributesHeaders
                                {
                                    HeaderName = header.Key.HeaderName,
                                    Attributes = header.GroupBy(attrKey => attrKey.FieldName)
                                    .Select(attr => new AnnualFeedbackDetailsAttributes
                                    {
                                        AttrID = attr.FirstOrDefault().FieldID,
                                        AttrName = attr.Key,
                                        rating = attr.Select(r => new AnnualFeedbackDetailsRating
                                        {
                                            ReviewerName = r.ReviewerName,
                                            Rating = r.ReviewRating,
                                            Comments = r.ReviewComments,

                                        }).ToList()
                                    }).ToList()
                                }).ToList();
            //get the interim feedback for year.
            model.InterimReviewDetails = unitOfwork.RIC_PersonalFeedback
                        .Get(r => r.RP_EmployeeID == review.RA_EmployeeId && r.RP_Status == "Completed" && r.RP_ReviewDate.Year == review.RA_Date.Year)
                        .Select(s => new InterimReviewDetails
                        {
                            InterimReviewID = s.RP_ID,
                            ReviewerName = unitOfwork.User.GetByEmpID(s.RP_ReviewerID).RE_Jobdiva_User_Name,
                            Date = s.RP_ReviewDate,
                            FromDate = s.RP_FromDate,
                            ToDate = s.RP_ToDate,
                        }).ToList();
            //get the submission analaysys last four quarter for user.   
            DashBoardIndexAction dashBoardIndex = new DashBoardIndexAction();
            model.QuarterSubmissionAnalysys = dashBoardIndex.GetSubmissionAnalysis(Convert.ToDateTime(Reviewdate), employee.RE_Emp_Cd);
            //set the return URl
            model.ReturnUrl = returnUrl;

            model.ShowHrReview = role == HrRoleName || role == stafingDirectorRole || ((role == directorRoleName || role == stafingDirectorRole) && review.RA_Status == "Completed");
            model.ShowAvgRating = review.RA_Status == "Completed" && (role == HrRoleName || role == directorRoleName || role == stafingDirectorRole);
            return View(model);
        }

        public ActionResult AnnualFeedback()
        {
            //  IEnumerable<AnnualFeedbackWM> AnnualFeedbackWM;

            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            Session["ReviewUpdate"] = unitOfwork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen == false
                                                       && s.RN_NotificationType == "ReviewUpdate").Count();
            var AnnualReviewRequest = unitOfwork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen == false
                                           && s.RN_NotificationType == "AnnualReviewRequest").ToList();
            //set the notification to seen
            AnnualReviewRequest.ForEach(m => m.RN_IsSeen = true);
            unitOfwork.Save();
            //set the session values
            Session["AnnualReviewRequest"] = 0;

            // get the feedback list.
            if (empCd == "SBS0210")
            {
                RolePrincipal r = (RolePrincipal)User;
                var UserRole = r.GetRoles()[0];
                var userList = (from user in unitOfwork.User.Get()
                                join rh in unitOfwork.User.getReportingHistory(empCd, usDate.Date, usDate, UserRole)
                           on user.RE_Emp_Cd equals rh.RR_EmpCD
                                select new
                                {

                                    EmpCd = user.RE_Emp_Cd,
                                    EmployeeName = user.RE_Jobdiva_User_Name,
                                    FormID = user.RE_AnnualFeedBackFormID,
                                    MgrCd = rh.RR_MgrCD,
                                    ReportingTo = rh.Mgr_Name,
                                    RoleName = user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name
                                }).ToList();
                
                var linq1 = (from reviewList in unitOfwork.RIC_AnnualFeedback.Get((s => s.RIC_AnnualFeedbacReviewer.Count() == 0))
                             join Empoyee in unitOfwork.User.Get()
                             on reviewList.RA_EmployeeId equals Empoyee.RE_Emp_Cd
                             join Hr in unitOfwork.User.Get()
                             on reviewList.RA_HrId equals Hr.RE_Emp_Cd
                             select new AnnualFeedbackWM
                             {
                                 ReviewID = reviewList.RA_ReviewId,
                                 EmployeeName = Empoyee.RE_Jobdiva_User_Name,
                                 IssuedBy = Hr.RE_Jobdiva_User_Name,
                                 ReviewDate = reviewList.RA_Date,
                                 FirstSubmission = true,
                                 HighlightRow = AnnualReviewRequest.Count(ar => ar.Rn_ReviewID == reviewList.RA_ReviewId) > 0
                             }).Union
                 (
              from reviewList in unitOfwork.RIC_AnnualFeedback.Get(s => s.RIC_AnnualFeedbacReviewer.Count() > 0
              && s.RIC_AnnualFeedbacReviewer.FirstOrDefault() != null
              )
              join Empoyee in unitOfwork.User.Get()
              on reviewList.RA_EmployeeId equals Empoyee.RE_Emp_Cd
              join Hr in unitOfwork.User.Get()
              on reviewList.RA_HrId equals Hr.RE_Emp_Cd
              join user in userList
              on reviewList.RA_EmployeeId equals user.EmpCd
              //into y
              //from z in y.DefaultIfEmpty()
              join annualFeedback in unitOfwork.RIC_AnnualFeedback.Get()
               on user.EmpCd equals annualFeedback.RA_EmployeeId into u
              from Feedback in u.DefaultIfEmpty()
              where Empoyee.RE_Resign_Date.ToString() == "" 
              //&&  Empoyee.RE_Jobdiva_User_Name.ToLower().Trim ()== "abdul, ajeej"

              select new AnnualFeedbackWM
              {
                  ReviewID = reviewList.RA_ReviewId,
                  EmployeeName = Empoyee.RE_Jobdiva_User_Name,
                  IssuedBy = Hr.RE_Jobdiva_User_Name,
                  Status = Feedback != null ? Feedback.RA_Status : "Not Released",
                  ReviewDate = reviewList.RA_Date,
                  FirstSubmission = false,
                  HighlightRow = AnnualReviewRequest.Count(ar => ar.Rn_ReviewID == reviewList.RA_ReviewId) > 0,
                  reviewers = Feedback != null ? (from reviewTbl in Feedback.RIC_AnnualFeedbacReviewer
                                                  join uTbl in unitOfwork.User.Get()
                                                  on reviewTbl.RR_ReviewerID equals uTbl.RE_Emp_Cd
                                                  where reviewTbl.RR_Status == "InProgress"
                                                  select new { uTbl.RE_Jobdiva_User_Name })
                                                             .Select(s => s.RE_Jobdiva_User_Name)
                                                             .ToList() : null,
                  ReportingTo = user.ReportingTo

              }).Where(s=>s.reviewers.Count >0); 
                //var linq4 = unitOfwork.RIC_AnnualFeedback.Get(s => s.RA_Date.Year == usDate.Year);
                //List<DateTime> testdept = linq1.Select(a => a.ReviewDate).ToList();

                
                ViewData["EmpData"] = linq1;
                linq1 = linq1.ToList();

                var linq3 = (
                from reviewList in unitOfwork.RIC_AnnualFeedback.Get(s => s.RIC_AnnualFeedbacReviewer.Count() > 0
                && s.RIC_AnnualFeedbacReviewer.FirstOrDefault(a => a.RR_ReviewerID == empCd && a.RR_Status == "InProgress") != null
                )

                join Empoyee in unitOfwork.User.Get()
                on reviewList.RA_EmployeeId equals Empoyee.RE_Emp_Cd
                join Hr in unitOfwork.User.Get()
                on reviewList.RA_HrId equals Hr.RE_Emp_Cd
                //where Empoyee.RE_Jobdiva_User_Name == "abdul, ajeej"

                select new AnnualFeedbackWM
                {
                    ReviewID = reviewList.RA_ReviewId,
                    EmployeeName = Empoyee.RE_Jobdiva_User_Name,
                    IssuedBy = Hr.RE_Jobdiva_User_Name,
                    ReviewDate = reviewList.RA_Date,
                    FirstSubmission = false,
                    HighlightRow = AnnualReviewRequest.Count(ar => ar.Rn_ReviewID == reviewList.RA_ReviewId) > 0
                });


                ViewData["VRMList"] = linq3;

                return View(linq1);

            }

            else
            {
                var linq2 = (from reviewList in unitOfwork.RIC_AnnualFeedback.Get(s => s.RA_LeadReviewerId == empCd
                 && s.RIC_AnnualFeedbacReviewer.Count() == 0)
                             join Empoyee in unitOfwork.User.Get()
                             on reviewList.RA_EmployeeId equals Empoyee.RE_Emp_Cd
                             join Hr in unitOfwork.User.Get()
                             on reviewList.RA_HrId equals Hr.RE_Emp_Cd
                             select new AnnualFeedbackWM
                             {
                                 ReviewID = reviewList.RA_ReviewId,
                                 EmployeeName = Empoyee.RE_Jobdiva_User_Name,
                                 IssuedBy = Hr.RE_Jobdiva_User_Name,
                                 ReviewDate = reviewList.RA_Date,
                                 FirstSubmission = true,
                                 HighlightRow = AnnualReviewRequest.Count(ar => ar.Rn_ReviewID == reviewList.RA_ReviewId) > 0
                             }).Union(
                from reviewList in unitOfwork.RIC_AnnualFeedback.Get(s => s.RIC_AnnualFeedbacReviewer.Count() > 0
                && s.RIC_AnnualFeedbacReviewer.FirstOrDefault(a => a.RR_ReviewerID == empCd && a.RR_Status == "InProgress") != null
                )
                join Empoyee in unitOfwork.User.Get()
                on reviewList.RA_EmployeeId equals Empoyee.RE_Emp_Cd
                join Hr in unitOfwork.User.Get()
                on reviewList.RA_HrId equals Hr.RE_Emp_Cd
                select new AnnualFeedbackWM
                {
                    ReviewID = reviewList.RA_ReviewId,
                    EmployeeName = Empoyee.RE_Jobdiva_User_Name,
                    IssuedBy = Hr.RE_Jobdiva_User_Name,
                    ReviewDate = reviewList.RA_Date,
                    FirstSubmission = false,
                    HighlightRow = AnnualReviewRequest.Count(ar => ar.Rn_ReviewID == reviewList.RA_ReviewId) > 0
                });


                return View(linq2);
            }

            //var AnnualFeedbackWM =
            //                  (from reviewList in unitOfwork.RIC_AnnualFeedback.Get(s => s.RA_LeadReviewerId == empCd
            //                      && s.RIC_AnnualFeedbacReviewer.Count() == 0)
            //                   join Empoyee in unitOfwork.User.Get()
            //                   on reviewList.RA_EmployeeId equals Empoyee.RE_Emp_Cd
            //                   join Hr in unitOfwork.User.Get()
            //                   on reviewList.RA_HrId equals Hr.RE_Emp_Cd
            //                   select new AnnualFeedbackWM
            //                   {
            //                       ReviewID = reviewList.RA_ReviewId,
            //                       EmployeeName = Empoyee.RE_Jobdiva_User_Name,
            //                       IssuedBy = Hr.RE_Jobdiva_User_Name,
            //                       ReviewDate = reviewList.RA_Date,
            //                       FirstSubmission = true,
            //                       HighlightRow = AnnualReviewRequest.Count(ar => ar.Rn_ReviewID == reviewList.RA_ReviewId) > 0
            //                   }).Union(
            //                   from reviewList in unitOfwork.RIC_AnnualFeedback.Get(s => s.RIC_AnnualFeedbacReviewer.Count() > 0
            //                       && s.RIC_AnnualFeedbacReviewer.FirstOrDefault(a => a.RR_ReviewerID == empCd && a.RR_Status == "InProgress") != null
            //                       )
            //                   join Empoyee in unitOfwork.User.Get()
            //               on reviewList.RA_EmployeeId equals Empoyee.RE_Emp_Cd
            //                   join Hr in unitOfwork.User.Get()
            //                   on reviewList.RA_HrId equals Hr.RE_Emp_Cd
            //                   select new AnnualFeedbackWM
            //                   {
            //                       ReviewID = reviewList.RA_ReviewId,
            //                       EmployeeName = Empoyee.RE_Jobdiva_User_Name,
            //                       IssuedBy = Hr.RE_Jobdiva_User_Name,
            //                       ReviewDate = reviewList.RA_Date,
            //                       FirstSubmission = false,
            //                       HighlightRow = AnnualReviewRequest.Count(ar => ar.Rn_ReviewID == reviewList.RA_ReviewId) > 0
            //                   });


            //return View(AnnualFeedbackWM);



        }

        public ActionResult AddReviewerFeedback(int id, string ReturnUrl)
        {
            // get the role for user.
            RolePrincipal RolePrincipal = (RolePrincipal)User;
            var role = RolePrincipal.GetRoles()[0];

            AddAnnualFeedbackWM feedbackModel = new AddAnnualFeedbackWM();

            var feedback = unitOfwork.RIC_AnnualFeedback.GetByID(id);

            var userDetails = unitOfwork.User.GetByEmpID(feedback.RA_EmployeeId);

            feedbackModel.ReviewId = feedback.RA_ReviewId;
            feedbackModel.ReviewDate = feedback.RA_Date;
            feedbackModel.EmployeeName = userDetails.RE_Jobdiva_User_Name;
            feedbackModel.JoiningDate = userDetails.RE_Joining_Date;
            feedbackModel.Designation = userDetails.RMS_Designation != null ? userDetails.RMS_Designation.RD_Designation : null;
            // bind the collection
            feedbackModel.FeedbackFields = unitOfwork.RIC_AnnualFeedbackForm.GetByID(feedback.RA_FormId)
                .RIC_AnnualFeedbackFields
                 .Where(fields => fields.AF_Roles == "All" || fields.AF_Roles == role)
                .Select(s =>
                 new AnnualFeedbackFields
                 {
                     FieldName = s.AF_FieldName,
                     fieldID = s.AF_FieldId,
                     IsHeader = s.AF_Pid == null,
                     TotalRecords = unitOfwork.RIC_AnnualFeedbackForm.GetByID(feedback.RA_FormId)
                                    .RIC_AnnualFeedbackFields.Count(tr => tr.AF_Pid == s.AF_Pid),
                     Weightage = s.AF_Weightage

                 });

            //get the interim feedback for year.
            feedbackModel.InterimReviewDetails = unitOfwork.RIC_PersonalFeedback
                        .Get(r => r.RP_EmployeeID == userDetails.RE_Emp_Cd && r.RP_Status == "Completed" && r.RP_ReviewDate.Year == usDate.Year)
                        .Select(s => new InterimReviewDetails
                        {
                            InterimReviewID = s.RP_ID,
                            ReviewerName = unitOfwork.User.GetByEmpID(s.RP_ReviewerID).RE_Jobdiva_User_Name,
                            Date = s.RP_ReviewDate,
                            FromDate = s.RP_FromDate,
                            ToDate = s.RP_ToDate,
                        }).ToList();
            //feedbackModel.submissionAnalysys = unitOfwork.RIC_Job_Report.GetSubmissionAnalysisByEnpCd(userDetails.RE_Emp_Cd);
            //get the submission analaysys last four quarter for user.
            var date1 = Convert.ToDateTime(Reviewdate);//new DateTime(DateTime.Now.Year-1, 12, 31);//DateTime.Now.Date;

            DashBoardIndexAction dashBoardIndex = new DashBoardIndexAction();
            feedbackModel.QuarterSubmissionAnalysys = dashBoardIndex.GetSubmissionAnalysis(date1, userDetails.RE_Emp_Cd);

            ViewBag.ReturnUrl = ReturnUrl;

            return View(feedbackModel);
        }

        [HttpPost]
        public ActionResult AddReviewerFeedback(int ReviewId, string AdditionalComments, string ImprovementsRecommended, FormCollection fc)
        {
            string empCd = User.Identity.Name;
            var userDetails = unitOfwork.User.GetByEmpID(empCd);
            // get the parameters from formcollection.
            var Fc_list = fc.AllKeys.Where(w => w.Contains("_")).Select(s => new
            {
                value = fc[s],
                id = s,
                fieldID = s.Substring(s.IndexOf('_') + 1),
                detailID = s.Substring(0, s.IndexOf('_'))
            }).Where(w => w.detailID == "Rating" || w.detailID == "Comment")
            .GroupBy(g => g.fieldID).Select(s => new
            {
                FieldId = int.Parse(s.Key),
                Rating = int.Parse(s.FirstOrDefault(R => R.detailID == "Rating").value),
                Comments = s.FirstOrDefault(C => C.detailID == "Comment").value
            });
            //get the current reviewer from reviewer tbl.
            var reviewer = unitOfwork.RIC_AnnualFeedbackReviewers.Get(s => s.RR_ReviewID == ReviewId && s.RR_ReviewerID == empCd)
                .FirstOrDefault();
            // add the commnts..
            reviewer.RR_AdditionalComments = AdditionalComments;
            reviewer.RR_ImprovementsRecommended = ImprovementsRecommended;
            // set the user review status.
            reviewer.RR_Status = "Completed";
            //set the review details in database.
            reviewer.RIC_AnnualFeedbackDtl =
                                       Fc_list.Select(s => new RIC_AnnualFeedbackDtl
                                       {
                                           //  RA_ReviewID = ReviewId,
                                           RA_ReviewerID = empCd,
                                           RA_FieldID = s.FieldId,
                                           RA_ReviewRating = s.Rating,
                                           RA_ReviewComments = s.Comments,
                                           RA_Status = "Completed"
                                       }).ToList();
            unitOfwork.Save();

            // get the review so we can add notification.
            var review = unitOfwork.RIC_AnnualFeedback.GetByID(ReviewId);
            foreach (var reviwer in review.RIC_AnnualFeedbacReviewer)
            {
                string userRoleName = unitOfwork.User
                                    .GetByEmpID(reviwer.RR_ReviewerID)
                                    .RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
                //if role is director role name then insert the notification.
                if (userRoleName == directorRoleName)
                {
                    // add the notification for Reviewers.
                    unitOfwork.Notification.Insert(new RIC_Notification
                    {
                        RN_EmpCd = reviwer.RR_ReviewerID,
                        Rn_ReviewID = review.RA_ReviewId,
                        RN_NotificationText = "Review Added by  " + userDetails.RE_Jobdiva_User_Name,
                        RN_IsSeen = false,
                        Date = usDate,
                        RN_NotificationType = "ReviewUpdate"
                    });
                }
            }
            review.RA_UpdatedBy = userDetails.RE_Jobdiva_User_Name;
            review.RA_UpdatedDate = usDate;
            // add the notification for HR.
            unitOfwork.Notification.Insert(new RIC_Notification
            {
                RN_EmpCd = review.RA_HrId,
                Rn_ReviewID = review.RA_ReviewId,
                RN_NotificationText = "Review Added by  " + userDetails.RE_Jobdiva_User_Name,
                RN_IsSeen = false,
                Date = usDate,
                RN_NotificationType = "ReviewUpdate"
            });

            // set the review status to completed.
            if (review.RIC_AnnualFeedbacReviewer.Count(s => s.RR_Status != "Completed") == 0 && review.RA_Hr_ReviewStatus == "Completed")
            {
                review.RA_Status = "Completed";
                // Add the average rating in database.
                var avgRating = review.RIC_AnnualFeedbacReviewer
                               .Average(s => s.RIC_AnnualFeedbackDtl.Average(D => D.RA_ReviewRating));
                var avgHr = (review.RA_Hr_LoyaltyToCompanyValues + review.RA_Hr_BehavingToCulturalValue + review.RA_Hr_RespectsDiffInCulturalValue) / 3;
                review.RA_AvgRating = (avgRating + avgHr) / 2;

            }
            else
                review.RA_Status = "InProgress";

            unitOfwork.Save();


            var GetEmployeeName = (from e in unitOfwork.RIC_Employee.Get()
                                   where e.RE_Emp_Cd == review.RA_EmployeeId
                                   select new
                                   {

                                       EmployeeName = e.RE_Employee_Name
                                   }).ToList();

            var GetHrName = (from e in unitOfwork.RIC_Employee.Get()
                             where e.RE_Emp_Cd == review.RA_HrId
                             select new
                             {

                                 HrName = e.RE_Employee_Name
                             }).ToList();

            var GetLeadName = (from e in unitOfwork.RIC_Employee.Get()
                               where e.RE_Emp_Cd == review.RA_LeadReviewerId
                               select new
                               {

                                   LeadName = e.RE_Employee_Name
                               }).ToList();


            string error;
            string ReviewDetails = @"<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='viewport' content='width=device-width, initial-scale=1'><meta name='ProgId' content='Word.Document'></head><body style='font-family: Arial; font-size: 12px;'>
<div style='font-size: 14px; width: 50%; margin-left: 25%;box-sizing: unset;border-radius: 7px;border: 1px solid #c5c5c5!important;box-shadow: 3px 3px 10px #bfbfbf;margin-top: 5%;padding: 1%;'>
<h3>Annual Review Feedback Details</h3>
<span style='font-weight:bold'>Employee Name: </span><span>" + GetEmployeeName.FirstOrDefault().EmployeeName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Reporting To : </span><span>" + GetLeadName.FirstOrDefault().LeadName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Issued By : </span><span>" + GetHrName.FirstOrDefault().HrName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Reviewer Name: </span><span>" + userDetails.RE_Employee_Name.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Review Date: </span><span style='text-align:left'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</span><br/><br/><p><span style='font-weight:bold'>Note:</span>This is an automated email message. Please do not reply to this email.<p></div></body></html>";

            Email.SendMail1(GetEmail(), "Annual Review Feedback Details", ReviewDetails, out error);


            return Content(("<script language='javascript' type='text/javascript'> alert('Review Submitted Successfully');window.location = '" + Url.Action("SubmittedAnnualFeedback") + "';</script>"));
            // return RedirectToAction("AnnualFeedback");
        }

        [HttpGet]
        public ActionResult AnuulaFeedbackPrint(int reviewID, string showComments)
        {

            //bind the annual feedback model.
            AnnualFeedbackDetailsVM model = new AnnualFeedbackDetailsVM();
            var review = unitOfwork.RIC_AnnualFeedback.GetByID(reviewID);
            var employee = unitOfwork.User.GetByEmpID(review.RA_EmployeeId);
            // get employee id from from authentication. 
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            // get the role for user.
            RolePrincipal rolePrincipal = (RolePrincipal)User;
            var role = rolePrincipal.GetRoles()[0];
            // set the flag to show all users.
            bool showAllUser = (role == HrRoleName || role == directorRoleName || role == stafingDirectorRole) && review.RA_Status == "Completed";
            // bool showAllUser = (role == HrRoleName || role == directorRoleName);
            // bind the user details to model.
            model.ReviewID = reviewID;
            model.EmployeeID = employee.RE_Emp_Cd;
            model.EmployeeName = employee.RE_Jobdiva_User_Name;
            model.JoiningDate = employee.RE_Joining_Date;
            model.Designation = employee.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
            model.ReviewDate = review.RA_Date;
            model.Hr_LeaveTakenOnLop = review.RA_Hr_LeavesTakenOnLop;
            model.Hr_LeaveTaken = review.RA_Hr_LeaveHistory;
            model.Hr_Warnings = review.RA_Hr_Warnings;

            model.Hr_CompanyValue = review.RA_Hr_LoyaltyToCompanyValues;
            model.Hr_CultValue = review.RA_Hr_BehavingToCulturalValue;
            model.Hr_CultPeople = review.RA_Hr_RespectsDiffInCulturalValue;
            model.AverageRating = review.RA_AvgRating;
            model.AdditionalComments = new List<AdditionalComments>();
            model.ImprovementsRecommended = new List<ImprovementsRecommended>();
            string UserName = unitOfwork.User.GetByEmpID(review.RA_HrId).RE_Jobdiva_User_Name;

            if (showAllUser)
            {
                // Add HR comments.
                model.AdditionalComments.Add(new AdditionalComments { ReviewerName = UserName, Comments = review.RA_Hr_AdditionalComments });
                model.ImprovementsRecommended.Add(new ImprovementsRecommended { ReviewerName = UserName, Comments = review.RA_Hr_ImprovementsRecommended });

                //add the additional comments for reviewer
                foreach (var reviewer in review.RIC_AnnualFeedbacReviewer)
                {
                    //get the user Name
                    UserName = unitOfwork.User.GetByEmpID(reviewer.RR_ReviewerID).RE_Jobdiva_User_Name;
                    //add the comments in list
                    model.AdditionalComments.Add(new AdditionalComments { ReviewerName = UserName, Comments = reviewer.RR_AdditionalComments });
                    model.ImprovementsRecommended.Add(new ImprovementsRecommended { ReviewerName = UserName, Comments = reviewer.RR_ImprovementsRecommended });
                }
            }
            else if (role == HrRoleName)
            {
                // Add HR comments.
                model.AdditionalComments.Add(new AdditionalComments { ReviewerName = UserName, Comments = review.RA_Hr_AdditionalComments });
                model.ImprovementsRecommended.Add(new ImprovementsRecommended { ReviewerName = UserName, Comments = review.RA_Hr_ImprovementsRecommended });
            }
            else if (role == stafingDirectorRole)
            {

                foreach (var reviewer1 in review.RIC_AnnualFeedbacReviewer)
                {
                    //get the user Name
                    UserName = unitOfwork.User.GetByEmpID(reviewer1.RR_ReviewerID).RE_Jobdiva_User_Name;
                    //add the comments in list
                    model.AdditionalComments.Add(new AdditionalComments { ReviewerName = UserName, Comments = reviewer1.RR_AdditionalComments });
                    model.ImprovementsRecommended.Add(new ImprovementsRecommended { ReviewerName = UserName, Comments = reviewer1.RR_ImprovementsRecommended });
                }
            }

            else
            {
                UserName = unitOfwork.User.GetByEmpID(empCd).RE_Jobdiva_User_Name;
                var reviewer = review.RIC_AnnualFeedbacReviewer.FirstOrDefault(s => s.RR_ReviewerID == empCd);

                //add the comments in list
                model.AdditionalComments.Add(new AdditionalComments { ReviewerName = UserName, Comments = reviewer.RR_AdditionalComments });
                model.ImprovementsRecommended.Add(new ImprovementsRecommended { ReviewerName = UserName, Comments = reviewer.RR_ImprovementsRecommended });

            }
            // get the attribute list 
            model.AttributHeaders = (from reviewer in review.RIC_AnnualFeedbacReviewer
                                     join details in unitOfwork.RIC_AnnualFeedbackDtl.Get()
                                     on reviewer.RR_ID equals details.RA_ReviewID
                                     join reviewerName in unitOfwork.User.Get()
                                     on reviewer.RR_ReviewerID equals reviewerName.RE_Emp_Cd
                                     join fields in unitOfwork.RIC_AnnualFeedbackFields.Get()
                                     on details.RA_FieldID equals fields.AF_FieldId
                                     join header in unitOfwork.RIC_AnnualFeedbackFields.Get()
                                     on fields.AF_Pid equals header.AF_FieldId
                                     where showAllUser || role == stafingDirectorRole || reviewer.RR_ReviewerID == empCd
                                     select new
                                     {
                                         ReviewerID = reviewer.RR_ReviewerID,
                                         ReviewerName = reviewerName.RE_Jobdiva_User_Name,
                                         AdditionalComments = reviewer.RR_AdditionalComments,
                                         ImprovementsRecommended = reviewer.RR_ImprovementsRecommended,
                                         Status = reviewer.RR_Status,
                                         FieldID = details.RA_FieldID,
                                         HeaderName = header.AF_FieldName,
                                         FieldName = fields.AF_FieldName,
                                         ReviewRating = details.RA_ReviewRating,
                                         ReviewComments = details.RA_ReviewComments,
                                         // ImprovementsRecommended=reviewer.RR_ImprovementsRecommended
                                     }).GroupBy(g => new { g.HeaderName })
                                .Select(header => new AnnualFeedbackDetailsAttributesHeaders
                                {
                                    HeaderName = header.Key.HeaderName,
                                    Attributes = header.GroupBy(attrKey => attrKey.FieldName)
                                    .Select(attr => new AnnualFeedbackDetailsAttributes
                                    {
                                        AttrID = attr.FirstOrDefault().FieldID,
                                        AttrName = attr.Key,
                                        rating = attr.Select(r => new AnnualFeedbackDetailsRating
                                        {
                                            ReviewerName = r.ReviewerName,
                                            Rating = r.ReviewRating,
                                            Comments = r.ReviewComments,

                                        }).ToList()
                                    }).ToList()
                                }).ToList();

            //get the submission analaysys for user.           
            model.submissionAnalysys = unitOfwork.RIC_Job_Report.GetSubmissionAnalysisByEnpCd(employee.RE_Emp_Cd);

            DashBoardIndexAction dashBoardIndex = new DashBoardIndexAction();
            model.QuarterSubmissionAnalysys = dashBoardIndex.GetSubmissionAnalysis(Convert.ToDateTime( Reviewdate), employee.RE_Emp_Cd);

            model.ShowHrReview = role == HrRoleName || ((role == directorRoleName || role == stafingDirectorRole) && review.RA_Status == "Completed");
            model.ShowAvgRating = review.RA_Status == "Completed" && (role == HrRoleName || role == directorRoleName || role == stafingDirectorRole);
            ViewBag.showCommnets = showComments;
            return View(model);
        }

        [HttpGet]
        public ActionResult AddAnnualFeedback(int id, string ReturnUrl)
        {

            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];

            AddAnnualFeedbackWM feedbackModel = new AddAnnualFeedbackWM();

            var feedback = unitOfwork.RIC_AnnualFeedback.GetByID(id);

            var userDetails = unitOfwork.User.GetByEmpID(feedback.RA_EmployeeId);


            // add the user details.
            feedbackModel.ReviewId = feedback.RA_ReviewId;
            feedbackModel.ReviewDate = feedback.RA_Date;
            feedbackModel.EmployeeName = userDetails.RE_Jobdiva_User_Name;
            feedbackModel.JoiningDate = userDetails.RE_Joining_Date;
            feedbackModel.Designation = userDetails.RMS_Designation != null ? userDetails.RMS_Designation.RD_Designation : null;

            // add the fields..
            feedbackModel.FeedbackFields = unitOfwork.RIC_AnnualFeedbackForm
                .GetByID(feedback.RA_FormId)
               .RIC_AnnualFeedbackFields
               .Where(fields => fields.AF_Roles == "All" || fields.AF_Roles == role)
               .Select(s =>
                new AnnualFeedbackFields
                {
                    FieldName = s.AF_FieldName,
                    fieldID = s.AF_FieldId,
                    IsHeader = s.AF_Pid == null,
                    Weightage = s.AF_Weightage

                });

            ViewBag.ReturnUrl = ReturnUrl;
            return View(feedbackModel);
        }

        [HttpPost]
        public ActionResult AddAnnualFeedback(int ReviewId, string AdditionalComments, string ImprovementsRecommended, List<string> RE_Reviewer_List, FormCollection fc)
        {
            //get the reviewe id.
            string ReviewerId = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            //GET the user details.
            var userDetails = unitOfwork.User.GetByEmpID(ReviewerId);
            // get the attributes from form collections
            var Fc_list = fc.AllKeys.Where(w => w.Contains("_")).Select(s => new
            {
                value = fc[s],
                id = s,
                fieldID = s.Substring(s.IndexOf('_') + 1),
                detailID = s.Substring(0, s.IndexOf('_'))
            }).Where(w => w.detailID == "Rating" || w.detailID == "Comment")
                .GroupBy(g => g.fieldID).Select(s => new
                {
                    FieldId = int.Parse(s.Key),
                    Rating = int.Parse(s.FirstOrDefault(R => R.detailID == "Rating").value),
                    Comments = s.FirstOrDefault(C => C.detailID == "Comment").value

                });
            // get the review for user.
            var review = unitOfwork.RIC_AnnualFeedback.GetByID(ReviewId);

            List<RIC_AnnualFeedbackDtl> AnnualFeedbackDtl = new List<RIC_AnnualFeedbackDtl>();
            //add the TL review.
            review.RIC_AnnualFeedbacReviewer.Add(new RIC_AnnualFeedbackReviewers
            {
                RR_ReviewerID = review.RA_LeadReviewerId,
                RR_Status = "Completed",
                RR_AdditionalComments = AdditionalComments,
                RR_ImprovementsRecommended = ImprovementsRecommended
            });
            // add the anuual feedback details.
            AnnualFeedbackDtl.AddRange(
                                        Fc_list.Select(s => new RIC_AnnualFeedbackDtl
                                        {
                                            //  RA_ReviewID = ReviewId,
                                            RA_ReviewerID = ReviewerId,
                                            RA_FieldID = s.FieldId,
                                            RA_ReviewRating = s.Rating,
                                            RA_ReviewComments = s.Comments,
                                            RA_Status = "Completed"

                                        }));
            // add the details in context
            review.RIC_AnnualFeedbacReviewer
                .FirstOrDefault(s => s.RR_ReviewerID == ReviewerId).RIC_AnnualFeedbackDtl = AnnualFeedbackDtl;

            // add the reviewer List.
            foreach (string reviwer in RE_Reviewer_List)
            {
                review.RIC_AnnualFeedbacReviewer.Add(new RIC_AnnualFeedbackReviewers
                {
                    RR_ReviewerID = reviwer,
                    RR_Status = "InProgress"
                });
                // add the notification for users.
                unitOfwork.Notification.Insert(new RIC_Notification
                {
                    RN_EmpCd = reviwer,
                    Rn_ReviewID = review.RA_ReviewId,
                    RN_NotificationText = "Review Added by  " + userDetails.RE_Jobdiva_User_Name,
                    RN_IsSeen = false,
                    Date = usDate,
                    RN_NotificationType = "AnnualReviewRequest"
                });
            }
            // add the notification for HR.
            unitOfwork.Notification.Insert(new RIC_Notification
            {
                RN_EmpCd = review.RA_HrId,
                Rn_ReviewID = review.RA_ReviewId,
                RN_NotificationText = "Review added by " + ReviewerId,
                RN_IsSeen = false,
                Date = usDate,
                RN_NotificationType = "ReviewUpdate"
            });

            // set the status of review to in progress.
            review.RA_Status = "InProgress";

            // Save the changes.   
            unitOfwork.Save();

            return Content(("<script language='javascript' type='text/javascript'> alert('Review Submitted Successfully');window.location = '" + Url.Action("SubmittedAnnualFeedback") + "';</script>"));
            // return RedirectToAction("SubmittedAnnualFeedback");
        }

        [HttpGet]
        public ActionResult AnnualFeedbackRequestHr(int? page, string EmployeeName = null, int Year = 0, string Status = null)
        {
            //Status = (Status != "" && Status != null) ? Status : null; 
            //int Year=unitOfwork.RIC_AnnualFeedbackrequ
            //get the user id from authentication
            string UserID = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            Session["ReviewUpdate"] = unitOfwork.Notification.Get(s => s.RN_EmpCd == UserID && s.RN_IsSeen == false
                                                       && s.RN_NotificationType == "ReviewUpdate").Count();
            var AnnualReviewRequest = unitOfwork.Notification.Get(s => s.RN_EmpCd == UserID && s.RN_IsSeen == false
                                           && s.RN_NotificationType == "AnnualReviewRequest").ToList();
            //set the notification to seen
            AnnualReviewRequest.ForEach(m => m.RN_IsSeen = true);
            unitOfwork.Save();
            //set the session values
            Session["AnnualReviewRequest"] = 0;
            int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            // select all annual feedback and convert to paged list.
            IEnumerable<AnnualFeedbackRequestHrVM> reviewList =
            (from feedback in unitOfwork.RIC_AnnualFeedback.Get()
             join employee in unitOfwork.User.Get()
             on feedback.RA_EmployeeId equals employee.RE_Emp_Cd
             join lead in unitOfwork.User.Get()
             on feedback.RA_LeadReviewerId equals lead.RE_Emp_Cd into f
             from lead in f.DefaultIfEmpty()
             where (EmployeeName == null || EmployeeName == "" || employee.RE_Jobdiva_User_Name.ToLower().Contains(EmployeeName.ToLower()))
                    && (Convert.ToInt32(feedback.RA_Date.Year) == Convert.ToInt32(Year) || Year == 0)
                    && (feedback.RA_Status == Status || Status == null || Status == "")
             select new AnnualFeedbackRequestHrVM()
             {
                 ReviewID = feedback.RA_ReviewId,
                 EmployeeName = employee.RE_Jobdiva_User_Name,
                 LeadReviewer = lead == null ? null : lead.RE_Jobdiva_User_Name,
                 ReviewStatus = feedback.RA_Status,
                 ReviewDate = feedback.RA_Date,
                 HrReviewStatus = feedback.RA_Hr_ReviewStatus == "Completed" ? true : false,
                 IsCompleted = feedback.RA_Status == "Completed",
                 LastUpdateDate = feedback != null ? feedback.RA_UpdatedDate : null,
                 //get the pending reviewers.
                 PendingReviewers = (from PendingReviewers in feedback.RIC_AnnualFeedbacReviewer.Where(pr => pr.RR_Status == "InProgress")
                                     join PendingUserTbl in unitOfwork.User.Get()
                                     on PendingReviewers.RR_ReviewerID equals PendingUserTbl.RE_Emp_Cd
                                     select PendingUserTbl.RE_Jobdiva_User_Name).ToList(),
                 NotificationCount = AnnualReviewRequest.Count(rc => rc.Rn_ReviewID == feedback.RA_ReviewId)
             }).OrderByDescending(o => o.LastUpdateDate).ToPagedList(pageIndex, 10);



            //download data into excel


            var reviewDownloadList = (from feedback in unitOfwork.RIC_AnnualFeedback.Get()
                                      join employee in unitOfwork.User.Get()
                                      on feedback.RA_EmployeeId equals employee.RE_Emp_Cd
                                      join lead in unitOfwork.User.Get()
                                      on feedback.RA_LeadReviewerId equals lead.RE_Emp_Cd into f
                                      from lead in f.DefaultIfEmpty()
                                      where (EmployeeName == null || EmployeeName == "" || employee.RE_Jobdiva_User_Name.ToLower().Contains(EmployeeName.ToLower()))
                                             && (Convert.ToInt32(feedback.RA_Date.Year) == Convert.ToInt32(Year) || Year == 0)
                                             && (feedback.RA_Status == Status || Status == null || Status == "")
                                      select new AnnualFeedbackRequestHrVM()
                                      {
                                          EmployeeName = employee.RE_Jobdiva_User_Name,
                                          LeadReviewer = lead == null ? null : lead.RE_Jobdiva_User_Name,
                                          ReviewStatus = feedback.RA_Status,
                                          ReviewDate = feedback.RA_Date
                                      }).OrderByDescending(o => o.LastUpdateDate).ToList();




            Session["Data"] = reviewDownloadList;
            //end



            var YearSelectList = Enumerable.Range(2016, usDate.Year - 2015).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Value).ToList();


            List<SelectListItem> _lstStatus = new List<SelectListItem>();

            _lstStatus.Add(new SelectListItem() { Text = "All", Value = "" });
            _lstStatus.Add(new SelectListItem() { Text = "Released", Value = "Released" });
            _lstStatus.Add(new SelectListItem() { Text = "InProgress", Value = "InProgress" });
            _lstStatus.Add(new SelectListItem() { Text = "Completed", Value = "Completed" });



            ViewBag.EmployeeName = EmployeeName;
            ViewBag.Year = Year;
            ViewBag.Status = Status;
            ViewBag.YearSelectList = YearSelectList;
            ViewBag.StatusList = _lstStatus;


            return View(reviewList);
        }


        [Authorize]
        public ActionResult ExportToExcel()
        {

            var data = Session["Data"] as IEnumerable<AnnualFeedbackRequestHrVM>;


            DataTable dtVM = new DataTable();
            dtVM.Columns.Add("Employee Name", typeof(string));
            dtVM.Columns.Add("Lead Reviewer", typeof(string));
            dtVM.Columns.Add("Review Status", typeof(string));
            dtVM.Columns.Add("Review Date", typeof(string));

            DataRow dr;


            foreach (var item in data)
            {
                dr = dtVM.NewRow();


                dr[0] = item.EmployeeName;
                dr[1] = item.LeadReviewer;
                dr[2] = item.ReviewStatus;
                dr[3] = item.ReviewDate.ToString("d");

                dtVM.Rows.Add(dr);


            }

            string ViewName = "AnnualFeedback.xlsx";
            using (ExcelPackage pck = new ExcelPackage())
            {


                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ViewReport");
                ws.Cells["A1"].LoadFromDataTable(dtVM, true); //You can Use TableStyles property of your desire.    
                                                              //Read the Excel file in a byte array    
                Byte[] fileBytes = pck.GetAsByteArray();

                var response = new FileContentResult(fileBytes, "application/octet-stream");
                response.FileDownloadName = ViewName;// set the file name
                return response;// download file.
            }


        }




        [HttpPost]
        public ActionResult AnnualFeedbackRequestHr(AnnualFeedbackRequestHrPostVM hrFeedback)
        {
            string empCd = User.Identity.Name;
            var review = unitOfwork.RIC_AnnualFeedback.GetByID(hrFeedback.ReviewID);
            // save the HR reviews in database.
            review.RA_Hr_LeaveHistory = hrFeedback.LeavesTaken;
            review.RA_Hr_LeavesTakenOnLop = hrFeedback.LeavesTakenOnLop;
            review.RA_Hr_Warnings = hrFeedback.Warnings;
            review.RA_Hr_LoyaltyToCompanyValues = hrFeedback.Loyality;
            review.RA_Hr_BehavingToCulturalValue = hrFeedback.CultureValues;
            review.RA_Hr_RespectsDiffInCulturalValue = hrFeedback.RespectBetweenPeople;
            review.RA_Hr_ReviewStatus = "Completed";
            review.RA_Hr_AdditionalComments = hrFeedback.AdditionalComments;
            review.RA_Hr_ImprovementsRecommended = hrFeedback.ImprovementsRecommended;
            review.RA_UpdatedBy = unitOfwork.User.GetByEmpID(empCd).RE_Jobdiva_User_Name;
            review.RA_UpdatedDate = usDate;
            if (review.RIC_AnnualFeedbacReviewer.Count() > 0 && review.RIC_AnnualFeedbacReviewer.Count(s => s.RR_Status != "Completed") == 0)
            {
                review.RA_Status = "Completed";
                //Add the average rating in database.
                var avgRating = review.RIC_AnnualFeedbacReviewer
                             .Average(s => s.RIC_AnnualFeedbackDtl.Average(D => D.RA_ReviewRating));
                var avgHr = (review.RA_Hr_LoyaltyToCompanyValues + review.RA_Hr_BehavingToCulturalValue + review.RA_Hr_RespectsDiffInCulturalValue) / 3;
                review.RA_AvgRating = (avgRating + avgHr) / 2;
            }
            // save the changes
            unitOfwork.Save();
            return Content(("<script language='javascript' type='text/javascript'> alert('Review Submitted Successfully');window.location = '" + Url.Action("AnnualFeedbackRequestHr") + "';</script>"));
        }

        public JsonResult GetAnnualReviewListByUser(string empCd)
        {
            //get reviews list fpr user.
            var reviewList = (from review in unitOfwork.RIC_AnnualFeedback.Get(s => s.RA_EmployeeId == empCd)
                              join userName in unitOfwork.User.Get()
                              on review.RA_EmployeeId equals userName.RE_Emp_Cd
                              join leadReviewer in unitOfwork.User.Get()
                              on review.RA_LeadReviewerId equals leadReviewer.RE_Emp_Cd
                              into l
                              from leadReviewer in l.DefaultIfEmpty()
                              select new
                              {
                                  ReviewID = review.RA_ReviewId,
                                  EmployeeName = userName.RE_Jobdiva_User_Name,
                                  leadReviewerName = leadReviewer == null ? null : leadReviewer.RE_Jobdiva_User_Name,
                                  ReviewStatus = review.RA_Status,
                                  ReviewDate = review.RA_Date.ToString("MM/dd/yyyy")
                              }).ToList();
            return Json(reviewList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInterimReviewListByUser(string empCd)
        {
            //get reviews list fpr user.
            var reviewList = (from review in unitOfwork.RIC_PersonalFeedback.Get(s => s.RP_EmployeeID == empCd)
                              join userName in unitOfwork.User.Get()
                              on review.RP_EmployeeID equals userName.RE_Emp_Cd
                              join leadReviewer in unitOfwork.User.Get()
                              on review.RP_ReviewerID equals leadReviewer.RE_Emp_Cd
                              where review.RP_Status != "Draft"
                              select new
                              {
                                  ReviewID = review.RP_ID,
                                  EmployeeName = userName.RE_Jobdiva_User_Name,
                                  leadReviewerName = leadReviewer.RE_Jobdiva_User_Name,
                                  ReviewStatus = review.RP_Status,
                                  ReviewDate = review.RP_ReviewDate.ToString("MM/dd/yyyy")
                              }).ToList();
            return Json(reviewList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRepoartingHistory(string empCd)
        {
            //get the repoarting history for user.
            var reportingHistory = unitOfwork.User.getDirectReportingHistory(empCd).
                                    OrderByDescending(s => s.RR_FromDate)
                                    .Select(r => new
                                    {
                                        EmployeeName = r.Employee_Name,
                                        ReportingTo = r.Mgr_Name,
                                        FromDate = r.RR_FromDate.HasValue ? r.RR_FromDate.Value.ToString("MM/dd/yyyy") : null,
                                        ToDate = r.RR_ToDate.HasValue ? r.RR_ToDate.Value.ToString("MM/dd/yyyy") : null
                                    });
            return Json(reportingHistory, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Personal Feedback

        [HttpGet]
        public ActionResult InterimReviewSubmitted()
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            //get the notification
            var notification = unitOfwork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen == false && s.RN_NotificationType == "InterimReviewUpdate")
                      .ToList();
            //clear the update notification.
            notification.ForEach(u => u.RN_IsSeen = true);
            unitOfwork.Save();
            var _user = unitOfwork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
            //  get the list of reviews for users.          
            List<InterimReviewSubmittedModel> reviewModel =
                (from reviewerDetails in unitOfwork.RIC_PersonalFeedbackDtl.Get()
                 join review in unitOfwork.RIC_PersonalFeedback.Get()
                      on reviewerDetails.RD_ReviewID equals review.RP_ID
                 join employee in unitOfwork.User.Get()
                     on review.RP_EmployeeID equals employee.RE_Emp_Cd
                 join reportingTo in unitOfwork.User.Get()
                     on review.RP_ReviewerID equals reportingTo.RE_Emp_Cd
                 where (reviewerDetails.RD_Status == "Completed"
                       && review.RP_Status != "Draft"
                       && reviewerDetails.RD_RevewerID == empCd) || (
                         review.RP_Status == "Completed"
                         && reviewerDetails.RD_RevewerID == review.RP_ReviewerID &&
                         ((review.RP_DirectorID == empCd && review.RP_DirectorReviewStatus) ||
                          (review.RP_HrID == empCd && review.RP_Hr_ReviewStatus)))
                 orderby review.RP_UpdatedDate descending
                 select new InterimReviewSubmittedModel
                 {
                     ReviewID = review.RP_ID,
                     ReviewDate = review.RP_ReviewDate,
                     EmpCd = review.RP_EmployeeID,
                     CandidateName = employee.RE_Jobdiva_User_Name,
                     ManagerName = reportingTo.RE_Jobdiva_User_Name,
                     NextReviewDate = review.RP_NextReviewDate,
                     ReviewStatus = review.RP_Status,
                     NotificationCount = notification.Where(n => n.Rn_ReviewID == review.RP_ID).Count(),
                     PendingReviewers = (
                                from pendingReviewers in review.RIC_PersonalFeedbackDtl
                                join pendingUserName in unitOfwork.User.Get()
                             on pendingReviewers.RD_RevewerID equals pendingUserName.RE_Emp_Cd
                                where pendingReviewers.RD_Status == "Released"
                                select pendingUserName.RE_Jobdiva_User_Name).ToList()
                 }).ToList();
            ViewBag.showEditBtn = role == directorRoleName || role == HrRoleName || role == stafingDirectorRole;
            ViewBag.Role = role;
            return View(reviewModel);
        }

        public ActionResult InterimDrafts()
        {
            string empCd = User.Identity.Name;

            // var reviewModel = getDrafts();
            // get the drafts for user.
            List<InterimReviewSubmittedModel> reviewModel = (
                        from personalFeedback in unitOfwork.RIC_PersonalFeedback.Get()
                        join employee in unitOfwork.User.Get()
                        on personalFeedback.RP_EmployeeID equals employee.RE_Emp_Cd
                        join reportingTo in unitOfwork.User.Get()
                        on personalFeedback.RP_ReviewerID equals reportingTo.RE_Emp_Cd
                        where personalFeedback.RP_ReviewerID == empCd && personalFeedback.RP_Status == "Draft"
                        select new InterimReviewSubmittedModel
                        {
                            ReviewID = personalFeedback.RP_ID,
                            ReviewDate = personalFeedback.RP_ReviewDate,
                            EmpCd = personalFeedback.RP_EmployeeID,
                            CandidateName = employee.RE_Jobdiva_User_Name,
                            ManagerName = reportingTo.RE_Jobdiva_User_Name,
                            NextReviewDate = personalFeedback.RP_NextReviewDate,
                            ReviewStatus = personalFeedback.RP_Status,
                            PendingReviewers =
                            (
                             from pendingReviewers in personalFeedback.RIC_PersonalFeedbackDtl
                             join pendingUserName in unitOfwork.User.Get()
                          on pendingReviewers.RD_RevewerID equals pendingUserName.RE_Emp_Cd
                             where pendingReviewers.RD_Status == "Released"
                             select pendingUserName.RE_Jobdiva_User_Name).ToList()

                        }).ToList();


            return View(reviewModel);
        }

        [HttpGet]
        public ActionResult AddInterimReview()
        {
            AddInterimReviewModel reviewModel = new AddInterimReviewModel();
            // get the list fo employee.
            reviewModel.CandidateList = GetEmployeeList();
            return View(reviewModel);
        }

        [HttpPost]
        public ActionResult AddInterimReview(AddInterimReviewModel reviewModel, string submit)
        {
            RIC_Employee item = new RIC_Employee();

            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            //To get the employee role
            RolePrincipal rolePrinciple = (RolePrincipal)User;
            string role = rolePrinciple.GetRoles()[0];

            ReviewUtitity utility = new ReviewUtitity();
            // get the list fo employee.
            reviewModel.CandidateList = GetEmployeeList();
            // get data buttion click.
            if (submit == "GetDatabtn")
            {
                ViewBag.showReamingFields = true;
                DateTime toDate = reviewModel.ToDate.Value.AddDays(1);
                // get the jd data.
                var jdReport = unitOfwork
                    .RIC_Job_Report.Get_JobRepoartForUser(reviewModel.EmpCd, reviewModel.FromDate.Value, toDate);
                //get the call data
                reviewModel.Calls = unitOfwork.CallStatistics.GetCallStataticsForUser(reviewModel.EmpCd, reviewModel.FromDate.Value, reviewModel.ToDate.Value)
                    .Count(s => s.RC_CallType == "Out" && s.RC_Call_Connected == 1);
                reviewModel.Submissions = jdReport.Count(s => s.RJ_Submit_Date >= reviewModel.FromDate.Value && s.RJ_Submit_Date <= toDate);
                reviewModel.Interviews = jdReport.Count(s => s.RJ_Interview_Date >= reviewModel.FromDate.Value && s.RJ_Interview_Date <= toDate);
                reviewModel.Hires = jdReport.Count(s => s.RJ_Hire_Date >= reviewModel.FromDate.Value && s.RJ_Hire_Date <= toDate);
                reviewModel.DirectorList = utility.GetDirectorList(); //unitOfwork.User.Get(s => s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == directorRoleName)                    
                reviewModel.EmployeeRole = role;
                return View(reviewModel);
            }// save buttion click 
            else
            {
                if (ModelState.IsValid)
                {
                    var reviewerList = new List<RIC_PersonalFeedbackDtl>();
                    //add the reviewer if list is not null.
                    if (reviewModel.Reviewer_List != null)
                        reviewerList = reviewModel.Reviewer_List.Select(s => new RIC_PersonalFeedbackDtl
                        {
                            RD_RevewerID = s,
                            RD_Status = "Released"
                        }).ToList();
                    // add the review.
                    reviewerList.Add(new RIC_PersonalFeedbackDtl
                    {
                        RD_RevewerID = empCd,
                        RD_Findings = reviewModel.TeamLeadReview,
                        RD_Improvements = reviewModel.ImprovementsRequired,
                        RD_Status = "Completed"
                    });
                    //save data in database.


                    var personalReview = new RIC_PersonalFeedback
                    {
                        RP_EmployeeID = reviewModel.EmpCd,
                        RP_ReviewerID = empCd,
                        RP_DirectorID = reviewModel.DirectorID,
                        RP_FromDate = reviewModel.FromDate.Value,
                        RP_ToDate = reviewModel.ToDate.Value,
                        RP_TotalSubmissions = reviewModel.Submissions,
                        RP_TotalInterview = reviewModel.Interviews,
                        RP_TotalHires = reviewModel.Hires,
                        RP_TotalCalls = reviewModel.Calls,
                        RP_NextReviewDate = reviewModel.NextReviewDate.Value,
                        RP_SubmissionTarget = reviewModel.SubmissionTarget,
                        RP_InterviewTarget = reviewModel.InterviewsTarget,
                        RP_HiresTarget = reviewModel.HiresTarget,
                        RP_CallsTarget = reviewModel.CallsTarget,
                        RP_ReviewDate = usDate,
                        RP_Status = reviewModel.SaveAsDraft ? "Draft" : reviewModel.Reviewer_List == null ? "Completed" : "InProgress",
                        RIC_PersonalFeedbackDtl = reviewerList
                    };
                    unitOfwork.RIC_PersonalFeedback.Insert(personalReview);
                    unitOfwork.Save();


                    var GetEmployeeName = (from e in unitOfwork.RIC_Employee.Get()
                                           where e.RE_Emp_Cd == personalReview.RP_EmployeeID
                                           select new
                                           {

                                               EmployeeName = e.RE_Employee_Name
                                           }).ToList();

                    var GetRevewierName = (from e in unitOfwork.RIC_Employee.Get()
                                           where e.RE_Emp_Cd == personalReview.RP_ReviewerID
                                           select new
                                           {
                                               ReviewName = e.RE_Employee_Name

                                           }).ToList();


                    string error;
                    string ReviewDetails = @"<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='viewport' content='width=device-width, initial-scale=1'><meta name='ProgId' content='Word.Document'></head><body style='font-family: Arial; font-size: 12px;'>
<div style='font-size: 14px; width: 50%; margin-left: 25%;box-sizing: unset;border-radius: 7px;border: 1px solid #c5c5c5!important;box-shadow: 3px 3px 10px #bfbfbf;margin-top: 5%;padding: 1%;'>
<h3>Interim Review Details</h3>
<span style='font-weight:bold'>Employee Name: </span><span>" + GetEmployeeName.FirstOrDefault().EmployeeName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Reviewer Name: </span><span>" + GetRevewierName.FirstOrDefault().ReviewName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Review Date: </span><span style='text-align:left'>" + personalReview.RP_ReviewDate.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>From Date: </span><span style='text-align:left'>" + personalReview.RP_FromDate.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>To Date: </span><span style='text-align:left'>" + personalReview.RP_ToDate.ToString("dd/MM/yyyy") + "</span><br/><br/><p><span style='font-weight:bold'>Note:</span>This is an automated email message. Please do not reply to this email.<p></div></body></html>";

                    Email.SendMail1(GetEmail(), "Interim Review Details", ReviewDetails, out error);
                    if (!reviewModel.SaveAsDraft)
                    {
                        // add the notification for Manager.
                        foreach (var pr in personalReview.RIC_PersonalFeedbackDtl.Where(s => s.RD_RevewerID != empCd))
                        {
                            unitOfwork.Notification.Insert(new RIC_Notification
                            {
                                Rn_ReviewID = personalReview.RP_ID,
                                RN_EmpCd = pr.RD_RevewerID,
                                Date = usDate,
                                RN_IsSeen = false,
                                RN_NotificationType = "InterimReviewRequest",
                                RN_NotificationText = "Review submited by " + empCd
                            });
                        }
                        if (personalReview.RP_Status == "Completed")
                        {
                            unitOfwork.Notification.Insert(new RIC_Notification
                            {
                                Rn_ReviewID = personalReview.RP_ID,
                                RN_EmpCd = personalReview.RP_DirectorID,
                                Date = usDate,
                                RN_IsSeen = false,
                                RN_NotificationType = "InterimReviewRequest",
                                RN_NotificationText = "Review submited by " + empCd
                            });
                        }
                        unitOfwork.Save();


                    }
                    return Content(("<script language='javascript' type='text/javascript'> alert('Review Submitted Successfully. ');window.location = '" + Url.Action("InterimReviewSubmitted") + "';</script>"));
                }
                else
                {
                    // show error
                    return View(reviewModel);
                }
            }
        }

        public ActionResult DeleteInterimReview(int id)
        {
            string empCd = User.Identity.Name;
            var entityToDelete = unitOfwork.RIC_PersonalFeedback.GetByID(id);
            if (entityToDelete != null)
            {
                unitOfwork.RIC_PersonalFeedback.Delete(entityToDelete);
                unitOfwork.Save();
            }
            List<InterimReviewSubmittedModel> reviewModel = (
                         from personalFeedback in unitOfwork.RIC_PersonalFeedback.Get()
                         join employee in unitOfwork.User.Get()
                         on personalFeedback.RP_EmployeeID equals employee.RE_Emp_Cd
                         join reportingTo in unitOfwork.User.Get()
                         on personalFeedback.RP_ReviewerID equals reportingTo.RE_Emp_Cd
                         where personalFeedback.RIC_PersonalFeedbackDtl
                         .FirstOrDefault(PF => PF.RD_RevewerID == empCd) != null && personalFeedback.RP_Status == "Draft"
                         select new InterimReviewSubmittedModel
                         {
                             ReviewID = personalFeedback.RP_ID,
                             ReviewDate = personalFeedback.RP_ReviewDate,
                             EmpCd = personalFeedback.RP_EmployeeID,
                             CandidateName = employee.RE_Jobdiva_User_Name,
                             ManagerName = reportingTo.RE_Jobdiva_User_Name,
                             NextReviewDate = personalFeedback.RP_NextReviewDate,
                             ReviewStatus = personalFeedback.RP_Status,
                             PendingReviewers =
                             (
                              from pendingReviewers in personalFeedback.RIC_PersonalFeedbackDtl
                              join pendingUserName in unitOfwork.User.Get()
                           on pendingReviewers.RD_RevewerID equals pendingUserName.RE_Emp_Cd
                              where pendingReviewers.RD_Status == "Released"
                              select pendingUserName.RE_Jobdiva_User_Name).ToList()

                         }).ToList();

            return View("InterimDrafts", reviewModel);
        }

        [HttpGet]
        public ActionResult EditInterimDraft(int id, string retUrl)
        {
            string empCd = User.Identity.Name;
            var review = unitOfwork.RIC_PersonalFeedback.GetByID(id);
            string CandidateName = unitOfwork.User.GetByEmpID(review.RP_EmployeeID).RE_Jobdiva_User_Name;
            EditInterimDraftModel model = new EditInterimDraftModel
            {
                ReviewID = review.RP_ID,
                DirectorID = review.RP_DirectorID,
                CandidateName = CandidateName,
                FromDate = review.RP_FromDate,
                ToDate = review.RP_ToDate,
                Submissions = review.RP_TotalSubmissions,
                Interviews = review.RP_TotalInterview,
                Hires = review.RP_TotalHires,
                Calls = review.RP_TotalCalls,
                TeamLeadReview = review.RIC_PersonalFeedbackDtl.FirstOrDefault(rp => rp.RD_RevewerID == empCd).RD_Findings,
                ImprovementsRequired = review.RIC_PersonalFeedbackDtl.FirstOrDefault(rp => rp.RD_RevewerID == empCd).RD_Improvements,
                NextReviewDate = review.RP_NextReviewDate,
                SubmissionTarget = review.RP_SubmissionTarget,
                InterviewsTarget = review.RP_InterviewTarget,
                HiresTarget = review.RP_HiresTarget,
                CallsTarget = review.RP_CallsTarget,
                DefaultReviewerList = review.RIC_PersonalFeedbackDtl.Where(w => w.RD_RevewerID != empCd)
                .Select(s => s.RD_RevewerID).ToList()
            };
            // get the select list.
            ReviewUtitity reviewUtility = new ReviewUtitity();
            ViewBag.reviewerSelectList = reviewUtility.GetReviewerList();
            // get the director list
            model.DirectorList = reviewUtility.GetDirectorList();
            ViewBag.retUrl = retUrl;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditInterimDraft(EditInterimDraftModel draftModel, string submit)
        {
            string empCd = User.Identity.Name;
            var reviewerList = new List<RIC_PersonalFeedbackDtl>();
            if (draftModel.SelectedReviewer != null)
            {
                reviewerList = draftModel.SelectedReviewer.Distinct()
                   .Select(s => new RIC_PersonalFeedbackDtl
                   {
                       RD_RevewerID = s,
                       RD_Status = "Released"
                   }).ToList();
            }
            reviewerList.Add(new RIC_PersonalFeedbackDtl()
            {
                RD_RevewerID = empCd,
                RD_Findings = draftModel.TeamLeadReview,
                RD_Improvements = draftModel.ImprovementsRequired,
                RD_Status = "Completed"
            });
            // save the review.
            var review = unitOfwork.RIC_PersonalFeedback.GetByID(draftModel.ReviewID);
            //delete old child details from entity.
            foreach (var child in review.RIC_PersonalFeedbackDtl.ToList())
                unitOfwork.RIC_PersonalFeedbackDtl.Delete(child);
            unitOfwork.Save();
            review.RP_NextReviewDate = draftModel.NextReviewDate.Value;
            review.RP_SubmissionTarget = draftModel.SubmissionTarget.Value;
            review.RP_InterviewTarget = draftModel.InterviewsTarget.Value;
            review.RP_HiresTarget = draftModel.HiresTarget.Value;
            review.RP_ReviewDate = usDate;
            review.RP_CallsTarget = draftModel.CallsTarget.Value;
            review.RP_DirectorID = draftModel.DirectorID;
            review.RIC_PersonalFeedbackDtl = reviewerList;
            if (submit == "SaveBtn")
            {
                review.RP_Status = draftModel.SelectedReviewer == null ? "Completed" : "InProgress";
                // add the notification for Reviewer.
                foreach (var pr in review.RIC_PersonalFeedbackDtl.Where(s => s.RD_RevewerID != empCd))
                {
                    unitOfwork.Notification.Insert(new RIC_Notification
                    {
                        Rn_ReviewID = review.RP_ID,
                        RN_EmpCd = pr.RD_RevewerID,
                        Date = usDate,
                        RN_IsSeen = false,
                        RN_NotificationType = "InterimReviewRequest",
                        RN_NotificationText = "Review submited by " + empCd
                    });
                }
                if (review.RP_Status == "Completed")
                {
                    unitOfwork.Notification.Insert(new RIC_Notification
                    {
                        Rn_ReviewID = review.RP_ID,
                        RN_EmpCd = review.RP_DirectorID,
                        Date = usDate,
                        RN_IsSeen = false,
                        RN_NotificationType = "InterimReviewRequest",
                        RN_NotificationText = "Review submited by " + empCd
                    });
                }
            }
            unitOfwork.RIC_PersonalFeedback.Update(review);
            //save the context.

            var GetEmployeeName = (from e in unitOfwork.RIC_Employee.Get()
                                   where e.RE_Emp_Cd == review.RP_EmployeeID
                                   select new
                                   {

                                       EmployeeName = e.RE_Employee_Name
                                   }).ToList();

            var GetRevewierName = (from e in unitOfwork.RIC_Employee.Get()
                                   where e.RE_Emp_Cd == review.RP_ReviewerID
                                   select new
                                   {
                                       ReviewName = e.RE_Employee_Name

                                   }).ToList();


            string error;
            string ReviewDetails = @"<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='viewport' content='width=device-width, initial-scale=1'><meta name='ProgId' content='Word.Document'></head><body style='font-family: Arial; font-size: 12px;'>
<div style='font-size: 14px; width: 50%; margin-left: 25%;box-sizing: unset;border-radius: 7px;border: 1px solid #c5c5c5!important;box-shadow: 3px 3px 10px #bfbfbf;margin-top: 5%;padding: 1%;'>
<h3>Interim Review Details</h3>
<span style='font-weight:bold'>Employee Name: </span><span>" + GetEmployeeName.FirstOrDefault().EmployeeName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Reviewer Name: </span><span>" + GetRevewierName.FirstOrDefault().ReviewName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Review Date: </span><span style='text-align:left'>" + review.RP_ReviewDate.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>From Date: </span><span style='text-align:left'>" + review.RP_FromDate.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>To Date: </span><span style='text-align:left'>" + review.RP_ToDate.ToString("dd/MM/yyyy") + "</span><br/><br/><p><span style='font-weight:bold'>Note:</span>This is an automated email message. Please do not reply to this email.<p></div></body></html>";

            Email.SendMail1(GetEmail(), "Interim Review Details", ReviewDetails, out error);
            unitOfwork.Save();
            return Content(("<script language='javascript' type='text/javascript'> alert('Review Submitted Successfully. ');window.location = '" + Url.Action("InterimDrafts") + "';</script>"));
        }

        [HttpGet]
        public ActionResult InterimReviewRequest()
        {
            string empCd = User.Identity.Name;
            //get the notification for user.
            var notification = unitOfwork.Notification.Get(s => s.RN_EmpCd == empCd && s.RN_IsSeen == false && s.RN_NotificationType == "InterimReviewRequest")
                      .ToList();
            //clear the interim review request.
            notification.ForEach(u => u.RN_IsSeen = true);
            unitOfwork.Save();
            RolePrincipal rolePrinciple = (RolePrincipal)User;
            //get the review requests..
            //List<InterimReviewRequestVM> requestModel =
            //    (from reviewDetail in unitOfwork.RIC_PersonalFeedbackDtl.Get()
            //     join review in unitOfwork.RIC_PersonalFeedback.Get()
            //     on reviewDetail.RD_ReviewID equals review.RP_ID
            //     join candidateName in unitOfwork.User.Get()
            //     on review.RP_EmployeeID equals candidateName.RE_Emp_Cd
            //     join issuedBy in unitOfwork.User.Get()
            //     on review.RP_ReviewerID equals issuedBy.RE_Emp_Cd
            //     where 
            //     (review.RP_Status == "InProgress" && reviewDetail.RD_Status == "Released" && reviewDetail.RD_RevewerID == empCd)
            //            || (review.RP_Status == "Completed" && reviewDetail.RD_RevewerID == review.RP_ReviewerID
            //               && ((review.RP_DirectorID == empCd && review.RP_DirectorReviewStatus == false) 
            //                || (review.RP_HrID == empCd && review.RP_Hr_ReviewStatus == false)))      
            //     select new InterimReviewRequestVM
            //     {
            //         ReviewID = reviewDetail.RD_ReviewID,
            //         ReviewDate = review.RP_ReviewDate,
            //         CndidateName = candidateName.RE_Jobdiva_User_Name,
            //         IssuedBy = issuedBy.RE_Jobdiva_User_Name,
            //         NextReviewDate = review.RP_NextReviewDate,
            //         NotificationCount=notification.Where(n=>n.Rn_ReviewID==reviewDetail.RD_ReviewID).Count(),
            //         ActionName = review.RP_ReviewerID == empCd ? "AddInterimReviewerFeedback" : review.RP_DirectorID == empCd ? "AddInterimDirectorFeedback" : review.RP_HrID == empCd ? "AddInterimHrFeedback" : "AddInterimReviewerFeedback"
            //     }).ToList();

            List<InterimReviewRequestVM> requestModel =
             (from reviewDetail in unitOfwork.RIC_PersonalFeedbackDtl.Get()
              join review in unitOfwork.RIC_PersonalFeedback.Get()
              on reviewDetail.RD_ReviewID equals review.RP_ID
              join candidateName in unitOfwork.User.Get()
              on review.RP_EmployeeID equals candidateName.RE_Emp_Cd
              join issuedBy in unitOfwork.User.Get()
              on review.RP_ReviewerID equals issuedBy.RE_Emp_Cd
              where reviewDetail.RD_RevewerID == empCd && reviewDetail.RD_Status == "Released"
              //(review.RP_Status == "InProgress" && reviewDetail.RD_Status == "Released" && reviewDetail.RD_RevewerID == empCd)
              //       || (review.RP_Status == "Completed" && reviewDetail.RD_RevewerID == review.RP_ReviewerID
              //          && ((review.RP_DirectorID == empCd && review.RP_DirectorReviewStatus == false)
              //           || (review.RP_HrID == empCd && review.RP_Hr_ReviewStatus == false)))
              select new InterimReviewRequestVM
              {
                  ReviewID = reviewDetail.RD_ReviewID,
                  ReviewDate = review.RP_ReviewDate,
                  CndidateName = candidateName.RE_Jobdiva_User_Name,
                  IssuedBy = issuedBy.RE_Jobdiva_User_Name,
                  NextReviewDate = review.RP_NextReviewDate,
                  NotificationCount = notification.Where(n => n.Rn_ReviewID == reviewDetail.RD_ReviewID).Count(),
                  ActionName = "AddInterimReviewerFeedback"
              }).Union(// get the reviews for director.
              from review in unitOfwork.RIC_PersonalFeedback.Get()
              join candidateName in unitOfwork.User.Get()
             on review.RP_EmployeeID equals candidateName.RE_Emp_Cd
              join issuedBy in unitOfwork.User.Get()
              on review.RP_ReviewerID equals issuedBy.RE_Emp_Cd
              where review.RP_DirectorID == empCd && review.RP_Status == "Completed" && review.RP_DirectorReviewStatus == false
              select new InterimReviewRequestVM
              {
                  ReviewID = review.RP_ID,
                  ReviewDate = review.RP_ReviewDate,
                  CndidateName = candidateName.RE_Jobdiva_User_Name,
                  IssuedBy = issuedBy.RE_Jobdiva_User_Name,
                  NextReviewDate = review.RP_NextReviewDate,
                  NotificationCount = notification.Where(n => n.Rn_ReviewID == review.RP_ID).Count(),
                  ActionName = "AddInterimDirectorFeedback"
              }
              // "AddInterimHrFeedback" : "AddInterimReviewerFeedback"
              ).Union(// get the reviews for HR.
              from review in unitOfwork.RIC_PersonalFeedback.Get()
              join candidateName in unitOfwork.User.Get()
             on review.RP_EmployeeID equals candidateName.RE_Emp_Cd
              join issuedBy in unitOfwork.User.Get()
              on review.RP_ReviewerID equals issuedBy.RE_Emp_Cd
              where review.RP_HrID == empCd && review.RP_Status == "Completed" && review.RP_Hr_ReviewStatus == false
              select new InterimReviewRequestVM
              {
                  ReviewID = review.RP_ID,
                  ReviewDate = review.RP_ReviewDate,
                  CndidateName = candidateName.RE_Jobdiva_User_Name,
                  IssuedBy = issuedBy.RE_Jobdiva_User_Name,
                  NextReviewDate = review.RP_NextReviewDate,
                  NotificationCount = notification.Where(n => n.Rn_ReviewID == review.RP_ID).Count(),
                  ActionName = "AddInterimHrFeedback"
              }).ToList();
            ViewBag.RedirectActionname = rolePrinciple.IsInRole(directorRoleName) ? "AddInterimDirectorFeedback" : rolePrinciple.IsInRole(HrRoleName) ? "AddInterimHrFeedback" : "AddInterimReviewerFeedback";
            return View(requestModel);
        }

        [HttpGet]
        public ActionResult AddInterimReviewerFeedback(int id, string retUrl)
        {
            //get the review by id.
            var review = unitOfwork.RIC_PersonalFeedback.GetByID(id);
            // get the candidate name from user table.
            string CandidateName = unitOfwork.User.GetByEmpID(review.RP_EmployeeID).RE_Jobdiva_User_Name;
            // bind the model.
            AddInterimReviewerFeedbackVM model = new AddInterimReviewerFeedbackVM
            {
                ReviewID = review.RP_ID,
                CandidateName = CandidateName,
                FromDate = review.RP_FromDate,
                ToDate = review.RP_ToDate,
                Submissions = review.RP_TotalSubmissions,
                Interviews = review.RP_TotalInterview,
                Hires = review.RP_TotalHires,
                Calls = review.RP_TotalCalls,
                NextReviewDate = review.RP_NextReviewDate,
                TargetForNextReview = review.RP_SubmissionTarget + " / " + review.RP_InterviewTarget + " / " + review.RP_HiresTarget + " / " + review.RP_CallsTarget,
                // DirectorReview = review.RR_DirectorFeedback
            };
            ViewBag.retUrl = retUrl;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddInterimReviewerFeedback(AddInterimReviewerFeedbackVM reviewModel)
        {
            string empCd = User.Identity.Name;
            var reviewDtl = unitOfwork.RIC_PersonalFeedbackDtl
                .Get(s => s.RD_ReviewID == reviewModel.ReviewID && s.RD_RevewerID == empCd).First();
            //add the Director feedback in context.
            reviewDtl.RD_Improvements = reviewModel.Improvements;
            reviewDtl.RD_Findings = reviewModel.Findings;
            reviewDtl.RD_Status = "Completed";
            unitOfwork.RIC_PersonalFeedbackDtl.Update(reviewDtl);
            unitOfwork.Save();



            var GetEmployeeName = (from e in unitOfwork.RIC_Employee.Get()
                                   where e.RE_Emp_Cd == reviewDtl.RIC_PersonalFeedback.RP_EmployeeID
                                   select new
                                   {

                                       EmployeeName = e.RE_Employee_Name
                                   }).ToList();

            var GetRevewierName = (from e in unitOfwork.RIC_Employee.Get()
                                   where e.RE_Emp_Cd == reviewDtl.RIC_PersonalFeedback.RP_ReviewerID
                                   select new
                                   {
                                       ReviewName = e.RE_Employee_Name

                                   }).ToList();

            var GetRevewierFeedbackName = (from e in unitOfwork.RIC_Employee.Get()
                                           where e.RE_Emp_Cd == reviewDtl.RD_RevewerID
                                           select new
                                           {
                                               ReviewName = e.RE_Employee_Name

                                           }).ToList();


            string error;
            string ReviewDetails = @"<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='viewport' content='width=device-width, initial-scale=1'><meta name='ProgId' content='Word.Document'></head><body style='font-family: Arial; font-size: 12px;'>
<div style='font-size: 14px; width: 50%; margin-left: 25%;box-sizing: unset;border-radius: 7px;border: 1px solid #c5c5c5!important;box-shadow: 3px 3px 10px #bfbfbf;margin-top: 5%;padding: 1%;'>
<h3>Interim Review Feedback Details</h3>
<span style='font-weight:bold'>Employee Name: </span><span>" + GetEmployeeName.FirstOrDefault().EmployeeName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Issued By: </span><span>" + GetRevewierName.FirstOrDefault().ReviewName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Reviewer Name: </span><span>" + GetRevewierFeedbackName.FirstOrDefault().ReviewName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Review Date: </span><span style='text-align:left'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>From Date: </span><span style='text-align:left'>" + reviewDtl.RIC_PersonalFeedback.RP_FromDate.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>To Date: </span><span style='text-align:left'>" + reviewDtl.RIC_PersonalFeedback.RP_ToDate.ToString("dd/MM/yyyy") + "</span><br/><br/><p><span style='font-weight:bold'>Note:</span>This is an automated email message. Please do not reply to this email.<p></div></body></html>";

            Email.SendMail1(GetEmail(), "Interim Review Feedback Details", ReviewDetails, out error);



            var review = unitOfwork.RIC_PersonalFeedback.GetByID(reviewModel.ReviewID);
            if (review.RIC_PersonalFeedbackDtl.Where(s => s.RD_Status == "Released").Count() == 0)
            {
                //send the notification to director.
                unitOfwork.Notification.Insert(new RIC_Notification
                {
                    Rn_ReviewID = review.RP_ID,
                    RN_EmpCd = review.RP_DirectorID,
                    Date = usDate,
                    RN_IsSeen = false,
                    RN_NotificationType = "InterimReviewRequest",
                    RN_NotificationText = "Review submited by " + empCd
                });
                //set the status of review to completed.
                review.RP_Status = "Completed";
            }
            // send the notification to reviewer.
            unitOfwork.Notification.Insert(new RIC_Notification
            {
                Rn_ReviewID = review.RP_ID,
                RN_EmpCd = review.RP_ReviewerID,
                Date = usDate,
                RN_IsSeen = false,
                RN_NotificationType = "InterimReviewUpdate",
                RN_NotificationText = "Review submited by " + empCd
            });
            unitOfwork.Save();
            return Content(("<script language='javascript' type='text/javascript'> alert('Review Submitted Successfully. ');window.location = '" + Url.Action("InterimReviewSubmitted") + "';</script>"));
        }

        [HttpGet]
        public ActionResult AddInterimDirectorFeedback(int id, string ReturnUrl)
        {
            var review = unitOfwork.RIC_PersonalFeedback.GetByID(id);
            var model = new InterimReviewDirectorFeedbackVM();
            model.CandidateName = unitOfwork.User.GetByEmpID(review.RP_EmployeeID).RE_Jobdiva_User_Name;
            model.ReviewerName = unitOfwork.User.GetByEmpID(review.RP_ReviewerID).RE_Jobdiva_User_Name;
            model.FromDate = review.RP_FromDate;
            model.ToDate = review.RP_ToDate;
            model.Submissions = review.RP_TotalSubmissions;
            model.Interviews = review.RP_TotalInterview;
            model.Hires = review.RP_TotalHires;
            model.Calls = review.RP_TotalCalls;
            model.NextReviewDate = review.RP_NextReviewDate;
            model.SubmissionTarget = review.RP_SubmissionTarget;
            model.InterviewTarget = review.RP_InterviewTarget;
            model.HireTarget = review.RP_HiresTarget;
            model.CallsTarget = review.RP_CallsTarget;
            model.ReviewID = id;
            model.ReviewerDtl = review.RIC_PersonalFeedbackDtl.Select(s => new InterimReviewerDtl
            {
                ReviewerName = unitOfwork.User.GetByEmpID(s.RD_RevewerID).RE_Jobdiva_User_Name,
                Findings = s.RD_Findings,
                Improvement = s.RD_Improvements,
                Status = s.RD_Status
            }).ToList();
            ReviewUtitity utility = new ReviewUtitity();
            model.HrList = utility.GetHrLsit();
            ViewBag.retUrl = ReturnUrl;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddInterimDirectorFeedback(InterimReviewDirectorFeedbackVM InterimReviewDirector, string ReviewerNames)
        {
            if (ModelState.IsValid)
            {
                var review = unitOfwork.RIC_PersonalFeedback.GetByID(InterimReviewDirector.ReviewID);
                review.RP_DirectorFeedback = InterimReviewDirector.DirectorFeedback;
                review.RP_ForwardToHr = InterimReviewDirector.ForwardToHR;
                review.RP_HrID = InterimReviewDirector.HrID;
                review.RP_DirectorReviewStatus = true;

                // send the notification to HR.
                if (review.RP_ForwardToHr)
                {
                    unitOfwork.Notification.Insert(new RIC_Notification
                    {
                        Rn_ReviewID = review.RP_ID,
                        RN_EmpCd = review.RP_HrID,
                        Date = usDate,
                        RN_IsSeen = false,
                        RN_NotificationType = "InterimReviewRequest",
                        RN_NotificationText = "Review submited by "
                    });
                }
                unitOfwork.Save();

                var GetEmployeeName = (from e in unitOfwork.RIC_Employee.Get()
                                       where e.RE_Emp_Cd == review.RP_EmployeeID
                                       select new
                                       {

                                           EmployeeName = e.RE_Employee_Name
                                       }).ToList();

                var GetDirectorName = (from e in unitOfwork.RIC_Employee.Get()
                                       where e.RE_Emp_Cd == review.RP_DirectorID
                                       select new
                                       {

                                           DirectorName = e.RE_Employee_Name
                                       }).ToList();

                var GetIssuedBy = (from e in unitOfwork.RIC_Employee.Get()
                                   where e.RE_Emp_Cd == review.RP_ReviewerID
                                   select new
                                   {

                                       IssuedBy = e.RE_Employee_Name
                                   }).ToList();

                string error;
                string ReviewDetails = @"<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='viewport' content='width=device-width, initial-scale=1'><meta name='ProgId' content='Word.Document'></head><body style='font-family: Arial; font-size: 12px;'>
<div style='font-size: 14px; width: 50%; margin-left: 25%;box-sizing: unset;border-radius: 7px;border: 1px solid #c5c5c5!important;box-shadow: 3px 3px 10px #bfbfbf;margin-top: 5%;padding: 1%;'>
<h3>Interim Review Feedback Details</h3>
<span style='font-weight:bold'>Employee Name: </span><span>" + GetEmployeeName.FirstOrDefault().EmployeeName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Issued By : </span><span>" + GetIssuedBy.FirstOrDefault().IssuedBy.ToString() + "</span><br/><br/><span style='font-weight:bold'>Review Director : </span><span>" + GetDirectorName.FirstOrDefault().DirectorName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Reviewer Names: </span><span>" + ReviewerNames + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Review Date: </span><span style='text-align:left'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>From Date: </span><span style='text-align:left'>" + review.RP_FromDate.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>To Date: </span><span style='text-align:left'>" + review.RP_ToDate.ToString("dd/MM/yyyy") + "</span><br/><br/><p><span style='font-weight:bold'>Note:</span>This is an automated email message. Please do not reply to this email.<p></div></body></html>";

                Email.SendMail1(GetEmail(), "Interim Review Feedback Details", ReviewDetails, out error);






                return Content(("<script language='javascript' type='text/javascript'> alert('Review Submitted Successfully. ');window.location = '" + Url.Action("InterimReviewSubmitted") + "';</script>"));
            }
            return RedirectToAction("AddInterimDirectorFeedback");
        }

        [HttpGet]
        public ActionResult AddInterimHrFeedback(int id, string ReturnUrl)
        {
            var review = unitOfwork.RIC_PersonalFeedback.GetByID(id);
            var model = new HrInterimReviewModel();
            model.CandidateName = unitOfwork.User.GetByEmpID(review.RP_EmployeeID).RE_Jobdiva_User_Name;
            model.ReviewerName = unitOfwork.User.GetByEmpID(review.RP_ReviewerID).RE_Jobdiva_User_Name;
            model.FromDate = review.RP_FromDate;
            model.ToDate = review.RP_ToDate;
            model.Submissions = review.RP_TotalSubmissions;
            model.Interviews = review.RP_TotalInterview;
            model.Hires = review.RP_TotalHires;
            model.Calls = review.RP_TotalCalls;
            model.NextReviewDate = review.RP_NextReviewDate;
            model.SubmissionTarget = review.RP_SubmissionTarget;
            model.InterviewTarget = review.RP_InterviewTarget;
            model.HireTarget = review.RP_HiresTarget;
            model.CallsTarget = review.RP_CallsTarget;
            model.DirectorName = unitOfwork.User.GetByEmpID(review.RP_DirectorID).RE_Jobdiva_User_Name;
            model.DirectorFeedback = review.RP_DirectorFeedback;
            model.ReviewID = id;
            model.ReviewerDtl = review.RIC_PersonalFeedbackDtl.Select(s => new InterimReviewerDtl
            {
                ReviewerName = unitOfwork.User.GetByEmpID(s.RD_RevewerID).RE_Jobdiva_User_Name,
                Findings = s.RD_Findings,
                Improvement = s.RD_Improvements,
                Status = s.RD_Status
            }).ToList();

            ViewBag.retUrl = ReturnUrl;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddInterimHrFeedback(HrInterimReviewModel reviewModel, string ReviewerNames)
        {
            var review = unitOfwork.RIC_PersonalFeedback.GetByID(reviewModel.ReviewID);
            if (ModelState.IsValid)
            {
                //add the HR feedback in context.
                review.RP_HrFeedback = reviewModel.HrFeedback;
                review.RP_Hr_ReviewStatus = true;
                // send the notification to HR.
                unitOfwork.Notification.Insert(new RIC_Notification
                {
                    Rn_ReviewID = review.RP_ID,
                    RN_EmpCd = review.RP_DirectorID,
                    Date = usDate,
                    RN_IsSeen = false,
                    RN_NotificationType = "InterimReviewUpdate",
                    RN_NotificationText = "Review submited by "
                });
                unitOfwork.Save();

                var GetEmployeeName = (from e in unitOfwork.RIC_Employee.Get()
                                       where e.RE_Emp_Cd == review.RP_EmployeeID
                                       select new
                                       {

                                           EmployeeName = e.RE_Employee_Name
                                       }).ToList();

                var GetDirectorName = (from e in unitOfwork.RIC_Employee.Get()
                                       where e.RE_Emp_Cd == review.RP_DirectorID
                                       select new
                                       {

                                           DirectorName = e.RE_Employee_Name
                                       }).ToList();

                var GetIssuedBy = (from e in unitOfwork.RIC_Employee.Get()
                                   where e.RE_Emp_Cd == review.RP_ReviewerID
                                   select new
                                   {

                                       IssuedBy = e.RE_Employee_Name
                                   }).ToList();
                var GetHrName = (from e in unitOfwork.RIC_Employee.Get()
                                 where e.RE_Emp_Cd == review.RP_HrID
                                 select new
                                 {

                                     HrName = e.RE_Employee_Name
                                 }).ToList();


                string error;
                string ReviewDetails = @"<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='viewport' content='width=device-width, initial-scale=1'><meta name='ProgId' content='Word.Document'></head><body style='font-family: Arial; font-size: 12px;'>
<div style='font-size: 14px; width: 50%; margin-left: 25%;box-sizing: unset;border-radius: 7px;border: 1px solid #c5c5c5!important;box-shadow: 3px 3px 10px #bfbfbf;margin-top: 5%;padding: 1%;'>
<h3>Interim Review Feedback Details</h3>
<span style='font-weight:bold'>Employee Name: </span><span>" + GetEmployeeName.FirstOrDefault().EmployeeName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Issued By : </span><span>" + GetIssuedBy.FirstOrDefault().IssuedBy.ToString() + "</span><br/><br/><span style='font-weight:bold'>Review Director : </span><span>" + GetDirectorName.FirstOrDefault().DirectorName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Review Hr: </span><span>" + GetHrName.FirstOrDefault().HrName.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Reviewer Names: </span><span>" + ReviewerNames + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Review Date: </span><span style='text-align:left'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>From Date: </span><span style='text-align:left'>" + review.RP_FromDate.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>To Date: </span><span style='text-align:left'>" + review.RP_ToDate.ToString("dd/MM/yyyy") + "</span><br/><br/><p><span style='font-weight:bold'>Note:</span>This is an automated email message. Please do not reply to this email.<p></div></body></html>";

                Email.SendMail1(GetEmail(), "Interim Review Feedback Details", ReviewDetails, out error);


                return Content(("<script language='javascript' type='text/javascript'> alert('Review Submitted Successfully. ');window.location = '" + Url.Action("InterimReviewSubmitted") + "';</script>"));
            }
            else
            {
                reviewModel.CandidateName = unitOfwork.User.GetByEmpID(review.RP_EmployeeID).RE_Jobdiva_User_Name;
                reviewModel.ReviewerName = unitOfwork.User.GetByEmpID(review.RP_ReviewerID).RE_Jobdiva_User_Name;
                reviewModel.FromDate = review.RP_FromDate;
                reviewModel.ToDate = review.RP_ToDate;
                reviewModel.Submissions = review.RP_TotalSubmissions;
                reviewModel.Interviews = review.RP_TotalInterview;
                reviewModel.Hires = review.RP_TotalHires;
                reviewModel.Calls = review.RP_TotalCalls;
                reviewModel.NextReviewDate = review.RP_NextReviewDate;
                reviewModel.SubmissionTarget = review.RP_SubmissionTarget;
                reviewModel.InterviewTarget = review.RP_InterviewTarget;
                reviewModel.HireTarget = review.RP_HiresTarget;
                reviewModel.CallsTarget = review.RP_CallsTarget;
                reviewModel.DirectorName = unitOfwork.User.GetByEmpID(review.RP_DirectorID).RE_Jobdiva_User_Name;
                reviewModel.DirectorFeedback = review.RP_DirectorFeedback;
                reviewModel.ReviewID = review.RP_ID;
                reviewModel.ReviewerDtl = review.RIC_PersonalFeedbackDtl.Select(s => new InterimReviewerDtl
                {
                    ReviewerName = unitOfwork.User.GetByEmpID(s.RD_RevewerID).RE_Jobdiva_User_Name,
                    Findings = s.RD_Findings,
                    Improvement = s.RD_Improvements,
                    Status = s.RD_Status
                }).ToList();
                return View(reviewModel);
            }
            //  unitOfwork.Review.Update(review);
            // add the notification for Manager.
            //unitOfwork.Notification.Insert(new RIC_Notification
            //{
            //    Rn_ReviewID = review.RR_Id,
            //    RN_EmpCd = review.RR_MgrCd,
            //    Date = usDate
            //});
            //unitOfwork.Save();

        }

        public ActionResult Details(int id, string retUrl)
        {
            var review = unitOfwork.RIC_PersonalFeedback.GetByID(id);
            var model = new InterimReviewDetailsModel();
            model.CandidateName = unitOfwork.User.GetByEmpID(review.RP_EmployeeID).RE_Jobdiva_User_Name;
            model.ReviewerName = unitOfwork.User.GetByEmpID(review.RP_ReviewerID).RE_Jobdiva_User_Name;
            model.FromDate = review.RP_FromDate;
            model.ToDate = review.RP_ToDate;
            model.ReviewDate = review.RP_ReviewDate;
            model.Submissions = review.RP_TotalSubmissions;
            model.Interviews = review.RP_TotalInterview;
            model.Hires = review.RP_TotalHires;
            model.Calls = review.RP_TotalCalls;
            model.NextReviewDate = review.RP_NextReviewDate;
            model.SubmissionTarget = review.RP_SubmissionTarget;
            model.InterviewTarget = review.RP_InterviewTarget;
            model.HireTarget = review.RP_HiresTarget;
            model.CallsTarget = review.RP_CallsTarget;
            model.ReviewerDtl = review.RIC_PersonalFeedbackDtl.Select(s => new InterimReviewerDtl
            {
                ReviewerName = unitOfwork.User.GetByEmpID(s.RD_RevewerID).RE_Jobdiva_User_Name,
                Findings = s.RD_Findings,
                Improvement = s.RD_Improvements,
                Status = s.RD_Status
            }).ToList();

            //get the roles for user.
            RolePrincipal rolePrinciple = (RolePrincipal)User;
            model.ShowHrReview = (rolePrinciple.GetRoles()[0] == directorRoleName
                                || rolePrinciple.GetRoles()[0] == stafingDirectorRole
                                || rolePrinciple.GetRoles()[0] == HrRoleName) && review.RP_Hr_ReviewStatus;
            model.ShowDirectorReview = (rolePrinciple.GetRoles()[0] == directorRoleName
                                || rolePrinciple.GetRoles()[0] == stafingDirectorRole
                                || rolePrinciple.GetRoles()[0] == HrRoleName) && review.RP_DirectorReviewStatus;
            // add the hr and director review.
            if (review.RP_DirectorReviewStatus)
            {
                model.DirectorName = unitOfwork.User.GetByEmpID(review.RP_DirectorID).RE_Jobdiva_User_Name;
                model.DirectorComments = review.RP_DirectorFeedback;
            }
            if (review.RP_Hr_ReviewStatus)
            {
                model.HrName = unitOfwork.User.GetByEmpID(review.RP_HrID).RE_Jobdiva_User_Name;
                model.HrComments = review.RP_HrFeedback;
            }
            //set the return Url.
            ViewBag.retUrl = retUrl;
            return View(model);
        }

        [HttpGet]
        public ActionResult EditInterimReview(int id, string retUrl)
        {
            //get the review by id.
            var review = unitOfwork.RIC_PersonalFeedback.GetByID(id);
            //bind the review model.
            EditInterimReviewVM EditInterimReviewVM = new EditInterimReviewVM()
            {
                ReviewID = review.RP_ID,
                EmployeeName = unitOfwork.User.GetByEmpID(review.RP_EmployeeID).RE_Jobdiva_User_Name,
                ReviewerName = unitOfwork.User.GetByEmpID(review.RP_ReviewerID).RE_Jobdiva_User_Name,
                ReviewDate = review.RP_ReviewDate,
                FromDate = review.RP_FromDate,
                ToDate = review.RP_ToDate,
                NextReviewDate = review.RP_NextReviewDate,
                SubmissionAchivement = review.RP_TotalSubmissions,
                InterviewAchivement = review.RP_TotalInterview,
                HiresAchivements = review.RP_TotalHires,
                SubmissionTarget = review.RP_SubmissionTarget,
                InterviewTarget = review.RP_InterviewTarget,
                HiresTarget = review.RP_HiresTarget,
                CallsTarget = review.RP_CallsTarget,
                DirectorReviewStatus = review.RP_DirectorReviewStatus,
                DirectorName = unitOfwork.User.GetByEmpID(review.RP_DirectorID).RE_Jobdiva_User_Name,
                DirectorComments = review.RP_DirectorFeedback,
                HrReviewStatus = review.RP_Hr_ReviewStatus,
                HrName = review.RP_ForwardToHr ? unitOfwork.User.GetByEmpID(review.RP_HrID).RE_Jobdiva_User_Name : null,
                HrComments = review.RP_HrFeedback,
                LastUpdatedBy = review.RP_UpdatedBy,
                LastUpdatedDate = review.RP_UpdatedDate,
                ReviewerList = review.RIC_PersonalFeedbackDtl.Where(dtl => dtl.RD_Status == "Completed").Select(s => new EditReviewersDetails
                {
                    Id = s.RD_ID,
                    EmployeeName = unitOfwork.User.GetByEmpID(s.RD_RevewerID).RE_Jobdiva_User_Name,
                    Findings = s.RD_Findings,
                    ImprovementsRequired = s.RD_Improvements
                }).ToList()
            };
            ViewBag.retUrl = retUrl;
            return View(EditInterimReviewVM);
        }
        public ActionResult EditInterimReview(EditInterimReviewVM editInterimReviewVM, string ReviewerNames)
        {

            string empCd = User.Identity.Name;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            string role = r.GetRoles()[0];

            if (ModelState.IsValid)
            {
                var review = unitOfwork.RIC_PersonalFeedback.GetByID(editInterimReviewVM.ReviewID);
                review.RP_DirectorFeedback = editInterimReviewVM.DirectorComments;
                review.RP_HrFeedback = editInterimReviewVM.HrComments;
                review.RP_SubmissionTarget = editInterimReviewVM.SubmissionTarget;
                review.RP_InterviewTarget = editInterimReviewVM.InterviewTarget;
                review.RP_HiresTarget = editInterimReviewVM.HiresTarget;
                review.RP_CallsTarget = editInterimReviewVM.CallsTarget;
                foreach (var item in editInterimReviewVM.ReviewerList)
                {
                    var reviewDetails = review.RIC_PersonalFeedbackDtl.FirstOrDefault(s => s.RD_ID == item.Id);
                    reviewDetails.RD_Findings = item.Findings;
                    reviewDetails.RD_Improvements = item.ImprovementsRequired;
                }
                review.RP_UpdatedBy = unitOfwork.User.GetByEmpID(empCd).RE_Jobdiva_User_Name;
                review.RP_UpdatedDate = usDate;
                unitOfwork.Save();
                var GetEmployeeName = (from e in unitOfwork.RIC_Employee.Get()
                                       where e.RE_Emp_Cd == review.RP_EmployeeID
                                       select new
                                       {

                                           EmployeeName = e.RE_Employee_Name
                                       }).ToList();

                var GetDirectorName = (from e in unitOfwork.RIC_Employee.Get()
                                       where e.RE_Emp_Cd == review.RP_DirectorID
                                       select new
                                       {

                                           DirectorName = e.RE_Employee_Name
                                       }).ToList();

                var GetIssuedBy = (from e in unitOfwork.RIC_Employee.Get()
                                   where e.RE_Emp_Cd == review.RP_ReviewerID
                                   select new
                                   {

                                       IssuedBy = e.RE_Employee_Name
                                   }).ToList();


                var GetHrName = (from e in unitOfwork.RIC_Employee.Get()
                                 where e.RE_Emp_Cd == review.RP_HrID
                                 select new
                                 {

                                     Hrname = e.RE_Employee_Name
                                 }).ToList();



                string error;
                string ReviewDetails;
                if (role == "Director")
                {

                    ReviewDetails = @"<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='viewport' content='width=device-width, initial-scale=1'><meta name='ProgId' content='Word.Document'></head><body style='font-family: Arial; font-size: 12px;'>
<div style='font-size: 14px; width: 50%; margin-left: 25%;box-sizing: unset;border-radius: 7px;border: 1px solid #c5c5c5!important;box-shadow: 3px 3px 10px #bfbfbf;margin-top: 5%;padding: 1%;'>
<h3>Interim Review Feedback Update Details</h3>
<span style='font-weight:bold'>Employee Name: </span><span>" + GetEmployeeName.FirstOrDefault().EmployeeName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Issued By : </span><span>" + GetIssuedBy.FirstOrDefault().IssuedBy.ToString() + "</span><br/><br/><span style='font-weight:bold'>Review Director : </span><span>" + GetDirectorName.FirstOrDefault().DirectorName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Review Hr : </span><span>" + ReviewerNames.Split('?')[1].ToString() + "</span><br/><br/><span></span><span style='text-align:left;font-weight:bold'>Reviewer Names: </span><span>" + ReviewerNames.Split('?')[0].ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Review Date: </span><span style='text-align:left'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>From Date: </span><span style='text-align:left'>" + review.RP_FromDate.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>To Date: </span><span style='text-align:left'>" + review.RP_ToDate.ToString("dd/MM/yyyy") + "</span><br/><br/><p><span style='font-weight:bold'>Note:</span>This is an automated email message. Please do not reply to this email.<p></div></body></html>";

                    ReviewDetails = ReviewerNames.Split('?')[1].ToString() == "" ? ReviewDetails.Replace("<br/><br/><span style='font-weight:bold'>Review Hr : </span><span></span>", "") : ReviewDetails;

                }
                else
                {

                    ReviewDetails = @"<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='viewport' content='width=device-width, initial-scale=1'><meta name='ProgId' content='Word.Document'></head><body style='font-family: Arial; font-size: 12px;'>
<div style='font-size: 14px; width: 50%; margin-left: 25%;box-sizing: unset;border-radius: 7px;border: 1px solid #c5c5c5!important;box-shadow: 3px 3px 10px #bfbfbf;margin-top: 5%;padding: 1%;'>
<h3>Interim Review Feedback Update Details</h3>
<span style='font-weight:bold'>Employee Name: </span><span>" + GetEmployeeName.FirstOrDefault().EmployeeName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Issued By : </span><span>" + GetIssuedBy.FirstOrDefault().IssuedBy.ToString() + "</span><br/><br/><span style='font-weight:bold'>Review Director : </span><span>" + GetDirectorName.FirstOrDefault().DirectorName.ToString() + "</span><br/><br/><span style='font-weight:bold'>Review Hr : </span><span>" + GetHrName.FirstOrDefault().Hrname.ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Reviewer Names: </span><span>" + ReviewerNames.Split('?')[0].ToString() + "</span><br/><br/><span style='text-align:left;font-weight:bold'>Review Date: </span><span style='text-align:left'>" + DateTime.Now.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>From Date: </span><span style='text-align:left'>" + review.RP_FromDate.ToString("dd/MM/yyyy") + "</span><br/><br/><span style='text-align:left;font-weight:bold'>To Date: </span><span style='text-align:left'>" + review.RP_ToDate.ToString("dd/MM/yyyy") + "</span><br/><br/><p><span style='font-weight:bold'>Note:</span>This is an automated email message. Please do not reply to this email.<p></div></body></html>";

                }


                Email.SendMail1(GetEmail(), "Interim Review Feedback Update Details", ReviewDetails, out error);

                return Content(("<script language='javascript' type='text/javascript'> alert('Review Submitted Successfully. ');window.location = '" + Url.Action("InterimReviewSubmitted") + "';</script>"));
            }
            return View(editInterimReviewVM);
        }

        public JsonResult GetReviewerList()
        {
            ReviewUtitity reviewUtility = new ReviewUtitity();
            var reviewerList = reviewUtility.GetReviewerList().Where(s => s.Value != User.Identity.Name);

            return Json(reviewerList, JsonRequestBehavior.AllowGet);

        }

        [NonAction]
        public List<SelectListItem> GetEmployeeList()
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            RolePrincipal rolePrinciple = (RolePrincipal)User;
            AddInterimReviewModel reviewModel = new AddInterimReviewModel();
            string role = rolePrinciple.GetRoles()[0];
            List<string> roles = new List<string> { empRoleName, tlRoleName, mgrRoleName };
            DateTime date = usDate.Date;
            // get the list fo employee.
            return unitOfwork.User.getReportingHistory(empCd, date, date, role)
                    .Select(s => new SelectListItem
                    {
                        Text = s.Employee_Name,
                        Value = s.RR_EmpCD
                    }).ToList();
            //return  unitOfwork.User.getAllUsers()
            //    .Where(w => (w.MgrCD == empCd || rolePrinciple.IsInRole(directorRoleName)
            //        ||rolePrinciple.IsInRole(HrRoleName))
            //            && roles.Contains(w.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name))
            //    .Select(s => new SelectListItem
            //    {ss
            //        Text = s.RE_Jobdiva_User_Name,
            //        Value = s.RE_Emp_Cd
            //    }).OrderBy(o=>o.Text).ToList();          
        }
        #endregion

        #region
        [HttpGet]
        public ActionResult EditReview(int id, string retUrl)
        {
            //edit the review...
            var review = unitOfwork.Review.GetByID(id);
            string CandidateName = unitOfwork.User.GetByEmpID(review.RR_EmpCd).RE_Jobdiva_User_Name;
            EditReviewModel model = new EditReviewModel
            {
                ReviewID = review.RR_Id,
                CandidateName = CandidateName,
                FromDate = review.RR_FromDate,
                ToDate = review.RR_ToDate,
                Submissions = review.RR_Submissions,
                Interviews = review.RR_Interviews,
                Hires = review.RR_Hires,
                Calls = review.RR_Calls,
                TeamLeadReview = review.RR_TlFindings,
                ImprovementsRequired = review.RR_Improvements,
                DirectorFeedback = review.RR_DirectorFeedbackRequired == true ? "Yes" : "No",
                HrFeedback = review.RR_HrFeedbackRequired == true ? "Yes" : "No",
                NextReviewDate = review.RR_NextReviewDate,
                TargetForNextReview = review.RR_SubmissionTarget + " / " + review.RR_InterviewTarget + " / " + review.RR_HiresTarget + " / " + review.RR_CallsTarget,
                AdditionalComments = review.RR_AdditionalComments
            };
            ViewBag.retUrl = retUrl;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditReview(EditReviewModel reviewModel)
        {
            var review = unitOfwork.Review.GetByID(reviewModel.ReviewID);
            //add the additional comments in context.
            review.RR_AdditionalComments = reviewModel.AdditionalComments;
            unitOfwork.Review.Update(review);
            unitOfwork.Save();
            return Content(("<script language='javascript' type='text/javascript'> alert('Review Submitted Successfully. ');window.location = '" + Url.Action("ViewAllReviews") + "';</script>"));
        }
        #endregion


        [NonAction]
        public static string[] GetEmail()
        {
            var Emailsent = EmailTo.Split(',');

            string[] mail = new string[Emailsent.Count()];

            int count = 0;

            foreach (var items in Emailsent)
            {
                mail[count++] = items;
            }

            return mail;

        }


        #region HR Discussion

        [NonAction]
        public List<SelectListItem> GetAdendaTypeList()
        {

            //agenda type lst
            List<SelectListItem> _lstAgendaType = new List<SelectListItem>();

            _lstAgendaType.Add(new SelectListItem() { Text = "7 Days Feedback", Value = "7 Days Feedback" });
            _lstAgendaType.Add(new SelectListItem() { Text = "30 Days Feedback", Value = "30 Days Feedback" });
            _lstAgendaType.Add(new SelectListItem() { Text = "Employee Query Discussion", Value = "Employee Query Discussion" });
            _lstAgendaType.Add(new SelectListItem() { Text = "Others", Value = "Others" });

            return _lstAgendaType;

        }

        [HttpGet]
        public ActionResult HRDiscussionSendInvite()
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            RolePrincipal rolePrinciple = (RolePrincipal)User;
            string role = rolePrinciple.GetRoles()[0];

            DateTime date = usDate.Date;
            // get the list fo employee.
            var employeeList = (from reportingList in unitOfwork.User.getReportingHistory(empCd, date, date, role)
                                join empList in unitOfwork.User.Get() on reportingList.RR_EmpCD.ToString() equals empList.RE_Emp_Cd.ToString()
                                select new HRDiscussionSendInvite
                                {
                                    CandidateName = reportingList.Employee_Name,
                                    EmpCd = reportingList.RR_EmpCD,
                                    ManagerName = reportingList.Mgr_Name,
                                    EmployeePrimaryId = empList.RE_EmpId
                                }).ToList();
            return View(employeeList);
        }




        public ActionResult HRDiscussionSubmitted()
        {

            List<HRDiscussionSubmittedModel> discussionModel =
                (from hrDiscussionDetails in unitOfwork.HRDiscussionDtl.Get()
                 join hrDicussion in unitOfwork.HRDiscussion.Get()
                      on hrDiscussionDetails.RHD_RefID equals hrDicussion.RH_ID
                 join employee in unitOfwork.User.Get()
                    on hrDicussion.RH_EmployeeID equals employee.RE_Emp_Cd
                 join reportingTo in unitOfwork.User.Get()
                    on hrDicussion.RH_ReviewerID equals reportingTo.RE_Emp_Cd
                 where hrDicussion.RH_Status == "Submitted"
                 select new HRDiscussionSubmittedModel
                 {
                     DiscussionId = hrDicussion.RH_ID,
                     DiscussionDate = hrDicussion.RH_ReviewDate,
                     CandidateName = employee.RE_Employee_Name,
                     ManagerName = reportingTo.RE_Employee_Name,
                     AgendaType = hrDicussion.RH_AgendaType
                 }).ToList();



            return View(discussionModel);
        }

        public ActionResult HRDiscussionDrafts()
        {
            List<HRDiscussionSubmittedModel> discussionModel =
                (from hrDicussionDetails in unitOfwork.HRDiscussionDtl.Get()
                 join hrDicussion in unitOfwork.HRDiscussion.Get()
                      on hrDicussionDetails.RHD_RefID equals hrDicussion.RH_ID
                 join employee in unitOfwork.User.Get()
                    on hrDicussion.RH_EmployeeID equals employee.RE_Emp_Cd
                 join reportingTo in unitOfwork.User.Get()
                    on hrDicussion.RH_ReviewerID equals reportingTo.RE_Emp_Cd
                 where hrDicussion.RH_Status == "Draft"
                 select new HRDiscussionSubmittedModel
                 {
                     DiscussionId = hrDicussion.RH_ID,
                     DiscussionDate = hrDicussion.RH_ReviewDate,
                     CandidateName = employee.RE_Employee_Name,
                     ManagerName = reportingTo.RE_Employee_Name,
                     AgendaType = hrDicussion.RH_AgendaType
                 }).ToList();



            return View(discussionModel);
        }

        [HttpGet]
        public ActionResult AddHRDiscussion()
        {
            AddHRDicussionModel hrDiscussionModel = new AddHRDicussionModel();
            // get the list fo employee.
            hrDiscussionModel.CandidateList = GetEmployeeList();
            hrDiscussionModel.AgendaTypeList = GetAdendaTypeList();

            return View(hrDiscussionModel);

        }

        [HttpPost]
        public ActionResult AddHRDiscussion(AddHRDicussionModel hrDiscussionModel, string GetDatabtnDraft)
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            var discussionList = new List<RIC_HRDiscussion_Dtl>();

            // add the discussion.
            discussionList.Add(new RIC_HRDiscussion_Dtl
            {
                RHD_Description = hrDiscussionModel.Description,
                RHD_DiscussioNotes = hrDiscussionModel.DiscussionNotes,
                RHD_ActionItems = hrDiscussionModel.ActionItems,
                RHD_NextMeetUpDate = hrDiscussionModel.NextMeetUpDate,
                RHD_InternalCommnets = hrDiscussionModel.HRInternalComments

            });
            //save data in database.


            var hrDiscussion = new RIC_HRDiscussion_Hdr
            {
                RH_EmployeeID = hrDiscussionModel.EmpCd,
                RH_ReviewerID = empCd,
                RH_ReviewDate = hrDiscussionModel.DiscussionDate,
                RH_AgendaType = hrDiscussionModel.AgendaType,
                RH_Status = (GetDatabtnDraft == null) ? "Submitted" : "Draft",
                RH_CreatedBy = empCd,
                RH_CreatedDate = usDate,
                RIC_HRDiscussion_Dtl = discussionList
            };
            unitOfwork.HRDiscussion.Insert(hrDiscussion);
            unitOfwork.Save();

            if (GetDatabtnDraft == null)
            {
                return Content(("<script language='javascript' type='text/javascript'> alert('Submitted Successfully.');window.location = '" + Url.Action("HRDiscussionSubmitted") + "';</script>"));
            }
            else
            {
                return Content(("<script language='javascript' type='text/javascript'> alert('Draft Saved Successfully.');window.location = '" + Url.Action("HRDiscussionSubmitted") + "';</script>"));

            }



        }

        public ActionResult DeleteHRDiscussion(int id)
        {
            string empCd = User.Identity.Name;
            var entityToDelete = unitOfwork.HRDiscussion.GetByID(id);
            if (entityToDelete != null)
            {
                unitOfwork.HRDiscussion.Delete(entityToDelete);
                unitOfwork.Save();
            }

            List<HRDiscussionSubmittedModel> discussionModel =
                (from hrDicussionDetails in unitOfwork.HRDiscussionDtl.Get()
                 join hrDicussion in unitOfwork.HRDiscussion.Get()
                      on hrDicussionDetails.RHD_RefID equals hrDicussion.RH_ID
                 join employee in unitOfwork.User.Get()
                    on hrDicussion.RH_EmployeeID equals employee.RE_Emp_Cd
                 join reportingTo in unitOfwork.User.Get()
                    on hrDicussion.RH_ReviewerID equals reportingTo.RE_Emp_Cd
                 where hrDicussion.RH_Status == "Draft"
                 select new HRDiscussionSubmittedModel
                 {
                     DiscussionId = hrDicussion.RH_ID,
                     DiscussionDate = hrDicussion.RH_ReviewDate,
                     CandidateName = employee.RE_Employee_Name,
                     ManagerName = reportingTo.RE_Employee_Name,
                     AgendaType = hrDicussion.RH_AgendaType
                 }).ToList();

            return View("HRDiscussionDrafts", discussionModel);

        }

        [HttpGet]
        public ActionResult EditHRDiscussionDraft(int id, string retUrl)
        {
            string empCd = User.Identity.Name;
            var discussion = unitOfwork.HRDiscussion.GetByID(id);

            EditHRDiscussionDraftModel model = new EditHRDiscussionDraftModel
            {
                CandidateList = GetEmployeeList(),
                AgendaTypeList = GetAdendaTypeList(),
                EmpCd = discussion.RH_EmployeeID,
                DiscussionDate = discussion.RH_ReviewDate,
                AgendaType = discussion.RH_AgendaType,
                Description = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_Description,
                DiscussionNotes = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_DiscussioNotes,
                ActionItems = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_ActionItems,
                NextMeetUpDate = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_NextMeetUpDate,
                HRInternalComments = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_InternalCommnets,
                Discussionid = id
            };


            ViewBag.retUrl = retUrl;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditHRDiscussionDraft(EditHRDiscussionDraftModel model)
        {
            string empCd = User.Identity.Name;

            if (ModelState.IsValid)
            {
                var discussion = unitOfwork.HRDiscussion.GetByID(model.Discussionid);

                var discussionDetails = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault(s => s.RHD_RefID == discussion.RH_ID);

                discussionDetails.RHD_Description = model.Description;
                discussionDetails.RHD_DiscussioNotes = model.DiscussionNotes;
                discussionDetails.RHD_ActionItems = model.ActionItems;
                discussionDetails.RHD_NextMeetUpDate = model.NextMeetUpDate;
                discussionDetails.RHD_InternalCommnets = model.HRInternalComments;


                discussion.RH_EmployeeID = model.EmpCd;
                discussion.RH_ReviewerID = empCd;
                discussion.RH_ReviewDate = model.DiscussionDate;
                discussion.RH_AgendaType = model.AgendaType;
                discussion.RH_Status = "Submitted";




                discussion.RH_UpdatedBy = unitOfwork.User.GetByEmpID(empCd).RE_Jobdiva_User_Name;
                discussion.RH_UpdatedDate = usDate;
                unitOfwork.Save();


                return Content(("<script language='javascript' type='text/javascript'> alert('Submitted Successfully.');window.location = '" + Url.Action("HRDiscussionDrafts") + "';</script>"));
            }
            return View(model);






        }

        [HttpGet]
        public ActionResult EditHRDiscussionSubmit(int id)
        {
            string empCd = User.Identity.Name;
            var discussion = unitOfwork.HRDiscussion.GetByID(id);

            EditHRDiscussionDraftModel model = new EditHRDiscussionDraftModel
            {
                CandidateList = GetEmployeeList(),
                AgendaTypeList = GetAdendaTypeList(),
                EmpCd = discussion.RH_EmployeeID,
                DiscussionDate = discussion.RH_ReviewDate,
                AgendaType = discussion.RH_AgendaType,
                Description = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_Description,
                DiscussionNotes = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_DiscussioNotes,
                ActionItems = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_ActionItems,
                NextMeetUpDate = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_NextMeetUpDate,
                HRInternalComments = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_InternalCommnets,
                Discussionid = id,
                AdditionalComments = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_AdditionalComments
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditHRDiscussionSubmit(EditHRDiscussionDraftModel model)
        {
            string empCd = User.Identity.Name;

            if (ModelState.IsValid)
            {
                var discussion = unitOfwork.HRDiscussion.GetByID(model.Discussionid);

                var discussionDetails = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault(s => s.RHD_RefID == discussion.RH_ID);

                discussionDetails.RHD_AdditionalComments = model.AdditionalComments;

                discussion.RH_UpdatedBy = unitOfwork.User.GetByEmpID(empCd).RE_Jobdiva_User_Name;
                discussion.RH_UpdatedDate = usDate;
                unitOfwork.Save();


                return Content(("<script language='javascript' type='text/javascript'> alert('Updated Successfully.');window.location = '" + Url.Action("HRDiscussionSubmitted") + "';</script>"));
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult PrintHRDiscussion(int id)
        {
            string empCd = User.Identity.Name;
            var discussion = unitOfwork.HRDiscussion.GetByID(id);

            PrintHRDiscussion model = new PrintHRDiscussion
            {

                CandidateName = unitOfwork.User.GetByEmpID(discussion.RH_EmployeeID).RE_Employee_Name,
                DiscussionDate = discussion.RH_ReviewDate,
                AgendaType = discussion.RH_AgendaType,
                Description = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_Description,
                DiscussionNotes = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_DiscussioNotes,
                ActionItems = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_ActionItems,
                NextMeetUpDate = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_NextMeetUpDate,
                AdditionalComments = discussion.RIC_HRDiscussion_Dtl.FirstOrDefault().RHD_AdditionalComments
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult HRDiscussionSendEmail(int id, string subject, string location, DateTime StartDate, DateTime EndDate, string Body, string StatusMail = null)
        {

            string sendMail;

            if (StatusMail == null)
            {
                var discussion = unitOfwork.HRDiscussion.GetByID(id);
                sendMail = unitOfwork.RIC_Job_Report.SendEmailHrDiscussion(discussion.RH_EmployeeID, subject, location, StartDate, EndDate, Body);
                return Content(("<script language='javascript' type='text/javascript'> alert('Mail Sent Successfully.');window.location = '" + Url.Action("HRDiscussionSubmitted") + "';</script>"));

            }
            else
            {
                var empList = unitOfwork.User.GetByID(id);

                sendMail = unitOfwork.RIC_Job_Report.SendEmailHrDiscussion(empList.RE_Emp_Cd, subject, location, StartDate, EndDate, Body);
            }
            return Content(("<script language='javascript' type='text/javascript'> alert('Mail Sent Successfully.');window.location = '" + Url.Action("HRDiscussionSendInvite") + "';</script>"));


        }


        #endregion


    }

}