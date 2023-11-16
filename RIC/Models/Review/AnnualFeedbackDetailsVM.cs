using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RIC.Models.Client;

namespace RIC.Models.Review
{
    public class AnnualFeedbackDetailsVM
    {
        public int ReviewID { get; set; }

        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Date Of Joining")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? JoiningDate { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Month of review")]
        [DisplayFormat(DataFormatString = "{0:MMM,yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReviewDate { get; set; }

        [Display(Name = "Leaves taken on LOP")]
        public int Hr_LeaveTaken { get; set; }


        [Display(Name = "Leave history")]
        public int Hr_LeaveTakenOnLop { get; set; }

        [Display(Name = "Any warnings given in the last 12 months? If yes, share the instance")]
        public string Hr_Warnings { get; set; }

        [Display(Name = "Shows loyalty to Sunrise company values.")]
        public int Hr_CompanyValue { get; set; }

        [Display(Name = "Is behaving according to our cultural values.")]
        public int Hr_CultValue { get; set; }

        [Display(Name = "Respects differences in culture between people")]
        public int Hr_CultPeople { get; set; }

        [Display(Name = "Additional Comments")]
        public List<AdditionalComments> AdditionalComments { get; set; }

        [Display(Name = "Improvements Recommended")]
        public List<ImprovementsRecommended> ImprovementsRecommended { get; set; }

        public string ReturnUrl { get; set; }

        [DisplayFormat(DataFormatString = "{0:n1}", ApplyFormatInEditMode = true)]
        public float AverageRating { get; set; }

        public bool ShowAvgRating { get; set; }

       // public bool ShowAllUser { get; set; }

        public bool ShowHrReview { get; set; }

        public List<AnnualFeedbackDetailsAttributesHeaders> AttributHeaders { get; set; }

        public List<InterimReviewDetails> InterimReviewDetails { get; set; }

        public DBLibrary.RMS_SubmissionAnalysis submissionAnalysys { get; set; }

        public List<ClientDashboardQuaterly> QuarterSubmissionAnalysys { get; set; }

    }

    public class InterimReviewDetails 
    {
        public int InterimReviewID { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public String ReviewerName { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

    }

    public class ImprovementsRecommended
    {
        public string ReviewerName { get; set; }
        public string Comments { get; set; }
    }

    public class AdditionalComments
    {
        public string ReviewerName { get; set; }
        public string Comments { get; set; }
    }

    public class AnnualFeedbackDetailsAttributesHeaders
    {

        public string HeaderName { get; set; }

        public List<AnnualFeedbackDetailsAttributes> Attributes { get; set; }
    }

    public class AnnualFeedbackDetailsAttributes
    {
        public int AttrID { get; set; }

        public string AttrName { get; set; }


        public List<AnnualFeedbackDetailsRating> rating { get; set; }

    }




    public class AnnualFeedbackDetailsRating
    {
        public string ReviewerName { get; set; }

        public float Rating { get; set; }

        public string Comments { get; set; }



    }

}