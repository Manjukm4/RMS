namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InterimReview : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_PersonalFeedback", "RP_DirectorID", c => c.String(maxLength: 50));
            AddColumn("dbo.RIC_PersonalFeedback", "RP_DirectorFeedback", c => c.String());
            AddColumn("dbo.RIC_PersonalFeedback", "RP_HrID", c => c.String(maxLength: 50));
            AddColumn("dbo.RIC_PersonalFeedback", "RP_HrFeedback", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_PersonalFeedback", "RP_HrFeedback");
            DropColumn("dbo.RIC_PersonalFeedback", "RP_HrID");
            DropColumn("dbo.RIC_PersonalFeedback", "RP_DirectorFeedback");
            DropColumn("dbo.RIC_PersonalFeedback", "RP_DirectorID");
        }
    }
}
