namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeedbackAudit_Trail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedback", "RA_CreatedBy", c => c.String());
            AddColumn("dbo.RIC_AnnualFeedback", "RA_UpdatedBy", c => c.String());
            AddColumn("dbo.RIC_AnnualFeedback", "RA_CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RIC_AnnualFeedback", "RA_UpdatedDate", c => c.DateTime());
            AddColumn("dbo.RIC_PersonalFeedback", "RP_CreatedBy", c => c.String());
            AddColumn("dbo.RIC_PersonalFeedback", "RP_UpdatedBy", c => c.String());
            AddColumn("dbo.RIC_PersonalFeedback", "RP_CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RIC_PersonalFeedback", "RP_UpdatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_PersonalFeedback", "RP_UpdatedDate");
            DropColumn("dbo.RIC_PersonalFeedback", "RP_CreatedDate");
            DropColumn("dbo.RIC_PersonalFeedback", "RP_UpdatedBy");
            DropColumn("dbo.RIC_PersonalFeedback", "RP_CreatedBy");
            DropColumn("dbo.RIC_AnnualFeedback", "RA_UpdatedDate");
            DropColumn("dbo.RIC_AnnualFeedback", "RA_CreatedDate");
            DropColumn("dbo.RIC_AnnualFeedback", "RA_UpdatedBy");
            DropColumn("dbo.RIC_AnnualFeedback", "RA_CreatedBy");
        }
    }
}
