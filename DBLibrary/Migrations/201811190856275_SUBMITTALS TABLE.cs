namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SUBMITTALSTABLE : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RMS_SharedReqSubmittals",
                c => new
                    {
                        RS_ID = c.Int(nullable: false, identity: true),
                        RS_RefID = c.Int(nullable: false),
                        RS_CandidateName = c.String(),
                        RS_CandidateEmail = c.String(),
                        RS_EnteredName = c.String(),
                        RS_EntredDate = c.DateTime(nullable: false),
                        RS_SubmittedBy = c.String(),
                        RS_SubmittedDate = c.DateTime(nullable: false),
                        RS_CreatedBy = c.String(maxLength: 50),
                        RS_CreatedDt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RS_ID)
                .ForeignKey("dbo.RMS_AssignSharedReq", t => t.RS_RefID, cascadeDelete: true)
                .Index(t => t.RS_RefID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RMS_SharedReqSubmittals", "RS_RefID", "dbo.RMS_AssignSharedReq");
            DropIndex("dbo.RMS_SharedReqSubmittals", new[] { "RS_RefID" });
            DropTable("dbo.RMS_SharedReqSubmittals");
        }
    }
}
