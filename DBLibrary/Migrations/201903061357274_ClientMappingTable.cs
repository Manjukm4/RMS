namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientMappingTable : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.RIC_ClientMapping", name: "RCM_Emp_Cd", newName: "RCM_EmpId");
            RenameIndex(table: "dbo.RIC_ClientMapping", name: "IX_RCM_Emp_Cd", newName: "IX_RCM_EmpId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.RIC_ClientMapping", name: "IX_RCM_EmpId", newName: "IX_RCM_Emp_Cd");
            RenameColumn(table: "dbo.RIC_ClientMapping", name: "RCM_EmpId", newName: "RCM_Emp_Cd");
        }
    }
}
