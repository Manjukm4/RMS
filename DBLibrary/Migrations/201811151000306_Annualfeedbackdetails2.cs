namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Annualfeedbackdetails2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedback", "RA_Hr_LeavesTakenOnLop", c => c.Int(nullable: false));
            AlterColumn("dbo.RIC_AnnualFeedback", "RA_Hr_LoyaltyToCompanyValues", c => c.Int(nullable: false));
            AlterColumn("dbo.RIC_AnnualFeedback", "RA_Hr_BehavingToCulturalValue", c => c.Int(nullable: false));
            AlterColumn("dbo.RIC_AnnualFeedback", "RA_Hr_RespectsDiffInCulturalValue", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_AnnualFeedback", "RA_Hr_RespectsDiffInCulturalValue", c => c.String());
            AlterColumn("dbo.RIC_AnnualFeedback", "RA_Hr_BehavingToCulturalValue", c => c.String());
            AlterColumn("dbo.RIC_AnnualFeedback", "RA_Hr_LoyaltyToCompanyValues", c => c.String());
            DropColumn("dbo.RIC_AnnualFeedback", "RA_Hr_LeavesTakenOnLop");
        }
    }
}
