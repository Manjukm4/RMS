using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_AnnualFeedbackReviewers
    {
        [Key]
        public  int RR_ID { get; set; }

       [ForeignKey("RIC_AnnualFeedback")]
        public int RR_ReviewID { get; set; }

       [StringLength(50)]
        public string RR_ReviewerID { get; set; }

        public string RR_AdditionalComments { get; set; }

        public string RR_ImprovementsRecommended { get; set; }

        public string RR_Status { get; set; }

        public virtual RIC_AnnualFeedback RIC_AnnualFeedback { get; set; }

        public virtual ICollection<RIC_AnnualFeedbackDtl> RIC_AnnualFeedbackDtl { get; set; }
    }
}
