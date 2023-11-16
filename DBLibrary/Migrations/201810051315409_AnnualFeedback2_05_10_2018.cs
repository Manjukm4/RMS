namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnualFeedback2_05_10_2018 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_AnnualFeedback", "Status", c => c.String(maxLength: 10));
            AlterColumn("dbo.RIC_AnnualFeedbackDtl", "RA_Status", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_AnnualFeedbackDtl", "RA_Status", c => c.String(maxLength: 1));
            AlterColumn("dbo.RIC_AnnualFeedback", "Status", c => c.String(maxLength: 1));
        }
    }
}
