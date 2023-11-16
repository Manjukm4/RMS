namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShredDtl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_Dtl", "contact_id", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "company_id", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "company", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "optional_reference", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "address2", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "country", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "zipcode", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "first_name", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "last_name", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "department", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "job_type", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "start_date", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "end_date", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "minimum_rate", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "maximum_rate", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "minimum_bill_rate", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "maximum_bill_rate", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "DATEISSUED", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "DATEUPDATED", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "DATEUSERFIELDUPDATED", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CUSTOMERID", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "COMPANYID", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "REFNO", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "JOBDIVANO", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "STARTDATE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "ENDDATE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "POSITIONS", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "FILLS", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "MAXALLOWEDSUBMITTALS", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "BILLRATEMIN", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "BILLRATEPER", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "AYRATEMIN", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "AYRATEPER", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "POSITIONTYPE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "SKILLS", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "JOBTITLE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "JOBDESCRIPTION", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "REMARKS", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "SUBMITTALINSTRUCTION", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "POSTTOPORTAL", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "POSTING_TITLE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "POSTING_DATE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "POSTINGDESCRIPTION", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CRITERIA_DEGREE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "JOBCATALOGID", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGCOMPANYID", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGTITLE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGREFNO", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGNAME", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGACTIVE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGEFFECTIVEDATE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGEXPIRATIONDATE", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGCATEGORY", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGBILLRATELOW", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGBILLRATEHIGH", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGBILLRATEPER", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGPAYRATELOW", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGPAYRATEHIGH", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGPAYRATEPER", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "POSITIONREFNO", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "PREVENTLOWERPAY", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "PREVENTHIGHERBILL", c => c.String());
            AddColumn("dbo.RMS_SharedReq_Dtl", "CATALOGNOTES", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGNOTES");
            DropColumn("dbo.RMS_SharedReq_Dtl", "PREVENTHIGHERBILL");
            DropColumn("dbo.RMS_SharedReq_Dtl", "PREVENTLOWERPAY");
            DropColumn("dbo.RMS_SharedReq_Dtl", "POSITIONREFNO");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGPAYRATEPER");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGPAYRATEHIGH");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGPAYRATELOW");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGBILLRATEPER");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGBILLRATEHIGH");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGBILLRATELOW");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGCATEGORY");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGEXPIRATIONDATE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGEFFECTIVEDATE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGACTIVE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGNAME");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGREFNO");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGTITLE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CATALOGCOMPANYID");
            DropColumn("dbo.RMS_SharedReq_Dtl", "JOBCATALOGID");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CRITERIA_DEGREE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "POSTINGDESCRIPTION");
            DropColumn("dbo.RMS_SharedReq_Dtl", "POSTING_DATE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "POSTING_TITLE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "POSTTOPORTAL");
            DropColumn("dbo.RMS_SharedReq_Dtl", "SUBMITTALINSTRUCTION");
            DropColumn("dbo.RMS_SharedReq_Dtl", "REMARKS");
            DropColumn("dbo.RMS_SharedReq_Dtl", "JOBDESCRIPTION");
            DropColumn("dbo.RMS_SharedReq_Dtl", "JOBTITLE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "SKILLS");
            DropColumn("dbo.RMS_SharedReq_Dtl", "POSITIONTYPE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "AYRATEPER");
            DropColumn("dbo.RMS_SharedReq_Dtl", "AYRATEMIN");
            DropColumn("dbo.RMS_SharedReq_Dtl", "BILLRATEPER");
            DropColumn("dbo.RMS_SharedReq_Dtl", "BILLRATEMIN");
            DropColumn("dbo.RMS_SharedReq_Dtl", "MAXALLOWEDSUBMITTALS");
            DropColumn("dbo.RMS_SharedReq_Dtl", "FILLS");
            DropColumn("dbo.RMS_SharedReq_Dtl", "POSITIONS");
            DropColumn("dbo.RMS_SharedReq_Dtl", "ENDDATE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "STARTDATE");
            DropColumn("dbo.RMS_SharedReq_Dtl", "JOBDIVANO");
            DropColumn("dbo.RMS_SharedReq_Dtl", "REFNO");
            DropColumn("dbo.RMS_SharedReq_Dtl", "COMPANYID");
            DropColumn("dbo.RMS_SharedReq_Dtl", "CUSTOMERID");
            DropColumn("dbo.RMS_SharedReq_Dtl", "DATEUSERFIELDUPDATED");
            DropColumn("dbo.RMS_SharedReq_Dtl", "DATEUPDATED");
            DropColumn("dbo.RMS_SharedReq_Dtl", "DATEISSUED");
            DropColumn("dbo.RMS_SharedReq_Dtl", "maximum_bill_rate");
            DropColumn("dbo.RMS_SharedReq_Dtl", "minimum_bill_rate");
            DropColumn("dbo.RMS_SharedReq_Dtl", "maximum_rate");
            DropColumn("dbo.RMS_SharedReq_Dtl", "minimum_rate");
            DropColumn("dbo.RMS_SharedReq_Dtl", "end_date");
            DropColumn("dbo.RMS_SharedReq_Dtl", "start_date");
            DropColumn("dbo.RMS_SharedReq_Dtl", "job_type");
            DropColumn("dbo.RMS_SharedReq_Dtl", "department");
            DropColumn("dbo.RMS_SharedReq_Dtl", "last_name");
            DropColumn("dbo.RMS_SharedReq_Dtl", "first_name");
            DropColumn("dbo.RMS_SharedReq_Dtl", "zipcode");
            DropColumn("dbo.RMS_SharedReq_Dtl", "country");
            DropColumn("dbo.RMS_SharedReq_Dtl", "address2");
            DropColumn("dbo.RMS_SharedReq_Dtl", "optional_reference");
            DropColumn("dbo.RMS_SharedReq_Dtl", "company");
            DropColumn("dbo.RMS_SharedReq_Dtl", "company_id");
            DropColumn("dbo.RMS_SharedReq_Dtl", "contact_id");
        }
    }
}
