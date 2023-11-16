namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RMS_JobCheckedNotesToDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RMS_JobCheckedNotes",
                c => new
                    {
                        RJ_ID = c.Int(nullable: false, identity: true),
                        RJ_RefHDRID = c.Int(nullable: false),
                        RJ_Notes = c.String(maxLength: 1000),
                        RJ_CreatedBy = c.String(maxLength: 50),
                        RJ_CreatedDt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RJ_ID)
                .ForeignKey("dbo.RMS_SharedReq_HDR", t => t.RJ_RefHDRID, cascadeDelete: true)
                .Index(t => t.RJ_RefHDRID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RMS_JobCheckedNotes", "RJ_RefHDRID", "dbo.RMS_SharedReq_HDR");
            DropIndex("dbo.RMS_JobCheckedNotes", new[] { "RJ_RefHDRID" });
            DropTable("dbo.RMS_JobCheckedNotes");
        }
    }
}
