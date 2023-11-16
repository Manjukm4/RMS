using DBLibrary;
using DBLibrary.UnitOfWork;
using RIC.com.jobdiva.ws_API;
using RIC.Models.AssignSharedReq;
using RIC.Models.SharedReq;
using RIC.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace RIC.Controllers
{
	[Authorize]
	public class SharedReqController : Controller
	{
		UnitOfWork unitOfwork = new UnitOfWork();

		//
		// GET: /SharedReq/
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult CallAPI(SharedReqVM SharedReqVM)
		{
			if (SharedReqVM.SharedReqSearch.ClientID == null || SharedReqVM.SharedReqSearch.CompanyName == null)
			{
				SharedReqVM.Message = "Client ID and Company Name are Mandatory.";
				return View("Index", SharedReqVM);
			}

			JobDivaAPI JobDivaAPI = new JobDivaAPI();
			string OutMessage;
			RIC.com.jobdiva.ws_API.DataSet DsJobDivaAPI;

			JobDivaAPI.searchJob(278, "Autoload@sunrisesys.com", "Welcome123", 0, false, null, SharedReqVM.SharedReqSearch.ClientID.Trim(), null, null, null, 0, false, 0, false,
				 SharedReqVM.SharedReqSearch.CompanyName.Trim(), 0, false, null, DateTime.MinValue, false, DateTime.MaxValue, false, DateTime.MinValue, false, DateTime.MaxValue, false, out OutMessage, out DsJobDivaAPI);

			if (OutMessage != "")
			{
				// If more than 1 row is returned from API call.
				SharedReqVM.Message = OutMessage;
				return View("Index", SharedReqVM);
			}

			SharedReq SharedReq;
			if (DsJobDivaAPI != null && DsJobDivaAPI.Row.Count() > 0 && OutMessage == "")
			{
				SharedReq = new SharedReq();
				foreach (var Row in DsJobDivaAPI.Row)
				{
					//For Analysis
					//DataTable dtSubCounts12 = new DataTable();
					//foreach (var col in DsJobDivaAPI.Columns)
					//{
					//  dtSubCounts12.Columns.Add(col);
					//  }
					// dtSubCounts.Rows.Add(DsJobDivaBI.Row.Select(e => e.Value).ToArray());
					/// dtSubCounts12.Rows.Add(DsJobDivaAPI.Row.Select(e => e.Value).ToArray());
					//
					switch (Row.Name.ToLower())
					{
						case "id":
						SharedReq.JobId = int.Parse(Row.Value);
						SharedReq.SearchJobApi_id = Row.Value;
						break;
						case "reference #":
						SharedReq.JobDivaRef = Row.Value;
						SharedReq.SearchJobApi_reference = Row.Value;
						break;
						case "company":
						SharedReq.Company = Row.Value;
						SharedReq.SearchJobApi_company = Row.Value;
						break;
						case "job title":
						SharedReq.JobTitle = Row.Value;
						SharedReq.SearchJobApi_job_title = Row.Value;
						break;
						case "address1":
						SharedReq.WorkLocation = Row.Value;
						SharedReq.SearchJobApi_address1 = Row.Value;
						break;
						case "city":
						SharedReq.City = Row.Value;
						SharedReq.SearchJobApi_city = Row.Value;
						break;
						case "state":
						SharedReq.State = Row.Value;
						SharedReq.SearchJobApi_state = Row.Value;
						break;
						case "issue date":
						SharedReq.JobIssueDate = DateTime.Parse(Row.Value);
						SharedReq.SearchJobApi_issue_date = Row.Value;
						break;
						case "users(last name | first name | role(s) | id)":
						SharedReq.PrimaryAssigment = Row.Value.Replace("[", "").Replace("]", "").Split(',');
						break;
						case "contact id":
						SharedReq.SearchJobApi_contact_id = Row.Value;
						break;
						case "company id":
						SharedReq.SearchJobApi_company_id = Row.Value;
						break;
						case "optional reference":
						SharedReq.ClientID = Row.Value;
						SharedReq.SearchJobApi_optional_reference = Row.Value;
						break;
						case "address2":
						SharedReq.SearchJobApi_address2 = Row.Value;
						break;
						case "country":
						SharedReq.SearchJobApi_country = Row.Value;
						break;
						case "zipcode":
						SharedReq.SearchJobApi_zipcode = Row.Value;
						break;
						case "first name":
						SharedReq.SearchJobApi_first_name = Row.Value;
						break;
						case "last name":
						SharedReq.SearchJobApi_last_name = Row.Value;
						break;
						case "department":
						SharedReq.SearchJobApi_department = Row.Value;
						break;
						case "job status":
						SharedReq.SearchJobApi_job_status = Row.Value;
						break;
						case "job type":
						SharedReq.SearchJobApi_job_type = Row.Value;
						break;
						case "start date":
						SharedReq.SearchJobApi_start_date = Row.Value;
						break;
						case "end date":
						SharedReq.SearchJobApi_end_date = Row.Value;
						break;
						case "minimum rate":
						SharedReq.SearchJobApi_minimum_rate = Row.Value;
						break;
						case "maximum rate":
						SharedReq.SearchJobApi_maximum_rate = Row.Value;
						break;
						case "minimum bill rate":
						SharedReq.SearchJobApi_minimum_bill_rate = Row.Value;
						break;
						case "maximum bill rate":
						SharedReq.SearchJobApi_maximum_bill_rate = Row.Value;
						break;
						default:
						break;
					}
				}

				if (SharedReq.JobId != 0)
				{
					BIData JDBI = new BIData();
					DataSet DsJobDivaBI;

					JDBI.getBIData("Job Detail", "278", "Autoload@sunrisesys.com", "Welcome123", DateTime.MinValue, false,
										  DateTime.MaxValue, false, new string[] { SharedReq.JobId.ToString() }, out DsJobDivaBI);

					if (DsJobDivaBI != null && DsJobDivaBI.Row.Count() > 0)
					{
						foreach (var item in DsJobDivaBI.Row)
						{
							//For Analysis 
							//DataTable dtSubCounts1 = new DataTable();
							//foreach (var col in DsJobDivaBI.Columns)
							//{
							//    dtSubCounts1.Columns.Add(col);
							//}

							//// dtSubCounts.Rows.Add(DsJobDivaBI.Row.Select(e => e.Value).ToArray());

							//foreach (var item1 in DsJobDivaBI.Row)
							//{
							//    dtSubCounts1.Rows.Add(item1.RowData1.Select(e => e.Value).ToArray());
							//}

							foreach (var Element in item.RowData1)
							{
								switch (Element.Name)
								{
									case "PRIORITY":
									SharedReq.Priority = Element.Value;
									SharedReq.GetBIDataApi_JobDetails_PRIORITY = Element.Value;
									break;
									case "JOBSTATUS":
									SharedReq.JobDivaStatus = Element.Value;
									SharedReq.GetBIDataApi_JobDetails_JOBSTATUS = Element.Value;
									break;
									case "DIVISION":
									SharedReq.Division = Element.Value;
									SharedReq.GetBIDataApi_JobDetails_DIVISION = Element.Value;
									break;
									case "MAXALLOWEDSUBMITTALS":
									SharedReq.MaxSubAllowed = int.Parse(Element.Value);
									SharedReq.GetBIDataApi_JobDetails_MAXALLOWEDSUBMITTALS = Element.Value;
									break;
									case "BILLRATEMAX":
									SharedReq.BillRate = float.Parse(Element.Value);
									SharedReq.GetBIDataApi_JobDetails_BILLRATEMAX = Element.Value;
									break;
									case "PAYRATEMAX":
									SharedReq.PayRate = float.Parse(Element.Value);
									SharedReq.GetBIDataApi_JobDetails_PAYRATEMAX = Element.Value;
									break;
									//
									case "ID":
									SharedReq.GetBIDataApi_JobDetails_ID = Element.Value;
									break;
									case "DATEISSUED":
									SharedReq.GetBIDataApi_JobDetails_DATEISSUED = Element.Value;
									break;
									case "DATEUPDATED":
									SharedReq.GetBIDataApi_JobDetails_DATEUPDATED = Element.Value;
									break;
									case "DATEUSERFIELDUPDATED":
									SharedReq.GetBIDataApi_JobDetails_DATEUSERFIELDUPDATED = Element.Value;
									break;
									case "CUSTOMERID":
									SharedReq.GetBIDataApi_JobDetails_CUSTOMERID = Element.Value;
									break;
									case "COMPANYID":
									SharedReq.GetBIDataApi_JobDetails_COMPANYID = Element.Value;
									break;
									case "ADDRESS1":
									SharedReq.GetBIDataApi_JobDetails_ADDRESS1 = Element.Value;
									break;
									case "ADDRESS2":
									SharedReq.GetBIDataApi_JobDetails_ADDRESS2 = Element.Value;
									break;
									case "CITY":
									SharedReq.GetBIDataApi_JobDetails_CITY = Element.Value;
									break;
									case "STATE":
									SharedReq.GetBIDataApi_JobDetails_STATE = Element.Value;
									break;
									case "ZIPCODE":
									SharedReq.GetBIDataApi_JobDetails_ZIPCODE = Element.Value;
									break;
									case "REFNO":
									SharedReq.GetBIDataApi_JobDetails_REFNO = Element.Value;
									break;
									case "JOBDIVANO":
									SharedReq.GetBIDataApi_JobDetails_JOBDIVANO = Element.Value;
									break;
									case "STARTDATE":
									SharedReq.GetBIDataApi_JobDetails_STARTDATE = Element.Value;
									break;
									case "ENDDATE":
									SharedReq.GetBIDataApi_JobDetails_ENDDATE = Element.Value;
									break;
									case "POSITIONS":
									SharedReq.GetBIDataApi_JobDetails_POSITIONS = Element.Value;
									break;
									case "FILLS":
									SharedReq.GetBIDataApi_JobDetails_FILLS = Element.Value;
									break;
									case "BILLRATEMIN":
									SharedReq.GetBIDataApi_JobDetails_BILLRATEMIN = Element.Value;
									break;
									case "BILLRATEPER":
									SharedReq.GetBIDataApi_JobDetails_BILLRATEPER = Element.Value;
									break;
									case "AYRATEMIN":
									SharedReq.GetBIDataApi_JobDetails_AYRATEMIN = Element.Value;
									break;
									case "AYRATEPER":
									SharedReq.GetBIDataApi_JobDetails_AYRATEPER = Element.Value;
									break;
									case "POSITIONTYPE":
									SharedReq.GetBIDataApi_JobDetails_POSITIONTYPE = Element.Value;
									break;
									case "SKILLS":
									SharedReq.GetBIDataApi_JobDetails_SKILLS = Element.Value;
									break;
									case "JOBTITLE":
									SharedReq.GetBIDataApi_JobDetails_JOBTITLE = Element.Value;
									break;
									case "JOBDESCRIPTION":
									SharedReq.GetBIDataApi_JobDetails_JOBDESCRIPTION = Element.Value;
									break;
									case "REMARKS":
									SharedReq.GetBIDataApi_JobDetails_REMARKS = Element.Value;
									break;
									case "SUBMITTALINSTRUCTION":
									SharedReq.GetBIDataApi_JobDetails_SUBMITTALINSTRUCTION = Element.Value;
									break;
									case "POSTTOPORTAL":
									SharedReq.GetBIDataApi_JobDetails_POSTTOPORTAL = Element.Value;
									break;
									case "POSTING_TITLE":
									SharedReq.GetBIDataApi_JobDetails_POSTING_TITLE = Element.Value;
									break;
									case "POSTING_DATE":
									SharedReq.GetBIDataApi_JobDetails_POSTING_DATE = Element.Value;
									break;
									case "POSTINGDESCRIPTION":
									SharedReq.GetBIDataApi_JobDetails_POSTINGDESCRIPTION = Element.Value;
									break;
									case "CRITERIA_DEGREE":
									SharedReq.GetBIDataApi_JobDetails_CRITERIA_DEGREE = Element.Value;
									break;
									case "JOBCATALOGID":
									SharedReq.GetBIDataApi_JobDetails_JOBCATALOGID = Element.Value;
									break;
									case "CATALOGCOMPANYID":
									SharedReq.GetBIDataApi_JobDetails_CATALOGCOMPANYID = Element.Value;
									break;
									case "CATALOGTITLE":
									SharedReq.GetBIDataApi_JobDetails_CATALOGTITLE = Element.Value;
									break;
									case "CATALOGREFNO":
									SharedReq.GetBIDataApi_JobDetails_CATALOGREFNO = Element.Value;
									break;
									case "CATALOGNAME":
									SharedReq.GetBIDataApi_JobDetails_CATALOGNAME = Element.Value;
									break;
									case "CATALOGACTIVE":
									SharedReq.GetBIDataApi_JobDetails_CATALOGACTIVE = Element.Value;
									break;
									case "CATALOGEFFECTIVEDATE":
									SharedReq.GetBIDataApi_JobDetails_CATALOGEFFECTIVEDATE = Element.Value;
									break;
									case "CATALOGEXPIRATIONDATE":
									SharedReq.GetBIDataApi_JobDetails_CATALOGEXPIRATIONDATE = Element.Value;
									break;
									case "CATALOGCATEGORY":
									SharedReq.GetBIDataApi_JobDetails_CATALOGCATEGORY = Element.Value;
									break;
									case "CATALOGBILLRATELOW":
									SharedReq.GetBIDataApi_JobDetails_CATALOGBILLRATELOW = Element.Value;
									break;
									case "CATALOGBILLRATEHIGH":
									SharedReq.GetBIDataApi_JobDetails_CATALOGBILLRATEHIGH = Element.Value;
									break;
									case "CATALOGBILLRATEPER":
									SharedReq.GetBIDataApi_JobDetails_CATALOGBILLRATEPER = Element.Value;
									break;
									case "CATALOGPAYRATELOW":
									SharedReq.GetBIDataApi_JobDetails_CATALOGPAYRATELOW = Element.Value;
									break;
									case "CATALOGPAYRATEHIGH":
									SharedReq.GetBIDataApi_JobDetails_CATALOGPAYRATEHIGH = Element.Value;
									break;
									case "CATALOGPAYRATEPER":
									SharedReq.GetBIDataApi_JobDetails_CATALOGPAYRATEPER = Element.Value;
									break;
									case "POSITIONREFNO":
									SharedReq.GetBIDataApi_JobDetails_POSITIONREFNO = Element.Value;
									break;
									case "PREVENTLOWERPAY":
									SharedReq.GetBIDataApi_JobDetails_PREVENTLOWERPAY = Element.Value;
									break;
									case "PREVENTHIGHERBILL":
									SharedReq.GetBIDataApi_JobDetails_PREVENTHIGHERBILL = Element.Value;
									break;
									case "CATALOGNOTES":
									SharedReq.GetBIDataApi_JobDetails_CATALOGNOTES = Element.Value;
									break;
									//
									default:
									break;
								}
							}
						}
					}

					//
					JDBI.getBIData("Job Submittals Detail", "278", "Autoload@sunrisesys.com", "Welcome123", DateTime.MinValue, false, DateTime.MaxValue, false, new string[] { SharedReq.JobId.ToString() }, out DsJobDivaBI);

					DataTable dtSubCounts = new DataTable();
					if (DsJobDivaBI.Row != null)
					{
						foreach (var col in DsJobDivaBI.Columns)
						{
							dtSubCounts.Columns.Add(col);
						}

						// dtSubCounts.Rows.Add(DsJobDivaBI.Row.Select(e => e.Value).ToArray());
						foreach (var item in DsJobDivaBI.Row)
						{
							dtSubCounts.Rows.Add(item.RowData1.Select(e => e.Value).ToArray());
						}

						if (dtSubCounts.Rows.Count > 0)
						{
							SharedReq.InternalSub = dtSubCounts.Select("RoleID <> '1'").Length;
							//SharedReq.ExternalSub = dtSubCounts.Select("RoleID = '1'").Length;
						}
					}

					//Use RMS database for External-Submissions.
					SharedReq.ExternalSub = unitOfwork.RIC_Job_Report.GetAll().Where(e => e.RJ_JobDiva_Ref == SharedReq.JobDivaRef && e.RJ_Optional_Ref == SharedReq.SearchJobApi_optional_reference).Count();
				}
				SharedReqVM.ListSharedReq = new List<SharedReq>();
				SharedReqVM.ListSharedReq.Add(SharedReq);

				return View("Index", SharedReqVM);
			}
			else
			{
				SharedReqVM.Message = "No data found for given search parameters.";
				return View("Index", SharedReqVM);
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult SavetoRMS(SharedReqVM SharedReqVM)
		{
			if (SharedReqVM.ListSharedReq != null && SharedReqVM.ListSharedReq.Count >= 1)
			{
				if ((from sharedReq_Hdr in unitOfwork.RMS_SharedReq_HDR.Get()
					  where sharedReq_Hdr.RS_JobDivaRef == SharedReqVM.ListSharedReq[0].JobDivaRef
					  select new
					  {
						  sharedReq_Hdr.RS_JobDivaRef
					  }).Count() > 0)
				{
					SharedReqVM.Message = "This requirement already exists in RMS Database.";
					return View("Index", SharedReqVM);
				}

				List<RMS_SharedReq_Dtl> RMS_SharedReq_Dtl = new List<RMS_SharedReq_Dtl>();
				foreach (SharedReq sharedReq in SharedReqVM.ListSharedReq)
				{
					RMS_SharedReq_Dtl SharedReq_Dtl = new RMS_SharedReq_Dtl()
					 {
						 RS_JobTitle = sharedReq.JobTitle,
						 RS_JobIssueDate = sharedReq.JobIssueDate,
						 RS_Company = sharedReq.Company,
						 RS_WorkLocation = sharedReq.WorkLocation,
						 RS_ClientID = sharedReq.ClientID,
						 RS_Division = sharedReq.Division,
						 RS_Priority = sharedReq.Priority,

						 RS_Category = sharedReq.Category,
						 RS_State = sharedReq.State,
						 RS_City = sharedReq.City,
						 RS_BillRate = sharedReq.BillRate,
						 RS_PayRate = sharedReq.PayRate,

						 RS_PrimaryAssigment = sharedReq.PrimaryAssigment != null? String.Join(",", sharedReq.PrimaryAssigment):null,
						 RS_MaxSubAllowed = sharedReq.MaxSubAllowed,
						 RS_InternalSub = sharedReq.InternalSub,
						 RS_ExternalSub = sharedReq.ExternalSub,

						 // RS_Instructions = sharedReq.Instructions,
						 //
						 SearchJobApi_id = sharedReq.SearchJobApi_id,
						 SearchJobApi_contact_id = sharedReq.SearchJobApi_contact_id,
						 SearchJobApi_company_id = sharedReq.SearchJobApi_company_id,
						 SearchJobApi_company = sharedReq.SearchJobApi_company,
						 SearchJobApi_reference = sharedReq.SearchJobApi_reference,
						 SearchJobApi_optional_reference = sharedReq.SearchJobApi_optional_reference,
						 SearchJobApi_job_title = sharedReq.SearchJobApi_job_title,
						 SearchJobApi_address1 = sharedReq.SearchJobApi_address1,
						 SearchJobApi_address2 = sharedReq.SearchJobApi_address2,
						 SearchJobApi_city = sharedReq.SearchJobApi_city,
						 SearchJobApi_state = sharedReq.SearchJobApi_state,
						 SearchJobApi_country = sharedReq.SearchJobApi_country,
						 SearchJobApi_zipcode = sharedReq.SearchJobApi_zipcode,
						 SearchJobApi_first_name = sharedReq.SearchJobApi_first_name,
						 SearchJobApi_last_name = sharedReq.SearchJobApi_last_name,
						 SearchJobApi_department = sharedReq.SearchJobApi_department,
						 SearchJobApi_job_status = sharedReq.SearchJobApi_job_status,
						 SearchJobApi_job_type = sharedReq.SearchJobApi_job_type,
						 SearchJobApi_issue_date = sharedReq.SearchJobApi_issue_date,
						 SearchJobApi_start_date = sharedReq.SearchJobApi_start_date,
						 SearchJobApi_end_date = sharedReq.SearchJobApi_end_date,
						 SearchJobApi_minimum_rate = sharedReq.SearchJobApi_minimum_rate,
						 SearchJobApi_maximum_rate = sharedReq.SearchJobApi_maximum_rate,
						 SearchJobApi_minimum_bill_rate = sharedReq.SearchJobApi_minimum_bill_rate,
						 SearchJobApi_maximum_bill_rate = sharedReq.SearchJobApi_maximum_bill_rate,

						 GetBIDataApi_JobDetails_ID = sharedReq.GetBIDataApi_JobDetails_ID,
						 GetBIDataApi_JobDetails_DATEISSUED = sharedReq.GetBIDataApi_JobDetails_DATEISSUED,
						 GetBIDataApi_JobDetails_DATEUPDATED = sharedReq.GetBIDataApi_JobDetails_DATEUPDATED,
						 GetBIDataApi_JobDetails_DATEUSERFIELDUPDATED = sharedReq.GetBIDataApi_JobDetails_DATEUSERFIELDUPDATED,
						 GetBIDataApi_JobDetails_JOBSTATUS = sharedReq.GetBIDataApi_JobDetails_JOBSTATUS,
						 GetBIDataApi_JobDetails_CUSTOMERID = sharedReq.GetBIDataApi_JobDetails_CUSTOMERID,
						 GetBIDataApi_JobDetails_COMPANYID = sharedReq.GetBIDataApi_JobDetails_COMPANYID,
						 GetBIDataApi_JobDetails_ADDRESS1 = sharedReq.GetBIDataApi_JobDetails_ADDRESS1,
						 GetBIDataApi_JobDetails_ADDRESS2 = sharedReq.GetBIDataApi_JobDetails_ADDRESS2,
						 GetBIDataApi_JobDetails_CITY = sharedReq.GetBIDataApi_JobDetails_CITY,
						 GetBIDataApi_JobDetails_STATE = sharedReq.GetBIDataApi_JobDetails_STATE,
						 GetBIDataApi_JobDetails_ZIPCODE = sharedReq.GetBIDataApi_JobDetails_ZIPCODE,
						 GetBIDataApi_JobDetails_PRIORITY = sharedReq.GetBIDataApi_JobDetails_PRIORITY,
						 GetBIDataApi_JobDetails_DIVISION = sharedReq.GetBIDataApi_JobDetails_DIVISION,
						 GetBIDataApi_JobDetails_REFNO = sharedReq.GetBIDataApi_JobDetails_REFNO,
						 GetBIDataApi_JobDetails_JOBDIVANO = sharedReq.GetBIDataApi_JobDetails_JOBDIVANO,
						 GetBIDataApi_JobDetails_STARTDATE = sharedReq.GetBIDataApi_JobDetails_STARTDATE,
						 GetBIDataApi_JobDetails_ENDDATE = sharedReq.GetBIDataApi_JobDetails_ENDDATE,
						 GetBIDataApi_JobDetails_POSITIONS = sharedReq.GetBIDataApi_JobDetails_POSITIONS,
						 GetBIDataApi_JobDetails_FILLS = sharedReq.GetBIDataApi_JobDetails_FILLS,
						 GetBIDataApi_JobDetails_MAXALLOWEDSUBMITTALS = sharedReq.GetBIDataApi_JobDetails_MAXALLOWEDSUBMITTALS,
						 GetBIDataApi_JobDetails_BILLRATEMIN = sharedReq.GetBIDataApi_JobDetails_BILLRATEMIN,
						 GetBIDataApi_JobDetails_BILLRATEMAX = sharedReq.GetBIDataApi_JobDetails_BILLRATEMAX,
						 GetBIDataApi_JobDetails_BILLRATEPER = sharedReq.GetBIDataApi_JobDetails_BILLRATEPER,
						 GetBIDataApi_JobDetails_AYRATEMIN = sharedReq.GetBIDataApi_JobDetails_AYRATEMIN,
						 GetBIDataApi_JobDetails_PAYRATEMAX = sharedReq.GetBIDataApi_JobDetails_PAYRATEMAX,
						 GetBIDataApi_JobDetails_AYRATEPER = sharedReq.GetBIDataApi_JobDetails_AYRATEPER,
						 GetBIDataApi_JobDetails_POSITIONTYPE = sharedReq.GetBIDataApi_JobDetails_POSITIONTYPE,
						 GetBIDataApi_JobDetails_SKILLS = sharedReq.GetBIDataApi_JobDetails_SKILLS,
						 GetBIDataApi_JobDetails_JOBTITLE = sharedReq.GetBIDataApi_JobDetails_JOBTITLE,
						 GetBIDataApi_JobDetails_JOBDESCRIPTION = sharedReq.GetBIDataApi_JobDetails_JOBDESCRIPTION,
						 GetBIDataApi_JobDetails_REMARKS = sharedReq.GetBIDataApi_JobDetails_REMARKS,
						 GetBIDataApi_JobDetails_SUBMITTALINSTRUCTION = sharedReq.GetBIDataApi_JobDetails_SUBMITTALINSTRUCTION,
						 GetBIDataApi_JobDetails_POSTTOPORTAL = sharedReq.GetBIDataApi_JobDetails_POSTTOPORTAL,
						 GetBIDataApi_JobDetails_POSTING_TITLE = sharedReq.GetBIDataApi_JobDetails_POSTING_TITLE,
						 GetBIDataApi_JobDetails_POSTING_DATE = sharedReq.GetBIDataApi_JobDetails_POSTING_DATE,
						 GetBIDataApi_JobDetails_POSTINGDESCRIPTION = sharedReq.GetBIDataApi_JobDetails_POSTINGDESCRIPTION,
						 GetBIDataApi_JobDetails_CRITERIA_DEGREE = sharedReq.GetBIDataApi_JobDetails_CRITERIA_DEGREE,
						 GetBIDataApi_JobDetails_JOBCATALOGID = sharedReq.GetBIDataApi_JobDetails_JOBCATALOGID,
						 GetBIDataApi_JobDetails_CATALOGCOMPANYID = sharedReq.GetBIDataApi_JobDetails_CATALOGCOMPANYID,
						 GetBIDataApi_JobDetails_CATALOGTITLE = sharedReq.GetBIDataApi_JobDetails_CATALOGTITLE,
						 GetBIDataApi_JobDetails_CATALOGREFNO = sharedReq.GetBIDataApi_JobDetails_CATALOGREFNO,
						 GetBIDataApi_JobDetails_CATALOGNAME = sharedReq.GetBIDataApi_JobDetails_CATALOGNAME,
						 GetBIDataApi_JobDetails_CATALOGACTIVE = sharedReq.GetBIDataApi_JobDetails_CATALOGACTIVE,
						 GetBIDataApi_JobDetails_CATALOGEFFECTIVEDATE = sharedReq.GetBIDataApi_JobDetails_CATALOGEFFECTIVEDATE,
						 GetBIDataApi_JobDetails_CATALOGEXPIRATIONDATE = sharedReq.GetBIDataApi_JobDetails_CATALOGEXPIRATIONDATE,
						 GetBIDataApi_JobDetails_CATALOGCATEGORY = sharedReq.GetBIDataApi_JobDetails_CATALOGCATEGORY,
						 GetBIDataApi_JobDetails_CATALOGBILLRATELOW = sharedReq.GetBIDataApi_JobDetails_CATALOGBILLRATELOW,
						 GetBIDataApi_JobDetails_CATALOGBILLRATEHIGH = sharedReq.GetBIDataApi_JobDetails_CATALOGBILLRATEHIGH,
						 GetBIDataApi_JobDetails_CATALOGBILLRATEPER = sharedReq.GetBIDataApi_JobDetails_CATALOGBILLRATEPER,
						 GetBIDataApi_JobDetails_CATALOGPAYRATELOW = sharedReq.GetBIDataApi_JobDetails_CATALOGPAYRATELOW,
						 GetBIDataApi_JobDetails_CATALOGPAYRATEHIGH = sharedReq.GetBIDataApi_JobDetails_CATALOGPAYRATEHIGH,
						 GetBIDataApi_JobDetails_CATALOGPAYRATEPER = sharedReq.GetBIDataApi_JobDetails_CATALOGPAYRATEPER,
						 GetBIDataApi_JobDetails_POSITIONREFNO = sharedReq.GetBIDataApi_JobDetails_POSITIONREFNO,
						 GetBIDataApi_JobDetails_PREVENTLOWERPAY = sharedReq.GetBIDataApi_JobDetails_PREVENTLOWERPAY,
						 GetBIDataApi_JobDetails_PREVENTHIGHERBILL = sharedReq.GetBIDataApi_JobDetails_PREVENTHIGHERBILL,
						 GetBIDataApi_JobDetails_CATALOGNOTES = sharedReq.GetBIDataApi_JobDetails_CATALOGNOTES,
						 //
					 };

					if (sharedReq.File != null)
					{
						SharedReq_Dtl.RS_FileName = sharedReq.JobDivaRef + Path.GetExtension(sharedReq.File.FileName);
						SharedReq_Dtl.RS_FileType = sharedReq.File.ContentType;
						using (Stream fs = sharedReq.File.InputStream)
						{
							using (BinaryReader br = new BinaryReader(fs))
							{
								SharedReq_Dtl.RS_FileData = br.ReadBytes((Int32)fs.Length);
							}
						}
					}
					RMS_SharedReq_Dtl.Add(SharedReq_Dtl);

					List<RMS_SharedReqNotes> RMS_SharedReqNotes = new List<RMS_SharedReqNotes>();
					RMS_SharedReqNotes SharedReqNotes = new RMS_SharedReqNotes()
					{
						RS_Notes = sharedReq.Instructions,
						RS_CreatedBy = User.Identity.Name,
						RS_CreatedDt = SystemClock.US_Date
					};
					RMS_SharedReqNotes.Add(SharedReqNotes);

					RMS_SharedReq_HDR Shared_HDR = new RMS_SharedReq_HDR()
					{
						RS_JobID = sharedReq.JobId,
						RS_JobDivaRef = sharedReq.JobDivaRef,
						RS_CreatedDt = SystemClock.US_Date,
						RS_Req = "Shared",
						RS_CreatedBy = User.Identity.Name,
						RS_RMSJobStatus = "OPEN",
						RS_JobDivaStatus = sharedReq.JobDivaStatus,
						RMS_SharedReq_Dtl = RMS_SharedReq_Dtl,
						RMS_SharedReqNotes = RMS_SharedReqNotes
					};

					unitOfwork.RMS_SharedReq_HDR.Insert(Shared_HDR);
					unitOfwork.Save();

					// Send mail..
					string  error;

					string MailBody = @"<!DOCTYPE html>
										<html lang='en'>
										<head><meta charset='UTF-8'>
										    <title>Notification</title><style>body{font-family: arial, sans-serif;font-size:16px;}#div{padding:1%;background: white;display: block;margin: 0 auto;margin-bottom: 0.5cm;box-shadow: 0 0 0.5cm rgba(0,0,0,0.5);}table {font-family: arial, sans-serif;border-collapse: collapse;width: 100%;}td, th {border: 1px solid #dddddd;text-align: left;padding: 8px;}tr:nth-child(odd){background-color: #dddddd;}</style>
										</head>
										<body><div id='div'><p >Hi,</p><div style='padding:1%;'><p> New job as been added to RMS shared requirement.</p><table><tr><th>Job ID</th><th>JD Ref</th><th>Title</th><th>Company</th><th>Requester</th><th>MaxSub</th></tr><tr><td><a href='https://www1.jobdiva.com/employers/myjobs/vieweditjobform.jsp?jobid=@JobID'>@JobID</a></td><td>@JDRef</td><td>@Title</td><td>@Company</td><td>@Requester</td><td>@MaxSub</td></tr></table></div><br><p>Please do not reply to this mail.<p></div></body></html>
										";

				 MailBody = MailBody.Replace("@JobID", sharedReq.JobId.ToString());
				 MailBody = MailBody.Replace("@JDRef", sharedReq.JobDivaRef);
				 MailBody = MailBody.Replace("@Title", sharedReq.JobTitle);
				 MailBody = MailBody.Replace("@Company", sharedReq.Company);
				 MailBody = MailBody.Replace("@Requester", unitOfwork.User.GetByEmpID(User.Identity.Name).RE_Jobdiva_User_Name);
				 MailBody = MailBody.Replace("@MaxSub", sharedReq.MaxSubAllowed.ToString());

				 //Email.SendMail(ConfigurationManager.AppSettings["SharedReq_Email"].Split(';'), "New Job Added To RMS", MailBody, out error);
  				}
 				//		if (string.IsNullOrEmpty(error))
				//return Content(("<script language='javascript' type='text/javascript'> alert('Password reset link sent to your registered email ');window.location = '" + Url.Action("Login") + "';</script>"));
				//			else
				//					return Content(("<script language='javascript' type='text/javascript'> alert('" + error + "');window.location = '" + Request.UrlReferrer.ToString() + "';</script>"));
				//

			}
			SharedReqVM.Message = "Sucessfully saved to RMS Database.";
			return View("Index", SharedReqVM);
		}

		[HttpPost]
		public JsonResult GetCompany(string Prefix)
		{
			unitOfwork.context.Configuration.AutoDetectChangesEnabled = false;

			return Json(unitOfwork.RMS_SharedReq_HDR.GetCompany(Prefix), JsonRequestBehavior.AllowGet);
		}

	}
}
