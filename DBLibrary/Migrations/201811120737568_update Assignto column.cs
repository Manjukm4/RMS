namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAssigntocolumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_AssignSharedReq", "RA_AssignedTo", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_AssignSharedReq", "RA_AssignedTo", c => c.Int(nullable: false));
        }
    }
}
