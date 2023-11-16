using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using DBLibrary;
using OfficeOpenXml.FormulaParsing.Excel.Operators;
using System.Web.Security;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Globalization;

namespace RIC.Controllers
{
    public class ImportFilesController : Controller
    {

        RIC_DBEntities context = new RIC_DBEntities();
        string j1timeout = System.Configuration.ConfigurationManager.AppSettings["J1Time"];
        string callTime = System.Configuration.ConfigurationManager.AppSettings["CallTime"];

        // GET: /ImportFiles/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CallStatistics()
        {
            return View();
        }
        public ActionResult ImportFile(HttpPostedFileBase importFile)
        {

            int? totalrecords = null;
            int? newrecords = null;
            string empCd = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;


            if (importFile == null) return Json(new { Status = 0, Message = "No File Selected" });

            try
            {
                DataTable dtCsv = ConvertCSVtoDataTable(importFile.InputStream);

                DataTable dtTemp;
                foreach (DataRow item in dtCsv.Rows)
                {
                    dtTemp = dtCsv.Clone();
                    dtTemp.ImportRow(item);


                    DateTime TempDateTime;  //This is to fix Date of this format '06/12/2023 00:46 PM', which will be rejected by sql.
                    if (DateTime.TryParse(dtTemp.Rows[0]["Date Issued"].ToString(), out TempDateTime))
                    {
                        dtTemp.Rows[0]["Date Issued"] = TempDateTime;
                    }
                    else
                    {
                        dtTemp.Rows[0]["Date Issued"] = null;
                    }
                    if (DateTime.TryParse(dtTemp.Rows[0]["Start Date"].ToString(), out TempDateTime))
                    {
                        dtTemp.Rows[0]["Start Date"] = TempDateTime;
                    }
                    else
                    {
                        dtTemp.Rows[0]["Start Date"] = null;
                    }
                    if (DateTime.TryParse(dtTemp.Rows[0]["End Date"].ToString(), out TempDateTime))
                    {
                        dtTemp.Rows[0]["End Date"] = TempDateTime;
                    }
                    else
                    {
                        dtTemp.Rows[0]["End Date"] = null;
                    }
                    if (DateTime.TryParse(dtTemp.Rows[0]["Submit Date"].ToString(), out TempDateTime))
                    {
                        dtTemp.Rows[0]["Submit Date"] = TempDateTime;
                    }
                    else
                    {
                        dtTemp.Rows[0]["Submit Date"] = null;
                    }
                    if (DateTime.TryParse(dtTemp.Rows[0]["Interview Date"].ToString(), out TempDateTime))
                    {
                        dtTemp.Rows[0]["Interview Date"] = TempDateTime;
                    }
                    else
                    {
                        dtTemp.Rows[0]["Interview Date"] = null;
                    }
                    if (DateTime.TryParse(dtTemp.Rows[0]["Hire Date"].ToString(), out TempDateTime))
                    {
                        dtTemp.Rows[0]["Hire Date"] = TempDateTime;
                    }
                    else
                    {
                        dtTemp.Rows[0]["Hire Date"] = null;
                    }
                    if (DateTime.TryParse(dtTemp.Rows[0]["Rejection Date"].ToString(), out TempDateTime))
                    {
                        dtTemp.Rows[0]["Rejection Date"] = TempDateTime;
                    }
                    else
                    {
                        dtTemp.Rows[0]["Rejection Date"] = null;
                    }
                    dtTemp.Columns.Add("Rec_Create_Date");
                    if (DateTime.TryParse(dtTemp.Rows[0]["Rec_Create_Date"].ToString(), out TempDateTime))
                    {
                        dtTemp.Rows[0]["Rec_Create_Date"] = TempDateTime;
                    }
                    else
                    {

                        dtTemp.Rows[0]["Rec_Create_Date"] = DateTime.Now;
                    }
                    dtTemp.Columns.Add("Submit_Seq");
                    dtTemp.Columns.Add("Interview_Seq");
                    dtTemp.Columns.Add("Hire_Seq");


                    var tblEmployeeParameter = new SqlParameter("@TAB_MasterAll_J1_Details", SqlDbType.Structured)
                    {

                        SqlDbType = SqlDbType.Structured,
                        TypeName = "dbo.tbl_type_TAB_MasterAll_J1_Details_Temp",
                        Value = dtTemp
                    };
                    try
                    {
                        if (dtTemp.Rows[0]["Date Issued"].ToString() != "Total")
                        {
                            context.Database.CommandTimeout = int.Parse(j1timeout);
                            context.Database.ExecuteSqlCommand("EXEC sp_BulkImport_TAB_MasterAll_J1_Details_Temp @TAB_MasterAll_J1_Details", tblEmployeeParameter);
                        }
                    }
                    catch (SqlException ex)
                    {
                        ExceptionLogging.SendErrorToText(ex, "ImportFilesController");
                        try
                        {

                            DateTime dateTime1 = Convert.ToDateTime(dtTemp.Rows[0]["Issued_Date"]);
                            DateTime dateTime2 = Convert.ToDateTime(dtTemp.Rows[0]["Start_Date"]);
                            DateTime dateTime3 = Convert.ToDateTime(dtTemp.Rows[0]["End_Date"]);
                            DateTime dateTime4 = Convert.ToDateTime(dtTemp.Rows[0]["Submit_Date"]);
                            DateTime dateTime5 = Convert.ToDateTime(dtTemp.Rows[0]["Interview_Date"]);
                            DateTime dateTime6 = Convert.ToDateTime(dtTemp.Rows[0]["Hire_Date"]);
                            DateTime dateTime7 = Convert.ToDateTime(dtTemp.Rows[0]["Rejection_Date"]);
                            DateTime dateTime8 = Convert.ToDateTime(dtTemp.Rows[0]["Rec_Create_Date"]);

                        }
                        catch (Exception extemp)
                        {
                            ExceptionLogging.SendErrorToText(extemp, "ImportFilesController");
                        }
                    }
                    dtTemp = null;
                }

                try
                {

                    System.Data.DataSet ds = new System.Data.DataSet();
                    var commands = context.Database.Connection.CreateCommand();
                    context.Database.Connection.Open();
                    commands.CommandText = "[dbo].[sp_Update_TAB_MasterAll_J1_Details_v3]";
                    commands.CommandType = CommandType.StoredProcedure;
                    commands.CommandTimeout = int.Parse(j1timeout); 
                    SqlParameter cmd = new SqlParameter("@Uploaded_By", empCd);
                    commands.Parameters.Add(cmd);
                    var reader = commands.ExecuteReader();
                    while (!reader.IsClosed)
                        ds.Tables.Add().Load(reader);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        totalrecords = (int)ds.Tables[0].Rows[0][0];
                        newrecords = (int)ds.Tables[0].Rows[0][1];
                    }

                }
                catch (SqlException ex)
                {
                    ExceptionLogging.SendErrorToText(ex, "ImportFilesController");
                    return Json(new { Status = 0, Message = ex.Message });
                }
                return Json(new { Status = 1, TotalRecords = totalrecords, NewRecords = newrecords, Message = "File imported successfully." });


            }

            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, "ImportFilesController");
                return Json(new { Status = 0, Message = ex.Message });
            }
        }
        public static DataTable ConvertCSVtoDataTable(Stream stream)
        {

            DataTable dtCsv = new DataTable();
            using (StreamReader streamReader = new StreamReader(stream))
            {
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet myWorksheet = pck.Workbook.Worksheets.Add("sheet1");
                ExcelTextFormat excelTextFormat = new OfficeOpenXml.ExcelTextFormat();
                excelTextFormat.Delimiter = ',';
                excelTextFormat.TextQualifier = '"';
                excelTextFormat.DataTypes = new[] { eDataTypes.String };

                myWorksheet.Cells.LoadFromText(streamReader.ReadToEnd(), excelTextFormat, TableStyles.None, true);

                int totalRows = myWorksheet.Dimension.End.Row;
                int totalColumns = myWorksheet.Dimension.End.Column;
                int rowIndex = 1;
                for (int c = 1; c <= totalColumns; c++)
                {
                    dtCsv.Columns.Add(myWorksheet.Cells[rowIndex, c].Text);

                }
                rowIndex = 2;
                for (int r = rowIndex; r <= totalRows; r++)
                {
                    DataRow dr = dtCsv.NewRow();
                    for (int c = 1; c <= totalColumns; c++)
                    {
                        dr[c - 1] = myWorksheet.Cells[r, c].Value;
                    }

                    dtCsv.Rows.Add(dr);
                }

                dtCsv.Columns["Submitted By Email ID"].SetOrdinal(74);
                dtCsv.Columns["Comments"].SetOrdinal(74);
            }


            return dtCsv;
        }

        public ActionResult getAuditDetails()
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            var command = context.Database.Connection.CreateCommand();
            context.Database.Connection.Open();
            var con = context.Database.Connection;
            command.CommandText = "select MJAL_Updt_By,MJAL_Updt_Dt from MasterAll_J1_Audit_Log";
            command.CommandType = CommandType.StoredProcedure;
            var adapert = new SqlDataAdapter(command.CommandText, (SqlConnection)con);
            adapert.Fill(ds);
            DataTable dt = new DataTable();
            dt = ds.Tables[0];
            return View("ImportFilePartial", dt);
        }

        public JsonResult CallProccesser(HttpPostedFileBase importFile)
        {
            List<string> name = new List<string>();
            var TotalRecords = 0;
            string extname = Path.GetFileNameWithoutExtension(importFile.FileName);
            try
            {

                DataTable dt = new DataTable();
                Stream stream = importFile.InputStream;
                //DateTime ct1;
                //DateTime ct = DateTime.Now;
                //CultureInfo cultureInfo1 = CultureInfo.CreateSpecificCulture("en-US");
                var t = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd, HH:mm:ss.fff"));
                // var t1= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                //var t1 = ct.ToString("MM/dd/yyyy HH:mm:ss");
                //var t2=DateTime.Parse(t1);
                //DateTime.TryParse(t1, cultureInfo1, DateTimeStyles.AssumeLocal, out ct1);
                //var t3=new DateTime(ct.Year,ct.Month,ct.Day,ct.Hour,ct.Minute,ct.Second);
                //var t4=Convert.ToDateTime(t1);
                ExcelPackage xlPackage = new ExcelPackage();
                //ExcelPackage xlPackage = new ExcelPackage(importFile.InputStream, "");
                //ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
                // ExcelPackage xlPackage = new ExcelPackage(new FileInfo(importFile.FileName));
                xlPackage.Load(stream);
                //OfficeOpenXml.ExcelWorksheet sheet = xlPackage.Workbook.Worksheets.First();



                try
                {
                    List<RIC_Call_Statistics> CallList = new List<RIC_Call_Statistics>();
                    List<RIC_CallStatisticError> Callerror = new List<RIC_CallStatisticError>();
                    List<RIC_Employee> employees = context.RIC_Employee.ToList();
                    CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
                    if (extname == "8X8")
                    {
                        if (xlPackage.Workbook.Worksheets.Count > 0)
                        {
                            using (ExcelWorksheet myWorksheet = xlPackage.Workbook.Worksheets.First())
                            {

                                //TimeSpan Hr;
                                var totalRows = myWorksheet.Dimension.End.Row;
                                var totalColumns = myWorksheet.Dimension.End.Column;
                                int rowIndex = 1;
                                for (int c = 1; c <= totalColumns; c++)
                                {
                                    dt.Columns.Add(myWorksheet.Cells[rowIndex, c].Text);
                                }
                                rowIndex = 2;
                                for (int r = rowIndex; r <= totalRows; r++)
                                {
                                    DataRow dr = dt.NewRow();
                                    for (int c = 1; c <= totalColumns; c++)
                                    {
                                        dr[c - 1] = myWorksheet.Cells[r, c].Value;
                                        //TimeSpan.TryParse(myWorksheet.Cells[r, c].Value.ToString(),out Hr);

                                    }
                                    dt.Rows.Add(dr);
                                }

                            }
                        }
                        if (dt.Rows.Count > 0)
                        {

                            DateTime CallDateTime;
                            //int RecordCtr = 0;

                            foreach (DataRow row in dt.Rows)
                            {
                                string EmpName = row["Caller Name"].ToString().Trim().ToLower();
                                if (!string.IsNullOrEmpty(EmpName))
                                {
                                    RIC_Employee rIC_Employee = employees.Where(e => (e.RE_Shortel_Name != null ? e.RE_Shortel_Name.Trim().ToLower() : "") == EmpName).FirstOrDefault();
                                    if (rIC_Employee != null)
                                    {

                                        if (DateTime.TryParse(row["Start Time"].ToString(), cultureInfo, DateTimeStyles.None, out CallDateTime))
                                        {
                                            TimeSpan CallTime = TimeSpan.Parse(row["Talk Time"].ToString());
                                            int callconnected = 0;
                                            int voicemessage = 0;
                                            if (CallTime.Minutes >= 1)
                                            {
                                                callconnected = 1;
                                            }
                                            else if (CallTime.Minutes < 1)
                                            {
                                                voicemessage = 1;
                                            }
                                            CallList.Add(new RIC_Call_Statistics
                                            {
                                                RC_Emp_Cd = rIC_Employee.RE_Emp_Cd,
                                                RC_Date = CallDateTime,
                                                RC_Time = CallDateTime.TimeOfDay,
                                                RC_Duration = CallTime,
                                                RC_Call_Connected = callconnected,
                                                RC_Voice_Message = voicemessage,
                                                RC_Calling = row["Callee"].ToString(),
                                                RC_CallType = row["Direction"].ToString() == "Incoming" ? "In" : "Out",
                                                RC_Dailed = row["Caller"].ToString(),
                                                RC_UploadedDate = DateTime.Now,
                                                RC_Source = extname == "8X8" ? "8x8" : null
                                            });
                                        }
                                    }
                                    else
                                    {
                                        name.Add(EmpName);
                                    }
                                }
                            }
                            var mind = CallList.Min(a => a.RC_Date);
                            var maxd = CallList.Max(a => a.RC_Date);
                            context.Database.CommandTimeout = int.Parse(callTime);
                            var emp = context.RIC_Call_Statistics.Where(a => a.RC_Date >= mind && a.RC_Date <= maxd && a.RC_Source == "8x8");
                            context.RIC_Call_Statistics.RemoveRange(emp);
                            context.SaveChanges();
                            string xyz = string.Join(";", name.Distinct().ToArray());
                            int x = name.Distinct().Count();
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.Configuration.ValidateOnSaveEnabled = false;                          
                            context.RIC_Call_Statistics.AddRange(CallList);
                            context.ChangeTracker.DetectChanges();
                            context.SaveChanges();
                        }
                    }
                    else if (extname.ToLower().Trim() == "ShoreTelReports".ToLower())
                    {
                        if (xlPackage.Workbook.Worksheets.Count > 0)
                        {
                            using (ExcelWorksheet myWorksheet = xlPackage.Workbook.Worksheets.First())
                            {
                                Double noDays = 0;
                                //TimeSpan Hr;
                                var totalRows = myWorksheet.Dimension.End.Row;
                                var totalColumns = myWorksheet.Dimension.End.Column;
                                int rowIndex = 1;
                                for (int c = 1; c <= totalColumns; c++)
                                {
                                    dt.Columns.Add(myWorksheet.Cells[rowIndex, c].Text);
                                }
                                rowIndex = 2;
                                for (int r = rowIndex; r <= totalRows; r++)
                                {
                                    DataRow dr = dt.NewRow();
                                    for (int c = 1; c <= totalColumns; c++)
                                    {
                                        dr[c - 1] = myWorksheet.Cells[r, c].Value;
                                        if (c == 6 && myWorksheet.Cells[r, c].Value != null)
                                        {
                                            string nud = myWorksheet.Cells[r, c].Value.ToString();
                                            if (Double.TryParse(nud, out noDays))
                                                dr[c - 1] = DateTime.FromOADate(noDays);
                                        }
                                        if (c == 8 && myWorksheet.Cells[r, c].Value != null)
                                        {
                                            //if (TimeSpan.TryParse(myWorksheet.Cells[r, c].Value.ToString(), out Hr))
                                            //    dr[c - 1] = Hr;
                                            string nud = myWorksheet.Cells[r, c].Value.ToString();
                                            if (Double.TryParse(nud, out noDays))
                                                dr[c - 1] = DateTime.FromOADate(noDays).TimeOfDay;
                                        }
                                        //TimeSpan.TryParse(myWorksheet.Cells[r, c].Value.ToString(),out Hr);

                                    }
                                    dt.Rows.Add(dr);
                                }

                            }
                        }
                        DataTable sh_dt = new DataTable();
                        sh_dt.Columns.Add("PRI");
                        sh_dt.Columns.Add("Date");
                        sh_dt.Columns.Add("Time");
                        sh_dt.Columns.Add("In/Out");
                        sh_dt.Columns.Add("Dialed");
                        sh_dt.Columns.Add("Calling");
                        sh_dt.Columns.Add("User");
                        sh_dt.Columns.Add("Duration");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            DataRow dr1 = sh_dt.NewRow();
                            dr1[0] = dt.Rows[i]["Column1"];
                            dr1[1] = dt.Rows[i]["Column5"];
                            dr1[2] = dt.Rows[i]["Column7"];
                            dr1[3] = dt.Rows[i]["Column9"];
                            dr1[4] = dt.Rows[i]["Column12"];
                            dr1[5] = dt.Rows[i]["Column15"];
                            dr1[6] = dt.Rows[i]["Column22"];
                            dr1[7] = dt.Rows[i]["Column29"];
                            sh_dt.Rows.Add(dr1);
                        }

                        if (sh_dt.Rows.Count > 0)
                        {
                            DateTime CallDateTime;
                            //int RecordCtr = 0;
                            foreach (DataRow row in sh_dt.Rows)
                            {
                                try
                                {
                                    //int a = 0;
                                    //int b = 1;
                                    //int c=b/a;
                                    string EmpName = row["User"].ToString().Trim().ToLower();
                                    if (!string.IsNullOrEmpty(EmpName))
                                    {
                                        RIC_Employee rIC_Employee = employees.Where(e => (e.RE_Shortel_Name != null ? e.RE_Shortel_Name.Trim().ToLower() : "") == EmpName).FirstOrDefault();
                                        if (rIC_Employee != null)
                                        {
                                            if (DateTime.TryParse(row["Date"].ToString(), cultureInfo, DateTimeStyles.None, out CallDateTime))
                                            {
                                                TimeSpan CallTime = TimeSpan.Parse(row["Time"].ToString());
                                                TimeSpan Duration = TimeSpan.Parse(row["Duration"].ToString());
                                                int callconnected = 0;
                                                int voicemessage = 0;
                                                if (Duration.Minutes >= 1)
                                                {
                                                    callconnected = 1;
                                                }
                                                else if (Duration.Minutes < 1)
                                                {
                                                    voicemessage = 1;
                                                }
                                                CallList.Add(new RIC_Call_Statistics
                                                {
                                                    RC_Emp_Cd = rIC_Employee.RE_Emp_Cd,
                                                    RC_Date = CallDateTime,
                                                    RC_Time = CallTime,
                                                    RC_Duration = Duration,
                                                    RC_Call_Connected = callconnected,
                                                    RC_Voice_Message = voicemessage,
                                                    RC_Calling = row["Calling"].ToString(),
                                                    RC_CallType = row["In/Out"].ToString() == "In" ? "In" : "Out",
                                                    RC_Dailed = row["Dialed"].ToString(),
                                                    RC_PRI = row["PRI"].ToString(),
                                                    RC_UploadedDate = DateTime.Now,
                                                    RC_Source = extname == "8X8" ? "8x8" : null
                                                }); ; ;
                                            }
                                        }
                                        else
                                        {
                                            name.Add(EmpName);
                                        }
                                    }
                                }
                                catch (Exception ric_callstatisricserror)
                                {
                                    ExceptionLogging.SendErrorToText(ric_callstatisricserror, "ImportFilesController");
                                    string EmpName = row["User"].ToString().Trim().ToLower();
                                    RIC_Employee rIC_Employee = new RIC_Employee();
                                    if (!string.IsNullOrEmpty(EmpName))
                                    {
                                        rIC_Employee = employees.Where(e => e.RE_Employee_Name.ToLower().ToString() == EmpName.ToLower().ToString()).FirstOrDefault();
                                    }

                                    Callerror.Add(new RIC_CallStatisticError
                                    {
                                        RE_Emp_Cd = rIC_Employee == null || rIC_Employee.RE_Emp_Cd == null ? "0001" : rIC_Employee.RE_Emp_Cd,
                                        PRI = row["PRI"].ToString(),
                                        CallDurationString = row["Calling"].ToString(),
                                        ShName = EmpName,
                                        CallType = row["In/Out"].ToString(),
                                        DateString = row["Date"].ToString(),
                                        TimeString = row["Time"].ToString(),
                                        Calling = row["Calling"].ToString(),
                                        Dialed = row["Dialed"].ToString(),
                                        ErrorCode = 001,
                                        ErrorColumn = 1
                                    }
                                    );
                                    context.RIC_CallStatisticErrors.AddRange(Callerror);

                                    try
                                    {
                                        // Your code...
                                        // Could also be before try if you know the exception occurs in SaveChanges

                                        context.SaveChanges();
                                    }
                                    catch (System.Data.Entity.Validation.DbEntityValidationException e)
                                    {
                                        ExceptionLogging.SendErrorToText(e, "ImportFilesController");
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
                                        throw;
                                    }
                                }
                            }
                            var mind = CallList.Min(a => a.RC_Date);
                            var maxd = CallList.Max(a => a.RC_Date);
                            context.Database.CommandTimeout = int.Parse(callTime);
                            var emp = context.RIC_Call_Statistics.Where(a => a.RC_Date >= mind && a.RC_Date <= maxd && a.RC_Source == null);
                            context.RIC_Call_Statistics.RemoveRange(emp);
                            context.SaveChanges();
                            string xyz = string.Join(";", name.Distinct().ToArray());
                            int x = name.Distinct().Count();
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.Configuration.ValidateOnSaveEnabled = false;
                            context.RIC_Call_Statistics.AddRange(CallList);
                            context.ChangeTracker.DetectChanges();
                            context.SaveChanges();
                            TotalRecords = CallList.Count;
                            //writelog(name.Distinct().ToList());
                            // DownloadFile(name.Distinct().ToList());
                        }
                    }

                }
                catch (Exception ex)
                {
                    ExceptionLogging.SendErrorToText(ex, "ImportFilesController");
                    return Json(new { Status = 0, Message = ex.Message });
                }

            }
            catch (Exception extemp)
            {
                ExceptionLogging.SendErrorToText(extemp, "ImportFilesController");
                return Json(new { Status = 0, Message = extemp.Message });
            }
            List<string> names = name.Distinct().ToList();
            if (extname == "8X8")
            {
                return Json(new { Status = 1, Message = "File Imported Successfully" });
            }
            else
            {
                return Json(new { names, Status = 2, Message = "File Imported Successfully", TotalRecords }, JsonRequestBehavior.AllowGet);
                // return Content("This is some text.", "text/plain");
            }
        }
        private Tuple<DateTime, bool> FormateDate(string stringDate)
        {
            Tuple<DateTime, bool> result = default(Tuple<DateTime, bool>);
            var splitDate = stringDate.Split('/');
            if (splitDate.Count() == 3)
            {
                var year = Convert.ToInt32(splitDate[2].Substring(0, 4));
                var month = Convert.ToInt32(splitDate[0]);
                var day = Convert.ToInt32(splitDate[1]);
                var time = splitDate[2].Substring(5);
                var hr = Convert.ToInt32(time.Substring(0, 2));
                var min = Convert.ToInt32(time.Substring(3, 2));
                var sec = Convert.ToInt32(time.Substring(6, 2));
                result = new Tuple<DateTime, bool>(new DateTime(year, month, day, hr, min, sec), true);



            }
            return result;
        }


    }




}
