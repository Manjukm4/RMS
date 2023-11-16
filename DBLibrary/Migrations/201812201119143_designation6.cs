namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class designation6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RIC_Employee", "RMS_Designation_RD_ID", "dbo.RMS_Designation");
            DropIndex("dbo.RIC_Employee", new[] { "RMS_Designation_RD_ID" });
            RenameColumn(table: "dbo.RIC_Employee", name: "RMS_Designation_RD_ID", newName: "RE_DID");
            AlterColumn("dbo.RIC_Employee", "RE_DID", c => c.Int(nullable: true));
            CreateIndex("dbo.RIC_Employee", "RE_DID");
            AddForeignKey("dbo.RIC_Employee", "RE_DID", "dbo.RMS_Designation", "RD_ID", cascadeDelete: true);
            DropColumn("dbo.RIC_Employee", "RE_DesignationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_Employee", "RE_DesignationID", c => c.Int(nullable: false));
            DropForeignKey("dbo.RIC_Employee", "RE_DID", "dbo.RMS_Designation");
            DropIndex("dbo.RIC_Employee", new[] { "RE_DID" });
            AlterColumn("dbo.RIC_Employee", "RE_DID", c => c.Int());
            RenameColumn(table: "dbo.RIC_Employee", name: "RE_DID", newName: "RMS_Designation_RD_ID");
            CreateIndex("dbo.RIC_Employee", "RMS_Designation_RD_ID");
            AddForeignKey("dbo.RIC_Employee", "RMS_Designation_RD_ID", "dbo.RMS_Designation", "RD_ID");
        }
    }
}
