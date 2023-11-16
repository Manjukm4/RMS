using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Review
{
    public class EditReviewModel
    {
        public int ReviewID { get; set; }
        [Display(Name = "Candidate Name")]
        public string CandidateName { get; set; }

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

        [Display(Name = "Findings (By TL)")]
        public string TeamLeadReview { get; set; }

        [Display(Name = "Improvements Required")]
        public string ImprovementsRequired { get; set; }

        [Display(Name = "Required Director Feedback")]
        public string DirectorFeedback { get; set; }

        [Display(Name = "Required HR Feedback")]
        public string HrFeedback { get; set; }

        [Display(Name = "Next Review Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? NextReviewDate { get; set; }

        [Display(Name = "Target For Next Review (Submission/Interview/Hires/Calls)")]
        public string TargetForNextReview { get; set; }

        [Display(Name = "Additional Comments")]
        public string AdditionalComments { get; set; }
        //[Display(Name = "Submission Target")]
        //public int? SubmissionTarget { get; set; }

        //[Display(Name = "Interview Target")]
        //public int? InterviewsTarget { get; set; }

        //[Display(Name = "Hire Target")]
        //public int? HiresTarget { get; set; }

        //[Display(Name = "Call Target")]
        //public int? CallsTarget { get; set; }

        // public bool SaveAsDraft { get; set; }
    }
}