using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Web;
using DBLibrary;
using DBLibrary.UnitOfWork;
using RIC.Models;
using RIC.Models.Dashboard;
using RIC.Models.Client;
using RIC.Models.ManageJobsVM;

namespace RIC.Utility
{
    public class DashBoardIndexAction
    {
        UnitOfWork unitOfwork = new UnitOfWork();
        //ashutosh
        public List<RIC_Call_Statistics> getGridForCalls(string empCd, string role = null)
        {
            List<RIC_Call_Statistics> callStatistics = unitOfwork.CallStatistics.GetCallStatatics(empCd, role).ToList();
            return callStatistics;
        }

        public Tuple<List<JobRepoartWeek>, List<CallsStatusWeek>> getDashboardMatrics(string empCd, DateTime startOfMonth, DateTime endOfMonth, string role = null,string getIndividualRecord=null,int EmpID=0)
        {
            //get the date.
            DateTime today = SystemClock.US_Date.Date;
            endOfMonth = endOfMonth.AddDays(1);
            DateTime endDate = endOfMonth;
            // if end date is < today then change the end date to todays date.
            if (endDate >= today)
                endDate = today.AddDays(1);
            var dashboardMatrix = unitOfwork.RIC_Job_Report.Get_DashboardMatrics(empCd, startOfMonth, endOfMonth, endDate, role,getIndividualRecord,EmpID).ToList();
            List<JobRepoartWeek> jr_Week_List = dashboardMatrix.GroupBy(g => g.DM_Week)
                .Select(s => new JobRepoartWeek
                {
                    weekNumber = s.Key,
                    jrData = s.Select(jr => new Job_RepoartData
                    {
                        date = jr.DM_Date,
                        submission = jr.DM_Submissions,
								submissionToolTip = jr.DM_SubmissionsToolTipData,
                        interviews = jr.DM_Interviews,
								InterviewToolTip = jr.DM_InterviewsToolTipData,
                        hires = jr.DM_Hires,
								HireToolTip = jr.DM_HiresToolTipData,
                        Prod_Recruiters = jr.DM_Prod_Recruiters,
                        //get the avg call/submission '(callconnectedin+callconnectedOut)/submissions'
                        Avg_Call = jr.DM_Submissions != 0 ? (jr.DM_CallConnected) / jr.DM_Submissions : 0,
                        //get the avg duration/calls 'total duration/(callConnected)'
                        Avg_Duration = (jr.DM_CallConnected == null) ? (TimeSpan?)null
                                : jr.DM_CallConnected.Value == 0 ? new TimeSpan()
                                : TimeSpan.FromSeconds(jr.DM_TotalDuration.Value / jr.DM_CallConnected.Value) // 
                    }).ToList(),
                    dayCount = s.Count(),
                    totalSubmission = s.Where(w => w.DM_Submissions != null).Sum(sum => sum.DM_Submissions.Value),
                    totalInterviews = s.Where(w => w.DM_Interviews != null).Sum(sum => sum.DM_Interviews.Value),
                    totalHires = s.Where(w => w.DM_Hires != null).Sum(sum => sum.DM_Hires.Value),
                    //get the avg call/submission '(callconnectedin+callconnectedOut)/submissions'for week
                    Avg_CallWk = s.Sum(sum => sum.DM_Submissions) != 0 ? (s.Sum(sum => sum.DM_CallConnected_In) + s.Sum(sum => sum.DM_Call_Connected_Out))
                                / s.Sum(sum => sum.DM_Submissions) : 0,
                    //get the avg duration/calls 'total duration/(callConnectedIn+callConnectedOut)' for week
                    Avg_DurationWk = s.Where(w => w.DM_CallConnected != null).Sum(sum => sum.DM_CallConnected.Value) == 0 ? new TimeSpan()
                                    : TimeSpan.FromSeconds(
                                                        (s.Where(w => w.DM_TotalDuration != null)
                                                           .Sum(sum => sum.DM_TotalDuration.Value))
                                                            / s.Where(w => w.DM_CallConnected != null).Sum(sum => sum.DM_CallConnected.Value)
                                                         )
                }).ToList();
            //Add the call Statistics.
            List<CallsStatusWeek> callStatusWk = dashboardMatrix.GroupBy(g => g.DM_Week)
                .Select(s => new CallsStatusWeek()
                {
                    WeekNumber = s.Key,
                    InCallStatus = s.Select(cs => new CallStatus()
                    {
                        Date = cs.DM_Date,
                        Call_Connected = cs.DM_CallConnected_In,
                        Voice_Message = cs.DM_VoiceMessages_In,
                        Duration = cs.DM_TotalDuration_In.HasValue ? TimeSpan.FromSeconds(cs.DM_TotalDuration_In.Value) : (TimeSpan?)null
                    }).ToList(),
                    OutCallStatus = s.Select(cs => new CallStatus()
                    {
                        Date = cs.DM_Date,
                        Call_Connected = cs.DM_Call_Connected_Out,
                        Voice_Message = cs.DM_Voice_Messages_Out,
                        Duration = cs.DM_Total_Duration_Out.HasValue ? TimeSpan.FromSeconds(cs.DM_Total_Duration_Out.Value) : (TimeSpan?)null
                    }).ToList(),
                    TotalCalls = s.Select(tc => new TotalCallList { 
                    Date=tc.DM_Date,
                    Call_Connected=tc.DM_TotalCalls,
                    ProdCalls=tc.DM_Prod_RecruitersCalls
                    }).ToList(),
                    // add the total durations in week 
                    TotalCallsConnected = s.Where(w => w.DM_TotalCalls.HasValue).Sum(sum => sum.DM_TotalCalls.Value),
                    InTotalDuration = TimeSpan.FromSeconds(s.Where(w => w.DM_TotalDuration_In.HasValue).Sum(sum => sum.DM_TotalDuration_In.Value)),
                    OutTotalDuration = TimeSpan.FromSeconds(s.Where(w => w.DM_Voice_Messages_Out.HasValue).Sum(sum => sum.DM_Total_Duration_Out.Value)),
                    OutTotalCallConnected = s.Where(w => w.DM_Call_Connected_Out.HasValue).Sum(sum => sum.DM_Call_Connected_Out.Value),
                    OutTotalVoiceMessage = s.Where(w => w.DM_Voice_Messages_Out.HasValue).Sum(sum => sum.DM_Voice_Messages_Out.Value),
                    InTotalCallConnected = s.Where(w => w.DM_CallConnected_In.HasValue).Sum(sum => sum.DM_CallConnected_In.Value),
                    InTotalVoiceMessage = s.Where(w => w.DM_VoiceMessages_In.HasValue).Sum(sum => sum.DM_VoiceMessages_In.Value),
                }).ToList();
            return Tuple.Create(jr_Week_List, callStatusWk);
        }

        public Tuple<List<JobRepoartWeek>, List<CallsStatusWeek>> getGridForUserOperational(string empCd, DateTime startOfMonth, DateTime endOfMonth, string role = null)
        {
            List<CallsStatusWeek> callStatusWk = new List<CallsStatusWeek>();

            List<CallStatus> InCallStatus = new List<CallStatus>();
            List<CallStatus> OutCallStatus = new List<CallStatus>();



            int day = endOfMonth.Day;
            // DateTime dateToday = SystemClock.US_Date.AddDays(-1);
            //int count = 0;
            //  int workingDays = 0;
            //int week = 1;
            // int DayCount = 0;
            int inTotalCallConnectedWk = 0;
            int inTotalVoiceMessageWk = 0;
            TimeSpan inTotalDuration = new TimeSpan();
            int outTotalCallConnectedWk = 0;
            int outTotalVoiceMessageWk = 0;
            TimeSpan outTotalDuration = new TimeSpan();



            List<JobRepoartWeek> jr_Week_List = new List<JobRepoartWeek>();
            //get the holiday list from db.
            List<RIC_Holidays> holidayList = unitOfwork.Holidays.Get().ToList();
            List<Job_RepoartData> jrDataList = new List<Job_RepoartData>();


            DateTime today = SystemClock.US_Date.Date;
            // DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));            
            DateTime dateToday = today.AddDays(0);
            int lastDay_Of_Month = endOfMonth.Day;

            //get the start data of month.
            // DateTime startOfMonth = new DateTime(dateToday.Year, dateToday.Month, 1);
            //get the submissions for user.
            List<RIC_Job_Report> Job_Report_List = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser(empCd, startOfMonth, endOfMonth.AddDays(1), role).ToList();
            List<RIC_Call_Statistics> callStatistics = unitOfwork.CallStatistics.GetCallStataticsForUser(empCd, startOfMonth, endOfMonth.AddDays(1), role).ToList();



            //int count = 0;
            int workingDays = 0;
            int week = 1;
            int total_SubmissionWk = 0;//total submissions in week.
            int totalInterviewWk = 0;// total interviews in week.
            int totalHireWk = 0;// total hiews in week
            int remainingDays = 0;// remaining days in month.

            int prodRecrutersWk = 0;
            int totalCallWk = 0;
            TimeSpan total_Call_Connected_DurationWk = new TimeSpan();


            for (int i = 0; i < lastDay_Of_Month; ++i)
            {

                DateTime currentDate = new DateTime(startOfMonth.Year, startOfMonth.Month, i + 1);
                DateTime nextDay = currentDate.Date.AddDays(1);
                var holiday = holidayList.FirstOrDefault(s => s.RH_Date == currentDate);
                // if data is uploaded for the current date.

                //  var dtAvalible = unitOfwork.RIC_Job_Report.Get().Where(s =>  s.RJ_UploadedDate.Date == currentDate.Date).FirstOrDefault();

                // var submission = submisionList.FirstOrDefault(s => s.RS_Date == currentDate);
                var currentSubmission = Job_Report_List.Where(s => s.RJ_Submit_Date.Date == currentDate.Date).ToList();
                var currentInterview = Job_Report_List.Where(s => s.RJ_Interview_Date >= currentDate && s.RJ_Interview_Date <= nextDay).ToList();
                var currentHire = Job_Report_List.Where(s => s.RJ_Hire_Date >= currentDate && s.RJ_Hire_Date <= nextDay).ToList();

                int prodRecruiters = currentSubmission.Select(s => s.RJ_EmpCd).Distinct().Count();

                //get the 'IN' call statistics.
                var inCallStatistics = callStatistics.Where(s => s.RC_Date == currentDate && s.RC_CallType == "In");
                //get 'Out' call statistics
                var outCallStatistics = callStatistics.Where(s => s.RC_Date == currentDate && s.RC_CallType == "Out");


                var callConnected = callStatistics.Where(s => s.RC_Date == currentDate && s.RC_Call_Connected == 1);

                int totalCalls = callStatistics.Where(s => s.RC_Date == currentDate).Sum(s => s.RC_Call_Connected);

                if ((currentSubmission.Count() > 0 || currentInterview.Count() > 0 || currentHire.Count() > 0 || inCallStatistics.Count() != 0 || outCallStatistics.Count() != 0)
                    && currentDate < dateToday)
                {


                    if (currentSubmission.Count() > 0 || currentInterview.Count() > 0 || currentHire.Count() > 0)
                    {
                        // add submission.
                        int submissions = currentSubmission.Count();
                        int interviews = currentInterview.Count();
                        int hires = currentHire.Count();
                        float avgCalls = 0;
                        TimeSpan avgDuration = new TimeSpan();
                        // if submission is non 0.
                        if (submissions != 0)
                            avgCalls = totalCalls / submissions;

                        TimeSpan durationDay = callConnected.Select(item => item.RC_Duration)
                              .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));

                        if (totalCalls != 0 && callConnected.Count() != 0)
                        {


                            double seconds = durationDay.TotalSeconds / totalCalls;
                            avgDuration = TimeSpan.FromSeconds(seconds);
                        }


                        jrDataList.Add(new Job_RepoartData()
                        {
                            date = currentDate,
                            submission = submissions,
                            interviews = interviews,
                            hires = hires,

                            Prod_Recruiters = prodRecruiters,
                            Avg_Call = avgCalls,
                            Avg_Duration = avgDuration
                        });
                        // add the total submission ,interview,hire.
                        total_SubmissionWk += submissions;
                        totalInterviewWk += interviews;
                        totalHireWk += hires;

                        prodRecrutersWk += prodRecruiters;
                        totalCallWk += totalCalls;
                        total_Call_Connected_DurationWk += durationDay;
                    }
                    else
                    {
                        // add 0 if no submission /interview / hire data
                        jrDataList.Add(new Job_RepoartData()
                        {
                            date = currentDate,
                            submission = 0,
                            interviews = 0,
                            hires = 0,

                            Prod_Recruiters = 0,
                            Avg_Call = 0,
                            Avg_Duration = new TimeSpan()
                        });
                    }
                    if (inCallStatistics.Count() != 0)
                    {
                        //get total call duration in day.
                        TimeSpan inDurationDay = inCallStatistics.Select(item => item.RC_Duration)
                            .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));
                        //get the total call connected in day
                        int inCall_ConnectedCountDay = inCallStatistics.Sum(s => s.RC_Call_Connected);
                        //get the total voice call in day.
                        int inVoice_MessageCountDay = inCallStatistics.Sum(s => s.RC_Voice_Message);
                        // add the in call statistics.
                        InCallStatus.Add(new CallStatus()
                        {
                            Date = currentDate,
                            Call_Connected = inCall_ConnectedCountDay,
                            Voice_Message = inVoice_MessageCountDay,
                            Duration = inDurationDay
                        });
                        // add the total in week.
                        inTotalCallConnectedWk += inCall_ConnectedCountDay;
                        inTotalVoiceMessageWk += inVoice_MessageCountDay;
                        inTotalDuration += inDurationDay;
                    }
                    else
                    {
                        // add null valuse in call statistics.
                        InCallStatus.Add(new CallStatus()
                        {
                            Date = currentDate,
                            Call_Connected = 0,
                            Voice_Message = 0,
                            Duration = new TimeSpan()
                        });
                    }

                    if (outCallStatistics.Count() != 0)
                    {
                        // add the out call statistics.
                        //get total call duration in day.
                        TimeSpan outDurationDay = outCallStatistics.Select(item => item.RC_Duration)
                            .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));
                        //get the total call connected in day
                        int outCall_ConnectedCountDay = outCallStatistics.Sum(s => s.RC_Call_Connected);
                        //get the total voice call in day.
                        int outVoice_MessageCountDay = outCallStatistics.Sum(s => s.RC_Voice_Message);
                        // add the in call statistics.
                        OutCallStatus.Add(new CallStatus()
                        {
                            Date = currentDate,
                            Call_Connected = outCall_ConnectedCountDay,
                            Voice_Message = outVoice_MessageCountDay,
                            Duration = outDurationDay
                        });
                        // add the total in week.
                        outTotalCallConnectedWk += outCall_ConnectedCountDay;
                        outTotalVoiceMessageWk += outVoice_MessageCountDay;
                        outTotalDuration += outDurationDay;
                    }
                    else
                    {
                        // add null valuse out call statistics.
                        OutCallStatus.Add(new CallStatus()
                        {
                            Date = currentDate,
                            Call_Connected = 0,
                            Voice_Message = 0,
                            Duration = new TimeSpan()
                        });
                    }

                    workingDays = workingDays + 1;// add the working days.
                    // add the remaining days.
                    if (currentDate >= dateToday)
                        remainingDays++;
                }
                else if (currentDate.DayOfWeek == DayOfWeek.Sunday || currentDate.DayOfWeek == DayOfWeek.Saturday
                    || holiday != null)
                {
                }
                // if cureent date smaller than 2 days then add 0 submissions.
                else if (currentDate <= dateToday.AddDays(-1))
                {
                    // add submission.
                    jrDataList.Add(new Job_RepoartData()
                    {
                        date = currentDate,
                        interviews = 0,
                        submission = 0,
                        hires = 0,

                        Prod_Recruiters = 0,
                        Avg_Call = 0,
                        Avg_Duration = new TimeSpan()
                    });
                    //add call ststistics.
                    // add 0 valuse in call statistics.
                    InCallStatus.Add(new CallStatus()
                    {
                        Date = currentDate,
                        Call_Connected = 0,
                        Voice_Message = 0,
                        Duration = new TimeSpan()
                    });

                    OutCallStatus.Add(new CallStatus()
                    {
                        Date = currentDate,
                        Call_Connected = 0,
                        Voice_Message = 0,
                        Duration = new TimeSpan()
                    });



                    workingDays = workingDays + 1;// add the working days.
                    if (currentDate >= dateToday)
                        remainingDays++;

                }
                else
                {
                    // add submission.
                    jrDataList.Add(new Job_RepoartData()
                    {
                        date = currentDate,
                        interviews = null,
                        submission = null,
                        hires = null,

                        Prod_Recruiters = null,
                        Avg_Call = null
                    });
                    // add call ststistics.

                    // add null valuse in call statistics.
                    InCallStatus.Add(new CallStatus()
                    {
                        Date = currentDate,
                        Call_Connected = null,
                        Voice_Message = null,
                        Duration = null
                    });
                    // add null valuse Out call statistics.
                    OutCallStatus.Add(new CallStatus()
                    {
                        Date = currentDate,
                        Call_Connected = null,
                        Voice_Message = null,
                        Duration = null
                    });



                    workingDays = workingDays + 1;// add the working days.
                    if (currentDate >= dateToday)
                        remainingDays++;
                }

                if (currentDate.DayOfWeek == DayOfWeek.Sunday || currentDate == endOfMonth)
                {
                    float avgCallWk = 0;
                    if (total_SubmissionWk != 0)// if submission in week is non 0 then divide by submission
                        avgCallWk = totalCallWk / total_SubmissionWk;

                    float avgProdRecWk = 0;
                    int subDaysInWeek = jrDataList.Count(s => s.submission != 0);
                    if (subDaysInWeek != 0)// if count in week is non 0 then divide by 
                        avgProdRecWk = prodRecrutersWk / subDaysInWeek;

                    //get the total call connected.
                    int totalCallConnected = inTotalCallConnectedWk + outTotalCallConnectedWk;
                    TimeSpan avgDurationWk = new TimeSpan();
                    if (totalCallConnected != 0)
                    {

                        double seconds = total_Call_Connected_DurationWk.TotalSeconds / totalCallConnected;
                        avgDurationWk = TimeSpan.FromSeconds(seconds);
                    }


                    if (workingDays != 0)
                    {
                        //add week in list.
                        jr_Week_List.Add(new JobRepoartWeek()
                        {
                            dayCount = workingDays,
                            weekNumber = week,
                            totalSubmission = total_SubmissionWk,
                            totalInterviews = totalInterviewWk,
                            totalHires = totalHireWk,
                            jrData = jrDataList,

                            Avg_CallWk = avgCallWk,
                            Avg_DurationWk = avgDurationWk
                            // Prod_RecruitersWk =(int)avgProdRecWk
                        });

                        //add week in list.
                        callStatusWk.Add(new CallsStatusWeek()
                        {
                            DayCount = workingDays,
                            WeekNumber = week,
                            InTotalCallConnected = inTotalCallConnectedWk,
                            InTotalVoiceMessage = inTotalVoiceMessageWk,
                            InTotalDuration = inTotalDuration,
                            OutTotalCallConnected = outTotalCallConnectedWk,
                            OutTotalVoiceMessage = outTotalVoiceMessageWk,
                            OutTotalDuration = outTotalDuration,
                            InCallStatus = InCallStatus,
                            OutCallStatus = OutCallStatus
                        });

                        // reset the count.
                        inTotalCallConnectedWk = 0;
                        inTotalVoiceMessageWk = 0;
                        inTotalDuration = new TimeSpan();
                        outTotalCallConnectedWk = 0;
                        outTotalVoiceMessageWk = 0;
                        outTotalDuration = new TimeSpan();
                        InCallStatus = new List<CallStatus>();
                        OutCallStatus = new List<CallStatus>();
                        //subList = new List<SubmissionData>();





                        week++;
                        workingDays = 0;
                        // reset the count.
                        total_SubmissionWk = 0;
                        totalInterviewWk = 0;
                        totalHireWk = 0;

                        prodRecrutersWk = 0;
                        totalCallWk = 0;
                        jrDataList = new List<Job_RepoartData>();
                    }
                }
            }
            jr_Week_List.FirstOrDefault().remainingDays = remainingDays;
            return Tuple.Create(jr_Week_List, callStatusWk);
        }


        public List<TeamDashboardListModel> getJobRepoartForTeamLead(string tlCd, DateTime startOfMonth, DateTime endOfMonth)
        {
            DateTime dateToday = SystemClock.US_Date.Date;
            List<TeamDashboardListModel> teamList = new List<TeamDashboardListModel>();
            //get the submission interviews and hire for team lead.
            List<RIC_Job_Report> Job_Report_List = unitOfwork.RIC_Job_Report.Get_JobRepoartForUser
                                                    (tlCd, startOfMonth, endOfMonth.AddDays(1)).ToList();
            //get call statistics for team lead.
            List<RIC_Call_Statistics> callStatistics = unitOfwork.CallStatistics.GetCallStataticsForUser
                                                       (tlCd, startOfMonth, endOfMonth.AddDays(1)).ToList();

            //get the holiday list from db.
            List<RIC_Holidays> holidayList = unitOfwork.Holidays.Get().ToList();


            TeamDashboardListModel model = new TeamDashboardListModel();

            //get the employee list for user.
            var employees = unitOfwork.User.getAllUsers().Where(s => (s.MgrCD == tlCd) && (s.RE_Emp_Cd != tlCd))
                            .ToList();

            foreach (var emp in employees)
            {


                DateTime currentDate = startOfMonth;
                List<TeamDashboardDate> month = new List<TeamDashboardDate>();
                for (; currentDate <= endOfMonth; currentDate = currentDate.AddDays(1))
                {
                    bool dataAvl = true;

                    var holiday = holidayList.FirstOrDefault(s => s.RH_Date == currentDate);
                    DateTime nextDay = currentDate.AddDays(1);

                    //if date avalable then add the day..
                    var totalSubmission = Job_Report_List.Where(s => s.RJ_Submit_Date.Date == currentDate.Date).ToList().Count;
                    var totalInterview = Job_Report_List.Where(s => s.RJ_Interview_Date >= currentDate && s.RJ_Interview_Date <= nextDay).ToList().Count;
                    var totalHire = Job_Report_List.Where(s => s.RJ_Hire_Date >= currentDate && s.RJ_Hire_Date <= nextDay).ToList().Count;
                    if (totalSubmission > 0 || totalInterview > 0 || totalHire > 0)
                        dataAvl = false;




                    // get the submissions ,interviews and hires for user.
                    var currentSubmission = Job_Report_List.Where(s => s.RJ_EmpCd == emp.RE_Emp_Cd && s.RJ_Submit_Date.Date == currentDate.Date).ToList();
                    var currentInterview = Job_Report_List.Where(s => s.RJ_EmpCd == emp.RE_Emp_Cd && s.RJ_Interview_Date >= currentDate && s.RJ_Interview_Date <= nextDay).ToList();
                    var currentHire = Job_Report_List.Where(s => s.RJ_EmpCd == emp.RE_Emp_Cd && s.RJ_Hire_Date >= currentDate && s.RJ_Hire_Date <= nextDay).ToList();

                    //get the 'IN' call statistics.
                    var inCallStatistics = callStatistics.Where(s => s.RC_Emp_Cd == emp.RE_Emp_Cd && s.RC_Date == currentDate && s.RC_CallType == "In");
                    //get 'Out' call statistics
                    var outCallStatistics = callStatistics.Where(s => s.RC_Emp_Cd == emp.RE_Emp_Cd && s.RC_Date == currentDate && s.RC_CallType == "Out");


                    //get total call duration in day.
                    TimeSpan inDurationDay = inCallStatistics.Select(item => item.RC_Duration)
                        .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));
                    //get the total call connected in day
                    int inCall_ConnectedCountDay = inCallStatistics.Sum(s => s.RC_Call_Connected);
                    //get the total voice call in day.
                    int inVoice_MessageCountDay = inCallStatistics.Sum(s => s.RC_Voice_Message);



                    //get total call duration in day.
                    TimeSpan outDurationDay = outCallStatistics.Select(item => item.RC_Duration)
                        .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));
                    //get the total call connected in day
                    int outCall_ConnectedCountDay = outCallStatistics.Sum(s => s.RC_Call_Connected);
                    //get the total voice call in day.
                    int outVoice_MessageCountDay = outCallStatistics.Sum(s => s.RC_Voice_Message);




                    if ((currentSubmission.Count() > 0 || currentInterview.Count() > 0 || currentHire.Count() > 0) && currentDate <= dateToday)
                    {
                        // add the date and submissions details in month list.
                        month.Add(new TeamDashboardDate
                        {
                            date = currentDate,
                            Submission = currentSubmission.Count(),
                            Interview = currentInterview.Count,
                            Hire = currentHire.Count,


                            In_CallConnected = inCall_ConnectedCountDay,
                            In_VoiceMessages = inVoice_MessageCountDay,
                            In_TotalDuration = inDurationDay,

                            Out_CallConnected = outCall_ConnectedCountDay,
                            Out_VoiceMessages = outVoice_MessageCountDay,
                            Out_TotalDuration = outDurationDay

                        });
                    }
                    else if ((currentDate.DayOfWeek == DayOfWeek.Sunday || currentDate.DayOfWeek == DayOfWeek.Saturday
                   || holiday != null) && dataAvl)
                    {
                    }
                    // else if (currentDate <= dateToday.AddDays(-1))
                    else if (currentDate <= dateToday)
                    {
                        month.Add(new TeamDashboardDate
                       {
                           date = currentDate,
                           Submission = 0,
                           Interview = 0,
                           Hire = 0,

                           In_CallConnected = inCall_ConnectedCountDay,
                           In_VoiceMessages = inVoice_MessageCountDay,
                           In_TotalDuration = inDurationDay,

                           Out_CallConnected = outCall_ConnectedCountDay,
                           Out_VoiceMessages = outVoice_MessageCountDay,
                           Out_TotalDuration = outDurationDay

                       });
                    }
                    else
                    {
                        month.Add(new TeamDashboardDate
                      {
                          date = currentDate,
                          Submission = null,
                          Interview = null,
                          Hire = null,

                          In_CallConnected = null,
                          In_VoiceMessages = null,
                          In_TotalDuration = null,

                          Out_CallConnected = null,
                          Out_VoiceMessages = null,
                          Out_TotalDuration = null

                      });
                    }
                }
                // add the month in the list
                teamList.Add(new TeamDashboardListModel
                {
                    EmpCd = emp.RE_Emp_Cd,
                    Job_Diva_User_Name = emp.RE_Jobdiva_User_Name,
                    Month = month,

                    Submission_Month = month.Sum(s => s.Submission),
                    Interview_Month = month.Sum(s => s.Interview),
                    Hire_Month = month.Sum(s => s.Hire),

                    In_CallConnected_Month = month.Sum(s => s.In_CallConnected),
                    In_VoiceMessages_Month = month.Sum(s => s.In_VoiceMessages),

                    Out_CallConnected_Month = month.Sum(s => s.Out_CallConnected),
                    Out_VoiceMessages_Month = month.Sum(s => s.Out_VoiceMessages),

                    In_TotalDuration_Month = month.Where(s => s.In_TotalDuration != null)
                                             .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t.In_TotalDuration.Value)),
                    Out_TotalDuration_Month = month.Where(s => s.Out_TotalDuration != null)
                                            .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t.Out_TotalDuration.Value)),

                    Submissions_Target = emp.RIC_SubmissionRule.RS_MonthlySubmissions,
                    Interviews_Target = emp.RIC_SubmissionRule.RS_Monthly_Interviews,
                    Hires_Target = emp.RIC_SubmissionRule.RS_Monthyl_Hire
                });
            }
            return teamList;
        }
        public TeamDashboardListModel_Main getJobRepoartForTeamLead(string empCd, DateTime startOfMonth, DateTime endOfMonth,string role ,int EmpID)
        {      
              //get the date.
            DateTime today = SystemClock.US_Date.Date;
            endOfMonth = endOfMonth.AddDays(1);
            DateTime endDate = endOfMonth;
            // if end date is < today then change the end date to todays date.
            if (endDate >= today)
                endDate = today.AddDays(1);
            TeamDashboardListModel_Main teamBoardMatrixfinal = new TeamDashboardListModel_Main();

            AccessLogList TeamMatrix= unitOfwork.RIC_Job_Report
                            .GetTeamMatrixForUser(empCd, startOfMonth, endOfMonth, endDate, role,EmpID);

            var teamBoardMatrix_firsttable = (TeamMatrix.TeamMatrixResult.GroupBy(g => new { g.TmEmpCd, g.TmMgrCd, g.TmId })
                                    .Select(s => new TeamDashboardListModel
                                    {
                                        Id = s.Key.TmId,
                                        EmpCd = s.Key.TmEmpCd,
                                        Job_Diva_User_Name = s.FirstOrDefault().TmEmpName+ s.FirstOrDefault().RE_Join_Date,
                                        MgrCd = s.FirstOrDefault().TmMgrCd,
                                        MgrName = s.FirstOrDefault().TmMgrName,
                                        EmployeeLvl = s.FirstOrDefault().TmEmpLvl,
                                        RE_Join_Date=s.FirstOrDefault().RE_Join_Date,
                                        //  FromDate=s.Where(w=>w.TmSubmission!=null).Min(d=>d.TmDate),
                                        // ToDate = s.Where(w => w.TmSubmission != null).Max(d => d.TmDate), 
                                        FromDate = s.Min(d => d.TmDate),
                                        ToDate = s.Max(d => d.TmDate),
                                      // Attendance=s.FirstOrDefault().Attendance,
                                        Month = s.Select(m => new TeamDashboardDate
                                        {
                                            date = m.TmDate,
                                            Submission = m.TmSubmission,
                                            Interview = m.TmInterviews,
                                            Hire = m.TmHires,
                                            Attendance=m.Attendance,
                                            In_CallConnected = m.TmCallConnectedIn,
                                            In_VoiceMessages = m.TmVoiceMessageIn,
                                            In_TotalDuration = m.TmTotalDurationIn.HasValue ?
                                                                TimeSpan.FromSeconds(m.TmTotalDurationIn.Value) : (TimeSpan?)null,
                                            InDurationInSeconds = m.TmTotalDurationIn,
                                            Out_CallConnected = m.TmCallConnectedOut,
                                            Out_VoiceMessages = m.TmVoiceMessageOut,
                                            Out_TotalDuration = m.TmTotalDurationOut.HasValue ?
                                                                TimeSpan.FromSeconds(m.TmTotalDurationOut.Value) : (TimeSpan?)null,
                                            OutDurationInSeconds = m.TmTotalDurationOut
                                        }).ToList(),
                                      

                                        Submission_Month = s.Sum(sum => sum.TmSubmission),
                                        Interview_Month = s.Sum(sum => sum.TmInterviews),
                                        Hire_Month = s.Sum(sum => sum.TmHires),
                                        In_CallConnected_Month = s.Sum(sum => sum.TmCallConnectedIn),
                                        In_VoiceMessages_Month = s.Sum(sum => sum.TmVoiceMessageIn),
                                        Out_CallConnected_Month = s.Sum(sum => sum.TmCallConnectedOut),
                                        Out_VoiceMessages_Month = s.Sum(sum => sum.TmVoiceMessageOut),
                                        In_TotalDuration_Month = s.Sum(sum => sum.TmTotalDurationIn).HasValue ?
                                                                      TimeSpan.FromSeconds(s.Sum(sum => sum.TmTotalDurationIn).Value)
                                                                       : (TimeSpan?)null,
                                        Out_TotalDuration_Month = s.Sum(sum => sum.TmTotalDurationOut).HasValue ?
                                                                       TimeSpan.FromSeconds(s.Sum(sum => sum.TmTotalDurationOut).Value)
                                                                       : (TimeSpan?)null,
                                        Submissions_Target = s.FirstOrDefault().TmSubmissionTarget,
                                        Interviews_Target = s.FirstOrDefault().TmInterviewTarget,
                                        Hires_Target = s.FirstOrDefault().TmHireTarget
                                    })).ToList();

                
             



            var teamBoardMatrix_secondtable = TeamMatrix.AccessLogResults.Select(d=> new AccessLogResult { 
                                         RC_Emp_Cd =d.RC_Emp_Cd,
                                         RC_Date= d.RC_Date,
                                        Category=d.Category,
                                        CategoryCount= d.CategoryCount,          
                                    }).ToList();


            //var teamBoardMatrix_final = (from er in teamBoardMatrix_firsttable
            //                             join es in teamBoardMatrix_secondtable on er.EmpCd equals es.RC_Emp_Cd into getfulllist
            //                             from getjoinlist in getfulllist.DefaultIfEmpty()
            //                             where er.Month.FirstOrDefault().date == getjoinlist.RC_Date

            //                                        select new TeamDashboardListModel_Main
            //                                        {


            //                                        }).ToList();


            teamBoardMatrixfinal.TeamDashboardListModel = teamBoardMatrix_firsttable;
            teamBoardMatrixfinal.AccessLogResults = teamBoardMatrix_secondtable;

            return teamBoardMatrixfinal;
        }

        public TeamDashboardListModel_Main getJobRepoartForTeamLead_Accesslog(string empCd, DateTime startOfMonth, DateTime endOfMonth, string role)
        {
            //get the date.
            DateTime today = SystemClock.US_Date.Date;
            endOfMonth = endOfMonth.AddDays(1);
            DateTime endDate = endOfMonth;
            // if end date is < today then change the end date to todays date.
            if (endDate >= today)
                endDate = today.AddDays(1);
            TeamDashboardListModel_Main teamBoardMatrixfinal = new TeamDashboardListModel_Main();

            AccessLogList TeamMatrix = unitOfwork.RIC_Job_Report
                            .GetTeamMatrixForUser_AccessLog(empCd, startOfMonth, endOfMonth, endDate, role);

   

            var teamBoardMatrix_secondtable = TeamMatrix.AccessLogResults.Select(d => new AccessLogResult
            {
                RC_Emp_Cd = d.RC_Emp_Cd,
                //RE_Employee_Name=d.RE_Employee_Name,
                RC_Date = d.RC_Date,
                Category = d.Category,
                RC_Description=d.RC_Description
            }).ToList();


            teamBoardMatrixfinal.AccessLogResults = teamBoardMatrix_secondtable;

            return teamBoardMatrixfinal;
        }

        //public List<TeamDashboardListModel> accessloggetJobRepoartForTeamLead(string empCd, DateTime startOfMonth, DateTime endOfMonth, string role)
        //{
        //    get the date.
        //    DateTime today = SystemClock.US_Date.Date;
        //    endOfMonth = endOfMonth.AddDays(1);
        //    DateTime endDate = endOfMonth;
        //    if end date is < today then change the end date to todays date.
        //    if (endDate >= today)
        //        endDate = today.AddDays(1);

        //    var TeamMatrix = unitOfwork.RIC_Job_Report
        //                    .GetTeamMatrixForUser(empCd, startOfMonth, endOfMonth, endDate, role);

        //    var teamBoardMatrix = (TeamMatrix.GroupBy(g => new { g.TmEmpCd, g.TmMgrCd, g.TmId })
        //                            .Select(s => new TeamDashboardListModel
        //                            {
        //                                Id = s.Key.TmId,
        //                                EmpCd = s.Key.TmEmpCd,
        //                                Job_Diva_User_Name = s.FirstOrDefault().TmEmpName,
        //                                MgrCd = s.FirstOrDefault().TmMgrCd,
        //                                MgrName = s.FirstOrDefault().TmMgrName,
        //                                EmployeeLvl = s.FirstOrDefault().TmEmpLvl,
        //                                FromDate = s.Where(w => w.TmSubmission != null).Min(d => d.TmDate),
        //                                ToDate = s.Where(w => w.TmSubmission != null).Max(d => d.TmDate),
        //                                FromDate = s.Min(d => d.TmDate),
        //                                ToDate = s.Max(d => d.TmDate),
        //                                Month = s.Select(m => new TeamDashboardDate
        //                                {
        //                                    date = m.TmDate,
        //                                    Submission = m.TmSubmission,
        //                                    Interview = m.TmInterviews,
        //                                    Hire = m.TmHires,
        //                                    In_CallConnected = m.TmCallConnectedIn,
        //                                    In_VoiceMessages = m.TmVoiceMessageIn,
        //                                    In_TotalDuration = m.TmTotalDurationIn.HasValue ?
        //                                                        TimeSpan.FromSeconds(m.TmTotalDurationIn.Value) : (TimeSpan?)null,
        //                                    InDurationInSeconds = m.TmTotalDurationIn,
        //                                    Out_CallConnected = m.TmCallConnectedOut,
        //                                    Out_VoiceMessages = m.TmVoiceMessageOut,
        //                                    Out_TotalDuration = m.TmTotalDurationOut.HasValue ?
        //                                                        TimeSpan.FromSeconds(m.TmTotalDurationOut.Value) : (TimeSpan?)null,
        //                                    OutDurationInSeconds = m.TmTotalDurationOut
        //                                }).ToList(),
        //                                Submission_Month = s.Sum(sum => sum.TmSubmission),
        //                                Interview_Month = s.Sum(sum => sum.TmInterviews),
        //                                Hire_Month = s.Sum(sum => sum.TmHires),
        //                                In_CallConnected_Month = s.Sum(sum => sum.TmCallConnectedIn),
        //                                In_VoiceMessages_Month = s.Sum(sum => sum.TmVoiceMessageIn),
        //                                Out_CallConnected_Month = s.Sum(sum => sum.TmCallConnectedOut),
        //                                Out_VoiceMessages_Month = s.Sum(sum => sum.TmVoiceMessageOut),
        //                                In_TotalDuration_Month = s.Sum(sum => sum.TmTotalDurationIn).HasValue ?
        //                                                              TimeSpan.FromSeconds(s.Sum(sum => sum.TmTotalDurationIn).Value)
        //                                                               : (TimeSpan?)null,
        //                                Out_TotalDuration_Month = s.Sum(sum => sum.TmTotalDurationOut).HasValue ?
        //                                                               TimeSpan.FromSeconds(s.Sum(sum => sum.TmTotalDurationOut).Value)
        //                                                               : (TimeSpan?)null,
        //                                Submissions_Target = s.FirstOrDefault().TmSubmissionTarget,
        //                                Interviews_Target = s.FirstOrDefault().TmInterviewTarget,
        //                                Hires_Target = s.FirstOrDefault().TmHireTarget
        //                            }
        //                                    )).ToList();

        //    return teamBoardMatrix;
        //}
        public List<ClientDashboard> getClientDashboard(string empCd, DateTime startOfMonth, DateTime endOfMonth, DateTime TodayDate,string role)
        {
            //get the date.
            DateTime today = SystemClock.US_Date.Date;
            endOfMonth = endOfMonth.AddDays(1);
            DateTime endDate = endOfMonth;
            // if end date is < today then change the end date to todays date.
            if (endDate >= today)
                endDate = today.AddDays(1);

            var TeamMatrix = unitOfwork.RIC_Job_Report
                            .Get_ClientDashboardMatrics(startOfMonth, endOfMonth, empCd, TodayDate,role);
            var clientDashboardMatrix = (TeamMatrix.GroupBy(g => new { g.RJ_Company })
                                    .Select(s => new ClientDashboard
                                    {
                                        RJ_Company = s.Key.RJ_Company,
                                        Month = s.Select(m => new ClientMonthDashboardDate
                                        {
                                            date =m.DM_Date ,
                                            Submission = m.Submission,
                                            Interview = m.Interview,
                                            Hire = m.Hire,
                                            Jobs_Issued = m.Jobs_Issued,
                                        }).ToList(),
                                        Submission_Month = s.Sum(sum => sum.Submission),
                                        Interview_Month = s.Sum(sum => sum.Interview),
                                        Hire_Month = s.Sum(sum => sum.Hire),
                                        Required_Month = s.Sum(sum => sum.Jobs_Issued),
                                        EmpCd = empCd,
                                        FromDate = startOfMonth,
                                        ToDate = endOfMonth

                                    })).ToList();

            return clientDashboardMatrix;
        }


        public List<ClientDashboardQuaterly> ClientDashboardQuaterWise(string empCd, int Year,string role)
        {

            var ClientQuaterly = unitOfwork.RIC_Job_Report.Get_ClientDashboard_QuarterWise(empCd, Year,role);
            var QuaterWiseData = (ClientQuaterly
                .Select(s => new ClientDashboardQuaterly
                                    {
                                        Q1Requirements = s.Q1Requirements,
                                        Q2Requirements = s.Q2Requirements,
                                        Q3Requirements = s.Q3Requirements,
                                        Q4Requirements = s.Q4Requirements,
                                        Q1Submissions =s.Q1Submissions,
                                        Q2Submissions=s.Q2Submissions,
                                        Q3Submissions = s.Q3Submissions,
                                        Q4Submissions = s.Q4Submissions,
                                        Q1Hires=s.Q1Hires,
                                        Q2Hires = s.Q2Hires,
                                        Q3Hires = s.Q3Hires,
                                        Q4Hires = s.Q4Hires,
                                        Q1Interviews = s.Q1Interviews,
                                        Q2Interviews = s.Q2Interviews,
                                        Q3Interviews = s.Q3Interviews,
                                        Q4Interviews = s.Q4Interviews,
                                        Q1Percentage=s.Q1Percentage,
                                        Q2Percentage=s.Q2Percentage,
                                        Q3Percentage=s.Q3Percentage,
                                        Q4Percentage=s.Q4Percentage,
                                        Q1Responded=s.Q1Responded,
                                        Q2Responded=s.Q2Responded,
                                        Q3Responded=s.Q3Responded,
                                        Q4Responded=s.Q4Responded,
                                        Q1SubByInterview = Math.Round((Convert.ToDouble(s.Q1Interviews) / Convert.ToDouble(s.Q1Submissions != 0 ? s.Q1Submissions : 1) * 100), 2),
                                        Q2SubByInterview = Math.Round((Convert.ToDouble(s.Q2Interviews) / Convert.ToDouble(s.Q2Submissions != 0 ? s.Q2Submissions : 1) * 100), 2),
                                        Q3SubByInterview = Math.Round((Convert.ToDouble(s.Q3Interviews) / Convert.ToDouble(s.Q3Submissions != 0 ? s.Q3Submissions : 1) * 100), 2),
                                        Q4SubByInterview = Math.Round((Convert.ToDouble(s.Q4Interviews) / Convert.ToDouble(s.Q4Submissions != 0 ? s.Q4Submissions : 1) * 100), 2),
                                        Q1SubByHire = Math.Round((Convert.ToDouble(s.Q1Hires) / Convert.ToDouble(s.Q1Submissions != 0 ? s.Q1Submissions : 1) * 100), 2),
                                        Q2SubByHire = Math.Round((Convert.ToDouble(s.Q2Hires) / Convert.ToDouble(s.Q2Submissions != 0 ? s.Q2Submissions : 1) * 100), 2),
                                        Q3SubByHire= Math.Round((Convert.ToDouble(s.Q3Hires) / Convert.ToDouble(s.Q3Submissions != 0 ? s.Q3Submissions : 1) * 100), 2),
                                        Q4SubByHire = Math.Round((Convert.ToDouble(s.Q4Hires) / Convert.ToDouble(s.Q4Submissions != 0 ? s.Q4Submissions : 1) * 100), 2),
                                        Q1InterviewByHire = Math.Round((Convert.ToDouble(s.Q1Hires) / Convert.ToDouble(s.Q1Interviews != 0 ? s.Q1Interviews : 1) * 100), 2),
                                        Q2InterviewByHire = Math.Round((Convert.ToDouble(s.Q2Hires) / Convert.ToDouble(s.Q2Interviews != 0 ? s.Q2Interviews : 1) * 100), 2),
                                        Q3InterviewByHire = Math.Round((Convert.ToDouble(s.Q3Hires) / Convert.ToDouble(s.Q3Interviews != 0 ? s.Q3Interviews : 1) * 100), 2),
                                        Q4InterviewByHire = Math.Round((Convert.ToDouble(s.Q4Hires) / Convert.ToDouble(s.Q4Interviews != 0 ? s.Q4Interviews : 1) * 100), 2),
                                        Q1StartDate = DateTime.Parse("1/1/" + Year.ToString()),
                                        Q1EndDate=DateTime.Parse("3/31/" + Year.ToString()),
                                        Q2StartDate = DateTime.Parse("4/1/" + Year.ToString()),
                                        Q2EndDate = DateTime.Parse("6/30/" + Year.ToString()),
                                        Q3StartDate = DateTime.Parse("7/1/" + Year.ToString()),
                                        Q3EndDate = DateTime.Parse("9/30/" + Year.ToString()),
                                        Q4StartDate = DateTime.Parse("10/1/" + Year.ToString()),
                                        Q4EndDate = DateTime.Parse("12/31/" + Year.ToString()),
                                        TotalSubmissons=s.TotalSubmissons,
                                        TotalInterviews=s.TotalInterviews,
                                        TotalHires=s.TotalHires,
                                        TotalRequirements=s.TotalRequirements,
                                        TotPercentage=s.TotPercentage,
                                        TotalResponse=s.TotalResponse,
                                        TotalSubByInterview= Math.Round((Convert.ToDouble(s.TotalInterviews) / Convert.ToDouble(s.TotalSubmissons != 0 ? s.TotalSubmissons : 1) * 100), 2),
                                        TotalSubByHire= Math.Round((Convert.ToDouble(s.TotalHires) / Convert.ToDouble(s.TotalSubmissons != 0 ? s.TotalSubmissons : 1) * 100), 2),
                                        TotalInterviewByHire= Math.Round((Convert.ToDouble(s.TotalHires) / Convert.ToDouble(s.TotalInterviews != 0 ? s.TotalInterviews : 1) * 100), 2),
                                        RJ_Company =s.RJ_Company                                        
                                    }))                                                               
                                    .ToList();
 
            return QuaterWiseData;
        }

        public List<PositionDashboardQuaterly> PositionDashboardQuaterWise(string empCd, int Year, string role,string Company)
        {

            var ClientQuaterly = unitOfwork.RIC_Job_Report.Get_PositionDashboard_QuarterWise(empCd, Year, role,Company);
            var QuaterWiseData = (ClientQuaterly
                .Select(s => new PositionDashboardQuaterly
                {
                    Categories=s.Position_Type,
                    Q1Positions=s.Q1Pos,
                    Q2Positions=s.Q2Pos,
                    Q3Positions=s.Q3Pos,
                    Q4Positions=s.Q4Pos,
                    TotalPositions=s.Total,
                    Q1Submission = s.Q1Submissions,
                    Q2Submission = s.Q2Submissions,
                    Q3Submission = s.Q3Submissions,
                    Q4Submission = s.Q4Submissions,
                    TotalSubmission = s.TotalSubmissions,
                    Q1Interview = s.Q1Interview,
                    Q2Interview = s.Q2Interview,
                    Q3Interview = s.Q3Interview,
                    Q4Interview = s.Q4Interview,
                    TotalInterview = s.TotalInterview,
                    Q1Hire= s.Q1Hire,
                    Q2Hire = s.Q2Hire,
                    Q3Hire = s.Q3Hire,
                    Q4Hire = s.Q4Hire,
                    TotalHire = s.TotalHire,
                    Q1SubByInterview = Math.Round((Convert.ToDouble(s.Q1Interview) / Convert.ToDouble(s.Q1Submissions != 0 ? s.Q1Submissions : 1) * 100), 2),
                    Q2SubByInterview = Math.Round((Convert.ToDouble(s.Q2Interview) / Convert.ToDouble(s.Q2Submissions != 0 ? s.Q2Submissions : 1) * 100), 2),
                    Q3SubByInterview = Math.Round((Convert.ToDouble(s.Q3Interview) / Convert.ToDouble(s.Q3Submissions != 0 ? s.Q3Submissions : 1) * 100), 2),
                    Q4SubByInterview = Math.Round((Convert.ToDouble(s.Q4Interview) / Convert.ToDouble(s.Q4Submissions != 0 ? s.Q4Submissions : 1) * 100), 2),
                    Q1SubByHire = Math.Round((Convert.ToDouble(s.Q1Hire) / Convert.ToDouble(s.Q1Submissions != 0 ? s.Q1Submissions : 1) * 100), 2),
                    Q2SubByHire = Math.Round((Convert.ToDouble(s.Q2Hire) / Convert.ToDouble(s.Q2Submissions != 0 ? s.Q2Submissions : 1) * 100), 2),
                    Q3SubByHire = Math.Round((Convert.ToDouble(s.Q3Hire) / Convert.ToDouble(s.Q3Submissions != 0 ? s.Q3Submissions : 1) * 100), 2),
                    Q4SubByHire = Math.Round((Convert.ToDouble(s.Q4Hire) / Convert.ToDouble(s.Q4Submissions != 0 ? s.Q4Submissions : 1) * 100), 2),
                    Q1InterviewByHire = Math.Round((Convert.ToDouble(s.Q1Hire) / Convert.ToDouble(s.Q1Interview != 0 ? s.Q1Interview : 1) * 100), 2),
                    Q2InterviewByHire = Math.Round((Convert.ToDouble(s.Q2Hire) / Convert.ToDouble(s.Q2Interview != 0 ? s.Q2Interview : 1) * 100), 2),
                    Q3InterviewByHire = Math.Round((Convert.ToDouble(s.Q3Hire) / Convert.ToDouble(s.Q3Interview != 0 ? s.Q3Interview : 1) * 100), 2),
                    Q4InterviewByHire = Math.Round((Convert.ToDouble(s.Q4Hire) / Convert.ToDouble(s.Q4Interview != 0 ? s.Q4Interview : 1) * 100), 2),
                    TotalSubByInterview = Math.Round((Convert.ToDouble(s.TotalInterview) / Convert.ToDouble(s.TotalSubmissions != 0 ? s.TotalSubmissions : 1) * 100), 2),
                    TotalSubByHire = Math.Round((Convert.ToDouble(s.TotalHire) / Convert.ToDouble(s.TotalSubmissions != 0 ? s.TotalSubmissions : 1) * 100), 2),
                    TotalInterviewByHire = Math.Round((Convert.ToDouble(s.TotalHire) / Convert.ToDouble(s.TotalInterview != 0 ? s.TotalInterview : 1) * 100), 2),
                    Q1StartDate = DateTime.Parse("1/1/" + Year.ToString()),
                    Q1EndDate = DateTime.Parse("3/31/" + Year.ToString()),
                    Q2StartDate = DateTime.Parse("4/1/" + Year.ToString()),
                    Q2EndDate = DateTime.Parse("6/30/" + Year.ToString()),
                    Q3StartDate = DateTime.Parse("7/1/" + Year.ToString()),
                    Q3EndDate = DateTime.Parse("9/30/" + Year.ToString()),
                    Q4StartDate = DateTime.Parse("10/1/" + Year.ToString()),
                    Q4EndDate = DateTime.Parse("12/31/" + Year.ToString()),
                      
                }))
                   .ToList();

            return QuaterWiseData;
        }

        public List<ClientDashboardQuaterly> ClientDashboardQuaterWiseNew(string empCd, int Year, string role,string Company)
        {

            var ClientQuaterly = unitOfwork.RIC_Job_Report.Get_ClientDashboard_QuarterWiseNew(empCd, Year, role,Company);
            //var ClientQuaterly_TotalRecruiter = unitOfwork.RIC_Job_Report.Get_ClientDashboard_QuarterWise_Recruiter(empCd, Year, role).ToList();
          //  var ActiveRecruiter = unitOfwork.RIC_Job_Report.Get_ClientDashboard_QuarterWise_Active_Recruiter(empCd, Year, role).ToList();
           
            var QuaterWiseData = (from s in  ClientQuaterly
                                  //from REC in ClientQuaterly_TotalRecruiter
                                  //from act in ActiveRecruiter
                                  select new ClientDashboardQuaterly
                {
                    Q1Requirements = s.Q1Requirements,
                    Q2Requirements = s.Q2Requirements,
                    Q3Requirements = s.Q3Requirements,
                    Q4Requirements = s.Q4Requirements,
                    Q1Submissions = s.Q1Submissions,
                    Q2Submissions = s.Q2Submissions,
                    Q3Submissions = s.Q3Submissions,
                    Q4Submissions = s.Q4Submissions,
                    Q1Hires = s.Q1Hires,
                    Q2Hires = s.Q2Hires,
                    Q3Hires = s.Q3Hires,
                    Q4Hires = s.Q4Hires,
                    Q1Rectuiters = s.Q1Recruiter ,
                    Q2Rectuiters = s.Q2Recruiter,
                    Q3Rectuiters = s.Q3Recruiter,
                    Q4Rectuiters = s.Q4Recruiter,
                    Q1ActiveRec = s.Q1Active_rec,
                    Q2ActiveRec = s.Q2Active_rec,
                    Q3ActiveRec = s.Q3Active_rec,
                    Q4ActiveRec = s.Q4Active_rec,
                    Q1Interviews = s.Q1Interviews,
                    Q2Interviews = s.Q2Interviews,
                    Q3Interviews = s.Q3Interviews,
                    Q4Interviews = s.Q4Interviews,
                    Q1Percentage = s.Q1Percentage,
                    Q2Percentage = s.Q2Percentage,
                    Q3Percentage = s.Q3Percentage,
                    Q4Percentage = s.Q4Percentage,
                    Q1Responded = s.Q1Responded,
                    Q2Responded = s.Q2Responded,
                    Q3Responded = s.Q3Responded,
                    Q4Responded = s.Q4Responded,
                    Q1SubByInterview = Math.Round((Convert.ToDouble(s.Q1Interviews) / Convert.ToDouble(s.Q1Submissions != 0 ? s.Q1Submissions : 1) * 100), 2),
                    Q2SubByInterview = Math.Round((Convert.ToDouble(s.Q2Interviews) / Convert.ToDouble(s.Q2Submissions != 0 ? s.Q2Submissions : 1) * 100), 2),
                    Q3SubByInterview = Math.Round((Convert.ToDouble(s.Q3Interviews) / Convert.ToDouble(s.Q3Submissions != 0 ? s.Q3Submissions : 1) * 100), 2),
                    Q4SubByInterview = Math.Round((Convert.ToDouble(s.Q4Interviews) / Convert.ToDouble(s.Q4Submissions != 0 ? s.Q4Submissions : 1) * 100), 2),
                    Q1SubByHire = Math.Round((Convert.ToDouble(s.Q1Hires) / Convert.ToDouble(s.Q1Submissions != 0 ? s.Q1Submissions : 1) * 100), 2),
                    Q2SubByHire = Math.Round((Convert.ToDouble(s.Q2Hires) / Convert.ToDouble(s.Q2Submissions != 0 ? s.Q2Submissions : 1) * 100), 2),
                    Q3SubByHire = Math.Round((Convert.ToDouble(s.Q3Hires) / Convert.ToDouble(s.Q3Submissions != 0 ? s.Q3Submissions : 1) * 100), 2),
                    Q4SubByHire = Math.Round((Convert.ToDouble(s.Q4Hires) / Convert.ToDouble(s.Q4Submissions != 0 ? s.Q4Submissions : 1) * 100), 2),
                    Q1InterviewByHire = Math.Round((Convert.ToDouble(s.Q1Hires) / Convert.ToDouble(s.Q1Interviews != 0 ? s.Q1Interviews : 1) * 100), 2),
                    Q2InterviewByHire = Math.Round((Convert.ToDouble(s.Q2Hires) / Convert.ToDouble(s.Q2Interviews != 0 ? s.Q2Interviews : 1) * 100), 2),
                    Q3InterviewByHire = Math.Round((Convert.ToDouble(s.Q3Hires) / Convert.ToDouble(s.Q3Interviews != 0 ? s.Q3Interviews : 1) * 100), 2),
                    Q4InterviewByHire = Math.Round((Convert.ToDouble(s.Q4Hires) / Convert.ToDouble(s.Q4Interviews != 0 ? s.Q4Interviews : 1) * 100), 2),
                    Q1StartDate = DateTime.Parse("1/1/" + Year.ToString()),
                    Q1EndDate = DateTime.Parse("3/31/" + Year.ToString()),
                    Q2StartDate = DateTime.Parse("4/1/" + Year.ToString()),
                    Q2EndDate = DateTime.Parse("6/30/" + Year.ToString()),
                    Q3StartDate = DateTime.Parse("7/1/" + Year.ToString()),
                    Q3EndDate = DateTime.Parse("9/30/" + Year.ToString()),
                    Q4StartDate = DateTime.Parse("10/1/" + Year.ToString()),
                    Q4EndDate = DateTime.Parse("12/31/" + Year.ToString()),
                    TotalSubmissons = s.TotalSubmissons,
                    TotalInterviews = s.TotalInterviews,
                    TotalHires = s.TotalHires,
                    TotalRequirements = s.TotalRequirements,
                    TotPercentage = s.TotPercentage,
                    TotalResponse = s.TotalResponse,
                    TotalActiveRecruiters= s.TotalActiveRecruiter,
                    TotalRecruiters= s.TotalRecruiter,
                    TotalSubByInterview = Math.Round((Convert.ToDouble(s.TotalInterviews) / Convert.ToDouble(s.TotalSubmissons != 0 ? s.TotalSubmissons : 1) * 100), 2),
                    TotalSubByHire = Math.Round((Convert.ToDouble(s.TotalHires) / Convert.ToDouble(s.TotalSubmissons != 0 ? s.TotalSubmissons : 1) * 100), 2),
                    TotalInterviewByHire = Math.Round((Convert.ToDouble(s.TotalHires) / Convert.ToDouble(s.TotalInterviews != 0 ? s.TotalInterviews : 1) * 100), 2)
                   // RJ_Company = s.RJ_Company
                })
                                    .ToList();
           

            return QuaterWiseData;
        }


        public List<ClientOperationalList> ClientDashboardQuaterWiseDetails(string empCd, string Type, string Client, DateTime Fromdate, DateTime Todate,string role)
        { 
        
             var ClientQuaterly = unitOfwork.RIC_Job_Report.Get_ClientDashboard_QuarterWiseDetails(empCd, Type, Client,Fromdate,Todate,role);
            if (Client == "")
            {
                ClientQuaterly = unitOfwork.RIC_Job_Report.Get_ClientDashboard_QuarterWiseDetails_Portfolio(empCd, Type, Fromdate, Todate, role);
            }

             var ClientDetails = (ClientQuaterly
                 .Select (s => new ClientOperationalList

                 {    Client =s.RJ_Company,
                      EmpCd = s.RJ_EmpCd,
                      EmployeeName = s.RJ_Submitted_By,
                      //count = s.count
                      Submissions=s.TotalSubmissons,
                      Interviews=s.TotalInterviews,
                      Hires=s.TotalHires,
                      count= Type == "Submission" ? s.TotalSubmissons: (Type=="Interview" ? s.TotalInterviews: s.TotalHires)
                 })).ToList();
                  return ClientDetails;

        }

        public List<ClientOperationalList> ClientDashboardQuaterWiseDetails1(string empCd, string Type, string Client, DateTime Fromdate, DateTime Todate,string Position_type ,string role)
        {

            //var ClientQuaterly = unitOfwork.RIC_Job_Report.Get_ClientDashboard_QuarterWiseDetails(empCd, Type, Client, Fromdate, Todate, role);
            //if (Client == "")
            //{
            var    ClientQuaterly = unitOfwork.RIC_Job_Report.Get_Position_Quaterwise(empCd, Type, Fromdate, Todate, Position_type, role);
            //}
            //var c1 = ClientQuaterly.ToList();
            var ClientDetails = (ClientQuaterly
                .Select(s => new ClientOperationalList

                {
                    Client = s.RJ_Company,
                    EmpCd = s.RJ_EmpCd,
                    EmployeeName = s.RJ_Submitted_By,
                     //count = s.count
                     Submissions = s.TotalSubmissons,
                    Interviews = s.TotalInterviews,
                    Hires = s.TotalHires,
                    count = Type == "Submission" ? s.TotalSubmissons : (Type == "Interview" ? s.TotalInterviews : s.TotalHires)
                })).ToList();
            return ClientDetails;

        }


        public List<ClientDashboardMonthly> ClientDashboardMonthly(string empCd, int Year, string role)
        {

            var ClientQuaterly = unitOfwork.RIC_Job_Report.Get_ClientDashboard_Monthly(empCd, Year,role);
            
            var QuaterWiseData = (from s in ClientQuaterly
                          
                                  select new ClientDashboardMonthly
                          {
                              RequirementsCount = s.RequirementsCount,
                              SubmissonCount = s.SubmissonCount,
                              InterviewCount = s.InterviewCount,
                              HireCount = s.HireCount,
                              Percentage = s.Percentage,
                              Respond = s.Respond,
                              SubByInterview = Math.Round((Convert.ToDouble(s.InterviewCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
                              SubByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
                              InterviewByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.InterviewCount != 0 ? s.InterviewCount : 1) * 100), 2),
                              StartDate = s.StartDate.Date,
                              EndDate = s.EndDate.Date,
                              MonthName = s.MonthName,
                              RJ_Company = s.RJ_Company,
                              CompanyActiveRecruiterCount= s.Active_rec_cnt,
                              TotalRecruiterCount =s.RecruiterCount,
                              Monthnum = s.Monthnum
                          }).ToList();
            
            //var QuaterWiseData = (ClientQuaterly
            //    .Select(s => new ClientDashboardMonthly
            //    {
            //        RequirementsCount=s.RequirementsCount,
            //        SubmissonCount = s.SubmissonCount,
            //        InterviewCount = s.InterviewCount,
            //        HireCount = s.HireCount,
            //        Percentage=s.Percentage,
            //        Respond=s.Respond,
            //        SubByInterview = Math.Round((Convert.ToDouble(s.InterviewCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
            //        SubByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
            //        InterviewByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.InterviewCount != 0 ? s.InterviewCount : 1) * 100), 2),
            //        StartDate = s.StartDate.Date,
            //        EndDate = s.EndDate.Date,
            //        MonthName=s.MonthName,
            //        RJ_Company=s.RJ_Company,
            //        Monthnum=s.Monthnum
            //    })).ToList();

            return QuaterWiseData;
        }

        public List<ClientSubmissionDetails> ClientSubmissionDeatils(string empCd, int Year, string role,DateTime Fromdate, DateTime Todate)
        {

            var ClientQuaterly = unitOfwork.RIC_Job_Report.ClientSubmissionDeatils(empCd, Year, role, Fromdate, Todate);

            var QuaterWiseData = (from s in ClientQuaterly

                                  select new ClientSubmissionDetails
                                  {
                                      Company=s.RJ_Company,
                                      DateIssued=s.RJ_DateIssued,
                                      JobDiva_Ref=s.RJ_JobDiva_Ref,
                                      submission2=s.submission2,
                                      submission8=s.submission8,
                                      submission4=s.submission4,
                                      submissionNxtD=s.submissionNxtD,
                                      submissionNxt2D=s.submissionNxt2D,
                                      submissionNxt3D=s.submissionNxt3D,
                                      submissionNxt5D=s.submissionNxt5D
                                       
                                  }).ToList();

   

            return QuaterWiseData;
        }


        public List<PositionDashboardMonthly> PositionDashboardMonthly(string empCd, int Year, string role,string Company)
        {

            var ClientQuaterly = unitOfwork.RIC_Job_Report.Get_PositionDashboard_Monthly(empCd, Year, role, Company);

            var QuaterWiseData = (from s in ClientQuaterly

                                  select new PositionDashboardMonthly
                                  {
                                     Categories=s.Position_Type,
                                     Position=s.PositionCount,
                                     Submission=s.SubmissonCount,
                                     Interview=s.InterviewCount,
                                     Hire=s.HireCount,
                                      StartDate = s.StartDate.Date,
                                      EndDate = s.EndDate.Date,
                                      MonthName = s.MonthName,
                                      SubByInterview = Math.Round((Convert.ToDouble(s.InterviewCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
                                      SubByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
                                      InterviewByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.InterviewCount != 0 ? s.InterviewCount : 1) * 100), 2),
                                      Monthnum = s.Monthnum
                                  }).ToList();

            //var QuaterWiseData = (ClientQuaterly
            //    .Select(s => new ClientDashboardMonthly
            //    {
            //        RequirementsCount=s.RequirementsCount,
            //        SubmissonCount = s.SubmissonCount,
            //        InterviewCount = s.InterviewCount,
            //        HireCount = s.HireCount,
            //        Percentage=s.Percentage,
            //        Respond=s.Respond,
            //        SubByInterview = Math.Round((Convert.ToDouble(s.InterviewCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
            //        SubByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
            //        InterviewByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.InterviewCount != 0 ? s.InterviewCount : 1) * 100), 2),
            //        StartDate = s.StartDate.Date,
            //        EndDate = s.EndDate.Date,
            //        MonthName=s.MonthName,
            //        RJ_Company=s.RJ_Company,
            //        Monthnum=s.Monthnum
            //    })).ToList();

            return QuaterWiseData;
        }


        public List<ClientDashboardMonthly> ClientDashboardMonthlyNew(string empCd, int Year, string role,string Company)
        {

            var ClientQuaterly = unitOfwork.RIC_Job_Report.Get_ClientDashboard_MonthlyNew(empCd, Year, role, Company);
           // var ClientTotRec = unitOfwork.RIC_Job_Report.Get_ClientDashboard_MonthlyRecruiters(empCd, Year, role);
            //var ClientActRec = unitOfwork.RIC_Job_Report.Get_ClientDashboard_Monthly_ActiveRecruiters(empCd, Year, role);


            var QuaterWiseData = (from s in ClientQuaterly
                          //join t in ClientTotRec on s.Monthnum equals t.MonthNumber into tr
                          //from
                          //join A in ClientActRec on s.Monthnum equals A.MonthNumber into Ar
                          //from ArRestul in Ar.DefaultIfEmpty() trRestul in tr.DefaultIfEmpty()

                          select new ClientDashboardMonthly
                          {
                              RequirementsCount = s.RequirementsCount,
                              SubmissonCount = s.SubmissonCount,
                              InterviewCount = s.InterviewCount,
                              HireCount = s.HireCount,
                              Percentage = s.Percentage,
                              Respond = s.Respond,
                              SubByInterview = Math.Round((Convert.ToDouble(s.InterviewCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
                              SubByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
                              InterviewByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.InterviewCount != 0 ? s.InterviewCount : 1) * 100), 2),
                              StartDate = s.StartDate.Date,
                              EndDate = s.EndDate.Date,
                              MonthName = s.MonthName,
                              TotalRecruiterCount =s.RecruiterCount,
                              ActiveRecruiterCount =s.Active_rec_cnt,
                              Monthnum = s.Monthnum
                          }).ToList();
            //var QuaterWiseData = (ClientQuaterly
            //    .Select(s => new ClientDashboardMonthly
            //    {
            //        RequirementsCount = s.RequirementsCount,
            //        SubmissonCount = s.SubmissonCount,
            //        InterviewCount = s.InterviewCount,
            //        HireCount = s.HireCount,
            //        Percentage = s.Percentage,
            //        Respond = s.Respond,
            //        SubByInterview = Math.Round((Convert.ToDouble(s.InterviewCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
            //        SubByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.SubmissonCount != 0 ? s.SubmissonCount : 1) * 100), 2),
            //        InterviewByHire = Math.Round((Convert.ToDouble(s.HireCount) / Convert.ToDouble(s.InterviewCount != 0 ? s.InterviewCount : 1) * 100), 2),
            //        StartDate = s.StartDate.Date,
            //        EndDate = s.EndDate.Date,
            //        MonthName = s.MonthName,
            //        //RJ_Company = s.RJ_Company,
            //        Monthnum = s.Monthnum
            //    })).ToList();

            return QuaterWiseData;
        }

        //added by suman on 12-11-2019
        //to get last four quarters data for user(submissons,interviews,hires)
        public List<ClientDashboardQuaterly> GetSubmissionAnalysis(DateTime ReviewDate, string empCd)
        {
            var SubmissionAnalysisQuaterly = unitOfwork.RIC_Job_Report.GetSubmissionAnalysisByEmpCd(ReviewDate, empCd);

            var SubmissionAnalysisQuaterWiseData = (SubmissionAnalysisQuaterly
                .Select(s => new ClientDashboardQuaterly
                {
                    Q1Submissions = s.Q1Submissions,
                    Q2Submissions = s.Q2Submissions,
                    Q3Submissions = s.Q3Submissions,
                    Q4Submissions = s.Q4Submissions,
                    Q1Hires = s.Q1Hires,
                    Q2Hires = s.Q2Hires,
                    Q3Hires = s.Q3Hires,
                    Q4Hires = s.Q4Hires,
                    Q1Interviews = s.Q1Interviews,
                    Q2Interviews = s.Q2Interviews,
                    Q3Interviews = s.Q3Interviews,
                    Q4Interviews = s.Q4Interviews,                  
                    TotalSubmissons = s.TotalSubmissons,
                    TotalInterviews = s.TotalInterviews,
                    TotalHires = s.TotalHires,
                    TotalRequirements = s.TotalRequirements,
                    Quarter1=s.Quarter1,
                    Quarter2=s.Quarter2,
                    Quarter3=s.Quarter3,
                    Quarter4=s.Quarter4,
                    EmpCd = s.RJ_EmpCd
                })).ToList();
            return SubmissionAnalysisQuaterWiseData;

        }


        public IEnumerable<Jobs> GetAllJobs(string empCd)
        {

            var getJobs = unitOfwork.RIC_Job_Report.Get_ManageJobs(empCd);
            var ResultData = (getJobs
                .Select(s => new Jobs
                {
                    HDRID = s.RS_ID,
                    JobID = s.RS_JobID,
                    JobDivaRef = s.RS_JobDivaRef,
                    JobTitle = s.RS_JobTitle,
                    JobIssueDate = s.RS_JobIssueDate,
                    Company = s.RS_Company,
                    WorkLocation = s.RS_WorkLocation,
                    Priority = s.RS_Priority,
                    Division = s.RS_Division,
                    Category = s.RS_Category,
                    City = s.RS_City,
                    State = s.RS_State,
                    BillRate = s.RS_BillRate == null ? 0 : float.Parse(s.RS_BillRate.ToString()),
                    PayRate = s.RS_PayRate == null ? 0 : float.Parse(s.RS_PayRate.ToString()),
                    MaxSubAllowed = s.RS_MaxSubAllowed,
                    InternalSub = s.RS_InternalSub,
                    ExternalSub = s.RS_ExternalSub,
                    RMSJobStatus = s.RS_RMSJobStatus,
                    CreatedDate = s.RS_CreatedDt,
                    JobDivaStatus = s.RS_JobDivaStatus,
                    CheckInStatus=s.IsCheckedOut,
                    CheckInRefId=s.RJ_ID,
                    ClientId= s.Clientid
                })).ToList();

            return ResultData;
        }


        public IEnumerable<Jobs> GetAll7daysActiveJobs(string empCd)
        {

            var getJobs = unitOfwork.RIC_Job_Report.Get7daysActiveJobs(empCd);
            var ResultData = (getJobs
                .Select(s => new Jobs
                {
                    HDRID = s.RS_ID,
                    JobID = s.RS_JobID,
                    JobDivaRef = s.RS_JobDivaRef,
                    JobTitle = s.RS_JobTitle,
                    JobIssueDate = s.RS_JobIssueDate,
                    Company = s.RS_Company,
                    WorkLocation = s.RS_WorkLocation,
                    Priority = s.RS_Priority,
                    Division = s.RS_Division,
                    Category = s.RS_Category,
                    City = s.RS_City,
                    State = s.RS_State,
                    BillRate = s.RS_BillRate == null ? 0 : float.Parse(s.RS_BillRate.ToString()),
                    PayRate = s.RS_PayRate == null ? 0 : float.Parse(s.RS_PayRate.ToString()),
                    MaxSubAllowed = s.RS_MaxSubAllowed,
                    InternalSub = s.RS_InternalSub,
                    ExternalSub = s.RS_ExternalSub,
                    RMSJobStatus = s.RS_RMSJobStatus,
                    CreatedDate = s.RS_CreatedDt,
                    JobDivaStatus = s.RS_JobDivaStatus,
                    CheckInStatus = s.IsCheckedOut,
                    CheckInRefId = s.RJ_ID,
                    ClientId = s.Clientid
                })).ToList();

            return ResultData;
        }

    }
}