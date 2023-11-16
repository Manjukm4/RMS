namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReqNotes", "RS_RefHDRID", c => c.Int(nullable: false));
            AddColumn("dbo.RMS_SharedReqNotes", "RS_CreatedBy", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_SharedReqNotes", "RS_CreatedDt", c => c.DateTime(nullable: false));
            CreateIndex("dbo.RMS_SharedReqNotes", "RS_RefHDRID");
            AddForeignKey("dbo.RMS_SharedReqNotes", "RS_RefHDRID", "dbo.RMS_SharedReq_HDR", "RS_ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RMS_SharedReqNotes", "RS_RefHDRID", "dbo.RMS_SharedReq_HDR");
            DropIndex("dbo.RMS_SharedReqNotes", new[] { "RS_RefHDRID" });
            DropColumn("dbo.RMS_SharedReqNotes", "RS_CreatedDt");
            DropColumn("dbo.RMS_SharedReqNotes", "RS_CreatedBy");
            DropColumn("dbo.RMS_SharedReqNotes", "RS_RefHDRID");
        }
    }
}
