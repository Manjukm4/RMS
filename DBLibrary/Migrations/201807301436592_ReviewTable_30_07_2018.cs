namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReviewTable_30_07_2018 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_Review",
                c => new
                    {
                        RR_Id = c.Int(nullable: false, identity: true),
                        RR_EmpCd = c.String(nullable: false, maxLength: 50),
                        RR_MgrCd = c.String(nullable: false, maxLength: 50),
                        RR_FromDate = c.DateTime(nullable: false),
                        RR_ToDate = c.DateTime(nullable: false),
                        RR_Submissions = c.Int(nullable: false),
                        RR_Interviews = c.Int(nullable: false),
                        RR_Hires = c.Int(nullable: false),
                        RR_Calls = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RR_Id);
            
            CreateTable(
                "dbo.RIC_Feedback",
                c => new
                    {
                        RR_Id = c.Int(nullable: false),
                        RR_TlFindings = c.String(),
                        RR_Improvements = c.String(),
                        RR_DirectorFeedback = c.String(),
                        RR_HrFeedback = c.String(),
                        RR_DirectorFeedbackRequired = c.Boolean(nullable: false),
                        RR_HrFeedbackRequired = c.Boolean(nullable: false),
                        RR_NextReviewDate = c.DateTime(nullable: false),
                        RR_SubmissionTarget = c.Int(nullable: false),
                        RR_InterviewTarget = c.Int(nullable: false),
                        RR_HiresTarget = c.Int(nullable: false),
                        RR_CallsTarget = c.Int(nullable: false),
                        RR_Draft = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RR_Id)
                .ForeignKey("dbo.RIC_Review", t => t.RR_Id)
                .Index(t => t.RR_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_Feedback", "RR_Id", "dbo.RIC_Review");
            DropIndex("dbo.RIC_Feedback", new[] { "RR_Id" });
            DropTable("dbo.RIC_Feedback");
            DropTable("dbo.RIC_Review");
        }
    }
}
