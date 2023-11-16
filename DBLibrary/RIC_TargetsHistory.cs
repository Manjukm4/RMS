using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
  public  class RIC_TargetsHistory
    {
        [Key]
        public int RT_ID { get; set; }
       
        public int RT_ReportingId { get; set; }

        [StringLength(50)]
        public string RT_Emp_Cd { get; set; }

        [StringLength(50)]
        public string RT_MgrCd { get; set; }

        public int RT_Year { get; set; }

        [StringLength(20)]
        public string RT_Month { get; set; }

        public float RT_SubmissionsTarget { get; set; }

        public float RT_InterviewsTarget { get; set; }

        public float RT_HiresTarget { get; set; }

        [StringLength(500)]
        public string RT_Comments { get; set; }

        public DateTime RT_CreatedDate { get; set; }

        [StringLength(50)]
        public string RT_CreatedBy { get; set; }

        public DateTime? RT_UpdatedDate { get; set; }

        [StringLength(50)]
        public string RT_UpdatedBy { get; set; }

    }
}
