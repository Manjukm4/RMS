namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RIC_JobReport_UpdateCandidateProp : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_Job_Report", "RJ_Users", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Job_Report", "RJ_Users", c => c.String(maxLength: 100));
        }
    }
}
