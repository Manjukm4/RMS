using System;
using DBLibrary;
using DBLibrary.UnitOfWork;
using RIC.Models.AssignSharedReq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using RIC.Models.SharedReqSubmittals;
using System.Web;
using System.Net.Mime;

namespace RIC.Controllers
{
    [Authorize]
    public class SharedReqSubmittalsController : Controller
    {
        //
        // GET: /SharedReqSubmittals/
        UnitOfWork unitOfwork = new UnitOfWork();

        public ActionResult Index()
        {
            SharedReqSubmittalsVM SharedReqSubmittalsVM = new SharedReqSubmittalsVM();
            List<SharedReq> ListSharedReq = (from rms_AssignSharedReq in unitOfwork.RMS_AssignSharedReq.Get()
                                             join rms_SharedReq_Dtl in unitOfwork.RMS_SharedReq_Dtl.Get() on rms_AssignSharedReq.RA_SharedReqHDRID equals rms_SharedReq_Dtl.RS_RefID
                                             join rms_SharedReq_Hdr in unitOfwork.RMS_SharedReq_HDR.Get() on rms_AssignSharedReq.RA_SharedReqHDRID equals rms_SharedReq_Hdr.RS_ID
                                             join ric_Employee1 in unitOfwork.RIC_Employee.Get() on rms_AssignSharedReq.RA_AssignedBy equals ric_Employee1.RE_Emp_Cd
                                             join ric_Employee2 in unitOfwork.RIC_Employee.Get() on rms_SharedReq_Hdr.RS_CreatedBy equals ric_Employee2.RE_Emp_Cd
                                             where rms_AssignSharedReq.RA_AssignedTo == User.Identity.Name
                                             select new SharedReq()
                                             {
                                                 HDRID = rms_SharedReq_Hdr.RS_ID,
                                                 AssignedID = rms_AssignSharedReq.RA_ID,
                                                 JobID = rms_SharedReq_Hdr.RS_JobID,
                                                 JobDivaRef = rms_SharedReq_Hdr.RS_JobDivaRef,
                                                 JobTitle = rms_SharedReq_Dtl.RS_JobTitle,
                                                 JobIssueDate = rms_SharedReq_Dtl.RS_JobIssueDate,
                                                 Company = rms_SharedReq_Dtl.RS_Company,
                                                 WorkLocation = rms_SharedReq_Dtl.RS_WorkLocation,
                                                 Priority = rms_SharedReq_Dtl.RS_Priority,
                                                 Division = rms_SharedReq_Dtl.RS_Division,
                                                 Category = rms_SharedReq_Dtl.RS_Category,
                                                 City = rms_SharedReq_Dtl.RS_City,
                                                 State = rms_SharedReq_Dtl.RS_State,
                                                 BillRate = rms_SharedReq_Dtl.RS_BillRate == null ? 0 : float.Parse(rms_SharedReq_Dtl.RS_BillRate.ToString()),
                                                 PayRate = rms_SharedReq_Dtl.RS_PayRate == null ? 0 : float.Parse(rms_SharedReq_Dtl.RS_PayRate.ToString()),
                                                 PrimaryAssigment = rms_SharedReq_Dtl.RS_PrimaryAssigment != null?rms_SharedReq_Dtl.RS_PrimaryAssigment.Split(',').ToArray():null,
                                                 MaxSubAllowed = rms_SharedReq_Dtl.RS_MaxSubAllowed,
                                                 InternalSub = rms_SharedReq_Dtl.RS_InternalSub,
                                                 ExternalSub = rms_SharedReq_Dtl.RS_ExternalSub,
                                                 RMSJobStatus = rms_SharedReq_Hdr.RS_RMSJobStatus,

                                                 FileType = rms_SharedReq_Dtl.RS_FileType,

                                                 JobCreatedBy = ric_Employee2.RE_Jobdiva_User_Name,
                                                 JobCreatedDate = rms_SharedReq_Hdr.RS_CreatedDt,
                                                 JobInstructions = rms_SharedReq_Dtl.RS_Instructions,

                                                 JobAssignedBy = ric_Employee1.RE_Jobdiva_User_Name,
                                                 JobAssignedDate = rms_AssignSharedReq.RA_AssignedDt,
                                                 JobAssignedInstructions = rms_AssignSharedReq.RA_Instructions
                                             }).ToList();

            foreach (var Element in ListSharedReq)
            {
                Element.Notes = (from rms_SharedReqNotes in unitOfwork.RMS_SharedReqNotes.Get().Where(e => e.RS_RefHDRID == Element.HDRID)
                                   join rms_Employee in unitOfwork.RIC_Employee.Get()
                                   on rms_SharedReqNotes.RS_CreatedBy equals rms_Employee.RE_Emp_Cd
                                   select new
                                   {
                                       Name = rms_Employee.RE_Jobdiva_User_Name,
                                       Date = rms_SharedReqNotes.RS_CreatedDt,
                                       Notes = rms_SharedReqNotes.RS_Notes
                                   }).OrderByDescending(o => o.Date).Select(e => "<b>" + e.Name.Trim() + "</b>" + " - " + e.Date + " - " + e.Notes).ToArray();

                Element.RMS_SharedReqSubmittals = unitOfwork.RMS_SharedReqSubmittals.Get().Where(e => e.RS_RefID == Element.AssignedID).ToList();
            }

            SharedReqSubmittalsVM.ListSharedReq = ListSharedReq;

            return View(SharedReqSubmittalsVM);
        }

        // Save submittals for assigned Job.
        [HttpPost]
        public ActionResult SaveSubmittals(int AssignedID, string CandidateName, string CandidateEmail, string EnteredUser, DateTime EnteredDate, string SubmittedBy, DateTime SubmittedDate, string Comments)
        {

            if (AssignedID != 0 && CandidateName != "" && CandidateEmail != "" && EnteredUser != "" && EnteredDate != null && SubmittedBy != "" && SubmittedDate != null)
            {

                RMS_AssignSharedReq obj = unitOfwork.RMS_AssignSharedReq.GetByID(AssignedID);
                obj.RMS_SharedReqSubmittals = new List<RMS_SharedReqSubmittals>();

                RMS_SharedReqSubmittals newobj = new RMS_SharedReqSubmittals()
                {
                    RS_CandidateName = CandidateName,
                    RS_CandidateEmail = CandidateEmail,

                    RS_EnteredName = EnteredUser,
                    RS_EntredDate = EnteredDate,

                    RS_SubmittedBy = SubmittedBy,
                    RS_SubmittedDate = SubmittedDate,

                    RS_CreatedBy = User.Identity.Name,
                    RS_CreatedDt = SystemClock.US_Date,

                    RS_Comments = Comments
                };

                obj.RMS_SharedReqSubmittals.Add(newobj);
                unitOfwork.Save();
            }

            return RedirectToAction("Index");
        }

        //SaveComments
        [HttpPost]
        public ActionResult SaveComments(int RS_HDRID, string Instructions, string AssignTo, string RMSJobStatus)
        {
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


        public FileResult GetFile(int AssignedID)
        {

            var Data = (from rms_AssignSharedReq in unitOfwork.RMS_AssignSharedReq.Get()
                        join rms_SharedReq_Dtl in unitOfwork.RMS_SharedReq_Dtl.Get() on rms_AssignSharedReq.RA_SharedReqHDRID equals rms_SharedReq_Dtl.RS_RefID
                        where rms_AssignSharedReq.RA_ID == AssignedID
                        select new
                        {
                            RS_FileName = rms_SharedReq_Dtl.RS_FileName,
                            rms_SharedReq_Dtl.RS_FileType,
                            rms_SharedReq_Dtl.RS_FileData
                        }).ToList().First();

            Response.AppendHeader("Content-Disposition", new ContentDisposition
            {
                FileName = Data.RS_FileName,
                Inline = true,
            }.ToString());

            return File(Data.RS_FileData, Data.RS_FileType);
        }

    }
}
