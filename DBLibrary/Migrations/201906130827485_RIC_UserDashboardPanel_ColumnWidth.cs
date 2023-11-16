namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RIC_UserDashboardPanel_ColumnWidth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_UserDashboardPanel", "RU_ColumnNumber", c => c.Int(nullable: false));
            AddColumn("dbo.RIC_UserDashboardPanel", "RU_ColumnWidth", c => c.Int(nullable: false));
            DropColumn("dbo.RIC_UserDashboardPanel", "RU_RowNumber");
            DropColumn("dbo.RIC_UserDashboardPanel", "RU_RowWidth");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_UserDashboardPanel", "RU_RowWidth", c => c.Int(nullable: false));
            AddColumn("dbo.RIC_UserDashboardPanel", "RU_RowNumber", c => c.Int(nullable: false));
            DropColumn("dbo.RIC_UserDashboardPanel", "RU_ColumnWidth");
            DropColumn("dbo.RIC_UserDashboardPanel", "RU_ColumnNumber");
        }
    }
}
