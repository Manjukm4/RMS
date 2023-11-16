
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBLibrary;
using DBLibrary.UnitOfWork;
using System.Security.Cryptography;
using RIC.Utility;
using PagedList;
using System.Web.UI.WebControls;
using RIC.Models;
using System.Data;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Newtonsoft.Json;
using System.Globalization;
using RIC.Models.MonthlyTarget;
using RIC.Models.Client;

namespace RIC.Controllers
{
    public class MonthlyTargetController : Controller
    {

        // GET: /MonthlyTarget/
        int adminRoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AdminRoleID"]);
        string AdminRoleName = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
        string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        string HrRoleName = System.Configuration.ConfigurationManager.AppSettings["HRRole"];
        string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
        string EmployeeRoleName = System.Configuration.ConfigurationManager.AppSettings["EmployeeRole"];
        string DevLeadRole = System.Configuration.ConfigurationManager.AppSettings["DEV_LeadRole"];
        string mgrRoleName = System.Configuration.ConfigurationManager.AppSettings["ManagerRole"];
        string tlRoleName = System.Configuration.ConfigurationManager.AppSettings["TLRole"];
        string stafingDirectorRole = System.Configuration.ConfigurationManager.AppSettings["StaffingDirector"];

        DataTable dtYer = new DataTable(); int ddlyear, yearcount;
        List<SP_EmployeeYearlyTarget> Eytlist = new List<SP_EmployeeYearlyTarget>();
        private UnitOfWork unitOfWork = new UnitOfWork();
        [Authorize(Roles = "Admin,Manager,Team Lead, Director,Director-Staffing, Employee")]
        //[ActionName("AddMonthlyTarget")]
        public ActionResult Index()
        {
            string empCd = User.Identity.Name;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            DateTime usDate = SystemClock.US_Date;
            var model = new MonthlyTargetsVM();

            SetDropdownYear();
            //get the year list.
            model.YearList = Enumerable.Range(ddlyear, yearcount).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Text).ToList();
            //get the month list.
            model.MonthList = Enumerable.Range(1, 12)
            //model.MonthList = new List<SelectListItem>()

                              .Select(i => new SelectListItem
                              {
                                  Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i),
                                  Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                              }).ToList();


            model.Year = usDate.Year;
            model.Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month);

            var employeename = unitOfWork.User.GetEmployeeName(empCd);

            var targets = unitOfWork.User.getemployeemonthlytarget(empCd, model.Month, model.Year);

            //var v=unitOfWork.User.getemployeeYearlytarget(empCd, model.Year);
            List<MonthlyTargetsEmployee> lst = FillEmployeeData(empCd, model.Month, model.Year, usDate, employeename, targets);

            model.EmployeeList = lst;
            //var lst = new List<MonthlyTargetsEmployee>();
            //model.EmployeeList = lst;

            return View(model);

        }
        public ActionResult ExportToExcel(int RequestedDate)
        {
            //ViewBag.Firstname = Request.QueryString["FirstName"];
            //DateTime RequestedDate = DateTime.Parse(HiddenDate);

            //DateTime endDate = RequestedDate.AddDays(1).AddTicks(-1);

            //ViewBag.date = RequestedDate;
            //ViewBag.toDate = RequestedDate.AddDays(1);
            string sel = null;
            string empCd = User.Identity.Name;

            var _user = unitOfWork.User.GetByEmpID(empCd);//get the role for user.
            string role = _user.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name;
            if (role == "Director" || role == "Director-Staffing")
            {
                sel = "All";
            }
            else
            {
                sel = role;
            }

            List<SP_EmployeeYearlyTarget> res = unitOfWork.User.getemployeeYearlytarget(empCd, RequestedDate, sel).Select(s => new SP_EmployeeYearlyTarget
            {

                Emp_Name = s.Emp_Name,
                Emp_Cd = s.Emp_Cd,
                Manager_name = s.Manager_name,
                Mgr_Cd = s.Mgr_Cd,
                Month = s.Month,
                Year = s.Year,
                SubmissionTarget = s.SubmissionTarget,
                HireTarget = s.HireTarget,
                InterviewTarget = s.InterviewTarget,
                Supervisor_Comments = s.Supervisor_Comments,
                Team_Comments = s.Team_Comments

            }).OrderBy(e => e.Month).ToList();

            dtYer.Columns.Add("EmployeeName", typeof(string));
            dtYer.Columns.Add("EmployeeCode", typeof(string));
            dtYer.Columns.Add("ManagerName", typeof(string));
            dtYer.Columns.Add("ManagerCode", typeof(string));
            dtYer.Columns.Add("Month", typeof(string));
            dtYer.Columns.Add("Year", typeof(int)).DefaultValue = 0;
            dtYer.Columns.Add("SubmissionTarget", typeof(int)).DefaultValue = 0;
            dtYer.Columns.Add("InterviewTarget", typeof(int)).DefaultValue = 0;
            dtYer.Columns.Add("HireTarget", typeof(int)).DefaultValue = 0;
            dtYer.Columns.Add("SupervisorComment", typeof(string));
            dtYer.Columns.Add("TeamComments", typeof(string));

            // dtYer.Columns.Add("EmployeeName", typeof(string));
            Eytlist = res;
            if (res != null)
            {
                if (res.Count() > 0)
                {
                    OperationalList(res, "itemid");
                }
                else
                {
                    // OperationalList(res.Where(s => s. == 0).ToList(), "itemid");
                }
            }

            using (ExcelPackage pck = new ExcelPackage())
            {
                DataTable dt = dtYer;
                DataView view = new DataView(dt);
                view.Sort = "Month ASC";
                dt = view.ToTable();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("EmployeeTargetReport");
                ws.Cells["A1"].LoadFromDataTable(dt, true); //You can Use TableStyles property of your desire.    
                //Read the Excel file in a byte array    
                Byte[] fileBytes = pck.GetAsByteArray();

                var response = new FileContentResult(fileBytes, "application/octet-stream");
                response.FileDownloadName = "EmployeeTarget.xlsx";// set the file name
                return response;// download file.
            }
        }

        private void OperationalList(List<SP_EmployeeYearlyTarget> res, string EmpCD = null)
        {

            DataRow dr;


            foreach (var item in res)
            {
                dr = dtYer.NewRow();
                //var dataitem = res.Where(s => s.Mgr_Cd == item.Emp_Cd).ToList();

                //if (dataitem.Count() > 0)
                //{
                //    dr[0] = item.Emp_Name;
                //    dr[1] = item.Emp_Cd;
                //    dr[2] = item.Manager_name;
                //    dr[3] = item.Mgr_Cd;
                //    dr[4] = item.Month;
                //    dr[5] = item.Year;
                //    dr[6] = item.SubmissionTarget;
                //    dr[7] = item.InterviewTarget;
                //    dr[8] = item.HireTarget;
                //    dr[9] = item.Supervisor_Comments;
                //    dr[10] = item.Team_Comments;

                //    //dr[4] = item.Month;
                //    //dr[5] = item.Year;
                //    dtYer.Rows.Add(dr);
                //    OperationalList(dataitem, item.Emp_Cd);
                //}
                //else
                //{
                dr[0] = item.Emp_Name;
                dr[1] = item.Emp_Cd;
                dr[2] = item.Manager_name;
                dr[3] = item.Mgr_Cd;
                dr[4] = item.Month;
                dr[5] = item.Year;
                dr[6] = item.SubmissionTarget;
                dr[7] = item.InterviewTarget;
                dr[8] = item.HireTarget;
                dr[9] = item.Supervisor_Comments;
                dr[10] = item.Team_Comments;

                //dr[9] = item.Month;

                dtYer.Rows.Add(dr);
                //}

            }


        }

        public ActionResult SupervisorIndex()
        {
            string empCd = User.Identity.Name;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            DateTime usDate = SystemClock.US_Date;
            var model = new MonthlyTargetsVM();
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName, EmployeeRoleName };
            //get the reporting history for user.
            var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role, "REC")
                                   .Select(s => s.RR_EmpCD).ToList();
            reportingHistory.Add(empCd);
            //get the user list for tl.

            model.ReportingList = unitOfWork.User.getAllUsers()
                     .Where(s => roleList.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name)
                           && reportingHistory.Contains(s.RE_Emp_Cd)).Where(s => s.RE_Resign_Date == null)
                    .Select(s => new SelectListItem
                    {
                        Text = s.RE_Jobdiva_User_Name
                        ,
                        Value = s.RE_Emp_Cd
                    }).OrderBy(o => o.Text).ToList();

            SetDropdownYear();
            //get the year list.
            model.YearList = Enumerable.Range(ddlyear, yearcount).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Text).ToList();
            //get the month list.
            model.MonthList = Enumerable.Range(1, 12)
            //model.MonthList = new List<SelectListItem>()

                              .Select(i => new SelectListItem
                              {
                                  Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i),
                                  Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                              }).ToList();


            model.Year = usDate.Year;
            model.Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month);

            FillSupervisorData(model, empCd, model.Month, model.Year);
            var employeename = unitOfWork.User.GetEmployeeName(empCd);

            var empSupervisor = GetSupervisorDetail(empCd, model.Month, model.Year);

            model.EmpSupervisor = empSupervisor;

            //var lst = new List<MonthlyTargetsEmployee>();
            //model.EmployeeList = lst;
            return View("AddTargetsSupervisor", model);

        }

        public ActionResult SupervisorLevel2Index()
        {
            string empCd = User.Identity.Name;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            DateTime usDate = SystemClock.US_Date;
            var model = new MonthlyTargetEmpSupervisorLevel2VM();
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName, EmployeeRoleName };
            //get the reporting history for user.
            var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role, "REC")
                                   .Select(s => s.RR_EmpCD).ToList();
            reportingHistory.Add(empCd);
            //get the user list for tl.
            model.ReportingList = unitOfWork.User.getAllUsers()
                     .Where(s => roleList.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name)
                           && reportingHistory.Contains(s.RE_Emp_Cd)).Where(s => s.RE_Resign_Date == null)
                    .Select(s => new SelectListItem
                    {
                        Text = s.RE_Jobdiva_User_Name
                        ,
                        Value = s.RE_Emp_Cd
                    }).OrderBy(o => o.Text).ToList();
            //get the year list.
            SetDropdownYearSupervisor();
            model.YearList = Enumerable.Range(ddlyear, yearcount).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Text).ToList();
            //get the month list.
            model.MonthList = Enumerable.Range(1, 12)
            //model.MonthList = new List<SelectListItem>()

                              .Select(i => new SelectListItem
                              {
                                  Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i),
                                  Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                              }).ToList();


            model.Year = usDate.Year;
            model.Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month);
            FillSupervisorLevel2Data(model, empCd, model.Month, model.Year);
            var employeename = unitOfWork.User.GetEmployeeName(empCd);

            //var lst = new List<MonthlyTargetsEmployeeSupervisor>();
            //model.EmpSupervisorList = lst;
            return View("AddSupervisorLevel2Targets", model);

        }

        [HttpPost]
        public ActionResult EmployeeSearch(MonthlyTargetsVM model, string submit)
        {
            ModelState.Clear();
            string empCd = User.Identity.Name;
            string month = model.Month;
            int year = model.Year;
            ViewBag.EmpCd = empCd;
            TempData["EmpCd"] = empCd;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            //get the date.
            DateTime usDate = SystemClock.US_Date;
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName, EmployeeRoleName };

            //get the year list.
            SetDropdownYear();
            model.YearList = Enumerable.Range(ddlyear, yearcount).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Text).ToList();
            //get the month list.
            model.MonthList = Enumerable.Range(1, 12)
                              .Select(i => new SelectListItem
                              {
                                  Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i),
                                  Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                              }).ToList();
            //get the reporting history for user.
            //var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role, "REC")
            //                      .Select(s => s.RR_EmpCD).ToList();

            //reportingHistory.Add(empCd);
            var employeename = unitOfWork.User.GetEmployeeName((string)TempData["EmpCd"]);

            var targets = unitOfWork.User.getemployeemonthlytarget(empCd, month, year);

            List<MonthlyTargetsEmployee> lst = FillEmployeeData(empCd, month, year, usDate, employeename, targets);


            model.EmployeeList = lst;
            //var currentdatetime = DateTime.Now.Date;
            //string currentmonth = currentdatetime.ToString("MMMM");

            //var currentyear = DateTime.Now.Year;

            //if (currentmonth==month && currentyear == year)
            //{
            //   model.IsDisable = false;
            //}
            //else
            //{
            //    model.IsDisable = true;
            //}
            var inputmonth = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;
            var currentmonth = DateTime.Now.Date.Month;
            //var futuredatetime = DateTime.Now.Date.AddDays(15).Month;
            //int monthdays = int.Parse(currentmonth.ToString("dd"));
            var currentyear = DateTime.Now.Year;
            //string currentmonth = currentdatetime.ToString("MMMM");
            //int monthcount = 

            if (inputmonth == currentmonth && currentyear == year)
            {
                model.IsDisable = false;
            }
            /*this logic applies to 1 month in future including year*/
            else if (inputmonth > (currentmonth == 12 ? 0 : currentmonth) &&
                (inputmonth - (currentyear <= year ? 0 : currentmonth) == 1)
                && DateTime.Now.Date.Day > 15 && DateTime.Now.Date.Day <= 31 && currentyear <= year)
            {
                model.IsDisable = false;
            }
            //if (monthdays > 15 && currentyear == year)
            //{
            //    model.IsDisable = false;
            //}
            else
            {
                model.IsDisable = true;
            }

            return View("Index", model);




        }

        private static List<MonthlyTargetsEmployee> FillEmployeeData(string empCd, string month, int year, DateTime usDate, string employeename, IEnumerable<RIC_MonthlyTarget> targets)
        {
            var lst = new List<MonthlyTargetsEmployee>();
            foreach (var item in targets)
            {

                MonthlyTargetsEmployee obj = new MonthlyTargetsEmployee();
                obj.Emp_Cd = item.Emp_Cd;
                obj.Emp_Name = item.Emp_Name;
                obj.Margin = item.Margin.ToString();
                obj.SubmissionTarget = item.SubmissionTarget.ToString();
                obj.InterviewTarget = item.InterviewTarget.ToString();
                obj.HireTarget = item.HireTarget.ToString();
                obj.Team_Comments = item.Team_Comments;
                obj.Supervisor_Comments = item.Supervisor_Comments;
                obj.Status = item.Status;
                obj.Month = item.Month;
                obj.Year = item.Year;
                obj.Created_By = item.Created_By;
                obj.Created_Date = item.Created_Date;
                lst.Add(obj);
            }

            if (lst.Count == 0)
            {
                MonthlyTargetsEmployee obj = new MonthlyTargetsEmployee();
                obj.Emp_Cd = empCd;
                obj.Emp_Name = employeename;
                obj.Margin = string.Empty;
                obj.SubmissionTarget = string.Empty;
                obj.InterviewTarget = string.Empty;
                obj.HireTarget = string.Empty;
                obj.Team_Comments = string.Empty;
                obj.Supervisor_Comments = string.Empty;
                obj.Status = "Pending";
                obj.Month = month;
                obj.Year = year;
                obj.Created_By = empCd;
                obj.Created_Date = usDate;
                lst.Add(obj);
            }

            return lst;
        }

        public ActionResult SupervisorSearch(MonthlyTargetsVM model)
        {
            ModelState.Clear();
            string empCd = User.Identity.Name;
            string month = model.Month;
            int year = model.Year;
            ViewBag.EmpCd = empCd;
            TempData["EmpCd"] = empCd;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            //get the date.
            DateTime usDate = SystemClock.US_Date;
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName, EmployeeRoleName };

            //get the year list.
            SetDropdownYear();
            model.YearList = Enumerable.Range(ddlyear, yearcount).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Text).ToList();
            //get the month list.
            model.MonthList = Enumerable.Range(1, 12)
                              .Select(i => new SelectListItem
                              {
                                  Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i),
                                  Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                              }).ToList();

            //get the target for the team.
            FillSupervisorData(model, empCd, month, year);

            var empSupervisor = GetSupervisorDetail(empCd, month, year);

            model.EmpSupervisor = empSupervisor;
            //model.EmployeeList = lst;
            //var currentdatetime = DateTime.Now.Date;
            //string currentmonth = currentdatetime.ToString("MMMM");

            //var currentyear = DateTime.Now.Year;

            //if (currentmonth == month && currentyear == year)
            //{
            //    model.IsDisable = false;
            //}
            //else
            //{
            //    model.IsDisable = true;
            //}
            var inputmonth = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;
            var currentmonth = DateTime.Now.Date.Month;
            //var futuredatetime = DateTime.Now.Date.AddDays(15).Month;
            //int monthdays = int.Parse(currentmonth.ToString("dd"));
            var currentyear = DateTime.Now.Year;
            //string currentmonth = currentdatetime.ToString("MMMM");
            //int monthcount = 

            if (inputmonth == currentmonth && currentyear == year)
            {
                model.IsDisable = false;
            }
            /*this logic applies to 1 month in future including year*/
            else if (inputmonth > (currentmonth == 12 ? 0 : currentmonth) &&
                (inputmonth - (currentyear <= year ? 0 : currentmonth) == 1)
                && DateTime.Now.Date.Day > 15 && DateTime.Now.Date.Day <= 31 && currentyear <= year)
            {
                model.IsDisable = false;
            }
            //if (monthdays > 15 && currentyear == year)
            //{
            //    model.IsDisable = false;
            //}
            else
            {
                model.IsDisable = true;
            }


            return View("AddTargetsSupervisor", model);

        }

        private void FillSupervisorData(MonthlyTargetsVM model, string empCd, string month, int year)
        {
            var lstEmployees = unitOfWork.User.GetAllMgrEmployees(empCd, month, year);

            var lst = new List<MonthlyTargetsEmployee>();
            foreach (var item in lstEmployees)
            {

                MonthlyTargetsEmployee obj = new MonthlyTargetsEmployee();
                obj.Emp_Cd = item.Emp_Cd;
                obj.Emp_Name = item.Emp_Name;
                obj.Margin = item.Margin.ToString();
                obj.SubmissionTarget = item.SubmissionTarget.ToString();
                obj.InterviewTarget = item.InterviewTarget.ToString();
                obj.HireTarget = item.HireTarget.ToString();
                obj.Team_Comments = item.Team_Comments;
                obj.Supervisor_Comments = item.Supervisor_Comments;
                obj.Status = item.Status;
                obj.Month = item.Month;
                obj.Year = item.Year;
                obj.Created_By = item.Created_By;
                obj.Created_Date = item.Created_Date;
                lst.Add(obj);
            }
            model.EmployeeList = lst;
        }

        public ActionResult SupervisorLevel2Search(MonthlyTargetEmpSupervisorLevel2VM model)
        {
            ModelState.Clear();
            string empCd = User.Identity.Name;
            string month = model.Month;
            int year = model.Year;
            ViewBag.EmpCd = empCd;
            TempData["EmpCd"] = empCd;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            //get the date.
            DateTime usDate = SystemClock.US_Date;
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName, EmployeeRoleName };

            //get the year list.
            SetDropdownYearSupervisor();
            model.YearList = Enumerable.Range(ddlyear, yearcount).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Text).ToList();
            //get the month list.
            model.MonthList = Enumerable.Range(1, 12)
                              .Select(i => new SelectListItem
                              {
                                  Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i),
                                  Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                              }).ToList();

            //get the target for the team.
            FillSupervisorLevel2Data(model, empCd, month, year);
            //model.EmployeeList = lstSupervisorEmployees;
            //var currentdatetime = DateTime.Now.Date;
            //string currentmonth = currentdatetime.ToString("MMMM");

            //var currentyear = DateTime.Now.Year;

            //if (currentmonth == month && currentyear == year)
            //{
            //    model.IsDisable = false;
            //}
            //else
            //{
            //    model.IsDisable = true;
            //}
            CultureInfo culture = new CultureInfo("en-US");
            var inputmonth = DateTime.ParseExact(month, "MMMM", culture).Month;
            var currentmonth = DateTime.Now.Date.Month;
            //var futuredatetime = DateTime.Now.Date.AddDays(15).Month;
            //int monthdays = int.Parse(currentmonth.ToString("dd"));
            var currentyear = DateTime.Now.Year;
            //string currentmonth = currentdatetime.ToString("MMMM");
            //int monthcount = (currentyear <= year? 0:currentmonth) ;



            if (inputmonth == currentmonth && currentyear == year)
            {
                model.IsDisable = false;
            }
            /*this logic applies to 1 month in future including year*/
            else if (inputmonth > (currentmonth == 12 ? 0 : currentmonth) &&
                (inputmonth - (currentyear <= year ? 0 : currentmonth) == 1)
                && DateTime.Now.Date.Day > 15 && DateTime.Now.Date.Day <= 31 && currentyear <= year)
            {
                model.IsDisable = false;
            }
            //if (monthdays > 15 && currentyear == year)
            //{
            //    model.IsDisable = false;
            //}
            else
            {
                model.IsDisable = true;
            }

            return View("AddSupervisorLevel2Targets", model);

        }

        private void FillSupervisorLevel2Data(MonthlyTargetEmpSupervisorLevel2VM model, string empCd, string month, int year)
        {


            var lstSupervisors = unitOfWork.User.GetAllMgrEmployees(empCd, month, year);

            var empSupervisorList = new List<MonthlyTargetsEmployeeSupervisor>();
            foreach (var item in lstSupervisors)
            {

                MonthlyTargetsEmployeeSupervisor obj = new MonthlyTargetsEmployeeSupervisor();
                obj.Emp_Cd = item.Emp_Cd;
                obj.Emp_Name = item.Emp_Name;
                obj.Role = item.Role;
                obj.Margin = item.Margin.ToString();
                obj.SubmissionTarget = item.SubmissionTarget.ToString();
                obj.InterviewTarget = item.InterviewTarget.ToString();
                obj.HireTarget = item.HireTarget.ToString();
                obj.Team_Comments = item.Team_Comments;
                obj.Supervisor_Comments = item.Supervisor_Comments;
                obj.Account_Name = item.Account_Name;
                obj.Starts = item.Starts.ToString();
                obj.Status = item.Status;
                obj.Month = item.Month;
                obj.Year = item.Year;
                obj.Created_By = item.Created_By;
                obj.Created_Date = item.Created_Date;
                empSupervisorList.Add(obj);
            }
            var empSupervisorLevel2 = GetSupervisorLevel2Detail(empCd, month, year);
            //var lstSupervisorEmployeesRIC = new List<RIC_MonthlyTarget>();

            foreach (var item in empSupervisorList)
            {
                //lstSupervisorEmployeesRIC.AddRange(unitOfWork.User.GetAllMgrEmployees(item.Emp_Cd, month, year));
                var lstReportie = unitOfWork.User.GetAllMgrEmployees(item.Emp_Cd, month, year);
                if (lstReportie.Any())
                {
                    item.EmployeeList = new List<MonthlyTargetsEmployee>();
                    foreach (var emp in lstReportie)
                    {
                        MonthlyTargetsEmployee obj = new MonthlyTargetsEmployee();
                        obj.Emp_Cd = emp.Emp_Cd;
                        obj.Emp_Name = emp.Emp_Name;
                        obj.Margin = emp.Margin.ToString();
                        obj.SubmissionTarget = emp.SubmissionTarget.ToString();
                        obj.InterviewTarget = emp.InterviewTarget.ToString();
                        obj.HireTarget = emp.HireTarget.ToString();
                        obj.Team_Comments = emp.Team_Comments;
                        obj.Supervisor_Comments = emp.Supervisor_Comments;
                        obj.Status = emp.Status;
                        obj.Month = emp.Month;
                        obj.Year = emp.Year;
                        obj.Created_By = emp.Created_By;
                        obj.Created_Date = emp.Created_Date;
                        item.EmployeeList.Add(obj);
                    }

                }

            }
            //var lstSupervisorEmployees = new List<MonthlyTargetsEmployee>();
            //foreach (var item in lstSupervisorEmployeesRIC)
            //{

            //    MonthlyTargetsEmployee obj = new MonthlyTargetsEmployee();
            //    obj.Emp_Cd = item.Emp_Cd;
            //    obj.Emp_Name = item.Emp_Name;
            //    obj.Margin = item.Margin.ToString();
            //    obj.SubmissionTarget = item.SubmissionTarget.ToString();
            //    obj.InterviewTarget = item.InterviewTarget.ToString();
            //    obj.HireTarget = item.HireTarget.ToString();
            //    obj.Team_Comments = item.Team_Comments;
            //    obj.Supervisor_Comments = item.Supervisor_Comments;
            //    obj.Status = item.Status;
            //    obj.Month = item.Month;
            //    obj.Year = item.Year;
            //    obj.Created_By = item.Created_By;
            //    obj.Created_Date = item.Created_Date;
            //    lstSupervisorEmployees.Add(obj);
            //}
            //model.EmployeeList = lstSupervisorEmployees;

            model.EmpSupervisorlevel2 = empSupervisorLevel2;
            model.EmpSupervisorList = empSupervisorList;
        }

        private MonthlyTargetsEmpSupervisorlevel2 GetSupervisorLevel2Detail(string empCd, string month, int year)
        {
            var supervisortarget = unitOfWork.User.GetMonthlyTarget(empCd, month, year);
            MonthlyTargetsEmpSupervisorlevel2 obj = new MonthlyTargetsEmpSupervisorlevel2();
            var employeename = unitOfWork.User.GetEmployeeName(empCd);

            if (supervisortarget == null)
            {
                obj.Emp_Cd = empCd;
                obj.Emp_Name = employeename;
                obj.Margin = string.Empty;
                obj.SubmissionTarget = string.Empty;
                obj.InterviewTarget = string.Empty;
                obj.HireTarget = string.Empty;
                obj.Team_Comments = string.Empty;
                obj.Supervisor_Comments = string.Empty;
                obj.Status = "Submitted";
                obj.Month = month;
                obj.Year = year;
                obj.Created_By = empCd;
                obj.Account_Name = string.Empty;
                obj.Starts = string.Empty;
                //obj.Created_Date = usDate;

            }
            else
            {

                obj.Emp_Cd = supervisortarget.Emp_Cd;
                obj.Emp_Name = supervisortarget.Emp_Name;
                obj.Margin = supervisortarget.Margin.ToString();
                obj.Account_Name = supervisortarget.Account_Name;
                obj.Starts = supervisortarget.Starts.ToString();
                obj.SubmissionTarget = supervisortarget.SubmissionTarget.ToString();
                obj.InterviewTarget = supervisortarget.InterviewTarget.ToString();
                obj.HireTarget = supervisortarget.HireTarget.ToString();
                obj.Team_Comments = supervisortarget.Team_Comments;
                obj.Supervisor_Comments = supervisortarget.Supervisor_Comments;
                obj.Status = supervisortarget.Status;
                obj.Month = supervisortarget.Month;
                obj.Year = supervisortarget.Year;
                obj.Created_By = supervisortarget.Created_By;
                obj.Created_Date = supervisortarget.Created_Date;
            }

            return obj;

        }

        private MonthlyTargetsEmployeeSupervisor GetSupervisorDetail(string empCd, string month, int year)
        {
            var employeename = unitOfWork.User.GetEmployeeName(empCd);

            var supervisortarget = unitOfWork.User.GetMonthlyTarget(empCd, month, year);
            MonthlyTargetsEmployeeSupervisor obj = new MonthlyTargetsEmployeeSupervisor();
            if (supervisortarget == null)
            {
                obj.Emp_Cd = empCd;
                //obj.Emp_Name = string.Empty;
                obj.Emp_Name = employeename;
                //obj.Margin = 0;
                obj.Margin = string.Empty;
                obj.SubmissionTarget = string.Empty;
                obj.InterviewTarget = string.Empty;
                obj.HireTarget = string.Empty;
                obj.Team_Comments = string.Empty;
                obj.Supervisor_Comments = string.Empty;
                obj.Status = "Submitted";
                obj.Month = month;
                obj.Year = year;
                obj.Created_By = empCd;
                obj.Account_Name = string.Empty;
                obj.Starts = string.Empty;
                //obj.Created_Date = usDate;

            }
            else
            {

                obj.Emp_Cd = supervisortarget.Emp_Cd;
                obj.Emp_Name = supervisortarget.Emp_Name;
                obj.Margin = supervisortarget.Margin.ToString();
                obj.Account_Name = supervisortarget.Account_Name;
                obj.Starts = supervisortarget.Starts.ToString();
                obj.SubmissionTarget = supervisortarget.SubmissionTarget.ToString();
                obj.InterviewTarget = supervisortarget.InterviewTarget.ToString();
                obj.HireTarget = supervisortarget.HireTarget.ToString();
                obj.Team_Comments = supervisortarget.Team_Comments;
                obj.Supervisor_Comments = supervisortarget.Supervisor_Comments;
                obj.Status = supervisortarget.Status;
                obj.Month = supervisortarget.Month;
                obj.Year = supervisortarget.Year;
                obj.Created_By = supervisortarget.Created_By;
                obj.Created_Date = supervisortarget.Created_Date;
            }

            return obj;

        }

        public ActionResult SaveTargets(MonthlyTargetsEmployee model)
        {

            UnitOfWork unitOfWork = new UnitOfWork();
            string empCd = User.Identity.Name;
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            var target = unitOfWork.User.GetMonthlyTarget(empCd, model.Month, model.Year);
            var managercode = unitOfWork.User.GetManagerId(empCd);
            var employeename = unitOfWork.User.GetEmployeeName(empCd);

            DateTime usDate = SystemClock.US_Date;
            var monthlytarget = new RIC_MonthlyTarget();
            //var empCd = TempData["EmpCd"];

            if (target == null)
            {
                monthlytarget.Emp_Cd = empCd;
                monthlytarget.Emp_Name = employeename;
                monthlytarget.Role = role;
                monthlytarget.SubmissionTarget = Convert.ToInt32(model.SubmissionTarget);
                monthlytarget.InterviewTarget = Convert.ToInt32(model.InterviewTarget);
                monthlytarget.HireTarget = Convert.ToInt32(model.HireTarget);
                monthlytarget.Team_Comments = model.Team_Comments;
                //monthlytarget.Supervisor_Comments = model.Supervisor_Comments;
                monthlytarget.Mgr_Cd = managercode;
                monthlytarget.Status = "Submitted";
                monthlytarget.Margin = (decimal)Convert.ToDouble(model.Margin);
                monthlytarget.Month = model.Month;
                monthlytarget.Year = model.Year;
                monthlytarget.Created_By = model.Emp_Cd;
                monthlytarget.Created_Date = usDate;
                unitOfWork.User.InsertMonthlyTarget(monthlytarget);
            }

            else
            {
                //target.Emp_Cd = (string)TempData["EmpCd"];
                //target.Emp_Name = model.Emp_Name;
                target.Role = role;
                target.SubmissionTarget = Convert.ToInt32(model.SubmissionTarget);
                target.InterviewTarget = Convert.ToInt32(model.InterviewTarget);
                target.HireTarget = Convert.ToInt32(model.HireTarget);
                target.Team_Comments = model.Team_Comments;
                //target.Supervisor_Comments = model.Supervisor_Comments;
                target.Status = "Submitted";
                target.Month = model.Month;
                target.Year = model.Year;
                target.Mgr_Cd = managercode;
                target.Created_By = model.Emp_Cd;
                target.Created_Date = usDate;
                target.Margin = (decimal)Convert.ToDouble(model.Margin);
                //unitOfWork.User.UpdateMonthlyTarget(empCd.ToString());
                //return Content("Record Updated");

            }
            unitOfWork.Save();

            return RedirectToAction("Index");
        }

        public ActionResult AcceptTargetsBySupervisor(MonthlyTargetsEmployee model)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            var target = unitOfWork.User.GetMonthlyTarget(model.Emp_Cd, model.Month, model.Year);
            var managercode = unitOfWork.User.GetManagerId(model.Emp_Cd);
            var employeename = unitOfWork.User.GetEmployeeName(model.Emp_Cd);

            var monthlytarget = new RIC_MonthlyTarget();
            DateTime usDate = SystemClock.US_Date;
            //var empCd = TempData["EmpCd"];

            if (target == null)
            {
                monthlytarget.Emp_Cd = model.Emp_Cd;
                monthlytarget.Emp_Name = employeename;
                monthlytarget.Role = role;
                monthlytarget.Margin = (decimal)Convert.ToDouble(model.Margin);
                monthlytarget.SubmissionTarget = Convert.ToInt32(model.SubmissionTarget);
                monthlytarget.InterviewTarget = Convert.ToInt32(model.InterviewTarget);
                monthlytarget.HireTarget = Convert.ToInt32(model.HireTarget);
                monthlytarget.Team_Comments = model.Team_Comments;
                monthlytarget.Supervisor_Comments = model.Supervisor_Comments;
                if (monthlytarget.Margin == 0 && monthlytarget.SubmissionTarget == 0 && monthlytarget.InterviewTarget == 0 && monthlytarget.HireTarget == 0)
                {
                    monthlytarget.Status = "Pending";
                }
                else
                {
                    monthlytarget.Status = "Accepted";

                }
                monthlytarget.Mgr_Cd = managercode;
                monthlytarget.Month = model.Month;
                monthlytarget.Year = model.Year;
                monthlytarget.Created_By = model.Emp_Cd;
                monthlytarget.Created_Date = usDate;
                unitOfWork.User.InsertMonthlyTarget(monthlytarget);
            }

            else
            {
                //target.Emp_Cd = (string)TempData["EmpCd"];
                //target.Emp_Name = model.Emp_Name;
                target.Role = role;
                target.Margin = (decimal)Convert.ToDouble(model.Margin);
                target.SubmissionTarget = Convert.ToInt32(model.SubmissionTarget);
                target.InterviewTarget = Convert.ToInt32(model.InterviewTarget);
                target.HireTarget = Convert.ToInt32(model.HireTarget);
                target.Team_Comments = model.Team_Comments;
                target.Supervisor_Comments = model.Supervisor_Comments;
                if (target.Margin == 0 && target.SubmissionTarget == 0 && target.InterviewTarget == 0 && target.HireTarget == 0)
                {
                    target.Status = "Pending";
                }
                else
                {
                    target.Status = "Accepted";

                }
                target.Month = model.Month;
                target.Year = model.Year;
                target.Created_By = model.Emp_Cd;
                target.Created_Date = usDate;
                target.Mgr_Cd = managercode;
                //unitOfWork.User.UpdateMonthlyTarget(empCd.ToString());
                //return Content("Record Updated");

            }
            unitOfWork.Save();

            return RedirectToAction("SupervisorIndex");
        }

        public ActionResult SaveSupervisorTargets(MonthlyTargetsEmployeeSupervisor model)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            string empCd = User.Identity.Name;
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            var target = unitOfWork.User.GetMonthlyTarget(empCd, model.Month, model.Year);
            var managercode = unitOfWork.User.GetManagerId(empCd);
            var employeename = unitOfWork.User.GetEmployeeName(empCd);
            DateTime usDate = SystemClock.US_Date;
            var monthlytarget = new RIC_MonthlyTarget();
            //var empCd = TempData["EmpCd"];

            if (target == null)
            {
                monthlytarget.Emp_Cd = empCd;
                monthlytarget.Emp_Name = employeename;
                monthlytarget.Mgr_Cd = managercode;
                monthlytarget.Role = role;
                monthlytarget.SubmissionTarget = Convert.ToInt32(model.SubmissionTarget);
                monthlytarget.InterviewTarget = Convert.ToInt32(model.InterviewTarget);
                monthlytarget.HireTarget = Convert.ToInt32(model.HireTarget);
                monthlytarget.Team_Comments = model.Team_Comments;
                monthlytarget.Supervisor_Comments = model.Supervisor_Comments;
                monthlytarget.Status = "Submitted";
                monthlytarget.Account_Name = model.Account_Name;
                monthlytarget.Starts = Convert.ToDecimal(model.Starts);
                monthlytarget.Margin = Convert.ToDecimal(model.Margin);
                monthlytarget.Month = model.Month;
                monthlytarget.Year = model.Year;
                monthlytarget.Created_By = model.Emp_Cd;
                monthlytarget.Created_Date = usDate;
                unitOfWork.User.InsertMonthlyTarget(monthlytarget);
            }

            else
            {
                //target.Emp_Cd = (string)TempData["EmpCd"];
                //target.Emp_Name = model.Emp_Name;
                target.Role = role;
                target.Mgr_Cd = managercode;
                target.Account_Name = model.Account_Name;
                target.Starts = Convert.ToDecimal(model.Starts);
                target.SubmissionTarget = Convert.ToInt32(model.SubmissionTarget);
                target.InterviewTarget = Convert.ToInt32(model.InterviewTarget);
                target.HireTarget = Convert.ToInt32(model.HireTarget);
                target.Team_Comments = model.Team_Comments;
                target.Supervisor_Comments = model.Supervisor_Comments;
                target.Status = "Submitted";
                target.Month = model.Month;
                target.Year = model.Year;
                target.Created_By = model.Emp_Cd;
                target.Created_Date = usDate;
                target.Margin = Convert.ToDecimal(model.Margin);

            }
            unitOfWork.Save();

            return RedirectToAction("SupervisorIndex");
        }

        public ActionResult AcceptTargetsBySupervisorLevel2(MonthlyTargetsEmployee model)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            var target = unitOfWork.User.GetMonthlyTarget(model.Emp_Cd, model.Month, model.Year);
            var managercode = unitOfWork.User.GetManagerId(model.Emp_Cd);
            var employeename = unitOfWork.User.GetEmployeeName(model.Emp_Cd);
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles();
            ViewBag.roles = role;
            var monthlytarget = new RIC_MonthlyTarget();
            DateTime usDate = SystemClock.US_Date;
            //var empCd = TempData["EmpCd"];

            if (target == null)
            {
                monthlytarget.Emp_Cd = model.Emp_Cd;
                monthlytarget.Emp_Name = employeename;
                //monthlytarget.Role = role;
                monthlytarget.Margin = (decimal)Convert.ToDouble(model.Margin);
                monthlytarget.SubmissionTarget = Convert.ToInt32(model.SubmissionTarget);
                monthlytarget.InterviewTarget = Convert.ToInt32(model.InterviewTarget);
                monthlytarget.HireTarget = Convert.ToInt32(model.HireTarget);
                monthlytarget.Account_Name = model.Account_Name;
                monthlytarget.Starts = Convert.ToDecimal(model.Starts);
                monthlytarget.Team_Comments = model.Team_Comments;
                monthlytarget.Supervisor_Comments = model.Supervisor_Comments;
                if (monthlytarget.Account_Name == null && monthlytarget.Margin == 0 && monthlytarget.Starts == 0 && monthlytarget.HireTarget == 0 && monthlytarget.InterviewTarget == 0
                    && monthlytarget.SubmissionTarget == 0)
                {
                    monthlytarget.Status = "Pending";
                }
                else
                {
                    monthlytarget.Status = "Accepted";

                }
                monthlytarget.Mgr_Cd = managercode;
                monthlytarget.Month = model.Month;
                monthlytarget.Year = model.Year;
                monthlytarget.Created_By = model.Emp_Cd;
                monthlytarget.Created_Date = usDate;
                unitOfWork.User.InsertMonthlyTarget(monthlytarget);
            }

            else
            {
                //target.Emp_Cd = (string)TempData["EmpCd"];
                //target.Emp_Name = model.Emp_Name;
                target.Margin = (decimal)Convert.ToDouble(model.Margin);
                target.SubmissionTarget = Convert.ToInt32(model.SubmissionTarget);
                target.InterviewTarget = Convert.ToInt32(model.InterviewTarget);
                target.HireTarget = Convert.ToInt32(model.HireTarget);
                target.Account_Name = model.Account_Name;
                target.Starts = Convert.ToDecimal(model.Starts);
                if (target.Margin == 0 && target.Account_Name == "" && target.Starts == 0 && monthlytarget.HireTarget == 0 && monthlytarget.InterviewTarget == 0
                    && monthlytarget.SubmissionTarget == 0)
                {
                    target.Status = "Pending";
                }
                else
                {
                    target.Status = "Accepted";

                }
                target.Month = model.Month;
                target.Year = model.Year;
                target.Team_Comments = model.Team_Comments;
                target.Supervisor_Comments = model.Supervisor_Comments;
                target.Created_By = model.Emp_Cd;
                target.Created_Date = usDate;
                target.Mgr_Cd = managercode;
                //unitOfWork.User.UpdateMonthlyTarget(empCd.ToString());
                //return Content("Record Updated");

            }
            unitOfWork.Save();

            return RedirectToAction("SupervisorLevel2Index");
        }

        public ActionResult SaveSupervisorLevel2Targets(MonthlyTargetsEmployeeSupervisor model)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            string empCd = User.Identity.Name;
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            var target = unitOfWork.User.GetMonthlyTarget(empCd, model.Month, model.Year);
            var employeename = unitOfWork.User.GetEmployeeName(empCd);
            DateTime usDate = SystemClock.US_Date;
            var monthlytarget = new RIC_MonthlyTarget();
            //var empCd = TempData["EmpCd"];

            if (target == null)
            {
                monthlytarget.Emp_Cd = empCd;
                //monthlytarget.Emp_Name = model.Emp_Name;
                monthlytarget.Emp_Name = employeename;
                monthlytarget.Role = role;
                //monthlytarget.SubmissionTarget = (int)model.SubmissionTarget;
                //monthlytarget.InterviewTarget = (int)model.InterviewTarget;
                // monthlytarget.HireTarget = (int)model.HireTarget;
                monthlytarget.Team_Comments = model.Team_Comments;
                monthlytarget.Supervisor_Comments = model.Supervisor_Comments;
                monthlytarget.Status = "Submitted";
                monthlytarget.Account_Name = model.Account_Name;
                monthlytarget.Starts = Convert.ToDecimal(model.Starts);
                monthlytarget.Margin = Convert.ToDecimal(model.Margin);
                monthlytarget.Month = model.Month;
                monthlytarget.Year = model.Year;
                monthlytarget.Created_By = model.Emp_Cd;
                monthlytarget.Created_Date = usDate;
                unitOfWork.User.InsertMonthlyTarget(monthlytarget);
            }

            else
            {
                //target.Emp_Cd = (string)TempData["EmpCd"];
                //target.Emp_Name = model.Emp_Name;
                target.Account_Name = model.Account_Name;
                target.Starts = Convert.ToDecimal(model.Starts);
                target.Role = role;
                // target.SubmissionTarget = (int)model.SubmissionTarget;
                //target.InterviewTarget = (int)model.InterviewTarget;
                //target.HireTarget = (int)model.HireTarget;
                target.Team_Comments = model.Team_Comments;
                target.Supervisor_Comments = model.Supervisor_Comments;
                target.Status = "Submitted";
                target.Month = model.Month;
                target.Year = model.Year;
                target.Created_By = model.Emp_Cd;
                target.Created_Date = usDate;
                target.Margin = Convert.ToDecimal(model.Margin);

            }
            unitOfWork.Save();

            return RedirectToAction("SupervisorLevel2Index");
        }

        [HttpGet]
        public ActionResult AcceptTargetsBySupervisorbyAjax(MonthlyTargetsEmployee data)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            DateTime usDate = SystemClock.US_Date;
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            data.Year = usDate.Year;
            data.Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month);
            var target = unitOfWork.User.GetMonthlyTarget(data.Emp_Cd, data.Month, data.Year);
            var managercode = unitOfWork.User.GetManagerId(data.Emp_Cd);
            var employeename = unitOfWork.User.GetEmployeeName(data.Emp_Cd);

            var monthlytarget = new RIC_MonthlyTarget();
            //DateTime usDate = SystemClock.US_Date;
            //var empCd = TempData["EmpCd"];

            if (target == null)
            {
                monthlytarget.Emp_Cd = data.Emp_Cd;
                monthlytarget.Emp_Name = employeename;
                monthlytarget.Role = role;
                monthlytarget.Margin = (decimal)Convert.ToDouble(data.Margin);
                monthlytarget.SubmissionTarget = Convert.ToInt32(data.SubmissionTarget);
                monthlytarget.InterviewTarget = Convert.ToInt32(data.InterviewTarget);
                monthlytarget.HireTarget = Convert.ToInt32(data.HireTarget);
                monthlytarget.Team_Comments = data.Team_Comments;
                monthlytarget.Supervisor_Comments = data.Supervisor_Comments;
                if (monthlytarget.Margin == 0 && monthlytarget.SubmissionTarget == 0 && monthlytarget.InterviewTarget == 0 && monthlytarget.HireTarget == 0)
                {
                    monthlytarget.Status = "Pending";
                }
                else
                {
                    monthlytarget.Status = "Accepted";

                }
                monthlytarget.Mgr_Cd = managercode;
                monthlytarget.Month = data.Month;
                monthlytarget.Year = data.Year;
                monthlytarget.Created_By = data.Emp_Cd;
                monthlytarget.Created_Date = usDate;
                unitOfWork.User.InsertMonthlyTarget(monthlytarget);
            }

            else
            {
                //target.Emp_Cd = (string)TempData["EmpCd"];
                //target.Emp_Name = model.Emp_Name;
                target.Role = role;
                target.Margin = (decimal)Convert.ToDouble(data.Margin);
                target.SubmissionTarget = Convert.ToInt32(data.SubmissionTarget);
                target.InterviewTarget = Convert.ToInt32(data.InterviewTarget);
                target.HireTarget = Convert.ToInt32(data.HireTarget);
                target.Team_Comments = data.Team_Comments;
                target.Supervisor_Comments = data.Supervisor_Comments;
                if (target.Margin == 0 && target.SubmissionTarget == 0 && target.InterviewTarget == 0 && target.HireTarget == 0)
                {
                    target.Status = "Pending";
                }
                else
                {
                    target.Status = "Accepted";

                }
                target.Month = data.Month;
                target.Year = data.Year;
                target.Created_By = data.Emp_Cd;
                target.Created_Date = usDate;
                target.Mgr_Cd = managercode;
                //unitOfWork.User.UpdateMonthlyTarget(empCd.ToString());
                //return Content("Record Updated");

            }
            unitOfWork.Save();

            return RedirectToAction("SupervisorLevel2Index");
        }

        private void SetDropdownYear()
        {
            DateTime usDate = SystemClock.US_Date;

            ddlyear = usDate.Year - 2;
            // int ddlyear = 2021;
            yearcount = 3;
            if (usDate.Month == 12 && usDate.Day >= 15)
            {
                ddlyear++;
                yearcount = 3;
            }
        }

        private void SetDropdownYearSupervisor()
        {
            DateTime usDate = SystemClock.US_Date;

            ddlyear = usDate.Year - 4;
            // int ddlyear = 2021;
            yearcount = 5;
            if (usDate.Month == 12 && usDate.Day >= 15)
            {
                ddlyear++;
                yearcount = 5;
            }
        }

        #region
        [Authorize(Roles = " Director-Staffing ")]
        public ActionResult StaffingDirector()
        {
            string empCd = User.Identity.Name;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            DateTime usDate = SystemClock.US_Date;
            var model = new MonthlyStaffingDirectorVM();
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName, EmployeeRoleName };
            //get the reporting history for user.
            var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role, "REC")
                                   .Select(s => s.RR_EmpCD).ToList();
            reportingHistory.Add(empCd);
            //get the user list for tl.
            model.ReportingList = unitOfWork.User.getAllUsers()
                     .Where(s => roleList.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name)
                           && reportingHistory.Contains(s.RE_Emp_Cd)).Where(s => s.RE_Resign_Date == null)
                    .Select(s => new SelectListItem
                    {
                        Text = s.RE_Jobdiva_User_Name
                        ,
                        Value = s.RE_Emp_Cd
                    }).OrderBy(o => o.Text).ToList();
            //get the year list.
            SetDropdownYearSupervisor();
            model.YearList = Enumerable.Range(ddlyear, yearcount).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Text).ToList();
            //get the month list.
            model.MonthList = Enumerable.Range(1, 12)
            //model.MonthList = new List<SelectListItem>()

                              .Select(i => new SelectListItem
                              {
                                  Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i),
                                  Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                              }).ToList();


            model.Year = usDate.Year;
            model.Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month);
            FillStaffingDirector(model, empCd, model.Month, model.Year);
            var employeename = unitOfWork.User.GetEmployeeName(empCd);
            return View("AddTargetsDirectorLevel", model);
        }

        public ActionResult StaffingDirectorSearch(MonthlyStaffingDirectorVM model)
        {
            ModelState.Clear();
            string empCd = User.Identity.Name;
            string month = model.Month;
            int year = model.Year;
            ViewBag.EmpCd = empCd;
            TempData["EmpCd"] = empCd;
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            //get the date.
            DateTime usDate = SystemClock.US_Date;
            var roleList = new List<string>() { stafingDirectorRole, tlRoleName, mgrRoleName, EmployeeRoleName };

            //get the year list.
            SetDropdownYearSupervisor();
            model.YearList = Enumerable.Range(ddlyear, yearcount).Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString()
            }).OrderByDescending(s => s.Text).ToList();
            //get the month list.
            model.MonthList = Enumerable.Range(1, 12)
                              .Select(i => new SelectListItem
                              {
                                  Text = DateTimeFormatInfo.CurrentInfo.GetMonthName(i),
                                  Value = DateTimeFormatInfo.CurrentInfo.GetMonthName(i)
                              }).ToList();

            //get the target for the team.
            FillStaffingDirector(model, empCd, month, year);
            //model.EmployeeList = lstSupervisorEmployees;
            //var currentdatetime = DateTime.Now.Date;
            //string currentmonth = currentdatetime.ToString("MMMM");

            //var currentyear = DateTime.Now.Year;

            //if (currentmonth == month && currentyear == year)
            //{
            //    model.IsDisable = false;
            //}
            //else
            //{
            //    model.IsDisable = true;
            //}
            CultureInfo culture = new CultureInfo("en-US");
            var inputmonth = DateTime.ParseExact(month, "MMMM", culture).Month;
            var currentmonth = DateTime.Now.Date.Month;
            //var futuredatetime = DateTime.Now.Date.AddDays(15).Month;
            //int monthdays = int.Parse(currentmonth.ToString("dd"));
            var currentyear = DateTime.Now.Year;
            //string currentmonth = currentdatetime.ToString("MMMM");
            //int monthcount = (currentyear <= year? 0:currentmonth) ;



            if (inputmonth == currentmonth && currentyear == year)
            {
                model.IsDisable = false;
            }
            /*this logic applies to 1 month in future including year*/
            else if (inputmonth > (currentmonth == 12 ? 0 : currentmonth) &&
                (inputmonth - (currentyear <= year ? 0 : currentmonth) == 1)
                && DateTime.Now.Date.Day > 15 && DateTime.Now.Date.Day <= 31 && currentyear <= year)
            {
                model.IsDisable = false;
            }
            //if (monthdays > 15 && currentyear == year)
            //{
            //    model.IsDisable = false;
            //}
            else
            {
                model.IsDisable = true;
            }

            return View("AddTargetsDirectorLevel", model);

        }

        private StaffingDirector GetStaffingdirector(string empCd, string month, int year)
        {
            var supervisortarget = unitOfWork.User.GetMonthlyTarget(empCd, month, year);
            StaffingDirector obj = new StaffingDirector();
            var employeename = unitOfWork.User.GetEmployeeName(empCd);

            if (supervisortarget == null)
            {
                obj.Emp_Cd = empCd;
                obj.Emp_Name = employeename;
                obj.Margin = string.Empty;
                obj.SubmissionTarget = string.Empty;
                obj.InterviewTarget = string.Empty;
                obj.HireTarget = string.Empty;
                obj.Team_Comments = string.Empty;
                obj.Supervisor_Comments = string.Empty;
                obj.Status = "Submitted";
                obj.Month = month;
                obj.Year = year;
                obj.Created_By = empCd;
                obj.Account_Name = string.Empty;
                obj.Starts = string.Empty;
                //obj.Created_Date = usDate;

            }
            else
            {

                obj.Emp_Cd = supervisortarget.Emp_Cd;
                obj.Emp_Name = supervisortarget.Emp_Name;
                obj.Margin = supervisortarget.Margin.ToString();
                obj.Account_Name = supervisortarget.Account_Name;
                obj.Starts = supervisortarget.Starts.ToString();
                obj.SubmissionTarget = supervisortarget.SubmissionTarget.ToString();
                obj.InterviewTarget = supervisortarget.InterviewTarget.ToString();
                obj.HireTarget = supervisortarget.HireTarget.ToString();
                obj.Team_Comments = supervisortarget.Team_Comments;
                obj.Supervisor_Comments = supervisortarget.Supervisor_Comments;
                obj.Status = supervisortarget.Status;
                obj.Month = supervisortarget.Month;
                obj.Year = supervisortarget.Year;
                obj.Created_By = supervisortarget.Created_By;
                obj.Created_Date = supervisortarget.Created_Date;
            }

            return obj;

        }

        private void FillStaffingDirector(MonthlyStaffingDirectorVM model, string empCd, string month, int year)
        {


            var lstSupervisors = unitOfWork.User.GetAllMgrEmployees(empCd, month, year);

            var empSupervisorList = new List<MonthlyTargetsEmployeeSupervisor>();
            foreach (var item in lstSupervisors)
            {

                MonthlyTargetsEmployeeSupervisor obj = new MonthlyTargetsEmployeeSupervisor();
                obj.Emp_Cd = item.Emp_Cd;
                obj.Emp_Name = item.Emp_Name;
                obj.Role = item.Role;
                obj.Margin = item.Margin.ToString();
                obj.SubmissionTarget = item.SubmissionTarget.ToString();
                obj.InterviewTarget = item.InterviewTarget.ToString();
                obj.HireTarget = item.HireTarget.ToString();
                obj.Team_Comments = item.Team_Comments;
                obj.Supervisor_Comments = item.Supervisor_Comments;
                obj.Account_Name = item.Account_Name;
                obj.Starts = item.Starts.ToString();
                obj.Status = item.Status;
                obj.Month = item.Month;
                obj.Year = item.Year;
                obj.Created_By = item.Created_By;
                obj.Created_Date = item.Created_Date;
                empSupervisorList.Add(obj);
            }
            var empStaffingDirector = GetStaffingdirector(empCd, month, year);
            //var lstSupervisorEmployeesRIC = new List<RIC_MonthlyTarget>();

            foreach (var item in empSupervisorList)
            {
                //lstSupervisorEmployeesRIC.AddRange(unitOfWork.User.GetAllMgrEmployees(item.Emp_Cd, month, year));
                var lstReportie = unitOfWork.User.GetAllMgrEmployees(item.Emp_Cd, month, year);
                if (lstReportie.Any())
                {
                    item.EmployeeList = new List<MonthlyTargetsEmployee>();
                    foreach (var emp in lstReportie)
                    {
                        MonthlyTargetsEmployee obj = new MonthlyTargetsEmployee();
                        obj.Emp_Cd = emp.Emp_Cd;
                        obj.Emp_Name = emp.Emp_Name;
                        obj.Margin = emp.Margin.ToString();
                        obj.SubmissionTarget = emp.SubmissionTarget.ToString();
                        obj.InterviewTarget = emp.InterviewTarget.ToString();
                        obj.HireTarget = emp.HireTarget.ToString();
                        obj.Team_Comments = emp.Team_Comments;
                        obj.Supervisor_Comments = emp.Supervisor_Comments;
                        obj.Status = emp.Status;
                        obj.Month = emp.Month;
                        obj.Year = emp.Year;
                        obj.Role = emp.Role;
                        obj.Created_By = emp.Created_By;
                        obj.Created_Date = emp.Created_Date;
                        item.EmployeeList.Add(obj);
                        obj.employees = new List<MonthlyTargetsEmployee>();
                        var lstReportie2 = unitOfWork.User.GetAllMgrEmployees(emp.Emp_Cd, month, year);
                        if (lstReportie2.Any())
                        {

                            foreach (var emplead in lstReportie2)
                            {
                                MonthlyTargetsEmployee objlead = new MonthlyTargetsEmployee();
                                objlead.Emp_Cd = emplead.Emp_Cd;
                                objlead.Emp_Name = emplead.Emp_Name;
                                objlead.Mgr_Cd = emplead.Mgr_Cd;
                                objlead.Margin = emplead.Margin.ToString();
                                objlead.SubmissionTarget = emplead.SubmissionTarget.ToString();
                                objlead.InterviewTarget = emplead.InterviewTarget.ToString();
                                objlead.HireTarget = emplead.HireTarget.ToString();
                                objlead.Team_Comments = emplead.Team_Comments;
                                objlead.Supervisor_Comments = emplead.Supervisor_Comments;
                                objlead.Status = emplead.Status;
                                objlead.Month = emplead.Month;
                                objlead.Year = emplead.Year;
                                objlead.Role = emplead.Role;
                                objlead.Created_By = emplead.Created_By;
                                objlead.Created_Date = emplead.Created_Date;
                                obj.employees.Add(objlead);
                            }

                        }
                    }

                }

            }


            model.StaffingDirector = empStaffingDirector;
            model.EmpSupervisorList = empSupervisorList;
        }

        public ActionResult SaveStaffingDirectorTargets(MonthlyTargetsEmployeeSupervisor model)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            string empCd = User.Identity.Name;
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            var target = unitOfWork.User.GetMonthlyTarget(empCd, model.Month, model.Year);
            var employeename = unitOfWork.User.GetEmployeeName(empCd);
            DateTime usDate = SystemClock.US_Date;
            var monthlytarget = new RIC_MonthlyTarget();
            //var empCd = TempData["EmpCd"];

            if (target == null)
            {
                monthlytarget.Emp_Cd = empCd;
                //monthlytarget.Emp_Name = model.Emp_Name;
                monthlytarget.Emp_Name = employeename;
                monthlytarget.Role = role;
                //monthlytarget.SubmissionTarget = (int)model.SubmissionTarget;
                //monthlytarget.InterviewTarget = (int)model.InterviewTarget;
                // monthlytarget.HireTarget = (int)model.HireTarget;
                monthlytarget.Team_Comments = model.Team_Comments;
                monthlytarget.Supervisor_Comments = model.Supervisor_Comments;
                monthlytarget.Status = "Submitted";
                monthlytarget.Account_Name = model.Account_Name;
                monthlytarget.Starts = Convert.ToDecimal(model.Starts);
                monthlytarget.Margin = Convert.ToDecimal(model.Margin);
                monthlytarget.Month = model.Month;
                monthlytarget.Year = model.Year;
                monthlytarget.Created_By = model.Emp_Cd;
                monthlytarget.Created_Date = usDate;
                unitOfWork.User.InsertMonthlyTarget(monthlytarget);
            }

            else
            {
                //target.Emp_Cd = (string)TempData["EmpCd"];
                //target.Emp_Name = model.Emp_Name;
                target.Account_Name = model.Account_Name;
                target.Starts = Convert.ToDecimal(model.Starts);
                target.Role = role;
                // target.SubmissionTarget = (int)model.SubmissionTarget;
                //target.InterviewTarget = (int)model.InterviewTarget;
                //target.HireTarget = (int)model.HireTarget;
                target.Team_Comments = model.Team_Comments;
                target.Supervisor_Comments = model.Supervisor_Comments;
                target.Status = "Submitted";
                target.Month = model.Month;
                target.Year = model.Year;
                target.Created_By = model.Emp_Cd;
                target.Created_Date = usDate;
                target.Margin = Convert.ToDecimal(model.Margin);

            }
            unitOfWork.Save();

            return RedirectToAction("StaffingDirector");
        }

        [HttpGet]
        public ActionResult AcceptTargetsByStaffingDirectorbyAjax(MonthlyTargetsEmployee data)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            DateTime usDate = SystemClock.US_Date;
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            data.Year = usDate.Year;
            data.Month = DateTimeFormatInfo.CurrentInfo.GetMonthName(usDate.Month);
            var target = unitOfWork.User.GetMonthlyTarget(data.Emp_Cd, data.Month, data.Year);
            var managercode = unitOfWork.User.GetManagerId(data.Emp_Cd);
            var employeename = unitOfWork.User.GetEmployeeName(data.Emp_Cd);

            var monthlytarget = new RIC_MonthlyTarget();
            //DateTime usDate = SystemClock.US_Date;
            //var empCd = TempData["EmpCd"];

            if (target == null)
            {
                monthlytarget.Emp_Cd = data.Emp_Cd;
                monthlytarget.Emp_Name = employeename;
                monthlytarget.Role = role;
                monthlytarget.Margin = (decimal)Convert.ToDouble(data.Margin);
                monthlytarget.SubmissionTarget = Convert.ToInt32(data.SubmissionTarget);
                monthlytarget.InterviewTarget = Convert.ToInt32(data.InterviewTarget);
                monthlytarget.HireTarget = Convert.ToInt32(data.HireTarget);
                monthlytarget.Team_Comments = data.Team_Comments;
                monthlytarget.Supervisor_Comments = data.Supervisor_Comments;
                if (monthlytarget.Margin == 0 && monthlytarget.SubmissionTarget == 0 && monthlytarget.InterviewTarget == 0 && monthlytarget.HireTarget == 0)
                {
                    monthlytarget.Status = "Pending";
                }
                else
                {
                    monthlytarget.Status = "Accepted";

                }
                monthlytarget.Mgr_Cd = managercode;
                monthlytarget.Month = data.Month;
                monthlytarget.Year = data.Year;
                monthlytarget.Created_By = data.Emp_Cd;
                monthlytarget.Created_Date = usDate;
                unitOfWork.User.InsertMonthlyTarget(monthlytarget);
            }

            else
            {
                //target.Emp_Cd = (string)TempData["EmpCd"];
                //target.Emp_Name = model.Emp_Name;
                target.Role = role;
                target.Margin = (decimal)Convert.ToDouble(data.Margin);
                target.SubmissionTarget = Convert.ToInt32(data.SubmissionTarget);
                target.InterviewTarget = Convert.ToInt32(data.InterviewTarget);
                target.HireTarget = Convert.ToInt32(data.HireTarget);
                target.Team_Comments = data.Team_Comments;
                target.Supervisor_Comments = data.Supervisor_Comments;
                if (target.Margin == 0 && target.SubmissionTarget == 0 && target.InterviewTarget == 0 && target.HireTarget == 0)
                {
                    target.Status = "Pending";
                }
                else
                {
                    target.Status = "Accepted";

                }
                target.Month = data.Month;
                target.Year = data.Year;
                target.Created_By = data.Emp_Cd;
                target.Created_Date = usDate;
                target.Mgr_Cd = managercode;
                //unitOfWork.User.UpdateMonthlyTarget(empCd.ToString());
                //return Content("Record Updated");

            }
            unitOfWork.Save();

            return RedirectToAction("StaffingDirector");
        }

        public ActionResult AcceptTargetsByStaffingDirector(MonthlyTargetsEmployee model)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            var target = unitOfWork.User.GetMonthlyTarget(model.Emp_Cd, model.Month, model.Year);
            var managercode = unitOfWork.User.GetManagerId(model.Emp_Cd);
            var employeename = unitOfWork.User.GetEmployeeName(model.Emp_Cd);
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles();
            ViewBag.roles = role;
            var monthlytarget = new RIC_MonthlyTarget();
            DateTime usDate = SystemClock.US_Date;
            //var empCd = TempData["EmpCd"];

            if (target == null)
            {
                monthlytarget.Emp_Cd = model.Emp_Cd;
                monthlytarget.Emp_Name = employeename;
                //monthlytarget.Role = role;
                monthlytarget.Margin = (decimal)Convert.ToDouble(model.Margin);
                monthlytarget.SubmissionTarget = Convert.ToInt32(model.SubmissionTarget);
                monthlytarget.InterviewTarget = Convert.ToInt32(model.InterviewTarget);
                monthlytarget.HireTarget = Convert.ToInt32(model.HireTarget);
                monthlytarget.Account_Name = model.Account_Name;
                monthlytarget.Starts = Convert.ToDecimal(model.Starts);
                monthlytarget.Team_Comments = model.Team_Comments;
                monthlytarget.Supervisor_Comments = model.Supervisor_Comments;
                if (monthlytarget.Account_Name == null && monthlytarget.Margin == 0 && monthlytarget.Starts == 0 && monthlytarget.HireTarget == 0 && monthlytarget.InterviewTarget == 0
                    && monthlytarget.SubmissionTarget == 0)
                {
                    monthlytarget.Status = "Pending";
                }
                else
                {
                    monthlytarget.Status = "Accepted";

                }
                monthlytarget.Mgr_Cd = managercode;
                monthlytarget.Month = model.Month;
                monthlytarget.Year = model.Year;
                monthlytarget.Created_By = model.Emp_Cd;
                monthlytarget.Created_Date = usDate;
                unitOfWork.User.InsertMonthlyTarget(monthlytarget);
            }

            else
            {
                //target.Emp_Cd = (string)TempData["EmpCd"];
                //target.Emp_Name = model.Emp_Name;
                target.Margin = (decimal)Convert.ToDouble(model.Margin);
                target.SubmissionTarget = Convert.ToInt32(model.SubmissionTarget);
                target.InterviewTarget = Convert.ToInt32(model.InterviewTarget);
                target.HireTarget = Convert.ToInt32(model.HireTarget);
                target.Account_Name = model.Account_Name;
                target.Starts = Convert.ToDecimal(model.Starts);
                if (target.Margin == 0 && target.Account_Name == "" && target.Starts == 0 && monthlytarget.HireTarget == 0 && monthlytarget.InterviewTarget == 0
                    && monthlytarget.SubmissionTarget == 0)
                {
                    target.Status = "Pending";
                }
                else
                {
                    target.Status = "Accepted";

                }
                target.Month = model.Month;
                target.Year = model.Year;
                target.Team_Comments = model.Team_Comments;
                target.Supervisor_Comments = model.Supervisor_Comments;
                target.Created_By = model.Emp_Cd;
                target.Created_Date = usDate;
                target.Mgr_Cd = managercode;
                //unitOfWork.User.UpdateMonthlyTarget(empCd.ToString());
                //return Content("Record Updated");

            }
            unitOfWork.Save();

            return RedirectToAction("StaffingDirector");
        }
        # endregion 

    }
}