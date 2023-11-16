using DBLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class JobReportPartial
    {
        public string idText { get; set; }
      public  List<JobRepoartWeek> JobReportWeek { get; set; }
      public List<CallsStatusWeek> CallStatisticsWeek { get; set; }

      
    }
}