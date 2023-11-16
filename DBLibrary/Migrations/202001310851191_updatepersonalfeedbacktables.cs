namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatepersonalfeedbacktables : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_PersonalFeedback", "RP_SubmissionTarget", c => c.Int());
            AlterColumn("dbo.RIC_PersonalFeedback", "RP_InterviewTarget", c => c.Int());
            AlterColumn("dbo.RIC_PersonalFeedback", "RP_HiresTarget", c => c.Int());
            AlterColumn("dbo.RIC_PersonalFeedback", "RP_CallsTarget", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_PersonalFeedback", "RP_CallsTarget", c => c.Int(nullable: false));
            AlterColumn("dbo.RIC_PersonalFeedback", "RP_HiresTarget", c => c.Int(nullable: false));
            AlterColumn("dbo.RIC_PersonalFeedback", "RP_InterviewTarget", c => c.Int(nullable: false));
            AlterColumn("dbo.RIC_PersonalFeedback", "RP_SubmissionTarget", c => c.Int(nullable: false));
        }
    }
}
