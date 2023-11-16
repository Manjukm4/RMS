using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Dashboard
{
    public class ProgressReportPartialVM
    {
        public string EmpCd { get; set; }
        public int SubmissionTarget { get; set; }
        public int InterviewTarget { get; set; }
        public int HiresTarget { get; set; }

        public int Submissions { get; set; }
        public int Interviews { get; set; }
        public int Hires { get; set; }

        public int SubmissionsPer { get; set; }
        public int InterviewsPer { get; set; }
        public int HiresPer { get; set; }
        public bool IsActive { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }
         [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public List<ProgressReportEmployeeList> EmployeeList { get; set; }


        public  string Role { get; set; }


    }

    public class ProgressReportEmployeeList
    {
        public string EmpCd { get; set; }
        public string EmployeeName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        
        public int Submissions { get; set; }
        public int Interviews { get; set; }
        public int Hires { get; set; }

        public int SubmissionsTarget { get; set; }
        public int InterviewTarget { get; set; }
        public int HiresTarget { get; set; }

        public int SubmissionsProgress { get; set; }
        public int InterviewsProgress { get; set; }
        public int HiresProgress { get; set; }

        public string SubmissionsProgressColor { get; set; }
        public string InterviewsProgressColor { get; set; }
        public string HiresProgressColor { get; set; }

        public string AvgProgressClass { get; set; }

    }

    


}