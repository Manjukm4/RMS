using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Operational
{
    public class OperationalReportView
    {
        public OperationalReportView()
        {
            //bind the select list items.
            this.SubmissionSelectList =new List<SelectListItem>(){
                    new SelectListItem() { Text=">=",Value=">" },
                    new SelectListItem() {Text="<=",Value="<"  }
                };
            this.CallsSelectList = new List<SelectListItem>(){
                    new SelectListItem() { Text=">=",Value=">" },
                    new SelectListItem() {Text="<=",Value="<"  }
            };

            this.FilterData = new List<OperationalList>();
        }

        [Display(Name = "Submissions")]
        public int  Submissions { get; set; }
        
        [Display(Name="Calls (Out)")]
        public int  Calls { get; set; }
        public List<SelectListItem> SubmissionSelectList { get; set; }
        public List<SelectListItem> CallsSelectList { get; set; }

        public List<SelectListItem> ExpSelectList { get; set; }

        public string SubSelected { get; set; }
        public string CallSelect { get; set; }

        public string ExpSelected { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        [Display(Name = "Include (TL)")]
        public bool IncludeTL { get; set; }

        [Display(Name = "Include Inactive Employees")]
        public bool RemoveInactiveMember { get; set; }

        public List<OperationalList> FilterData { get; set; }
    }
    public class OperationalList
    {
		 [Display(Name = "Employee Code")]
		 public string EmpCd { get; set; }

        [Display(Name="Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name="Reporting To")]
        public string TeamLeadName { get; set; }

        [Display(Name="Submissions")]
        public int Submissions { get; set; }

        [Display(Name = "Interviews")]
        public int Interviews { get; set; }

        [Display(Name = "Hires")]
        public int Hires { get; set; }

        [Display(Name = "Call Connected(In)")]
        public int CallConnectedIn { get; set; }
        
        [Display(Name = "Voice Messages(In)")]
        public int VoiceMessageIn { get; set; }
        
        [Display(Name = "Call Connected(Out)")]
        public int CallConnectedOut { get; set; }
        
        [Display(Name = "Voice Messages(Out)")]
        public int VoiceMessageOut { get; set; }
    }
}