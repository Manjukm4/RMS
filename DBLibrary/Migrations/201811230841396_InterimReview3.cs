namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InterimReview3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_PersonalFeedback", "RP_Hr_ReviewStatus", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_PersonalFeedback", "RP_Hr_ReviewStatus", c => c.Boolean());
        }
    }
}
