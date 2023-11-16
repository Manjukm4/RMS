using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class ClientDashboardMonthlyDB
    {
        public string RJ_Company { get; set; }

        public int Monthnum { get; set; }

        public int RequirementsCount { get; set; }
        public int RecruiterCount { get; set; }
        public int Active_rec_cnt { get; set; }

        public int SubmissonCount { get; set; }

        public int InterviewCount { get; set; }

        public int HireCount { get; set; }
        public decimal Percentage { get; set; }
        public int Respond { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string MonthName { get; set; }

       
    }

    public class ClientSubmissionDetailsDB
    {
        public string RJ_Company { get; set; }
        public DateTime RJ_DateIssued { get; set; }
        public string RJ_JobDiva_Ref { get; set; }
        public int submission2 { get; set; }
        public int submission4 { get; set; }
        public int submission8 { get; set; }
        public int submissionNxtD { get; set; }

        public int submissionNxt2D { get; set; }
        public int submissionNxt3D { get; set; }
        public int submissionNxt5D { get; set; }

    }
}
