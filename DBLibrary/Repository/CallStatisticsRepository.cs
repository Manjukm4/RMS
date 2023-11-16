using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.Entity;


namespace DBLibrary.Repository
{
   public class CallStatisticsRepository<TEntity> : GenericRepositiory<TEntity> where TEntity : RIC_Call_Statistics
    {
       string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        public CallStatisticsRepository(RIC_DBEntities context)
            : base(context) // call the base constructor
        {
        }

        public IEnumerable<RIC_Call_Statistics> GetCallStataticsForUser(string empCd, DateTime startDt, DateTime endDate,string role=null)
        {
            string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
            IEnumerable<RIC_Call_Statistics> submissions = new List<RIC_Call_Statistics>();
            //get the call statistics for user.
            context.Database.CommandTimeout = 950;
            submissions = (from callStatistics in context.RIC_Call_Statistics
                           join reportingHistory in context.RIC_ReportingHistory
                           on callStatistics.RC_Emp_Cd equals reportingHistory.RR_EmpCD
                           where (reportingHistory.RR_MgrCD == empCd )
                           && (callStatistics.RC_Date >= reportingHistory.RR_FromDate)
                           && (callStatistics.RC_Date <= reportingHistory.RR_ToDate)
                           && (callStatistics.RC_Date >= startDt)
                           && (callStatistics.RC_Date <= endDate)
                           select callStatistics).Union(
                       from callStatistics in context.RIC_Call_Statistics
                       where callStatistics.RC_Date >= startDt && callStatistics.RC_Date <= endDate && (callStatistics.RC_Emp_Cd == empCd || role == directorRoleName)
                       select callStatistics
                       ).ToList().Select(cs => new RIC_Call_Statistics()
                       {
                           RC_Id=cs.RC_Id,
                           RC_Emp_Cd=cs.RC_Emp_Cd,
                           RC_Date=cs.RC_Date,
                           RC_Time=cs.RC_Time,
                           RC_CallType=cs.RC_CallType,
                           RC_Dailed=cs.RC_Dailed,
                           RC_Calling=cs.RC_Calling,
                           RC_Duration=cs.RC_Duration,
                           RC_Call_Connected=cs.RC_Call_Connected,
                           RC_Voice_Message=cs.RC_Voice_Message,
                           RC_PRI=cs.RC_PRI
                       });
            return submissions;
        }
       //ashutosh
        public IEnumerable<RIC_Call_Statistics> GetCallStatatics(string empCd,string role = null)
        {
           
            IEnumerable<RIC_Call_Statistics> submissions = new List<RIC_Call_Statistics>();
            //get the call statistics for user.
            submissions = (from callStatistics in context.RIC_Call_Statistics
                           join reportingHistory in context.RIC_ReportingHistory
                           on callStatistics.RC_Emp_Cd equals reportingHistory.RR_EmpCD
                           where reportingHistory.RR_MgrCD == empCd 
                           && (callStatistics.RC_Date >= reportingHistory.RR_FromDate)
                           && (callStatistics.RC_Date <= reportingHistory.RR_ToDate)                      
                           select callStatistics).Union(
                       from callStatistics in context.RIC_Call_Statistics
                       where callStatistics.RC_Emp_Cd == empCd || role == directorRoleName
                       select callStatistics
                       ).ToList().Select(cs => new RIC_Call_Statistics()
                       {
                           RC_Id = cs.RC_Id,
                           RC_Emp_Cd = cs.RC_Emp_Cd,
                           RC_Date = cs.RC_Date,
                           RC_Time = cs.RC_Time,
                           RC_CallType = cs.RC_CallType,
                           RC_Dailed = cs.RC_Dailed,
                           RC_Calling = cs.RC_Calling,
                           RC_Duration = cs.RC_Duration,
                           RC_Call_Connected = cs.RC_Call_Connected,
                           RC_Voice_Message = cs.RC_Voice_Message,
                           RC_PRI = cs.RC_PRI
                       });
            return submissions;
        }

        //public IEnumerable<OperationalViewModel> GetCallStataticsOperational(string empCd, DateTime startDt, DateTime endDate, string role)
        //{
        //    string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        //    IEnumerable<RIC_Call_Statistics> submissions = new List<RIC_Call_Statistics>();
        //    IEnumerable<OperationalViewModel> submissions1 = new List<OperationalViewModel>();
        //    //get the call statistics for user.
        //    submissions = (from callStatistics in context.RIC_Call_Statistics
        //                   join reportingHistory in context.RIC_ReportingHistory
        //                   on callStatistics.RC_Emp_Cd equals reportingHistory.RR_EmpCD
        //                   where reportingHistory.RR_MgrCD == empCd || role == directorRoleName
        //                   && (callStatistics.RC_Date >= reportingHistory.RR_FromDate)
        //                   && (callStatistics.RC_Date <= reportingHistory.RR_ToDate)
        //                   && (callStatistics.RC_Date >= startDt)
        //                   && (callStatistics.RC_Date <= endDate)
        //                   select callStatistics).Union(
        //               from callStatistics in context.RIC_Call_Statistics
        //               where callStatistics.RC_Date >= startDt && callStatistics.RC_Date <= endDate && callStatistics.RC_Emp_Cd == empCd
        //               select callStatistics
        //               ).ToList().Select(cs => new RIC_Call_Statistics()
        //               {
        //                   RC_Id = cs.RC_Id,
        //                   RC_Emp_Cd = cs.RC_Emp_Cd,
        //                   RC_Date = cs.RC_Date,
        //                   RC_Time = cs.RC_Time,
        //                   RC_CallType = cs.RC_CallType,
        //                   RC_Dailed = cs.RC_Dailed,
        //                   RC_Calling = cs.RC_Calling,
        //                   RC_Duration = cs.RC_Duration,
        //                   RC_Call_Connected = cs.RC_Call_Connected,
        //                   RC_Voice_Message = cs.RC_Voice_Message,
        //                   RC_PRI = cs.RC_PRI
        //               });

        //   submissions1 = (from callStatistics in context.RIC_Call_Statistics
        //                   where
        //                      // Convert.ToString(Convert.ToDateTime(RIC_Call_Statistics.RC_Date)) == "2018-06-15" &&
        //                       // callStatistics.RC_Date.Date == DateTime.Today &&
        //                        //Convert.ToDateTime(callStatistics.RC_Date.Date).Year == SystemClock.US_Date.Year &&
        //                     callStatistics.RC_Emp_Cd == empCd //"SBS0088"
        //                   group callStatistics by new
        //                   {
        //                       callStatistics.RC_CallType,
        //                       callStatistics.RC_Call_Connected,
        //                       callStatistics.RC_Voice_Message,
        //                       Column1 = (int?)callStatistics.RC_Time.Hours
        //                   } into g

        //                   select new OperationalViewModel
        //                   {
        //                       OpCount = g.Count(),
        //                       OpHour = g.Key.Column1,
        //                       //OpCallTime = (DateTime?)g.Max(p => Convert.ToDateTime(p.RC_Date)),
        //                       OpCallType =
        //                       g.Key.RC_CallType == "Out" ? (
        //                       g.Key.RC_Call_Connected == 1 ? "Call Connected (IN)" :
        //                       g.Key.RC_Voice_Message == 1 ? "Voice Message (IN)" : null) :
        //                       g.Key.RC_CallType == "In" ? (
        //                       g.Key.RC_Call_Connected == 1 ? "Call Connected (Out)" :
        //                       g.Key.RC_Voice_Message == 1 ? "Voice Message (Out)" : null) : null
        //                   }).ToList();



        //   // submissions1 = submissions2.ToList();





        //    return submissions1;
        //}

        //public object GetCallStataticsOperational(string empCd, DateTime startOfMonth, DateTime dateTime, string role)
        //{
        //    throw new NotImplementedException();
        //}

        public IEnumerable<OperationalResult> getOperationalResults(string empCd,DateTime currentDate,string role = null)
        {

            DateTime endDate = currentDate.AddDays(1);
            //added by ashish.
           //var   callStats1 = (from callStatistics in context.RIC_Call_Statistics
           //                join reportingHistory in context.RIC_ReportingHistory
           //                on callStatistics.RC_Emp_Cd equals reportingHistory.RR_EmpCD
           //                where reportingHistory.RR_MgrCD == empCd 
           //                && (callStatistics.RC_Date >= reportingHistory.RR_FromDate)
           //                && (callStatistics.RC_Date <= reportingHistory.RR_ToDate)
           //                && (callStatistics.RC_Date >= currentDate)
           //                && (callStatistics.RC_Date <= endDate)
           //                select callStatistics).Union(
           //           from callStatistics in context.RIC_Call_Statistics
           //           where callStatistics.RC_Date >= currentDate && callStatistics.RC_Date <= endDate && (callStatistics.RC_Emp_Cd == empCd || role == directorRoleName)
           //           select callStatistics
           //           );


           var callStats = (from callStatistics in context.RIC_Call_Statistics
                           join reportingHistory in context.RIC_ReportingHistory
                           on callStatistics.RC_Emp_Cd equals reportingHistory.RR_EmpCD
                           where (reportingHistory.RR_MgrCD == empCd )
                           && (callStatistics.RC_Date >= reportingHistory.RR_FromDate)
                           && (callStatistics.RC_Date <= reportingHistory.RR_ToDate)
                           && (callStatistics.RC_Date >= currentDate)
                           && (callStatistics.RC_Date <= endDate)
                           select callStatistics).Union(
                       from callStatistics in context.RIC_Call_Statistics
                       where callStatistics.RC_Date >= currentDate && callStatistics.RC_Date <= endDate &&( callStatistics.RC_Emp_Cd == empCd || role == directorRoleName)
                       select callStatistics
                      );



            //var result =
            // context.Database.SqlQuery<OperationalResults>("[dbo].[SP_UnpivotIncentives]").ToList();
            //return result;

            //IEnumerable<OperationalResults> OpResults = new List<OperationalResults>();
            var submissions1 = (
                     //from callStatistics in context.RIC_Call_Statistics
                          from callStatistics in callStats//changed by ashish 25-06-2018 so we will get call statistics for all users in team.
                                where
                                  //DbFunctions.TruncateTime(callStatistics.RC_Date) == DbFunctions.TruncateTime(DbFunctions.AddDays(DateTime.Today,-10)) &&
                                   DbFunctions.TruncateTime(callStatistics.RC_Date) == DbFunctions.TruncateTime(currentDate)//&&
                                group callStatistics by new
                                {
                                    callStatistics.RC_CallType,
                                    callStatistics.RC_Call_Connected,
                                    callStatistics.RC_Voice_Message,
                                    Column1 = (int?)callStatistics.RC_Time.Hours
                                } into g

                                select new OperationalResult
                                {
                                    OpCount = g.Count(),
                                    OpHour = g.Key.Column1,
                                    OpCallTime = DbFunctions.TruncateTime(g.Max(p => p.RC_Date)),
                                    //OpCallTime = (DateTime?)g.Max(p => Convert.ToDateTime(p.RC_Date)),
                                    OpCallType =
                                    g.Key.RC_CallType == "Out" ? (
                                    g.Key.RC_Call_Connected == 1 ? "Call Connected (Out)" :
                                    g.Key.RC_Voice_Message == 1 ? "Voice Message (Out)" : null) :
                                    g.Key.RC_CallType == "In" ? (
                                    g.Key.RC_Call_Connected == 1 ? "Call Connected (In)" :
                                    g.Key.RC_Voice_Message == 1 ? "Voice Message (In)" : null) : null
                                }).ToList();

            return submissions1;
        }
    }

}
