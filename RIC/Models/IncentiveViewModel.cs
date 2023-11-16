using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class IncentiveViewModel
    {
        [Display(Name="Month")]
        public DateTime Month { get; set; }

        [Display(Name="Total Start")]
        public int TotalStart { get; set; }

        [Display(Name="Total Margin")]
        public float TotalMargin { get; set; }

        [Display(Name = "Total Incentive")]
        public float TotalIncentive { get; set; }
    }
}