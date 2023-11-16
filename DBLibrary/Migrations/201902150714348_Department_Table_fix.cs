namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Department_Table_fix : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RIC_Employee", "RE_DepartmentID");
            RenameColumn(table: "dbo.RIC_Employee", name: "RMS_Department_RD_ID", newName: "RE_DepartmentID");
            RenameIndex(table: "dbo.RIC_Employee", name: "IX_RMS_Department_RD_ID", newName: "IX_RE_DepartmentID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.RIC_Employee", name: "IX_RE_DepartmentID", newName: "IX_RMS_Department_RD_ID");
            RenameColumn(table: "dbo.RIC_Employee", name: "RE_DepartmentID", newName: "RMS_Department_RD_ID");
            AddColumn("dbo.RIC_Employee", "RE_DepartmentID", c => c.Int());
        }
    }
}
