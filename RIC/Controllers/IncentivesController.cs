using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DBLibrary;
using DBLibrary.UnitOfWork;
using RIC.Models;
using RIC.Utility;
using System.IO;
using OfficeOpenXml;

namespace RIC.Controllers
{
    public class IncentivesController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        //
        // GET: /Incentives/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult IncentivePartial(int Year)
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            //  IncentiveIndexAction incetive=new IncentiveIndexAction();

            List<GetIncentiveResult> incentiveResult = unitOfWork.Incentive.getTncetive(empCd, Year).ToList();


            var incentiveData = from incentive in unitOfWork.Incentive.Get()
                                where incentive.RI_Year == Year && incentive.RI_EmpCd == empCd && incentive.RI_TotalRecurringPaid != null
                                group incentive by 1 into incentiveSum
                                select new IncentiveResultData
                                {
                                    RI_TotalRecurringPaid = incentiveSum.Sum(s => s.RI_TotalRecurringPaid),
                                    RI_OneTimeIncentive = incentiveSum.Sum(s => s.RI_OneTimeIncentive),
                                    RI_finalDifference = incentiveSum.Sum(s => s.RI_finalDifference)
                                };

            incentiveResult.FirstOrDefault().IncentiveResultData = incentiveData.ToList();

            ViewBag.Year = Year;

            return PartialView(incentiveResult);
        }

        [Authorize]
        public ActionResult ViewIncentivePopup(string date, string showTotal, int Year, string getType = null)
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            List<Get_All_IncentivesResult> incentives = new List<Get_All_IncentivesResult>();


            ViewBag.Header = getType == null ? "Incentives for " + date.ToLower() : "Incentives";
            ViewBag.IncentiveType = getType;
            int month = getType == null ? DateTime.ParseExact(date, "MMMM", CultureInfo.CurrentCulture).Month : 0;

            if (getType != null)
            {
                switch (getType)
                {
                    case "RecurringPaid":
                        incentives = unitOfWork.Incentive.getAllIncentives().Where(s => s.RI_EmpCd == empCd && s.RI_Year == Year && s.RI_TotalRecurringPaid != null).ToList();
                        break;

                    case "OneTimeIncentive":
                        incentives = unitOfWork.Incentive.getAllIncentives().Where(s => s.RI_EmpCd == empCd && s.RI_Year == Year && s.RI_OneTimeIncentive != null).ToList();
                        break;

                    case "FinalDiffrence":
                        incentives = unitOfWork.Incentive.getAllIncentives().Where(s => s.RI_EmpCd == empCd && s.RI_Year == Year && s.RI_finalDifference != null).ToList();
                        break;
                }

                incentives = incentives.GroupBy(item => new { item.RI_Company, item.RI_Candidate }).Select(grouping => grouping.FirstOrDefault()).ToList();

            }
            else if (showTotal == "Starts In a Month" || showTotal == "Margin ($)")//if start of month click.
            {
                incentives = unitOfWork.Incentive.getAllIncentives().
                           Where(s => s.RI_Month == date && s.RI_EmpCd == empCd && s.RI_Year == Year && s.RI_StartDate.Month == month && s.RI_StartDate.Year == Year).ToList();
            }
            else if (showTotal == "Ends in a Month")//if end of month is clicked
            {
                // select if end date is equal to month.
                incentives = unitOfWork.Incentive.getAllIncentives().
                   Where(s => s.RI_Month == date && s.RI_EmpCd == empCd && s.RI_EndDate != null && s.RI_EndDate.Value.Month == month && s.RI_EndDate.Value.Year == Year && s.RI_Year == Year).ToList();
            }
            else
            {
                // select if end date is equal to month.
                incentives = unitOfWork.Incentive.getAllIncentives().
                             Where(s => s.RI_Month == date && s.RI_EmpCd == empCd && s.RI_Year == Year).ToList();
            }

            return View(incentives);
        }


        [Authorize]
        [HttpGet]
        public ActionResult QuarterlyIncentive()
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            string EmployeeName = unitOfWork.User.Get().Where(s => s.RE_Emp_Cd == empCd).Select(s => s.RE_AKA_Name).FirstOrDefault();
            // get the role for user.
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            //if (role == "Director")
            //{
            //    ViewBag.role = role;
            //    return View();
            //}
            IEnumerable<RIC_QuarterlyIncentive> listuser = unitOfWork.QuarterlyIncentive.Get();//New

            List<SelectListItem> listOfYears = new List<SelectListItem>();
            int year = DateTime.Now.Year;
            for (int j = year; j >= 2019; j--)
            {
                SelectListItem lt = new SelectListItem();
                lt.Text = j.ToString();
                lt.Value = j.ToString();
                lt.Selected = j == year ? true : false;
                listOfYears.Add(lt);
            }
            ViewBag.years = listOfYears;//New ,this code will help to display the year to dropdownlist           

            List<SelectListItem> listOfQuaters = new List<SelectListItem>();
            int quater = GetQuarter(DateTime.Now.Date);
            for (int j = 1; j <= 4; j++)
            {
                SelectListItem lt = new SelectListItem();
                lt.Text = "Q" + j;
                lt.Value = j.ToString();
                lt.Selected = j == quater ? true : false;
                listOfQuaters.Add(lt);
            }
            ViewBag.quaters = listOfQuaters;   //New ,this code will help to display the quaters to dropdownlist     


            DateTime usDate = SystemClock.US_Date;

            //get the reporting history for user.(This code,who is reporting to TeamLead)
            var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role, "REC")
                                   .Select(s => new
                                   {
                                       R_EmpCD = s.RR_EmpCD,
                                       Employee_Name = s.Employee_Name,

                                   });
            List<SelectListItem> Items = new List<SelectListItem>();
            foreach (var employee in reportingHistory)
            {

                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = employee.Employee_Name;
                selectListItem.Value = employee.R_EmpCD;

                if (!Items.Any(s => s.Value == selectListItem.Value))
                {
                    Items.Add(selectListItem);
                }

            }


            List<SelectListItem> itemofEmployee = Items.OrderBy(x => x.Text).ToList();
            ViewBag.listofemployee = itemofEmployee;

            return View();
        }
        [HttpGet]
        public ActionResult Incentives()
        {
            return View();
        }
        public ActionResult GetQuarterlyListPartial(string years, string quaterly, string employeeId)
        {
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            IEnumerable<RIC_QuarterlyIncentive> listOfUsers;
            //string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;  
            RolePrincipal r = (RolePrincipal)User;
            var role = r.GetRoles()[0];
            DateTime usDate = SystemClock.US_Date;

            //get the reporting history for user.(This code,who is reporting to TeamLead)
            var reportingHistory = unitOfWork.User.getReportingHistory(empCd, usDate.Date, usDate.Date, role, "REC")
                                   .Select(s => s.RR_EmpCD).ToList();

            if (employeeId == string.Empty)
            {

                var EMpnew = new List<RIC_QuarterlyIncentive>();
                for (int i = 0; i < reportingHistory.Count(); i++)
                {
                    var item = reportingHistory[i];

                    listOfUsers = unitOfWork.QuarterlyIncentive.Get().Where(s => s.RI_EmpCd == item);



                    if (listOfUsers.Count() > 0)
                    {

                        EMpnew.AddRange(listOfUsers);

                    }

                }
                //listOfUsers = EMpnew.Where(s => Convert.ToDateTime(s.RI_StartDate).Year == years && (Convert.ToDateTime(s.RI_StartDate).Month + 2) / 3 == quaterly).ToList();
                listOfUsers = EMpnew.Where(s => s.RI_IncentiveCategory.Contains(years) && s.RI_IncentiveCategory.ToLower().Contains(quaterly.ToLower())).ToList();

                return PartialView("GetQuarterlyListPartial", listOfUsers);
            }
            else
            {

                if (User.IsInRole(System.Configuration.ConfigurationManager.AppSettings["EmployeeRole"]))
                {
                    employeeId = empCd;
                }
                //listOfUsers = unitOfWork.QuarterlyIncentive.Get().Where(s => Convert.ToDateTime(s.RI_StartDate).Year == years && (Convert.ToDateTime(s.RI_StartDate).Month + 2) / 3 == quaterly && s.RI_EmpCd == employeeId).ToList();
                listOfUsers = unitOfWork.QuarterlyIncentive.Get().Where(s => s.RI_IncentiveCategory.Contains(years) && s.RI_IncentiveCategory.Contains(quaterly) && s.RI_EmpCd == employeeId).ToList();
                return PartialView("GetQuarterlyListPartial", listOfUsers);
            }

        }

        [HttpPost]
        public async Task<ActionResult> ImportFile(HttpPostedFileBase importFile)
        {

            RIC_DBEntities context = new RIC_DBEntities();
            //IEnumerable<RIC_QuarterlyIncentive> listOfUsers;
            if (importFile == null) return Json(new { Status = 0, Message = "No File Selected" });

            try
            {

                var fileData = GetDataFromCSVFile(importFile.InputStream);

                var dtEmployee = fileData.ToDataTable();
                DataTable dr = dtEmployee;
                var tblEmployeeParameter = new SqlParameter("@tbl_type_RIC_QuarterlyIncentive", SqlDbType.Structured)
                {
                    TypeName = "dbo.tbl_type_RIC_QuarterlyIncentive",
                    Value = dtEmployee
                };

                var data = fileData.GroupBy(s => s.RI_StartDate).Select(group => group.Take(1));
                //var year change code 
                int year = 0;
                int quarter = 0;
                foreach (var item in data)
                {


                    var getdata = from r in item
                                  select new
                                  {
                                      year = r.RI_StartDate.Year,

                                      quater = (r.RI_StartDate.Month + 2) / 3
                                  };
                    var result = getdata.ToList();
                    year = result[0].year;
                    quarter = result[0].quater;

                }




                var listOfUsers = unitOfWork.QuarterlyIncentive.Get().Where(s => Convert.ToDateTime(s.RI_StartDate).Year == year && (Convert.ToDateTime(s.RI_StartDate).Month + 2) / 3 == quarter).ToList();
                if (listOfUsers.Count > 0)
                {
                    return Json(new { Status = 2, Message = "Quarterly Incentive for Year " + year + " Q" + quarter + " already exists! " });


                }
                else
                {
                    await context.Database.ExecuteSqlCommandAsync("EXEC spBulkImportQuarterlyIncentive @tbl_type_RIC_QuarterlyIncentive", tblEmployeeParameter);
                    return Json(new { Status = 1, Message = "File Imported Successfully " });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, "IncentivesController");
                return Json(new { Status = 0, Message = ex.Message });

            }
        }

        [HttpGet]

        public ActionResult GetUploadData()
        {
            RIC_DBEntities context = new RIC_DBEntities();
            System.Data.DataSet ds = new System.Data.DataSet();
            var command = context.Database.Connection.CreateCommand();
            context.Database.Connection.Open();
            var con = context.Database.Connection;
            command.CommandText = "[dbo].[SP_get_uploaded_dt]";
            command.CommandType = CommandType.StoredProcedure;
            var adapter = new SqlDataAdapter(command.CommandText, (SqlConnection)con);
            adapter.Fill(ds);
            DataTable dt = new DataTable();
            dt = ds.Tables[0];
            return PartialView("ImportFilePartial", dt);
        }
        public int GetQuarter(DateTime date)
        {
            return (date.Month + 2) / 3;
        }
        private List<RIC_QuarterlyIncentive> GetDataFromCSVFile(Stream stream)
        {
            // DataTable dt = new DataTable();
            DataTable dt_ = new DataTable();

            //if you want to read data from a excel file use this

            ExcelPackage xlPackage = new ExcelPackage();
            xlPackage.Load(stream);
            if (xlPackage.Workbook.Worksheets.Count > 0)
            {
                using (ExcelWorksheet myWorksheet = xlPackage.Workbook.Worksheets.First())
                {
                    var totalRows = myWorksheet.Dimension.End.Row;
                    var totalColumns = myWorksheet.Dimension.End.Column;
                    int rowIndex = 1;
                    for (int c = 1; c <= totalColumns; c++)
                    {
                        dt_.Columns.Add(myWorksheet.Cells[rowIndex, c].Text);
                    }
                    rowIndex = 2;
                    for (int r = rowIndex; r <= totalRows; r++)
                    {
                        DataRow dr = dt_.NewRow();
                        for (int c = 1; c <= totalColumns; c++)
                        {
                            dr[c - 1] = myWorksheet.Cells[r, c].Value;
                        }
                        dt_.Rows.Add(dr);
                    }

                }
            }
            var empList = new List<RIC_QuarterlyIncentive>();
            try
            {
                DataTable tmp = dt_;
                DateTime TempDt;
                float Tempf;
                Session["tmpdata"] = tmp;
                DateTime dateTime = new DateTime(1900, 1, 1);
                foreach (DataRow objDataRow in tmp.Rows)
                {

                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                    RIC_QuarterlyIncentive rIC_QuarterlyIncentive = new RIC_QuarterlyIncentive();
                    rIC_QuarterlyIncentive.RI_EmpCd = Convert.ToString(objDataRow["EMP Code"]);
                    rIC_QuarterlyIncentive.RI_EmployeeName = Convert.ToString(objDataRow["EMP Name"]);
                    rIC_QuarterlyIncentive.RI_TeamLead = Convert.ToString(objDataRow["Team Lead"]);
                    rIC_QuarterlyIncentive.RI_Candidate = Convert.ToString(objDataRow["Candidate"]);
                    rIC_QuarterlyIncentive.RI_Company = Convert.ToString(objDataRow["Company"]);
                    if (DateTime.TryParse(objDataRow["Start Date"].ToString(), out TempDt))
                    {
                        rIC_QuarterlyIncentive.RI_StartDate = TempDt;
                    }
                    else
                    {
                        rIC_QuarterlyIncentive.RI_StartDate = dateTime;
                    }

                    if (DateTime.TryParse(objDataRow["End Date"].ToString(), out TempDt))
                    {
                        rIC_QuarterlyIncentive.RI_EndDate = TempDt;
                    }

                    if (float.TryParse(objDataRow["Net Margin"].ToString(), out Tempf))
                    {
                        rIC_QuarterlyIncentive.RI_NetMargin = Tempf;
                    }
                    if (float.TryParse(objDataRow["Incentive Amount"].ToString(), out Tempf))
                    {
                        rIC_QuarterlyIncentive.RI_Incentives = Tempf;
                    }

                    rIC_QuarterlyIncentive.RI_Remarks = Convert.ToString(objDataRow["Remarks"]);
                    rIC_QuarterlyIncentive.RI_IncentiveCategory = Convert.ToString(objDataRow["Incentive Category"]);
                    empList.Add(rIC_QuarterlyIncentive);


                }

            }
            catch (Exception)
            {
                throw;
            }


            return empList;
        }
    }
    public static class Extensions
    {
        public static DataTable ToDataTable<T>(this List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties  
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table   
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows  
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}
