//Author : Shaurya
using DBLibrary;
using DBLibrary.UnitOfWork;
using RIC.Models.AssignSharedReq;
using RIC.Utility;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace RIC.Controllers
{
	[Authorize]
	public class AssignSharedReqController : Controller
	{
		// GET: /AssignSharedReq/
		UnitOfWork unitOfwork = new UnitOfWork();
		string Director = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];

		public ActionResult Index()
		{
			AssignSharedReqVM SharedReqVM = new AssignSharedReqVM();
			List<AssignSharedReq> ListSharedReq = (from sharedReq_Hdr in unitOfwork.RMS_SharedReq_HDR.Get()
																join sharedReq_Dtl in unitOfwork.RMS_SharedReq_Dtl.Get()
																on sharedReq_Hdr.RS_ID equals sharedReq_Dtl.RS_RefID
																join ric_Employee2 in unitOfwork.RIC_Employee.Get() on sharedReq_Hdr.RS_CreatedBy equals ric_Employee2.RE_Emp_Cd
																select new AssignSharedReq()
																{
																	HDRID = sharedReq_Hdr.RS_ID,
																	JobID = sharedReq_Hdr.RS_JobID,
																	JobDivaRef = sharedReq_Hdr.RS_JobDivaRef,
																	JobTitle = sharedReq_Dtl.RS_JobTitle,
																	JobIssueDate = sharedReq_Dtl.RS_JobIssueDate,
																	Company = sharedReq_Dtl.RS_Company,
																	WorkLocation = sharedReq_Dtl.RS_WorkLocation,
																	Priority = sharedReq_Dtl.RS_Priority,
																	Division = sharedReq_Dtl.RS_Division,
																	Category = sharedReq_Dtl.RS_Category,
																	City = sharedReq_Dtl.RS_City,
																	State = sharedReq_Dtl.RS_State,
																	BillRate = sharedReq_Dtl.RS_BillRate==null?0: float.Parse(sharedReq_Dtl.RS_BillRate.ToString()),
																	PayRate = sharedReq_Dtl.RS_PayRate==null?0:float.Parse(sharedReq_Dtl.RS_PayRate.ToString()),
																	PrimaryAssigment = sharedReq_Dtl.RS_PrimaryAssigment != null?  sharedReq_Dtl.RS_PrimaryAssigment.Split(',').ToArray():null,
																	MaxSubAllowed = sharedReq_Dtl.RS_MaxSubAllowed,
																	InternalSub = sharedReq_Dtl.RS_InternalSub,
																	ExternalSub = sharedReq_Dtl.RS_ExternalSub,
																	RMSJobStatus = sharedReq_Hdr.RS_RMSJobStatus,
																	CreatedBy = ric_Employee2.RE_Jobdiva_User_Name,
																	CreatedDate = sharedReq_Hdr.RS_CreatedDt,
																	//Instructions = sharedReq_Dtl.RS_Instructions
																}).ToList();

			string CurrentUser = unitOfwork.RIC_Employee.Get().Where(e => e.RE_Emp_Cd == User.Identity.Name).First().RE_Jobdiva_User_Name;

			foreach (AssignSharedReq sharedReq in ListSharedReq)
			{

				sharedReq.Notes = (from rms_SharedReqNotes in unitOfwork.RMS_SharedReqNotes.Get().Where(e => e.RS_RefHDRID == sharedReq.HDRID)
										 join rms_Employee in unitOfwork.RIC_Employee.Get()
										 on rms_SharedReqNotes.RS_CreatedBy equals rms_Employee.RE_Emp_Cd
										 select new
										  {
											  Name = rms_Employee.RE_Jobdiva_User_Name,
											  Date = rms_SharedReqNotes.RS_CreatedDt,
											  Notes = rms_SharedReqNotes.RS_Notes
										  }).OrderByDescending(o => o.Date).Select(e => "<b>" + e.Name.Trim() + "</b>" + " - " + e.Date + " - " + e.Notes).ToArray();

				sharedReq.PreviouslyAssignedTo = (from assignSharedReq in unitOfwork.RMS_AssignSharedReq.Get()
															 join rIC_Employee in unitOfwork.RIC_Employee.Get()
															 on assignSharedReq.RA_AssignedTo equals rIC_Employee.RE_Emp_Cd
															 where assignSharedReq.RA_SharedReqHDRID == sharedReq.HDRID
															 select new
															 {
																 Name = rIC_Employee.RE_Jobdiva_User_Name,
																 Date = assignSharedReq.RA_AssignedDt,
																 //Instructions = assignSharedReq.RA_Instructions
															 }).OrderByDescending(o => o.Date).Select(e => e.Name.Trim() + " - " + e.Date).ToArray();

				sharedReq.SelectRMSJobStatus = new List<SelectListItem>();

				sharedReq.SelectRMSJobStatus.Add(new SelectListItem()
				{
					Value = "OPEN",
					Text = "Open",
				});

				sharedReq.SelectRMSJobStatus.Add(new SelectListItem()
				{
					Value = "HOLD",
					Text = "Hold"
				});

				sharedReq.SelectRMSJobStatus.Add(new SelectListItem()
				{
					Value = "CLOSE",
					Text = "Close"
				});

				// Use empty string to identify current RMS JobStatus in database. 
				sharedReq.SelectRMSJobStatus.Find(e => e.Value == sharedReq.RMSJobStatus).Value = "";

				sharedReq.HasRights = (sharedReq.CreatedBy == CurrentUser || User.Identity.Name == Director) ? true : false;

				//sharedReq.IsCandidatesSubmitted = (from ric_SharedReq_HDR in unitOfwork.RMS_SharedReq_HDR.Get()
				//                                   join ric_AssignSharedReq in unitOfwork.RMS_AssignSharedReq.Get() on ric_SharedReq_HDR.RS_ID equals ric_AssignSharedReq.RA_SharedReqHDRID into sharedassign
				//                                   from subSharedAssign in sharedassign.DefaultIfEmpty()
				//                                  join ric_SharedReqSubmittals in unitOfwork.RMS_SharedReqSubmittals.Get()
				//                                   select new
				//                                   {
				//                                       ric_SharedReq_HDR.RS_ID

				//                                   }).Count() >= 1 ? true : false;

			}

			SharedReqVM.ListSharedReq = ListSharedReq;

			SharedReqVM.TeamLeads = (from rIC_Role in unitOfwork.Role.Get()
											 join rIC_User_Role in unitOfwork.RIC_User_Role.Get() on rIC_Role.RR_RoleId equals rIC_User_Role.RUR_Role_ID
											 join rIC_Employee in unitOfwork.RIC_Employee.Get() on rIC_User_Role.RUR_Emp_ID equals rIC_Employee.RE_EmpId
											 where rIC_Role.RR_Role_Name == "Team Lead"
											 select new SelectListItem()
											 {
												 Value = rIC_Employee.RE_Emp_Cd,
												 Text = rIC_Employee.RE_Employee_Name
											 });

			return View(SharedReqVM);
		}

		// for dynamic search.
		//[HttpPost]
		//public ActionResult Index(AssignSharedReqVM SharedReqVM)
		//{
		//    //if (SharedReqVM.SharedReqSearch.JobDivaRef == null)
		//    //{
		//    //    SharedReqVM.Message = "Please add search parameters.";
		//    //    return View("AssignSharedReq");
		//    //}

		//    List<AssignSharedReq> ListSharedReq = (from sharedReq_Hdr in unitOfwork.RMS_SharedReq_HDR.Get()
		//                                           join sharedReq_Dtl in unitOfwork.RMS_SharedReq_Dtl.Get()
		//                                           on sharedReq_Hdr.RS_ID equals sharedReq_Dtl.RS_RefID
		//                                           where sharedReq_Hdr.RS_JobDivaRef == SharedReqVM.SharedReqSearch.JobDivaRef || sharedReq_Dtl.RS_JobTitle == SharedReqVM.SharedReqSearch.JobTitle || sharedReq_Dtl.RS_Company == SharedReqVM.SharedReqSearch.CompanyName || sharedReq_Dtl.RS_JobIssueDate.Date == SharedReqVM.SharedReqSearch.JobIssuedDate.Date
		//                                           select new AssignSharedReq()
		//                                           {
		//                                               RS_HDRID = sharedReq_Hdr.RS_ID,
		//                                               JobDivaRef = sharedReq_Hdr.RS_JobDivaRef,
		//                                               JobTitle = sharedReq_Dtl.RS_JobTitle,
		//                                               JobIssueDate = sharedReq_Dtl.RS_JobIssueDate,
		//                                               Company = sharedReq_Dtl.RS_Company,
		//                                               WorkLocation = sharedReq_Dtl.RS_WorkLocation,
		//                                               Priority = sharedReq_Dtl.RS_Priority,
		//                                               Division = sharedReq_Dtl.RS_Division,
		//                                               Category = sharedReq_Dtl.RS_Category,
		//                                               State = sharedReq_Dtl.RS_State,
		//                                               BillRate = sharedReq_Dtl.RS_BillRate,
		//                                               PayRate = sharedReq_Dtl.RS_PayRate,
		//                                               PrimaryAssigment = sharedReq_Dtl.RS_PrimaryAssigment.Split(',').ToArray(),
		//                                               MaxSubAllowed = sharedReq_Dtl.RS_MaxSubAllowed,
		//                                               InternalSub = sharedReq_Dtl.RS_InternalSub,
		//                                               ExternalSub = sharedReq_Dtl.RS_ExternalSub
		//                                           }).ToList();

		//    foreach (AssignSharedReq sharedReq in ListSharedReq)
		//    {
		//        sharedReq.PreviouslyAssignedTo = (from assignSharedReq in unitOfwork.RMS_AssignSharedReq.Get()
		//                                          join rIC_Employee in unitOfwork.RIC_Employee.Get()
		//                                          on assignSharedReq.RA_AssignedTo equals rIC_Employee.RE_Emp_Cd
		//                                          where assignSharedReq.RA_SharedReqHDRID == sharedReq.RS_HDRID
		//                                          select new
		//                                          {
		//                                              rIC_Employee.RE_Employee_Name
		//                                          }).Select(e => e.ToString().Split('=')[1].Replace("}", "").Trim()).ToArray();
		//    }

		//    SharedReqVM.ListSharedReq = ListSharedReq;

		//    SharedReqVM.TeamLeads = (from rIC_Role in unitOfwork.Role.Get()
		//                             join rIC_User_Role in unitOfwork.RIC_User_Role.Get() on rIC_Role.RR_RoleId equals rIC_User_Role.RUR_Role_ID
		//                             join rIC_Employee in unitOfwork.RIC_Employee.Get() on rIC_User_Role.RUR_Emp_ID equals rIC_Employee.RE_EmpId
		//                             where rIC_Role.RR_Role_Name == "Team Lead"
		//                             select new SelectListItem()
		//                             {
		//                                 Value = rIC_Employee.RE_Emp_Cd,
		//                                 Text = rIC_Employee.RE_Employee_Name
		//                             });


		//    return View(SharedReqVM);
		//}

		[HttpPost]
		public ActionResult Assign(int RS_HDRID, string Instructions, string AssignTo, string RMSJobStatus)
		{
			if (AssignTo != "" && RS_HDRID > 0)
			{
				//Check if already assigned.	 Or RMSJobStatus is not set to Close.
				if (unitOfwork.RMS_AssignSharedReq.Get().Where(e => e.RA_AssignedTo == AssignTo && e.RA_SharedReqHDRID == RS_HDRID).Count() == 0 && RMSJobStatus != "CLOSE")
				{
					unitOfwork.RMS_AssignSharedReq.Insert(new RMS_AssignSharedReq
					{
						RA_SharedReqHDRID = RS_HDRID,
						RA_AssignedTo = AssignTo,
						RA_AssignedBy = User.Identity.Name,
						RA_AssignedDt = SystemClock.US_Date
					});

					unitOfwork.Save();
				}
			}

			if (RS_HDRID > 0 && RMSJobStatus != "")
			{
				unitOfwork.RMS_SharedReq_HDR.GetByID(RS_HDRID).RS_RMSJobStatus = RMSJobStatus;
				unitOfwork.Save();
				if (RMSJobStatus == "CLOSE")
				{
					string  error;

					string MailBody = @"<!DOCTYPE html>
										<html lang='en'>
										<head><meta charset='UTF-8'>
										    <title>Notification</title><style>body{font-family: arial, sans-serif;font-size:16px;}#div{padding:1%;background: white;display: block;margin: 0 auto;margin-bottom: 0.5cm;box-shadow: 0 0 0.5cm rgba(0,0,0,0.5);}table {font-family: arial, sans-serif;border-collapse: collapse;width: 100%;}td, th {border: 1px solid #dddddd;text-align: left;padding: 8px;}tr:nth-child(odd){background-color: #dddddd;}</style>
										</head>
										<body><div id='div'><p >Hi,</p><div style='padding:1%;'><p>Job as been closed in RMS shared requirement.</p><table><tr><th>Job ID</th><th>JD Ref</th><th>Title</th><th>Company</th><th>Requester</th><th>MaxSub</th></tr><tr><td><a href='https://www1.jobdiva.com/employers/myjobs/vieweditjobform.jsp?jobid=@JobID'>@JobID</a></td><td>@JDRef</td><td>@Title</td><td>@Company</td><td>@Requester</td><td>@MaxSub</td></tr></table></div><br><p>Please do not reply to this mail.<p></div></body></html>
										";

					var JobData = unitOfwork.RMS_SharedReq_HDR.GetByID(RS_HDRID);

					MailBody = MailBody.Replace("@JobID", JobData.RS_JobID.ToString());
					MailBody = MailBody.Replace("@JDRef", JobData.RS_JobDivaRef);
					MailBody = MailBody.Replace("@Title", JobData.RMS_SharedReq_Dtl.First().RS_JobTitle);
					MailBody = MailBody.Replace("@Company", JobData.RMS_SharedReq_Dtl.First().RS_Company);
					MailBody = MailBody.Replace("@Requester", unitOfwork.User.GetByEmpID(User.Identity.Name).RE_Jobdiva_User_Name);
					MailBody = MailBody.Replace("@MaxSub", JobData.RMS_SharedReq_Dtl.First().RS_MaxSubAllowed.ToString());
	
					Email.SendMail(ConfigurationManager.AppSettings["SharedReq_Email"].Split(';'), "Job CLOSED In RMS", MailBody, out error);
				}
			}

			if (RS_HDRID > 0 && Instructions != "")
			{
				RMS_SharedReq_HDR Obj = unitOfwork.RMS_SharedReq_HDR.GetByID(RS_HDRID);
				Obj.RMS_SharedReqNotes = new List<RMS_SharedReqNotes>();

				RMS_SharedReqNotes newobj = new RMS_SharedReqNotes()
			  {
				  RS_Notes = Instructions,
				  RS_CreatedBy = User.Identity.Name,
				  RS_CreatedDt = SystemClock.US_Date

			  };

				Obj.RMS_SharedReqNotes.Add(newobj);
				unitOfwork.Save();
			}

			return RedirectToAction("Index");
		}

	}
}
