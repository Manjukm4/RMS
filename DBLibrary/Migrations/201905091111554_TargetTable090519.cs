namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TargetTable090519 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_Targets",
                c => new
                    {
                        RT_ID = c.Int(nullable: false, identity: true),
                        RT_Emp_Cd = c.String(maxLength: 50),
                        RT_MgrCd = c.String(maxLength: 50),
                        RT_Year = c.Int(nullable: false),
                        RT_Month = c.String(maxLength: 20),
                        RT_SubmissionsTarget = c.Single(nullable: false),
                        RT_InterviewsTarget = c.Single(nullable: false),
                        RT_HiresTarget = c.Single(nullable: false),
                        RT_Comments = c.String(maxLength: 500),
                        RT_CreatedDate = c.DateTime(nullable: false),
                        RT_CreatedBy = c.String(maxLength: 50),
                        RT_UpdatedDate = c.DateTime(nullable: false),
                        RT_UpdatedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.RT_ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_Targets");
        }
    }
}
