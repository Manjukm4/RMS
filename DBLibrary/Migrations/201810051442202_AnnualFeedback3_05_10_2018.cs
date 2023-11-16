namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnualFeedback3_05_10_2018 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedback", "RA_Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_AnnualFeedback", "RA_Remarks");
        }
    }
}
