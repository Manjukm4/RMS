using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Account
{
    public class TargetSubIntHire
    {
        [Display(Name = "Employee ID")]
        public string RE_Emp_Cd { get; set; }

        [Display(Name = "Employee Name")]
        public string RE_Jobdiva_User_Name { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Joining Date in Sunrise")]

        public DateTime? RE_Joining_Date { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Duration in Sunrise")]

        public string RE_Resign_Date { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Effective From Date")]
        public DateTime? EffectiveFromDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Effective To Date")]
        public DateTime? EffectiveToDate { get; set; }

        [Display(Name = "Submissions")]
        public int RS_MonthlySubmissions { get; set; }
        [Display(Name = "Interviews")]
        public int RS_Monthly_Interviews { get; set; }
        [Display(Name = "Hires")]
        public int RS_Monthyl_Hire { get; set; }
    }
}