namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assignsharedrequpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_AssignSharedReq", "RA_CreatedDt", c => c.DateTime(nullable: false));
            DropColumn("dbo.RMS_AssignSharedReq", "CreatedDt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RMS_AssignSharedReq", "CreatedDt", c => c.DateTime(nullable: false));
            DropColumn("dbo.RMS_AssignSharedReq", "RA_CreatedDt");
        }
    }
}
