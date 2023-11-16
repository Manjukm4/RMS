namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnualFeedbackFields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_AnnualFeedbackFields",
                c => new
                    {
                        AF_FieldId = c.Int(nullable: false, identity: true),
                        AF_FieldName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.AF_FieldId);
            
            CreateTable(
                "dbo.RIC_AnnualFeedbackForm",
                c => new
                    {
                        Af_ID = c.Int(nullable: false, identity: true),
                        AF_FormID = c.Int(nullable: false),
                        AF_FieldID = c.Int(nullable: false),
                        AF_ParentID = c.Int(),
                        Role = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Af_ID)
                .ForeignKey("dbo.RIC_AnnualFeedbackFields", t => t.AF_FieldID, cascadeDelete: true)
                .Index(t => t.AF_FieldID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_AnnualFeedbackForm", "AF_FieldID", "dbo.RIC_AnnualFeedbackFields");
            DropIndex("dbo.RIC_AnnualFeedbackForm", new[] { "AF_FieldID" });
            DropTable("dbo.RIC_AnnualFeedbackForm");
            DropTable("dbo.RIC_AnnualFeedbackFields");
        }
    }
}
