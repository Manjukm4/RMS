using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Review
{
    public class InterimReviewDetailsModel
    {
        [Display(Name="Review Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime ReviewDate{get;set;}

        public int ReviewID { get; set; }
        [Display(Name = "Employee Name")]
        public string CandidateName { get; set; }

        [Display(Name="Reviewer Name")]
        public string ReviewerName { get; set; }

        [Required]
        public string EmpCd { get; set; }

        [Required]
        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? FromDate { get; set; }

        [Required]
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

        public string DirectorName { get; set; }

        public string DirectorComments { get; set; }

        public string HrName { get; set; }

        public string HrComments { get; set; }

        public bool ShowHrReview { get; set; }

        public bool ShowDirectorReview { get; set; }

        public List<InterimReviewerDtl> ReviewerDtl { get; set; }
    }

    public class InterimReviewerDtl
    {
        public string ReviewerName { get; set; }

        public string Findings { get; set; }

        public string Improvement { get; set; }

        public String Status { get; set; }
    }

}