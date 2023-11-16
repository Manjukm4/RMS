namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Namefix : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.JobChecked_Info", newName: "RMS_JobChecked_Info");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.RMS_JobChecked_Info", newName: "JobChecked_Info");
        }
    }
}
