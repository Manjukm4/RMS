using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class RIC_AnnualFeedbackDtl
    {
       [Key]
       public int RA_DtlId { get; set; }

       [ForeignKey("RIC_AnnualFeedbackReviewers")]
       public int RA_ReviewID { get; set; }

       public int RA_FieldID { get; set; }

       [StringLength(50)]
       public string RA_ReviewerID { get; set; }


       public float RA_ReviewRating { get; set; }

       public string RA_ReviewComments { get; set; }

       [StringLength(10)]
       public string RA_Status { get; set; }


       public virtual RIC_AnnualFeedbackReviewers RIC_AnnualFeedbackReviewers { get; set; }

    }
}
