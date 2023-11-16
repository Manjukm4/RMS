namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefreshAnnualFeedbackTbls : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_AnnualFeedback",
                c => new
                    {
                        RA_ReviewId = c.Int(nullable: false, identity: true),
                        RA_FormId = c.Int(nullable: false),
                        RA_EmployeeId = c.String(maxLength: 50),
                        RA_LeadReviewerId = c.String(maxLength: 50),
                        Hr_Id = c.String(maxLength: 50),
                        RA_Status = c.String(maxLength: 10),
                        RA_Date = c.DateTime(nullable: false),
                        RA_Hr_LeaveHistory = c.String(),
                        RA_Hr_LoyaltyToCompanyValues = c.String(),
                        RA_Hr_BehavingToCulturalValue = c.String(),
                        RA_Hr_RespectsDiffInCulturalValue = c.String(),
                        RA_Hr_AdditionalComments = c.String(),
                        RA_Hr_ImprovementsRecommended = c.String(),
                    })
                .PrimaryKey(t => t.RA_ReviewId);
            
            CreateTable(
                "dbo.RIC_AnnualFeedbackReviewers",
                c => new
                    {
                        RR_ID = c.Int(nullable: false, identity: true),
                        RR_ReviewID = c.Int(nullable: false),
                        RR_ReviewerID = c.String(maxLength: 50),
                        RR_AdditionalComments = c.String(),
                        RR_ImprovementsRecommended = c.String(),
                        RR_Status = c.String(),
                    })
                .PrimaryKey(t => t.RR_ID)
                .ForeignKey("dbo.RIC_AnnualFeedback", t => t.RR_ReviewID, cascadeDelete: true)
                .Index(t => t.RR_ReviewID);
            
            CreateTable(
                "dbo.RIC_AnnualFeedbackDtl",
                c => new
                    {
                        RA_DtlId = c.Int(nullable: false, identity: true),
                        RA_ReviewID = c.Int(nullable: false),
                        RA_FieldID = c.Int(nullable: false),
                        RA_ReviewerID = c.String(maxLength: 50),
                        RA_ReviewRating = c.Single(nullable: false),
                        RA_ReviewComments = c.String(),
                        RA_Status = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.RA_DtlId)
                .ForeignKey("dbo.RIC_AnnualFeedbackReviewers", t => t.RA_ReviewID, cascadeDelete: true)
                .Index(t => t.RA_ReviewID);
            
            CreateTable(
                "dbo.RIC_AnnualFeedbackFields",
                c => new
                    {
                        AF_FieldId = c.Int(nullable: false, identity: true),
                        AF_FormID = c.Int(nullable: false),
                        AF_Pid = c.Int(),
                        AF_FieldName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.AF_FieldId)
                .ForeignKey("dbo.RIC_AnnualFeedbackForm", t => t.AF_FormID, cascadeDelete: true)
                .Index(t => t.AF_FormID);
            
            CreateTable(
                "dbo.RIC_AnnualFeedbackForm",
                c => new
                    {
                        AF_FormID = c.Int(nullable: false, identity: true),
                        AF_FormName = c.String(),
                    })
                .PrimaryKey(t => t.AF_FormID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_AnnualFeedbackFields", "AF_FormID", "dbo.RIC_AnnualFeedbackForm");
            DropForeignKey("dbo.RIC_AnnualFeedbackDtl", "RA_ReviewID", "dbo.RIC_AnnualFeedbackReviewers");
            DropForeignKey("dbo.RIC_AnnualFeedbackReviewers", "RR_ReviewID", "dbo.RIC_AnnualFeedback");
            DropIndex("dbo.RIC_AnnualFeedbackFields", new[] { "AF_FormID" });
            DropIndex("dbo.RIC_AnnualFeedbackDtl", new[] { "RA_ReviewID" });
            DropIndex("dbo.RIC_AnnualFeedbackReviewers", new[] { "RR_ReviewID" });
            DropTable("dbo.RIC_AnnualFeedbackForm");
            DropTable("dbo.RIC_AnnualFeedbackFields");
            DropTable("dbo.RIC_AnnualFeedbackDtl");
            DropTable("dbo.RIC_AnnualFeedbackReviewers");
            DropTable("dbo.RIC_AnnualFeedback");
        }
    }
}
