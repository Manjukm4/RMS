namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SharedReqFileuPLOAD : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_HDR", "RS_JobDivaStatus", c => c.String(maxLength: 1));
            AddColumn("dbo.RMS_SharedReq_HDR", "ContentType", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_SharedReq_HDR", "FileName", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_SharedReq_HDR", "FileData", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMS_SharedReq_HDR", "FileData");
            DropColumn("dbo.RMS_SharedReq_HDR", "FileName");
            DropColumn("dbo.RMS_SharedReq_HDR", "ContentType");
            DropColumn("dbo.RMS_SharedReq_HDR", "RS_JobDivaStatus");
        }
    }
}
