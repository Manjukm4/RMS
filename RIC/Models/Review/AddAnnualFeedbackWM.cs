using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RIC.Models.Client;


namespace RIC.Models.Review
{
    public class AddAnnualFeedbackWM
    {
        public DBLibrary.RMS_SubmissionAnalysis submissionAnalysys;

        public List<ClientDashboardQuaterly> QuarterSubmissionAnalysys { get; set; }

        public int ReviewId { get; set; }

        public int FormId { get; set; }

        public int FeedbackID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Joining")]
        public DateTime? JoiningDate { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM , yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Month of review")]
        public DateTime ReviewDate { get; set; }

        List<string> RE_Reviewer_List { get; set; }

        public List<InterimReviewDetails> InterimReviewDetails { get; set; }

        public IEnumerable<AnnualFeedbackFields> FeedbackFields { get; set; }
    }


    public class AnnualFeedbackFields
    {
        public int fieldID { get; set; }
        public string FieldName { get; set; }
        public bool IsHeader { get; set; }
        [Display(Name = "WEIGHTAGE")]
        public int Weightage { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }

        public int TotalRecords { get; set; }
    }



}