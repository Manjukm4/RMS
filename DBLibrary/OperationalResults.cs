using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DBLibrary
{
   public class OperationalResult
    {
        public int OpCount { get; set; }
        public int? OpHour { get; set; }
        public DateTime? OpCallTime { get; set; }
        public string OpCallType { get; set; }
    }

   public class RecruitDetails
   {
       public int forHours { get; set; }
       public int submissionCount { get; set; }
       public int interviewCount { get; set; }
       public int hireCount { get; set; }

      //Added by ashish 26-06-2018. 
       public DateTime FromDate { get; set; }
       public DateTime ToDate { get; set; }
   }
}
