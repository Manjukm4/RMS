using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DBLibrary
{
   public  class RIC_ReportingHistory
    {
       [Key]
       public int RR_ID { get; set; }

       [StringLength(50)]
       [Display(Name="Employee ID")]
       public string RR_EmpCD { get; set; }

       [StringLength(50)]
       [Display(Name="Manager ID")]
       public string RR_MgrCD { get; set; }

       [Display(Name="From Date")]
       public DateTime ? RR_FromDate { get; set; }

       [Display(Name="To Date")]
       public DateTime ? RR_ToDate { get; set; }

       [Display(Name = "Created Date")]
       public DateTime RR_CreatedDate { get; set; }

       [StringLength(50)]
       [Display(Name="Updated By")]
       public string RR_UpdatedBy { get; set; }

       [NotMapped]
       [Display(Name = "Reporting To")]
       public IEnumerable<SelectListItem> ManagerList { get; set; }
       [NotMapped]
       //[Display(Name = "Reporting Manager")]
       public int MgrId { get; set; }
       [NotMapped]
       [Display(Name = "Jobdiva User Name")]
       public string  JobdivaUsername { get; set; }

       [NotMapped]
       [Display(Name="Employee Name")]
       public string  Employee_Name{get;set;}
       [NotMapped]
       [Display(Name = "Reporting To")]
       public string Mgr_Name{get;set;}

       [NotMapped]
       public int EmpLevel { get; set; }

       public ICollection<RIC_Targets> Targets { get; set; }
    }

   public class ReportingHistoryResult
   {

       public int RR_ID { get; set; }

       [Display(Name = "Employee ID")]
       public string RR_EmpCD { get; set; }

       [Display(Name = "Manager ID")]
       public string RR_MgrCD { get; set; }

       [Display(Name = "From Date")]
       public DateTime? RR_FromDate { get; set; }

       [Display(Name = "To Date")]
       public DateTime? RR_ToDate { get; set; }

       [Display(Name = "Created Date")]
       public DateTime RR_CreatedDate { get; set; }

       [Display(Name = "Updated By")]
       public string RR_UpdatedBy { get; set; }

       [Display(Name = "Reporting To")]
       public IEnumerable<SelectListItem> ManagerList { get; set; }

       public int MgrId { get; set; }

       [Display(Name = "Jobdiva User Name")]
       public string JobdivaUsername { get; set; }


       [Display(Name = "Employee Name")]
       public string Employee_Name { get; set; }

       [Display(Name = "Reporting To")]
       public string Mgr_Name { get; set; }

       public int EmpLevel { get; set; }
      
   }

}
