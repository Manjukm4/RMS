namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class designation7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RIC_Employee", "RE_DID", "dbo.RMS_Designation");
            DropIndex("dbo.RIC_Employee", new[] { "RE_DID" });
            RenameColumn(table: "dbo.RIC_Employee", name: "RE_DID", newName: "RE_DesignationID");
            AlterColumn("dbo.RIC_Employee", "RE_DesignationID", c => c.Int(nullable: true));
            CreateIndex("dbo.RIC_Employee", "RE_DesignationID");
            AddForeignKey("dbo.RIC_Employee", "RE_DesignationID", "dbo.RMS_Designation", "RD_ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_Employee", "RE_DesignationID", "dbo.RMS_Designation");
            DropIndex("dbo.RIC_Employee", new[] { "RE_DesignationID" });
            AlterColumn("dbo.RIC_Employee", "RE_DesignationID", c => c.Int());
            RenameColumn(table: "dbo.RIC_Employee", name: "RE_DesignationID", newName: "RE_DID");
            CreateIndex("dbo.RIC_Employee", "RE_DID");
            AddForeignKey("dbo.RIC_Employee", "RE_DID", "dbo.RMS_Designation", "RD_ID");
        }
    }
}
