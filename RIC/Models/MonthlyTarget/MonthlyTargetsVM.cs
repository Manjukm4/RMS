using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace RIC.Models.MonthlyTarget
{
    public class MonthlyTargetsVM
    {
        public bool IsDisable { get; set; }
        public string EmpCd { get; set; }
        public string Month { get; set; }
        
        public int Year { get; set; }
        
        public List<SelectListItem> ReportingList { get; set; }
        public List<SelectListItem> YearList { get; set; }
        public List<SelectListItem> MonthList { get; set; }
        
        public List<MonthlyTargetsEmployee> EmployeeList { get; set; }

        public MonthlyTargetsEmployeeSupervisor EmpSupervisor { get; set; }
        public MonthlyTargetsEmpSupervisorlevel2 EmpSupervisorlevel2 { get; set; }

    }

    public class MonthlyTargetsEmployee
    {
        public bool EditCheckBox { get; set; }
        public int MT_Id { get; set; }
        public string Emp_Cd { get; set; }
        public string Mgr_Cd { get; set; }
        
        public string Emp_Name { get; set; }

        public string Month { get; set; }

        public int Year { get; set; }

        public string Role { get; set; }

        public string SubmissionTarget { get; set; }

        public string InterviewTarget { get; set; }

        public string HireTarget { get; set; }

        public string Status { get; set; }

        public string Team_Comments { get; set; }

        public string Supervisor_Comments { get; set; }

        
        public string Margin { get; set; }

        public string Account_Name { get; set; }

        public string Starts { get; set; }


        public string Created_By { get; set; }

        public DateTime? Created_Date { get; set; }

        public string Updated_By { get; set; }

        public DateTime? Updated_Date { get; set; }

        public List<MonthlyTargetsEmployee> employees { get; set; }


    }


    public class MonthlyTargetsEmployeeSupervisor
    {
        public bool EditCheckBox { get; set; }
        public int MT_Id { get; set; }
        public string Emp_Cd { get; set; }
        public string Role { get; set; }
        public string Emp_Name { get; set; }

        public string Month { get; set; }
        public List<MonthlyTargetsEmployee> EmployeeList { get; set; }

        public int Year { get; set; }


        public string SubmissionTarget { get; set; }

        public string InterviewTarget { get; set; }

        public string HireTarget { get; set; }

        public string Status { get; set; }

        public string Team_Comments { get; set; }

        public string Supervisor_Comments { get; set; }


        public string Margin { get; set; }

        public string Account_Name { get; set; }

        public string Starts { get; set; }


        public string Created_By { get; set; }

        public DateTime? Created_Date { get; set; }

        public string Updated_By { get; set; }

        public DateTime? Updated_Date { get; set; }

        public static implicit operator List<object>(MonthlyTargetsEmployeeSupervisor v)
        {
            throw new NotImplementedException();
        }
    }
    public class MonthlyTargetsEmpSupervisorlevel2
    { 
    public int MT_Id { get; set; }
    public string Emp_Cd { get; set; }

    public string Emp_Name { get; set; }

    public string Month { get; set; }

    public int Year { get; set; }

        public string Role { get; set; }
    public string SubmissionTarget { get; set; }

    public string InterviewTarget { get; set; }

    public string HireTarget { get; set; }

    public string Status { get; set; }

    public string Team_Comments { get; set; }

    public string Supervisor_Comments { get; set; }


    public string Margin { get; set; }

    public string Account_Name { get; set; }

    public string Starts { get; set; }


    public string Created_By { get; set; }

    public DateTime? Created_Date { get; set; }

    public string Updated_By { get; set; }

    public DateTime? Updated_Date { get; set; }
    }

    public class StaffingDirector
    {
        public int MT_Id { get; set; }
        public string Emp_Cd { get; set; }

        public string Emp_Name { get; set; }

        public string Month { get; set; }

        public int Year { get; set; }

        public string Role { get; set; }
        public string SubmissionTarget { get; set; }

        public string InterviewTarget { get; set; }

        public string HireTarget { get; set; }

        public string Status { get; set; }

        public string Team_Comments { get; set; }

        public string Supervisor_Comments { get; set; }


        public string Margin { get; set; }

        public string Account_Name { get; set; }

        public string Starts { get; set; }


        public string Created_By { get; set; }

        public DateTime? Created_Date { get; set; }

        public string Updated_By { get; set; }

        public DateTime? Updated_Date { get; set; }
    }
}