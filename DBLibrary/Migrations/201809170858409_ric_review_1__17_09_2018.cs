namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ric_review_1__17_09_2018 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Feedback", "RR_Discarded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Feedback", "RR_Discarded");
        }
    }
}
