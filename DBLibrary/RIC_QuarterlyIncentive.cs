using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_QuarterlyIncentive
    {
        [Key]
        public int RI_ID { get; set; }

        [Display(Name = "Employee Id")]
        [StringLength(50)]
        public string RI_EmpCd { get; set; }

        [Display(Name = "Employee Name")]
        [StringLength(200)]
        public string RI_EmployeeName { get; set; }

        [Display(Name = "Team Lead")]
        [StringLength(200)]
        public string RI_TeamLead { get; set; }

        [Display(Name = "Candidate")]
        [StringLength(200)]
        public string RI_Candidate { get; set; }

        [Display(Name = "Company")]
        [StringLength(200)]
        public string RI_Company { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime RI_StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime? RI_EndDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Net Margin")]
        public float RI_NetMargin { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Incentives")]
        public float RI_Incentives { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(2000)]
        public string RI_Remarks { get; set; }

        [Display(Name = "Incentive Category")]
        [StringLength(200)]
        public string RI_IncentiveCategory { get; set; }




    }
}
