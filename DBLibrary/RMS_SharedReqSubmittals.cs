using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RMS_SharedReqSubmittals
    {
        [Key]
        public int RS_ID { get; set; }

        [ForeignKey("RMS_AssignSharedReq")]
        public int RS_RefID { get; set; }

        public string RS_CandidateName { get; set; }
        public string RS_CandidateEmail { get; set; }
        
        public string RS_EnteredName { get; set; }
        public DateTime RS_EntredDate { get; set; }

        public string RS_SubmittedBy { get; set; }
        public DateTime RS_SubmittedDate { get; set; }

        [StringLength(50)]
        public string RS_CreatedBy { get; set; }
        public DateTime RS_CreatedDt { get; set; }

        [StringLength(1000)]
        public string RS_Comments { get; set; }

        public RMS_AssignSharedReq RMS_AssignSharedReq { get; set; }
    }
}
