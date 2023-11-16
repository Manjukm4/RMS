using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_PersonalFeedback
    {
        [Key]
        public int RP_ID { get; set; }

        [StringLength(50)]
        public string RP_EmployeeID { get; set; }

        [StringLength(50)]
        public string RP_ReviewerID { get; set; }

        public DateTime RP_FromDate { get; set; }

        public DateTime RP_ToDate { get; set; }

        public DateTime RP_ReviewDate { get; set; }

        public int RP_TotalSubmissions { get; set; }

        public int RP_TotalInterview { get; set; }

        public int RP_TotalHires { get; set; }

        public int RP_TotalCalls { get; set; }

        [StringLength(50)]
        public string RP_DirectorID { get; set; }

        public string RP_DirectorFeedback { get; set; }

        public bool RP_ForwardToHr { get; set; }

        [StringLength(50)]
        public string RP_HrID { get; set; }

        public string RP_HrFeedback { get; set; }

        public bool  RP_Hr_ReviewStatus { get; set; }

        public bool RP_DirectorReviewStatus { get; set; }

        public DateTime RP_NextReviewDate { get; set; }

        public int? RP_SubmissionTarget { get; set; }

        public int? RP_InterviewTarget { get; set; }

        public int? RP_HiresTarget { get; set; }

        public int? RP_CallsTarget { get; set; }

        [StringLength(10)]
        public string RP_Status { get; set; }

        public string RP_UpdatedBy { get; set; }

        public DateTime ? RP_UpdatedDate { get; set; }

        public virtual ICollection<RIC_PersonalFeedbackDtl> RIC_PersonalFeedbackDtl { get; set; }
    }
}
