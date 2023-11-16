namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tbl_SubmissionRule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_SubmissionRule",
                c => new
                    {
                        RS_Id = c.Int(nullable: false, identity: true),
                        RS_Level = c.Int(nullable: false),
                        RS_Experience = c.String(nullable: false, maxLength: 20),
                        RS_Requirement = c.String(),
                        RS_MonthlySubmissions = c.Int(nullable: false),
                        RS_Monthly_Interviews = c.Int(nullable: false),
                        RS_MinimumSubs = c.Int(nullable: false),
                        RS_Minimum_Interviews = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RS_Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_SubmissionRule");
        }
    }
}
