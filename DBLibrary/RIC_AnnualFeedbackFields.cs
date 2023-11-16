using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_AnnualFeedbackFields
    {
        [Key]
        public int AF_FieldId { get; set; }

        [ForeignKey("RIC_AnnualFeedbackForm")]
        public int AF_FormID { get; set; }

        public int? AF_Pid { get; set; }

        [StringLength(200)]
        public string AF_FieldName { get; set; }

        public int AF_Weightage { get; set; }

        [StringLength(100)]
        public string AF_Roles { get; set; }

        public virtual RIC_AnnualFeedbackForm RIC_AnnualFeedbackForm { get; set; }

        
       // public virtual ICollection<RIC_AnnualFeedbackForm> RIC_AnnualFeedbackForm { get; set; }
    }
}
