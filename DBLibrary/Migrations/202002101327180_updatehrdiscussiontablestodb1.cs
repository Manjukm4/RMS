namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatehrdiscussiontablestodb1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_DiscussioNotes", c => c.String());
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_ActionItems", c => c.String());
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_InternalCommnets", c => c.String());
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_AgendaType", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_AgendaType", c => c.String(nullable: false));
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_InternalCommnets", c => c.String(nullable: false));
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_ActionItems", c => c.String(nullable: false));
            AlterColumn("dbo.RIC_HRDiscussion_Dtl", "RHD_DiscussioNotes", c => c.String(nullable: false));
        }
    }
}
