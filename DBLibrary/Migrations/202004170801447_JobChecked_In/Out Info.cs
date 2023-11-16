namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobChecked_InOutInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JobChecked_Info",
                c => new
                    {
                        RC_ID = c.Int(nullable: false, identity: true),
                        RC_RefHDRID = c.Int(nullable: false),
                        CheckedTo = c.String(maxLength: 10),
                        RS_CheckedInDt = c.DateTime(nullable: false),
                        RS_CheckedOutDt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RC_ID)
                .ForeignKey("dbo.RMS_SharedReq_HDR", t => t.RC_RefHDRID, cascadeDelete: true)
                .Index(t => t.RC_RefHDRID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobChecked_Info", "RC_RefHDRID", "dbo.RMS_SharedReq_HDR");
            DropIndex("dbo.JobChecked_Info", new[] { "RC_RefHDRID" });
            DropTable("dbo.JobChecked_Info");
        }
    }
}
