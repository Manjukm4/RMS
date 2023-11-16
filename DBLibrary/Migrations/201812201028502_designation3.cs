namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class designation3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "RMS_Designation_RD_ID", c => c.Int());
            CreateIndex("dbo.RIC_Employee", "RMS_Designation_RD_ID");
            AddForeignKey("dbo.RIC_Employee", "RMS_Designation_RD_ID", "dbo.RMS_Designation", "RD_ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_Employee", "RMS_Designation_RD_ID", "dbo.RMS_Designation");
            DropIndex("dbo.RIC_Employee", new[] { "RMS_Designation_RD_ID" });
            DropColumn("dbo.RIC_Employee", "RMS_Designation_RD_ID");
        }
    }
}
