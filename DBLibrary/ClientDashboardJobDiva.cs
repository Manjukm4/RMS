using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DBLibrary
{
 public   class ClientDashboardJobDiva
    {
        [Display(Name = "CompanyName")]
        public string Company { get; set; }
        [Display(Name = "EmployeeCode")]
        public string re_emp_cd { get; set; }
        [Display(Name = "EmployeeName")]
        public string submitted_by { get; set; }
        public string submittedBy_Email { get; set; }
        [Display(Name = "JI/JR ratio")]
        public decimal percentage { get; set; }
        public int responded { get; set; }
        public int jobCount { get; set; }


    }
}
