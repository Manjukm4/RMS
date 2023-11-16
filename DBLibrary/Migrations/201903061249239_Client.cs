namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Client : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RIC_ClientMapping", "RIC_Employee_RE_EmpId", "dbo.RIC_Employee");
            DropIndex("dbo.RIC_ClientMapping", new[] { "RIC_Employee_RE_EmpId" });
            DropColumn("dbo.RIC_ClientMapping", "RCM_Emp_Cd");
            RenameColumn(table: "dbo.RIC_ClientMapping", name: "RIC_Employee_RE_EmpId", newName: "RCM_Emp_Cd");
            AlterColumn("dbo.RIC_ClientMapping", "RCM_Emp_Cd", c => c.Decimal(nullable: false, precision: 18, scale: 0, storeType: "numeric"));
            AlterColumn("dbo.RIC_ClientMapping", "RCM_Emp_Cd", c => c.Decimal(nullable: false, precision: 18, scale: 0, storeType: "numeric"));
            CreateIndex("dbo.RIC_ClientMapping", "RCM_Emp_Cd");
            AddForeignKey("dbo.RIC_ClientMapping", "RCM_Emp_Cd", "dbo.RIC_Employee", "RE_EmpId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_ClientMapping", "RCM_Emp_Cd", "dbo.RIC_Employee");
            DropIndex("dbo.RIC_ClientMapping", new[] { "RCM_Emp_Cd" });
            AlterColumn("dbo.RIC_ClientMapping", "RCM_Emp_Cd", c => c.Decimal(precision: 18, scale: 0, storeType: "numeric"));
            AlterColumn("dbo.RIC_ClientMapping", "RCM_Emp_Cd", c => c.String());
            RenameColumn(table: "dbo.RIC_ClientMapping", name: "RCM_Emp_Cd", newName: "RIC_Employee_RE_EmpId");
            AddColumn("dbo.RIC_ClientMapping", "RCM_Emp_Cd", c => c.String());
            CreateIndex("dbo.RIC_ClientMapping", "RIC_Employee_RE_EmpId");
            AddForeignKey("dbo.RIC_ClientMapping", "RIC_Employee_RE_EmpId", "dbo.RIC_Employee", "RE_EmpId");
        }
    }
}
