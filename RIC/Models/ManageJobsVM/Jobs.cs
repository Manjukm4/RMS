using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RIC.Models.ManageJobsVM
{
    public class Jobs
    {
        public int JobID { get; set; }
        public int HDRID { get; set; }
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
        public string Instructions { get; set; }
        public string LastAssignedTo { get; set; }
        public string[] PreviouslyAssignedTo { get; set; }

        public string[] Notes { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool HasRights { get; set; }
        public List<SelectListItem> SelectRMSJobStatus { get; set; }
        public string RMSJobStatus { get; set; }
        public string JobDivaStatus { get; set; }

        public bool CheckInStatus { get; set; }
        public int? CheckInRefId { get; set; }
        public string ClientId { get; set; }

        public DateTime CheckOutDate { get; set; }
        public DateTime? CheckInDate { get; set; }


    }
}