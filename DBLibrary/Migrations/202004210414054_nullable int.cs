namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullableint : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_MaxSubAllowed", c => c.Int());
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_InternalSub", c => c.Int());
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_ExternalSub", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_ExternalSub", c => c.Int(nullable: false));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_InternalSub", c => c.Int(nullable: false));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_MaxSubAllowed", c => c.Int(nullable: false));
        }
    }
}
