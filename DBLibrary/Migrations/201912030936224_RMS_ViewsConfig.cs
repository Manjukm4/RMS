namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RMS_ViewsConfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_ViewsConfig", "RV_DisplayName", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_ViewsConfig", "RV_Description", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMS_ViewsConfig", "RV_Description");
            DropColumn("dbo.RMS_ViewsConfig", "RV_DisplayName");
        }
    }
}
