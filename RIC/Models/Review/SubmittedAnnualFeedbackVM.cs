using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Review
{
    public class SubmittedAnnualFeedbackVM
    {
        public int ReviewID { get; set; }

        public string EmployeeName { get; set; }

        public string IssuedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReviewDate { get; set; }

    }
}