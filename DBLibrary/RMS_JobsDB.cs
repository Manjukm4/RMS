using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DBLibrary
{
    public class RMS_JobsDB
    {
        public int RS_ID { get; set; }
        public int RS_JobID { get; set; }
        public string RS_JobDivaRef { get; set; }
        public string RS_JobTitle { get; set; }
        public DateTime RS_JobIssueDate { get; set; }
        public string RS_Company { get; set; }
        public string RS_JobDivaStatus { get; set; }
        public string RS_WorkLocation { get; set; }
        public string RS_Priority { get; set; }
        public string RS_Division { get; set; }
        public string RS_Category { get; set; }
        public string RS_City { get; set; }
        public string RS_State { get; set; }
        public float? RS_BillRate { get; set; }
        public float? RS_PayRate { get; set; }
        public int? RS_MaxSubAllowed { get; set; }
        public int? RS_InternalSub { get; set; }
        public int? RS_ExternalSub { get; set; }
        public DateTime RS_CreatedDt { get; set; }
        public string RS_RMSJobStatus { get; set; }
        public bool IsCheckedOut { get; set; }
        public int? RJ_ID { get; set; }
        public string Clientid { get; set; }

    }
}
