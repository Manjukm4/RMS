using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.MonthlyTarget
{
    public class MonthlyStaffingDirectorVM
    {
        public bool IsDisable { get; set; }
        public string EmpCd { get; set; }
        public string Month { get; set; }
        public string Role { get; set; }
        public int Year { get; set; }

        public List<SelectListItem> ReportingList { get; set; }
        public List<SelectListItem> YearList { get; set; }
        public List<SelectListItem> MonthList { get; set; }


        public List<MonthlyTargetsEmployeeSupervisor> EmpSupervisorList { get; set; }
        public List<MonthlyTargetsEmpSupervisorlevel2> EmpSupervisorlevel2 { get; set; }

        public StaffingDirector StaffingDirector { get; set; }
    }
}