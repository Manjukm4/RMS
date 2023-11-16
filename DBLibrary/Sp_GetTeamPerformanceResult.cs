using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class Sp_GetTeamPerformanceResult
    {
        public string EmpCd { get; set; }
        public string EmployeeName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int SubmissionTarget { get; set; }
        public int InterviewTarget { get; set; }
        public int HiresTarget { get; set; }
        public int Submissions { get; set; }
        public int Interviews { get; set; }
        public int Hires { get; set; }
        public int SubmissionProgress { get; set; }
        public int InterviewProgress { get; set; }
        public int HiresProgress { get; set; }

        public DateTime? ResignDate  { get; set; }

    }
}
