namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeedbackAudit_Trail1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RIC_AnnualFeedback", "RA_CreatedBy");
            DropColumn("dbo.RIC_AnnualFeedback", "RA_CreatedDate");
            DropColumn("dbo.RIC_PersonalFeedback", "RP_CreatedBy");
            DropColumn("dbo.RIC_PersonalFeedback", "RP_CreatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_PersonalFeedback", "RP_CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RIC_PersonalFeedback", "RP_CreatedBy", c => c.String());
            AddColumn("dbo.RIC_AnnualFeedback", "RA_CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RIC_AnnualFeedback", "RA_CreatedBy", c => c.String());
        }
    }
}
