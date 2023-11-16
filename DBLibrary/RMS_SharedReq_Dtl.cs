using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RMS_SharedReq_Dtl
    {
        [Key]
        public int RS_ID { get; set; }

        [ForeignKey("RMS_SharedReq_HDR")]
        public int RS_RefID { get; set; }

        [StringLength(20)]
        public string RS_ClientID { get; set; }

        [StringLength(500)]
        public string RS_JobTitle { get; set; }
        public DateTime RS_JobIssueDate { get; set; }

        [StringLength(100)]
        public string RS_Company { get; set; }
        [StringLength(20)]
        public string RS_State { get; set; }

        [StringLength(20)]
        public string RS_City { get; set; }

        [StringLength(500)]
        public string RS_WorkLocation { get; set; }

        [StringLength(50)]
        public string RS_Priority { get; set; }

        [StringLength(50)]
        public string RS_Division { get; set; }

        [StringLength(50)]
        public string RS_Category { get; set; }

        public float? RS_BillRate { get; set; }
        public float? RS_PayRate { get; set; }

        [StringLength(1000)]
        public string RS_PrimaryAssigment { get; set; }
        public int? RS_MaxSubAllowed { get; set; }
        public int? RS_InternalSub { get; set; }
        public int? RS_ExternalSub { get; set; }

        [StringLength(1000)]
        public string RS_Instructions { get; set; }

        [StringLength(50)]
        public string RS_FileName { get; set; }
        [StringLength(100)]
        public string RS_FileType { get; set; }
        public byte[] RS_FileData { get; set; }
        
        // Raw data from Api calls.
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

		  public virtual RMS_SharedReq_HDR RMS_SharedReq_HDR { get; set; }
    }
}
