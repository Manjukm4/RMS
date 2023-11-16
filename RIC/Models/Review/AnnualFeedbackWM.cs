using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace RIC.Models.Review
{
    public class AnnualFeedbackWM
    {

        public int ReviewID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }


        [Display(Name = "Issued By")]
        public string IssuedBy { get; set; }

        public string Status { get; set; }
        public List<string> reviewers { get; set; }

        public string ReportingTo { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Review Date")]
        public DateTime ReviewDate { get; set; }
        
        public bool HighlightRow { get; set; }

        public bool FirstSubmission { get; set; }

    }
}