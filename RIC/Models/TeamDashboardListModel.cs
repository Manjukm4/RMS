using DBLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class TeamDashboardListModel
    {
        //public List<TeamDashboardMonth> Month { get; set; }
        public string  Id { get; set; }
        
        public string EmpCd { get; set; }

        public string Job_Diva_User_Name { get; set; }

        public string MgrCd { get; set; }

        public string MgrName { get; set; } 

        public int EmployeeLvl { get; set; }

        public string RE_Join_Date { get; set; }

        public List<TeamDashboardDate> Month { get; set; }

        public int? CheckedOut_Month { get; set; }
        public int? CheckedIn_Month { get; set; }



        public int ? Submission_Month { get; set; }
        public int? Interview_Month { get; set; }
        public int? Hire_Month { get; set; }

        public int? In_CallConnected_Month { get; set; }
        public int? In_VoiceMessages_Month { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? In_TotalDuration_Month { get; set; }

        public int? Out_CallConnected_Month { get; set; }
        public int? Out_VoiceMessages_Month { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? Out_TotalDuration_Month { get; set; }
        

        public double Submissions_Target { get; set; }
        public double Interviews_Target { get; set; }
        public double Hires_Target { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<AccessLogResult> AccessLogResults { get; set; }


    }
}