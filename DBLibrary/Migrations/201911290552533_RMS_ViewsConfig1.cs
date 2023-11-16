namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RMS_ViewsConfig1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_ViewsConfig", "Lk_ViewName", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_ViewsConfig", "LK_ColumnName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMS_ViewsConfig", "LK_ColumnName");
            DropColumn("dbo.RMS_ViewsConfig", "Lk_ViewName");
        }
    }
}
