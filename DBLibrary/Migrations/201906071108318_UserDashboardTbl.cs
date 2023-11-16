namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserDashboardTbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_Panel",
                c => new
                    {
                        RP_Id = c.Int(nullable: false, identity: true),
                        RP_PanelName = c.String(maxLength: 50),
                        RP_ActionName = c.String(maxLength: 50),
                        RP_ConTrollerName = c.String(maxLength: 50),
                        RP_ImageName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.RP_Id);
            
            CreateTable(
                "dbo.RIC_UserDashboardPanel",
                c => new
                    {
                        RU_Id = c.Int(nullable: false, identity: true),
                        RU_EmpCd = c.String(maxLength: 50),
                        RU_PanelId = c.Int(nullable: false),
                        RU_RowNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RU_Id)
                .ForeignKey("dbo.RIC_Panel", t => t.RU_PanelId, cascadeDelete: true)
                .Index(t => t.RU_PanelId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_UserDashboardPanel", "RU_PanelId", "dbo.RIC_Panel");
            DropIndex("dbo.RIC_UserDashboardPanel", new[] { "RU_PanelId" });
            DropTable("dbo.RIC_UserDashboardPanel");
            DropTable("dbo.RIC_Panel");
        }
    }
}
