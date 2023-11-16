using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class GetIncentiveResult
    {
       [Display(Name = "Details / Months")]
       public string Details { get; set; }

       [Display(Name="Jan")]
       public string  JANUARY { get; set; }

       [Display(Name="Feb")]
       public string FEBRUARY { get; set; }

       [Display(Name="Mar")]
       public string MARCH { get; set; }

       [Display(Name="Apr")]
       public string APRIL { get; set; }

       [Display(Name="May")]
       public string MAY { get; set; }

       [Display(Name="Jun")]
       public string JUNE { get; set; }

       [Display(Name="Jul")]
       public string JULY { get; set; }

       [Display(Name="Aug")]
       public string AUGUST { get; set; }

       [Display(Name="Sep")]
       public string SEPTEMBER { get; set; }

       [Display(Name="Oct")]
       public string OCTOBER { get; set; }

       [Display(Name="Nov")]
       public string NOVEMBER { get; set; }

       [Display(Name="Dec")]
       public string DECEMBER { get; set; }

        public List<IncentiveResultData> IncentiveResultData { get; set; }

       //[Display(Name = "Month")]
       // public string Month { get; set; }

       //[Display(Name = "Starts On Month")]
       // public int TotalStarts { get; set; }

       //[Display(Name = "Margin On Month ($)")]
       // public double MonthMargin { get; set; }

       //[Display(Name = "Incentives (₹)")]
       // public int Incentive { get; set; }

       //[Display(Name = "Total Starts On Billing")]
       // public int TotalBiling { get; set; }

       // [Display(Name = "Total Margin ($)")]
       // public double TotalMargin { get; set; }
    }

    public class IncentiveResultData
    {
        public float? RI_TotalRecurringPaid { get; set; }
    
        public float? RI_OneTimeIncentive { get; set; }

        public float? RI_finalDifference { get; set; }

    }
}
