namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bugfixJobChecked : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_SharedReq_HDR", "RS_UpdatedDt", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_SharedReq_HDR", "RS_UpdatedDt", c => c.DateTime(nullable: false));
        }
    }
}
