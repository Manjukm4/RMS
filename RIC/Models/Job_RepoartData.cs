using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class Job_RepoartData
    {
        public DateTime date { get; set; }
        [Display(Name = "Submissions")]
        public int? submission { get; set; }
		  public string submissionToolTip { get; set; }

        [Display(Name = "Interviews")]
        public int? interviews { get; set; }
		  public string InterviewToolTip { get; set; }

        [Display(Name = "Hires")]
        public int? hires { get; set; }
		  public string HireToolTip { get; set; }

        public int? Prod_Recruiters { get; set; }

        public float? Avg_Call { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan ? Avg_Duration { get; set; }
    }
}