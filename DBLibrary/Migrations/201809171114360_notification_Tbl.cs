namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notification_Tbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_Notification",
                c => new
                    {
                        RN_ID = c.Int(nullable: false, identity: true),
                        RN_EmpCd = c.String(maxLength: 50),
                        Rn_ReviewID = c.Int(nullable: false),
                        RN_NotificationText = c.String(maxLength: 100),
                        RN_IsSeen = c.Boolean(nullable: false),
                        RN_DirSeen = c.Boolean(nullable: false),
                        RN_HrSeen = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RN_ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_Notification");
        }
    }
}
