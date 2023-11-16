namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_uniqueKye_submissions : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.RIC_SubmissionDaily", new[] { "RS_Emp_Cd", "RS_Date" }, unique: true, name: "IX_RC_SubmissionComposite");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RIC_SubmissionDaily", "IX_RC_SubmissionComposite");
        }
    }
}
