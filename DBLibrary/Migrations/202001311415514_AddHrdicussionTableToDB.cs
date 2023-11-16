namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHrdicussionTableToDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_HRDiscussion_Dtl",
                c => new
                    {
                        RHD_ID = c.Int(nullable: false, identity: true),
                        RHD_RefID = c.Int(nullable: false),
                        RHD_Description = c.String(),
                        RHD_DiscussioNotes = c.String(),
                        RHD_ActionItems = c.String(),
                        RHD_NextMeetUpDate = c.DateTime(),
                        RHD_InternalCommnets = c.String(),
                        RHD_AdditionalComments = c.String(),
                    })
                .PrimaryKey(t => t.RHD_ID)
                .ForeignKey("dbo.RIC_HRDiscussion_Hdr", t => t.RHD_RefID, cascadeDelete: true)
                .Index(t => t.RHD_RefID);
            
            CreateTable(
                "dbo.RIC_HRDiscussion_Hdr",
                c => new
                    {
                        RH_ID = c.Int(nullable: false, identity: true),
                        RH_EmployeeID = c.String(maxLength: 50),
                        RH_ReviewerID = c.String(maxLength: 50),
                        RH_ReviewDate = c.DateTime(nullable: false),
                        RH_AgendaType = c.String(),
                        RH_Status = c.String(maxLength: 50),
                        RH_CreatedBy = c.String(),
                        RH_CreatedDate = c.DateTime(nullable: false),
                        RH_UpdatedBy = c.String(),
                        RH_UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RH_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_HRDiscussion_Dtl", "RHD_RefID", "dbo.RIC_HRDiscussion_Hdr");
            DropIndex("dbo.RIC_HRDiscussion_Dtl", new[] { "RHD_RefID" });
            DropTable("dbo.RIC_HRDiscussion_Hdr");
            DropTable("dbo.RIC_HRDiscussion_Dtl");
        }
    }
}
