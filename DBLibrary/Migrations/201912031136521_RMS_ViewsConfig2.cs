namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RMS_ViewsConfig2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_ViewsConfig", "RV_Description", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_ViewsConfig", "RV_Description", c => c.String(maxLength: 50));
        }
    }
}
