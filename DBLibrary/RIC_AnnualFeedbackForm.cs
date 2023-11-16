using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public  class RIC_AnnualFeedbackForm
    {
        [Key]
        public int AF_FormID { get; set; }

        public string AF_FormName { get; set; }

        public virtual ICollection<RIC_AnnualFeedbackFields> RIC_AnnualFeedbackFields { get; set; }

        ////public int AF_FormID { get; set; }

        ////[ForeignKey("RIC_AnnualFeedbackFields")]
        ////public int AF_FieldID { get; set; }

        ////public int ? AF_ParentID { get; set; }

        ////public int AF_Weightage { get; set; }

        ////[StringLength(100)]
        ////public string Role { get; set; }

        ////public virtual RIC_AnnualFeedbackFields RIC_AnnualFeedbackFields { get; set; }

    }
}
