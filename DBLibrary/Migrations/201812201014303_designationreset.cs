namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class designationreset : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RMS_Designation", "RIC_Employee_RE_EmpId", "dbo.RIC_Employee");
            DropIndex("dbo.RMS_Designation", new[] { "RIC_Employee_RE_EmpId" });
            DropColumn("dbo.RIC_Employee", "RE_DesignationID");
            DropTable("dbo.RMS_Designation");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RMS_Designation",
                c => new
                    {
                        RD_ID = c.Int(nullable: false, identity: true),
                        RD_Designation = c.String(),
                        RIC_Employee_RE_EmpId = c.Decimal(precision: 18, scale: 0, storeType: "numeric"),
                    })
                .PrimaryKey(t => t.RD_ID);
            
            AddColumn("dbo.RIC_Employee", "RE_DesignationID", c => c.Int(nullable: false));
            CreateIndex("dbo.RMS_Designation", "RIC_Employee_RE_EmpId");
            AddForeignKey("dbo.RMS_Designation", "RIC_Employee_RE_EmpId", "dbo.RIC_Employee", "RE_EmpId");
        }
    }
}
