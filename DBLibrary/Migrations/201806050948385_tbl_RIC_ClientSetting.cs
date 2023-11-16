namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tbl_RIC_ClientSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_ClientSetting",
                c => new
                    {
                        RC_Id = c.Int(nullable: false, identity: true),
                        RC_EmpCd = c.String(maxLength: 50),
                        RC_ClientList = c.String(),
                    })
                .PrimaryKey(t => t.RC_Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_ClientSetting");
        }
    }
}
