using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class AccessLogResult
    {
        public string RC_Emp_Cd { get; set; }
        public string Category { get; set; }
        public int? CategoryCount { get; set; }
        //public string RE_Employee_Name { get; set; }
        public DateTime? RC_Date { get; set; }
        public string RC_Description { get; set; }
    }
}
