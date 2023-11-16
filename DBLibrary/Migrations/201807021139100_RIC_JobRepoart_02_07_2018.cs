namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RIC_JobRepoart_02_07_2018 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_Job_Report", "RJ_Submit_Elapsed", c => c.String(maxLength: 100));
            AlterColumn("dbo.RIC_Job_Report", "RJ_Interview_Elapsed", c => c.String(maxLength: 100));
            AlterColumn("dbo.RIC_Job_Report", "RJ_Hire_Elapsed", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Job_Report", "RJ_Hire_Elapsed", c => c.Time(precision: 7));
            AlterColumn("dbo.RIC_Job_Report", "RJ_Interview_Elapsed", c => c.Time(precision: 7));
            AlterColumn("dbo.RIC_Job_Report", "RJ_Submit_Elapsed", c => c.Time(precision: 7));
        }
    }
}
