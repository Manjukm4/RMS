using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBLibrary;
using DBLibrary.UnitOfWork;
using OfficeOpenXml;
using RIC.Models;
using RIC.Utility;
using PagedList;
using System.Data.SqlClient;
using System.Diagnostics;
using RIC.Models.Dashboard;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using DBLibrary.Repository;
namespace RIC.Controllers
{

    public class DashboardWeekelyDataController : Controller

    {

        UnitOfWork unitOfwork = new UnitOfWork();
        [Authorize]

        // public ActionResult DashboardData(SearchEmployeeData employeeData)
        public ActionResult DashboardData(string empId)

        {
            if (empId.Equals("Directors"))
            {
                empId = "SBS0229";
            }
            int month = DateTime.Now.Month;
            DateTime date = SystemClock.US_Date.AddMonths(month); DateTime weekstdate; DateTime weekenddate;
            DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            //get the user details.
            var _user = unitOfwork.User.GetByEmpID(empId);
            ViewBag.EmpCd = empId;
            // get the joining date.
            DateTime? joiningDate = _user.RE_Joining_Date;
            if (joiningDate != null && joiningDate >= endOfMonth.AddMonths(-1))
                ViewBag.ShowPreviousBtn = "N"; // hide the show back button.
            CultureInfo culture = new CultureInfo("en-US");
            DateTime startDate = Convert.ToDateTime(DateTime.Today, culture);
            ViewBag.date = startDate;
            //if (empCd == null || empCd == "") //get the employee code from authentication.

            ViewBag.WeekNumber = 0;//new DateTime(2014, 1, 6)//DateTime.Now.Date

            // ViewBag.WeekNumber = ViewBag.WeekNumber + 1;
            WeeklyDataReport weekreport = new WeeklyDataReport();
            var emplist = weekreport.Get_EmployeePerformanceReport(startDate, empId);
            // DateTime dt = new DateTime(2022, 7, 5);
            // var br = weekreport.Get_EmployeeBreakDetails(dt, empId);
            var empdata = emplist.Where(x => x.EmpCd == empId).ToList();
            ViewData["Emp"] = empdata;

            DateTime prweek = DateTime.Now.AddDays(-0);
            while (prweek.DayOfWeek != DayOfWeek.Monday)
            {
                prweek = prweek.AddDays(-1);
            }

            DateTime prweekm = prweek;
            DateTime prweekf = prweek.AddDays(4);

            //var stdate = emplist.Select(x => x.Login_Time).FirstOrDefault();
            //var enddate = emplist.Select(x => x.Login_Time).LastOrDefault();
            //if (stdate == null && enddate == null)
            //{
            weekstdate = prweekm;
            weekenddate = prweekf;
            //}
            //else
            //{
            //    weekstdate = (DateTime)stdate;
            //    weekenddate = (DateTime)enddate;
            //}

            ViewBag.weekstdate = weekstdate.ToShortDateString();
            ViewBag.weekenddate = weekenddate.ToShortDateString();
            return View("DashBoarddata");
            //return View("DashBoarddata", new SearchEmployeeData());

        }
        public ActionResult DashboardData1(int week, string empId)
        {
            if (empId.Equals("Directors"))
            {
                empId = "SBS0229";
            }
            int month = DateTime.Now.Month;
            DateTime date = SystemClock.US_Date.AddMonths(month);
            DateTime weekstdate = DateTime.Now.AddDays(-0);
            DateTime weekenddate = DateTime.Now.AddDays(4);
            DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            //get the user details.
            var _user = unitOfwork.User.GetByEmpID(empId);
            ViewBag.EmpCd = empId;
            // get the joining date.
            DateTime? joiningDate = _user.RE_Joining_Date;
            if (joiningDate != null && joiningDate >= endOfMonth.AddMonths(-1))
                ViewBag.ShowPreviousBtn = "N"; // hide the show back button.
            CultureInfo culture = new CultureInfo("en-US");
            DateTime startDate = Convert.ToDateTime(DateTime.Today, culture);
            var previousWeekDates = unitOfwork.User.getPreviousWeekDate().ToList();
            if (week == -1)
            {
                startDate = DateTime.Now.AddDays(-7);
                // startDate = new DateTime(2022, 7, 5);
                weekstdate = previousWeekDates.Select(x => x.Start_week1).FirstOrDefault();
                weekenddate = previousWeekDates.Select(x => x.end_week1).FirstOrDefault();
            }
            else if (week == -2)
            {
                startDate = DateTime.Now.AddDays(-14);

                weekstdate = previousWeekDates.Select(x => x.Start_week2).FirstOrDefault();
                weekenddate = previousWeekDates.Select(x => x.end_week2).FirstOrDefault();
            }
            else if (week == -3)
            {
                startDate = DateTime.Now.AddDays(-21);

                weekstdate = previousWeekDates.Select(x => x.Start_week3).FirstOrDefault();
                weekenddate = previousWeekDates.Select(x => x.end_week3).FirstOrDefault();
            }
            else if (week == 0)
            {
                startDate = DateTime.Now.AddDays(-0);

                weekstdate = previousWeekDates.Select(x => x.Cur_wk_st).FirstOrDefault();
                weekenddate = previousWeekDates.Select(x => x.Cur_wk_end).FirstOrDefault();
            }
            ViewBag.date = startDate;
            ViewBag.WeekNumber = week;//new DateTime(2022, 3, 24)//DateTime.Now.Date
                                      // Hidweek
            WeeklyDataReport weekreport = new WeeklyDataReport();
            var emplist = weekreport.Get_EmployeePerformanceReport(startDate, empId);//startDate
            var empdata = emplist.Where(x => x.EmpCd == empId).ToList();
            ViewData["Emp"] = empdata;

            ViewBag.weekstdate = weekstdate.ToShortDateString();
            ViewBag.weekenddate = weekenddate.ToShortDateString();
            // return View("DashBoarddata");

            return PartialView("DashboardDataPartial");
        }
        //DateTime prweek = DateTime.Now.AddDays(-7);
        //while (prweek.DayOfWeek != DayOfWeek.Monday)
        //{
        //    prweek = prweek.AddDays(-1);
        //}

        //DateTime prweekm = startDate;
        //DateTime prweekf = startDate.AddDays(4);
        //var stdate = emplist.Select(x => x.Login_Time ).FirstOrDefault();
        //var enddate = emplist.Select(x => x.Login_Time).LastOrDefault();
        //if (stdate != null && enddate != null)
        //{


        //    weekstdate = (DateTime)stdate;
        //    weekenddate = (DateTime)enddate;
        //}
        //else
        //{
        //    weekstdate = prweekm;
        //    weekenddate = prweekf;
        //}
        //
        //weekstdate=DateTime.Now.AddDays(-14);
        //while (weekstdate.DayOfWeek != DayOfWeek.Monday)
        //{
        //    weekstdate = weekstdate.AddDays(-1);
        //}
        //weekenddate = weekstdate.AddDays(4);

        public ActionResult BreaksDetails(string empCD, DateTime getDate, int addDays)
        {

            // var _user = unitOfwork.User.GetByEmpID(empCD);
            // ViewBag.WeekNumber = ViewBag.WeekNumber + 1;
            ViewBag.EmpCd = empCD;
            getDate = getDate.AddDays(addDays);
            WeeklyDataReport weekreport = new WeeklyDataReport();
            var br = weekreport.Get_EmployeeBreakDetails(getDate, empCD);
            ViewData["Emp"] = br;
            return View();
        }




    }

}




