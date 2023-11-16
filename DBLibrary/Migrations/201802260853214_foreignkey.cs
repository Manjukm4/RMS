namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignkey : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.RIC_Employee", "RE_Mgr_ID");
            AddForeignKey("dbo.RIC_Employee", "RE_Mgr_ID", "dbo.RIC_Employee", "RE_EmpId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_Employee", "RE_Mgr_ID", "dbo.RIC_Employee");
            DropIndex("dbo.RIC_Employee", new[] { "RE_Mgr_ID" });
        }
    }
}
