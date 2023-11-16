using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Dashboard
{
    public class TreeViewPartial
    {

        [Display(Name = "Employee ID")]
        public string EmpCD { get; set; }

        [Display(Name = "Manager ID")]
        public string MgrCD { get; set; }

        public string ManagerName { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        public int EmpLvl { get; set; }

       // public List<TreeViewPartial> TreeNode { get; set; }
       
    }
}