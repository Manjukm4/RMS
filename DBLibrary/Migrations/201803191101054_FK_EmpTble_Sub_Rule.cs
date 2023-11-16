namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FK_EmpTble_Sub_Rule : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.RIC_Employee", "RE_Sub_Rule_ID");
            AddForeignKey("dbo.RIC_Employee", "RE_Sub_Rule_ID", "dbo.RIC_SubmissionRule", "RS_Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_Employee", "RE_Sub_Rule_ID", "dbo.RIC_SubmissionRule");
            DropIndex("dbo.RIC_Employee", new[] { "RE_Sub_Rule_ID" });
        }
    }
}
