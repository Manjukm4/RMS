using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_DBEntities : DbContext
    {
        public DbSet<RIC_Employee> RIC_Employee { get; set; }
        public DbSet<RIC_User_Role> RIc_User_Role { get; set; }
        public DbSet<RIC_Role> RIC_Role { get; set; }
        public DbSet<RIC_Menu_Module> RIC_Menu_Module { get; set; }
        public DbSet<RIC_Submission> RIC_Submission { get; set; }
        public DbSet<RIC_SubmissionRule> RIC_SubmissionRule { get; set; }
        public DbSet<RIC_Holidays> RIC_Holidays { get; set; }
        public DbSet<RIC_SubmissionDaily> RIC_SubmissionDaily { get; set; }
        public DbSet<RIC_Call_Statistics> RIC_Call_Statistics { get; set; }
        public DbSet<RIC_CallStatisticError> RIC_CallStatisticErrors { get; set; }
        public DbSet<RIC_ReportingHistory> RIC_ReportingHistory { get; set; }
        public DbSet<RIC_Job_Report> RIC_Job_Report { get; set; }

        public DbSet<RIC_Incentive> RIC_Incentive { get; set; }
        public DbSet<RIC_ClientSetting> RIC_ClientSetting { get; set; }

        public DbSet<RIC_Review> RIC_Review { get; set; }

        public DbSet<RIC_Notification> RIC_Notification { get; set; }
        //public DbSet<RIC_Accesslog> RIC_Accesslogs { get; set; }

        // annual feedback tables
        public DbSet<RIC_AnnualFeedbackFields> RIC_AnnualFeedbackFields { get; set; }

        public DbSet<RIC_AnnualFeedbackForm> RIC_AnnualFeedbackForm { get; set; }

        public DbSet<RIC_AnnualFeedbackReviewers> RIC_AnnualFeedbackReviewers { get; set; }

        public DbSet<RIC_AnnualFeedback> RIC_AnnualFeedback { get; set; }
        public DbSet<RIC_AnnualFeedbackDtl> RIC_AnnualFeedbackDtl { get; set; }

        public DbSet<RMS_SharedReq_HDR> RMS_SharedReq_HDR { get; set; }
        public DbSet<RMS_SharedReq_Dtl> RMS_SharedReq_Dtl { get; set; }

        public DbSet<RMS_AssignSharedReq> RMS_AssignSharedReq { get; set; }

        public DbSet<RIC_PersonalFeedback> RIC_PersonalFeedback { get; set; }

        public DbSet<RIC_PersonalFeedbackDtl> RIC_PersonalFeedbackDtl { get; set; }

        public DbSet<RMS_SharedReqNotes> RMS_SharedReqNotes { get; set; }

       public DbSet<RMS_Designation> RMS_Designation { get; set; }

       public DbSet<RMS_Department> RMS_Department { get; set; }

       public DbSet<RIC_Client> RIC_Client { get; set; }
       public DbSet<RIC_ClientMapping> RIC_ClientMapping { get; set; }

       public DbSet<RIC_Targets> RIC_Targets { get; set; }

        public DbSet<RIC_MonthlyTarget> RIC_MonthlyTarget { get; set; }
        public DbSet<RIC_TargetsHistory> RIC_TargetsHistory { get; set; }

       public DbSet<RIC_UserDashboardPanel> RIC_UserDashboardPanel { get; set; }

       public DbSet<RIC_Panel> RIC_Panel { get; set; }

       public DbSet<RIC_QuarterlyIncentive> RIC_QuarterlyIncentive { get; set; }
        
       public DbSet<RIC_HRDiscussion_Hdr> RIC_HRDiscussion_Hdr { get; set; }

       public DbSet<RIC_HRDiscussion_Dtl> RIC_HRDiscussion_Dtl { get; set; }

       public DbSet<RMS_JobCheckedNotes> RMS_JobCheckedNotes { get; set; }
        




        //temp table
        //public DbSet<RIC_SubmissionsTemp> RIC_SubmissionsTemp { get; set;}
        public DbSet<RMS_ViewsConfig> RMS_ViewsConfig { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.Configuration.LazyLoadingEnabled = true;

            modelBuilder.Entity<RIC_Review>().Map(map =>
            {
                map.Properties(p =>
                    new
                    {
                        p.RR_Id,
                        p.RR_EmpCd,
                        p.RR_MgrCd,
                        p.RR_DirCd,
                        p.RR_HrCd,
                        p.RR_FromDate,
                        p.RR_ToDate,
                        p.RR_Submissions,
                        p.RR_Interviews,
                        p.RR_Hires,
                        p.RR_Calls,
                        p.RR_ReviewDate
                    });
                map.ToTable("RIC_Review");

            });
            modelBuilder.Entity<RIC_Review>().Map(map =>
            {
                map.Properties(p =>
                    new
                    {
                        p.RR_TlFindings,
                        p.RR_Improvements,
                        p.RR_AdditionalComments,
                        p.RR_DirectorFeedback,
                        p.RR_HrFeedback,
                        p.RR_DirectorFeedbackRequired,
                        p.RR_HrFeedbackRequired,
                        p.RR_NextReviewDate,
                        p.RR_SubmissionTarget,
                        p.RR_InterviewTarget,
                        p.RR_HiresTarget,
                        p.RR_CallsTarget,
                        p.RR_Draft,
                        p.RR_DirectorFeedbackStatus,
                        p.RR_HrFeedbackStatus,
                        p.RR_Discarded
                    });
                map.ToTable("RIC_Feedback");

            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
