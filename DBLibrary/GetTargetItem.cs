using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class GetTargetItem
    {

       
        public string RE_Emp_Cd { get; set; }
        public string RE_Jobdiva_User_Name { get; set; }     
        public string Designation { get; set; }     
        public DateTime? RE_Joining_Date { get; set; }    
        public string RE_Resign_Date { get; set; }      
        public DateTime? EffectiveFromDate { get; set; }      
        public DateTime? EffectiveToDate { get; set; }       
        public int RS_MonthlySubmissions { get; set; }       
        public int RS_Monthly_Interviews { get; set; }    
        public int RS_Monthyl_Hire { get; set; }
    }
}
