using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DBLibrary;
namespace RIC.Models
{
    public class DashBoardViewModel
    {
        public DashBoardViewModel()
        {
            lastTwoMonthData = new List<SubmissionData>();
        }

        public DateTime ProgressReportFromDate { get; set; }
        public DateTime ProgressReportToDate { get; set; }

        public int CurrentWeek { get; set; }

        public string In_CallType { get; set; }
        public int In_Call_CallConnected { get; set; }
        public int In_Voice_Message { get; set; }
        public int In_Total { get; set; }
        public TimeSpan In_CallDuration { get; set; }

       

        public string Out_CallType { get; set; }
        public int Out_Call_Connected { get; set; }
        public int Out_Voice_Message { get; set; }
        public int Out_Total { get; set; }
        public TimeSpan Out_CallDuration { get; set; }

       public List<SubmissionData> lastTwoMonthData { get; set; } 

        public int RS_SubmissionTr { get; set; }
        public int RS_InterviewsTr { get; set; }
        public int RS_HiresTr { get; set; }
        public int Requirement { get; set; }

        public List<Week> week { get; set; }
        public List<CallsStatusWeek> callsStatusWeek { get; set; }

        public int totalSubmissionMonth { get; set; }
        public int totalInterviewsMonth { get; set; }
        public int totalHiresMonth { get; set; }
        public int remainingDays { get; set; }
        
        public List<JobRepoartWeek> Job_Repoart_Week { get; set; }

        public List<InterimFeedbackNotification> InterimFeedbackNotification { get; set; }

        public List<NewJoineeInterimNotification> NewJoineeInterimNotification { get; set; }


        public DateTime StartDateOfMonth { get; set; }
       public DateTime EndDateOfMonth { get; set; }

    }
    //submission week
    public class Week
    {
        public int dayCount { get; set; }
        public int weekNum { get; set; }

        public int totalSubmission { get; set; }
        public int totalInterviews { get; set; }
        public int totalHires { get; set; }

        public int remainingDays { get; set; }
        public List<SubmissionData> subData { get; set; }
    }
    public class SubmissionData
    {
        public DateTime date { get; set; }
        [Display(Name = "Submissions")]
        public int ? submission { get; set; }
        [Display(Name = "Interviews")]
        public int ? interviews { get; set; }
         [Display(Name = "Hires")]
        public int ? hires { get; set; }

        public int SubmissionTarget { get; set; }

        public int InterviewTarget { get; set; }
        
        public int HiresTarget { get; set; }       
    }

    public class CallsStatusWeek
    {
        public int DayCount { get; set; }
        
        public int WeekNumber { get; set; }

        public int InTotalCallConnected { get; set; }
        
        public int InTotalVoiceMessage { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan InTotalDuration { get; set; }

        public int OutTotalCallConnected { get; set; }
        
        public int OutTotalVoiceMessage { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan OutTotalDuration { get; set; }

        public int TotalCallsConnected { get; set; }

        public List<CallStatus> InCallStatus { get; set; }

        public List<CallStatus> OutCallStatus { get; set; }

        public List<TotalCallList> TotalCalls { get; set; }
    }

    public class TotalCallList
    {
        public DateTime Date { get; set; }
        public int? Call_Connected { get; set; }
        public int ? ProdCalls { get; set; }       
    }

    public class CallStatus
    {
        public DateTime Date { get; set; }
        public TimeSpan ? Duration { get; set; }
        public int ? Call_Connected { get; set; }
        public int ? Voice_Message { get; set; }       
    }
   
    public class OperationalResults
    {
        public int OpCount { get; set; }
        public int? OpHour { get; set; }
        public DateTime? OpCallTime { get; set; }
        public string OpCallType { get; set; }
    }

    public class InterimFeedbackNotification
    {
        [Display(Name = "Employee Id")]
        public string EmpId { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Reviewer")]
        public string Reviewer { get; set; }

        [Display(Name = "From Date")]
        public DateTime RRStartDate { get; set; }

        [Display(Name = "To Date")]
        public DateTime RREndDate { get; set; }

        public string Department { get; set; }

        [Display(Name = "Last ReviewDate")]
        public DateTime ReviewDate { get; set; }
        
        [Display(Name = "Next ReviewDate")]
        public DateTime NextReviewDate { get; set; }
    }

    public class NewJoineeInterimNotification
    {
        [Display(Name = "Employee Id")]
        public string EmpId { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; }

        [Display(Name = "Scheduled Revie Date")]
        public DateTime ReviewDate { get; set; }
    }
}