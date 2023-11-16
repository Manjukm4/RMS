namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RJ_Job_Report : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_Job_Report",
                c => new
                    {
                        RJ_ID = c.Int(nullable: false, identity: true),
                        RJ_EmpCd = c.String(nullable: false, maxLength: 50),
                        RJ_DateIssued = c.DateTime(nullable: false),
                        RJ_Title = c.String(maxLength: 100),
                        RJ_Company = c.String(maxLength: 100),
                        RJ_JobDiva_Ref = c.String(maxLength: 100),
                        RJ_Optional_Ref = c.String(maxLength: 100),
                        RJ_Priority = c.String(maxLength: 100),
                        RJ_Job_Status = c.String(maxLength: 100),
                        RJ_Position_Type = c.String(maxLength: 100),
                        RJ_Openings = c.Int(nullable: false),
                        RJ_Fills = c.Int(nullable: false),
                        RJ_Manager = c.String(maxLength: 100),
                        RJ_Bill_Rate_MinMax = c.String(maxLength: 100),
                        RJ_Pay_Rate_Min_Max = c.String(maxLength: 100),
                        RJ_Division = c.String(maxLength: 50),
                        RJ_Address1 = c.String(maxLength: 100),
                        RJ_Address2 = c.String(maxLength: 100),
                        RJ_City = c.String(maxLength: 100),
                        RJ_State = c.String(maxLength: 100),
                        RJ_Zipcode = c.String(maxLength: 100),
                        RJ_Skills = c.String(maxLength: 500),
                        RJ_Prim_Sales = c.String(maxLength: 100),
                        RJ_Users = c.String(maxLength: 100),
                        RJ_Candidate = c.String(maxLength: 100),
                        RJ_Agreed_Bill_Rate = c.Single(),
                        RJ_Quoted_Bill_Rate = c.Single(),
                        RJ_Pay_Rate = c.Single(),
                        RJ_C2C = c.String(maxLength: 20),
                        RJ_Submit_Date = c.DateTime(nullable: false),
                        RJ_Interview_Date = c.DateTime(),
                        RJ_Hire_Date = c.DateTime(),
                        RJ_Submitted_By = c.String(maxLength: 100),
                        RJ_Rejection_Reason = c.String(maxLength: 100),
                        RJ_Comments = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.RJ_ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_Job_Report");
        }
    }
}
