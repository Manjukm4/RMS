using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Review
{
    public class InterimReviewRequestVM
    {
        public int ReviewID { get; set; }

        [Display(Name = "Review Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReviewDate { get; set; }

        [Display(Name = "Employee Name")]
        public string CndidateName { get; set; }

        [Display(Name = "Issued By")]
        public string IssuedBy { get; set; }

        [Display(Name = "Next Review Date")]
        public DateTime NextReviewDate { get; set; }

        public int NotificationCount { get; set; }

        public string ActionName { get; set; }
    }
}