namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_AnnualFeedbackTbls : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RIC_AnnualFeedbackDtl", "RA_ReviewID", "dbo.RIC_AnnualFeedback");
            DropForeignKey("dbo.RIC_AnnualFeedbackForm", "AF_FieldID", "dbo.RIC_AnnualFeedbackFields");
            DropIndex("dbo.RIC_AnnualFeedbackDtl", new[] { "RA_ReviewID" });
            DropIndex("dbo.RIC_AnnualFeedbackForm", new[] { "AF_FieldID" });
            DropTable("dbo.RIC_AnnualFeedback");
            DropTable("dbo.RIC_AnnualFeedbackDtl");
            DropTable("dbo.RIC_AnnualFeedbackFields");
            DropTable("dbo.RIC_AnnualFeedbackForm");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RIC_AnnualFeedbackForm",
                c => new
                    {
                        Af_ID = c.Int(nullable: false, identity: true),
                        AF_FormID = c.Int(nullable: false),
                        AF_FieldID = c.Int(nullable: false),
                        AF_ParentID = c.Int(),
                        AF_Weightage = c.Int(nullable: false),
                        Role = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Af_ID);
            
            CreateTable(
                "dbo.RIC_AnnualFeedbackFields",
                c => new
                    {
                        AF_FieldId = c.Int(nullable: false, identity: true),
                        AF_FieldName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.AF_FieldId);
            
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
                .PrimaryKey(t => t.RA_DtlId);
            
            CreateTable(
                "dbo.RIC_AnnualFeedback",
                c => new
                    {
                        RA_ReviewId = c.Int(nullable: false, identity: true),
                        RA_FormId = c.Int(nullable: false),
                        RA_EmployeeId = c.String(maxLength: 50),
                        RA_LeadReviewerId = c.String(maxLength: 50),
                        Hr_Id = c.String(maxLength: 50),
                        Status = c.String(maxLength: 10),
                        RA_Date = c.DateTime(nullable: false),
                        RA_Remarks = c.String(),
                    })
                .PrimaryKey(t => t.RA_ReviewId);
            
            CreateIndex("dbo.RIC_AnnualFeedbackForm", "AF_FieldID");
            CreateIndex("dbo.RIC_AnnualFeedbackDtl", "RA_ReviewID");
            AddForeignKey("dbo.RIC_AnnualFeedbackForm", "AF_FieldID", "dbo.RIC_AnnualFeedbackFields", "AF_FieldId", cascadeDelete: true);
            AddForeignKey("dbo.RIC_AnnualFeedbackDtl", "RA_ReviewID", "dbo.RIC_AnnualFeedback", "RA_ReviewId", cascadeDelete: true);
        }
    }
}
