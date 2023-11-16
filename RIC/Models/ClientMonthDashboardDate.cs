using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class ClientMonthDashboardDate
    {
        public DateTime date { get; set; }

        public int? Submission { get; set; }
        public int? Interview { get; set; }
        public int? Hire { get; set; }
        public int? Jobs_Issued { get; set; }
        
    }
}