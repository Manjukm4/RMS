namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_CompositKeyJob_Repoart : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RIC_Job_Report", "IX_RIC_Job_Report");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.RIC_Job_Report", new[] { "RJ_EmpCd", "RJ_JobDiva_Ref", "RJ_Optional_Ref" }, unique: true, name: "IX_RIC_Job_Report");
        }
    }
}
