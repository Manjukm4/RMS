using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Dashboard
{
    public class SubmissionPartialTable
    {
        public List<Days> Days { get; set; }
    }

    public class Days 
    {
        public String Day { get; set; }
        public List<Hours> Hours { get; set; }   
    }

    public class Hours 
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Submissions { get; set; }
    }

}