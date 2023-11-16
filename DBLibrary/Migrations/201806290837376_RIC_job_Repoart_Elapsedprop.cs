namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RIC_job_Repoart_Elapsedprop : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Job_Report", "RJ_Submit_Elapsed", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.RIC_Job_Report", "RJ_Interview_Elapsed", c => c.Time(precision: 7));
            AddColumn("dbo.RIC_Job_Report", "RJ_Hire_Elapsed", c => c.Time(precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Job_Report", "RJ_Hire_Elapsed");
            DropColumn("dbo.RIC_Job_Report", "RJ_Interview_Elapsed");
            DropColumn("dbo.RIC_Job_Report", "RJ_Submit_Elapsed");
        }
    }
}
