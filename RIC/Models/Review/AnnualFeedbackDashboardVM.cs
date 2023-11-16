using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Review
{
    public class AnnualFeedbackDashboardVM
    {
        public AnnualFeedbackDashboardVM()
        {
            this.reviewers = new List<string>();
        }
        public int FormID { get; set; }

        public string EmployeeID { get; set; }
        public string LeadReviewerId { get; set; }

        public string UserRole { get; set; }

        public string EmployeeName { get; set; }

        public string ReportingTo { get; set; }

        public string Status { get; set; }

        public bool ShowCheckBox { get; set; }

        public List<string> reviewers { get; set; }

        public int NotificationCount { get; set; }

        public string HrName { get; set; }

        public bool HrReviewStatus { get; set; }

        public DateTime ? LastUpdatedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastReviewDate { get; set; }
    }
}