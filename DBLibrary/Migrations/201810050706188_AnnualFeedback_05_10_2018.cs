namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnualFeedback_05_10_2018 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedback", "RA_LeadReviewerId", c => c.String(maxLength: 50));
            AddColumn("dbo.RIC_AnnualFeedback", "Status", c => c.String(maxLength: 1));
            AddColumn("dbo.RIC_AnnualFeedback", "RA_Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.RIC_AnnualFeedbackDtl", "RA_Status", c => c.String(maxLength: 1));
            AddColumn("dbo.RIC_AnnualFeedbackForm", "AF_Weightage", c => c.Int(nullable: false));
            DropColumn("dbo.RIC_AnnualFeedback", "RA_TlId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_AnnualFeedback", "RA_TlId", c => c.String(maxLength: 50));
            DropColumn("dbo.RIC_AnnualFeedbackForm", "AF_Weightage");
            DropColumn("dbo.RIC_AnnualFeedbackDtl", "RA_Status");
            DropColumn("dbo.RIC_AnnualFeedback", "RA_Date");
            DropColumn("dbo.RIC_AnnualFeedback", "Status");
            DropColumn("dbo.RIC_AnnualFeedback", "RA_LeadReviewerId");
        }
    }
}
