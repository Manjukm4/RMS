namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssignReq : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_Dtl", "RS_Instructions", c => c.String(maxLength: 1000));
            AddColumn("dbo.RMS_SharedReq_HDR", "RS_CreatedBy", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_SharedReq_HDR", "RS_Req", c => c.String(maxLength: 100));
            DropColumn("dbo.RMS_SharedReq_HDR", "Req");
            DropColumn("dbo.RMS_SharedReq_HDR", "Instructions");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RMS_SharedReq_HDR", "Instructions", c => c.String(maxLength: 1000));
            AddColumn("dbo.RMS_SharedReq_HDR", "Req", c => c.String(maxLength: 100));
            DropColumn("dbo.RMS_SharedReq_HDR", "RS_Req");
            DropColumn("dbo.RMS_SharedReq_HDR", "RS_CreatedBy");
            DropColumn("dbo.RMS_SharedReq_Dtl", "RS_Instructions");
        }
    }
}
