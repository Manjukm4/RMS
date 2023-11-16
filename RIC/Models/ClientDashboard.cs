using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class ClientDashboard
    {
        public string idText { get; set; }
        public string RJ_Company { get; set; }

        public List<ClientMonthDashboardDate> Month { get; set; }


        public int? Submission_Month { get; set; }
        public int? Interview_Month { get; set; }
        public int? Hire_Month { get; set; }
        public int? Required_Month { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string EmpCd { get; set; }

    }
}