using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RMS_SharedReq_HDR
    {

        [Key]
        public int RS_ID { get; set; }

        public int RS_JobID { get; set; }

        [StringLength(100)]
        public string RS_JobDivaRef { get; set; }


        [StringLength(100)]
        public string RS_Req { get; set; }

        [StringLength(10)]
        public string RS_RMSJobStatus { get; set; }

        [StringLength(10)]
        public string RS_JobDivaStatus { get; set; }
        public DateTime RS_CreatedDt { get; set; }

        [StringLength(50)]
        public string RS_CreatedBy { get; set; }

        [StringLength(50)]
        public string RS_UpdatedBy { get; set; }
        public DateTime? RS_UpdatedDt { get; set; }


        public virtual ICollection<RMS_SharedReq_Dtl> RMS_SharedReq_Dtl { get; set; }

        public virtual ICollection<RMS_SharedReqNotes> RMS_SharedReqNotes { get; set; }

        public virtual ICollection<RMS_JobChecked_Info> JobChecked_Info { get; set; }

        public virtual ICollection<RMS_JobCheckedNotes> RMS_JobCheckedNotes { get; set; }


    }
}
