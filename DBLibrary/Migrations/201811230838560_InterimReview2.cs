namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InterimReview2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_PersonalFeedback", "RP_Hr_ReviewStatus", c => c.Boolean());
            AddColumn("dbo.RIC_PersonalFeedback", "RP_DirectorReviewStatus", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_PersonalFeedback", "RP_DirectorReviewStatus");
            DropColumn("dbo.RIC_PersonalFeedback", "RP_Hr_ReviewStatus");
        }
    }
}
