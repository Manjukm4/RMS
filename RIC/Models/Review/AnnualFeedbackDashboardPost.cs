using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Review
{
    public class AnnualFeedbackDashboardPost
    {
        public string EmpID { get; set; }
        public int FormID { get; set; }
        public string LeadReviewerID { get; set; }
        public List<string> ReviewerList { get; set; }
    }
}