using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary.Repository
{
    public class SubmissionDailyRepository<TEntity> : GenericRepositiory<TEntity> where TEntity : RIC_SubmissionDaily
    {
        public SubmissionDailyRepository(RIC_DBEntities context)
            : base(context) // call the base constructor
        {
        }
        //get all submissions.
        public List<RIC_SubmissionDaily> GetSubmission()
        {
            List<RIC_SubmissionDaily> dailySubmission = new List<RIC_SubmissionDaily>();
            // get the submission fromn database.
            var list = (from sub in context.RIC_SubmissionDaily
                        join user in context.RIC_Employee
                        on sub.RS_Emp_Cd equals user.RE_Emp_Cd
                        select new
                            {
                                RS_Emp_Cd = sub.RS_Emp_Cd,
                                RE_Jobdiva_User_Name = user.RE_Jobdiva_User_Name,
                                RS_Date = sub.RS_Date,
                                RS_Submissions = sub.RS_Submissions,
                                RS_Interviews = sub.RS_Interviews,
                                RS_Hires = sub.RS_Hires
                            }).ToList();
            // var listOfSubmissions=                     
            foreach (var item in list) //retrieve each item and assign to model
            {
                dailySubmission.Add(new RIC_SubmissionDaily
                {
                    RS_Emp_Cd = item.RS_Emp_Cd,
                    RE_Jobdiva_User_Name = item.RE_Jobdiva_User_Name,
                    RS_Date = item.RS_Date,
                    RS_Submissions = item.RS_Submissions,
                    RS_Interviews = item.RS_Interviews,
                    RS_Hires = item.RS_Hires
                });
            }
            return dailySubmission;
        }


        public IEnumerable<RIC_SubmissionDaily> GetSubmissionsForUser(string empCd, DateTime startDt, DateTime endDate)
        {
            IEnumerable<RIC_SubmissionDaily> submissions = new List<RIC_SubmissionDaily>();         
            //get the total submisssions for user.
            submissions = (from submission in context.RIC_SubmissionDaily
                           join reportingHistory in context.RIC_ReportingHistory
                           on submission.RS_Emp_Cd equals reportingHistory.RR_EmpCD
                           where reportingHistory.RR_MgrCD == empCd
                           && (submission.RS_Date >= reportingHistory.RR_FromDate)
                           && (submission.RS_Date <= reportingHistory.RR_ToDate)
                           && (submission.RS_Date >= startDt )
                           && (submission.RS_Date <= endDate)
                           select submission).Union(
                       from submission in context.RIC_SubmissionDaily
                       where submission.RS_Date >= startDt && submission.RS_Date <= endDate && submission.RS_Emp_Cd == empCd
                       select submission
                       ).ToList().Select(sub => new RIC_SubmissionDaily()
                       {

                           RS_Date = sub.RS_Date,
                           RS_Emp_Cd = sub.RS_Emp_Cd,
                           RS_Hires = sub.RS_Hires,
                           RS_Interviews = sub.RS_Interviews,
                           RS_Sub_ID = sub.RS_Sub_ID,
                           RS_Submissions = sub.RS_Submissions
                       });
            return submissions;
        }




    }
}
