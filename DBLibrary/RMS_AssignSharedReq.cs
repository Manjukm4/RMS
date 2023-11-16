using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RMS_AssignSharedReq
    {
        [Key]
        public int RA_ID { get; set; }

        public int RA_SharedReqHDRID { get; set; }
        
        [StringLength(50)]
        public string RA_AssignedTo { get; set; }

        [StringLength(1000)]
        public string RA_Instructions { get; set; }

        [StringLength(50)]
        public string RA_AssignedBy { get; set; }
        public DateTime RA_AssignedDt { get; set; }

        public ICollection<RMS_SharedReqSubmittals> RMS_SharedReqSubmittals { get; set; }
    }
}
