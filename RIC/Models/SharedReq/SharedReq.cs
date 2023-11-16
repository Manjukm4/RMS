using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.SharedReq
{
    public class SharedReq
    {
        public int JobId { get; set; }
        public string JobDivaRef { get; set; }
        public string ClientID { get; set; }
        public string JobTitle { get; set; }
        public DateTime JobIssueDate { get; set; }
        public string Company { get; set; }
        public string WorkLocation { get; set; }
        public string Priority { get; set; }
        public string Division { get; set; }
        public string Category { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public float BillRate { get; set; }
        public float PayRate { get; set; }
        public string[] PrimaryAssigment { get; set; }
        public int MaxSubAllowed { get; set; }
        public int InternalSub { get; set; }
        public int ExternalSub { get; set; }
        public string JobDivaStatus { get; set; }
        public string Instructions { get; set; }
        public HttpPostedFileBase File { get; set; }

        public string SearchJobApi_id { get; set; }
        public string SearchJobApi_contact_id { get; set; }
        public string SearchJobApi_company_id { get; set; }
        public string SearchJobApi_company { get; set; }
        public string SearchJobApi_reference { get; set; }
        public string SearchJobApi_optional_reference { get; set; }
        public string SearchJobApi_job_title { get; set; }
        public string SearchJobApi_address1 { get; set; }
        public string SearchJobApi_address2 { get; set; }
        public string SearchJobApi_city { get; set; }
        public string SearchJobApi_state { get; set; }
        public string SearchJobApi_country { get; set; }
        public string SearchJobApi_zipcode { get; set; }
        public string SearchJobApi_first_name { get; set; }
        public string SearchJobApi_last_name { get; set; }
        public string SearchJobApi_department { get; set; }
        public string SearchJobApi_job_status { get; set; }
        public string SearchJobApi_job_type { get; set; }
        public string SearchJobApi_issue_date { get; set; }
        public string SearchJobApi_start_date { get; set; }
        public string SearchJobApi_end_date { get; set; }
        public string SearchJobApi_minimum_rate { get; set; }
        public string SearchJobApi_maximum_rate { get; set; }
        public string SearchJobApi_minimum_bill_rate { get; set; }
        public string SearchJobApi_maximum_bill_rate { get; set; }

        public string GetBIDataApi_JobDetails_ID { get; set; }
        public string GetBIDataApi_JobDetails_DATEISSUED { get; set; }
        public string GetBIDataApi_JobDetails_DATEUPDATED { get; set; }
        public string GetBIDataApi_JobDetails_DATEUSERFIELDUPDATED { get; set; }
        public string GetBIDataApi_JobDetails_JOBSTATUS { get; set; }
        public string GetBIDataApi_JobDetails_CUSTOMERID { get; set; }
        public string GetBIDataApi_JobDetails_COMPANYID { get; set; }
        public string GetBIDataApi_JobDetails_ADDRESS1 { get; set; }
        public string GetBIDataApi_JobDetails_ADDRESS2 { get; set; }
        public string GetBIDataApi_JobDetails_CITY { get; set; }
        public string GetBIDataApi_JobDetails_STATE { get; set; }
        public string GetBIDataApi_JobDetails_ZIPCODE { get; set; }
        public string GetBIDataApi_JobDetails_PRIORITY { get; set; }
        public string GetBIDataApi_JobDetails_DIVISION { get; set; }
        public string GetBIDataApi_JobDetails_REFNO { get; set; }
        public string GetBIDataApi_JobDetails_JOBDIVANO { get; set; }
        public string GetBIDataApi_JobDetails_STARTDATE { get; set; }
        public string GetBIDataApi_JobDetails_ENDDATE { get; set; }
        public string GetBIDataApi_JobDetails_POSITIONS { get; set; }
        public string GetBIDataApi_JobDetails_FILLS { get; set; }
        public string GetBIDataApi_JobDetails_MAXALLOWEDSUBMITTALS { get; set; }
        public string GetBIDataApi_JobDetails_BILLRATEMIN { get; set; }
        public string GetBIDataApi_JobDetails_BILLRATEMAX { get; set; }
        public string GetBIDataApi_JobDetails_BILLRATEPER { get; set; }
        public string GetBIDataApi_JobDetails_AYRATEMIN { get; set; }
        public string GetBIDataApi_JobDetails_PAYRATEMAX { get; set; }
        public string GetBIDataApi_JobDetails_AYRATEPER { get; set; }
        public string GetBIDataApi_JobDetails_POSITIONTYPE { get; set; }
        public string GetBIDataApi_JobDetails_SKILLS { get; set; }
        public string GetBIDataApi_JobDetails_JOBTITLE { get; set; }
        public string GetBIDataApi_JobDetails_JOBDESCRIPTION { get; set; }
        public string GetBIDataApi_JobDetails_REMARKS { get; set; }
        public string GetBIDataApi_JobDetails_SUBMITTALINSTRUCTION { get; set; }
        public string GetBIDataApi_JobDetails_POSTTOPORTAL { get; set; }
        public string GetBIDataApi_JobDetails_POSTING_TITLE { get; set; }
        public string GetBIDataApi_JobDetails_POSTING_DATE { get; set; }
        public string GetBIDataApi_JobDetails_POSTINGDESCRIPTION { get; set; }
        public string GetBIDataApi_JobDetails_CRITERIA_DEGREE { get; set; }
        public string GetBIDataApi_JobDetails_JOBCATALOGID { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGCOMPANYID { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGTITLE { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGREFNO { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGNAME { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGACTIVE { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGEFFECTIVEDATE { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGEXPIRATIONDATE { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGCATEGORY { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGBILLRATELOW { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGBILLRATEHIGH { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGBILLRATEPER { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGPAYRATELOW { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGPAYRATEHIGH { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGPAYRATEPER { get; set; }
        public string GetBIDataApi_JobDetails_POSITIONREFNO { get; set; }
        public string GetBIDataApi_JobDetails_PREVENTLOWERPAY { get; set; }
        public string GetBIDataApi_JobDetails_PREVENTHIGHERBILL { get; set; }
        public string GetBIDataApi_JobDetails_CATALOGNOTES { get; set; }

    }
}