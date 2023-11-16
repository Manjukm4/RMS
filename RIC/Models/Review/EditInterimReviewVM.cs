using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Review
{
    public class EditInterimReviewVM
    {
        public int ReviewID { get; set; }

        [Display(Name="Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name="Reviewer Name")]
        public string ReviewerName { get; set; }

        [Display(Name="Review Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReviewDate { get; set; }

        [Display(Name="From Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Display(Name="To Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [Display(Name="Next Review Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NextReviewDate { get; set; }

        [Display(Name="Submission")]
        public int SubmissionAchivement { get; set; }

        [Display(Name="Interview")]
        public int InterviewAchivement { get; set; }

        [Display(Name="Hires")]
        public int HiresAchivements { get; set; }

        [Display(Name="Number Of Calls")]
        public int CallsAchivements { get; set; }

        [Display(Name="Submission Target")]
        public int? SubmissionTarget { get; set; }

        [Display(Name="Interview Target")]
        public int? InterviewTarget { get; set; }

        [Display(Name="Hires Target")]
        public int? HiresTarget { get; set; }

        [Display(Name="Calls Target")]
        public int? CallsTarget { get; set; }

        public bool HrReviewStatus { get; set; }

        public string HrName { get; set; }

        [Display(Name = "Comments")]
        public string HrComments { get; set; }

        public bool DirectorReviewStatus { get; set; }

        public string DirectorName { get; set; }

        [Display(Name="Comments")]
        public string DirectorComments { get; set; }

        [Display(Name="Last Updated By")]
        public string LastUpdatedBy { get; set; }

        [Display(Name="Last Updated Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastUpdatedDate { get; set; }

        public List<EditReviewersDetails> ReviewerList { get; set; }
    }       

    public class EditReviewersDetails
    {
        public int Id { get; set; }

        [Display(Name="Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Findings")]
        public string Findings { get; set; }

        [Display(Name = "Improvements Required")]
        public string ImprovementsRequired { get; set; }

    }


}