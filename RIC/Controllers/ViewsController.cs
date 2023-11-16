using DBLibrary;
using DBLibrary.UnitOfWork;
using OfficeOpenXml;
using PagedList;
using RIC.CustomHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace RIC.Controllers
{
    public class SQLViewsController : Controller
    {
        PageListOfDataRow PageDataRow = new PageListOfDataRow();
        //
        // GET: /View list
        [Authorize]
        public ActionResult GetViews(string Value)
        {
            UnitOfWork unitOfWork = new UnitOfWork();

            Type ResultType = GetSQLQueryReturnType.DynamicSqlQuery(unitOfWork.context.Database, "EXEC SP_GetViews");
            var RMSViews = unitOfWork.ExecuteRawQuery(ResultType, "EXEC SP_GetViews");

            if (Value == null)
            {
                return View(RMSViews);
            }
            else
            {
                return View("GetSearchScr", RMSViews);
            }
        }

        //Get view column details.
        [Authorize]
        public JsonResult GETViewDetails(string SQlView)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            Dictionary<string, object> Dist = new Dictionary<string, object>();
            Dist.Add("@ViewName", SQlView);
            Type ResultType = GetSQLQueryReturnType.DynamicSqlQuery(unitOfWork.context.Database, "EXEC SP_GETViewDetails @ViewName", Dist);
            IEnumerable Parametrs = unitOfWork.ExecuteRawQuery(ResultType, "EXEC SP_GETViewDetails @ViewName", new SqlParameter("ViewName", SQlView));

            return Json(Parametrs, JsonRequestBehavior.AllowGet);
        }

        // Save search constraints to master table.
        [Authorize]
        [HttpPost]
        public ActionResult SaveViewDetails(List<RMS_ViewsConfig> ViewParameters)
        {
            UnitOfWork unitOfWork = new UnitOfWork();

            if (ViewParameters != null)
            {
                string ViewName = ViewParameters[0].RV_ViewName;
                IEnumerable<RMS_ViewsConfig> entities = unitOfWork.ViewsConfig.Get(e => e.RV_ViewName == ViewName);

                if (entities != null)
                {
                    foreach (var item in entities)
                    {
                        unitOfWork.ViewsConfig.Delete(item);

                    }
                    unitOfWork.Save();

                    foreach (var item in ViewParameters)
                    {
                        unitOfWork.ViewsConfig.Insert(item);
                    }
                    unitOfWork.Save();
                }
            }
            return Json(new { Success = true, Controller = "SQLViews", Action = "GetViews" });
        }

        //Clear all serach constraints from master table.
        [Authorize]
        public JsonResult ClearParameters(string DeleteView)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            IEnumerable<RMS_ViewsConfig> entities = unitOfWork.ViewsConfig.Get(e => e.RV_ViewName == DeleteView);

            if (entities != null)
            {
                foreach (var item in entities)
                {
                    unitOfWork.ViewsConfig.Delete(item);

                }
                unitOfWork.Save();
            }
            return Json(new { Success = true, Controller = "SQLViews", Action = "GetViews" }, JsonRequestBehavior.AllowGet);
        }


        //Partial for Search screen constraints
        [Authorize]
        public PartialViewResult SearchParameters_Partial(string View)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            Dictionary<string, object> Dist = new Dictionary<string, object>();
            Dist.Add("@ViewName", View);
            Type ResultType = GetSQLQueryReturnType.DynamicSqlQuery(unitOfWork.context.Database, "EXEC SP_GETViewDetails @ViewName", Dist);
            IEnumerable Parametrs = unitOfWork.ExecuteRawQuery(ResultType, "EXEC SP_GETViewDetails @ViewName", new SqlParameter("ViewName", View));

            return PartialView(Parametrs);
        }

        //Partial for search data
        [Authorize]
        [HttpPost]
        public PartialViewResult GetViewSearch_Partial(string SqlView, IEnumerable<RMS_ViewConstraints> ViewConstraints, string page)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            IEnumerable ViewObj = null;
            if (SqlView != null && ViewConstraints != null)
            {

                StringBuilder Parameters = new StringBuilder();
                int ctr = 1;
                foreach (RMS_ViewConstraints item in ViewConstraints.Where(i => i.ControlDataType.ToLower().Contains("date")))
                {
                    if (item.ControlValue != null)
                    {
                        if (ctr == 1)
                        {
                            // Build From date.
                            Parameters.Append(item.ControlName);
                            Parameters.Append(" BETWEEN ");
                            Parameters.Append("'" + Convert.ToDateTime(item.ControlValue) + "'");
                            ctr++;
                        }
                        else if (ctr == 2)
                        {
                            //Build To date
                            Parameters.Append(" AND ");
                            Parameters.Append("'" + Convert.ToDateTime(item.ControlValue).AddHours(23).AddMinutes(59).AddSeconds(59) + "'").AppendLine(" AND ");
                            ctr = 1;
                        }
                    }
                }

                foreach (RMS_ViewConstraints item in ViewConstraints.Where(i => !i.ControlDataType.ToLower().Contains("date")))
                {
                    if (item.ControlValue != null && item.ControlType != "Select")
                    {
                        Parameters.Append("[" + item.ControlName.Trim() + "]");
                        Parameters.Append(" Like ");
                        Parameters.Append("'%" + item.ControlValue.Trim() + "%'").AppendLine(" AND ");
                    }
                    else if (item.ControlValue != null)
                    {
                        Parameters.Append("[" + item.ControlName.Trim() + "]");
                        Parameters.Append(" = ");
                        Parameters.Append("'" + item.ControlValue.Trim() + "'").AppendLine(" AND ");

                    }
                }

                Type ResultType = GetSQLQueryReturnType.DynamicSqlQuery(unitOfWork.context.Database, "SELECT TOP 0 * FROM " + SqlView);
                //Remove trailing "AND"
                ViewObj = unitOfWork.ExecuteRawQuery(ResultType, "SELECT * FROM " + SqlView + " WHERE " + Parameters.ToString().Trim().Remove(Parameters.ToString().Trim().Length - 3, 3));
            }
            else if (SqlView != null && ViewConstraints == null)
            {
                Type ResultType = GetSQLQueryReturnType.DynamicSqlQuery(unitOfWork.context.Database, "SELECT TOP 0 * FROM " + SqlView);
                //Remove trailing "AND"
                ViewObj = unitOfWork.ExecuteRawQuery(ResultType, "SELECT * FROM " + SqlView);
            }

            if (ViewObj != null)
             {
                DataTable dtVM = new DataTable();
                DataRow dr;

                if (page == null)
                {
                    bool RanOnce = false;
                    foreach (var item in ViewObj)
                    {
                        dr = dtVM.NewRow();
                        foreach (PropertyInfo Prop in item.GetType().GetRuntimeProperties().ToArray())
                        {
                            if (!RanOnce)
                            {
                                dtVM.Columns.Add(Prop.Name);
                            }

                            dr[Prop.Name] = item.GetType().GetProperty(Prop.Name).GetValue(item);
                        }
                        RanOnce = true;
                        dtVM.Rows.Add(dr);
                    }
                    page = "1";
                    Session["ViewData"] = dtVM;
                    Session["ViewName"] = SqlView;
                }
                else
                {
                    dtVM = Session["ViewData"] as DataTable;
                }

                int PageNumber = Convert.ToInt32(page);

                PageDataRow.list = dtVM.AsEnumerable().ToList();
                PageDataRow.plist = new PagedList<DataRow>(PageDataRow.list, PageNumber, 10);

                DataTable filter = dtVM.Rows.Count > 0 ? dtVM.AsEnumerable().Skip(PageNumber * 10 - 10).Take(10).CopyToDataTable() : dtVM;



                ViewBag.ViewData = PageDataRow.plist;
                ViewBag.PageNumber = PageNumber;
                ViewBag.PageCount = PageDataRow.plist.PageCount;

                return PartialView(filter);
            }
            return PartialView();
        }

        [Authorize]
        public ActionResult ExportToExcel()
        {
            string ViewName = Session["ViewName"].ToString() + ".xlsx";
            using (ExcelPackage pck = new ExcelPackage())
            {
                DataTable dt = Session["ViewData"] as DataTable;
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ViewReport");
                ws.Cells["A1"].LoadFromDataTable(dt, true); //You can Use TableStyles property of your desire.    
                                                            //Read the Excel file in a byte array    
                Byte[] fileBytes = pck.GetAsByteArray();

                var response = new FileContentResult(fileBytes, "application/octet-stream");
                response.FileDownloadName = ViewName;// set the file name
                return response;// download file.
            }


        }

        [Authorize]
        public JsonResult GetViewsList(string Value,string ViewName)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            
            if (Value == null)
            {
                Type ResultType = GetSQLQueryReturnType.DynamicSqlQuery(unitOfWork.context.Database, "EXEC SP_GetViews_vwLK");
                var RMSViews = unitOfWork.ExecuteRawQuery(ResultType, "EXEC SP_GetViews_vwLK");
                return Json(RMSViews, JsonRequestBehavior.AllowGet);

            }
            else if(ViewName == null && Value != null)
            {
                Dictionary<string, object> Dist = new Dictionary<string, object>();
                Dist.Add("@ViewName", Value);
                Type ResultType = GetSQLQueryReturnType.DynamicSqlQuery(unitOfWork.context.Database, "EXEC SP_GETViewDetails @ViewName", Dist);
                IEnumerable Parametrs = unitOfWork.ExecuteRawQuery(ResultType, "EXEC SP_GETViewDetails @ViewName", new SqlParameter("ViewName", Value));
                return Json(Parametrs, JsonRequestBehavior.AllowGet);

            }
            else
            {
                Type ResultType = GetSQLQueryReturnType.DynamicSqlQuery(unitOfWork.context.Database, "SELECT TOP 0 * FROM " + ViewName);              
                IEnumerable viewsList = unitOfWork.ExecuteRawQuery(ResultType, "SELECT "+Value+" FROM " + ViewName);
                return Json(viewsList, JsonRequestBehavior.AllowGet);


            }

        }


    }

    public class PageListOfDataRow
    {
        public List<DataRow> list { get; set; }
        public PagedList<DataRow> plist { get; set; }

    }



}
