namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Filedatatodtl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_Dtl", "RS_FileName", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_SharedReq_Dtl", "RS_FileType", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_SharedReq_Dtl", "RS_FileData", c => c.Binary());
            DropColumn("dbo.RMS_SharedReq_HDR", "FileName");
            DropColumn("dbo.RMS_SharedReq_HDR", "FileType");
            DropColumn("dbo.RMS_SharedReq_HDR", "FileData");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RMS_SharedReq_HDR", "FileData", c => c.Binary());
            AddColumn("dbo.RMS_SharedReq_HDR", "FileType", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_SharedReq_HDR", "FileName", c => c.String(maxLength: 50));
            DropColumn("dbo.RMS_SharedReq_Dtl", "RS_FileData");
            DropColumn("dbo.RMS_SharedReq_Dtl", "RS_FileType");
            DropColumn("dbo.RMS_SharedReq_Dtl", "RS_FileName");
        }
    }
}
