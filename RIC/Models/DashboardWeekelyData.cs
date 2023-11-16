using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

    public class DashboardWeekelyData
    {

    //public string Date { get; set; }
    public string EmpCd { get; set; }

        public int ? Submissions { get; set; }

       // public int ? Interviews { get; set; }


        //public int ? Hires { get; set; }
         public int ? RC_CallConnectdOut { get; set; }
    public string RC_Req_Worked_On { get; set; } 
        public int ? RC_Dailed { get; set; }

    //[Column ("Break In Minutes")]
    public int ? Break_In_Minutes { get; set; }

    public string RE_Employee_Name { get; set; }

        public string MgrName { get; set; }

        public DateTime ? Login_Time { get; set; }

        public string RC_Description { get; set; }

        public DateTime ? Last_Activity_Timing { get; set; }
    }

