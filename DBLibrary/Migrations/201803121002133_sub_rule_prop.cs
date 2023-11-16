namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sub_rule_prop : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_SubmissionRule", "RS_Requirement", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_SubmissionRule", "RS_Requirement", c => c.String());
        }
    }
}
