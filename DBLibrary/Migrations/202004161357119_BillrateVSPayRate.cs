namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillrateVSPayRate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_BillRate", c => c.Single());
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_PayRate", c => c.Single());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_PayRate", c => c.Single(nullable: false));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_BillRate", c => c.Single(nullable: false));
        }
    }
}
