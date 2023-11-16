using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Client
{
    public class PositionDashboardMonthly
     {
        public int Position { get; set; }
        public int Submission { get; set; }
        public int Interview { get; set; }
        public int Hire { get; set; }
       // public int Position { get; set; }
        public string Categories { get; set; }
        public double SubByInterview { get; set; }

        public double SubByHire { get; set; }

        public double InterviewByHire { get; set; }

        public long Monthnum { get; set; }
        public string MonthName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}