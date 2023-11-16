using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Review
{
    public class AnnualFeedbackRequestHrVM
    {
        public int ReviewID { get; set; }

        public string EmployeeName { get; set; }

        public string LeadReviewer { get; set; }

        public string ReviewStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReviewDate { get; set; }

        public bool HrReviewStatus { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime ? LastUpdateDate { get; set; }

        public int NotificationCount { get; set; }
        public List<string> PendingReviewers { get; set; }

        public List<SelectListItem> YearSelectList { get; set; }

        public int Year { get; set; }

    }
}