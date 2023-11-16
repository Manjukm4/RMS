using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RMS_JobChecked_Info
    {
        [Key]
        public int RJ_ID { get; set; }

        [ForeignKey("RMS_SharedReq_HDR")]
        public int RJ_RefHDRID { get; set; }

        [StringLength(10)]
        public string RJ_CheckedTo { get; set; }

        public DateTime? RJ_CheckedInDt { get; set; }

        public DateTime RJ_CheckedOutDt { get; set; }

        [StringLength(10)]
        public string RJ_AssignedBy { get; set; }
        public DateTime RJ_AssignedDt { get; set; }
        public virtual RMS_SharedReq_HDR RMS_SharedReq_HDR { get; set; }
    }
}
