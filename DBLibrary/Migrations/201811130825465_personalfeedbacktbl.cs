namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class personalfeedbacktbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_PersonalFeedback",
                c => new
                    {
                        RP_ID = c.Int(nullable: false, identity: true),
                        RP_EmployeeID = c.String(maxLength: 50),
                        RP_ReviewerID = c.String(maxLength: 50),
                        RP_FromDate = c.DateTime(nullable: false),
                        RP_ToDate = c.DateTime(nullable: false),
                        RP_ReviewDate = c.DateTime(nullable: false),
                        RP_TotalSubmissions = c.Int(nullable: false),
                        RP_TotalInterview = c.Int(nullable: false),
                        RP_TotalHires = c.Int(nullable: false),
                        RP_TotalCalls = c.Int(nullable: false),
                        RP_NextReviewDate = c.DateTime(nullable: false),
                        RP_SubmissionTarget = c.Int(nullable: false),
                        RP_InterviewTarget = c.Int(nullable: false),
                        RP_HiresTarget = c.Int(nullable: false),
                        RP_CallsTarget = c.Int(nullable: false),
                        RP_Status = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.RP_ID);
            
            CreateTable(
                "dbo.RIC_PersonalFeedbackDtl",
                c => new
                    {
                        RD_ID = c.Int(nullable: false, identity: true),
                        RD_ReviewID = c.Int(nullable: false),
                        RD_RevewerID = c.String(),
                        RD_Findings = c.String(),
                        RD_Improvements = c.String(),
                        RD_Status = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.RD_ID)
                .ForeignKey("dbo.RIC_PersonalFeedback", t => t.RD_ReviewID, cascadeDelete: true)
                .Index(t => t.RD_ReviewID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_PersonalFeedbackDtl", "RD_ReviewID", "dbo.RIC_PersonalFeedback");
            DropIndex("dbo.RIC_PersonalFeedbackDtl", new[] { "RD_ReviewID" });
            DropTable("dbo.RIC_PersonalFeedbackDtl");
            DropTable("dbo.RIC_PersonalFeedback");
        }
    }
}
