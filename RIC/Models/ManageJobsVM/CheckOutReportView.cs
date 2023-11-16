using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.ManageJobsVM
{
    public class CheckOutReportView
    {

        [Display(Name = "JobStatus")]
        public string JobStatus { get; set; }

        [Display(Name = "Year")]
        public int? Year { get; set; }

        public List<SelectListItem> JobStatusList { get; set; }
        
        public List<SelectListItem> YearSelectList { get; set; }


        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }


        public int GetYear { get; set; }

        public string GetJobStatus { get; set; }


        public string JobdivaRefSelected { get; set; }
        public DateTime? JobIssueDateSelected { get; set; }
        public string ClientIDSelected { get; set; }


        public List<CheckOutOperationalList> FilterData { get; set; }

        public List<NoActionReportList> NoActionReport { get; set; }

    }


    public class NoActionReportList
    {

        [Display(Name = "JobDiva Ref Number")]
        public string JobDivaRef { get; set; }


        [Display(Name = "Client ID")]
        public string ClientRef { get; set; }


        [Display(Name = "Job Issue Date")]
        public DateTime JobIssueDate { get; set; }

        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Display(Name = "Company Name")]
        public string Company { get; set; }


        [Display(Name = "JobDiva Status")]
        public string JobDivaStatus { get; set; }


    }
    public class CheckOutOperationalList
    {

        [Display(Name = "JobDiva Ref Number")]
        public string JobDivaRef { get; set; }


        [Display(Name = "Client ID")]
        public string ClientRef { get; set; }


        [Display(Name = "Job Issue Date")]
        public DateTime JobIssueDate { get; set; }

        [Display(Name = "Employee Code")]
        public string EmpCd { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }


        [Display(Name = "Last Checked-In Date")]
        public DateTime? CheckedInDate { get; set; }

        [Display(Name = "First Checked-out Date")]
        public DateTime CheckedoutDate { get; set; }


        [Display(Name = "Status")]
        public string Jobstatus { get; set; }

    }
}


        
