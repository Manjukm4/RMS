namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RIC_UserDashboardPanel_RowWidth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_UserDashboardPanel", "RU_RowWidth", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_UserDashboardPanel", "RU_RowWidth");
        }
    }
}
