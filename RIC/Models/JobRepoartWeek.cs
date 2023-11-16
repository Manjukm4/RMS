using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class JobRepoartWeek
    {
        public int dayCount { get; set; }
        public int weekNumber { get; set; }

        public int totalSubmission { get; set; }
        public int totalInterviews { get; set; }
        public int totalHires { get; set; }

        public int remainingDays { get; set; }

        public int? Prod_RecruitersWk { get; set; }        
        public float? Avg_CallWk { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? Avg_DurationWk { get; set; }

        public List<Job_RepoartData> jrData { get; set; }
    }
}