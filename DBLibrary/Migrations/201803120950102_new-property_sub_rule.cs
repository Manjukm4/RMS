namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newproperty_sub_rule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_SubmissionRule", "RS_Monthyl_Hire", c => c.Int(nullable: false));
            AddColumn("dbo.RIC_SubmissionRule", "RS_Minimum_Hire", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_SubmissionRule", "RS_Minimum_Hire");
            DropColumn("dbo.RIC_SubmissionRule", "RS_Monthyl_Hire");
        }
    }
}
