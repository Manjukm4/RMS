using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBLibrary.Repository;

namespace DBLibrary.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        public RIC_DBEntities context;

        public UnitOfWork()
        {
            this.context = new RIC_DBEntities();
        }




        public EmployeeRepository<RIC_Employee> User
        {
            get
            {
                return new EmployeeRepository<RIC_Employee>(context);
            }
        }



        public RoleRepositery<RIC_Role> Role
        {
            get
            {
                return new RoleRepositery<RIC_Role>(context);
            }
        }

        public GenericRepositiory<RMS_Designation> Designation
        {
            get
            {
                return new GenericRepositiory<RMS_Designation>(context);
            }
        }


        public GenericRepositiory<RMS_Department> Department
        {
            get
            {
                return new GenericRepositiory<RMS_Department>(context);
            }
        }

        public MenuRepository<RIC_Menu_Module> Menu
        {
            get
            {
                return new MenuRepository<RIC_Menu_Module>(context);
            }
        }
        public GenericRepositiory<RIC_Submission> Submission
        {
            get
            {
                return new GenericRepositiory<RIC_Submission>(context);
            }
        }

        public GenericRepositiory<RIC_SubmissionRule> SubmissionRule
        {
            get
            {
                return new GenericRepositiory<RIC_SubmissionRule>(context);
            }
        }

        public GenericRepositiory<RIC_Holidays> Holidays
        {
            get
            {
                return new GenericRepositiory<RIC_Holidays>(context);
            }
        }

        public SubmissionDailyRepository<RIC_SubmissionDaily> SubmissionDaily
        {
            get
            {
                return new SubmissionDailyRepository<RIC_SubmissionDaily>(context);
            }
        }
        public CallStatisticsRepository<RIC_Call_Statistics> CallStatistics
        {
            get
            {
                return new CallStatisticsRepository<RIC_Call_Statistics>(context);
            }

        }

        public GenericRepositiory<RIC_CallStatisticError> RIC_CallStatisticError
        {
            get
            {
                return new GenericRepositiory<RIC_CallStatisticError>(context);
            }

        }


        public GenericRepositiory<RIC_ReportingHistory> ReportingHistory
        {
            get
            {
                return new GenericRepositiory<RIC_ReportingHistory>(context);
            }

        }

        public Job_ReportRepository<RIC_Job_Report> RIC_Job_Report
        {
            get
            {
                return new Job_ReportRepository<RIC_Job_Report>(context);
            }

        }

        //public Job_ReportRepository<AccessLogList> AccessLogList
        //{
        //    get
        //    {
        //        return new Job_ReportRepository<AccessLogList>(context);
        //    }

        //}
        public IncentiveRepository<RIC_Incentive> Incentive
        {

            get
            {
                return new IncentiveRepository<RIC_Incentive>(context);
            }
        }

        public GenericRepositiory<RIC_QuarterlyIncentive> QuarterlyIncentive
        {
            get
            {
                return new GenericRepositiory<RIC_QuarterlyIncentive>(context);
            }
        }


        public GenericRepositiory<RIC_ClientSetting> ClientSetting
        {
            get
            {
                return new GenericRepositiory<RIC_ClientSetting>(context);
            }
        }


        public GenericRepositiory<RIC_Review> Review
        {
            get
            {
                return new GenericRepositiory<RIC_Review>(context);
            }
        }


        public GenericRepositiory<RIC_Notification> Notification
        {

            get
            {

                return new GenericRepositiory<RIC_Notification>(context);
            }
        }

        public GenericRepositiory<RIC_AnnualFeedback> RIC_AnnualFeedback
        {
            get
            {
                return new GenericRepositiory<RIC_AnnualFeedback>(context);
            }
        }


        public GenericRepositiory<RIC_AnnualFeedbackDtl> RIC_AnnualFeedbackDtl
        {
            get { return new GenericRepositiory<RIC_AnnualFeedbackDtl>(context); }
        }

        public GenericRepositiory<RIC_AnnualFeedbackFields> RIC_AnnualFeedbackFields
        {
            get
            {
                return new GenericRepositiory<RIC_AnnualFeedbackFields>(context);
            }
        }


        public GenericRepositiory<RIC_AnnualFeedbackForm> RIC_AnnualFeedbackForm
        {
            get { return new GenericRepositiory<RIC_AnnualFeedbackForm>(context); }
        }


        public GenericRepositiory<RIC_AnnualFeedbackReviewers> RIC_AnnualFeedbackReviewers
        {
            get { return new GenericRepositiory<RIC_AnnualFeedbackReviewers>(context); }
        }

        //added by shaurya.
        public SharedReqRepository<RMS_SharedReq_HDR> RMS_SharedReq_HDR
        {
            get { return new SharedReqRepository<RMS_SharedReq_HDR>(context); }
        }

        public GenericRepositiory<RMS_SharedReq_Dtl> RMS_SharedReq_Dtl
        {
            get { return new GenericRepositiory<RMS_SharedReq_Dtl>(context); }
        }

        public GenericRepositiory<RMS_AssignSharedReq> RMS_AssignSharedReq
        {
            get { return new GenericRepositiory<RMS_AssignSharedReq>(context); }
        }


        public GenericRepositiory<RIC_User_Role> RIC_User_Role
        {
            get { return new GenericRepositiory<RIC_User_Role>(context); }
        }

        public GenericRepositiory<RIC_Employee> RIC_Employee
        {
            get { return new GenericRepositiory<RIC_Employee>(context); }
        }

        public GenericRepositiory<RIC_ClientMapping> RIC_ClientMapping
        {
            get { return new GenericRepositiory<RIC_ClientMapping>(context); }
        }

        public GenericRepositiory<RIC_Client> RIC_Client
        {
            get { return new GenericRepositiory<RIC_Client>(context); }
        }

        public GenericRepositiory<RMS_SharedReqSubmittals> RMS_SharedReqSubmittals
        {
            get { return new GenericRepositiory<RMS_SharedReqSubmittals>(context); }
        }


        public GenericRepositiory<RIC_PersonalFeedback> RIC_PersonalFeedback
        {
            get { return new GenericRepositiory<RIC_PersonalFeedback>(context); }
        }
        public GenericRepositiory<RIC_PersonalFeedbackDtl> RIC_PersonalFeedbackDtl
        {
            get { return new GenericRepositiory<RIC_PersonalFeedbackDtl>(context); }
        }

        public GenericRepositiory<RMS_SharedReqNotes> RMS_SharedReqNotes
        {
            get { return new GenericRepositiory<RMS_SharedReqNotes>(context); }
        }

        //temp table
        public GenericRepositiory<RIC_SubmissionsTemp> SubmissionsTemp
        {
            get
            {
                return new GenericRepositiory<RIC_SubmissionsTemp>(context);
            }
        }
        public GenericRepositiory<RMS_ViewsConfig> ViewsConfig
        {
            get
            {
                return new GenericRepositiory<RMS_ViewsConfig>(context);
            }
        }
        public IEnumerable ExecuteRawQuery(Type ResultType, string sql, params object[] parameters)
        {
           return context.Database.SqlQuery(ResultType, sql, parameters);
        }

        public GenericRepositiory<RIC_HRDiscussion_Hdr> HRDiscussion
        {
            get
            {
                return new GenericRepositiory<RIC_HRDiscussion_Hdr>(context);
            }
        }

        public GenericRepositiory<RIC_HRDiscussion_Dtl> HRDiscussionDtl
        {
            get
            {
                return new GenericRepositiory<RIC_HRDiscussion_Dtl>(context);
            }
        }

        public GenericRepositiory<RMS_JobChecked_Info> RMS_JobChecked_Info
        {
            get
            {
                return new GenericRepositiory<RMS_JobChecked_Info>(context);
            }
        }

        public GenericRepositiory<RMS_JobCheckedNotes> RMS_JobCheckedNotes
        {
            get
            {
                return new GenericRepositiory<RMS_JobCheckedNotes>(context);
            }
        }

        //Added by Dinesh
        public ClientRepository<RIC_Client> Client

        {
            get
            {
                return new ClientRepository<RIC_Client>(context);
            }
        }

        // save the changes.
        public void Save()
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw e;
            }
        }
        private bool disposed = false;
        // dispose the context.
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
