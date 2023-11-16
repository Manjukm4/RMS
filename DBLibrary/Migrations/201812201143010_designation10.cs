namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class designation10 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.RIC_Employee", "RE_DesignationID");
            AddForeignKey("dbo.RIC_Employee", "RE_DesignationID", "dbo.RMS_Designation", "RD_ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_Employee", "RE_DesignationID", "dbo.RMS_Designation");
            DropIndex("dbo.RIC_Employee", new[] { "RE_DesignationID" });
        }
    }
}
