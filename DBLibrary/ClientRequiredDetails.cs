using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class ClientRequiredDetails
    {
        public string RJ_JobDiva_Ref { get; set; }

        public DateTime RJ_DateIssued { get; set; }
        public string RJ_Title { get; set; }
        public string RJ_Company { get; set; }
        public string Client_ref { get; set; }
         
    }
    public class TotalRecruiterdetails
    {
        public string EmpCd { get; set; }
        public string EmpName { get; set; }
        public string MgrName { get; set; }
        public int? submission { get; set; }
        public int? interview { get; set; }
        public int? hire { get; set; }
         
        public DateTime? firstactivity { get; set; }
        public DateTime? lastactivity { get; set; }
      
 
    }
}
