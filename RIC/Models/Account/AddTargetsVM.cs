using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Account
{
    public class AddTargetsVM
    {
        public bool SelectAllCheck { get; set; }
        public string EmpCd { get; set; }
        //public string Month { get; set; }
        public string Quarter { get; set; }

        public int Year { get; set; }
        public string PreviousMonth { get; set; }
        public List<SelectListItem> ReportingList { get; set; }
        public List<SelectListItem> YearList { get; set; }
        // public List<SelectListItem> MonthList { get; set; }
        public List<SelectListItem> QuarterList { get; set; }
        public List<AddTargetEmployeeList> EmployeeList { get; set; }
    }
    public class AddTargetEmployeeList
    {
        public bool EditCheckBox { get; set; }
        public int TargetId { get; set; }
        public int ReportingID { get; set; }
        public string EmpCd { get; set; }
        public string MgrCd { get; set; }
        public string EmployeeName { get; set; }
        public string ExpInSunrise { get; set; }
        public int Year { get; set; }
        // public string Month { get; set; }
        public string Quarter { get; set; }
        public string TotalExp { get; set; }
        public string ReportingTo { get; set; }
        public float ? SubmissionTarget { get; set; }
        public float ? InterviewTarget { get; set; }
        public float ? HiresTarget { get; set; }
        public string Comments { get; set; }
      //  public bool Edit { get; set; } 
    }

}