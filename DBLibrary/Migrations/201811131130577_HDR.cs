namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HDR : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_SharedReq_HDR", "RS_RMSJobStatus", c => c.String(maxLength: 10));
            AlterColumn("dbo.RMS_SharedReq_HDR", "RS_JobDivaStatus", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_SharedReq_HDR", "RS_JobDivaStatus", c => c.String(maxLength: 5));
            AlterColumn("dbo.RMS_SharedReq_HDR", "RS_RMSJobStatus", c => c.String(maxLength: 5));
        }
    }
}
