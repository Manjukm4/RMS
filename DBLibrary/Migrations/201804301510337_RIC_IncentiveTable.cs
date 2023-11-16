namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RIC_IncentiveTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_Incentive",
                c => new
                    {
                        RI_ID = c.Int(nullable: false, identity: true),
                        RI_EmpCd = c.String(maxLength: 50),
                        RI_RecruitedBy = c.String(maxLength: 200),
                        RI_TeamLead = c.String(maxLength: 200),
                        RI_Candidate = c.String(maxLength: 200),
                        RI_Company = c.String(maxLength: 200),
                        RI_StartDate = c.DateTime(nullable: false),
                        RI_EndDate = c.DateTime(),
                        RI_NetMargin = c.Single(nullable: false),
                        RI_Jan_Incentive = c.Single(),
                        RI_Feb_Incentive = c.Single(),
                        RI_Mar_Incentive = c.Single(),
                        RI_Apr_Incentive = c.Single(),
                        RI_May_Incentive = c.Single(),
                        RI_Jun_Incentive = c.Single(),
                        RI_Jul_Incentive = c.Single(),
                        RI_Aug_Incentive = c.Single(),
                        RI_Sup_Incentive = c.Single(),
                        RI_Oct_Incentive = c.Single(),
                        RI_Nov_Incentive = c.Single(),
                        RI_Dec_Incentive = c.Single(),
                    })
                .PrimaryKey(t => t.RI_ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_Incentive");
        }
    }
}
