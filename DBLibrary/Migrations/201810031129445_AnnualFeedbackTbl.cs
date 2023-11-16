namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnualFeedbackTbl : DbMigration
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
                        RA_TlId = c.String(maxLength: 50),
                        Hr_Id = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.RA_ReviewId);
            
            CreateTable(
                "dbo.RIC_AnnualFeedbackDtl",
                c => new
                    {
                        RA_DtlId = c.Int(nullable: false, identity: true),
                        RA_ReviewID = c.Int(nullable: false),
                        RA_ReviewRating = c.Single(nullable: false),
                        RA_ReviewComments = c.String(),
                    })
                .PrimaryKey(t => t.RA_DtlId)
                .ForeignKey("dbo.RIC_AnnualFeedback", t => t.RA_ReviewID, cascadeDelete: true)
                .Index(t => t.RA_ReviewID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_AnnualFeedbackDtl", "RA_ReviewID", "dbo.RIC_AnnualFeedback");
            DropIndex("dbo.RIC_AnnualFeedbackDtl", new[] { "RA_ReviewID" });
            DropTable("dbo.RIC_AnnualFeedbackDtl");
            DropTable("dbo.RIC_AnnualFeedback");
        }
    }
}
