namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class designation8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RIC_Employee", "RE_DesignationID", "dbo.RMS_Designation");
            DropIndex("dbo.RIC_Employee", new[] { "RE_DesignationID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.RIC_Employee", "RE_DesignationID");
            AddForeignKey("dbo.RIC_Employee", "RE_DesignationID", "dbo.RMS_Designation", "RD_ID", cascadeDelete: true);
        }
    }
}
