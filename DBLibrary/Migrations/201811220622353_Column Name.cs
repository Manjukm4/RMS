namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ColumnName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_AssignSharedReq", "RA_AssignedBy", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_AssignSharedReq", "RA_AssignedDt", c => c.DateTime(nullable: false));
            DropColumn("dbo.RMS_AssignSharedReq", "RA_CreatedBy");
            DropColumn("dbo.RMS_AssignSharedReq", "RA_CreatedDt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RMS_AssignSharedReq", "RA_CreatedDt", c => c.DateTime(nullable: false));
            AddColumn("dbo.RMS_AssignSharedReq", "RA_CreatedBy", c => c.String(maxLength: 50));
            DropColumn("dbo.RMS_AssignSharedReq", "RA_AssignedDt");
            DropColumn("dbo.RMS_AssignSharedReq", "RA_AssignedBy");
        }
    }
}
