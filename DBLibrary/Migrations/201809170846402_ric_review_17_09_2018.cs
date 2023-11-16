namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ric_review_17_09_2018 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Feedback", "RR_AdditionalComments", c => c.String());
            AddColumn("dbo.RIC_Feedback", "RR_DirectorFeedbackStatus", c => c.Boolean(nullable: false));
            AddColumn("dbo.RIC_Feedback", "RR_HrFeedbackStatus", c => c.Boolean(nullable: false));
            AlterColumn("dbo.RIC_Feedback", "RR_CallsTarget", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Feedback", "RR_CallsTarget", c => c.Int(nullable: false));
            DropColumn("dbo.RIC_Feedback", "RR_HrFeedbackStatus");
            DropColumn("dbo.RIC_Feedback", "RR_DirectorFeedbackStatus");
            DropColumn("dbo.RIC_Feedback", "RR_AdditionalComments");
        }
    }
}
