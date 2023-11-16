using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Review
{
    public class PrintHRDiscussion
    {
        public DateTime? DiscussionDate { get; set; }

        public string CandidateName { get; set; }
        
        public string AgendaType { get; set; }

        public string Description { get; set; }

        public string DiscussionNotes { get; set; }

        public string ActionItems { get; set; }

        public DateTime? NextMeetUpDate { get; set; }

        public string AdditionalComments { get; set; }



    }
}