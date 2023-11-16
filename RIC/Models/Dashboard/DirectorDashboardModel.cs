using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Dashboard
{
    public class DirectorDashboardModel
    {
        public  List<DirectorDashboardTable> DirectorDashboardTable { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalSubmissions { get; set; }
        public int TotalInterviews { get; set; }
        public int TotalHires { get; set; }
    }
    public class DirectorDashboardTable
    {
        public string EmpCd { get; set; }
        public string UserName { get; set; }
        public int Submissions { get; set; }
        public int Interviews { get; set; }
        public int Hires { get; set; }

    }

}