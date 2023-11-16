using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class TeamMatrixResult
    {
        public string TmId { get; set; }
        public DateTime TmDate { get; set; }
        public string TmDay { get; set; }
        public string TmEmpCd { get; set; }
        public string TmEmpName { get; set; }
        public string TmMgrCd { get; set; }
        public string TmMgrName { get; set; }
        public string RE_Join_Date { get; set; }
        public int TmEmpLvl { get; set; }
        public double TmSubmissionTarget { get; set; }
        public double TmInterviewTarget { get; set; }
        public double TmHireTarget { get; set; }
        public int? TmSubmission { get; set; }
        public int? TmInterviews { get; set; }
        public int? TmHires { get; set; }
        public int? TmCallConnectedIn { get; set; }
        public int? TmVoiceMessageIn { get; set; }
        public double? TmTotalDurationIn { get; set; }
        public int? TmCallConnectedOut { get; set; }
        public int? TmVoiceMessageOut { get; set; }
        public double? TmTotalDurationOut { get; set; }
        public int? TmcheckedOutCount { get; set; }
        public int? TmcheckedInCount { get; set; }
        public string Attendance { get; set; }
        public List<AccessLogResult> AccessLogResults { get; set; }
        public List<AccessLogResult> Products { get; set; }
        //public AccessLog 

    }
}
