using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Review
{
    public class HRDiscussionSendInvite
    {
        public int EmployeePrimaryId { get; set; }

        [Display(Name = "Emp ID")]
        public string EmpCd { get; set; }

        [Display(Name = "Employee Name")]
        public string CandidateName { get; set; }

        [Display(Name = "Reporting To")]
        public string ManagerName { get; set; }
    }
}