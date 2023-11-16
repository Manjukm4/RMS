using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
  public  class RIC_SubmissionRule
    {
      [Key]
      public int RS_Id { get; set; }
      
      [Required]
      [Display(Name="Level")]
      public int RS_Level { get; set;}
      
      [Required]
      [StringLength(20)]
      [Display(Name="Experience")]
      public string RS_Experience { get; set; }

      [Display(Name="Requirement")]
      public int RS_Requirement { get; set; }

      [Required]
      [Display(Name="Monthly Submissions")]
      public int RS_MonthlySubmissions { get; set; }

      [Required]
      [Display(Name="Monthly Interviews")]
      public int RS_Monthly_Interviews { get; set; }

      [Required]
      [Display(Name = "Monthly Hire")]
      public int RS_Monthyl_Hire { get; set; }


      [Required]
      [Display(Name="Minimum Submissions")]
      public int RS_MinimumSubs { get; set; }

      [Required]
      [Display(Name="Minimum Interviews")]
      public int RS_Minimum_Interviews { get; set; }

      [Required]
      [Display(Name = "Minimum Hire")]
      public int RS_Minimum_Hire { get; set; }



      public virtual ICollection<RIC_Employee> RIC_Employee { get; set; }
    }
}
