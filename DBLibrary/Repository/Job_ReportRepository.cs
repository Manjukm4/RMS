using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data;

namespace DBLibrary.Repository
{
    public class Job_ReportRepository<TEntity> : GenericRepositiory<TEntity> where TEntity : RIC_Job_Report
    {
        string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
        string HrRoleName = System.Configuration.ConfigurationManager.AppSettings["HRRole"];
        string DirectorStaffingRoleName = System.Configuration.ConfigurationManager.AppSettings["StaffingDirector"];

        UnitOfWork.UnitOfWork unitOfWork = new UnitOfWork.UnitOfWork();

        DateTime usDate = SystemClock.US_Date;
        public Job_ReportRepository(RIC_DBEntities context)
              : base(context) // call the base constructor
        {

        }
        public IEnumerable<RIC_Job_Report> GetAll()
        {
            //get all distinct records.
            IEnumerable<RIC_Job_Report> distinctReport = context.Database.SqlQuery<RIC_Job_Report>("select * from  RIC_JobMasterView").ToList();
            return distinctReport;
        }

        public IEnumerable<RIC_Job_Report> Get_JobRepoartForUser(string empCd, DateTime startDt, DateTime endDate, string role = null, string dataJr = "All")
        {

            bool getAllrecord = false;
            if (role == this.directorRoleName || role == this.AccMgrRoleName || role == this.HrRoleName || role == this.DirectorStaffingRoleName)
                getAllrecord = true;

            string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
            string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
            string DirectorStaffingRoleName = System.Configuration.ConfigurationManager.AppSettings["StaffingDirector"];
            //IEnumerable<RIC_Job_Report> Job_Report = new List<RIC_Job_Report>();
            var Job_Report = context.Database.SqlQuery<RIC_Job_Report>(
                                 "[dbo].[SP_Get_JobRepoartForUser] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@DataType",
                                  new SqlParameter("StartDate", startDt),
                                  new SqlParameter("EndDate", endDate),
                                  new SqlParameter("EmpCd", empCd),
                                  new SqlParameter("GetAllRecord", getAllrecord),
                                  new SqlParameter("DataType", dataJr)
                                  ).ToList();
            return Job_Report;
        }

        public IEnumerable<ClientRequiredDetails> Get_Job_required_details(string Company,DateTime FromDt, DateTime endDate, string param,string role = null, string EmpCd = null)
        {

            bool getAllrecord = false;
            if (role == this.directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
            string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
            //IEnumerable<RIC_Job_Report> Job_Report = new List<RIC_Job_Report>();
            var Job_Report = context.Database.SqlQuery<ClientRequiredDetails>(
                                 "[dbo].[SP_Get_Required_Deatils] @client,@fromdate,@todate,@parm,@GetAllRecord,@EmpCd",
                                  new SqlParameter("client", Company),
                                 new SqlParameter("fromdate", FromDt),
                                  new SqlParameter("todate", endDate),
                                  new SqlParameter("parm", param),
                                   new SqlParameter("GetAllRecord", getAllrecord),
                                    new SqlParameter("EmpCd", EmpCd)
                                  ).ToList();
            return Job_Report;
        }

        public IEnumerable<TotalRecruiterdetails> Get_TotalRecruiter_details(string Company, DateTime FromDt, DateTime endDate, string param, string role = null, string EmpCd = null)
        {

            bool getAllrecord = false;
            if (role == this.directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
            string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
            //IEnumerable<RIC_Job_Report> Job_Report = new List<RIC_Job_Report>();
            var Job_Report = context.Database.SqlQuery<TotalRecruiterdetails>(
                                 "[dbo].[sp_totalrecruiter_details]  @EmpCd,@param,@company,@FromDate,@Todate,@GetAllRecord",
                                  //new SqlParameter("client", Company),
                                  new SqlParameter("EmpCd", EmpCd),
                                  new SqlParameter("param", param),
                                  new SqlParameter("company", Company),
                                 new SqlParameter("FromDate", FromDt),
                                  new SqlParameter("Todate", endDate),
                                   new SqlParameter("GetAllRecord", getAllrecord)
                                  //


                                  ).ToList();
            return Job_Report;
        }
        public IEnumerable<String> Get_ClientList_requirments( DateTime startDt, DateTime endDate)
        {
            endDate = endDate.AddSeconds(-1);
            //get the list of companys for user.
            List<string> list = context.Database.SqlQuery<string>(
                                 "[dbo].[get_clientList_Requirment]  @Startdate,@Enddate",
                                  new SqlParameter("Startdate", startDt),
                                 new SqlParameter("Enddate", endDate)
                                  
                                  ).ToList();
            return list;
        }

        public IEnumerable<Sp_PositionDetails> Get_Job_Position_Details(string empCd, DateTime startDt, DateTime endDate, string role, string PositionType)
        {

            bool getAllrecord = false;
            if (role == this.directorRoleName || role == this.DirectorStaffingRoleName)
                getAllrecord = true;

            var Job_Report = context.Database.SqlQuery<Sp_PositionDetails>(
                                 "[dbo].[sp_getPosition_details_companyWise] @StartDate,@Enddate,@getAllrecords,@Empcd,@Positiondetails",
                                  new SqlParameter("StartDate", startDt),
                                  new SqlParameter("Enddate", endDate),
                                  new SqlParameter("getAllrecords", getAllrecord),
                                  new SqlParameter("Empcd", empCd),
                                  new SqlParameter("Positiondetails", PositionType)
                                  ).ToList();
            return Job_Report;
        }

        public IEnumerable<Sp_PositionDetails> Get_SubmissionDetails(string empCd, DateTime startDt, DateTime endDate, string role, string Refno)
        {

            bool getAllrecord = false;
            if (role == this.directorRoleName || role == this.DirectorStaffingRoleName)
                getAllrecord = true;

            var Job_Report = context.Database.SqlQuery<Sp_PositionDetails>(
                                 "[dbo].[SP_SubmissionDetailsPop] @EmpCd,@Refno,@FromDate,@Todate,@GetAllRecord",
                                  new SqlParameter("EmpCd", empCd),
                                  new SqlParameter("Refno", Refno),
                                 new SqlParameter("FromDate", startDt),
                                  new SqlParameter("Todate", endDate),
                                  new SqlParameter("GetAllRecord", getAllrecord)
                                 
                                  ).ToList();
            return Job_Report;
        }

        public List<AccessLogResult> Get_AccesslogJobRepoartForUser(DateTime Startdate, DateTime EndDate, string EmpCd, string role = null)
        {
            bool getAllrecord = false;
            if (role == this.directorRoleName || role == this.AccMgrRoleName || role == this.HrRoleName)
                getAllrecord = true;

            string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
            string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
            //IEnumerable<RIC_Job_Report> Job_Report = new List<RIC_Job_Report>();
            var accessLogLists = context.Database.SqlQuery<AccessLogResult>(
                                 "[dbo].[Sp_AccessLogList] @StartDate,@EndDate,@EmpCd,@GetAllRecord",
                                  new SqlParameter("StartDate", Startdate),
                                  new SqlParameter("EndDate", EndDate),
                                   new SqlParameter("EmpCd", EmpCd),
                                  new SqlParameter("GetAllRecord", getAllrecord)

                                  ).ToList();
            return accessLogLists;
        }

        //added by Shaurya..
        public IEnumerable<RMS_JDIssuedDt> Get_RMS_JDIssuedDt()
        {
            return context.Database.SqlQuery<RMS_JDIssuedDt>("select * from RMS_JDIssuedDt").ToList();
        }

        public IEnumerable<GetDashboardMatricsResult> Get_DashboardMatrics(string empCd, DateTime startOfMonth, DateTime endOfMonth, DateTime endDate, string role = null, string getIndividualRecords = null, int EmpId = 0)
        {
            endDate = endDate.AddSeconds(-1);
            bool getAllrecord = false;

            bool individualRecords = getIndividualRecords == "Yes" ? true : false;
            if (role == directorRoleName || role == AccMgrRoleName)
                getAllrecord = true;

            if (role == "Accounting Manager")
            {
                getAllrecord = false;
                IEnumerable<GetDashboardMatricsResult> dashbordMatrix =
                  context.Database.SqlQuery<GetDashboardMatricsResult>(
                                     "[dbo].[SP_Dashboard_Metrics_Week_AM] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@EndOfMonth,@GetIndividualRecords,@EMP_ID",
                                      new SqlParameter("StartDate", startOfMonth),
                                      new SqlParameter("EndDate", endDate),
                                      new SqlParameter("EmpCd", empCd),
                                      new SqlParameter("GetAllRecord", getAllrecord),
                                      new SqlParameter("@EndOfMonth", endOfMonth),
                                      new SqlParameter("@GetIndividualRecords", individualRecords),
                                       new SqlParameter("@EMP_ID", EmpId)
                                      );
                return dashbordMatrix;
            }
            else
            {
                IEnumerable<GetDashboardMatricsResult> dashbordMatrix =
                 context.Database.SqlQuery<GetDashboardMatricsResult>(
                                    "[dbo].[SP_Dashboard_Metrics_Week] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@EndOfMonth,@GetIndividualRecords",
                                     new SqlParameter("StartDate", startOfMonth),
                                     new SqlParameter("EndDate", endDate),
                                     new SqlParameter("EmpCd", empCd),
                                     new SqlParameter("GetAllRecord", getAllrecord),
                                     new SqlParameter("@EndOfMonth", endOfMonth),
                                     new SqlParameter("@GetIndividualRecords", individualRecords)
                                     );
                return dashbordMatrix;
            }
        }

        public AccessLogList GetTeamMatrixForUser(string empCd, DateTime startOfMonth, DateTime endOfMonth, DateTime endDate, string role = null, int EmpID = 0)
        {
            endDate = endDate.AddSeconds(-1);
            bool getAllrecord = false;

            if (role == directorRoleName )
                getAllrecord = true;

            //IEnumerable<TeamMatrixResult> dashbordMatrix =
            //  context.Database.SqlQuery<TeamMatrixResult>(
            //                     "[dbo].[SP_TeamMatrixAccessLog] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@EndOfMonth",
            //                      new SqlParameter("@StartDate", startOfMonth),
            //                      new SqlParameter("@EndDate", endDate),
            //                      new SqlParameter("@EmpCd", empCd),
            //                      new SqlParameter("@GetAllRecord", getAllrecord),
            //                      new SqlParameter("@EndOfMonth", endOfMonth)
            //                      ).ToList();

            SqlParameter param;

            AccessLogList domainEntity = new AccessLogList();

            var command = context.Database.Connection.CreateCommand();



            context.Database.Connection.Open();
            if (role == "Accounting Manager")
            {
                command.CommandText = "[dbo].[SP_TeamMatrixAccessLog_AM]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@StartDate", startOfMonth));
                command.Parameters.Add(new SqlParameter("@EndDate", endDate));
                command.Parameters.Add(new SqlParameter("@EndOfMonth", endOfMonth));
                command.Parameters.Add(new SqlParameter("@GetAllRecord", getAllrecord));
                command.Parameters.Add(new SqlParameter("@EmpCd", empCd.ToString()));
                command.Parameters.Add(new SqlParameter("@EMP_ID", EmpID));
            }
            else
            {
                command.CommandText = "[dbo].[SP_TeamMatrixAccessLog]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@StartDate", startOfMonth));
                command.Parameters.Add(new SqlParameter("@EndDate", endDate));
                command.Parameters.Add(new SqlParameter("@EndOfMonth", endOfMonth));
                command.Parameters.Add(new SqlParameter("@GetAllRecord", getAllrecord));
                command.Parameters.Add(new SqlParameter("@EmpCd", empCd.ToString()));
            }
            //param = new SqlParameter("@StartDate", startOfMonth);
            //new SqlParameter("@EndDate", endDate);
            //new SqlParameter("@EmpCd", empCd);
            //new SqlParameter("@GetAllRecord", getAllrecord);
            //new SqlParameter("@EndOfMonth", endOfMonth);
            //param.Direction = ParameterDirection.Input;
            //param.DbType = DbType.String;
            //command.Parameters.Add(param);



            //var command1 = context.Database.SqlQuery<TeamMatrixResult>(
            //                     "[dbo].[SP_TeamMatrixAccessLog] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@EndOfMonth",
            //                      new SqlParameter("@StartDate", startOfMonth),
            //                      new SqlParameter("@EndDate", endDate),
            //                      new SqlParameter("@EmpCd", empCd),
            //                      new SqlParameter("@GetAllRecord", getAllrecord),
            //                      new SqlParameter("@EndOfMonth", endOfMonth)
            //                      ).ToList();
            var reader = command.ExecuteReader();






            List<TeamMatrixResult> _listOfMainMatrix =
            ((IObjectContextAdapter)context).ObjectContext.Translate<TeamMatrixResult>
            (reader).ToList();
            reader.NextResult();

            List<AccessLogResult> _listOfAccesslog =
                ((IObjectContextAdapter)context).ObjectContext.Translate<AccessLogResult>
        (reader).ToList();
            //  Console.WriteLine(domainEntity);
            domainEntity.TeamMatrixResult = _listOfMainMatrix;
            domainEntity.AccessLogResults = _listOfAccesslog;
            return domainEntity;
        }
        public AccessLogList GetTeamMatrixForUser_AccessLog(string empCd, DateTime startOfMonth, DateTime endOfMonth, DateTime endDate, string role = null)
        {
            endDate = endDate.AddSeconds(-1);
            bool getAllrecord = false;

            if (role == directorRoleName || role == AccMgrRoleName)
                getAllrecord = true;

            //IEnumerable<TeamMatrixResult> dashbordMatrix =
            //  context.Database.SqlQuery<TeamMatrixResult>(
            //                     "[dbo].[SP_TeamMatrixAccessLog] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@EndOfMonth",
            //                      new SqlParameter("@StartDate", startOfMonth),
            //                      new SqlParameter("@EndDate", endDate),
            //                      new SqlParameter("@EmpCd", empCd),
            //                      new SqlParameter("@GetAllRecord", getAllrecord),
            //                      new SqlParameter("@EndOfMonth", endOfMonth)
            //                      ).ToList();

            SqlParameter param;

            AccessLogList domainEntity = new AccessLogList();

            var command = context.Database.Connection.CreateCommand();

            command.CommandText = "[dbo].[Sp_AccessLogList]";
            command.CommandType = CommandType.StoredProcedure;

            context.Database.Connection.Open();



            command.Parameters.Add(new SqlParameter("@StartDate", startOfMonth));
            command.Parameters.Add(new SqlParameter("@EndDate", endDate));
            command.Parameters.Add(new SqlParameter("@EndOfMonth", endOfMonth));
            command.Parameters.Add(new SqlParameter("@EmpCd", empCd.ToString()));

            var reader = command.ExecuteReader();
            List<AccessLogResult> _listOfAccesslog =
                ((IObjectContextAdapter)context).ObjectContext.Translate<AccessLogResult>
        (reader).ToList();
            domainEntity.AccessLogResults = _listOfAccesslog;
            return domainEntity;
        }

        public IEnumerable<TeamMatrixResult> GetManageJobTeamMatrixForUser(string empCd, DateTime startOfMonth, DateTime endOfMonth, DateTime endDate, string role = null)
        {
            endDate = endDate.AddSeconds(-1);
            bool getAllrecord = false;

            if (role == directorRoleName || role == AccMgrRoleName)
                getAllrecord = true;

            IEnumerable<TeamMatrixResult> dashbordMatrix =
              context.Database.SqlQuery<TeamMatrixResult>(
                                 "[dbo].[SP_GetManageJobsDashboard] @StartDate,@EndDate,@EmpCd,@GetAllRecord,@EndOfMonth",
                                  new SqlParameter("@StartDate", startOfMonth),
                                  new SqlParameter("@EndDate", endDate),
                                  new SqlParameter("@EmpCd", empCd),
                                  new SqlParameter("@GetAllRecord", getAllrecord),
                                  new SqlParameter("@EndOfMonth", endOfMonth)
                                  ).ToList();
            return dashbordMatrix;
        }

        public RMS_SubmissionAnalysis GetSubmissionAnalysisByEnpCd(String EmpCd)
        {
            RMS_SubmissionAnalysis RMS_SubmissionAnalysis = new DBLibrary.RMS_SubmissionAnalysis();
            RMS_SubmissionAnalysis = context.Database.SqlQuery<RMS_SubmissionAnalysis>("select * from dbo.RMS_SubmissionAnalysis")
                   .FirstOrDefault(s => s.Emp_Cd == EmpCd);
            return RMS_SubmissionAnalysis;
        }

        public IEnumerable<String> Get_ClientList(string empCD, DateTime startDt, DateTime endDate)
        {
            endDate = endDate.AddSeconds(-1);
            //get the list of companys for user.
            List<string> list = this.Get_JobRepoartForUser(empCD, startDt, endDate).GroupBy(s => s.RJ_Company)
                                      .Select(g => g.Key).ToList();
            return list;
        }

        public List<OperationalDashboard_SpResult> GetOperationalSubmission(string empCd, DateTime startDate, DateTime endDate, string role = null)
        {
            //endDate = startDate.AddDays(1).AddTicks(-1);
            bool getAllrecord = false;

            if (role == directorRoleName || role == AccMgrRoleName)
                getAllrecord = true;



            List<OperationalDashboard_SpResult> OperationalSubmission =
              context.Database.SqlQuery<OperationalDashboard_SpResult>(
                                 "[dbo].[SP_GetOperationalDashboard] @StartDt,@EndDate,@EmpCd,@GetAllRecord",
                                  new SqlParameter("@StartDt", startDate),
                                  new SqlParameter("@EndDate", endDate),
                                  new SqlParameter("@EmpCd", empCd),
                                  new SqlParameter("@GetAllRecord", getAllrecord)
                                  ).ToList();
            return OperationalSubmission;
        }

        public IEnumerable<ClientMatrixDashboard> Get_ClientDashboardMatrics(DateTime startOfMonth, DateTime endOfMonth, string empCd, DateTime TodayDate, string role)
        {
            bool getAllrecord = false;
            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;
            IEnumerable<ClientMatrixDashboard> dashbordMatrix =
              context.Database.SqlQuery<ClientMatrixDashboard>(
                                 "[dbo].[SP_ClientDashboard] @StartDate,@EndDate,@EmpCd,@TodayDate,@GetAllrecords",
                                  new SqlParameter("@StartDate", startOfMonth),
                                  new SqlParameter("@EndDate", endOfMonth),
                                  new SqlParameter("@EmpCd", empCd),
                                  new SqlParameter("@TodayDate", TodayDate),
                                  new SqlParameter("@GetAllrecords", getAllrecord)
                                  );
            return dashbordMatrix;
        }
        public IEnumerable<ClientDashboardQuaterlyDB> Get_ClientDashboard_QuarterWise(string empCd, int Year, string role)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<ClientDashboardQuaterlyDB> ClientQuaterWise =
                context.Database.SqlQuery<ClientDashboardQuaterlyDB>(
                                   "[dbo].[SP_ClientDashboard_QuarterWise] @EmpCd,@year,@GetAllRecord",
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@year", Year),
                                    new SqlParameter("@GetAllRecord", getAllrecord)
                                    );
            return ClientQuaterWise;
        }
        public IEnumerable<PositionDashboardDB> Get_PositionDashboard_QuarterWise(string empCd, int Year, string role,string Company)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<PositionDashboardDB> ClientQuaterWise =
                context.Database.SqlQuery<PositionDashboardDB>(
                                   "[dbo].[sp_get_position_details_yearwise] @Year,@getAllrecords,@company,@Empcd",
                                    new SqlParameter("@Year", Year),
                                    new SqlParameter("@getAllrecords", getAllrecord),
                                    new SqlParameter("@company", Company),
                                    new SqlParameter("@Empcd", empCd)
                                    );
            return ClientQuaterWise;
        }
        public IEnumerable<ClientDashboardQuaterlyDB> Get_ClientDashboard_QuarterWiseNew(string empCd, int Year, string role, string Company)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;
            context.Database.CommandTimeout = 240;
            IEnumerable<ClientDashboardQuaterlyDB> ClientQuaterWise =
                context.Database.SqlQuery<ClientDashboardQuaterlyDB>(
                                   "[dbo].[SP_ClientDashboard_QuarterWise_New] @EmpCd,@year,@company,@GetAllRecord",
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@year", Year),
                                     new SqlParameter("@company", Company),
                                    new SqlParameter("@GetAllRecord", getAllrecord)
                                    );
            return ClientQuaterWise;
        }

      

        public IEnumerable<ClientDashboardQuaterlyDB> Get_ClientDashboard_QuarterWiseDetails(string empCd, string Type, string Client, DateTime Fromdate, DateTime Todate, string role)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<ClientDashboardQuaterlyDB> ClientQuaterWiseDetails =
                context.Database.SqlQuery<ClientDashboardQuaterlyDB>(
                                   "[dbo].[SP_ClientDashboard_QuarterWise_Details] @EmpCd,@Type,@Client,@Fromdate,@Todate,@GetAllRecord",
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@Type", Type),
                                    new SqlParameter("@Client", Client),
                                    new SqlParameter("@Fromdate", Fromdate),
                                    new SqlParameter("@Todate", Todate),
                                    new SqlParameter("@GetAllRecord", getAllrecord)
                                    );
            return ClientQuaterWiseDetails;
        }
        public IEnumerable<ClientDashboardQuaterlyDB> Get_ClientDashboard_QuarterWiseDetails_Portfolio(string empCd, string Type, DateTime Fromdate, DateTime Todate, string role)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<ClientDashboardQuaterlyDB> ClientQuaterWiseDetails =
                context.Database.SqlQuery<ClientDashboardQuaterlyDB>(
                                   "[dbo].[SP_ClientDashboard_QuarterWise_Details_Company] @EmpCd,@Type,@Fromdate,@Todate,@GetAllRecord",
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@Type", Type),
                                    //new SqlParameter("@Client", Client),
                                    new SqlParameter("@Fromdate", Fromdate),
                                    new SqlParameter("@Todate", Todate),
                                    new SqlParameter("@GetAllRecord", getAllrecord)
                                    );
            return ClientQuaterWiseDetails;
        }
        public IEnumerable<ClientDashboardQuaterlyDB> Get_Position_Quaterwise(string empCd, string Type, DateTime Fromdate, DateTime Todate,string Position_type, string role)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<ClientDashboardQuaterlyDB> ClientQuaterWiseDetails =
                context.Database.SqlQuery<ClientDashboardQuaterlyDB>(
                                   "[dbo].[SP_Position_Details_Quarterwise] @EmpCd,@Type,@Fromdate,@Todate,@Position_type,@GetAllRecord",
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@Type", Type),
                                   
                                    new SqlParameter("@Fromdate", Fromdate),
                                    new SqlParameter("@Todate", Todate),
                                    new SqlParameter("@Position_type", Position_type),
                                    new SqlParameter("@GetAllRecord", getAllrecord)
                                    );
            return ClientQuaterWiseDetails;
        }
        public IEnumerable<Sp_GetTeamPerformanceResult> Get_TeamPerformanceResult(DateTime fromDate, DateTime toDate, string empCd, string role, int EmpID = 0)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == AccMgrRoleName)
                getAllrecord = true;

            if (role == "Accounting Manager")
            {
                getAllrecord = false;
                IEnumerable<Sp_GetTeamPerformanceResult> teamPerformanceResult1 =
                 context.Database.SqlQuery<Sp_GetTeamPerformanceResult>(
                                    "[dbo].[Sp_GetTeamPerformance_AM] @FromDate,@ToDate,@EmpCd,@GetAllRecord,@EMP_ID",
                                     new SqlParameter("@FromDate", fromDate),
                                     new SqlParameter("@ToDate", toDate),
                                     new SqlParameter("@EmpCd", empCd),
                                     new SqlParameter("@GetAllRecord", getAllrecord),
                                     new SqlParameter("@EMP_ID", EmpID)
                                     );
                return teamPerformanceResult1;
            }
            else
            {
                IEnumerable<Sp_GetTeamPerformanceResult> teamPerformanceResult2 =
                context.Database.SqlQuery<Sp_GetTeamPerformanceResult>(
                                   "[dbo].[Sp_GetTeamPerformance] @FromDate,@ToDate,@EmpCd,@GetAllRecord",
                                    new SqlParameter("@FromDate", fromDate),
                                    new SqlParameter("@ToDate", toDate),
                                    new SqlParameter("@EmpCd", empCd),
                                    new SqlParameter("@GetAllRecord", getAllrecord)
                                    );
                return teamPerformanceResult2;
            }
            //IEnumerable<Sp_GetTeamPerformanceResult> teamPerformanceResult =
            //   context.Database.SqlQuery<Sp_GetTeamPerformanceResult>(
            //                      "[dbo].[Sp_GetTeamPerformance] @FromDate,@ToDate,@EmpCd,@GetAllRecord",
            //                       new SqlParameter("@FromDate", fromDate),
            //                       new SqlParameter("@ToDate", toDate),
            //                       new SqlParameter("@EmpCd", empCd),
            //                       new SqlParameter("@GetAllRecord", getAllrecord)
            //                       );
            // return teamPerformanceResult;

        }

        //added by suman.

        public IEnumerable<ClientDashboardMonthlyDB> Get_ClientDashboard_Monthly(string empCd, int Year, string role)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<ClientDashboardMonthlyDB> ClientMonthlyWise =
              context.Database.SqlQuery<ClientDashboardMonthlyDB>(
                                 "[dbo].[SP_ClientDashboard_Monthly] @EmpCd,@year,@GetAllRecord",
                                  new SqlParameter("@EmpCd", empCd),
                                  new SqlParameter("@year", Year),
                                  new SqlParameter("@GetAllRecord", getAllrecord)
                                  ).ToList();
            return ClientMonthlyWise;
        }

        public IEnumerable<ClientSubmissionDetailsDB> ClientSubmissionDeatils(string empCd, int Year, string role, DateTime Fromdate, DateTime Todate)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<ClientSubmissionDetailsDB> ClientMonthlyWise =
              context.Database.SqlQuery<ClientSubmissionDetailsDB>(
                                 "[dbo].[sp_SubmissionDetailsHourWise] @EmpCd,@year,@FromDate,@Todate,@GetAllRecord",
                                  new SqlParameter("@EmpCd", empCd),
                                  new SqlParameter("@year", Year),
                                   new SqlParameter("@FromDate", Fromdate),
                                  new SqlParameter("@Todate", Todate),
                                  new SqlParameter("@GetAllRecord", getAllrecord)
                                  ).ToList();
            return ClientMonthlyWise;
        }

        public IEnumerable<PositionDashboardMonthlyDB> Get_PositionDashboard_Monthly(string empCd, int Year, string role,string Company)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<PositionDashboardMonthlyDB> ClientMonthlyWise =
              context.Database.SqlQuery<PositionDashboardMonthlyDB>(
                                 "[dbo].[sp_get_position_details_monthwise] @Year,@getAllrecords,@company,@Empcd",
                                  new SqlParameter("@Year", Year),
                                  new SqlParameter("@getAllrecords", getAllrecord),
                                  new SqlParameter("@company", Company),
                                  new SqlParameter("@Empcd", empCd)
                                  ).ToList();
            return ClientMonthlyWise;
        }


        public IEnumerable<ClientDashboardMonthlyDB> Get_ClientDashboard_MonthlyNew(string empCd, int Year, string role,string Company)
        {
            bool getAllrecord = false;

            if (role == directorRoleName || role == DirectorStaffingRoleName)
                getAllrecord = true;

            IEnumerable<ClientDashboardMonthlyDB> ClientMonthlyWise =
              context.Database.SqlQuery<ClientDashboardMonthlyDB>(
                                 "[dbo].[SP_ClientDashboard_Monthly_New] @EmpCd,@year,@company,@GetAllRecord",
                                  new SqlParameter("@EmpCd", empCd),
                                  new SqlParameter("@year", Year),
                                  new SqlParameter("@company", Company),
                                  new SqlParameter("@GetAllRecord", getAllrecord)
                                  ).ToList();
            return ClientMonthlyWise;
        }

         
        //added by suman.
        //to get last four quarters data for user(submissons, interviews, hires)
        public IEnumerable<ClientDashboardQuaterlyDB> GetSubmissionAnalysisByEmpCd(DateTime ReviewDate, string empCd)
        {

            IEnumerable<ClientDashboardQuaterlyDB> GetSubmissionAnalysisQuarterWise =
              context.Database.SqlQuery<ClientDashboardQuaterlyDB>(
                                 "[dbo].[SP_QuarterWiseEmpData] @EmpCd,@ReviewDate",
                                  new SqlParameter("@EmpCd", empCd),
                                  new SqlParameter("@ReviewDate", ReviewDate)
                                  ).ToList();
            return GetSubmissionAnalysisQuarterWise;
        }

        //added by suman.
        //send a mail to employee have a discussion with hr.
        public string SendEmailHrDiscussion(string Empcd, string subject, string location, DateTime StartDate, DateTime EndDate, string Body)
        {
            context.Database.ExecuteSqlCommand("EXEC sp_SendEmail @Empcd,@Subject,@Location,@StartDate,@EndDate,@body",
                                   new SqlParameter("@Empcd", Empcd),
                                   new SqlParameter("@Subject", subject),
                                   new SqlParameter("@Location", location),
                                   new SqlParameter("@StartDate", StartDate),
                                   new SqlParameter("@EndDate", EndDate),
                                   new SqlParameter("@body", Body)
                                   );

            return null;
        }

        public IEnumerable<RMS_JobsDB> Get_ManageJobs(string empCd)
        {

            IEnumerable<RMS_JobsDB> objDataResult =
              context.Database.SqlQuery<RMS_JobsDB>(
                                 "[dbo].[Sp_Get7DaysJobs] @Emp_Cd",
                                  new SqlParameter("@Emp_Cd", empCd)
                                  );
            return objDataResult;
        }

        public IEnumerable<RMS_JobsDB> Get7daysActiveJobs(string empCd)
        {

            IEnumerable<RMS_JobsDB> objDataResult =
              context.Database.SqlQuery<RMS_JobsDB>(
                                 "[dbo].[Sp_Get7DaysActiveJobs] @Emp_Cd",
                                  new SqlParameter("@Emp_Cd", empCd)
                                  );
            return objDataResult;
        }

        //added by chikkegowda.
        public IEnumerable<ClientDashboardJobDiva> getClientDiva(DateTime StartDate, DateTime Enddate)
        {
            IEnumerable<ClientDashboardJobDiva> ClientJobDiva =
            context.Database.SqlQuery<ClientDashboardJobDiva>(
                               "[dbo].[SP_ClientDashboardJobDiva] @STARTDATE,@ENDDATE",
                                new SqlParameter("@STARTDATE", StartDate),
                                new SqlParameter("@ENDDATE", Enddate)

                                ).ToList();
            return ClientJobDiva;

        }


    }

}
