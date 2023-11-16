using DBLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.SharedReqSubmittals
{
    public class SharedReq
    {
        public int JobID { get; set; }
        public int HDRID { get; set; }
        public int AssignedID { get; set; }
        public string JobDivaRef { get; set; }
        public string JobTitle { get; set; }
        public DateTime JobIssueDate { get; set; }
        public string Company { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string WorkLocation { get; set; }
        public string Priority { get; set; }
        public string Division { get; set; }
        public string Category { get; set; }
        public float BillRate { get; set; }
        public float PayRate { get; set; }
        public string[] PrimaryAssigment { get; set; }
        public int? MaxSubAllowed { get; set; }
        public int? InternalSub { get; set; }
        public int? ExternalSub { get; set; }

        public string JobCreatedBy { get; set; }
        public DateTime JobCreatedDate { get; set; }
        public string JobInstructions { get; set; }

        public string JobAssignedBy { get; set; }
        public DateTime JobAssignedDate { get; set; }
        public string JobAssignedInstructions { get; set; }

        public string RMSJobStatus { get; set; }

        public string FileType { get; set; }
        public string FileData { get; set; }
        public string[] Notes { get; set; }

        public List<RMS_SharedReqSubmittals> RMS_SharedReqSubmittals { get; set; }

        public SharedReq()
        {
            RMS_SharedReqSubmittals = new List<RMS_SharedReqSubmittals>();
        }
    }
}