using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Dashboard
{
    public class AccessLogDetails
    {
        public string EmpCd { get; set; }
        public string EmployeeName { get; set; }
        public string Category { get; set; }
        public DateTime? RC_Date { get; set; }
        public string Description { get; set; }
    }
}