using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class RIC_Incentive
    {
       [Key]
       public int RI_ID { get; set; }

       [StringLength(50)]
       public string RI_EmpCd { get; set; }

       [StringLength(200)]
       public string RI_RecruitedBy { get; set; }

       [StringLength(200)]
       public string RI_TeamLead { get; set; }

       [Display(Name="Candidate")]
       [StringLength(200)]
       public string RI_Candidate { get; set; }

       [Display(Name = "Company")]
       [StringLength(200)]
       public string RI_Company { get; set; }

       [Display(Name = "Start Date")]
       public DateTime RI_StartDate { get; set; }

       [Display(Name = "End Date")]
       public DateTime ? RI_EndDate { get; set; }

       [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
       [Display(Name = "Net Margin ($)")]
       public float RI_NetMargin { get; set; }

      
       [Display(Name = "Jan Incentive")]
       public float ? RI_Jan_Incentive { get; set; }

       
       [Display(Name = "Feb Incentive")]
       public float ? RI_Feb_Incentive { get; set; }
       
       public float ? RI_Mar_Incentive { get; set; }
       
       public float ? RI_Apr_Incentive { get; set; }
       
       public float ? RI_May_Incentive { get; set; }
       
       public float ? RI_Jun_Incentive { get; set; }
       
       public float ? RI_Jul_Incentive { get; set; }
       
       public float ? RI_Aug_Incentive { get; set; }
       
       public float ? RI_Sep_Incentive { get; set; }
       
       public float ? RI_Oct_Incentive { get; set; }
       
       public float ? RI_Nov_Incentive { get; set; }
       
       public float ? RI_Dec_Incentive { get; set; }

       public int? RI_Year { get; set; }

       public float? RI_TotalRecurringPaid { get; set; }

       public float? RI_OneTimeIncentive{ get; set; }

       public float? RI_finalDifference { get; set; }

       public string RI_Remarks { get; set; }




    }
}
