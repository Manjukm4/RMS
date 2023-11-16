namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeProperties_ric_employee : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RIC_Employee", "RE_OrgID");
            DropColumn("dbo.RIC_Employee", "RE_DeptID");
            DropColumn("dbo.RIC_Employee", "RE_Start_Date");
            DropColumn("dbo.RIC_Employee", "RE_End_Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_Employee", "RE_End_Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.RIC_Employee", "RE_Start_Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.RIC_Employee", "RE_DeptID", c => c.Decimal(nullable: false, precision: 18, scale: 0, storeType: "numeric"));
            AddColumn("dbo.RIC_Employee", "RE_OrgID", c => c.Decimal(nullable: false, precision: 18, scale: 0, storeType: "numeric"));
        }
    }
}
