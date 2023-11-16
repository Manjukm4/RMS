namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RIC_QuarterlyIncentive : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_QuarterlyIncentive",
                c => new
                    {
                        RI_ID = c.Int(nullable: false, identity: true),
                        RI_EmpCd = c.String(maxLength: 50),
                        RI_EmployeeName = c.String(maxLength: 200),
                        RI_TeamLead = c.String(maxLength: 200),
                        RI_Candidate = c.String(maxLength: 200),
                        RI_Company = c.String(maxLength: 200),
                        RI_StartDate = c.DateTime(nullable: false),
                        RI_EndDate = c.DateTime(),
                        RI_NetMargin = c.Single(nullable: false),
                        RI_Incentives = c.Single(nullable: false),
                        RI_Remarks = c.String(maxLength: 2000),
                        RI_IncentiveCategory = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.RI_ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_QuarterlyIncentive");
        }
    }
}
