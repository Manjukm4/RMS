namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menu_module_tbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_Menu_Module",
                c => new
                    {
                        RM_MenuID = c.Int(nullable: false, identity: true),
                        RM_MenuParentId = c.Int(nullable: false),
                        RM_MenuName = c.String(maxLength: 20),
                        RM_ControllerName = c.String(maxLength: 20),
                        RM_ActionName = c.String(maxLength: 20),
                        RM_Sort_Order = c.Int(nullable: false),
                        RM_Menuitem_Level = c.Int(nullable: false),
                        RM_Roles = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.RM_MenuID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_Menu_Module");
        }
    }
}
