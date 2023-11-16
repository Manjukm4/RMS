namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssignedBy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_JobChecked_Info", "RJ_AssignedBy", c => c.String(maxLength: 10));
            AddColumn("dbo.RMS_JobChecked_Info", "RJ_AssignedDt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMS_JobChecked_Info", "RJ_AssignedDt");
            DropColumn("dbo.RMS_JobChecked_Info", "RJ_AssignedBy");
        }
    }
}
