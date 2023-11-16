namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnualFeedbackfieldWeightage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_AnnualFeedbackFields", "AF_Weightage", c => c.Int(nullable: false));
            DropColumn("dbo.RIC_AnnualFeedbackFields", "AF_AF_Weightage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_AnnualFeedbackFields", "AF_AF_Weightage", c => c.Int(nullable: false));
            DropColumn("dbo.RIC_AnnualFeedbackFields", "AF_Weightage");
        }
    }
}
