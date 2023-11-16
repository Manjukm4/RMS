namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobIDupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_HDR", "RS_JobID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMS_SharedReq_HDR", "RS_JobID");
        }
    }
}
