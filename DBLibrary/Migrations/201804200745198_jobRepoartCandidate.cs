namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobRepoartCandidate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_Job_Report", "RJ_Candidate", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Job_Report", "RJ_Candidate", c => c.String(maxLength: 100));
        }
    }
}
