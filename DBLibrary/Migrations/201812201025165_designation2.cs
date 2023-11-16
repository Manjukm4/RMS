namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class designation2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RMS_Designation",
                c => new
                    {
                        RD_ID = c.Int(nullable: false, identity: true),
                        RD_Designation = c.String(),
                    })
                .PrimaryKey(t => t.RD_ID);
            
            AddColumn("dbo.RIC_Employee", "RE_DesignationID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Employee", "RE_DesignationID");
            DropTable("dbo.RMS_Designation");
        }
    }
}
