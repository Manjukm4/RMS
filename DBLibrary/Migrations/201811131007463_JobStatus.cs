namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobStatus : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_SharedReq_HDR", "RS_JobDivaStatus", c => c.String(maxLength: 5));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_SharedReq_HDR", "RS_JobDivaStatus", c => c.String(maxLength: 1));
        }
    }
}
