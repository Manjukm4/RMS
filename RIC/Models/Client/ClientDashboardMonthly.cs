using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Client
{
    public class ClientDashboardMonthly
    {
        public int RequirementsCount { get; set; }

        public int SubmissonCount { get; set; }

        public int InterviewCount { get; set; }

        public int TotalRecruiterCount { get; set; }

        public int ActiveRecruiterCount { get; set; }
        public int CompanyActiveRecruiterCount { get; set; }

        public int HireCount { get; set; }

        public int Monthnum { get; set; }

        public decimal Percentage { get; set; }
        public int Respond { get; set; }
        
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string MonthName { get; set; }

        public double SubByInterview { get; set; }

        public double SubByHire { get; set; }

        public double InterviewByHire { get; set; }

        public string RJ_Company { get; set; }

    }

    public class ClientSubmissionDetails
    {
        public string Company { get; set; }
        public DateTime DateIssued { get; set; }
        public string JobDiva_Ref { get; set; }
        public int submission2 { get; set; }
        public int submission4 { get; set; }
        public int submission8 { get; set; }
        public int submissionNxtD { get; set; }

        public int submissionNxt2D { get; set; }
        public int submissionNxt3D { get; set; }
        public int submissionNxt5D { get; set; }
    }
}