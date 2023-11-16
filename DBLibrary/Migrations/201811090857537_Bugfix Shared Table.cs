namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BugfixSharedTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_HDR", "FileType", c => c.String(maxLength: 50));
            DropColumn("dbo.RMS_SharedReq_HDR", "ContentType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RMS_SharedReq_HDR", "ContentType", c => c.String(maxLength: 50));
            DropColumn("dbo.RMS_SharedReq_HDR", "FileType");
        }
    }
}
