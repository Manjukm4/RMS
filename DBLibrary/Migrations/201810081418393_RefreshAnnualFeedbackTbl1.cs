namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefreshAnnualFeedbackTbl1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedbackFields", "AF_AF_Weightage", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_AnnualFeedbackFields", "AF_AF_Weightage");
        }
    }
}
