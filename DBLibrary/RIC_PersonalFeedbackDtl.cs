using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_PersonalFeedbackDtl
    {
        [Key]
        public int RD_ID { get; set; }

        [ForeignKey("RIC_PersonalFeedback")]
        public int RD_ReviewID { get; set; }

        public string RD_RevewerID { get; set; }
        
        public string RD_Findings { get; set; }

        public string RD_Improvements { get; set; }

        [StringLength(10)]
        public string RD_Status { get; set; }

        public virtual RIC_PersonalFeedback RIC_PersonalFeedback { get; set; }
    }
}
