namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnualFeedBackroles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedbackFields", "AF_Roles", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_AnnualFeedbackFields", "AF_Roles");
        }
    }
}
