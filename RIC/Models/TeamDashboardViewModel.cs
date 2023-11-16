using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class TeamDashboardViewModel
    {
        public TeamDashboardViewModel()
        {
            employeeName = new List<string>();
            Submissions = new List<int>();
            Interviews = new List<int>();
            Hires = new List<int>();
            
            OutCalls = new List<int>();
            OutVoiceMessage = new List<int>();            
            InCallDuration = new List<TimeSpan>();
            OutCallDuration = new List<TimeSpan>();

        }
        public List<string> employeeName { get; set; }
        public List<int> Submissions { get; set; }
        public List<int> Interviews { get; set; }
        public List<int> Hires { get; set; }

       // public List<int> InCalls { get; set; }
        public List<int> OutCalls { get; set; }
        public List<int> OutVoiceMessage { get; set; }

        public List<TimeSpan> InCallDuration { get; set; }
        public List<TimeSpan> OutCallDuration { get; set; }

    }
}