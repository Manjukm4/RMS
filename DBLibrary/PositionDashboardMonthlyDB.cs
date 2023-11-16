using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class PositionDashboardMonthlyDB
    {
        public long Monthnum { get; set; }
        public int PositionCount { get; set; }
        public int SubmissonCount { get; set; }
        public int InterviewCount { get; set; }
        public int HireCount { get; set; }
       // public int PositionCount { get; set; }

        public string MonthName { get; set; }
        public string Position_Type { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
