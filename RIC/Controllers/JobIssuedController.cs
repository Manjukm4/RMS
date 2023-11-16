using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBLibrary;
using DBLibrary.UnitOfWork;
using RIC.Models.JobIssued;
using System.Data;
using System.Globalization;

namespace RIC.Controllers
{
    //Added by Shaurya..
    [Authorize]
    public class JobIssuedController : Controller
    {
        //
        // GET: /JobIssue/
        UnitOfWork unitOfwork = new UnitOfWork();

        public ActionResult Index()
        {
            JobIssuedVM JobIssuedVM = new JobIssuedVM();
            JobIssuedVM.Year = DateTime.Now.Year;
            JobIssuedVM.Month = DateTime.Now.Month;

            JobIssuedVM.ClientList = new SelectList(unitOfwork.RIC_Job_Report.Get_RMS_JDIssuedDt().
                                                    Where(D => D.RM_DateIssued.Value.Year == JobIssuedVM.Year).
                                                    OrderBy(s => s.RM_Company).
                                                    GroupBy(C => C.RM_Company, StringComparer.InvariantCultureIgnoreCase).
                                                    Select(s => new { text = s.Key, value = s.Key }), "text", "value").ToList();
            JobIssuedVM.ClientList.ElementAt(0).Selected = true;

            JobIssuedVM.YearList = new SelectList(unitOfwork.RIC_Job_Report.Get_RMS_JDIssuedDt().
                                                   GroupBy(D => D.RM_DateIssued.Value.Year).
                                                   Select(s => new { text = s.Key, value = s.Key }), "value", "text").ToList();
            JobIssuedVM.YearList.ElementAt(0).Selected = true;

            JobIssuedVM.MonthList = new SelectList(DateTimeFormatInfo.CurrentInfo.MonthNames.Select((r, index) => new SelectListItem { Text = r, Value = (index + 1).ToString() }), "value", "text").ToList();
            JobIssuedVM.MonthList.ElementAt(JobIssuedVM.Month).Selected = true;

            var JobIssued = (from rms_JDIssuedDt in unitOfwork.RIC_Job_Report.Get_RMS_JDIssuedDt()
                             where rms_JDIssuedDt.RM_Company == JobIssuedVM.YearList.ElementAt(0).Text
                             && rms_JDIssuedDt.RM_DateIssued.Value.Year == JobIssuedVM.Year
                             && rms_JDIssuedDt.RM_DateIssued.Value.Month == JobIssuedVM.Month
                             group rms_JDIssuedDt by new { rms_JDIssuedDt.RM_DateIssued, rms_JDIssuedDt.RM_JobDivaRef } into g
                             select new
                             {
                                 RM_DateIssued = g.Key.RM_DateIssued.Value,
                                 RM_JobDivaRef = g.Key.RM_JobDivaRef
                             }).ToList();

            var JDIssued = (from x in JobIssued
                            orderby x.RM_DateIssued
                            group x by new { x.RM_DateIssued.Day, x.RM_DateIssued.Hour } into g
                            select new
                             {
                                 Day = g.Key.Day,
                                 Hour = g.Key.Hour,
                                 Count = g.Count()
                             }).ToList();

            DataTable dtVM = new DataTable();
            dtVM.Columns.Add("Date", typeof(string));
            dtVM.Columns.Add("6-8AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("8-9AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("9-10AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("10-11AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("11-12PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("12-1PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("1-2PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("2-3PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("3-4PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("4-5PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("5-6PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("6-7PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("7-8PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("8-9PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("9-10PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("10-11PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("11-12AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("12-6AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("Total", typeof(int)).DefaultValue = 0;

            DataRow dr;

            int days = DateTime.DaysInMonth(JobIssuedVM.Year, JobIssuedVM.Month);
            for (int i = 1; i <= days; i++)
            {
                if (((i <= DateTime.Now.Day)))
                {//Display only data that is less then current date.
                    dr = dtVM.NewRow();
                    dr[0] = i + " - " + new DateTime(2000, JobIssuedVM.Month, 1).ToString("MMM");

                    foreach (var item in JDIssued.Where(x => x.Day == i))
                    {

                        if (item.Hour <= 5)
                        {
                            dr[18] = (int)dr[18] + item.Count;
                        }
                        else if (item.Hour <= 7)
                        {
                            dr[1] = (int)dr[1] + item.Count;
                        }
                        else
                        {
                            switch (item.Hour)
                            {
                                case 8:
                                    dr[2] = item.Count;
                                    break;

                                case 9:
                                    dr[3] = item.Count;
                                    break;

                                case 10:
                                    dr[4] = item.Count;
                                    break;
                                case 11:
                                    dr[5] = item.Count;
                                    break;
                                case 12:
                                    dr[6] = item.Count;
                                    break;
                                case 13:
                                    dr[7] = item.Count;
                                    break;
                                case 14:
                                    dr[8] = item.Count;
                                    break;
                                case 15:
                                    dr[9] = item.Count;
                                    break;
                                case 16:
                                    dr[10] = item.Count;
                                    break;
                                case 17:
                                    dr[11] = item.Count;
                                    break;
                                case 18:
                                    dr[12] = item.Count;
                                    break;
                                case 19:
                                    dr[13] = item.Count;
                                    break;
                                case 20:
                                    dr[14] = item.Count;
                                    break;
                                case 21:
                                    dr[15] = item.Count;
                                    break;
                                case 22:
                                    dr[16] = item.Count;
                                    break;
                                case 23:
                                    dr[17] = item.Count;
                                    break;
                            }
                        }

                        dr[19] = (int)dr[19] + item.Count;
                    }
                    dtVM.Rows.Add(dr);
                }
            }
            if (dtVM.Rows.Count > 1)
            {
                dr = dtVM.NewRow();
                dr[0] = "Totals :";
                dr[1] = dtVM.Compute("sum([6-8AM])", string.Empty);
                dr[2] = dtVM.Compute("sum([8-9AM])", string.Empty);
                dr[3] = dtVM.Compute("sum([9-10AM])", string.Empty);
                dr[4] = dtVM.Compute("sum([10-11AM])", string.Empty);
                dr[5] = dtVM.Compute("sum([11-12PM])", string.Empty);
                dr[6] = dtVM.Compute("sum([12-1PM])", string.Empty);
                dr[7] = dtVM.Compute("sum([1-2PM])", string.Empty);
                dr[8] = dtVM.Compute("sum([2-3PM])", string.Empty);
                dr[9] = dtVM.Compute("sum([3-4PM])", string.Empty);
                dr[10] = dtVM.Compute("sum([4-5PM])", string.Empty);
                dr[11] = dtVM.Compute("sum([5-6PM])", string.Empty);
                dr[12] = dtVM.Compute("sum([6-7PM])", string.Empty);
                dr[13] = dtVM.Compute("sum([7-8PM])", string.Empty);
                dr[14] = dtVM.Compute("sum([8-9PM])", string.Empty);
                dr[15] = dtVM.Compute("sum([9-10PM])", string.Empty);
                dr[16] = dtVM.Compute("sum([10-11PM])", string.Empty);
                dr[17] = dtVM.Compute("sum([11-12PM])", string.Empty);
                dr[18] = dtVM.Compute("sum([12-6AM])", string.Empty);
                dr[19] = dtVM.Compute("sum([Total])", string.Empty);
                dtVM.Rows.Add(dr);
            }

            JobIssuedVM.JDIssued = dtVM;
            return View(JobIssuedVM);
        }

        [HttpPost]
        public ActionResult Index(JobIssuedVM JobIssuedVM)
        {
            JobIssuedVM.ClientList = new SelectList(unitOfwork.RIC_Job_Report.Get_RMS_JDIssuedDt().
                                        Where(D => D.RM_DateIssued.Value.Year == JobIssuedVM.Year).
                                        OrderBy(s => s.RM_Company).
                                        GroupBy(C => C.RM_Company, StringComparer.InvariantCultureIgnoreCase).
                                        Select(s => new { text = s.Key, value = s.Key }), "text", "value").ToList();
            JobIssuedVM.ClientList.FirstOrDefault(e => e.Text == JobIssuedVM.Client).Selected = true;

            JobIssuedVM.YearList = new SelectList(unitOfwork.RIC_Job_Report.Get_RMS_JDIssuedDt().
                                                   GroupBy(D => D.RM_DateIssued.Value.Year).
                                                   Select(s => new { text = s.Key, value = (int)s.Key }), "value", "text").ToList();
            JobIssuedVM.YearList.FirstOrDefault(e => e.Text == JobIssuedVM.Year.ToString()).Selected = true;

            JobIssuedVM.MonthList = new SelectList(DateTimeFormatInfo.CurrentInfo.MonthNames.Select((r, index) => new SelectListItem { Text = r, Value = (index + 1).ToString() }), "value", "text").ToList();
            JobIssuedVM.MonthList.ElementAt(JobIssuedVM.Month).Selected = true;

            var JobIssued = (from rms_JDIssuedDt in unitOfwork.RIC_Job_Report.Get_RMS_JDIssuedDt()
                             where rms_JDIssuedDt.RM_Company == JobIssuedVM.Client
                             && rms_JDIssuedDt.RM_DateIssued.Value.Year == JobIssuedVM.Year
                             && rms_JDIssuedDt.RM_DateIssued.Value.Month == JobIssuedVM.Month
                             group rms_JDIssuedDt by new { rms_JDIssuedDt.RM_DateIssued, rms_JDIssuedDt.RM_JobDivaRef } into g
                             select new
                             {
                                 RM_DateIssued = g.Key.RM_DateIssued.Value,
                                 RM_JobDivaRef = g.Key.RM_JobDivaRef
                             }).ToList();

            var JDIssued = (from x in JobIssued
                            orderby x.RM_DateIssued
                            group x by new { x.RM_DateIssued.Day, x.RM_DateIssued.Hour } into g
                            select new
                            {
                                Day = g.Key.Day,
                                Hour = g.Key.Hour,
                                Count = g.Count()
                            }).ToList();

            DataTable dtVM = new DataTable();
            dtVM.Columns.Add("Date", typeof(string));
            dtVM.Columns.Add("6-8AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("8-9AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("9-10AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("10-11AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("11-12PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("12-1PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("1-2PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("2-3PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("3-4PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("4-5PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("5-6PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("6-7PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("7-8PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("8-9PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("9-10PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("10-11PM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("11-12AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("12-6AM", typeof(int)).DefaultValue = 0;
            dtVM.Columns.Add("Total", typeof(int)).DefaultValue = 0;

            DataRow dr;

            int days = DateTime.DaysInMonth(JobIssuedVM.Year, JobIssuedVM.Month);
            DateTime CurrentDate = DateTime.Now;
            for (int i = 1; i <= days; i++)
            {
                //(!(JobIssuedVM.Year > DateTime.Now.Year ))&&(!(JobIssuedVM.Month > DateTime.Now.Month)) && (!(i > DateTime.Now.Day))
                DateTime Date = new DateTime(JobIssuedVM.Year, JobIssuedVM.Month, i);
                if (Date <= CurrentDate)
                {// Display only data that is less then current date.
                    dr = dtVM.NewRow();
                    dr[0] = i + " - " + new DateTime(2000, JobIssuedVM.Month, 1).ToString("MMM");

                    foreach (var item in JDIssued.Where(x => x.Day == i))
                    {

                        if (item.Hour <= 5)
                        {
                            dr[18] = (int)dr[18] + item.Count;
                        }
                        else if (item.Hour <= 7)
                        {
                            dr[1] = (int)dr[1] + item.Count;
                        }
                        else
                        {
                            switch (item.Hour)
                            {
                                case 8:
                                    dr[2] = item.Count;
                                    break;

                                case 9:
                                    dr[3] = item.Count;
                                    break;

                                case 10:
                                    dr[4] = item.Count;
                                    break;
                                case 11:
                                    dr[5] = item.Count;
                                    break;
                                case 12:
                                    dr[6] = item.Count;
                                    break;
                                case 13:
                                    dr[7] = item.Count;
                                    break;
                                case 14:
                                    dr[8] = item.Count;
                                    break;
                                case 15:
                                    dr[9] = item.Count;
                                    break;
                                case 16:
                                    dr[10] = item.Count;
                                    break;
                                case 17:
                                    dr[11] = item.Count;
                                    break;
                                case 18:
                                    dr[12] = item.Count;
                                    break;
                                case 19:
                                    dr[13] = item.Count;
                                    break;
                                case 20:
                                    dr[14] = item.Count;
                                    break;
                                case 21:
                                    dr[15] = item.Count;
                                    break;
                                case 22:
                                    dr[16] = item.Count;
                                    break;
                                case 23:
                                    dr[17] = item.Count;
                                    break;
                            }
                        }

                        dr[19] = (int)dr[19] + item.Count;
                    }
                    dtVM.Rows.Add(dr);
                }
            }
            if (dtVM.Rows.Count > 1)
            {
                dr = dtVM.NewRow();
                dr[0] = "Totals :";
                dr[1] = dtVM.Compute("sum([6-8AM])", string.Empty);
                dr[2] = dtVM.Compute("sum([8-9AM])", string.Empty);
                dr[3] = dtVM.Compute("sum([9-10AM])", string.Empty);
                dr[4] = dtVM.Compute("sum([10-11AM])", string.Empty);
                dr[5] = dtVM.Compute("sum([11-12PM])", string.Empty);
                dr[6] = dtVM.Compute("sum([12-1PM])", string.Empty);
                dr[7] = dtVM.Compute("sum([1-2PM])", string.Empty);
                dr[8] = dtVM.Compute("sum([2-3PM])", string.Empty);
                dr[9] = dtVM.Compute("sum([3-4PM])", string.Empty);
                dr[10] = dtVM.Compute("sum([4-5PM])", string.Empty);
                dr[11] = dtVM.Compute("sum([5-6PM])", string.Empty);
                dr[12] = dtVM.Compute("sum([6-7PM])", string.Empty);
                dr[13] = dtVM.Compute("sum([7-8PM])", string.Empty);
                dr[14] = dtVM.Compute("sum([8-9PM])", string.Empty);
                dr[15] = dtVM.Compute("sum([9-10PM])", string.Empty);
                dr[16] = dtVM.Compute("sum([10-11PM])", string.Empty);
                dr[17] = dtVM.Compute("sum([11-12PM])", string.Empty);
                dr[18] = dtVM.Compute("sum([12-6AM])", string.Empty);
                dr[19] = dtVM.Compute("sum([Total])", string.Empty);
                dtVM.Rows.Add(dr);
            }

            JobIssuedVM.JDIssued = dtVM;
            return View(JobIssuedVM);
        }
    }
}
