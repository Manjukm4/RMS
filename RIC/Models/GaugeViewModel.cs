using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class GaugeViewModel
    {
        public int SubmissionTarget { get; set; }
        public int InterviewsTarget { get; set; }
        public int HiresTarget { get; set; }
        public int Requirement { get; set; }

        public int TotalSubmissionMonth { get; set; }
        public int TotalInterviewsMonth { get; set; }
        public int TotalHiresMonth { get; set; }
        public int RemainingDays { get; set; }

        public DateTime StartDateOfMonth { get; set; }
        public DateTime EndDateOfMonth { get; set; }

    }
}