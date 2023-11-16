using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RMS_SharedReqNotes
    {
        [Key]
        public int RS_ID { get; set; }

        [ForeignKey("RMS_SharedReq_HDR")]
        public int RS_RefHDRID { get; set; }

        [StringLength(1000)]
        public string RS_Notes { get; set; }

        [StringLength(50)]
        public string RS_CreatedBy { get; set; }
        public DateTime RS_CreatedDt { get; set; }


        public RMS_SharedReq_HDR RMS_SharedReq_HDR { get; set; }
    }
}
