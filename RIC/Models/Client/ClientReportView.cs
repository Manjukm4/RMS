using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Client
{
    public class ClientReportView
    {

        public ClientReportView()
        {
            //bind the select list items.
            this.SubmissionSelectList =new List<SelectListItem>(){
                    new SelectListItem() { Text=">=",Value=">" },
                    new SelectListItem() {Text="<=",Value="<"  }
                };
            this.InterviewsSelectList = new List<SelectListItem>(){
                    new SelectListItem() { Text=">=",Value=">" },
                    new SelectListItem() {Text="<=",Value="<"  }
            };

            this.HiresSelectList = new List<SelectListItem>(){
                    new SelectListItem() { Text=">=",Value=">" },
                    new SelectListItem() {Text="<=",Value="<"  }
            };

            this.FilterData = new List<ClientOperationalList>();
        }

        [Display(Name = "Submissions")]
        public int Submissions { get; set; }

        [Display(Name = "Interviews")]
        public int Interviews { get; set; }


        [Display(Name = "Hires")]
        public int Hires { get; set; }

    
        [Display(Name = "Client")]
        public string Client { get; set; }

        [Display(Name = "Year")]
        public int? Year { get; set; }

        public List<SelectListItem> SubmissionSelectList { get; set; }
        public List<SelectListItem> ClientList { get; set; }

        public List<SelectListItem> InterviewsSelectList { get; set; }

        public List<SelectListItem> HiresSelectList { get; set; }

        public List<SelectListItem> YearSelectList { get; set; }
        public List<SelectListItem> AMSelectList { get; set; }



        //public List<SelectListItem> ExpSelectList { get; set; }

        public string SubSelected { get; set; }
        [Required]
        public string ClientName{ get; set; }
        

        public int GetYear { get; set; }

        public string InterviewSelected { get; set; }
        public string HireSelected { get; set; }
        public string ClientSelected { get; set; }
        //public string ExpSelected { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        //[Display(Name = "Include (TL)")]
        //public bool IncludeTL { get; set; }

        //[Display(Name = "Include Inactive Employees")]
        //public bool RemoveInactiveMember { get; set; }

        public List<ClientOperationalList> FilterData { get; set; }
    }

    public class ClientOperationalList
    {
        [Display(Name = "Employee Code")]
        public string EmpCd { get; set; }

        [Display(Name = "Client")]
        public string Client { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        //[Display(Name = "Repoarting To")]
        //public string TeamLeadName { get; set; }

        [Display(Name = "Submissions")]
        public int Submissions { get; set; }

        [Display(Name = "Interviews")]
        public int Interviews { get; set; }

        [Display(Name = "Hires")]
        public int Hires { get; set; }


        public int count { get; set; }

        [Display(Name = "JI/JR Ratio")]
        public decimal JdPercentage { get; set; }

        [Display(Name = "I/S Ratio")]
        public double SubByInterview { get; set; }

        [Display(Name = "H/S Ratio")]
        public double SubByHire { get; set; }

        [Display(Name = "H/I Ratio")]
        public double InterviewByHire { get; set; }

    }

    public class ClientMonthlyReportview
    {
        [Display(Name = "Quater")]
        public List<SelectListItem> QuaterSelectList { get; set; }
        [Display(Name = "AM")]
        public List<SelectListItem> AMSelectList { get; set; }
        [Display(Name = "Client")]
        public List<SelectListItem> ClientList { get; set; }
        [Display(Name = "Month")]
        public List<SelectListItem> MonthList { get; set; }
    }

    public class ClientSubmissionView
    {
        [Display(Name = "AM")]
        public List<SelectListItem> AMSelectList { get; set; }

        [Display(Name = "Client")]
        public List<SelectListItem> ClientList { get; set; }
        [Display(Name = "Year")]
        public List<SelectListItem> YearSelectList { get; set; }
    }
}