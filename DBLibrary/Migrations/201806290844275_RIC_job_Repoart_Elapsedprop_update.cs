namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RIC_job_Repoart_Elapsedprop_update : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_Job_Report", "RJ_Submit_Elapsed", c => c.Time(precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Job_Report", "RJ_Submit_Elapsed", c => c.Time(nullable: false, precision: 7));
        }
    }
}
