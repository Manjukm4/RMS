namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnualFeedback1_05_10_2018 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedbackDtl", "RA_ReviewerID", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_AnnualFeedbackDtl", "RA_ReviewerID");
        }
    }
}
