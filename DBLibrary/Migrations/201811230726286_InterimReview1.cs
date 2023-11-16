namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InterimReview1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_PersonalFeedback", "RP_ForwardToHr", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_PersonalFeedback", "RP_ForwardToHr");
        }
    }
}
