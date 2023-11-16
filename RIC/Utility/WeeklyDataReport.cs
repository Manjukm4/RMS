using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data;
using DBLibrary;
using DBLibrary.UnitOfWork;
using RIC.Models;
namespace DBLibrary.Repository

{
    public class WeeklyDataReport
        {

        UnitOfWork.UnitOfWork unitOfWork = new UnitOfWork.UnitOfWork();
        //SP_Employees_Performance_Report
        public IEnumerable<DashboardWeekelyData> Get_EmployeePerformanceReport(DateTime StartDate,string EmpCd)
        {
            unitOfWork.context.Database.CommandTimeout=1000;

            var WeeklyData = unitOfWork.context.Database.SqlQuery<DashboardWeekelyData>
                (
                                  "[dbo].[SP_Employees_Performance_Report] @StartDate,@EmpCd",
                                  new SqlParameter("StartDate", StartDate),
                                   new SqlParameter("EmpCd", EmpCd)
                                   ).ToList();

            
                        return WeeklyData;
        }

        public IEnumerable<Sp_Break_details> Get_EmployeeBreakDetails(DateTime StartDate, string EmpCd)
        {
            unitOfWork.context.Database.CommandTimeout = 1000;

            var WeeklyData = unitOfWork.context.Database.SqlQuery<Sp_Break_details>
                (
                                  "[dbo].[Sp_Get_Breaks_Details] @EmpCd,@date",
                                  new SqlParameter("date", StartDate),
                                   new SqlParameter("EmpCd", EmpCd)
                                   ).ToList();


            return WeeklyData;
        }

    }
}