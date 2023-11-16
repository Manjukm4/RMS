namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class undochanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RIC_SubmissionRule", "RIC_Employee_RE_EmpId", "dbo.RIC_Employee");
            DropIndex("dbo.RIC_SubmissionRule", new[] { "RIC_Employee_RE_EmpId" });
            DropColumn("dbo.RIC_Employee", "RE_SubRule_ID");
            DropColumn("dbo.RIC_SubmissionRule", "RIC_Employee_RE_EmpId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_SubmissionRule", "RIC_Employee_RE_EmpId", c => c.Decimal(precision: 18, scale: 0, storeType: "numeric"));
            AddColumn("dbo.RIC_Employee", "RE_SubRule_ID", c => c.Decimal(precision: 18, scale: 0, storeType: "numeric"));
            CreateIndex("dbo.RIC_SubmissionRule", "RIC_Employee_RE_EmpId");
            AddForeignKey("dbo.RIC_SubmissionRule", "RIC_Employee_RE_EmpId", "dbo.RIC_Employee", "RE_EmpId");
        }
    }
}
