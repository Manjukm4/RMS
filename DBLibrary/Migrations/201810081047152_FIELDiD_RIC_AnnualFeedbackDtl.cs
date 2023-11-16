namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FIELDiD_RIC_AnnualFeedbackDtl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedbackDtl", "RA_FieldID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_AnnualFeedbackDtl", "RA_FieldID");
        }
    }
}
