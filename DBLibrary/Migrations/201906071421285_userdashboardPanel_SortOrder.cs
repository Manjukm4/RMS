namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userdashboardPanel_SortOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_UserDashboardPanel", "RU_SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_UserDashboardPanel", "RU_SortOrder");
        }
    }
}
