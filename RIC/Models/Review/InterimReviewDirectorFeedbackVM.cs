using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Review
{
    public class InterimReviewDirectorFeedbackVM
    {
        public int ReviewID { get; set; }
        [Display(Name = "Employee Name")]
        public string CandidateName { get; set; }

        [Display(Name = "Reviewer Name")]
        public string ReviewerName { get; set; }

       
        public string EmpCd { get; set; }

        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Submissions")]
        public int Submissions { get; set; }

        [Display(Name = "Interviews")]
        public int Interviews { get; set; }

        [Display(Name = "Hires")]
        public int Hires { get; set; }

        [Display(Name = "Number Of Calls")]
        public int Calls { get; set; }

        public int? SubmissionTarget { get; set; }

        [Display(Name = "Next Review Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime NextReviewDate { get; set; }

        public int? InterviewTarget { get; set; }

        public int? HireTarget { get; set; }

        public int? CallsTarget { get; set; }
       
        [Required]
        [Display(Name = "Comments")]
        public string DirectorFeedback { get; set; }

        public List<SelectListItem> HrList { get; set; }

        public string HrID { get; set; }

        [Display(Name="Forward To HR")]
        public bool ForwardToHR { get; set; }

        public List<InterimReviewerDtl> ReviewerDtl { get; set; }
    }
}