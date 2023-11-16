namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateHrdiscussiontables : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_DiscussioNotes", c => c.String(nullable: false));
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_ActionItems", c => c.String(nullable: false));
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_InternalCommnets", c => c.String(nullable: false));
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_EmployeeID", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_ReviewerID", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_AgendaType", c => c.String(nullable: false));
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_Status", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_Status", c => c.String(maxLength: 50));
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_AgendaType", c => c.String());
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_ReviewerID", c => c.String(maxLength: 50));
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_EmployeeID", c => c.String(maxLength: 50));
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_InternalCommnets", c => c.String());
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_ActionItems", c => c.String());
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_DiscussioNotes", c => c.String());
        }
    }
}
