namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RIC_Job_Report_Prop : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Job_Report", "RJ_Prim_Recruiter", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Job_Report", "RJ_Prim_Recruiter");
        }
    }
}
