namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SQLVIEW2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RMS_ViewsConfig",
                c => new
                    {
                        RV_Id = c.Int(nullable: false, identity: true),
                        RV_ViewName = c.String(maxLength: 50),
                        RV_ColumnName = c.String(maxLength: 50),
                        RV_ColumnType = c.String(maxLength: 20),
                        RV_ControlType = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.RV_Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RMS_ViewsConfig");
        }
    }
}
