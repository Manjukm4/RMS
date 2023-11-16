using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_AnnualFeedback
    {
        public RIC_AnnualFeedback()
        {
            this.RIC_AnnualFeedbacReviewer = new List<RIC_AnnualFeedbackReviewers>();

        }

        [Key]
        public int RA_ReviewId { get; set; }

        public int RA_FormId { get; set; }

        [StringLength(50)]
        public string RA_EmployeeId { get; set; }

        [StringLength(50)]
        public string RA_LeadReviewerId { get; set; }

        [StringLength(50)]
        public string RA_HrId { get; set; }

        [StringLength(10)]
        public string RA_Status { get; set; }

        public DateTime RA_Date { get; set; }

        // HR Review Details..
        public int RA_Hr_LeaveHistory { get; set; }

        [StringLength(10)]
        public string RA_Hr_ReviewStatus { get; set; }

        public int RA_Hr_LeavesTakenOnLop { get; set; }

        public string RA_Hr_Warnings { get; set; }

        public int RA_Hr_LoyaltyToCompanyValues { get; set; }

        public int RA_Hr_BehavingToCulturalValue { get; set; }

        public int RA_Hr_RespectsDiffInCulturalValue { get; set; }

        public string RA_Hr_AdditionalComments { get; set; }

        public string RA_Hr_ImprovementsRecommended { get; set; }

        public float RA_AvgRating { get; set; }

        public string RA_UpdatedBy { get; set; }

        public DateTime? RA_UpdatedDate { get; set; }

        public virtual ICollection<RIC_AnnualFeedbackReviewers> RIC_AnnualFeedbacReviewer { get; set; }
    }
}
