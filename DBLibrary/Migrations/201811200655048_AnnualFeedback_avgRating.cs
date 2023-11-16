namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnualFeedback_avgRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedback", "RA_AvgRating", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_AnnualFeedback", "RA_AvgRating");
        }
    }
}
