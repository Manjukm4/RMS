namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ric_Job_Repart_UpdatedProp : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_Job_Report", "RJ_Openings", c => c.Int());
            AlterColumn("dbo.RIC_Job_Report", "RJ_Fills", c => c.Int());
            AlterColumn("dbo.RIC_Job_Report", "RJ_Max_Submittals", c => c.Int());
            AlterColumn("dbo.RIC_Job_Report", "RJ_Prim_Sales", c => c.String(maxLength: 500));
            AlterColumn("dbo.RIC_Job_Report", "RJ_Prim_Recruiter", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Job_Report", "RJ_Prim_Recruiter", c => c.String(maxLength: 100));
            AlterColumn("dbo.RIC_Job_Report", "RJ_Prim_Sales", c => c.String(maxLength: 100));
            AlterColumn("dbo.RIC_Job_Report", "RJ_Max_Submittals", c => c.Int(nullable: false));
            AlterColumn("dbo.RIC_Job_Report", "RJ_Fills", c => c.Int(nullable: false));
            AlterColumn("dbo.RIC_Job_Report", "RJ_Openings", c => c.Int(nullable: false));
        }
    }
}
