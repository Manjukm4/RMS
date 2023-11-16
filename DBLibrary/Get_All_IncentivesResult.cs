using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class Get_All_IncentivesResult
    {
        [Display(Name = "Employee ID")]
        public string RI_EmpCd { get; set; }

        [Display(Name = "Candidate")]
        public string RI_Candidate { get; set; }

        [Display(Name = "Company")]
        public string RI_Company { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime RI_StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime? RI_EndDate { get; set; }

       
        [Display(Name = "Net Margin ($)")]
        public float? RI_NetMargin { get; set; }

        [Display(Name = "Month")]
        public string RI_Month { get; set; }

        [Display(Name = "Incentive (₹)")]
        public float? RI_Incentive { get; set; }

        public int RI_Year { get; set; }

        [Display(Name = "Remarks")]
        public string RI_Remarks { get; set; }

        [Display(Name = "Total Recurring Paid")]
        public float? RI_TotalRecurringPaid { get; set; }

        [Display(Name = "One Time Incentive")]
        public float? RI_OneTimeIncentive { get; set; }

        [Display(Name = "Final Difference")]
        public float? RI_finalDifference { get; set; }

    }
}
