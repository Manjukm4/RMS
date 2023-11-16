using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



using System.ComponentModel.DataAnnotations;
namespace RIC.Models.Review
{
    public class InterimReviewSubmittedModel
    {
        public int ReviewID { get; set; }
        [Display(Name = "Emp ID")]

        public string EmpCd { get; set; }

        [Display(Name = "Employee Name")]
        public string CandidateName { get; set; }

        [Display(Name = "Issued By")]
        public string ManagerName { get; set; }

        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Review Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReviewDate { get; set; }

        [Display(Name = "Submissions/Interviews/Hires")]
        public string Sub_Int_Hire { get; set; }

        //[Display(Name = "Director Feedback")]
        //public bool DirectorFeedbackRequired { get; set; }

        //[Display(Name = "HR Feedback")]
        //public bool HrFeedbackRequired { get; set; }

        [Display(Name = "Next Review Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NextReviewDate { get; set; }

        [Display(Name = "Draft")]
        public bool Draft { get; set; }

        [Display(Name = "Status")]
        public string ReviewStatus { get; set; }

        public List<string> PendingReviewers { get; set; }

        public bool DirectorFeedbackStatus { get; set; }

        public bool HRFeedbackStatus { get; set; }

        public int NotificationCount { get; set; }
    }
}