using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RMS_JobCheckedNotes
    {
        [Key]
        public int RJ_ID { get; set; }

        [ForeignKey("RMS_SharedReq_HDR")]
        public int RJ_RefHDRID { get; set; }

        [StringLength(1000)]
        public string RJ_Notes { get; set; }

        [StringLength(50)]
        public string RJ_CreatedBy { get; set; }
        public DateTime RJ_CreatedDt { get; set; }


        public RMS_SharedReq_HDR RMS_SharedReq_HDR { get; set; }
    }
}
