using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Review
{
    public class EditHRDiscussionDraftModel
    {
        [Display(Name = "Select Candidate")]
        public IEnumerable<SelectListItem> CandidateList { get; set; }

        public IEnumerable<SelectListItem> AgendaTypeList { get; set; }

        public int Discussionid { get; set; }

        [Display(Name = "Emp ID")]
        public string EmpCd { get; set; }

        [Display(Name = "Discussion Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DiscussionDate { get; set; }


        [Display(Name = "Agenda Type")]
        public string AgendaType { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Discussion Notes")]
        public string DiscussionNotes { get; set; }

        [Display(Name = "Action Items")]
        public string ActionItems { get; set; }

        [Display(Name = "Next MeetUp Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NextMeetUpDate { get; set; }

        [Display(Name = "HR InternalComments")]
        public string HRInternalComments { get; set; }

        [Display(Name = "Additional Comments")]
        public string AdditionalComments { get; set; }
    }
}