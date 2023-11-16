namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnualReviewTbl_propWarnings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedback", "RA_Hr_Warnings", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_AnnualFeedback", "RA_Hr_Warnings");
        }
    }
}
