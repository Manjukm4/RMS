using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class TeamDashboardDate
    {
        public DateTime date { get; set; }
        
        public int? CheckedOutCount { get; set; }

        public int? CheckedInCount { get; set; }

        public int ? Submission { get; set; }
        public int ? Interview { get; set; }
        public int ? Hire { get; set; }

        public int ? In_CallConnected { get; set; }
        public int ? In_VoiceMessages { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan ? In_TotalDuration { get; set; }
        public double ? InDurationInSeconds { get; set; }

        public int ? Out_CallConnected { get; set; }
        public int ? Out_VoiceMessages { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan ? Out_TotalDuration { get; set; }

        public double ? OutDurationInSeconds { get; set; }

        public string Attendance { get; set; }
    }
}