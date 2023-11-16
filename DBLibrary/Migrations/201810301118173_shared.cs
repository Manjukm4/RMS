namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shared : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_WorkLocation", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_WorkLocation", c => c.String(maxLength: 10));
        }
    }
}
