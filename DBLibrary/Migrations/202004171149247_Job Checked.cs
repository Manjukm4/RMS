namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobChecked : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_HDR", "RS_UpdatedDt", c => c.DateTime(nullable: false));
            AddColumn("dbo.RMS_SharedReq_HDR", "RS_UpdatedBy", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMS_SharedReq_HDR", "RS_UpdatedBy");
            DropColumn("dbo.RMS_SharedReq_HDR", "RS_UpdatedDt");
        }
    }
}
