namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Department_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RMS_Department",
                c => new
                    {
                        RD_ID = c.Int(nullable: false, identity: true),
                        RD_Department = c.String(),
                    })
                .PrimaryKey(t => t.RD_ID);
            
            AddColumn("dbo.RIC_Employee", "RE_DepartmentID", c => c.Int());
            AddColumn("dbo.RIC_Employee", "RMS_Department_RD_ID", c => c.Int());
            CreateIndex("dbo.RIC_Employee", "RMS_Department_RD_ID");
            AddForeignKey("dbo.RIC_Employee", "RMS_Department_RD_ID", "dbo.RMS_Department", "RD_ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_Employee", "RMS_Department_RD_ID", "dbo.RMS_Department");
            DropIndex("dbo.RIC_Employee", new[] { "RMS_Department_RD_ID" });
            DropColumn("dbo.RIC_Employee", "RMS_Department_RD_ID");
            DropColumn("dbo.RIC_Employee", "RE_DepartmentID");
            DropTable("dbo.RMS_Department");
        }
    }
}
