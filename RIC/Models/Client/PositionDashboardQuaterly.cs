using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Client
{
    public class PositionDashboardQuaterly
    {

        public string Quarter1 { get; set; }
        public string Quarter2 { get; set; }
        public string Quarter3 { get; set; }
        public string Quarter4 { get; set; }
        public string Categories { get; set; }

        public int Q1Positions { get; set; }
        public int Q2Positions { get; set; }
        public int Q3Positions { get; set; }
        public int Q4Positions { get; set; }
        [Display(Name = "Total Positions")]
        public int TotalPositions { get; set; }

        public int Q1Submission { get; set; }
        public int Q2Submission { get; set; }
        public int Q3Submission { get; set; }
        public int Q4Submission { get; set; }
        public int TotalSubmission { get; set; }

        public int Q1Interview { get; set; }
        public int Q2Interview { get; set; }
        public int Q3Interview { get; set; }
        public int Q4Interview { get; set; }
        public int TotalInterview { get; set; }

        public int Q1Hire { get; set; }
        public int Q2Hire { get; set; }
        public int Q3Hire { get; set; }
        public int Q4Hire { get; set; }
        public int TotalHire { get; set; }

        public double Q1InterviewByHire { get; set; }
        public double Q2InterviewByHire { get; set; }
        public double Q3InterviewByHire { get; set; }
        public double Q4InterviewByHire { get; set; }

        public double Q1SubByInterview { get; set; }
        public double Q2SubByInterview { get; set; }
        public double Q3SubByInterview { get; set; }
        public double Q4SubByInterview { get; set; }

        public double Q1SubByHire { get; set; }
        public double Q2SubByHire { get; set; }
        public double Q3SubByHire { get; set; }
        public double Q4SubByHire { get; set; }

        [Display(Name = "I/S Ratio")]
        public double SubByInterview { get; set; }

        [Display(Name = "H/S Ratio")]
        public double SubByHire { get; set; }

        [Display(Name = "H/I Ratio")]
        public double InterviewByHire { get; set; }

        public DateTime Q1StartDate { get; set; }

        public DateTime Q1EndDate { get; set; }

        public DateTime Q2StartDate { get; set; }

        public DateTime Q2EndDate { get; set; }

        public DateTime Q3StartDate { get; set; }

        public DateTime Q3EndDate { get; set; }

        public DateTime Q4StartDate { get; set; }

        public DateTime Q4EndDate { get; set; }
        public double TotalSubByInterview { get; set; }
        public double TotalSubByHire { get; set; }
        public double TotalInterviewByHire { get; set; }
    }
}