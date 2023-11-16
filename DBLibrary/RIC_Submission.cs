using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class RIC_Submission
    {
       [Key]
       public int RS_Id { get; set; }
       
       [StringLength(50)]
       [Display(Name = "Employee ID")]
       public string RS_EmpId { get; set;}

       [StringLength(50)]
       [Display(Name="Employee Name")]
       public string RS_RecruterName { get; set; }

       [Display(Name="From Date")]
       public DateTime RS_FromDt { get; set; }

       [Display(Name="To Date")]
       public DateTime RS_ToDt { get; set; }

       [StringLength(500)]
       [Display(Name="Email")]
       public string RS_Email { get; set; }

       [StringLength(50)]
       [Display(Name="Division")]
       public string RS_Division { get; set; }

       [Display(Name="Submission")]
       public int RS_Submission { get; set; }

       [Display(Name="Interviews")]
       public int RS_Interviews { get; set; }
        [Display(Name = "Hires")]
       public int RS_Hires { get; set; }
        [Display(Name = "Net Bill Hires/ Hour")]
       public float RS_NetbillHires_Hour { get; set; }

       public float RS_CostOfHires_Hour { get; set; }

       public float RS_MarginOfHires_Hour { get; set; }

       public float RS_MarginOfHires_Per { get; set; }
    }
}
