namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annual_feedback3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedback", "RA_Hr_ReviewStatus", c => c.String(maxLength: 10));
            AlterColumn("dbo.RIC_AnnualFeedback", "RA_Hr_LeaveHistory", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_AnnualFeedback", "RA_Hr_LeaveHistory", c => c.String());
            DropColumn("dbo.RIC_AnnualFeedback", "RA_Hr_ReviewStatus");
        }
    }
}
