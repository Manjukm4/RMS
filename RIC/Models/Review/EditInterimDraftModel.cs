using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Review
{
    public class EditInterimDraftModel
    {

        public int ReviewID { get; set; }

        [Display(Name = "Employee Name")]
        public string CandidateName { get; set; }

        [Required]
        public string EmpCd { get; set; }

        public IEnumerable<SelectListItem> DirectorList { get; set; }

        public string DirectorID { get; set; }

        [Required]
        [Display(Name = "From Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime? FromDate { get; set; }

        [Required]
        [Display(Name = "To Date")]
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

        [Display(Name = "Director Feedback")]
        public string DirectorFeedback { get; set; }

        [Display(Name = "HR Feedback")]
        public string HrFeedback { get; set; }

        [Display(Name = "Next Review Date")]
        public DateTime? NextReviewDate { get; set; }

        [Display(Name = "Submission Target")]
        public int? SubmissionTarget { get; set; }

        [Display(Name = "Interview Target")]
        public int? InterviewsTarget { get; set; }

        [Display(Name = "Hire Target")]
        public int? HiresTarget { get; set; }

        [Display(Name = "Call Target")]
        public int? CallsTarget { get; set; }

        //   public List<string> ReviewerList { get; set; }
        public List<string> DefaultReviewerList { get; set; }

        public List<string> SelectedReviewer { get; set; }

        public bool SaveAsDraft { get; set; }
    }
}