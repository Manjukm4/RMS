using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_HRDiscussion_Dtl
    {
        [Key]
        public int RHD_ID { get; set; }

        [ForeignKey("RIC_HRDiscussion_Hdr")]
        public int RHD_RefID { get; set; }

        public string RHD_Description { get; set; }

        public string RHD_DiscussioNotes { get; set; }

        public string RHD_ActionItems { get; set; }

        public DateTime? RHD_NextMeetUpDate { get; set; }

        public string RHD_InternalCommnets { get; set; }

        public string RHD_AdditionalComments { get; set; }

        public virtual RIC_HRDiscussion_Hdr RIC_HRDiscussion_Hdr { get; set; }
    }
}
