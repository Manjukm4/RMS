using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class RIC_Review
    {
        [Key]
        public int RR_Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RR_EmpCd { get; set; }

        [Required]
        [StringLength(50)]
        public string RR_MgrCd { get; set; }

       [StringLength(50)]
        public string RR_DirCd { get; set; }

       [StringLength(50)]
        public string RR_HrCd { get; set; }

        [Required]
        public DateTime RR_FromDate { get; set; }

        [Required]
        public DateTime RR_ToDate { get; set; }
        
        public int RR_Submissions { get; set; }
        
        public int RR_Interviews { get; set; }
        
        public int RR_Hires { get; set; }
        
        public int RR_Calls { get; set; }

        public DateTime RR_ReviewDate { get; set; }

        public string RR_TlFindings { get; set; }

        public string RR_Improvements { get; set; }

        public string RR_AdditionalComments { get; set; }
        
        public string RR_DirectorFeedback { get; set; }

        public string RR_HrFeedback { get; set; }

        public bool RR_DirectorFeedbackRequired { get; set; }

        public bool RR_HrFeedbackRequired { get; set; }

        public DateTime RR_NextReviewDate { get; set; }

        public int RR_SubmissionTarget { get; set; }

        public int RR_InterviewTarget { get; set; }

        public int RR_HiresTarget { get; set; }

        public int ? RR_CallsTarget { get; set; }

        public bool RR_Draft { get; set; }

        public bool RR_DirectorFeedbackStatus { get; set; }

        public bool RR_HrFeedbackStatus { get; set; }

        public bool RR_Discarded { get; set; }
    }
}
