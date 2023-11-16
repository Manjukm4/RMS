using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Review
{
    public class AnnualFeedbackRequestHrPostVM
    {
        public int ReviewID { get; set; }

        public int LeavesTaken { get; set; }

        public int LeavesTakenOnLop { get; set; }

        public string Warnings { get; set; }

        public int Loyality { get; set; }

        public int CultureValues { get; set; }

        public int RespectBetweenPeople { get; set; }

        public string AdditionalComments { get; set; }

        public string ImprovementsRecommended { get; set; }
    }
}