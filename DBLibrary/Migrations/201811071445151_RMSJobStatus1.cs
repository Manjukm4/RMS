namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RMSJobStatus1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_HDR", "RS_RMSJobStatus", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMS_SharedReq_HDR", "RS_RMSJobStatus");
        }
    }
}
